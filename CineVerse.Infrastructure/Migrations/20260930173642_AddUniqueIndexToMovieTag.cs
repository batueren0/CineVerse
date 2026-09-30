using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineVerse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToMovieTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovieTags_MovieId",
                table: "MovieTags");

            migrationBuilder.CreateIndex(
                name: "IX_MovieTags_MovieId_TagId",
                table: "MovieTags",
                columns: new[] { "MovieId", "TagId" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovieTags_MovieId_TagId",
                table: "MovieTags");

            migrationBuilder.CreateIndex(
                name: "IX_MovieTags_MovieId",
                table: "MovieTags",
                column: "MovieId");
        }
    }
}
