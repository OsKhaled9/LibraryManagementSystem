using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Readify_Library.Migrations
{
    /// <inheritdoc />
    public partial class somechangesss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "08505dc7-49ef-4d94-98dd-3b26e6af79f9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11f9f52c-9bcf-4e86-bb8a-fbadf95f1009");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "437f2353-acea-40bd-b127-151a7e7aec35", null, "Admin", "ADMIN" },
                    { "f7ca110d-2a9b-4ea0-9e19-dbd2e90a17c8", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "437f2353-acea-40bd-b127-151a7e7aec35");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f7ca110d-2a9b-4ea0-9e19-dbd2e90a17c8");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "08505dc7-49ef-4d94-98dd-3b26e6af79f9", null, "Admin", "ADMIN" },
                    { "11f9f52c-9bcf-4e86-bb8a-fbadf95f1009", null, "User", "USER" }
                });
        }
    }
}
