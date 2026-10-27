using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookSystem.Database.Migrations
{
    /// <inheritdoc />
    public partial class shelfUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shelfs_UserId",
                table: "Shelfs");

            migrationBuilder.CreateIndex(
                name: "IX_Shelfs_UserId_BookId",
                table: "Shelfs",
                columns: new[] { "UserId", "BookId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shelfs_UserId_BookId",
                table: "Shelfs");

            migrationBuilder.CreateIndex(
                name: "IX_Shelfs_UserId",
                table: "Shelfs",
                column: "UserId");
        }
    }
}
