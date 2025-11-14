using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;

namespace ITAssetKeeper.Services;

public interface IDeviceService
{
    Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition);
}
