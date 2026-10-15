using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Florarium.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "flower",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    variety = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    latin_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    price_per_stem = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    color_family = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    photo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flower", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    flower_id = table.Column<Guid>(type: "uuid", nullable: false),
                    available = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    reserved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    back_in_stock_at = table.Column<DateOnly>(type: "date", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stock", x => x.flower_id);
                    table.ForeignKey(
                        name: "fk_stock_flower_flower_id",
                        column: x => x.flower_id,
                        principalTable: "flower",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_flower_color_family",
                table: "flower",
                column: "color_family");

            migrationBuilder.CreateIndex(
                name: "ix_flower_name",
                table: "flower",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock");

            migrationBuilder.DropTable(
                name: "flower");
        }
    }
}
