using ITAssetKeeper.Constants;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace ITAssetKeeper.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // パスワード期限が超過しているかチェック
    public bool IsPasswordExpired(ApplicationUser user)
    {
        return user.PasswordExpirationDate < DateTime.Now;
    }

    // ログイン成功後のRole別のリダイレクト先の振り分け
    public async Task<string> ResolveRedirectAfterLoginAsync(ApplicationUser user)
    {
        // Admin であれば Dashboardへリダイレクト
        if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
        {
            return "/Dashboard/Admin";
        }
        
        // Admin以外は機器一覧にリダイレクト
        return "/Device/Index";
    }

    // パスワード期限の更新処理
    public void UpdatePasswordExpiration(ApplicationUser user)
    {
        // パスワード変更時、パスワード期限を更新
        user.PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS);
    }
}
