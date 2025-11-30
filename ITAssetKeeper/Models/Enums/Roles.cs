using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum Roles
{
    [Display(Name = "管理者")]
    Admin,

    [Display(Name = "特殊ユーザー")]
    Editor,

    [Display(Name = "一般ユーザー")]
    Viewer
}
