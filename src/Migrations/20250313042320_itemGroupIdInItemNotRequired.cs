﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class itemGroupIdInItemNotRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemGroups_ItemGroupID",
                table: "Items");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemGroupID",
                table: "Items",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemGroups_ItemGroupID",
                table: "Items",
                column: "ItemGroupID",
                principalTable: "ItemGroups",
                principalColumn: "ItemGroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemGroups_ItemGroupID",
                table: "Items");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemGroupID",
                table: "Items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemGroups_ItemGroupID",
                table: "Items",
                column: "ItemGroupID",
                principalTable: "ItemGroups",
                principalColumn: "ItemGroupID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
