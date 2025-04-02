using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerProject.Migrations
{
    /// <inheritdoc />
    public partial class IdentityDbContextInit20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MentorEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MentorEmail",
                table: "AspNetUsers");
        }
    }
}
