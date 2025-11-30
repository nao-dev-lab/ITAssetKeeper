using System.ComponentModel.DataAnnotations;
using ITAssetKeeper.Constants;

namespace ITAssetKeeper.Models.ViewModels.Account;

// パスワード変更画面のビューモデル
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD)]
    [StringLength(30, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [DataType(DataType.Password)]
    [Display(Name = "現在のパスワード")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD)]
    [StringLength(256, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [DataType(DataType.Password)]
    [Display(Name = "新しいパスワード")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD)]
    [StringLength(256, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [DataType(DataType.Password)]
    [Display(Name = "新しいパスワードを再入力")]
    [Compare("NewPassword", ErrorMessage = ValidationMessages.INVALID_CONFIRM_PASSWORD)]
    public string ConfirmPassword { get; set; }
}
