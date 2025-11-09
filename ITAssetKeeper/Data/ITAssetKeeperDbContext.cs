using ITAssetKeeper.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Data
{
    // DbContextクラス
    public class ITAssetKeeperDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        ///////////////////////////////////////////
        // データベースのテーブル名のプロパティを用意
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceHistory> DeviceHistory { get; set; }

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
                .ToTable("Devices")
                .HasIndex(b => b.ManagementId)
                .IsUnique();

            // DeviceHistoriesテーブル
            modelBuilder.Entity<DeviceHistory>()
                .ToTable("DeviceHistories")
                .HasIndex(b => new { b.ManagementId, b.HistoryId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        ///////////////////////////////////////////
        // DBへの新規登録時に Device の CreatedAt に自動で現在日時を設定する
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Deviceテーブルの新規登録となる対象のデータを取得
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is Device);

            // CreatedAt に 現在日時を入れていく
            foreach (var entry in entries)
            {
                ((Device)entry.Entity).CreatedAt = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
