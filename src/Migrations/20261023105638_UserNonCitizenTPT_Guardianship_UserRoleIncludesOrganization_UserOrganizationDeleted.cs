using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giraf_core_v2.Migrations
{
    /// <inheritdoc />
    public partial class UserNonCitizenTPT_Guardianship_UserRoleIncludesOrganization_UserOrganizationDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citizen_User_GuardianId",
                table: "Citizen");

            migrationBuilder.DropForeignKey(
                name: "FK_Citizen_User_UserId",
                table: "Citizen");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Citizen_CitizenId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropTable(
                name: "UserOrganization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Citizen",
                table: "Citizen");

            migrationBuilder.DropIndex(
                name: "IX_Citizen_GuardianId",
                table: "Citizen");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Citizen");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "UserRole",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserRole",
                newName: "NonCitizenId");

            migrationBuilder.RenameColumn(
                name: "GuardianId",
                table: "Citizen",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserRole",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "NonCitizenId", "Role", "OrganizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Citizen",
                table: "Citizen",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "NonCitizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonCitizen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonCitizen_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guardianship",
                columns: table => new
                {
                    CitizenId = table.Column<int>(type: "integer", nullable: false),
                    GuardianId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardianship", x => new { x.CitizenId, x.GuardianId });
                    table.ForeignKey(
                        name: "FK_Guardianship_Citizen_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guardianship_NonCitizen_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "NonCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_OrganizationId",
                table: "UserRole",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardianship_GuardianId",
                table: "Guardianship",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_NonCitizen_Email",
                table: "NonCitizen",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Citizen_User_Id",
                table: "Citizen",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Citizen_CitizenId",
                table: "Image",
                column: "CitizenId",
                principalTable: "Citizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_NonCitizen_NonCitizenId",
                table: "UserRole",
                column: "NonCitizenId",
                principalTable: "NonCitizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Organization_OrganizationId",
                table: "UserRole",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citizen_User_Id",
                table: "Citizen");

            migrationBuilder.DropForeignKey(
                name: "FK_Image_Citizen_CitizenId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_NonCitizen_NonCitizenId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Organization_OrganizationId",
                table: "UserRole");

            migrationBuilder.DropTable(
                name: "Guardianship");

            migrationBuilder.DropTable(
                name: "NonCitizen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_OrganizationId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_User_Username",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Citizen",
                table: "Citizen");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserRole");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "UserRole",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "NonCitizenId",
                table: "UserRole",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Citizen",
                newName: "GuardianId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Citizen",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Citizen",
                table: "Citizen",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "UserOrganization",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganization", x => new { x.UserId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_UserOrganization_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganization_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_GuardianId",
                table: "Citizen",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganization_OrganizationId",
                table: "UserOrganization",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citizen_User_GuardianId",
                table: "Citizen",
                column: "GuardianId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citizen_User_UserId",
                table: "Citizen",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Citizen_CitizenId",
                table: "Image",
                column: "CitizenId",
                principalTable: "Citizen",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}