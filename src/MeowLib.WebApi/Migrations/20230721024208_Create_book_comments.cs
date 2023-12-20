using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowLib.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Create_book_comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookEntityModelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookComments_Books_BookEntityModelId",
                        column: x => x.BookEntityModelId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookComments_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_AuthorId",
                table: "BookComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_BookEntityModelId",
                table: "BookComments",
                column: "BookEntityModelId");

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_BookId",
                table: "BookComments",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookComments");
        }
    }
}
