using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowLib.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Updatechaptersystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Chapters",
                newName: "TranslationId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_BookId",
                table: "Chapters",
                newName: "IX_Chapters_TranslationId");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Chapters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Translations_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_BookId",
                table: "Translations",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_TeamId",
                table: "Translations",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Translations_TranslationId",
                table: "Chapters",
                column: "TranslationId",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Translations_TranslationId",
                table: "Chapters");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "TranslationId",
                table: "Chapters",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_TranslationId",
                table: "Chapters",
                newName: "IX_Chapters_BookId");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
