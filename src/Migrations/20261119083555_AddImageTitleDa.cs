using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giraf_core_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTitleDa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleDa",
                table: "Image",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleDa",
                table: "Image");
        }
    }
}