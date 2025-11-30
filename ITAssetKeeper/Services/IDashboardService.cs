using ITAssetKeeper.Models.ViewModels.Dashboard;

namespace ITAssetKeeper.Services;

public interface IDashboardService
{
    Task<DashboardAdminViewModel?> GetAdminDashboardDataAsync();
}
