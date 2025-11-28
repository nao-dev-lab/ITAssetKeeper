using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITAssetKeeper.Models.Entities;

public class DeviceHistory
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "履歴ID")]
    public string HistoryId { get; set; }

    // 更新種別（Created / Updated / Deleted）
    [Required]
    [MaxLength(20)]
    [Display(Name = "更新種別")]
    public string ChangeType { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "更新者")]
    public string UpdatedBy { get; set; }

    [Required]
    [Display(Name = "更新日時")]
    public DateTime UpdatedAt { get; set; }

    // --- AfterSnapshot（更新後の機器情報） ---
    [Required]
    [MaxLength(20)]
    [Display(Name = "機器管理ID")]
    public string ManagementIdAtHistory { get; set; }

    [MaxLength(50)]
    public string? CategoryAtHistory { get; set; }

    [MaxLength(50)]
    public string? PurposeAtHistory { get; set; }

    [MaxLength(100)]
    public string? ModelNumberAtHistory { get; set; }

    [MaxLength(100)]
    public string? SerialNumberAtHistory { get; set; }

    [MaxLength(100)]
    public string? HostNameAtHistory { get; set; }

    [MaxLength(200)]
    public string? LocationAtHistory { get; set; }

    [MaxLength(100)]
    public string? UserNameAtHistory { get; set; }

    [MaxLength(20)]
    public string? StatusAtHistory { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? MemoAtHistory { get; set; }

    public DateTime? PurchaseDateAtHistory { get; set; }

    public DateTime? CreatedAtHistory { get; set; }

    public bool IsDeletedAtHistory { get; set; } = false;

    public DateTime? DeletedAtHistory { get; set; }

    [MaxLength(100)]
    public string? DeletedByAtHistory { get; set; }
}
