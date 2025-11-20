using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class _251120_RenameDeviceSequenceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceSequence",
                table: "DeviceSequence");

            migrationBuilder.RenameTable(
                name: "DeviceSequence",
                newName: "DeviceSequences");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceSequences",
                table: "DeviceSequences",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceSequences",
                table: "DeviceSequences");

            migrationBuilder.RenameTable(
                name: "DeviceSequences",
                newName: "DeviceSequence");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceSequence",
                table: "DeviceSequence",
                column: "Id");
        }
    }
}
