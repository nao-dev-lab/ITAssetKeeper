using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

// 機器種別用Enum
public enum DeviceCategory
{
    [Display(Name = "ノートPC")]
    Laptop = 1,

    [Display(Name = "デスクトップPC")]
    DesktopPc = 2,

    [Display(Name = "L2スイッチ")]
    Switch_L2 = 3,

    [Display(Name = "L3スイッチ")]
    Switch_L3 = 4,

    [Display(Name = "業務用ルーター")]
    Router_Core = 5,  // Cisco,YAMAHA RTX/NVR,Juniperなど

    [Display(Name = "小規模用ルーター")]
    Router_Soho = 6,  // Buffalo,Elecom,NEC Aterm など

    [Display(Name = "サーバー")]
    Server = 7,

    [Display(Name = "ファイアウォール")]
    Firewall = 8,

    [Display(Name = "UPS")]
    Ups = 9,

    [Display(Name = "その他")]
    Other = 99
}
