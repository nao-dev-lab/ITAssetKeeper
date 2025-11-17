using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace ITAssetKeeper.Services;

public class DeviceService : IDeviceService
{
    private readonly ITAssetKeeperDbContext _context;

    public DeviceService(ITAssetKeeperDbContext context)
    {
        _context = context;
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
    public IQueryable<Device> FilterDevices(IQueryable<Device> query, DeviceListViewModel condition)
    {
        // Deviceテーブルから全てのデータを取得する
        query = _context.Devices;

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
        if (condition.PurchaseDateFrom != null && condition.PurchaseDateTo == null)
        {
            // PurchaseDateFrom ～ 今日まで
            query = query.Where(x => x.PurchaseDate >= condition.PurchaseDateFrom && x.PurchaseDate <= DateTime.Now);
        }
        else if (condition.PurchaseDateFrom == null && condition.PurchaseDateTo != null)
        {
            // PurchaseDateTo 以前のものすべて
            query = query.Where(x => x.PurchaseDate <= condition.PurchaseDateTo);
        }
        else if (condition.PurchaseDateFrom != null && condition.PurchaseDateTo != null)
        {
            if (condition.PurchaseDateFrom == condition.PurchaseDateTo)
            {
                // 同一の日付が入力されている場合は、PurchaseDateFromで指定された日付
                query = query.Where(x => x.PurchaseDate == condition.PurchaseDateFrom);
            }
            else
            {
                // PurchaseDateFrom ～ PurchaseDateToの範囲
                query = query.Where(x => x.PurchaseDate >= condition.PurchaseDateFrom && x.PurchaseDate <= condition.PurchaseDateTo);
            }
        }

        // 日付範囲：更新日
        if (condition.UpdatedDateFrom != null && condition.UpdatedDateTo == null)
        {
            // UpdatedDateFrom ～ 今日まで
            query = query.Where(x => x.UpdatedAt >= condition.UpdatedDateFrom && x.UpdatedAt <= DateTime.Now);
        }
        else if (condition.UpdatedDateFrom == null && condition.UpdatedDateTo != null)
        {
            // UpdatedDateTo 以前のものすべて
            query = query.Where(x => x.UpdatedAt <= condition.UpdatedDateTo);
        }
        else if (condition.UpdatedDateFrom != null && condition.UpdatedDateTo != null)
        {
            if (condition.UpdatedDateFrom == condition.UpdatedDateTo)
            {
                // 同一の日付が入力されている場合は、UpdatedDateFromで指定された日付
                query = query.Where(x => x.UpdatedAt == condition.UpdatedDateFrom);
            }
            else
            {
                // UpdatedDateFrom ～ UpdatedDateToの範囲
                query = query.Where(x => x.UpdatedAt >= condition.UpdatedDateFrom && x.UpdatedAt <= condition.UpdatedDateTo);
            }
        }

        return query;
    }

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    public IQueryable<Device> SortDevices(IQueryable<Device> query, SortKeys sortKey, SortOrders sortOrder)
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
    public IQueryable<Device> PagingDevices(IQueryable<Device> query, int pageNumber, int pageSize)
    {
        // ページングを適用
        query = query.Skip((pageNumber - 1) * pageSize);
        query = query.Take(pageSize);

        return query;
    }

    // ViewModel 変換(結果を DeviceListViewModel に詰める)
    public DeviceListViewModel ToViewModel(DeviceListViewModel condition, List<Device> devices)
    {
        // プルダウン用のデータを定数から取得
        SelectListHelper.SetEnumSelectList<DeviceCategory>(condition, selectList => condition.Category = selectList);
        SelectListHelper.SetEnumSelectList<DevicePurpose>(condition, selectList => condition.Purpose = selectList);
        SelectListHelper.SetEnumSelectList<DeviceStatus>(condition, selectList => condition.Status = selectList);
        SelectListHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        SelectListHelper.SetEnumSelectList<SortKeys>(condition, selectList => condition.SortKeyList = selectList);

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
                PurchaseDate = x.PurchaseDate.ToString("yyyy/MM/dd"),
                CreatedAt = x.CreatedAt.ToString("yyyy/MM/dd"),
                UpdatedAt = x.UpdatedAt.ToString("yyyy/MM/dd")
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
        SelectListHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        SelectListHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        SelectListHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);
        
        // 購入日に今日の日付をセット
        model.PurchaseDate = DateTime.Now;

        return model;
    }

    // 機器情報の新規登録処理
    public async Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model)
    {
        // トランザクション処理
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // ManagementIDを自動採番する為に、DBに存在するManagementIdを取得
            var idList = await _context.Devices
                .Select(x => x.ManagementId)
                .ToListAsync();

            // プレフィックスを除いた数字部分をint型で取得しなおす
            // 一番大きい数字を取得し、新しいManagementId用に +1 する
            var maxNum = idList
            .Select(id => ExtractNumericId(id))
            .DefaultIfEmpty(0)
            .Max() + 1;


            // 新しい ManagementId を生成
            // プレフィックスを付けて、数字部分が規定の桁数になるように先行0埋めする
            string newMid = DeviceConstants.DEVICE_ID_PREFIX + maxNum.ToString($"D{DeviceConstants.DEVICE_ID_NUM_DIGIT_COUNT}");

            // 受け取ったビューモデルからEntity 作成
            var entity = new Device
            {
                ManagementId = newMid,
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

            _context.Devices.Add(entity);

            // DBへの登録処理を実施、状態エントリの数を受け取る
            var result =  await _context.SaveChangesAsync();

            // Commitする
            scope.Complete();

            // 結果の状態エントリの数を返す
            return result;
        }
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
    public async Task<DeviceDto?> GetDeviceByIdAsync(int id)
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
            PurchaseDate = device.PurchaseDate.ToString("yyyy/MM/dd"),
            CreatedAt = device.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
            UpdatedAt = device.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
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
        SelectListHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        SelectListHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        SelectListHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);

        // ビューモデルを返す
        return model;
    }

    // Edit画面に表示する為の Device情報を取得し、ビューモデルを返す
    // Role別の編集可否項目もここで設定する
    public async Task<DeviceEditViewModel?> GetDeviceEditViewAsync(int id, Roles role)
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
            HiddenId = device.Id,
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
        SelectListHelper.SetEnumSelectList<DeviceCategory>(model, selectList => model.CategoryItems = selectList);
        SelectListHelper.SetEnumSelectList<DevicePurpose>(model, selectList => model.PurposeItems = selectList);
        SelectListHelper.SetEnumSelectList<DeviceStatus>(model, selectList => model.StatusItems = selectList);

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
    public async Task<int> UpdateDeviceAsync(DeviceEditViewModel model, Roles role)
    {
        // Devicesテーブルから指定のIdのデータを取得する
        var entity = await _context.Devices.FindAsync(model.HiddenId);

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
                entity.PurchaseDate = model.PurchaseDate.Value;
            }
            else
            {
                entity.Location = model.Location;
                entity.UserName = model.UserName;
            }

            // DBへの登録処理を実施、状態エントリの数を受け取る
            var result = await _context.SaveChangesAsync();

            // Commitする
            scope.Complete();

            // 結果の状態エントリの数を返す
            return result;
        }
    }
}
