using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class manytomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTeacher_Students_StudentsId",
                table: "StudentTeacher");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTeacher_Teachers_TeachersId",
                table: "StudentTeacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentTeacher",
                table: "StudentTeacher");

            migrationBuilder.RenameTable(
                name: "StudentTeacher",
                newName: "StudentTeacherManyToMany");

            migrationBuilder.RenameIndex(
                name: "IX_StudentTeacher_TeachersId",
                table: "StudentTeacherManyToMany",
                newName: "IX_StudentTeacherManyToMany_TeachersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentTeacherManyToMany",
                table: "StudentTeacherManyToMany",
                columns: new[] { "StudentsId", "TeachersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTeacherManyToMany_Students_StudentsId",
                table: "StudentTeacherManyToMany",
                column: "StudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTeacherManyToMany_Teachers_TeachersId",
                table: "StudentTeacherManyToMany",
                column: "TeachersId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTeacherManyToMany_Students_StudentsId",
                table: "StudentTeacherManyToMany");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTeacherManyToMany_Teachers_TeachersId",
                table: "StudentTeacherManyToMany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentTeacherManyToMany",
                table: "StudentTeacherManyToMany");

            migrationBuilder.RenameTable(
                name: "StudentTeacherManyToMany",
                newName: "StudentTeacher");

            migrationBuilder.RenameIndex(
                name: "IX_StudentTeacherManyToMany_TeachersId",
                table: "StudentTeacher",
                newName: "IX_StudentTeacher_TeachersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentTeacher",
                table: "StudentTeacher",
                columns: new[] { "StudentsId", "TeachersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTeacher_Students_StudentsId",
                table: "StudentTeacher",
                column: "StudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTeacher_Teachers_TeachersId",
                table: "StudentTeacher",
                column: "TeachersId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
