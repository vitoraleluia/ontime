using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTime.API.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ClientId",
                table: "Appoitnments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ProfessionalId",
                table: "Appoitnments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appoitnments_AspNetUsers_UserId",
                table: "Appoitnments");

            migrationBuilder.DropIndex(
                name: "IX_Appoitnments_UserId",
                table: "Appoitnments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Appoitnments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ClientId",
                table: "Appoitnments",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ProfessionalId",
                table: "Appoitnments",
                column: "ProfessionalId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ClientId",
                table: "Appoitnments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ProfessionalId",
                table: "Appoitnments");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Appoitnments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appoitnments_UserId",
                table: "Appoitnments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ClientId",
                table: "Appoitnments",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appoitnments_AspNetUsers_ProfessionalId",
                table: "Appoitnments",
                column: "ProfessionalId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appoitnments_AspNetUsers_UserId",
                table: "Appoitnments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
