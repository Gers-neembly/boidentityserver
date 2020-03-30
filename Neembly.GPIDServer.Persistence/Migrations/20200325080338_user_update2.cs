using Microsoft.EntityFrameworkCore.Migrations;

namespace Neembly.BOIDServer.Persistence.Migrations
{
    public partial class user_update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitialPassword",
                table: "BackOfficeUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialPassword",
                table: "BackOfficeUsers");
        }
    }
}
