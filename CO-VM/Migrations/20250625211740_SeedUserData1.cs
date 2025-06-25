using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CO_VM.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserData1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Relations",
                columns: new[] { "RelationID", "RelationType" },
                values: new object[,]
                {
                    { 1, "Father" },
                    { 2, "Mother" },
                    { 3, "Son" },
                    { 4, "Daughter" },
                    { 5, "Brother" },
                    { 6, "Sister" },
                    { 7, "Husband" },
                    { 8, "Wife" },
                    { 9, "Grandfather" },
                    { 10, "Grandmother" },
                    { 11, "Uncle" },
                    { 12, "Aunt" },
                    { 13, "Cousin" },
                    { 14, "Other" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "RelationID",
                keyValue: 14);
        }
    }
}
