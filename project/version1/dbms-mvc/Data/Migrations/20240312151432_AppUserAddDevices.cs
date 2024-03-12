using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbms_mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppUserAddDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedDeviced",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDeviced",
                table: "AspNetUsers");
        }
    }
}
