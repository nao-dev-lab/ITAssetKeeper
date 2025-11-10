using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Enum;
using ITAssetKeeper.Models.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Admin()
    {
        // Deviceテーブルから新規登録された直近5件分を取得
        // 登録日で降順にソート、先頭から5件分
        // DTO型でデータを詰める
        var recentlyAdded5 = _context.Devices
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new DeviceRecentlyAddedDto
            {
                DisplayDate = x.CreatedAt.ToString("yyyy/MM/dd"),
                ManagementId = x.ManagementId,
                Category = x.Category,
                ModelNumber = x.ModelNumber,
                HostName = x.HostName,
                Location = x.Location,
                UserName = x.UserName,
                Status = x.Status
            });

        // DeviceHistoryテーブルから直近7日分の件数を日別で取得
        // 当日から7日前までを取得し、更新日で降順にソート
        // 更新日でグルーピングし、件数を取得する
        var historyLast7days = _context.DeviceHistory
            .Where(x => x.UpdatedAt >= DateTime.Now.AddDays(-7))
            .OrderByDescending(x => x.UpdatedAt.Date)
            .GroupBy(g => g.UpdatedAt.Date)
            .Select(g => new DeviceHistoryLast7DaysDto
            {
                Count = g.Count(),
                DisplayDate = g.Key.ToString("yyyy/MM/dd")
            });

        // Deviceテーブルから機器の状態の件数を取得
        var status = _context.Devices.Select(x => x.Status);

        // DashboardAdminViewModelのインスタンス生成
        // StatusCountを該当のDTO型で初期化しておく
        var adminVM = new DashboardAdminViewModel
        {
            StatusCount = new DeviceStatusCountDto()
        };

        // 取得した情報をViewModelに詰めていく
        // 最近追加された機器一覧(5件)
        adminVM.RecentlyAddedDevices = await recentlyAdded5.ToListAsync();

        // 過去7日間の更新履歴件数
        adminVM.HistoryLast7Days = await historyLast7days.ToListAsync();

        // 状態別の台数集計
        adminVM.StatusCount.WorkingCount = await status.CountAsync(x => x == DeviceStatus.Working.ToString());
        adminVM.StatusCount.ReserveCount = await status.CountAsync(x => x == DeviceStatus.Reserve.ToString());
        adminVM.StatusCount.BrokenCount = await status.CountAsync(x => x == DeviceStatus.Broken.ToString());
        adminVM.StatusCount.ScheduledForDisposalCount = await status.CountAsync(x => x == DeviceStatus.ScheduledForDisposal.ToString());
        adminVM.StatusCount.DisposedCount = await status.CountAsync(x => x == DeviceStatus.Disposed.ToString());

        // 実行結果をビューに返す
        return View(adminVM);
    }
}
