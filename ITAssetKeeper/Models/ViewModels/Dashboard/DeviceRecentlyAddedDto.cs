using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Dashboard;

// DTO:
// Admin用Dashboard：最近追加された機器一覧(5件)
public class DeviceRecentlyAddedDto
{
    [Display(Name = "登録日")]
    public string DisplayDate { get; set; }

    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Display(Name = "種別")]
    public string Category { get; set; }

    [Display(Name = "型番(モデル)")]
    public string ModelNumber { get; set; }

    [Display(Name = "ホスト名")]
    public string HostName { get; set; }

    [Display(Name = "設置場所")]
    public string Location { get; set; }

    [Display(Name = "使用者")]
    public string UserName { get; set; }

    [Display(Name = "状態")]
    public string Status { get; set; }
}
