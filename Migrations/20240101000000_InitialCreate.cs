using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrepaidWallet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrepaidUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0.00m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaidUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BalanceTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrepaidUserId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OperatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceTransactions_PrepaidUsers_PrepaidUserId",
                        column: x => x.PrepaidUserId,
                        principalTable: "PrepaidUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PrepaidUsers",
                columns: new[] { "Id", "Balance", "CreatedAt", "Email", "FullName", "IsActive", "PhoneNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 5000.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "adaeze.okonkwo@email.com", "Adaeze Okonkwo", true, "08012345678", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 12500.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "emeka.chukwu@email.com", "Emeka Chukwu", true, "08098765432", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 750.50m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "fatima.bello@email.com", "Fatima Bello", true, "07031122334", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_PrepaidUserId",
                table: "BalanceTransactions",
                column: "PrepaidUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidUsers_PhoneNumber",
                table: "PrepaidUsers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrepaidUsers_Email",
                table: "PrepaidUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL AND [Email] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BalanceTransactions");
            migrationBuilder.DropTable(name: "PrepaidUsers");
        }
    }
}
