using ITAssetKeeper.Models;
using ITAssetKeeper.Models.Enum;
using ITAssetKeeper.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static ITAssetKeeper.Constants.ApplicationIdentityConstants;

namespace ITAssetKeeper.Controllers;

// Login用コントローラー
public class AccountController : Controller
{
    // フィールド
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // コンストラクタ
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Account/Login
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind("UserName, Password, RememberMe")] LoginViewModel model)
    {
        // 入力エラーがある場合、ビューに戻す
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // ログイン処理の実施
        // ログイン失敗時はロックアウトカウントを増やす
        var result = await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true
        );
        
        // ログインに失敗したら戻す
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "ログインに失敗しました");
            return View(model);
        }

        // ユーザー情報を取得
        // ユーザー情報が見つからなかったらログイン失敗とみなす
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "ログインに失敗しました");
            return View(model);
        }

        // ログインに成功したら、
        // AdminはDashboard、他RoleはIndexにリダイレクト
        if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
        {
            return RedirectToAction("Admin", "Dashboard");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    // POST: Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // サインアウト処理
        await _signInManager.SignOutAsync();

        // ログイン画面にリダイレクト
        return RedirectToAction("Login", "Account");
    }
}
