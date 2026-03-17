using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practice_Pro.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityVerificationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityVerificationD",
                table: "client",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityVerificationD",
                table: "client");
        }
    }
}
