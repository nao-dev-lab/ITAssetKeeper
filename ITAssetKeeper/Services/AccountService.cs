using ITAssetKeeper.Constants;
using ITAssetKeeper.Controllers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;

namespace ITAssetKeeper.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserManager<ApplicationUser> userManager, ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // パスワード期限が超過しているかチェック
    public bool IsPasswordExpired(ApplicationUser user)
    {
        var result = user.PasswordExpirationDate < DateTime.Now;
        _logger.LogDebug("IsPasswordExpired パスワード期限切れ判定: {Result}", result);
        return result;
    }

    // ログイン成功後のRole別のリダイレクト先の振り分け
    public async Task<string> ResolveRedirectAfterLoginAsync(ApplicationUser user)
    {
        _logger.LogDebug("ResolveRedirectAfterLoginAsync リダイレクト先判定開始");
        try
        {
            // Admin であれば Dashboardへリダイレクト
            if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
            {
                _logger.LogDebug("ResolveRedirectAfterLoginAsync Adminユーザーのため/Dashboard/Adminへリダイレクト");
                return "/Dashboard/Admin";
            }

            // Admin以外は機器一覧にリダイレクト
            _logger.LogDebug("ResolveRedirectAfterLoginAsync 一般ユーザーのため/Device/Indexへリダイレクト");
            return "/Device/Index";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResolveRedirectAfterLoginAsync ロール判定中にエラーが発生しました。");

            // 例外にコンテキストを付与して上位へ再スロー
            throw new InvalidOperationException(
                "ロール判定中にエラーが発生しました。", ex);
        }
    }

    // パスワード期限の更新処理
    public void UpdatePasswordExpiration(ApplicationUser user)
    {
        // パスワード変更時、パスワード期限を更新
        user.PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS);
        _logger.LogDebug("UpdatePasswordExpiration パスワード期限を {Date} に更新", user.PasswordExpirationDate);
    }

    // アカウント情報表示用のビューモデルを取得
    public async Task<ProfileViewModel> GetProfileViewModelAsync(ApplicationUser user)
    {
        _logger.LogDebug("GetProfileViewModelAsync アカウント情報表示用ビューモデルの取得開始");

        // user が無ければ null を返す
        if (user == null || user.UserName == null)
        {
            _logger.LogWarning("GetProfileViewModelAsync ユーザー情報が null のためビューモデルを取得できません");
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
        _logger.LogDebug("GetProfileViewModelAsync ユーザーのRole={Role}", role);

        // ビューモデルに値を設定
        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Role = role,
            Email = user.Email == null ? "-" : user.Email,
            PasswordExpirationDate = user.PasswordExpirationDate,
        };

        // ビューモデルを返す
        _logger.LogDebug("GetProfileViewModelAsync アカウント情報表示用ビューモデルの取得完了");
        return model;
    }
}
