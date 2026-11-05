using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giraf_core_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTitleAndTicketId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Image",
                newName: "TicketId");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Image",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "Image",
                newName: "Name");
        }
    }
}
