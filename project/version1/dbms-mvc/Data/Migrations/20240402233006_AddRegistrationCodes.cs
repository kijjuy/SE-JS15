using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbms_mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registrationCodes",
                columns: table => new
                {
                    RegistrationCodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<Guid>(type: "TEXT", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrationCodes", x => x.RegistrationCodeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registrationCodes");
        }
    }
}
