using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class _251117_RenameUpdateAtToUpdatedAtOnDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Devices",
                newName: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Devices",
                newName: "UpdateAt");
        }
    }
}
