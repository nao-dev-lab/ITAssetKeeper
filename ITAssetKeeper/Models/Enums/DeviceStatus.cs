using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

// 機器状態用Enum
public enum DeviceStatus
{
    [Display(Name = "稼働中")]
    Active = 1,

    [Display(Name = "予備")]
    Spare = 2,

    [Display(Name = "故障")]
    Broken = 3,

    [Display(Name = "廃棄予定")]
    Retiring = 4,

    [Display(Name = "廃棄済")]
    Retired = 5,

    [Display(Name = "削除済")]
    Deleted = 99
}
