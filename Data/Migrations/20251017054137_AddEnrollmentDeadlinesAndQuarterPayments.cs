using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEnrollmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentDeadlinesAndQuarterPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuarterPaymentRequirementId",
                table: "PromissoryNotes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EnrollmentPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Semester = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OpenDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentPeriods_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuarterPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Quarter = table.Column<int>(type: "INTEGER", nullable: false),
                    Semester = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentDeadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuarterPeriods_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuarterPaymentRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    QuarterPeriodId = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalBalanceAtQuarterStart = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequiredPaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualPaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeetsRequirement = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasPromissoryNote = table.Column<bool>(type: "INTEGER", nullable: false),
                    PromissoryNoteId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterPaymentRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuarterPaymentRequirements_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuarterPaymentRequirements_PromissoryNotes_PromissoryNoteId",
                        column: x => x.PromissoryNoteId,
                        principalTable: "PromissoryNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_QuarterPaymentRequirements_QuarterPeriods_QuarterPeriodId",
                        column: x => x.QuarterPeriodId,
                        principalTable: "QuarterPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentPeriods_CreatedBy",
                table: "EnrollmentPeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentPeriods_Semester_AcademicYear_IsActive",
                table: "EnrollmentPeriods",
                columns: new[] { "Semester", "AcademicYear", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_QuarterPaymentRequirements_PromissoryNoteId",
                table: "QuarterPaymentRequirements",
                column: "PromissoryNoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuarterPaymentRequirements_QuarterPeriodId",
                table: "QuarterPaymentRequirements",
                column: "QuarterPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_QuarterPaymentRequirements_StudentId_QuarterPeriodId",
                table: "QuarterPaymentRequirements",
                columns: new[] { "StudentId", "QuarterPeriodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuarterPeriods_CreatedBy",
                table: "QuarterPeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuarterPeriods_Quarter_Semester_AcademicYear",
                table: "QuarterPeriods",
                columns: new[] { "Quarter", "Semester", "AcademicYear" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrollmentPeriods");

            migrationBuilder.DropTable(
                name: "QuarterPaymentRequirements");

            migrationBuilder.DropTable(
                name: "QuarterPeriods");

            migrationBuilder.DropColumn(
                name: "QuarterPaymentRequirementId",
                table: "PromissoryNotes");
        }
    }
}
