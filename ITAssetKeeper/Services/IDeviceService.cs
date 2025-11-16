using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;

namespace ITAssetKeeper.Services;

public interface IDeviceService
{
    //////////////////////////////////////////
    // --- Index ---

    // Index用 統合メソッド
    Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition);

    // フィルタリング (条件に応じて IQueryable<Device> を返す)
    IQueryable<Device> FilterDevices(IQueryable<Device> query, DeviceListViewModel condition);

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    IQueryable<Device> SortDevices(IQueryable<Device> query, SortKeyColums sortKey, SortOrder sortOrder);

    // ページング (Skip/Take の適用)
    IQueryable<Device> PagingDevices(IQueryable<Device> query, int pageNumber, int pageSize);

    // ViewModel 変換(結果を DeviceListViewModel に詰める)
    DeviceListViewModel ToViewModel(DeviceListViewModel condition, List<Device> devices);


    //////////////////////////////////////////
    // --- Create ---

    // Create,画面のinitialize
    DeviceCreateViewModel InitializeCreateView(DeviceCreateViewModel model);

    // --- Create ---
    // 機器登録処理
    Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model);


    //////////////////////////////////////////
    // --- Details ---
    Task<DeviceDto?> GetDeviceByIdAsync(int id);
}
