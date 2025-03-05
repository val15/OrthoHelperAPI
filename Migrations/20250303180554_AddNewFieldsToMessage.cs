using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrthoHelperAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseText",
                table: "Messages",
                newName: "ProcessingTime");

            migrationBuilder.AddColumn<string>(
                name: "OutputText",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputText",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ProcessingTime",
                table: "Messages",
                newName: "ResponseText");
        }
    }
}
