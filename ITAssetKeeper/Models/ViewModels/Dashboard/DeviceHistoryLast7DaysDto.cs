namespace ITAssetKeeper.Models.ViewModels.Dashboard;

// DTO:
// Admin用Dashboard：過去7日間の更新履歴件数
public class DeviceHistoryLast7DaysDto
{
    public int Count { get; set; }
    public string DisplayDate { get; set; }
}
