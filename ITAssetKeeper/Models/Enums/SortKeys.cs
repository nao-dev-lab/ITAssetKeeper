using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum SortKeys
{
    [Display(Name = "機器管理ID")]
    ManagementId,

    [Display(Name = "種別")]
    Category,

    [Display(Name = "用途")]
    Purpose,

    [Display(Name = "型番(モデル)")]
    ModelNumber,

    [Display(Name = "製造番号(シリアル)")]
    SerialNumber,

    [Display(Name = "ホスト名")]
    HostName,

    [Display(Name = "設置場所")]
    Location,

    [Display(Name = "使用者")]
    UserName,

    [Display(Name = "状態")]
    Status,

    [Display(Name = "購入日")]
    PurchaseDate,

    [Display(Name = "更新日")]
    UpdatedAt
}
