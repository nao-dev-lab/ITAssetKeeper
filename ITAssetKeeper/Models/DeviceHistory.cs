using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models;

// DeviceHistory Entity
public class DeviceHistory
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "履歴ID")]
    public string HistoryId { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "更新項目")]
    public string ChangeField { get; set; }

    [Display(Name = "更新前の値")]
    public string? BeforeValue { get; set; }

    [Display(Name = "更新後の値")]
    public string? AfterValue { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "更新者")]
    public string UpdatedBy { get; set; }

    [Required]
    [Display(Name = "更新日時")]
    public DateTime UpdatedAt { get; set; }
}
