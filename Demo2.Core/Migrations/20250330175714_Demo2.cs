using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo2.Core.Migrations
{
    /// <inheritdoc />
    public partial class Demo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Difference = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Timestamp);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");
        }
    }
}
