using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorShopImageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Shops");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Shops",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_ImageId",
                table: "Shops",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Images_ImageId",
                table: "Shops",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Images_ImageId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_ImageId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Shops");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Shops",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}