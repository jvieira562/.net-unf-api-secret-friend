using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmigoSecreto.Web.Migrations
{
    /// <inheritdoc />
    public partial class CustomIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RedeSocial",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RedeSocialUrlPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedeSocial",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RedeSocialUrlPath",
                table: "AspNetUsers");
        }
    }
}