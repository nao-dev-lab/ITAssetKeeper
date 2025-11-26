using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public class UserRoleService : IUserRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRoleService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // 引数で与えられたClaimsPrincipalからユーザーのロールを取得するメソッド
    public async Task<Roles?> GetUserRoleAsync(ClaimsPrincipal user)
    {
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return null;
        }

        // ユーザーのロールを取得
        var appUser = await _userManager.GetUserAsync(user);

        if (appUser == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(appUser);

        // ロールの判定
        var result = roles.FirstOrDefault() ?? "Unknown";

        return result switch
        {
            "Admin" => Roles.Admin,
            "Editor" => Roles.Editor,
            "Viewer" => Roles.Viewer,
            _ => null
        };
    }
}
