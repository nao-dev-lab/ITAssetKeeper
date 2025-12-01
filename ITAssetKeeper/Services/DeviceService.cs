using ITAssetKeeper.Constants;
using ITAssetKeeper.Controllers;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public class DeviceService : IDeviceService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly IDeviceHistoryService _deviceHistoryService;
    private readonly IDeviceSequenceService _deviceSequenceService;
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        ITAssetKeeperDbContext context,
        IDeviceHistoryService deviceHistoryService,
        IDeviceSequenceService deviceSequenceService,
        IUserRoleService userRoleService,
        ILogger<DeviceService> logger)
    {
        _context = context;
        _deviceHistoryService = deviceHistoryService;
        _deviceSequenceService = deviceSequenceService;
        _userRoleService = userRoleService;
        _logger = logger;
    }

    ///////////////////////////////////////////////////
    // -- Index --
    // 検索 & 一覧表示
    public async Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition, ClaimsPrincipal user)
    {
        _logger.LogInformation("SearchDevicesAsync 開始");

        // Deviceテーブルから全てのデータを取得
        IQueryable<Device> query = _context.Devices;

        // ユーザーのRoleを取得
        var role = await _userRoleService.GetUserRoleAsync(user);
        _logger.LogInformation("SearchDevicesAsync ユーザーロール取得 Role={Role}", role);

        // Roleに応じてDeviceテーブルから取得する内容を変更
        // Admin:すべて表示
        // それ以外:インフラに関わる機器は除外して表示
        if (role != Roles.Admin)
        {
            // Deviceテーブルからインフラに関わる機器は除外して取得
            var hideCategoryList = DeviceConstants.HIDE_CATEGORIES.Select(c => c.ToString()).ToList();
            query = query.Where(x => !hideCategoryList.Contains(x.Category));
            _logger.LogInformation("SearchDevicesAsync インフラ機器除外処理適用");
        }

        if (!string.IsNullOrWhiteSpace(condition.FreeText))
        {
            _logger.LogInformation("SearchDevicesAsync フリーワード検索適用 FreeText={FreeText}", condition.FreeText);
            
            // フリーワードに入力があれば、フリーワード検索
            query = FilterDevicesByFreeText(query, condition);
        }
        else
        {
            _logger.LogInformation("SearchDevicesAsync 詳細検索適用");

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
        List<Device> devices;
        _logger.LogInformation("SearchDevicesAsync SQL実行開始");
        try
        {
            devices = await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchDevicesAsync SQL実行エラー");
            throw;
        }
        _logger.LogInformation("SearchDevicesAsync SQL実行完了 取得件数={Count}", devices.Count);
        
        // ビューモデルに詰めて、呼び出し元に返す
        return ToViewModel(condition, devices, role);
    }

    // フリーワード検索用フィルタリング
    private IQueryable<Device> FilterDevicesByFreeText(IQueryable<Device> query, DeviceListViewModel condition)
    {
        _logger.LogInformation("FilterDevicesByFreeText 開始 FreeText={FreeText}", condition.FreeText);

        // フリーワードの前後の空白をトリム
        var free = condition.FreeText!.Trim();

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

        _logger.LogInformation("FilterDevicesByFreeText 終了 FreeText={FreeText}", condition.FreeText);
        return query;
    }

    // フィルタリング (条件に応じて IQueryable<Device> を返す)
    private IQueryable<Device> FilterDevices(IQueryable<Device> query, DeviceListViewModel condition)
    {
        _logger.LogInformation("FilterDevices 開始");

        // 部分一致
        if (!string.IsNullOrWhiteSpace(condition.ManagementId))
        {
            query = query.Where(x => x.ManagementId.Contains(condition.ManagementId));
        }

        if (!string.IsNullOrWhiteSpace(condition.ModelNumber))
        {
            // 前後の空白をトリムした上で部分一致検索
            query = query.Where(x => x.ModelNumber.Contains(condition.ModelNumber.Trim()));
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
        _logger.LogInformation("FilterDevices 終了");
        return query;
    }

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    private IQueryable<Device> SortDevices(IQueryable<Device> query, DeviceColumns sortKey, SortOrders sortOrder)
    {
        _logger.LogInformation("SortDevices 開始 SortKey={SortKey}, SortOrder={SortOrder}", sortKey, sortOrder);

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

        _logger.LogInformation("SortDevices 終了 SortKey={SortKey}, SortOrder={SortOrder}", sortKey, sortOrder);
        return query;
    }

    // ページング (Skip/Take の適用)
    private IQueryable<Device> PagingDevices(IQueryable<Device> query, int pageNumber, int pageSize)
    {
        _logger.LogInformation("PagingDevices 開始 PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);

        // ページングを適用
        query = query.Skip((pageNumber - 1) * pageSize);
        query = query.Take(pageSize);

        _logger.LogInformation("PagingDevices 終了 PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);
        return query;
    }

    // ViewModel 変換(結果を DeviceListViewModel に詰める)
    private DeviceListViewModel ToViewModel(DeviceListViewModel condition, List<Device> devices, Roles? role)
    {
        _logger.LogInformation("ToViewModel 開始");

        // プルダウン用のデータを定数から取得
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(condition, selectList => condition.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceColumns>(condition, selectList => condition.SortKeyList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(condition, selectList => condition.StatusItems = selectList);

        // DeviceCategoryのみ、Role別でプルダウン用データを変更
        // Admin：すべて、それ以外：インフラに関わる機器を除外したもの
        if (role == Roles.Admin)
        {
            _logger.LogInformation("ToViewModel プルダウン用Categoryデータ設定 Admin");
            EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList => condition.CategoryItems = selectList);
        }
        else
        {
            _logger.LogInformation("ToViewModel プルダウン用Categoryデータ設定 非Admin");
            EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList =>
            condition.CategoryItems = new SelectList(
                EnumDisplayHelper.EnumToDictionary(
                    false, (DeviceConstants.HIDE_CATEGORIES)), "Key", "Value"));
        }

        // 検索結果の一覧表示のデータをDTO型で詰める
        _logger.LogInformation("ToViewModel 検索結果DTO変換 件数={Count}", devices.Count);
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

        _logger.LogInformation("ToViewModel 検索結果のDTO変換 終了 件数={Count}", condition.Devices.Count);
        return condition;
    }


    ///////////////////////////////////////////////////
    // -- Create --

    // Create画面のinitialize
    public DeviceCreateViewModel InitializeCreateView(DeviceCreateViewModel model)
    {
        _logger.LogInformation("InitializeCreateView 開始");

        // ビューモデルに、ドロップダウン用のSelectListをセット
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);
        
        // 購入日に今日の日付をセット
        model.PurchaseDate = DateTime.Now;

        _logger.LogInformation("InitializeCreateView 終了");
        return model;
    }

    // 機器情報の新規登録処理
    public async Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model, string userName)
    {
        _logger.LogInformation("RegisterNewDeviceAsync 開始");

        // トランザクション処理
        _logger.LogInformation("RegisterNewDeviceAsync トランザクション実行戦略作成");
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("RegisterNewDeviceAsync トランザクション開始");

            // トランザクション開始
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("RegisterNewDeviceAsync Entity作成開始");

                var now = DateTime.Now;

                // 受け取ったビューモデルからEntity 作成
                var entity = new Device
                {
                    // ModelNumber, Memo は前後の空白を除去して登録
                    ManagementId = await GenerateManagementIdAsync(),
                    Category = model.SelectedCategory,
                    Purpose = model.SelectedPurpose,
                    ModelNumber = model.ModelNumber.Trim(),
                    SerialNumber = model.SerialNumber,
                    HostName = model.HostName,
                    Location = model.Location,
                    UserName = model.UserName,
                    Status = model.SelectedStatus,
                    Memo = model.Memo != null ? model.Memo.Trim() : null,
                    PurchaseDate = model.PurchaseDate!.Value,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                // Device を追加
                _context.Devices.Add(entity);
                _logger.LogInformation("RegisterNewDeviceAsync Entity作成完了 ManagementId={ManagementId}", entity.ManagementId);

                // 新規登録に関する履歴を追加
                _logger.LogInformation("RegisterNewDeviceAsync 履歴レコード追加開始 ManagementId={ManagementId}", entity.ManagementId);
                await _deviceHistoryService.AddCreateHistoryAsync(entity, userName);

                // DBへの登録処理を実施
                _logger.LogInformation("RegisterNewDeviceAsync DB登録処理開始 ManagementId={ManagementId}", entity.ManagementId);
                var result = await _context.SaveChangesAsync();

                // Commitする
                await transaction.CommitAsync();
                _logger.LogInformation("RegisterNewDeviceAsync トランザクションCommit完了 ManagementId={ManagementId}", entity.ManagementId);

                // 結果の状態エントリの数を返す
                _logger.LogInformation("RegisterNewDeviceAsync 終了 ManagementId={ManagementId}, Result={Result}", entity.ManagementId, result);
                return result;
            }
            catch
            {
                _logger.LogError("RegisterNewDeviceAsync トランザクションエラー ロールバック実行");
                // 失敗したら Rollback
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    // ManagementId を生成して返す
    // 採番テーブルを使って、ManagementId が競合しないようにする
    private async Task<string> GenerateManagementIdAsync()
    {
        _logger.LogInformation("GenerateManagementIdAsync 開始");

        var seq = await _deviceSequenceService.GetNextManagementIdAsync();
        
        var id = DeviceConstants.DEVICE_ID_PREFIX +
                 seq.ToString($"D{DeviceConstants.DEVICE_ID_NUM_DIGIT_COUNT}");
        
        _logger.LogInformation("GenerateManagementIdAsync 機器管理ID生成成功 GeneratedId={GeneratedId}", id);

        return id;
    }

    // ManagementIdの自動採番を履歴テーブル内の最大 ManagementId からの連番になるよう同期
    // ダミーデータ追加時などの整合性の担保
    public async Task SyncDeviceSequenceAsync()
    {
        _logger.LogInformation("SyncDeviceSequenceAsync 開始");

        // DevicesテーブルからすべてのManagementIdを取得
        // 削除済みデータも含める
        var historyIdList = await _context.Devices
            .IgnoreQueryFilters()
            .Select(h => h.ManagementId)
            .ToListAsync();
        _logger.LogInformation("SyncDeviceSequenceAsync DevicesテーブルからManagementId取得 件数={Count}", historyIdList.Count);

        // ManagementId の数字部分の最大値を取得
        // "DE000001" → 数字部分 "000001" → 1 に変換
        int maxNum = historyIdList
            .Select(id => int.Parse(id.Substring(DeviceConstants.DEVICE_ID_PREFIX.Length)))
            .DefaultIfEmpty(0)
            .Max();
        _logger.LogInformation("SyncDeviceSequenceAsync ManagementId数字部分の最大値取得 MaxNum={MaxNum}", maxNum);

        // 採番テーブルを最新の値に同期
        // 最大値をプレースホルダーで渡す
        _logger.LogInformation("SyncDeviceSequenceAsync 採番テーブル更新開始 MaxNum={MaxNum}", maxNum);

        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE DeviceSequences SET LastUsedNumber = @p0 WHERE Id = 1", maxNum);
        _logger.LogInformation("SyncDeviceSequenceAsync 採番テーブル更新完了 MaxNum={MaxNum}", maxNum);

        _logger.LogInformation("SyncDeviceSequenceAsync 終了");
    }


    //////////////////////////////////////////
    // --- Details ---

    // Devices の Id を取得し、該当の機器情報をDTO型で返す
    public async Task<DeviceDto?> GetDeviceDetailsByIdAsync(int id)
    {
        _logger.LogInformation("GetDeviceDetailsByIdAsync 開始 Id={Id}", id);

        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        _logger.LogInformation("GetDeviceDetailsByIdAsync 取得結果 Device={Device}", device != null ? "Found" : "NotFound");

        // 見つからなかった場合は null を返す
        if (device == null)
        {
            _logger.LogInformation("GetDeviceDetailsByIdAsync 終了 Id={Id} NotFound", id);
            return null;
        }

        _logger.LogInformation("GetDeviceDetailsByIdAsync DTO変換開始 Id={Id}", id);
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

        _logger.LogInformation("GetDeviceDetailsByIdAsync 終了 Id={Id}", id);
        // DTO型を返す
        return dto;
    }


    //////////////////////////////////////////
    // --- Edit ---

    // Edit画面のinitialize
    public async Task<DeviceEditViewModel?> InitializeEditView(int id, Roles role)
    {
        _logger.LogInformation("InitializeEditView 開始 Id={Id}, Role={Role}", id, role);

        // Edit画面に表示する為の Device情報を取得
        // Role別の編集可否項目も設定されたビューモデルを受け取る
        var model = await GetDeviceEditViewAsync(id, role);
        _logger.LogInformation("InitializeEditView 取得結果 Model={Model}", model != null ? "Found" : "NotFound");

        // 取得できなければ null を返す
        if (model == null)
        {
            _logger.LogInformation("InitializeEditView 終了 Id={Id} NotFound", id);
            return null;
        }

        // ビューモデルに、ドロップダウン用のSelectListをセット
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);

        // ビューモデルを返す
        _logger.LogInformation("InitializeEditView 終了 Id={Id}", id);
        return model;
    }

    // Edit画面に表示する為の Device情報を取得し、ビューモデルを返す
    // Role別の編集可否項目もここで設定する
    private async Task<DeviceEditViewModel?> GetDeviceEditViewAsync(int id, Roles role)
    {
        _logger.LogInformation("GetDeviceEditViewAsync 開始 Id={Id}, Role={Role}", id, role);

        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices.FindAsync(id);
        _logger.LogInformation("GetDeviceEditViewAsync 取得結果 Device={Device}", device != null ? "Found" : "NotFound");

        // 見つからなかった場合は null を返す
        if (device == null)
        {
            _logger.LogInformation("GetDeviceEditViewAsync 終了 Id={Id} NotFound", id);
            return null;
        }

        _logger.LogInformation("GetDeviceEditViewAsync ビューモデル変換開始 Id={Id}", id);
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
            Memo = device.Memo,
            Role = role
        };
        _logger.LogInformation("GetDeviceEditViewAsync ビューモデル変換完了 Id={Id}", id);

        // Role別 ReadOnly制御設定
        var vm = SetIsReadOnly(model, role);

        // モデルを返す
        _logger.LogInformation("GetDeviceEditViewAsync 終了 Id={Id}", id);
        return vm;
    }

    // 入力エラー時、SelectList ＆ ReadOnly 制御だけ再設定する
    public async Task<DeviceEditViewModel> RestoreEditViewSettingsAsync(DeviceEditViewModel model, Roles role)
    {
        _logger.LogInformation("RestoreEditViewSettingsAsync 開始 Id={Id}, Role={Role}", model.IdHidden, role);

        // RoleがEditorの場合、DBから値を取得して、再度編集不可項目のみ上書きする
        if (role == Roles.Editor)
        {
            // Devicesテーブルから指定のIdのデータを取得する
            var device = await _context.Devices.FindAsync(model.IdHidden);

            // 見つからなかった場合は null を返す
            if (device == null)
            {
                _logger.LogInformation("RestoreEditViewSettingsAsync 終了 Id={Id} NotFound", model.IdHidden);
                return null;
            }

            model = new DeviceEditViewModel
            {
                IdHidden = model.IdHidden,
                SelectedCategory = device.Category,
                SelectedPurpose = device.Purpose,
                ModelNumber = device.ModelNumber,
                SerialNumber = device.SerialNumber,
                HostName = device.HostName,
                SelectedStatus = device.Status,
                PurchaseDate = device.PurchaseDate,
                Memo = device.Memo,
                Role = role
            };
        }

        // Role別 ReadOnly制御設定
        var vm = SetIsReadOnly(model, role);

        // ビューモデルに、ドロップダウン用のSelectListをセット
        EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);

        _logger.LogInformation("RestoreEditViewSettingsAsync 終了 Id={Id}", model.IdHidden);

        // Adminの場合、そのまま返す
        return vm;
    }

    // Role別 ReadOnly制御設定
    private DeviceEditViewModel SetIsReadOnly(DeviceEditViewModel model, Roles role)
    {
        _logger.LogInformation("SetIsReadOnly 開始");

        _logger.LogInformation("GetDeviceEditViewAsync ReadOnly制御設定 Role={Role}", role);
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

        _logger.LogInformation("SetIsReadOnly 終了");
        return model;
    }

    // 更新前の状態と変更があるかチェック
    // 1つでも異なっていれば true を返す
    public async Task<bool> HasDeviceChangedAsync(DeviceEditViewModel model)
    {
        _logger.LogInformation("HasDeviceChangedAsync 開始 Id={Id}", model.IdHidden);

        // Devicesテーブルから指定のIdの更新前データを取得する
        var device = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == model.IdHidden);
        _logger.LogInformation("HasDeviceChangedAsync 取得結果 Device={Device}", device != null ? "Found" : "NotFound");

        // 見つからなかった場合は false を返す
        if (device == null)
        {
            _logger.LogInformation("HasDeviceChangedAsync 終了 Id={Id} NotFound", model.IdHidden);
            return false;
        }

        // 各項目を比較し、1つでも異なっていれば true を返す
        if (device.Category != model.SelectedCategory ||
            device.Purpose != model.SelectedPurpose ||
            device.ModelNumber != model.ModelNumber.Trim() ||
            device.SerialNumber != model.SerialNumber ||
            device.HostName != model.HostName ||
            device.Location != model.Location ||
            device.UserName != model.UserName ||
            device.Status != model.SelectedStatus ||
            device.Memo != (model.Memo != null ? model.Memo.Trim() : null) ||
            device.PurchaseDate != model.PurchaseDate!.Value)
        {
            _logger.LogInformation("HasDeviceChangedAsync 終了 Id={Id} Changed", model.IdHidden);
            return true;
        }
        // すべて同じ場合は false を返す
        _logger.LogInformation("HasDeviceChangedAsync 終了 Id={Id} NotChanged", model.IdHidden);
        return false;
    }

    // 機器情報の更新処理
    public async Task<int> UpdateDeviceAsync(DeviceEditViewModel model, Roles role, string userName)
    {
        _logger.LogInformation("UpdateDeviceAsync 開始 Id={Id}, Role={Role}", model.IdHidden, role);

        // 履歴作成用に、トラッキングなしで事前のデータを取得しておく
        var before = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == model.IdHidden);
        _logger.LogInformation("UpdateDeviceAsync 取得結果 BeforeDevice={BeforeDevice}", before != null ? "Found" : "NotFound");

        // 見つからなかった場合は -1 を返す
        if (before == null)
        {
            _logger.LogInformation("UpdateDeviceAsync 終了 Id={Id} NotFound", model.IdHidden);
            return -1;
        }

        // Devicesテーブルから指定のIdのデータを取得する
        var entity = await _context.Devices.FindAsync(model.IdHidden);
        _logger.LogInformation("UpdateDeviceAsync 取得結果 Device={Device}", entity != null ? "Found" : "NotFound");

        // 見つからなかった場合は -1 を返す
        if (entity == null)
        {
            _logger.LogInformation("UpdateDeviceAsync 終了 Id={Id} NotFound", model.IdHidden);
            return -1;
        }

        // トランザクション処理
        var strategy = _context.Database.CreateExecutionStrategy();
        _logger.LogInformation("UpdateDeviceAsync トランザクション実行戦略作成 Id={Id}", model.IdHidden);
        
        return await strategy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("UpdateDeviceAsync トランザクション開始 Id={Id}", model.IdHidden);

            // トランザクション開始
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("UpdateDeviceAsync Entity更新処理開始 Id={Id}", model.IdHidden);
                // Role判定
                // Role が Editor であれば Location, UserName, UpdateAt のみ更新
                if (role == Roles.Admin)
                {
                    // ModelNumber, Memo は前後の空白を除去して登録
                    entity.Category = model.SelectedCategory;
                    entity.Purpose = model.SelectedPurpose;
                    entity.ModelNumber = model.ModelNumber.Trim();
                    entity.SerialNumber = model.SerialNumber;
                    entity.HostName = model.HostName;
                    entity.Location = model.Location;
                    entity.UserName = model.UserName;
                    entity.Status = model.SelectedStatus;
                    entity.Memo = model.Memo != null ? model.Memo.Trim() : null;
                    entity.PurchaseDate = model.PurchaseDate!.Value;
                    entity.UpdatedAt = DateTime.Now;
                }
                else
                {
                    entity.Location = model.Location;
                    entity.UserName = model.UserName;
                    entity.UpdatedAt = DateTime.Now;
                }

                // 更新に関する履歴レコードを追加
                _logger.LogInformation("UpdateDeviceAsync 履歴レコード追加開始 Id={Id}", model.IdHidden);
                await _deviceHistoryService.AddUpdateHistoryAsync(before, entity, userName);

                // DBへの登録処理を実施、状態エントリの数を受け取る
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("UpdateDeviceAsync DB登録処理完了 Id={Id}", model.IdHidden);

                // Commitする
                await transaction.CommitAsync();
                _logger.LogInformation("UpdateDeviceAsync トランザクションCommit完了 Id={Id}", model.IdHidden);

                // 結果の状態エントリの数を返す
                _logger.LogInformation("UpdateDeviceAsync 終了 Id={Id}, Result={Result}", model.IdHidden, result);
                return result;
            }
            catch
            {
                // 失敗したら Rollback
                _logger.LogError("UpdateDeviceAsync トランザクションエラー ロールバック実行 Id={Id}", model.IdHidden);
                
                await transaction.RollbackAsync();
                throw;
            }
        });
    }


    //////////////////////////////////////////
    // --- Delete ---

    // Devices の Id を取得し、該当の機器情報をDeviceDeleteViewModelで返す
    public async Task<DeviceDeleteViewModel?> GetDeleteDeviceByIdAsync(int id)
    {
        _logger.LogInformation("GetDeleteDeviceByIdAsync 開始 Id={Id}", id);

        // Devicesテーブルから指定のIdのデータを取得する
        var device = await _context.Devices.FindAsync(id);
        _logger.LogInformation("GetDeleteDeviceByIdAsync 取得結果 Device={Device}", device != null ? "Found" : "NotFound");

        // 見つからない or 削除フラグがすでに true の場合は、null を返す
        if (device == null || device.IsDeleted)
        {
            _logger.LogInformation("GetDeleteDeviceByIdAsync 終了 Id={Id} NotFoundOrDeleted", id);
            return null;
        }

        // 取得したデータをビューモデルに詰める
        _logger.LogInformation("GetDeleteDeviceByIdAsync ビューモデル変換開始 Id={Id}", id);
        var model = new DeviceDeleteViewModel
        {
            IdHidden = device.Id,
            ManagementId = device.ManagementId,
            Category = device.Category,
            Status = device.Status,
            UserName = device.UserName,
        };

        // ビューモデルを返す
        _logger.LogInformation("GetDeleteDeviceByIdAsync 終了 Id={Id}", id);
        return model;
    }

    // 対象の機器情報を論理削除(ソフトデリート)する
    public async Task<int> DeleteDeviceAsync(int id, string deletedBy)
    {
        _logger.LogInformation("DeleteDeviceAsync 開始 Id={Id}", id);

        // 履歴作成用に、トラッキングなしで事前のデータを取得しておく
        var before = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        _logger.LogInformation("DeleteDeviceAsync 取得結果 BeforeDevice={BeforeDevice}", before != null ? "Found" : "NotFound");

        // 見つからない or 削除フラグがすでに true の場合は、-1 を返す
        if (before == null || before.IsDeleted)
        {
            _logger.LogInformation("DeleteDeviceAsync 終了 Id={Id} NotFoundOrDeleted", id);
            return -1;
        }

        // Devicesテーブルから指定のIdのデータを取得する
        var entity = await _context.Devices.FindAsync(id);
        _logger.LogInformation("DeleteDeviceAsync 指定Idの取得結果 Device={Device}", entity != null ? "Found" : "NotFound");

        // 見つからない or 削除フラグがすでに true の場合は、-1 を返す
        if (entity == null || entity.IsDeleted)
        {
            _logger.LogInformation("DeleteDeviceAsync 終了 Id={Id} NotFoundOrDeleted", id);
            return -1;
        }

        // トランザクション処理
        _logger.LogInformation("DeleteDeviceAsync トランザクション実行戦略作成 Id={Id}", id);
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            // トランザクション開始
            _logger.LogInformation("DeleteDeviceAsync トランザクション開始 Id={Id}", id);
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("DeleteDeviceAsync 論理削除処理開始 Id={Id}", id);
                // 現在日時を取得
                var now = DateTime.Now;

                // 削除フラグ と 削除実施者を更新する
                entity.IsDeleted = true;
                entity.DeletedBy = deletedBy;
                entity.UpdatedAt = now;
                entity.DeletedAt = now;

                // 削除に関する履歴レコードを追加
                _logger.LogInformation("DeleteDeviceAsync 履歴レコード追加開始 Id={Id}", id);
                await _deviceHistoryService.AddDeleteHistoryAsync(before, entity, deletedBy);

                // DBへの更新処理を実施、状態エントリの数を受け取る
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("DeleteDeviceAsync DB更新処理完了 Id={Id}", id);

                // Commitする
                await transaction.CommitAsync();
                _logger.LogInformation("DeleteDeviceAsync トランザクションCommit完了 Id={Id}", id);

                // 結果の状態エントリの数を返す
                _logger.LogInformation("DeleteDeviceAsync 終了 Id={Id}, Result={Result}", id, result);
                return result;
            }
            catch
            {
                // 失敗したら Rollback
                _logger.LogError("DeleteDeviceAsync トランザクションエラー ロールバック実行 Id={Id}", id);

                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
