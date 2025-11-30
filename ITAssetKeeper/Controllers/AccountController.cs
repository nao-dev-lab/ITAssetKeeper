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
    private readonly ILogger<AccountController> _logger;

    // コンストラクタ
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAccountService accountService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _logger = logger;
    }

    // GET: Account/Login
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        _logger.LogInformation("Login画面表示");

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
        _logger.LogInformation("Login(GET) 開始");

        // 入力エラーがある場合、ビューに戻す
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login(GET) 入力エラー");

            // Invalidの対象のみを取得
            var errors = ModelState.Values
                .Where(x => x.Errors.Count > 0)
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            _logger.LogWarning("Login(GET) 入力エラー内容: {Errors}", string.Join(", ", errors));

            // エラーメッセージをモデルに渡す
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            _logger.LogInformation("Login(GET) 終了");
            return View(model);
        }

        // ユーザー情報を取得
        var user = await _userManager.FindByNameAsync(model.UserName);

        // ユーザー情報が見つからなかったらログイン失敗とみなし、ログイン画面に戻す
        if (user == null)
        {
            _logger.LogWarning("Login(GET) ユーザー情報が見つからない");
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
        _logger.LogInformation("Login(GET) ログイン試行結果 Succeeded={Succeeded}", result.Succeeded);

        // ログイン処理の結果が失敗であればログイン画面に戻す
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login(GET) ログイン失敗");
            ModelState.AddModelError(string.Empty, "ログインに失敗しました");
            return View(model);
        }

        // パスワード期限が超過していないかチェック
        if (_accountService.IsPasswordExpired(user))
        {
            _logger.LogInformation("Login(GET) パスワード期限超過");
            // 超過している場合はパスワード変更画面にリダイレクト
            TempData["ErrorMessage"]
                = "パスワードの有効期限が切れています。新しいパスワードを設定してください。";
            return RedirectToAction("ChangePassword", "Account");
        }

        // ログインに成功したら、
        // returnUrlが存在し、かつローカルURLであればリダイレクト
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            _logger.LogInformation("Login(GET) ローカルURLにリダイレクト ReturnUrl={ReturnUrl}", returnUrl);
            return LocalRedirect(returnUrl);
        }

        try
        {
            // 危険 or 未指定 returnUrl は無視して固定先に飛ばす
            // AdminはDashboard、他RoleはIndex
            var redirectUrl = await _accountService.ResolveRedirectAfterLoginAsync(user);
            _logger.LogInformation("Login(GET) 固定URLにリダイレクト RedirectUrl={RedirectUrl}", redirectUrl);
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login(GET) リダイレクト先の解決中にエラー発生");
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
        _logger.LogInformation("Logout(GET) 開始");

        // ログアウト処理
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Logout(GET) ログアウト処理完了");

        // ログイン画面にリダイレクト
        return RedirectToAction("Login", "Account", new { loggedout = true });
    }

    // GET: Account/Profile
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile(string username)
    {
        _logger.LogInformation("Profile(GET) 開始");

        // ユーザー情報を取得
        var user = await _userManager.FindByNameAsync(username);
        _logger.LogInformation("Profile(GET) ユーザー情報取得結果 Found={Found}", user != null ? "Found" : "NotFound");

        // ユーザー情報が取得できなければログアウト
        if (user == null)
        {
            _logger.LogWarning("Profile(GET) ユーザー情報が見つからない");
            await _signInManager.SignOutAsync();
            TempData["FailureMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login");
        }

        // ユーザー情報からモーダルに表示するアカウント情報を取得
        var model = await _accountService.GetProfileViewModelAsync(user);
        _logger.LogInformation("Profile(GET) 完了");

        // 部分ビューにモデルを渡す
        return PartialView("_ProfilePartial", model);
    }

    // GET: Account/ChangePassword
    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        _logger.LogInformation("ChangePassword(GET) 画面表示");

        // パスワード変更画面を表示
        return View();
    }

    // POST: Account/ChangePassword
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([Bind("CurrentPassword, NewPassword, ConfirmPassword")] ChangePasswordViewModel model)
    {
        _logger.LogInformation("ChangePassword(POST) 開始");

        // 入力エラーがある場合、ビューに戻す
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ChangePassword(POST) 入力エラー");

            // Invalidの対象のみを取得
            var errors = ModelState.Values
                .Where(x => x.Errors.Count > 0)
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            _logger.LogWarning("ChangePassword(POST) 入力エラー内容: {Errors}", string.Join(", ", errors));

            // エラーメッセージをモデルに渡す
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            _logger.LogInformation("ChangePassword(POST) 終了");
            return View(model);
        }

        // ユーザーが認証されていないか、ユーザー名が取得できなければ
        // ログアウト + ログイン画面にリダイレクト
        if (!User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(User.Identity.Name))
        {
            _logger.LogWarning("ChangePassword(POST) ユーザー認証に失敗");
            await _signInManager.SignOutAsync();
            TempData["FailureMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", nameof(AccountController));
        }

        // ログイン中のユーザー名からユーザー情報を取得
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        _logger.LogInformation("ChangePassword(POST) ユーザー情報取得結果 Found={Found}", user != null ? "Found" : "NotFound");

        // ユーザー情報が取得できなかったら、
        // ログアウト処理 + ログイン画面にリダイレクトする
        if (user == null)
        {
            _logger.LogWarning("ChangePassword(POST) ユーザー情報が見つからない");
            await _signInManager.SignOutAsync();
            TempData["FailureMessage"] = "ユーザーの検証に失敗しました。ログインからやり直してください。";
            return RedirectToAction("Login", nameof(AccountController));
        }

        // 新旧パスワードが同一なら弾く
        if (model.CurrentPassword == model.NewPassword)
        {
            _logger.LogWarning("ChangePassword(POST) 新旧パスワード同一エラー");
            ModelState.AddModelError(string.Empty, "現在のパスワードと同一のパスワードに変更できません。");
            return View(model);
        }

        // パスワード変更処理の実施
        var result = await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword,
            model.NewPassword
        );
        _logger.LogInformation("ChangePassword(POST) パスワード変更結果 Succeeded={Succeeded}", result.Succeeded);

        // パスワード変更処理の結果が失敗であれば、パスワード変更画面に戻す
        if (!result.Succeeded)
        {
            _logger.LogWarning("ChangePassword(POST) パスワード変更失敗");
            
            // エラー内容をコンソールに出力、エラー理由をモデルに渡す
            foreach (var err in result.Errors)
            {
                _logger.LogInformation("ChangePassword(POST) パスワード変更エラー理由={Description}", err.Description);
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        // パスワード変更に成功したら、パスワード期限を更新
        _accountService.UpdatePasswordExpiration(user);
        var updateResult = await _userManager.UpdateAsync(user);
        _logger.LogInformation("ChangePassword(POST) パスワード期限更新結果 Succeeded={Succeeded}", updateResult.Succeeded);

        // DB側の更新に失敗していればログを出力
        if (!updateResult.Succeeded)
        {
            _logger.LogError("ChangePassword(POST) パスワード期限更新失敗");

            foreach (var err in updateResult.Errors)
            {
                _logger.LogInformation("ChangePassword(POST) パスワード期限更新エラー理由={Description}", err.Description);
            }
        }

        // ログアウト + ログイン画面にリダイレクトする
        await _signInManager.SignOutAsync();
        _logger.LogInformation("ChangePassword(POST) パスワード変更成功");
        TempData["SuccessMessage"] = "パスワード変更に成功しました。ログインしてください。";
        return RedirectToAction("Login", "Account");
    }

    // 以下、将来実装予定の機能
    //[HttpGet]
    //[Authorize(Roles = "Admin, Editor")]
    //public IActionResult UserList()
    //{
    //    return View();
    //}

    //[HttpGet]
    //[Authorize(Roles = "Admin")]
    //public IActionResult Create()
    //{
    //    return View();
    //}

    //[HttpGet]
    //[AllowAnonymous]
    //public IActionResult ForgotPassword()
    //{
    //    return View();
    //}
}
