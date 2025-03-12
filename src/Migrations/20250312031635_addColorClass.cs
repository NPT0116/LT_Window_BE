using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class addColorClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Variants");

            migrationBuilder.AddColumn<Guid>(
                name: "ColorID",
                table: "Variants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ItemID = table.Column<Guid>(type: "uuid", nullable: false),
                    UrlImage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorID);
                    table.ForeignKey(
                        name: "FK_Colors_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ColorID",
                table: "Variants",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_Colors_ItemID",
                table: "Colors",
                column: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_Colors_ColorID",
                table: "Variants",
                column: "ColorID",
                principalTable: "Colors",
                principalColumn: "ColorID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Variants_Colors_ColorID",
                table: "Variants");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_Variants_ColorID",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "ColorID",
                table: "Variants");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Variants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Variants",
                type: "text",
                nullable: true);
        }
    }
}
