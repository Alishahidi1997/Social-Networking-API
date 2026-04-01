using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLikeTargetPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetPhotoId",
                table: "UserLikes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_TargetPhotoId",
                table: "UserLikes",
                column: "TargetPhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_Photos_TargetPhotoId",
                table: "UserLikes",
                column: "TargetPhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_Photos_TargetPhotoId",
                table: "UserLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserLikes_TargetPhotoId",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "TargetPhotoId",
                table: "UserLikes");
        }
    }
}
