using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enum;
using ITAssetKeeper.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static ITAssetKeeper.Constants.ApplicationIdentityConstants;

namespace ITAssetKeeper.Controllers;

// Login用コントローラー
public class AccountController : Controller
{
    // フィールド
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // コンストラクタ
    public AccountController(ITAssetKeeperDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Account/Login
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        // ログイン画面を表示
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

        // ユーザー情報を取得
        var user = await _userManager.FindByNameAsync(model.UserName);

        // ユーザー情報が見つからなかったらログイン失敗とみなし、ログイン画面に戻す
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "ログインに失敗しました");
            return View(model);
        }

        // ログイン処理の実施
        var result = await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            // ログイン失敗時はロックアウトカウントを増やす
            lockoutOnFailure: true
        );

        // ログイン処理の結果が失敗であればログイン画面に戻す
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "ログインに失敗しました");
            return View(model);
        }

        // パスワード期限が超過していないかチェック
        if (user.PasswordExpirationDate < DateTime.Now)
        {
            // 超過している場合はログアウト処理を実施し、
            // パスワード変更画面にリダイレクト
            // リダイレクト先で User.Identity IsAuthenticated が False になるので、
            // TempDataでユーザー名を渡す
            await _signInManager.SignOutAsync();
            TempData["PasswordExpiredMessage"] = "パスワードの有効期限が切れています。再設定してください";
            TempData["UserName"] = user.UserName;
            return RedirectToAction("ChangePassword", "Account");
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
        // ログアウト処理
        await _signInManager.SignOutAsync();

        // ログイン画面にリダイレクト
        return RedirectToAction("Login", "Account");
    }

    // GET: Account/ChangePassword
    [HttpGet]
    public IActionResult ChangePassword()
    {
        // ビューモデルのインスタンス生成
        var vm = new ChangePasswordViewModel();

        // パスワード変更時に必要なユーザー名を取得
        // 取得できなければログイン画面にリダイレクトする
        vm.UserName = TempData.Peek("UserName") as string;
        if (string.IsNullOrWhiteSpace(vm.UserName))
        {
            TempData["PasswordChangeMessage"] = "ユーザー情報の取得に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", "Account");
        }

        // 次のリクエストまで保持させる
        TempData.Keep("UserName");

        // パスワード変更画面を表示
        return View(vm);
    }

    // POST: Account/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([Bind("CurrentPassword, NewPassword, ConfirmPassword")] ChangePasswordViewModel model)
    {
        // 入力エラーがある場合、ビューに戻す
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // モデルからユーザー情報を取得
        var user = await _userManager.FindByNameAsync(model.UserName);

        // ユーザー情報が取得できなかったら、
        // ログアウト処理 + ログイン画面にリダイレクトする
        if (user == null)
        {
            TempData["PasswordChangeMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", "Account");
        }

        // 新旧パスワードが同一なら弾く
        if (model.CurrentPassword == model.NewPassword)
        {
            ModelState.AddModelError(nameof(model.NewPassword), "現在のパスワードと同一のパスワードは使用できません。");
            return View(model);
        }

        // パスワード変更処理の実施
        var result = await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword,
            model.NewPassword
        );

        // パスワード変更処理の結果が失敗であれば、パスワード変更画面に戻す
        if (!result.Succeeded)
        {
            // エラー内容をコンソールに出力
            foreach (var err in result.Errors)
            {
                Console.WriteLine($"[PasswordUpdateError] User={user.UserName}, Description={err.Description}");
            }

            ModelState.AddModelError(string.Empty, "パスワード変更に失敗しました。ログインからやり直してください。");
            return View(model);
        }

        // パスワード変更に成功したら、
        // パスワード期限を更新
        //user.PasswordExpirationDate = DateTime.Now.AddDays(PASSWORD_VALID_DAYS);
        user.PasswordExpirationDate = DateTime.Now.AddDays(42);
        var updateResult = await _userManager.UpdateAsync(user);

        // DB側の更新に失敗していればコンソールにログを出力
        if (!updateResult.Succeeded)
        {
            foreach (var err in updateResult.Errors)
            {
                Console.WriteLine($"[PasswordUpdateError] User={user.UserName}, Description={err.Description}");
            }
        }

        // ログイン画面にリダイレクトする
        TempData["PasswordChangeMessage"] = "パスワード変更に成功しました。ログインしてください。";
        return RedirectToAction("Login", "Account");
    }
}
