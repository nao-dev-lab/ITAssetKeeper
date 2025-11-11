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
        if (await userManager.FindByNameAsync(ApplicationIdentityConstants.ADMIN_DEFAULT_NAME) != null)
        {
            return;
        }

        // 作成する Seed Adminユーザーの設定
        string adminUserName = ApplicationIdentityConstants.ADMIN_DEFAULT_NAME;
        var adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            Email = ApplicationIdentityConstants.ADMIN_DEFAULT_EMAIL,
            EmailConfirmed = true,
            IsActive = true,
            PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS)
        };

        // 作成する Seed Editorユーザーの設定
        string editorUserName = ApplicationIdentityConstants.EDITOR_DEFAULT_NAME;
        var editorUser = new ApplicationUser
        {
            UserName = editorUserName,
            Email = ApplicationIdentityConstants.EDITOR_DEFAULT_EMAIL,
            EmailConfirmed = true,
            IsActive = true,
            PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS)
        };

        // 作成する Seed Viewerユーザーの設定
        string viewerUserName = ApplicationIdentityConstants.VIEWER_DEFAULT_NAME;
        var viewerUser = new ApplicationUser
        {
            UserName = viewerUserName,
            Email = ApplicationIdentityConstants.VIEWER_DEFAULT_EMAIL,
            EmailConfirmed = true,
            IsActive = true,
            PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS)
        };

        // Seed Adminの作成とRoleの追加を実施
        var adminResult = await userManager.CreateAsync(adminUser, ApplicationIdentityConstants.ADMIN_DEFAULT_PASSWORD);
        if (!adminResult.Succeeded)
        {
            foreach (var err in adminResult.Errors)
            {
                Console.WriteLine($"ADMIN ERROR: {err.Description}");
            }
        }
        await userManager.AddToRoleAsync(adminUser,Roles.Admin.ToString());

        // Seed Editorの作成とRoleの追加を実施
        var editorResult = await userManager.CreateAsync(editorUser, ApplicationIdentityConstants.EDITOR_DEFAULT_PASSWORD);
        if (!editorResult.Succeeded)
        {
            foreach (var err in editorResult.Errors)
            {
                Console.WriteLine($"EDITOR ERROR: {err.Description}");
            }
        }
        await userManager.AddToRoleAsync(editorUser, Roles.Editor.ToString());

        // Seed Viewerの作成とRoleの追加を実施
        var viewerResult = await userManager.CreateAsync(viewerUser, ApplicationIdentityConstants.VIEWER_DEFAULT_PASSWORD);
        if (!viewerResult.Succeeded)
        {
            foreach (var err in viewerResult.Errors)
            {
                Console.WriteLine($"VIEWER ERROR: {err.Description}");
            }
        }
        await userManager.AddToRoleAsync(viewerUser, Roles.Viewer.ToString());
    }
}
