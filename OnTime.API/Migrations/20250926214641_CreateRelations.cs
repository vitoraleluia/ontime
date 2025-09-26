using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OnTime.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Appoitnments_AppointmentId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_AppointmentId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Sessions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Sessions",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 26, 22, 46, 39, 354, DateTimeKind.Local).AddTicks(8989),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfessional",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Appoitnments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalId",
                table: "Appoitnments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AppointmentSession",
                columns: table => new
                {
                    AppointmentsId = table.Column<int>(type: "integer", nullable: false),
                    ServicesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSession", x => new { x.AppointmentsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_AppointmentSession_Appoitnments_AppointmentsId",
                        column: x => x.AppointmentsId,
                        principalTable: "Appoitnments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentSession_Sessions_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentUser",
                columns: table => new
                {
                    AppointmentsId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentUser", x => new { x.AppointmentsId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AppointmentUser_Appoitnments_AppointmentsId",
                        column: x => x.AppointmentsId,
                        principalTable: "Appoitnments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 26, 22, 46, 39, 346, DateTimeKind.Local).AddTicks(1313)),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_OrganizationId",
                table: "Sessions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Appoitnments_ClientId",
                table: "Appoitnments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appoitnments_ProfessionalId",
                table: "Appoitnments",
                column: "ProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSession_ServicesId",
                table: "AppointmentSession",
                column: "ServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentUser_UserId",
                table: "AppointmentUser",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Organizations_OrganizationId",
                table: "Sessions",
                column: "OrganizationId",
                principalTable: "Organizations",
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

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Organizations_OrganizationId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "AppointmentSession");

            migrationBuilder.DropTable(
                name: "AppointmentUser");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_OrganizationId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Appoitnments_ClientId",
                table: "Appoitnments");

            migrationBuilder.DropIndex(
                name: "IX_Appoitnments_ProfessionalId",
                table: "Appoitnments");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsProfessional",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Appoitnments");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "Appoitnments");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Sessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Sessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 26, 22, 46, 39, 354, DateTimeKind.Local).AddTicks(8989));

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AppointmentId",
                table: "Sessions",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Appoitnments_AppointmentId",
                table: "Sessions",
                column: "AppointmentId",
                principalTable: "Appoitnments",
                principalColumn: "Id");
        }
    }
}
