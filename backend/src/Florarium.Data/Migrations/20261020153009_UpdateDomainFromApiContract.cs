using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Florarium.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDomainFromApiContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_role_role_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_cart_customer_id",
                table: "cart");

            migrationBuilder.CreateSequence<int>(
                name: "order_number_seq",
                startValue: 1000L);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "user",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "stock",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "apartment",
                table: "delivery",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "entrance",
                table: "delivery",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "floor",
                table: "delivery",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "intercom",
                table: "delivery",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_phone",
                table: "delivery",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "delivery",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "composition",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_cart_customer_id",
                table: "cart",
                column: "customer_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_user_role_role_id",
                table: "user",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_role_role_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_cart_customer_id",
                table: "cart");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "user");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "stock");

            migrationBuilder.DropColumn(
                name: "apartment",
                table: "delivery");

            migrationBuilder.DropColumn(
                name: "entrance",
                table: "delivery");

            migrationBuilder.DropColumn(
                name: "floor",
                table: "delivery");

            migrationBuilder.DropColumn(
                name: "intercom",
                table: "delivery");

            migrationBuilder.DropColumn(
                name: "recipient_phone",
                table: "delivery");

            migrationBuilder.DropColumn(
                name: "street",
                table: "delivery");

            migrationBuilder.DropColumn(
                name: "title",
                table: "composition");

            migrationBuilder.DropSequence(
                name: "order_number_seq");

            migrationBuilder.CreateIndex(
                name: "ix_cart_customer_id",
                table: "cart",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_role_role_id",
                table: "user",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
