using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.ViewModels.Account;

namespace ITAssetKeeper.Services;

public interface IAccountService
{
    // パスワード期限が超過しているかチェック
    bool IsPasswordExpired(ApplicationUser user);

    // ログイン成功後のRole別のリダイレクト先の振り分け
    Task<string> ResolveRedirectAfterLoginAsync(ApplicationUser user);

    // パスワード期限の更新処理
    void UpdatePasswordExpiration(ApplicationUser user);

    Task<ProfileViewModel> GetProfileViewModelAsync(ApplicationUser user);
}
