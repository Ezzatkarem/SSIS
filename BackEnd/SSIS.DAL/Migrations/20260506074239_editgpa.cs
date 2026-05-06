using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSIS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class editgpa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ComulativeGpa",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCompletedCredits",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYear",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SemesterGpa",
                table: "Enrollments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCredits",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComulativeGpa",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalCompletedCredits",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AcademicYear",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "SemesterGpa",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "TotalCredits",
                table: "Enrollments");
        }
    }
}
