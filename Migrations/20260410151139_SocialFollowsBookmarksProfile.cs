using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SocialFollowsBookmarksProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Headline",
                table: "Users",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfileLinks",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    FollowerId = table.Column<int>(type: "INTEGER", nullable: false),
                    FollowingId = table.Column<int>(type: "INTEGER", nullable: false),
                    FollowedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => new { x.FollowerId, x.FollowingId });
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_FollowingId",
                table: "UserFollows",
                column: "FollowingId");

            migrationBuilder.Sql(
                """
                INSERT INTO "UserFollows" ("FollowerId", "FollowingId", "FollowedAtUtc")
                SELECT "SourceUserId", "TargetUserId", "LikedAt" FROM "UserLikes";
                """);

            migrationBuilder.CreateTable(
                name: "UserBookmarks",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookmarkedUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SavedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBookmarks", x => new { x.UserId, x.BookmarkedUserId });
                    table.ForeignKey(
                        name: "FK_UserBookmarks_Users_BookmarkedUserId",
                        column: x => x.BookmarkedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBookmarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_BookmarkedUserId",
                table: "UserBookmarks",
                column: "BookmarkedUserId");

            migrationBuilder.DropTable(
                name: "UserLikes");

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Limited follows per day; follower list locked.");

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Unlimited follows and see your followers.");

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Same as Plus with stronger feed placement.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBookmarks");

            migrationBuilder.DropTable(
                name: "UserFollows");

            migrationBuilder.DropColumn(
                name: "Headline",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileLinks",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserLikes",
                columns: table => new
                {
                    SourceUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetPhotoId = table.Column<int>(type: "INTEGER", nullable: true),
                    LikedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikes", x => new { x.SourceUserId, x.TargetUserId });
                    table.ForeignKey(
                        name: "FK_UserLikes_Photos_TargetPhotoId",
                        column: x => x.TargetPhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserLikes_Users_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLikes_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Daily like cap; who-liked-you is locked.");

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Unlimited likes and see who liked you.");

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Same as Plus with higher placement in discovery.");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_TargetPhotoId",
                table: "UserLikes",
                column: "TargetPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_TargetUserId",
                table: "UserLikes",
                column: "TargetUserId");
        }
    }
}
