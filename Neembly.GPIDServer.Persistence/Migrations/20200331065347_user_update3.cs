using Microsoft.EntityFrameworkCore.Migrations;

namespace Neembly.BOIDServer.Persistence.Migrations
{
    public partial class user_update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordReset",
                table: "BackOfficeUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPasswordReset",
                table: "BackOfficeUsers");
        }
    }
}
