using System;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Entities;

// Device Entity
public class Device
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "種別")]
    public string Category { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "用途")]
    public string Purpose { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "型番(モデル)")]
    public string ModelNumber { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "製造番号(シリアル)")]
    public string SerialNumber { get; set; }

    [MaxLength(100)]
    [Display(Name = "ホスト名")]
    public string? HostName { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "設置場所")]
    public string Location { get; set; }

    [MaxLength(100)]
    [Display(Name = "使用者")]
    public string? UserName { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "状態")]
    public string Status { get; set; }

    [Display(Name = "メモ")]
    public string? Memo { get; set; }

    [Required]
    [Display(Name = "購入日")]
    public DateTime PurchaseDate { get; set; }

    [Required]
    [Display(Name = "登録日")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "更新日")]
    public DateTime? UpdatedAt { get; set; }

    [Required]
    [Display(Name = "削除フラグ")]
    // 規定値を 0 とするので false を初期値としておく
    public bool IsDeleted { get; set; } = false;

    [Display(Name = "削除日")]
    public DateTime? DeletedAt { get; set; }

    [MaxLength(100)]
    [Display(Name = "削除実施者")]
    public string? DeletedBy { get; set; }
}
