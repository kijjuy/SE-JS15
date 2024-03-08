using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbms_mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomeCategory",
                table: "contacts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeCategory",
                table: "contacts");
        }
    }
}
