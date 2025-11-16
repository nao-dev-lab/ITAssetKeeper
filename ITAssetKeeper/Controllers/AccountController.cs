using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ITAssetKeeper.Constants;

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
            // Invalidの対象のみを取得
            var errors = ModelState.Values
                .Where(x => x.Errors.Count > 0)
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            // エラーメッセージをモデルに渡す
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

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
            // 超過している場合はパスワード変更画面にリダイレクト
            TempData[ChangePWTemp.PasswordExpiredMessage.ToString()]
                = "パスワードの有効期限が切れています。新しいパスワードを設定してください。";
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
        // パスワード変更画面を表示
        return View();
    }

    // POST: Account/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([Bind("CurrentPassword, NewPassword, ConfirmPassword")] ChangePasswordViewModel model)
    {
        // 入力エラーがある場合、ビューに戻す
        if (!ModelState.IsValid)
        {
            // Invalidの対象のみを取得
            var errors = ModelState.Values
                .Where(x => x.Errors.Count > 0)
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            // エラーメッセージをモデルに渡す
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // ユーザーが認証されていないか、ユーザー名が取得できなければ
        // ログアウト + ログイン画面にリダイレクト
        if (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(User.Identity.Name))
        {
            await _signInManager.SignOutAsync();
            TempData[ChangePWTemp.PWChangeFailMessage.ToString()] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", nameof(AccountController));
        }

        // ログイン中のユーザー名からユーザー情報を取得
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        // ユーザー情報が取得できなかったら、
        // ログアウト処理 + ログイン画面にリダイレクトする
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            TempData[ChangePWTemp.PWChangeFailMessage.ToString()] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", nameof(AccountController));
        }

        // 新旧パスワードが同一なら弾く
        if (model.CurrentPassword == model.NewPassword)
        {
            ModelState.AddModelError(string.Empty, "現在のパスワードと同一のパスワードに変更できません。");
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
            // エラー内容をコンソールに出力、エラー理由をモデルに渡す
            foreach (var err in result.Errors)
            {
                Console.WriteLine($"[PasswordUpdateError] User={user.UserName}, Description={err.Description}");
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        // パスワード変更に成功したら、
        // パスワード期限を更新
        user.PasswordExpirationDate = DateTime.Now.AddDays(ApplicationIdentityConstants.PASSWORD_VALID_DAYS);
        var updateResult = await _userManager.UpdateAsync(user);

        // DB側の更新に失敗していればコンソールにログを出力
        if (!updateResult.Succeeded)
        {
            foreach (var err in updateResult.Errors)
            {
                Console.WriteLine($"[PasswordUpdateError] User={user.UserName}, Description={err.Description}");
            }
        }

        // ログアウト + ログイン画面にリダイレクトする
        await _signInManager.SignOutAsync();
        TempData[ChangePWTemp.PWChangeSuccessMessage.ToString()] = "パスワード変更に成功しました。ログインしてください。";
        return RedirectToAction("Login", "Account");
    }
}
