using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.ViewModels.Account;
using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

// Login用コントローラー
public class AccountController : Controller
{
    // フィールド
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccountService _accountService;

    // コンストラクタ
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAccountService accountService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
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
    public async Task<IActionResult> Login(
        [Bind("UserName, Password, RememberMe")] LoginViewModel model,
        string? returnUrl = null)
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
        if (_accountService.IsPasswordExpired(user))
        {
            // 超過している場合はパスワード変更画面にリダイレクト
            TempData["ErrorMessage"]
                = "パスワードの有効期限が切れています。新しいパスワードを設定してください。";
            return RedirectToAction("ChangePassword", "Account");
        }

        // ログインに成功したら、
        // returnUrlが存在し、かつローカルURLであればリダイレクト
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        try
        {
            // 危険 or 未指定 returnUrl は無視して固定先に飛ばす
            // AdminはDashboard、他RoleはIndex
            var redirectUrl = await _accountService.ResolveRedirectAfterLoginAsync(user);
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "ログイン後処理で例外発生");
            TempData["ErrorMessage"] = "ログイン後の処理中にエラーが発生しました。";
            return RedirectToAction("Login", "Account");
        }
    }

    // POST: Account/Logout
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // ログアウト処理
        await _signInManager.SignOutAsync();

        // ログイン画面にリダイレクト
        return RedirectToAction("Login", "Account", new { loggedout = true });
    }

    // GET: Account/Profile
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile(string username)
    {
        // ユーザー情報を取得
        var user = await _userManager.FindByNameAsync(username);

        // ユーザー情報が取得できなければログアウト
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            TempData["FailureMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login");
        }

        // ユーザー情報からモーダルに表示するアカウント情報を取得
        var model = await _accountService.GetProfileViewModelAsync(user);

        // 部分ビューにモデルを渡す
        return PartialView("_ProfilePartial", model);
    }

    // GET: Account/ChangePassword
    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        // パスワード変更画面を表示
        return View();
    }

    // POST: Account/ChangePassword
    [HttpPost]
    [Authorize]
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
            TempData["FailureMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", nameof(AccountController));
        }

        // ログイン中のユーザー名からユーザー情報を取得
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        // ユーザー情報が取得できなかったら、
        // ログアウト処理 + ログイン画面にリダイレクトする
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            TempData["FailureMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
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

        // パスワード変更に成功したら、パスワード期限を更新
        _accountService.UpdatePasswordExpiration(user);
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
        TempData["SuccessMessage"] = "パスワード変更に成功しました。ログインしてください。";
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Editor")]
    public IActionResult UserList()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }
}
