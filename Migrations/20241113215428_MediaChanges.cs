using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace demo_boeing_peoplesoft.Migrations
{
    /// <inheritdoc />
    public partial class MediaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MediaFiles",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MediaFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MediaFiles");
        }
    }
}
