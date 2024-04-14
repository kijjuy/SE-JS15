using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbms_mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class createdModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mailingLists",
                columns: table => new
                {
                    MailingListId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    ShortenedName = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mailingLists", x => x.MailingListId);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Organization = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    StreetAddress1 = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Province = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Subscribed = table.Column<char>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Fax = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BedsCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Address2 = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 4, nullable: true),
                    MailingListId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_contacts_mailingLists_MailingListId",
                        column: x => x.MailingListId,
                        principalTable: "mailingLists",
                        principalColumn: "MailingListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_MailingListId",
                table: "contacts",
                column: "MailingListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "mailingLists");
        }
    }
}
