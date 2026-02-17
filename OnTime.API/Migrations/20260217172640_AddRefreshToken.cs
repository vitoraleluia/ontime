using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OnTime.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
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

            migrationBuilder.DropTable(
                name: "AppointmentUser");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Sessions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Sessions",
                type: "character varying(255)",
                maxLength: 255,
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
                oldDefaultValue: new DateTime(2025, 10, 12, 12, 2, 22, 646, DateTimeKind.Utc).AddTicks(4239));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Organizations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Organizations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 10, 12, 12, 2, 22, 639, DateTimeKind.Utc).AddTicks(188));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Appoitnments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appoitnments_UserId",
                table: "Appoitnments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
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
                name: "FK_Appoitnments_AspNetUsers_UserId",
                table: "Appoitnments");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Appoitnments_UserId",
                table: "Appoitnments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Appoitnments");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Sessions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Sessions",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 10, 12, 12, 2, 22, 646, DateTimeKind.Utc),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Organizations",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Organizations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 10, 12, 12, 2, 22, 639, DateTimeKind.Utc),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

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
        }
    }
}