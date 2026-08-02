using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserProfileRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "UserProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}