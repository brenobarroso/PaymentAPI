using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaymentAPI.Migrations
{
    public partial class AddWithdraws : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Accounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "Withdraws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisapprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdraws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Withdraws_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Withdraws_AccountId",
                table: "Withdraws",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Withdraws");

            migrationBuilder.AlterColumn<float>(
                name: "Balance",
                table: "Accounts",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
