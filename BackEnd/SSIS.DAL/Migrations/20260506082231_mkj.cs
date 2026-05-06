using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSIS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class mkj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoursePrerequesite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Courseid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrerequesiteCourseid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequesite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoursePrerequesite_Courses_Courseid",
                        column: x => x.Courseid,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoursePrerequesite_Courses_PrerequesiteCourseid",
                        column: x => x.PrerequesiteCourseid,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequesite_Courseid",
                table: "CoursePrerequesite",
                column: "Courseid");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequesite_PrerequesiteCourseid",
                table: "CoursePrerequesite",
                column: "PrerequesiteCourseid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrerequesite");
        }
    }
}
