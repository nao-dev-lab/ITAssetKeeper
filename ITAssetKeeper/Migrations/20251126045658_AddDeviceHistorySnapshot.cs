using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceHistorySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtHistory",
                table: "DeviceHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtHistory",
                table: "DeviceHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostNameAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedAtHistory",
                table: "DeviceHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LocationAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemoAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelNumberAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDateAtHistory",
                table: "DeviceHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PurposeAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SerialNumberAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtHistory",
                table: "DeviceHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameAtHistory",
                table: "DeviceHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "CreatedAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "DeletedAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "DeletedByAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "HostNameAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "IsDeletedAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "LocationAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "MemoAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "ModelNumberAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "PurchaseDateAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "PurposeAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "SerialNumberAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "StatusAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "UpdatedAtHistory",
                table: "DeviceHistories");

            migrationBuilder.DropColumn(
                name: "UserNameAtHistory",
                table: "DeviceHistories");
        }
    }
}
