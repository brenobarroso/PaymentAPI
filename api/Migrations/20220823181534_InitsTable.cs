using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaymentAPI.Migrations
{
    public partial class InitsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisapprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Confirmation = table.Column<bool>(type: "boolean", nullable: false),
                    GrossValue = table.Column<float>(type: "real", nullable: false),
                    NetValue = table.Column<float>(type: "real", nullable: false),
                    FlatRate = table.Column<float>(type: "real", nullable: false),
                    CardNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
