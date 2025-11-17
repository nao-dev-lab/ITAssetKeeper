using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class _251117_AddCreatedFieldsToDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Devices",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Devices");
        }
    }
}
