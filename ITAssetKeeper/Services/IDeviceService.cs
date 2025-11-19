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


    //////////////////////////////////////////
    // --- Create ---

    // Create,画面のinitialize
    DeviceCreateViewModel InitializeCreateView(DeviceCreateViewModel model);

    // --- Create ---
    // 機器登録処理
    Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model, string userName);


    //////////////////////////////////////////
    // --- Details ---

    // 機器情報を ID から取得し、DTO型で返す
    Task<DeviceDto?> GetDeviceDetailsByIdAsync(int id);


    //////////////////////////////////////////
    // --- Edit ---

    // Edit画面のinitialize
    Task<DeviceEditViewModel?> InitializeEditView(int id, Roles role);

    // 入力エラー時用：SelectList ＆ ReadOnly 制御だけ再設定する
    Task RestoreEditViewSettingsAsync(DeviceEditViewModel model, Roles role);

    // 機器情報の更新処理
    Task<int> UpdateDeviceAsync(DeviceEditViewModel model, Roles role, string userName);


    //////////////////////////////////////////
    // --- Delete ---

    // 機器情報を ID から取得し、DeviceDeleteViewModelで返す
    Task<DeviceDeleteViewModel?> GetDeleteDeviceByIdAsync(int id);

    // 対象の機器情報をソフトデリートする
    Task<int> DeleteDeviceAsync(int id, string deletedBy);
}
