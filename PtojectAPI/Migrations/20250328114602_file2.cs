using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtojectAPI.Migrations
{
    /// <inheritdoc />
    public partial class file2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkUpload_Tasks_SubtaskId",
                table: "LinkUpload");

            migrationBuilder.DropForeignKey(
                name: "FK_LinkUpload_Users_UserId",
                table: "LinkUpload");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinkUpload",
                table: "LinkUpload");

            migrationBuilder.RenameTable(
                name: "LinkUpload",
                newName: "Links");

            migrationBuilder.RenameIndex(
                name: "IX_LinkUpload_UserId",
                table: "Links",
                newName: "IX_Links_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LinkUpload_SubtaskId",
                table: "Links",
                newName: "IX_Links_SubtaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Links",
                table: "Links",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Tasks_SubtaskId",
                table: "Links",
                column: "SubtaskId",
                principalTable: "Tasks",
                principalColumn: "SubtaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Users_UserId",
                table: "Links",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Tasks_SubtaskId",
                table: "Links");

            migrationBuilder.DropForeignKey(
                name: "FK_Links_Users_UserId",
                table: "Links");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Links",
                table: "Links");

            migrationBuilder.RenameTable(
                name: "Links",
                newName: "LinkUpload");

            migrationBuilder.RenameIndex(
                name: "IX_Links_UserId",
                table: "LinkUpload",
                newName: "IX_LinkUpload_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Links_SubtaskId",
                table: "LinkUpload",
                newName: "IX_LinkUpload_SubtaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinkUpload",
                table: "LinkUpload",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUpload_Tasks_SubtaskId",
                table: "LinkUpload",
                column: "SubtaskId",
                principalTable: "Tasks",
                principalColumn: "SubtaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinkUpload_Users_UserId",
                table: "LinkUpload",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
