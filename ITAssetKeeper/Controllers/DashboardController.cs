using ITAssetKeeper.Data;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

public class DashboardController : Controller
{
    private readonly ITAssetKeeperDbContext _context;

    // DbContextをコンストラクタ注入
    public DashboardController(ITAssetKeeperDbContext context)
    {
        _context = context;
    }

    // GET: Dashboard/Admin
    [HttpGet]
    public IActionResult Admin()
    {
        return View();
    }
}
