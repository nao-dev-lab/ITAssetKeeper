using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Enums;
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
        // Deviceテーブルから全てのデータを取得する
        var devices = await _context.Devices.ToListAsync();

        // 取得したDeviceテーブルのデータから新規登録された直近5件分を取得
        // 登録日で降順にソート、先頭から5件分
        // DTO型でデータを詰める
        var recentlyAdded5 = devices
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
            })
            .ToList();

        // DeviceHistoryテーブルから直近7日分の件数を日別で取得
        // 当日から7日前までを取得し、更新日で降順にソート
        // 更新日でグルーピングし、件数を取得する
        var historyLast7days = _context.DeviceHistory
            .Where(x => x.UpdatedAt >= DateTime.Now.AddDays(-7))
            .OrderByDescending(x => x.UpdatedAt)
            .GroupBy(g => g.UpdatedAt.Date)
            .Select(g => new DeviceHistoryLast7DaysDto
            {
                Count = g.Count(),
                DisplayDate = g.Key.ToString("MM/dd")
            });

        // 取得したDeviceテーブルのデータから機器の状態の件数を取得
        var status = devices.Select(x => x.Status);

        // DashboardAdminViewModelのインスタンス生成
        // StatusCountを該当のDTO型で初期化しておく
        var adminVM = new DashboardAdminViewModel
        {
            StatusCount = new DeviceStatusCountDto()
        };

        // 取得した情報をViewModelに詰めていく
        // 最近追加された機器一覧(5件)
        adminVM.RecentlyAddedDevices = recentlyAdded5;

        // 過去7日間の更新履歴件数
        adminVM.HistoryLast7Days = await historyLast7days.ToListAsync();

        // 状態別の台数集計
        adminVM.StatusCount.WorkingCount = status.Count(x => x == DeviceStatus.Working.ToString());
        adminVM.StatusCount.ReserveCount = status.Count(x => x == DeviceStatus.Reserve.ToString());
        adminVM.StatusCount.BrokenCount = status.Count(x => x == DeviceStatus.Broken.ToString());
        adminVM.StatusCount.ScheduledForDisposalCount = status.Count(x => x == DeviceStatus.ScheduledForDisposal.ToString());
        adminVM.StatusCount.DisposedCount = status.Count(x => x == DeviceStatus.Disposed.ToString());

        // 実行結果をビューに返す
        return View(adminVM);
    }
}
