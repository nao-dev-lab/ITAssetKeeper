using ITAssetKeeper.Constants;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

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

    // アカウント情報表示用のビューモデルを取得
    public async Task<ProfileViewModel> GetProfileViewModelAsync(ApplicationUser user)
    {
        // user が無ければ null を返す
        if (user == null || user.UserName == null)
        {
            return null;
        }

        // Roleを取得
        var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
        
        // どのRoleにいるか判定
        bool isAdmin = roles.Contains("Admin");
        bool isEditor = roles.Contains("Editor");
        bool isViewer = roles.Contains("Viewer");

        // 割り当てられているRoleを取得
        string role;
        if (isAdmin) { role = "管理者"; }
        else if (isEditor) { role = "編集者"; }
        else { role = "一般ユーザー"; }

        // ビューモデルに値を設定
        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Role = role,
            Email = user.Email == null ? "-" : user.Email,
            PasswordExpirationDate = user.PasswordExpirationDate,
        };

        // ビューモデルを返す
        return model;
    }
}
