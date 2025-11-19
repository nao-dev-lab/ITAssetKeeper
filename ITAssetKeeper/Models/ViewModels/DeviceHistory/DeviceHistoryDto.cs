using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.DeviceHistory;

public class DeviceHistoryDto
{
    public int Id { get; set; }

    [Display(Name = "履歴ID")]
    public string HistoryId { get; set; }

    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Display(Name = "更新項目")]
    public string ChangeField { get; set; }

    [Display(Name = "更新前の値")]
    public string BeforeValue { get; set; }

    [Display(Name = "更新後の値")]
    public string AfterValue { get; set; }

    [Display(Name = "更新者")]
    public string UpdatedBy { get; set; }

    [Display(Name = "更新日時")]
    public DateTime UpdatedAt { get; set; }

    // 表示専用プロパティ（読み取り専用）
    public string UpdatedAtText => UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");

}
