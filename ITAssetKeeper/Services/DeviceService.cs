using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Formats.Asn1.AsnWriter;

namespace ITAssetKeeper.Services;

public class DeviceService : IDeviceService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly IDeviceHistoryService _deviceHistoryService;
    private readonly IDeviceSequenceService _deviceSequenceService;
    private readonly IUserRoleService _userRoleService;

    public DeviceService(
        ITAssetKeeperDbContext context,
        IDeviceHistoryService deviceHistoryService,
        IDeviceSequenceService deviceSequenceService,
        IUserRoleService userRoleService)
    {
        _context = context;
        _deviceHistoryService = deviceHistoryService;
        _deviceSequenceService = deviceSequenceService;
        _userRoleService = userRoleService;
    }

    ///////////////////////////////////////////////////
    // -- Index --
    // 検索 & 一覧表示
    public async Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition, ClaimsPrincipal user)
    {
        // Deviceテーブルから全てのデータを取得
        IQueryable<Device> query = _context.Devices;

        // ユーザーのRoleを取得
        var role = await _userRoleService.GetUserRoleAsync(user);

        // Roleに応じてDeviceテーブルから取得する内容を変更
        // Admin:すべて表示
        // それ以外:インフラに関わる機器は除外して表示
        if (role != Roles.Admin)
        {
            // Deviceテーブルからインフラに関わる機器は除外して取得
            var hideCategoryList = DeviceConstants.HIDE_CATEGORIES.Select(c => c.ToString()).ToList();
            query = query.Where(x => !hideCategoryList.Contains(x.Category));
        }

        if (!string.IsNullOrWhiteSpace(condition.FreeText))
        {
            // フリーワードに入力があれば、フリーワード検索
            query = FilterDevicesByFreeText(query, condition);
        }
        else
        {
            // なければ、詳細検索
            query = FilterDevices(query, condition);
        }

        // 並び替え
        query = SortDevices(query, condition.SortKeyValue, condition.SortOrderValue);

        // ページング前の全件数を取得
        condition.TotalCount = query.Count();

        // ページング
        query = PagingDevices(query, condition.PageNumber, condition.PageSize);

        // SQLを実行
        var devices = await query.ToListAsync();

        // ビューモデルに詰めて、呼び出し元に返す
        return ToViewModel(condition, devices, role);
    }

    // フリーワード検索用フィルタリング
    private IQueryable<Device> FilterDevicesByFreeText(IQueryable<Device> query, DeviceListViewModel condition)
    {
        var free = condition.FreeText;

        // フリーワードが日付として解釈できるかチェック
        DateTime dt;
        bool isDate = DateTime.TryParse(free, out dt);

        // フリーワードが含まれる表示名に対応する生値を取得
        var categoryValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<DeviceCategory>(free);
        var purposeValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<DevicePurpose>(free);
        var statusValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<DeviceStatus>(free);

        // すべての項目から部分一致で検索
        query = query.Where(x =>
            x.ManagementId.Contains(free)
            || x.ModelNumber.Contains(free)
            || x.SerialNumber.Contains(free)
            || x.HostName != null && x.HostName.Contains(free)
            || x.Location.Contains(free)
            || x.UserName != null && x.UserName.Contains(free)
            || x.Memo != null && x.Memo.Contains(free)
            || categoryValues.Contains(x.Category)
            || purposeValues.Contains(x.Purpose)
            || statusValues.Contains(x.Status)
            || (isDate && x.PurchaseDate.Date == dt.Date)
            || (isDate && x.UpdatedAt.Date == dt.Date)
            );

        return query;
    }

    // フィルタリング (条件に応じて IQueryable<Device> を返す)
    private IQueryable<Device> FilterDevices(IQueryable<Device> query, DeviceListViewModel condition)
    {
        // 部分一致
        if (!string.IsNullOrWhiteSpace(condition.ManagementId))
        {
            query = query.Where(x => x.ManagementId.Contains(condition.ManagementId));
        }

        if (!string.IsNullOrWhiteSpace(condition.ModelNumber))
        {
            query = query.Where(x => x.ModelNumber.Contains(condition.ModelNumber));
        }

        if (!string.IsNullOrWhiteSpace(condition.SerialNumber))
        {
            query = query.Where(x => x.SerialNumber.Contains(condition.SerialNumber));
        }

        if (!string.IsNullOrWhiteSpace(condition.HostName))
        {
            // 実データ側のnullチェックをいれる
            query = query.Where(x => x.HostName != null && x.HostName.Contains(condition.HostName));
        }

        if (!string.IsNullOrWhiteSpace(condition.Location))
        {
            query = query.Where(x => x.Location.Contains(condition.Location));
        }

        if (!string.IsNullOrWhiteSpace(condition.UserName))
        {
            // 実データ側のnullチェックをいれる
            query = query.Where(x => x.UserName != null && x.UserName.Contains(condition.UserName));
        }

        // 完全一致
        if (!string.IsNullOrWhiteSpace(condition.SelectedCategory))
        {
            query = query.Where(x => x.Category == condition.SelectedCategory);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedPurpose))
        {
            query = query.Where(x => x.Purpose == condition.SelectedPurpose);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedStatus))
        {
            query = query.Where(x => x.Status == condition.SelectedStatus);
        }

        // 日付範囲：購入日
        if (condition.PurchaseDateFrom != null || condition.PurchaseDateTo != null)
        {
            // 検索日付の丸め（Date にし、時刻 00:00:00 起点にする）
            var from = condition.PurchaseDateFrom?.Date;
            var to = condition.PurchaseDateTo?.Date;

            // From のみ指定されている場合
            if (from != null && to == null)
            {
                // from ～ 今日の翌日 00:00 まで
                query = query.Where(x =>
                    x.PurchaseDate >= from.Value &&
                    x.PurchaseDate < DateTime.Today.AddDays(1));
            }
            // To のみ指定されている場合
            else if (from == null && to != null)
            {
                // ～ to の翌日 00:00 まで
                query = query.Where(x =>
                    x.PurchaseDate < to.Value.AddDays(1));
            }
            // From と To 両方指定されている場合
            else if (from != null && to != null)
            {
                if (from == to)
                {
                    // 日付一致 → 当日の 00:00 ～ 翌日 00:00
                    query = query.Where(x =>
                        x.PurchaseDate >= from.Value &&
                        x.PurchaseDate < from.Value.AddDays(1));
                }
                else
                {
                    // from ～ to(包含) → 翌日 00:00 まで
                    query = query.Where(x =>
                        x.PurchaseDate >= from.Value &&
                        x.PurchaseDate < to.Value.AddDays(1));
                }
            }
        }

        // 日付範囲：更新日
        if (condition.UpdatedDateFrom != null || condition.UpdatedDateTo != null)
        {
            // 検索日付の丸め（Date にし、時刻 00:00:00 起点にする）
            var from = condition.UpdatedDateFrom?.Date;
            var to = condition.UpdatedDateTo?.Date;

            // From のみ指定されている場合
            if (from != null && to == null)
            {
                // from ～ 今日の翌日 00:00 まで
                query = query.Where(x =>
                    x.UpdatedAt >= from.Value &&
                    x.UpdatedAt < DateTime.Today.AddDays(1));
            }
            // To のみ指定されている場合
            else if (from == null && to != null)
            {
                // ～ to の翌日 00:00 まで
                query = query.Where(x =>
                    x.UpdatedAt < to.Value.AddDays(1));
            }
            // From と To 両方指定されている場合
            else if (from != null && to != null)
            {
                if (from == to)
                {
                    // 日付一致 → 当日の 00:00 ～ 翌日 00:00
                    query = query.Where(x =>
                        x.UpdatedAt >= from.Value &&
                        x.UpdatedAt < from.Value.AddDays(1));
                }
                else
                {
                    // from ～ to(包含) → 翌日 00:00 まで
                    query = query.Where(x =>
                        x.UpdatedAt >= from.Value &&
                        x.UpdatedAt < to.Value.AddDays(1));
                }
            }
        }

        return query;
    }

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    private IQueryable<Device> SortDevices(IQueryable<Device> query, DeviceColumns sortKey, SortOrders sortOrder)
    {
        // 指定されたSortKeyを基準に、
        // 指定されたSortOrderに応じて、昇順 or 降順でソートする
        if (sortOrder == SortOrders.Asc)
        {
            query = query.OrderBy(DeviceConstants.SORT_KEY_SELECTORS[sortKey]);
        }
        else
        {
            query = query.OrderByDescending(DeviceConstants.SORT_KEY_SELECTORS[sortKey]);
        }

        return query;
    }

    // ページング (Skip/Take の適用)
    private IQueryable<Device> PagingDevices(IQueryable<Device> query, int pageNumber, int pageSize)
    {
        // ページングを適用
        query = query.Skip((pageNumber - 1) * pageSize);
        query = query.Take(pageSize);

        return query;
    }

    // ViewModel 変換(結果を DeviceListViewModel に詰める)
    private DeviceListViewModel ToViewModel(DeviceListViewModel condition, List<Device> devices, Roles? role)
    {
        // プルダウン用のデータを定数から取得
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(condition, selectList => condition.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceColumns>(condition, selectList => condition.SortKeyList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(condition, selectList => condition.StatusItems = selectList);

        // DeviceCategoryのみ、Role別でプルダウン用データを変更
        // Admin：すべて、それ以外：インフラに関わる機器を除外したもの
        if (role == Roles.Admin)
        {
            EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList => condition.CategoryItems = selectList);
        }
        else
        {
            EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList =>
            condition.CategoryItems = new SelectList(
                EnumDisplayHelper.EnumToDictionary(
                    false, (DeviceConstants.HIDE_CATEGORIES)), "Key", "Value"));
        }

        // 検索結果の一覧表示のデータをDTO型で詰める
        condition.Devices = devices
            .Select(x => new DeviceDto
            {
                // Category,Purpose,Statusは Helperを使って日本語表示名に変換
                // HostName,UserNameは、nullなら "-"を表示
                Id = x.Id,
                ManagementId = x.ManagementId,
                Category = EnumDisplayHelper.ResolveDisplayName<DeviceCategory>(x.Category),
                Purpose = EnumDisplayHelper.ResolveDisplayName<DevicePurpose>(x.Purpose),
                ModelNumber = x.ModelNumber,
                SerialNumber = x.SerialNumber,
                HostName = x.HostName ?? "-",
                Location = x.Location,
                UserName = x.UserName ?? "-",
                Status = EnumDisplayHelper.ResolveDisplayName<DeviceStatus>(x.Status),
                PurchaseDate = x.PurchaseDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToList();

        return condition;
    }


    ///////////////////////////////////////////////////
    // -- Create --

    // Create画面のinitialize
    public DeviceCreateViewModel InitializeCreateView(DeviceCreateViewModel model)
    {
        // ビューモデルに、ドロップダウン用のSelectListをセット
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);
        
        // 購入日に今日の日付をセット
        model.PurchaseDate = DateTime.Now;

        return model;
    }

    // 機器情報の新規登録処理
    public async Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model, string userName)
    {
        // トランザクション処理
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // 受け取ったビューモデルからEntity 作成
            var entity = new Device
            {
                ManagementId = await GenerateManagementIdAsync(),
                Category = model.SelectedCategory,
                Purpose = model.SelectedPurpose,
                ModelNumber = model.ModelNumber,
                SerialNumber = model.SerialNumber,
                HostName = model.HostName,
                Location = model.Location,
                UserName = model.UserName,
                Status = model.SelectedStatus,
                Memo = model.Memo,
                PurchaseDate = model.PurchaseDate!.Value
            };

            // Device を追加
            _context.Devices.Add(entity);

            // 新規登録に関する履歴を追加
            await _deviceHistoryService.AddCreateHistoryAsync(entity, userName);

            // DBへの登録処理を実施
            var result = await _context.SaveChangesAsync();

            // Commitする
            await transaction.CommitAsync();

            // 結果の状態エントリの数を返す
            return result;
        });
    }

    // ManagementId を生成して返す
    // 採番テーブルを使って、ManagementId が競合しないようにする
    private async Task<string> GenerateManagementIdAsync()
    {
        var seq = await _deviceSequenceService.GetNextManagementIdAsync();

        return DeviceConstants.DEVICE_ID_PREFIX +
               seq.ToString($"D{DeviceConstants.DEVICE_ID_NUM_DIGIT_COUNT}");
    }

    // ManagementIdの自動採番を履歴テーブル内の最大 ManagementId からの連番になるよう同期
    // ダミーデータ追加時などの整合性の担保
    public async Task SyncDeviceSequenceAsync()
    {
        // 履歴テーブル内の最大 ManagementId を取得
        var maxHistoryId = await _context.Devices
            .Select(h => h.ManagementId)
            .ToListAsync();

        // "DE000001" → 数字部分 "000001" → 1 に変換
        int maxNum = maxHistoryId
            .Select(id => int.Parse(id.Substring(DeviceConstants.DEVICE_ID_PREFIX.Length)))
            .DefaultIfEmpty(0)
            .Max();

        // 採番テーブルを最新の値に同期
        // 最大値をプレースホルダーで渡す
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE DeviceSequences SET LastUsedNumber = @p0 WHERE Id = 1", maxNum);
    }


    //////////////////////////////////////////
    // --- Details ---

    // Devices の Id を取得し、該当の機器情報をDTO型で返す
    public async Task<DeviceDto?> GetDeviceDetailsByIdAsync(int id)
    {
        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        // 見つからなかった場合は null を返す
        if (device == null)
        {
            return null;
        }

        // 取得したデータをDTOの項目に詰める
        var dto = new DeviceDto
        {
            // Category,Purpose,Statusは Helperを使って日本語表示名に変換
            // HostName,UserName,Memoは、nullなら "-"を表示
            Id = device.Id,
            ManagementId = device.ManagementId,
            Category = EnumDisplayHelper.ResolveDisplayName<DeviceCategory>(device.Category),
            Purpose = EnumDisplayHelper.ResolveDisplayName<DevicePurpose>(device.Purpose),
            ModelNumber = device.ModelNumber,
            SerialNumber = device.SerialNumber,
            HostName = device.HostName ?? "-",
            Location = device.Location,
            UserName = device.UserName ?? "-",
            Status = EnumDisplayHelper.ResolveDisplayName<DeviceStatus>(device.Status),
            PurchaseDate = device.PurchaseDate,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt,
            Memo = device.Memo ?? "-"
        };

        // DTO型を返す
        return dto;
    }


    //////////////////////////////////////////
    // --- Edit ---

    // Edit画面のinitialize
    public async Task<DeviceEditViewModel?> InitializeEditView(int id, Roles role)
    {
        // Edit画面に表示する為の Device情報を取得
        // Role別の編集可否項目も設定されたビューモデルを受け取る
        var model = await GetDeviceEditViewAsync(id, role);

        // 取得できなければ null を返す
        if (model == null)
        {
            return null;
        }

        // ビューモデルに、ドロップダウン用のSelectListをセット
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);

        // ビューモデルを返す
        return model;
    }

    // Edit画面に表示する為の Device情報を取得し、ビューモデルを返す
    // Role別の編集可否項目もここで設定する
    private async Task<DeviceEditViewModel?> GetDeviceEditViewAsync(int id, Roles role)
    {
        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices.FindAsync(id);

        // 見つからなかった場合は null を返す
        if (device == null)
        {
            return null;
        }

        // 取得したデータをDeviceEditViewModelに詰める
        var model = new DeviceEditViewModel
        {
            IdHidden = device.Id,
            ManagementId = device.ManagementId,
            SelectedCategory = device.Category,
            SelectedPurpose = device.Purpose,
            ModelNumber = device.ModelNumber,
            SerialNumber = device.SerialNumber,
            HostName = device.HostName,
            Location = device.Location,
            UserName = device.UserName,
            SelectedStatus = device.Status,
            PurchaseDate = device.PurchaseDate,
            Memo = device.Memo
        };

        // Admin
        if (role == Roles.Admin)
        {
            // すべてReadOnlyとしない(すべて編集可)
            model.IsReadOnlyCategory = false;
            model.IsReadOnlyPurpose = false;
            model.IsReadOnlyStatus = false;
            model.IsReadOnlyModelNumber = false;
            model.IsReadOnlySerialNumber = false;
            model.IsReadOnlyHostName = false;
            model.IsReadOnlyLocation = false;
            model.IsReadOnlyUserName = false;
            model.IsReadOnlyMemo = false;
            model.IsReadOnlyPurchaseDate = false;
        }
        // それ以外(Editor)
        else
        {
            // 編集可とする項目のみfalseを指定
            model.IsReadOnlyLocation = false;
            model.IsReadOnlyUserName = false;
        }

        // モデルを返す
        return model;
    }

    // 入力エラー時、SelectList ＆ ReadOnly 制御だけ再設定する
    public async Task RestoreEditViewSettingsAsync(DeviceEditViewModel model, Roles role)
    {
        // ビューモデルに、ドロップダウン用のSelectListをセット
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);

        // ReadOnly制御再設定
        // Admin
        if (role == Roles.Admin)
        {
            // すべてReadOnlyとしない(すべて編集可)
            model.IsReadOnlyCategory = false;
            model.IsReadOnlyPurpose = false;
            model.IsReadOnlyStatus = false;
            model.IsReadOnlyModelNumber = false;
            model.IsReadOnlySerialNumber = false;
            model.IsReadOnlyHostName = false;
            model.IsReadOnlyLocation = false;
            model.IsReadOnlyUserName = false;
            model.IsReadOnlyMemo = false;
            model.IsReadOnlyPurchaseDate = false;
        }
        // それ以外(Editor)
        else
        {
            // 編集可とする項目のみfalseを指定
            model.IsReadOnlyLocation = false;
            model.IsReadOnlyUserName = false;
        }
    }

    // 機器情報の更新処理
    public async Task<int> UpdateDeviceAsync(DeviceEditViewModel model, Roles role, string userName)
    {
        // 履歴作成用に、トラッキングなしで事前のデータを取得しておく
        var before = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == model.IdHidden);

        // 見つからなかった場合は -1 を返す
        if (before == null)
        {
            return -1;
        }

        // Devicesテーブルから指定のIdのデータを取得する
        var entity = await _context.Devices.FindAsync(model.IdHidden);

        // 見つからなかった場合は -1 を返す
        if (entity == null)
        {
            return -1;
        }

        // トランザクション処理
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // Role判定
            // Role が Editor であれば Location と UserName のみ更新
            if (role == Roles.Admin)
            {
                entity.Category = model.SelectedCategory;
                entity.Purpose = model.SelectedPurpose;
                entity.ModelNumber = model.ModelNumber;
                entity.SerialNumber = model.SerialNumber;
                entity.HostName = model.HostName;
                entity.Location = model.Location;
                entity.UserName = model.UserName;
                entity.Status = model.SelectedStatus;
                entity.Memo = model.Memo;
                entity.PurchaseDate = model.PurchaseDate!.Value;
            }
            else
            {
                entity.Location = model.Location;
                entity.UserName = model.UserName;
            }

            // 更新に関する履歴レコードを追加
            await _deviceHistoryService.AddUpdateHistoryAsync(before, entity, userName);

            // DBへの登録処理を実施、状態エントリの数を受け取る
            var result = await _context.SaveChangesAsync();

            // Commitする
            await transaction.CommitAsync();

            // 結果の状態エントリの数を返す
            return result;
        });
    }


    //////////////////////////////////////////
    // --- Delete ---

    // Devices の Id を取得し、該当の機器情報をDeviceDeleteViewModelで返す
    public async Task<DeviceDeleteViewModel?> GetDeleteDeviceByIdAsync(int id)
    {
        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices.FindAsync(id);

        // 見つからない or 削除フラグがすでに true の場合は、null を返す
        if (device == null || device.IsDeleted)
        {
            return null;
        }

        // 取得したデータをビューモデルに詰める
        var model = new DeviceDeleteViewModel
        {
            IdHidden = device.Id,
            ManagementId = device.ManagementId,
            Category = device.Category,
            Status = device.Status,
            UserName = device.UserName,
        };

        // ビューモデルを返す
        return model;
    }

    // 対象の機器情報を論理削除(ソフトデリート)する
    public async Task<int> DeleteDeviceAsync(int id, string deletedBy)
    {
        // 履歴作成用に、トラッキングなしで事前のデータを取得しておく
        var before = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        // 見つからない or 削除フラグがすでに true の場合は、-1 を返す
        if (before == null || before.IsDeleted)
        {
            return -1;
        }

        // Devicesテーブルから指定のIdのデータを取得する
        var entity = await _context.Devices.FindAsync(id);

        // 見つからない or 削除フラグがすでに true の場合は、-1 を返す
        if (entity == null || entity.IsDeleted)
        {
            return -1;
        }

        // トランザクション処理
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            // 削除フラグ と 削除実施者を更新する
            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;

            // 削除に関する履歴レコードを追加
            await _deviceHistoryService.AddDeleteHistoryAsync(before, entity, deletedBy);

            // DBへの更新処理を実施、状態エントリの数を受け取る
            var result = await _context.SaveChangesAsync();

            // Commitする
            await transaction.CommitAsync();

            // 結果の状態エントリの数を返す
            return result;
        });
    }
}
