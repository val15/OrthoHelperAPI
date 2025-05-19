using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrthoHelperAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeColumnToMessage2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelName",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelName",
                table: "Messages");
        }
    }
}
