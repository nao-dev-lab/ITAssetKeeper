using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class _251120_AddDeviceSequenceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceSequence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUsedNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSequence", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DeviceSequence",
                columns: new[] { "Id", "LastUsedNumber" },
                values: new object[] { 1, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceSequence");
        }
    }
}
