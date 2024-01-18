using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeowLib.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addchaptervolume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "Volume",
                table: "Chapters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Chapters");
        }
    }
}
