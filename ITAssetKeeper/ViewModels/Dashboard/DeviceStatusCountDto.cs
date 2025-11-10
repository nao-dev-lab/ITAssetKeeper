namespace ITAssetKeeper.ViewModels.Dashboard;

// DTO:
// Admin用Dashboard：状態別の台数集計
public class DeviceStatusCountDto
{
    public int WorkingCount { get; set; }
    public int ReserveCount { get; set; }
    public int BrokenCount { get; set; }
    public int ScheduledForDisposalCount { get; set; }
    public int DisposedCount { get; set; }
}
