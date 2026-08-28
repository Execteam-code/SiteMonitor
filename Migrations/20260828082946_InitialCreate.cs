using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SiteMonitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    LastChecked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseTimeMs = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "IsOnline", "LastChecked", "Name", "ResponseTimeMs", "TargetUrl" },
                values: new object[,]
                {
                    { 1, false, null, "YouTube", null, "https://youtube.com" },
                    { 2, false, null, "GitHub", null, "https://github.com" },
                    { 3, false, null, "Reddit", null, "https://reddit.com" },
                    { 4, false, null, "Discord", null, "https://discord.com" },
                    { 5, false, null, "ChatGPT", null, "https://chatgpt.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
