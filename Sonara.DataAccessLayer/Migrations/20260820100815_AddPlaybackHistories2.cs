using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sonara.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaybackHistories2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlaybackHistories_UserId",
                table: "PlaybackHistories");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackHistories_UserId_SongId",
                table: "PlaybackHistories",
                columns: new[] { "UserId", "SongId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlaybackHistories_UserId_SongId",
                table: "PlaybackHistories");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackHistories_UserId",
                table: "PlaybackHistories",
                column: "UserId");
        }
    }
}
