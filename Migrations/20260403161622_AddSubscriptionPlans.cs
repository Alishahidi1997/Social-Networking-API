using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    MonthlyPriceUsd = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnlimitedLikes = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeeWhoLikedYou = table.Column<bool>(type: "INTEGER", nullable: false),
                    PriorityInDiscovery = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "Description", "MonthlyPriceUsd", "Name", "PriorityInDiscovery", "SeeWhoLikedYou", "UnlimitedLikes" },
                values: new object[,]
                {
                    { 1, "Daily like cap; who-liked-you is locked.", 0m, "Free", false, false, false },
                    { 2, "Unlimited likes and see who liked you.", 9.99m, "Plus", false, true, true },
                    { 3, "Same as Plus with higher placement in discovery.", 19.99m, "Premium", true, true, true }
                });

            migrationBuilder.AddColumn<int>(
                name: "DiscoveryBoostCached",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndsUtc",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SubscriptionPlanId",
                table: "Users",
                column: "SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SubscriptionPlans_SubscriptionPlanId",
                table: "Users",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SubscriptionPlans_SubscriptionPlanId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SubscriptionPlanId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DiscoveryBoostCached",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndsUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");
        }
    }
}
