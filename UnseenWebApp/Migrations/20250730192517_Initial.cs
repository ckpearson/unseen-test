using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnseenWebApp.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopScoreUniqueStrings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Word = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopScoreUniqueStrings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopScoreUniqueStrings_SubmittedAtUtc",
                table: "TopScoreUniqueStrings",
                column: "SubmittedAtUtc",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_TopScoreUniqueStrings_Word",
                table: "TopScoreUniqueStrings",
                column: "Word",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopScoreUniqueStrings");
        }
    }
}
