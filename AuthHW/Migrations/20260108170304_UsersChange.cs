using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthHW.Migrations
{
    /// <inheritdoc />
    public partial class UsersChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_NormalizedUsername",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "NormalizedUsername",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "NormalizedUsername",
                table: "AuthAttempts");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserAccounts",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "UserAccounts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_Tag",
                table: "UserAccounts",
                column: "Tag",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_Tag",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "UserAccounts");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUsername",
                table: "UserAccounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUsername",
                table: "AuthAttempts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_NormalizedUsername",
                table: "UserAccounts",
                column: "NormalizedUsername",
                unique: true);
        }
    }
}
