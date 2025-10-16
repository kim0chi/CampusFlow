using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEnrollmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToBulkEnrollmentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalSteps_Enrollments_EnrollmentId",
                table: "ApprovalSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentPayments_Enrollments_EnrollmentId",
                table: "EnrollmentPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PromissoryNotes_Enrollments_EnrollmentId",
                table: "PromissoryNotes");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "PromissoryNotes",
                newName: "EnrollmentBatchId");

            migrationBuilder.RenameIndex(
                name: "IX_PromissoryNotes_EnrollmentId",
                table: "PromissoryNotes",
                newName: "IX_PromissoryNotes_EnrollmentBatchId");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "EnrollmentPayments",
                newName: "EnrollmentBatchId");

            migrationBuilder.RenameIndex(
                name: "IX_EnrollmentPayments_EnrollmentId",
                table: "EnrollmentPayments",
                newName: "IX_EnrollmentPayments_EnrollmentBatchId");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "ApprovalSteps",
                newName: "EnrollmentBatchId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalSteps_EnrollmentId_StepType",
                table: "ApprovalSteps",
                newName: "IX_ApprovalSteps_EnrollmentBatchId_StepType");

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentBatchId",
                table: "Enrollments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EnrollmentBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    Semester = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StudentComments = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TotalCredits = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentBatches_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_EnrollmentBatchId",
                table: "Enrollments",
                column: "EnrollmentBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentBatches_StudentId_Semester_AcademicYear_SubmittedDate",
                table: "EnrollmentBatches",
                columns: new[] { "StudentId", "Semester", "AcademicYear", "SubmittedDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalSteps_EnrollmentBatches_EnrollmentBatchId",
                table: "ApprovalSteps",
                column: "EnrollmentBatchId",
                principalTable: "EnrollmentBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentPayments_EnrollmentBatches_EnrollmentBatchId",
                table: "EnrollmentPayments",
                column: "EnrollmentBatchId",
                principalTable: "EnrollmentBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_EnrollmentBatches_EnrollmentBatchId",
                table: "Enrollments",
                column: "EnrollmentBatchId",
                principalTable: "EnrollmentBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromissoryNotes_EnrollmentBatches_EnrollmentBatchId",
                table: "PromissoryNotes",
                column: "EnrollmentBatchId",
                principalTable: "EnrollmentBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalSteps_EnrollmentBatches_EnrollmentBatchId",
                table: "ApprovalSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentPayments_EnrollmentBatches_EnrollmentBatchId",
                table: "EnrollmentPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_EnrollmentBatches_EnrollmentBatchId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_PromissoryNotes_EnrollmentBatches_EnrollmentBatchId",
                table: "PromissoryNotes");

            migrationBuilder.DropTable(
                name: "EnrollmentBatches");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_EnrollmentBatchId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "EnrollmentBatchId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "EnrollmentBatchId",
                table: "PromissoryNotes",
                newName: "EnrollmentId");

            migrationBuilder.RenameIndex(
                name: "IX_PromissoryNotes_EnrollmentBatchId",
                table: "PromissoryNotes",
                newName: "IX_PromissoryNotes_EnrollmentId");

            migrationBuilder.RenameColumn(
                name: "EnrollmentBatchId",
                table: "EnrollmentPayments",
                newName: "EnrollmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EnrollmentPayments_EnrollmentBatchId",
                table: "EnrollmentPayments",
                newName: "IX_EnrollmentPayments_EnrollmentId");

            migrationBuilder.RenameColumn(
                name: "EnrollmentBatchId",
                table: "ApprovalSteps",
                newName: "EnrollmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalSteps_EnrollmentBatchId_StepType",
                table: "ApprovalSteps",
                newName: "IX_ApprovalSteps_EnrollmentId_StepType");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalSteps_Enrollments_EnrollmentId",
                table: "ApprovalSteps",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentPayments_Enrollments_EnrollmentId",
                table: "EnrollmentPayments",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromissoryNotes_Enrollments_EnrollmentId",
                table: "PromissoryNotes",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
