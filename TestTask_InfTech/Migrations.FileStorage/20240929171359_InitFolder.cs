using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestTask_InfTech.Migrations.FileStorage
{
    /// <inheritdoc />
    public partial class InitFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folder_Folder_ParentalFolderId",
                table: "Folder");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentalFolderId",
                table: "Folder",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Folder_Folder_ParentalFolderId",
                table: "Folder",
                column: "ParentalFolderId",
                principalTable: "Folder",
                principalColumn: "FolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folder_Folder_ParentalFolderId",
                table: "Folder");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentalFolderId",
                table: "Folder",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Folder_Folder_ParentalFolderId",
                table: "Folder",
                column: "ParentalFolderId",
                principalTable: "Folder",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
