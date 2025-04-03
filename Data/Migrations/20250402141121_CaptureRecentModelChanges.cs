using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CaptureRecentModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_MemberEntityId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_UserEntity_UserId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "UserEntity");

            migrationBuilder.DropIndex(
                name: "IX_Projects_MemberEntityId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MemberEntityId",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "MemberEntityId",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_MemberEntityId",
                table: "Projects",
                column: "MemberEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_MemberEntityId",
                table: "Projects",
                column: "MemberEntityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_UserEntity_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
