using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class inventoryTransationToInvoiceDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_InvoiceDetailID",
                table: "InventoryTransactions",
                column: "InvoiceDetailID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_InvoiceDetails_InvoiceDetailID",
                table: "InventoryTransactions",
                column: "InvoiceDetailID",
                principalTable: "InvoiceDetails",
                principalColumn: "InvoiceDetailID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_InvoiceDetails_InvoiceDetailID",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_InvoiceDetailID",
                table: "InventoryTransactions");
        }
    }
}
