using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CO_VM.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "AdminId", "AdminName", "Password", "Username" },
                values: new object[] { 1, "System Admin", new byte[] { 97, 100, 109, 105, 110, 49, 50, 51 }, "admin" });

            migrationBuilder.InsertData(
                table: "Centres",
                columns: new[] { "CentreID", "Address", "CentreName", "City", "ContactNumber", "State" },
                values: new object[,]
                {
                    { 1, "456 Main St", "City Health Center", "Dehradun", "0135-1234567", "Uttarakhand" },
                    { 2, "12 Hill Road", "Sunrise Clinic", "Mussoorie", "0135-7654321", "Uttarakhand" },
                    { 3, "78 Forest Lane", "Green Valley Hospital", "Nainital", "05942-246810", "Uttarakhand" },
                    { 4, "22 Blossom Street", "Rainbow MedCare", "Haridwar", "01334-135791", "Uttarakhand" }
                });

            migrationBuilder.InsertData(
                table: "Vaccines",
                columns: new[] { "VaccineID", "Description", "DosesRequired", "Image", "Manufacturer", "Price", "Stock", "VaccineName" },
                values: new object[,]
                {
                    { 1, "Oxford-AstraZeneca viral-vector COVID-19 vaccine", 2, null, "Serum Institute", 250m, 120, "Covishield" },
                    { 2, "Inactivated virus COVID-19 vaccine", 2, null, "Bharat Biotech", 300m, 90, "Covaxin" },
                    { 3, "Recombinant adenovirus vector vaccine", 2, null, "Gamaleya Institute", 500m, 60, "Sputnik V" },
                    { 4, "mRNA-1273 COVID-19 vaccine", 2, null, "Moderna Inc.", 750m, 45, "Moderna" },
                    { 5, "mRNA BNT162b2 COVID-19 vaccine", 2, null, "Pfizer & BioNTech", 780m, 50, "Pfizer-BioNTech" },
                    { 6, "Protein-subunit COVID-19 vaccine", 2, null, "Serum Institute / Novavax", 400m, 70, "Novavax" }
                });

            migrationBuilder.InsertData(
                table: "VaccineCentres",
                columns: new[] { "CentreID", "VaccineID", "DoctorName" },
                values: new object[,]
                {
                    { 1, 1, "Dr Sharma" },
                    { 2, 2, "Dr Verma" },
                    { 3, 3, "Dr Patel" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admin",
                keyColumn: "AdminId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Centres",
                keyColumn: "CentreID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "VaccineCentres",
                keyColumns: new[] { "CentreID", "VaccineID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "VaccineCentres",
                keyColumns: new[] { "CentreID", "VaccineID" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "VaccineCentres",
                keyColumns: new[] { "CentreID", "VaccineID" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Centres",
                keyColumn: "CentreID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Centres",
                keyColumn: "CentreID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Centres",
                keyColumn: "CentreID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vaccines",
                keyColumn: "VaccineID",
                keyValue: 3);
        }
    }
}
