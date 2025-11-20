using ITAssetKeeper.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ITAssetKeeper.Data
{
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
                .HasIndex(b => new { b.ManagementId, b.HistoryId })
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

        ///////////////////////////////////////////
        // Devicesへの新規登録・更新・削除(フラグ更新)時、
        // DeviceHistories の更新時に、自動で現在日時を設定する
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 現在日時を取得
            var now = DateTime.Now;

            // DeviceテーブルのEntryを取得
            var deviceEntries = ChangeTracker.Entries<Device>();

            foreach (var entry in deviceEntries)
            {
                // 新規登録なら、登録日と更新日を現在日時で設定
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                // 変更であれば、更新日を現在日時で設定
                // ソフトデリートの場合は、削除日も現在日時で設定
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;

                    if (entry.Entity.IsDeleted == true && entry.Property(nameof(Device.IsDeleted)).IsModified)
                    {
                        entry.Entity.DeletedAt = now;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
