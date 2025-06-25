using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CO_VM.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdminName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: true),
                    Username = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<byte[]>(type: "BLOB", fixedLength: true, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__719FE488C4A3B69B", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "Centres",
                columns: table => new
                {
                    CentreID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CentreName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", unicode: false, maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "TEXT", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Centres__A2E8F5FAB31DE283", x => x.CentreID);
                });

            migrationBuilder.CreateTable(
                name: "Relations",
                columns: table => new
                {
                    RelationID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RelationType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Relation__E2DA1695EDAF63EE", x => x.RelationID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<byte[]>(type: "BLOB", fixedLength: true, maxLength: 64, nullable: false),
                    AadhaarNo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 12, nullable: false),
                    Gender = table.Column<string>(type: "TEXT", unicode: false, maxLength: 10, nullable: false),
                    DOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", unicode: false, maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Security_question = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    Security_answer = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC9C69E1F5", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Vaccines",
                columns: table => new
                {
                    VaccineID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VaccineName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    DosesRequired = table.Column<int>(type: "INTEGER", nullable: true),
                    Stock = table.Column<int>(type: "INTEGER", nullable: true, defaultValue: 0),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vaccines__45DC68E955D14FFD", x => x.VaccineID);
                });

            migrationBuilder.CreateTable(
                name: "Family",
                columns: table => new
                {
                    FamilyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    DOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", unicode: false, maxLength: 10, nullable: false),
                    AadhaarNo = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false),
                    RelationID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Family__41D82F4BF856D4B5", x => x.FamilyID);
                    table.ForeignKey(
                        name: "FK_Relations_Family",
                        column: x => x.RelationID,
                        principalTable: "Relations",
                        principalColumn: "RelationID");
                    table.ForeignKey(
                        name: "FK__Family__UserID__59FA5E80",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "VaccinationFeedback",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    VaccineID = table.Column<int>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", unicode: false, maxLength: 15, nullable: false),
                    Rating = table.Column<byte>(type: "INTEGER", nullable: true),
                    Feedback = table.Column<string>(type: "TEXT", unicode: false, maxLength: 2000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vaccinat__6A4BEDD63879BD25", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedback_User",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Feedback_Vaccine",
                        column: x => x.VaccineID,
                        principalTable: "Vaccines",
                        principalColumn: "VaccineID");
                });

            migrationBuilder.CreateTable(
                name: "VaccineCentres",
                columns: table => new
                {
                    VaccineID = table.Column<int>(type: "INTEGER", nullable: false),
                    CentreID = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaccineC__FFF2E7B6F7A43965", x => new { x.VaccineID, x.CentreID });
                    table.ForeignKey(
                        name: "FK__VaccineCe__Centr__2A164134",
                        column: x => x.CentreID,
                        principalTable: "Centres",
                        principalColumn: "CentreID");
                    table.ForeignKey(
                        name: "FK__VaccineCe__Vacci__29221CFB",
                        column: x => x.VaccineID,
                        principalTable: "Vaccines",
                        principalColumn: "VaccineID");
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    SlotID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VaccineID = table.Column<int>(type: "INTEGER", nullable: false),
                    CentreID = table.Column<int>(type: "INTEGER", nullable: false),
                    SlotDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    SlotTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    UserID = table.Column<int>(type: "INTEGER", nullable: true),
                    FamilyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Slots__0A124A4F8B9DF5B9", x => x.SlotID);
                    table.ForeignKey(
                        name: "FK_Bookings_Family",
                        column: x => x.FamilyId,
                        principalTable: "Family",
                        principalColumn: "FamilyID");
                    table.ForeignKey(
                        name: "FK_Bookings_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Slots__CentreID__619B8048",
                        column: x => x.CentreID,
                        principalTable: "Centres",
                        principalColumn: "CentreID");
                    table.ForeignKey(
                        name: "FK__Slots__VaccineID__60A75C0F",
                        column: x => x.VaccineID,
                        principalTable: "Vaccines",
                        principalColumn: "VaccineID");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    FamilyID = table.Column<int>(type: "INTEGER", nullable: true),
                    SlotID = table.Column<int>(type: "INTEGER", nullable: false),
                    VaccineID = table.Column<int>(type: "INTEGER", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Status = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false),
                    DoseNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    VaccinatedOn = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    PaymentMode = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bookings__3DE0C227F2B69FFC", x => x.BookID);
                    table.ForeignKey(
                        name: "FK__Bookings__Family__68487DD7",
                        column: x => x.FamilyID,
                        principalTable: "Family",
                        principalColumn: "FamilyID");
                    table.ForeignKey(
                        name: "FK__Bookings__SlotID__693CA210",
                        column: x => x.SlotID,
                        principalTable: "Slots",
                        principalColumn: "SlotID");
                    table.ForeignKey(
                        name: "FK__Bookings__UserID__6754599E",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__Bookings__Vaccin__6A30C649",
                        column: x => x.VaccineID,
                        principalTable: "Vaccines",
                        principalColumn: "VaccineID");
                });

            migrationBuilder.CreateTable(
                name: "VaccineCertificate",
                columns: table => new
                {
                    CertificateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookID = table.Column<int>(type: "INTEGER", nullable: false),
                    Certificate = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CertificateDate = table.Column<DateOnly>(type: "TEXT", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaccineC__BBF8A7E107AB835A", x => x.CertificateID);
                    table.ForeignKey(
                        name: "FK__VaccineCe__BookI__6E01572D",
                        column: x => x.BookID,
                        principalTable: "Bookings",
                        principalColumn: "BookID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Admin__536C85E4B791D256",
                table: "Admin",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FamilyID",
                table: "Bookings",
                column: "FamilyID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SlotID",
                table: "Bookings",
                column: "SlotID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserID",
                table: "Bookings",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VaccineID",
                table: "Bookings",
                column: "VaccineID");

            migrationBuilder.CreateIndex(
                name: "IX_Family_RelationID",
                table: "Family",
                column: "RelationID");

            migrationBuilder.CreateIndex(
                name: "IX_Family_UserID",
                table: "Family",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Family__6F3B29F66F6ED6FF",
                table: "Family",
                column: "AadhaarNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Relation__836CC666742B04CD",
                table: "Relations",
                column: "RelationType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_CentreID",
                table: "Slots",
                column: "CentreID");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_FamilyId",
                table: "Slots",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_UserID",
                table: "Slots",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_VaccineID",
                table: "Slots",
                column: "VaccineID");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__536C85E421305566",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Users__6F3B29F6E278ACCC",
                table: "Users",
                column: "AadhaarNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationFeedback_UserID",
                table: "VaccinationFeedback",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationFeedback_VaccineID",
                table: "VaccinationFeedback",
                column: "VaccineID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccineCentres_CentreID",
                table: "VaccineCentres",
                column: "CentreID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccineCertificate_BookID",
                table: "VaccineCertificate",
                column: "BookID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "VaccinationFeedback");

            migrationBuilder.DropTable(
                name: "VaccineCentres");

            migrationBuilder.DropTable(
                name: "VaccineCertificate");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "Family");

            migrationBuilder.DropTable(
                name: "Centres");

            migrationBuilder.DropTable(
                name: "Vaccines");

            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
