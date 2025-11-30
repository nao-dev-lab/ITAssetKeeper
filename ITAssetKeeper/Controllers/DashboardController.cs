using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;
    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    // GET: Dashboard/Admin
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Admin()
    {
        _logger.LogInformation("Admin Dashboard 開始");

        // AdminのDashboard用のデータを取得
        var model = await _dashboardService.GetAdminDashboardDataAsync();

        // 実行結果をビューに返す
        _logger.LogInformation("Admin Dashboard 終了");
        return View(model);
    }
}
