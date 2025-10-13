using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Readify_Library.Migrations
{
    /// <inheritdoc />
    public partial class seedRolesAgain3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5608e3e6-5776-4f1d-b319-58c7661a3fb3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b53fd2be-c0f0-4777-96fa-82b830a7f2d0");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "08505dc7-49ef-4d94-98dd-3b26e6af79f9", null, "Admin", "ADMIN" },
                    { "11f9f52c-9bcf-4e86-bb8a-fbadf95f1009", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "08505dc7-49ef-4d94-98dd-3b26e6af79f9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11f9f52c-9bcf-4e86-bb8a-fbadf95f1009");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5608e3e6-5776-4f1d-b319-58c7661a3fb3", null, "Admin", null },
                    { "b53fd2be-c0f0-4777-96fa-82b830a7f2d0", null, "User", null }
                });
        }
    }
}
