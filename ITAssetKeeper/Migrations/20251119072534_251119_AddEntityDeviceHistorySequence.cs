using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITAssetKeeper.Migrations
{
    /// <inheritdoc />
    public partial class _251119_AddEntityDeviceHistorySequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceHistorySequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUsedNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceHistorySequences", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DeviceHistorySequences",
                columns: new[] { "Id", "LastUsedNumber" },
                values: new object[] { 1, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceHistorySequences");
        }
    }
}
