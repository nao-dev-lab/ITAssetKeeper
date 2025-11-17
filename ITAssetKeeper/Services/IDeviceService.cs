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
    IQueryable<Device> SortDevices(IQueryable<Device> query, SortKeys sortKey, SortOrders sortOrder);

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

    // 機器情報を ID から取得
    Task<DeviceDto?> GetDeviceByIdAsync(int id);


    //////////////////////////////////////////
    // --- Edit ---

    // Edit画面のinitialize
    Task<DeviceEditViewModel?> InitializeEditView(int id, Roles role);

    // Edit画面に表示する為の Device情報を取得し、ビューモデルを返す
    // Role別の編集可否項目もここで設定する
    Task<DeviceEditViewModel?> GetDeviceEditViewAsync(int id, Roles role);

    // 入力エラー時用：SelectList ＆ ReadOnly 制御だけ再設定する
    Task RestoreEditViewSettingsAsync(DeviceEditViewModel model, Roles role);

    // 機器情報の更新処理
    public Task<int> UpdateDeviceAsync(DeviceEditViewModel model, Roles role);
}
