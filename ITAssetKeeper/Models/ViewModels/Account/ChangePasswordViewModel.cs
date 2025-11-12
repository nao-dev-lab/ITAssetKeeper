using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Account;

// パスワード変更画面のビューモデル
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "{0}が入力されていません")]
    [StringLength(30, ErrorMessage = "{0}は{2}から{1}文字の範囲で入力してください")]
    [DataType(DataType.Password)]
    [Display(Name = "現在のパスワード")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "{0}が入力されていません")]
    [StringLength(256, ErrorMessage = "{0}は{2}から{1}文字の範囲で入力してください")]
    [DataType(DataType.Password)]
    [Display(Name = "新しいパスワード")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "{0}が入力されていません")]
    [StringLength(256, ErrorMessage = "{0}は{2}から{1}文字の範囲で入力してください")]
    [DataType(DataType.Password)]
    [Display(Name = "新しいパスワードを再入力")]
    [Compare("NewPassword", ErrorMessage = "新しいパスワードと再入力が一致しません")]
    public string ConfirmPassword { get; set; }

    // パスワード更新に必要なユーザー情報取得用に、
    // ユーザー名をフォームで持ち回す
    //public string UserName { get; set; }
}
