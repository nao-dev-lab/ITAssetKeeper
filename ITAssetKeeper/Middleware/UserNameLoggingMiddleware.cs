using Serilog.Context;

namespace ITAssetKeeper.Middleware;

// HTTPリクエストごとに UserName を Serilog の LogContext に追加するミドルウェア
public class UserNameLoggingMiddleware
{
    // 次のミドルウェアへの参照
    private readonly RequestDelegate _next;

    public UserNameLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // ミドルウェアの実行ロジック
    public async Task InvokeAsync(HttpContext context)
    {
        // Identity が存在しない場合は Anonymous とする
        string userName = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "Anonymous";

        // UserName を LogContext にプッシュ（ログ出力されるたびに自動付与される）
        using (LogContext.PushProperty("UserName", userName))
        {
            await _next(context);
        }
    }
}
