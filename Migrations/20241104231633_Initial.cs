using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace demo_boeing_peoplesoft.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    BlogCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.BlogCategoryId);
                    table.ForeignKey(
                        name: "FK_BlogCategories_BlogCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "BlogCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    MediaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AltText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    S3Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Filesize = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.MediaId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProfileImageID = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "date", nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_MediaFiles_ProfileImageID",
                        column: x => x.ProfileImageID,
                        principalTable: "MediaFiles",
                        principalColumn: "MediaId");
                });

            migrationBuilder.CreateTable(
                name: "BlogEntries",
                columns: table => new
                {
                    BlogEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UrlSlug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatePublished = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogEntries", x => x.BlogEntryId);
                    table.ForeignKey(
                        name: "FK_BlogEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogEntryCategories",
                columns: table => new
                {
                    BlogCategoryId = table.Column<int>(type: "int", nullable: false),
                    BlogEntryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogEntryCategories", x => new { x.BlogCategoryId, x.BlogEntryId });
                    table.ForeignKey(
                        name: "FK_BlogEntryCategories_BlogCategories_BlogCategoryId",
                        column: x => x.BlogCategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "BlogCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogEntryCategories_BlogEntries_BlogEntryId",
                        column: x => x.BlogEntryId,
                        principalTable: "BlogEntries",
                        principalColumn: "BlogEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_ParentCategoryId",
                table: "BlogCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogEntries_UrlSlug",
                table: "BlogEntries",
                column: "UrlSlug");

            migrationBuilder.CreateIndex(
                name: "IX_BlogEntries_UserId",
                table: "BlogEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogEntryCategories_BlogEntryId",
                table: "BlogEntryCategories",
                column: "BlogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfileImageID",
                table: "Users",
                column: "ProfileImageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogEntryCategories");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "BlogEntries");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MediaFiles");
        }
    }
}
