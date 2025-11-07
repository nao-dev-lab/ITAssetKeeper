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
    }
}
