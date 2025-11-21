using ITAssetKeeper.Models.Entities;

namespace ITAssetKeeper.Services;

public interface IAccountService
{
    // パスワード期限が超過しているかチェック
    bool IsPasswordExpired(ApplicationUser user);

    // ログイン成功後のRole別のリダイレクト先の振り分け
    Task<string> ResolveRedirectAfterLoginAsync(ApplicationUser user);

    // パスワード期限の更新処理
    void UpdatePasswordExpiration(ApplicationUser user);
}
