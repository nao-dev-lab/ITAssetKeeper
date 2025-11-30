using ITAssetKeeper.Controllers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public class UserRoleService : IUserRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserRoleService> _logger;

    public UserRoleService(UserManager<ApplicationUser> userManager, ILogger<UserRoleService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // 引数で与えられたClaimsPrincipalからユーザーのロールを取得するメソッド
    public async Task<Roles?> GetUserRoleAsync(ClaimsPrincipal user)
    {
        _logger.LogInformation("GetUserRoleAsync 開始");

        try
        {
            _logger.LogInformation("GetUserRoleAsync ユーザー認証情報確認");
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("GetUserRoleAsync 未認証ユーザーのためロール取得不可");
                return null;
            }

            // ユーザーのロールを取得
            var appUser = await _userManager.GetUserAsync(user);
            _logger.LogInformation("GetUserRoleAsync ユーザー情報取得 UserId={UserId}", appUser?.Id);

            if (appUser == null)
            {
                _logger.LogWarning("GetUserRoleAsync ユーザーが見つかりません UserId={UserId}", user.Identity.Name);
                return null;
            }

            var roles = await _userManager.GetRolesAsync(appUser);
            _logger.LogInformation("GetUserRoleAsync ユーザーロール取得 Roles={Roles}", string.Join(",", roles));

            // ロールの判定
            var result = roles.FirstOrDefault() ?? "Unknown";
            _logger.LogInformation("GetUserRoleAsync 最初のロールを判定 Result={Result}", result);

            _logger.LogInformation("GetUserRoleAsync 完了");
            return result switch
            {
                "Admin" => Roles.Admin,
                "Editor" => Roles.Editor,
                "Viewer" => Roles.Viewer,
                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserRoleAsync 例外発生");
            return null;
        }
    }
}
