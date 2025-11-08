using ITAssetKeeper.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

// Login用コントローラー
public class LoginController : Controller
{
    // フィールド
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // コンストラクタ
    public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Login
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

        // ログインに成功したらIndexに飛ばす
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        // ログインに失敗したら戻す
        else
        {
            ModelState.AddModelError(string.Empty, "ログインに失敗しました");
            return View(model);
        }
    }
}
