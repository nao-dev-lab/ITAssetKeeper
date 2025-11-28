using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class _20251128_SyncDeviceHistoryRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "ManagementId",
            //    table: "DeviceHistories",
            //    newName: "ManagementIdAtHistory");

            //migrationBuilder.RenameIndex(
            //    name: "IX_DeviceHistories_ManagementId_HistoryId",
            //    table: "DeviceHistories",
            //    newName: "IX_DeviceHistories_ManagementIdAtHistory_HistoryId");

            //migrationBuilder.AddColumn<string>(
            //    name: "CategoryAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ChangeType",
            //    table: "DeviceHistories",
            //    type: "nvarchar(20)",
            //    maxLength: 20,
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CreatedAtHistory",
            //    table: "DeviceHistories",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "DeletedAtHistory",
            //    table: "DeviceHistories",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "DeletedByAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "HostNameAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsDeletedAtHistory",
            //    table: "DeviceHistories",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<string>(
            //    name: "LocationAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(200)",
            //    maxLength: 200,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "MemoAtHistory",
            //    table: "DeviceHistories",
            //    type: "NVARCHAR(MAX)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ModelNumberAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "PurchaseDateAtHistory",
            //    table: "DeviceHistories",
            //    type: "datetime2",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "PurposeAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "SerialNumberAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "StatusAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(20)",
            //    maxLength: 20,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "UserNameAtHistory",
            //    table: "DeviceHistories",
            //    type: "nvarchar(100)",
            //    maxLength: 100,
            //    nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CategoryAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "ChangeType",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "CreatedAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "DeletedAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "DeletedByAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "HostNameAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "IsDeletedAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "LocationAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "MemoAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "ModelNumberAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "PurchaseDateAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "PurposeAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "SerialNumberAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "StatusAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.DropColumn(
            //    name: "UserNameAtHistory",
            //    table: "DeviceHistories");

            //migrationBuilder.RenameColumn(
            //    name: "ManagementIdAtHistory",
            //    table: "DeviceHistories",
            //    newName: "ManagementId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_DeviceHistories_ManagementIdAtHistory_HistoryId",
            //    table: "DeviceHistories",
            //    newName: "IX_DeviceHistories_ManagementId_HistoryId");

            //migrationBuilder.AddColumn<string>(
            //    name: "AfterValue",
            //    table: "DeviceHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "BeforeValue",
            //    table: "DeviceHistories",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ChangeField",
            //    table: "DeviceHistories",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.CreateTable(
            //    name: "DeviceHistories_New",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryAtHistory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        ChangeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        CreatedAtHistory = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        DeletedAtHistory = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        DeletedByAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        HistoryId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        HostNameAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        IsDeletedAtHistory = table.Column<bool>(type: "bit", nullable: false),
            //        LocationAtHistory = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            //        ManagementIdAtHistory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        MemoAtHistory = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
            //        ModelNumberAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        PurchaseDateAtHistory = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        PurposeAtHistory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        SerialNumberAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        StatusAtHistory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        UserNameAtHistory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DeviceHistories_New", x => x.Id);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_DeviceHistories_New_ManagementIdAtHistory_HistoryId",
            //    table: "DeviceHistories_New",
            //    columns: new[] { "ManagementIdAtHistory", "HistoryId" },
            //    unique: true);
        }
    }
}
