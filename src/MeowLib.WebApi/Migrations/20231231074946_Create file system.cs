using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowLib.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Createfilesystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Books",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileSystemName = table.Column<string>(type: "TEXT", nullable: false),
                    UploadAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_ImageId",
                table: "Books",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Files_ImageId",
                table: "Books",
                column: "ImageId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Files_ImageId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Books_ImageId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Books",
                type: "TEXT",
                nullable: true);
        }
    }
}
