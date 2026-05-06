using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSIS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mkjlo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrerequesite_Courses_Courseid",
                table: "CoursePrerequesite");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePrerequesite_Courses_PrerequesiteCourseid",
                table: "CoursePrerequesite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursePrerequesite",
                table: "CoursePrerequesite");

            migrationBuilder.RenameTable(
                name: "CoursePrerequesite",
                newName: "coursePrerequesites");

            migrationBuilder.RenameIndex(
                name: "IX_CoursePrerequesite_PrerequesiteCourseid",
                table: "coursePrerequesites",
                newName: "IX_coursePrerequesites_PrerequesiteCourseid");

            migrationBuilder.RenameIndex(
                name: "IX_CoursePrerequesite_Courseid",
                table: "coursePrerequesites",
                newName: "IX_coursePrerequesites_Courseid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_coursePrerequesites",
                table: "coursePrerequesites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_coursePrerequesites_Courses_Courseid",
                table: "coursePrerequesites",
                column: "Courseid",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_coursePrerequesites_Courses_PrerequesiteCourseid",
                table: "coursePrerequesites",
                column: "PrerequesiteCourseid",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_coursePrerequesites_Courses_Courseid",
                table: "coursePrerequesites");

            migrationBuilder.DropForeignKey(
                name: "FK_coursePrerequesites_Courses_PrerequesiteCourseid",
                table: "coursePrerequesites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_coursePrerequesites",
                table: "coursePrerequesites");

            migrationBuilder.RenameTable(
                name: "coursePrerequesites",
                newName: "CoursePrerequesite");

            migrationBuilder.RenameIndex(
                name: "IX_coursePrerequesites_PrerequesiteCourseid",
                table: "CoursePrerequesite",
                newName: "IX_CoursePrerequesite_PrerequesiteCourseid");

            migrationBuilder.RenameIndex(
                name: "IX_coursePrerequesites_Courseid",
                table: "CoursePrerequesite",
                newName: "IX_CoursePrerequesite_Courseid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursePrerequesite",
                table: "CoursePrerequesite",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrerequesite_Courses_Courseid",
                table: "CoursePrerequesite",
                column: "Courseid",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePrerequesite_Courses_PrerequesiteCourseid",
                table: "CoursePrerequesite",
                column: "PrerequesiteCourseid",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
