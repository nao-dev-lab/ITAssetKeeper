using ITAssetKeeper.Constants;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Account;

// Login画面のビューモデル
public class LoginViewModel
{
    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD)]
    [StringLength(30, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [Display(Name = "ユーザーID")]
    public string UserName { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD)]
    [StringLength(256, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [DataType(DataType.Password)]
    [Display(Name = "パスワード")]
    public string Password { get; set; }

    [Display(Name = "ログイン情報を保存する")]
    public bool RememberMe { get; set; }
}
