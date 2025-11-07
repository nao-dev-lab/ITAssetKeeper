using ITAssetKeeper.Constants;
using ITAssetKeeper.Models;
using Microsoft.AspNetCore.Identity;

namespace ITAssetKeeper.Data;

public class DbInitializer
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Roleをすべてstringで取得
        var allRoles = Enum.GetNames(typeof(ApplicationIdentityConstants.Roles));

        // Roleが存在しなければ追加する
        foreach (var role in allRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // すでに admin があれば何もしないで抜ける
        if (await userManager.FindByNameAsync(ApplicationIdentityConstants.DefaultName) != null)
        {
            return;
        }

        // 作成する Seed Adminユーザーの設定
        string adminUserName = ApplicationIdentityConstants.DefaultName;
        var adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            Email = ApplicationIdentityConstants.DefaultEmail,
            EmailConfirmed = true,
            IsActive = true,
            PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PasswordValidDays)
        };

        // Seed Adminの作成とRoleの追加を実施
        await userManager.CreateAsync(adminUser, ApplicationIdentityConstants.DefaultPassword);
        await userManager.AddToRoleAsync(adminUser,ApplicationIdentityConstants.Roles.Admin.ToString());
    }
}
