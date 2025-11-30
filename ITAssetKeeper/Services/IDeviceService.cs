using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public interface IDeviceService
{
    //////////////////////////////////////////
    // --- Index ---

    // Index用 統合メソッド
    Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition, ClaimsPrincipal user);


    //////////////////////////////////////////
    // --- Create ---

    // Create,画面のinitialize
    DeviceCreateViewModel InitializeCreateView(DeviceCreateViewModel model);

    // 機器登録処理
    Task<int> RegisterNewDeviceAsync(DeviceCreateViewModel model, string userName);

    // 採番テーブルの同期用
    Task SyncDeviceSequenceAsync();


    //////////////////////////////////////////
    // --- Details ---

    // 機器情報を ID から取得し、DTO型で返す
    Task<DeviceDto?> GetDeviceDetailsByIdAsync(int id);


    //////////////////////////////////////////
    // --- Edit ---

    // Edit画面のinitialize
    Task<DeviceEditViewModel?> InitializeEditView(int id, Roles role);

    // 入力エラー時用：SelectList ＆ ReadOnly 制御だけ再設定する
    Task<DeviceEditViewModel> RestoreEditViewSettingsAsync(DeviceEditViewModel model, Roles role);

    // 更新前の状態と変更があるかチェック
    Task<bool> HasDeviceChangedAsync(DeviceEditViewModel model);

    // 機器情報の更新処理
    Task<int> UpdateDeviceAsync(DeviceEditViewModel model, Roles role, string userName);


    //////////////////////////////////////////
    // --- Delete ---

    // 機器情報を ID から取得し、DeviceDeleteViewModelで返す
    Task<DeviceDeleteViewModel?> GetDeleteDeviceByIdAsync(int id);

    // 対象の機器情報をソフトデリートする
    Task<int> DeleteDeviceAsync(int id, string deletedBy);
}
