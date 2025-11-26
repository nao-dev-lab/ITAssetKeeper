using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public interface IUserRoleService
{
    // 引数で与えられたClaimsPrincipalからユーザーのロールを取得するメソッド
    Task<Roles?> GetUserRoleAsync(ClaimsPrincipal user);
}
