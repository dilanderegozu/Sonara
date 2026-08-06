using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sonara.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberShips_AspNetUsers_UserId",
                table: "UserMemberShips");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberShips_MembershipPlans_MembershipPlanId",
                table: "UserMemberShips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMemberShips",
                table: "UserMemberShips");

            migrationBuilder.RenameTable(
                name: "UserMemberShips",
                newName: "UserMemberships");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberShips_UserId",
                table: "UserMemberships",
                newName: "IX_UserMemberships_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberShips_MembershipPlanId",
                table: "UserMemberships",
                newName: "IX_UserMemberships_MembershipPlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMemberships",
                table: "UserMemberships",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberships_AspNetUsers_UserId",
                table: "UserMemberships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberships_MembershipPlans_MembershipPlanId",
                table: "UserMemberships",
                column: "MembershipPlanId",
                principalTable: "MembershipPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberships_AspNetUsers_UserId",
                table: "UserMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMemberships_MembershipPlans_MembershipPlanId",
                table: "UserMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMemberships",
                table: "UserMemberships");

            migrationBuilder.RenameTable(
                name: "UserMemberships",
                newName: "UserMemberShips");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberships_UserId",
                table: "UserMemberShips",
                newName: "IX_UserMemberShips_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMemberships_MembershipPlanId",
                table: "UserMemberShips",
                newName: "IX_UserMemberShips_MembershipPlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMemberShips",
                table: "UserMemberShips",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberShips_AspNetUsers_UserId",
                table: "UserMemberShips",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMemberShips_MembershipPlans_MembershipPlanId",
                table: "UserMemberShips",
                column: "MembershipPlanId",
                principalTable: "MembershipPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
