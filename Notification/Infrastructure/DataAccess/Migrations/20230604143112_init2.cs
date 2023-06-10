using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "ProjectMembers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TaskLeaderId",
                table: "AppTasks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppTasks_TaskLeaderId",
                table: "AppTasks",
                column: "TaskLeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_ProjectMembers_TaskLeaderId",
                table: "AppTasks",
                column: "TaskLeaderId",
                principalTable: "ProjectMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_ProjectMembers_TaskLeaderId",
                table: "AppTasks");

            migrationBuilder.DropIndex(
                name: "IX_AppTasks_TaskLeaderId",
                table: "AppTasks");

            migrationBuilder.DropColumn(
                name: "TaskLeaderId",
                table: "AppTasks");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "ProjectMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
