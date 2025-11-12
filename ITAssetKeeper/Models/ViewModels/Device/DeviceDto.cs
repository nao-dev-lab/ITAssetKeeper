using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Device;

public class DeviceDto
{
    public int Id { get; set; }

    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Display(Name = "種別")]
    public string Category { get; set; }

    [Display(Name = "用途")]
    public string Purpose { get; set; }

    [Display(Name = "型番(モデル)")]
    public string ModelNumber { get; set; }

    [Display(Name = "製造番号(シリアル)")]
    public string SerialNumber { get; set; }

    [Display(Name = "ホスト名")]
    public string HostName { get; set; }

    [Display(Name = "設置場所")]
    public string Location { get; set; }

    [Display(Name = "使用者")]
    public string UserName { get; set; }

    [Display(Name = "状態")]
    public string Status { get; set; }

    [Display(Name = "メモ")]
    public string Memo { get; set; }

    [Display(Name = "購入日")]
    public DateTime PurchaseDate { get; set; }

    [Display(Name = "登録日")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "削除フラグ")]
    public bool IsDeleted { get; set; }

    [Display(Name = "削除日")]
    public DateTime DeletedAt { get; set; }

    [Display(Name = "削除実施者")]
    public string DeletedBy { get; set; }
}
