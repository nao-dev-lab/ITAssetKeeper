namespace ITAssetKeeper.Models.ViewModels.Dashboard;

// Admin用DashboardのViewModel
public class DashboardAdminViewModel
{
    // メタデータにアクセスする踏み台
    public DeviceRecentlyAddedDto RecentlyHeader { get; } = new();
    public DeviceStatusCountDto StatusHeader { get; } = new();

    public List<DeviceRecentlyAddedDto> RecentlyAddedDevices { get; set; }
    public List<DeviceHistoryLast7DaysDto> HistoryLast7Days { get; set; }
    public DeviceStatusCountDto StatusCount { get; set; }
}
