using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Account;

// アカウント情報表示用ビューモデル
public class ProfileViewModel
{
    [Display(Name = "ユーザーID")]
    public string UserName { get; set; }

    [Display(Name = "権限")]
    public string Role { get; set; }

    [Display(Name = "メールアドレス")]
    public string? Email { get; set; }

    [Display(Name = "パスワード有効期限")]
    public DateTime PasswordExpirationDate { get; set; }

    // 表示専用プロパティ（読み取り専用）
    public string PasswordExpirationDateText => PasswordExpirationDate.ToString("yyyy/MM/dd HH:mm:ss");
}
