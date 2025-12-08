using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Services;

// Dashboard用Service
public class DashboardService : IDashboardService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(ITAssetKeeperDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Admin用Dashboardのデータを取得し、ビューモデルを返す
    public async Task<DashboardAdminViewModel?> GetAdminDashboardDataAsync()
    {
        _logger.LogInformation("GetAdminDashboardDataAsync 開始");

        // 取得したDeviceテーブルのデータから新規登録された直近5件分を取得
        // 登録日で降順にソート、先頭から5件分
        // DTO型でデータを詰める
        var recentlyAdded5 = await _context.Devices
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new DeviceRecentlyAddedDto
            {
                // Category,Statusは Helperを使って日本語表示名に変換
                // HostName,UserNameは、nullなら"-"を表示
                CreatedAt = x.CreatedAt.ToString("yyyy/MM/dd"),
                ManagementId = x.ManagementId,
                Category = EnumDisplayHelper.ResolveDisplayName<DeviceCategory>(x.Category),
                ModelNumber = x.ModelNumber,
                HostName = x.HostName ?? "-",
                Location = x.Location,
                UserName = x.UserName ?? "-",
                Status = EnumDisplayHelper.ResolveDisplayName<DeviceStatus>(x.Status)
            })
            .ToListAsync();
        _logger.LogInformation("GetAdminDashboardDataAsync 直近5件の新規登録機器データ取得完了 件数={Count}", recentlyAdded5.Count);

        // DeviceHistoryテーブルから直近7日分の件数を日別で取得
        // 当日から7日前までを取得し、更新日で降順にソート
        // 更新日でグルーピングし、件数を取得する
        var today = DateTime.Today;
        var historyLast7days = await _context.DeviceHistories
            .AsNoTracking()
            .Where(x => x.UpdatedAt >= today.AddDays(-7))
            .OrderByDescending(x => x.UpdatedAt)
            .GroupBy(g => g.UpdatedAt.Date)
            .Select(g => new DeviceHistoryLast7DaysDto
            {
                Count = g.Count(),
                DisplayDate = g.Key.ToString("MM/dd")
            })
            .ToListAsync();
        _logger.LogInformation("GetAdminDashboardDataAsync 直近7日間の更新履歴データ取得完了 件数={Count}", historyLast7days.Count);

        // 取得したDeviceテーブルのデータから機器の状態の件数を取得
        var status = _context.Devices
            .AsNoTracking()
            .Select(x => x.Status);
        _logger.LogInformation("GetAdminDashboardDataAsync 機器状態別の台数集計データ取得完了 件数={Count}", status.Count());

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
        adminVM.HistoryLast7Days = historyLast7days;

        // 状態別の台数集計
        adminVM.StatusCount.ActiveCount = status.Count(x => x == DeviceStatus.Active.ToString());
        adminVM.StatusCount.SpareCount = status.Count(x => x == DeviceStatus.Spare.ToString());
        adminVM.StatusCount.BrokenCount = status.Count(x => x == DeviceStatus.Broken.ToString());
        adminVM.StatusCount.RetiringCount = status.Count(x => x == DeviceStatus.Retiring.ToString());
        adminVM.StatusCount.RetiredCount = status.Count(x => x == DeviceStatus.Retired.ToString());

        _logger.LogInformation("GetAdminDashboardDataAsync 終了");
        return adminVM;
    }
}
