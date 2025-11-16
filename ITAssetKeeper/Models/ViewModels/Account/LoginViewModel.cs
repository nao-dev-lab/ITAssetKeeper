using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Account;

// Login画面のビューモデル
public class LoginViewModel
{
    [Required(ErrorMessage = "{0} が入力されていません")]
    [StringLength(30, ErrorMessage ="{0} は{2}から{1}文字の範囲で入力してください")]
    [Display(Name = "ユーザーID")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "{0} が入力されていません")]
    [StringLength(256, ErrorMessage = "{0} は{2}から{1}文字の範囲で入力してください")]
    [DataType(DataType.Password)]
    [Display(Name = "パスワード")]
    public string Password { get; set; }

    [Display(Name = "ログイン情報を保存する")]
    public bool RememberMe { get; set; }
}
