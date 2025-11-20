using ITAssetKeeper.Models.ViewModels.Dashboard;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;

namespace ITAssetKeeper.Services;

public interface IDashboardService
{
    Task<DashboardAdminViewModel?> GetAdminDashboardDataAsync();
}
