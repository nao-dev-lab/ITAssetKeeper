using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Dashboard;

// DTO:
// Admin用Dashboard：状態別の台数集計
public class DeviceStatusCountDto
{
    [Display(Name = "稼働中")]
    public int WorkingCount { get; set; }

    [Display(Name = "予備")]
    public int ReserveCount { get; set; }

    [Display(Name = "故障")]
    public int BrokenCount { get; set; }

    [Display(Name = "廃棄予定")]
    public int ScheduledForDisposalCount { get; set; }

    [Display(Name = "廃棄済")]
    public int DisposedCount { get; set; }
}
