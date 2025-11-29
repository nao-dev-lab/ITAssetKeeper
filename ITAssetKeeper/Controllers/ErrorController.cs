using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace ITAssetKeeper.Controllers;

public class ErrorController : Controller
{
    // 400番台エラー (404, 403, 401)
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusHandler(int statusCode)
    {
        switch (statusCode)
        {
            case 404:
                return View("404");

            case 403:
                return View("403");

            case 401:
                return View("401");

            default:
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
        // _logger.LogError(exceptionFeature?.Error, "Unhandled exception occurred at {Path}", exceptionFeature?.Path);

        // 500エラービューを返す
        return View("500");
    }
}
