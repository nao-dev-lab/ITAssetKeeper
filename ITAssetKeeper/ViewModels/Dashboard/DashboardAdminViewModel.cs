namespace ITAssetKeeper.ViewModels.Dashboard;

// Admin用DashboardのViewModel
public class DashboardAdminViewModel
{
    public List<DeviceRecentlyAddedDto> RecentlyAddedList { get; set; }
    public List<DeviceHistoryLast7DaysDto> HistoryLast7DaysList { get; set; }
    public DeviceStatusCountDto StatusCount { get; set; }
}
