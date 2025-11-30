using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace ITAssetKeeper.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    // 400番台エラー (404, 403, 401)
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusHandler(int statusCode)
    {
        _logger.LogWarning("HTTPエラー発生: ステータスコード={StatusCode}, パス={Path}",
            statusCode, HttpContext.Features.Get<IStatusCodeReExecuteFeature>()?.OriginalPath);

        switch (statusCode)
        {
            case 404:
                _logger.LogInformation("404エラー: 存在しないページへのアクセスがありました。");
                return View("404");

            case 403:
                _logger.LogInformation("403エラー: アクセス権限のないページへのアクセスがありました。");
                return View("403");

            case 401:
                _logger.LogInformation("401エラー: 認証が必要なページへのアクセスがありました。");
                return View("401");

            default:
                _logger.LogInformation("{StatusCode}エラー: 汎用エラー画面を表示します。", statusCode);
                // それ以外のステータスでも汎用エラー画面を使う
                return View("Error");
        } 
    }

    // 500 (未処理例外)
    [Route("Error/500")]
    public IActionResult Error505()
    {
        // 詳細な例外情報を取得
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        // ログ出力などの処理
         _logger.LogError("500エラー: サーバー内部で未処理例外が発生しました。 パス={Path}", exceptionDetails?.Path);

        // 500エラービューを返す
        return View("500");
    }
}
