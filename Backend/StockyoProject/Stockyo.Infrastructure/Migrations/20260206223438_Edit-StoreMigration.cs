using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stockyo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditStoreMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubscriptionStatus",
                table: "Stores",
                newName: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Stores",
                newName: "SubscriptionStatus");
        }
    }
}
