using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum SortKeyColums
{
    [Display(Name = "機器管理ID")]
    ManagementId = 1,

    [Display(Name = "種別")]
    Category = 2,

    [Display(Name = "用途")]
    Purpose = 3,

    [Display(Name = "型番(モデル)")]
    ModelNumber = 4,

    [Display(Name = "製造番号(シリアル)")]
    SerialNumber = 5,

    [Display(Name = "ホスト名")]
    HostName = 6,

    [Display(Name = "設置場所")]
    Location = 7,

    [Display(Name = "使用者")]
    UserName = 8,

    [Display(Name = "状態")]
    Status = 9,

    [Display(Name = "購入日")]
    PurchaseDate = 10
}
