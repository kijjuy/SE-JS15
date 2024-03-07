using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbms_mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContactMailingListToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contacts_mailingLists_MailingListId",
                table: "contacts");

            migrationBuilder.DropIndex(
                name: "IX_contacts_MailingListId",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "MailingListId",
                table: "contacts");

            migrationBuilder.AddColumn<string>(
                name: "MailingList",
                table: "contacts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailingList",
                table: "contacts");

            migrationBuilder.AddColumn<int>(
                name: "MailingListId",
                table: "contacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_contacts_MailingListId",
                table: "contacts",
                column: "MailingListId");

            migrationBuilder.AddForeignKey(
                name: "FK_contacts_mailingLists_MailingListId",
                table: "contacts",
                column: "MailingListId",
                principalTable: "mailingLists",
                principalColumn: "MailingListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
