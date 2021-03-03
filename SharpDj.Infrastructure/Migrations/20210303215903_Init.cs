using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SharpDj.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ip = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Port = table.Column<int>(type: "int", nullable: false),
                    ConnectionType = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAuths",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    AuthenticationKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AuthenticationExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAuths_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomChatPosts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Color = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomChatPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomChatPosts_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomChatPosts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicEnterMessage = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PublicLeaveMessage = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LocalEnterMessage = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LocalLeaveMessage = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomConfigs_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_Date",
                table: "Connections",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_Ip",
                table: "Connections",
                column: "Ip");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UserId",
                table: "Connections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomChatPosts_RoomId",
                table: "RoomChatPosts",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomChatPosts_Text",
                table: "RoomChatPosts",
                column: "Text");

            migrationBuilder.CreateIndex(
                name: "IX_RoomChatPosts_UserId",
                table: "RoomChatPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomConfigs_RoomId",
                table: "RoomConfigs",
                column: "RoomId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AuthorId",
                table: "Rooms",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Name",
                table: "Rooms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuths_Login",
                table: "UserAuths",
                column: "Login");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuths_UserId",
                table: "UserAuths",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "RoomChatPosts");

            migrationBuilder.DropTable(
                name: "RoomConfigs");

            migrationBuilder.DropTable(
                name: "UserAuths");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
