using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

// 機器用途用Enum
public enum DevicePurpose
{
    [Display(Name = "業務用")]
    Business = 1,

    [Display(Name = "検証用")]
    Verification = 2
}
