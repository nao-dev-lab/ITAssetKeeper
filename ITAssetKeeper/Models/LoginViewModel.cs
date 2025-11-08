using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models;

// Login画面のビューモデル
public class LoginViewModel
{
    [Required]
    [Display(Name = "ユーザーID")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "パスワード")]
    public string Password { get; set; }

    [Display(Name = "ログイン情報を保存する")]
    public bool RememberMe { get; set; }
}
