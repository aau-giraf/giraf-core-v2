using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giraf_core_v2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Image_StorageKey",
                table: "Image");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Image_SizeBytes_Positive",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "path",
                table: "Image",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Image",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Image",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Image",
                newName: "path");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Image",
                newName: "name");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Image",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Image",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "Image",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "Image",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Image_StorageKey",
                table: "Image",
                column: "StorageKey",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Image_SizeBytes_Positive",
                table: "Image",
                sql: "\"SizeBytes\" > 0");
        }
    }
}
