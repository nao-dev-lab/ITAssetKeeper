using ITAssetKeeper.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Data;

// DbContextクラス
public class ITAssetKeeperDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    ///////////////////////////////////////////
    // データベースのテーブル名のプロパティを用意
    public DbSet<Device> Devices { get; set; }

    public DbSet<DeviceHistory> DeviceHistories { get; set; }

    public DbSet<DeviceHistorySequence> DeviceHistorySequences { get; set; }
    public DbSet<DeviceSequence> DeviceSequences { get; set; }

    // コンストラクタ
    public ITAssetKeeperDbContext(DbContextOptions<ITAssetKeeperDbContext> options)
        : base(options)
    {
    }

    ///////////////////////////////////////////
    // DB構成を設定
    // テーブル名とIndex設定を実施
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Devicesテーブル
        modelBuilder.Entity<Device>()
            .HasQueryFilter(b => !b.IsDeleted)
            .ToTable("Devices")
            .HasIndex(b => b.ManagementId)
            .IsUnique();

        // DeviceHistoriesテーブル
        modelBuilder.Entity<DeviceHistory>()
            .ToTable("DeviceHistories")
            .HasIndex(b => new { b.ManagementIdAtHistory, b.HistoryId })
            .IsUnique();

        // DeviceSequenceテーブル
        modelBuilder.Entity<DeviceSequence>()
            .ToTable("DeviceSequences")
            .HasData(new DeviceSequence
            {
                Id = 1,
                LastUsedNumber = 0
            });

        // DeviceHistorySequenceテーブル
        modelBuilder.Entity<DeviceHistorySequence>()
            .ToTable("DeviceHistorySequences")
            .HasData(new DeviceHistorySequence
            {
                Id = 1,
                LastUsedNumber = 0
            });

        base.OnModelCreating(modelBuilder);
    }
}
