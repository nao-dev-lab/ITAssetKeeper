using ITAssetKeeper.Models.Enums;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public interface IUserRoleService
{
    // 引数で与えられたClaimsPrincipalからユーザーのロールを取得するメソッド
    Task<Roles?> GetUserRoleAsync(ClaimsPrincipal user);
}
