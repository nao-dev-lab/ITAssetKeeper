using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class CreateDeviceHistoriesNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ManagementIdAtHistory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CategoryAtHistory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurposeAtHistory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModelNumberAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNumberAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HostNameAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LocationAtHistory = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserNameAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StatusAtHistory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MemoAtHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDateAtHistory = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtHistory = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeletedAtHistory = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtHistory = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceHistories_ManagementIdAtHistory_HistoryId",
                table: "DeviceHistories",
                columns: new[] { "ManagementIdAtHistory", "HistoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceHistories");
        }
    }
}
