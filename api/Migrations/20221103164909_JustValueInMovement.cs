using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAPI.Migrations
{
    public partial class JustValueInMovement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossValue",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "NetValue",
                table: "Movements",
                newName: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Movements",
                newName: "NetValue");

            migrationBuilder.AddColumn<decimal>(
                name: "GrossValue",
                table: "Movements",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
