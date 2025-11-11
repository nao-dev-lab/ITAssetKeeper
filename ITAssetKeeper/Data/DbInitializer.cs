using ITAssetKeeper.Constants;
using ITAssetKeeper.Models.Enum;
using ITAssetKeeper.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITAssetKeeper.Data;

public class DbInitializer
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Roleをすべてstringで取得
        var allRoles = Enum.GetNames(typeof(Roles));

        // Roleが存在しなければ追加する
        foreach (var role in allRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // すでに admin があれば何もしないで抜ける
        if (await userManager.FindByNameAsync(ApplicationIdentityConstants.DEFAULT_NAME) != null)
        {
            return;
        }

        // 作成する Seed Adminユーザーの設定
        string adminUserName = ApplicationIdentityConstants.DEFAULT_NAME;
        var adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            Email = ApplicationIdentityConstants.DEFAULT_EMAIL,
            EmailConfirmed = true,
            IsActive = true,
            PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS)
        };

        // Seed Adminの作成とRoleの追加を実施
        await userManager.CreateAsync(adminUser, ApplicationIdentityConstants.DEFAULT_PASSWORD);
        await userManager.AddToRoleAsync(adminUser,Roles.Admin.ToString());
    }
}
