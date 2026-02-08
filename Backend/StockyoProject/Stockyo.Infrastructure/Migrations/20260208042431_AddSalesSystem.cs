using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stockyo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_StoreId",
                table: "SalesOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_StoreId",
                table: "Batches",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Stores_StoreId",
                table: "Batches",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Stores_StoreId",
                table: "SalesOrders",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Stores_StoreId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Stores_StoreId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_StoreId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_Batches_StoreId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");
        }
    }
}
