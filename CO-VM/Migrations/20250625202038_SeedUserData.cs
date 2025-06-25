using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CO_VM.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "AadhaarNo", "Address", "City", "DOB", "Email", "FullName", "Gender", "Password", "PhoneNumber", "Security_answer", "Security_question", "State", "Username" },
                values: new object[] { 1, "123456789012", "123 Main Street", "Delhi", new DateOnly(1995, 5, 15), "user1@example.com", "JohnDoe", "Male", new byte[] { 49, 50, 51 }, "9876543210", "Blue", "What is your favorite color?", "Delhi", "jd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);
        }
    }
}
