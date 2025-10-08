using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Readify_Library.Migrations
{
    /// <inheritdoc />
    public partial class ConvertEnumtoString_And_SeedDataInUersTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "UsersTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Borrowings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.InsertData(
                table: "UsersTypes",
                columns: new[] { "Id", "ExtraBooks", "ExtraDays", "ExtraPenalty", "TypeName" },
                values: new object[,]
                {
                    { 1, 0, 0, 0.00m, "Normal" },
                    { 2, 2, 7, 10.00m, "Premium" },
                    { 3, 5, 10, 5.00m, "VIP" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsersTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UsersTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UsersTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<byte>(
                name: "TypeName",
                table: "UsersTypes",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Borrowings",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
