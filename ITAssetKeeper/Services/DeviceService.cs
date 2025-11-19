using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace ITAssetKeeper.Services;

public class DeviceService : IDeviceService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly IDeviceHistoryService _deviceHistoryService;

    public DeviceService(ITAssetKeeperDbContext context, IDeviceHistoryService deviceHistoryService)
    {
        _context = context;
        _deviceHistoryService = deviceHistoryService;
    }

    ///////////////////////////////////////////////////
    // -- Index --
    // 検索 & 一覧表示
    public async Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition)
    {
        // Deviceテーブルから全てのデータを取得する
        IQueryable<Device> query = _context.Devices;

        // フィルタリング実施
        query = FilterDevices(query, condition); 

        // 並び替え
        query = SortDevices(query, condition.SortKeyValue, condition.SortOrderValue);

        // ページング前の全件数を取得
        condition.TotalCount = query.Count();

        // ページング
        query = PagingDevices(query, condition.PageNumber, condition.PageSize);

        // SQLを実行
        var devices = await query.ToListAsync();

        // ビューモデルに詰めて、呼び出し元に返す
        return ToViewModel(condition, devices);
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
    private DeviceListViewModel ToViewModel(DeviceListViewModel condition, List<Device> devices)
    {
        // プルダウン用のデータを定数から取得
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList => condition.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(condition, selectList => condition.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceColumns>(condition, selectList => condition.SortKeyList = selectList);

        // DeviceStatus は Deleted を除外して取得する
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(condition, selectList => 
            condition.StatusItems = new SelectList(EnumDisplayHelper.ToDictionary(DeviceStatus.Deleted),"Key", "Value"));

        // 検索結果の一覧表示のデータをDTO型で詰める
        condition.Devices = devices
            .Select(x => new DeviceDto
            {
                Id = x.Id,
                ManagementId = x.ManagementId,
                Category = x.Category,
                Purpose = x.Purpose,
                ModelNumber = x.ModelNumber,
                SerialNumber = x.SerialNumber,
                HostName = x.HostName,
                Location = x.Location,
                UserName = x.UserName,
                Status = x.Status,
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

        // DeviceStatus は Deleted を除外して取得する
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList =>
            model.StatusItems = new SelectList(EnumDisplayHelper.ToDictionary(DeviceStatus.Deleted), "Key", "Value"));
        
        // 購入日に今日の日付をセット
        model.PurchaseDate = DateTime.Now;

        return model;
    }

    // 機器情報の新規登録処理
    public async Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model, string userName)
    {
        // トランザクション処理
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // 受け取ったビューモデルからEntity 作成
            var entity = new Device
            {
                ManagementId = GenerateManagementId(),
                Category = model.SelectedCategory,
                Purpose = model.SelectedPurpose,
                ModelNumber = model.ModelNumber,
                SerialNumber = model.SerialNumber,
                HostName = model.HostName,
                Location = model.Location,
                UserName = model.UserName,
                Status = model.SelectedStatus,
                Memo = model.Memo,
                PurchaseDate = model.PurchaseDate
            };

            // Device を追加
            _context.Devices.Add(entity);

            // DeviceのDBへの登録処理を実施
            var result =  await _context.SaveChangesAsync();

            // 新規登録に関する履歴レコードを追加
            await _deviceHistoryService.AddCreateHistoryAsync(entity, userName);

            // Commitする
            scope.Complete();

            // 結果の状態エントリの数を返す
            return result;
        }
    }

    // ManagementID を生成して返す
    private string GenerateManagementId()
    {
        // ManagementIDを自動採番する為に、DBに存在するManagementIdを取得
        // 削除フラグがついているものも取得したいのでクエリフィルタを無効化
        var idList = _context.Devices
            .IgnoreQueryFilters()
            .Select(x => x.ManagementId);

        // プレフィックスを除いた数字部分をint型で取得しなおす
        // 一番大きい数字を取得し、新しいManagementId用に +1 する
        var maxNum = idList
            .AsEnumerable()
            .Select(id => ExtractNumericId(id))
            .DefaultIfEmpty(0)
            .Max() + 1;

        // 新しい ManagementId を生成
        // プレフィックスを付けて、数字部分が規定の桁数になるように先行0埋めする
        return DeviceConstants.DEVICE_ID_PREFIX + maxNum.ToString($"D{DeviceConstants.DEVICE_ID_NUM_DIGIT_COUNT}");
    }

    // ManagementIDから数字部分を取り出す
    private int ExtractNumericId(string managementId)
    {
        return int.Parse(managementId.Substring(
                DeviceConstants.DEVICE_ID_PREFIX.Length,
                DeviceConstants.DEVICE_ID_NUM_DIGIT_COUNT));
    }


    //////////////////////////////////////////
    // --- Details ---

    // Devices の Id を取得し、該当の機器情報をDTO型で返す
    public async Task<DeviceDto?> GetDeviceDetailsByIdAsync(int id)
    {
        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices.FindAsync(id);

        // 見つからなかった場合は null を返す
        if (device == null)
        {
            return null;
        }

        // 取得したデータをDTOの項目に詰める
        var dto = new DeviceDto
        {
            Id = device.Id,
            ManagementId = device.ManagementId,
            Category = device.Category,
            Purpose = device.Purpose,
            ModelNumber = device.ModelNumber,
            SerialNumber = device.SerialNumber,
            HostName = device.HostName,
            Location = device.Location,
            UserName = device.UserName,
            Status = device.Status,
            PurchaseDate = device.PurchaseDate,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt,
            Memo = device.Memo
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

        // DeviceStatus は Deleted を除外して取得する
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList =>
            model.StatusItems = new SelectList(EnumDisplayHelper.ToDictionary(DeviceStatus.Deleted), "Key", "Value"));

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

        // DeviceStatus は Deleted を除外して取得する
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList =>
            model.StatusItems = new SelectList(EnumDisplayHelper.ToDictionary(DeviceStatus.Deleted), "Key", "Value"));

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
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
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
                entity.PurchaseDate = model.PurchaseDate;
            }
            else
            {
                entity.Location = model.Location;
                entity.UserName = model.UserName;
            }

            // DBへの登録処理を実施、状態エントリの数を受け取る
            var result = await _context.SaveChangesAsync();

            // 更新に関する履歴レコードを追加
            await _deviceHistoryService.AddUpdateHistoryAsync(before, entity, userName);

            // Commitする
            scope.Complete();

            // 結果の状態エントリの数を返す
            return result;
        }
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
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // 削除フラグ と 削除実施者を更新する
            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;

            // DBへの更新処理を実施、状態エントリの数を受け取る
            var result = await _context.SaveChangesAsync();

            // 削除に関する履歴レコードを追加
            await _deviceHistoryService.AddDeleteHistoryAsync(before, deletedBy);

            // Commitする
            scope.Complete();

            // 結果の状態エントリの数を返す
            return result;
        }
    }
}
