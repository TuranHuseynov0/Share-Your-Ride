using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareYourRide.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateLimitReviewsAndChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications");

            migrationBuilder.CreateTable(
                name: "ChatThreads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RideApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PassengerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatThreads_RideApplications_RideApplicationId",
                        column: x => x.RideApplicationId,
                        principalTable: "RideApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatThreads_Users_DriverUserId",
                        column: x => x.DriverUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatThreads_Users_PassengerUserId",
                        column: x => x.PassengerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RideApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevieweeUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_RideApplications_RideApplicationId",
                        column: x => x.RideApplicationId,
                        principalTable: "RideApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_RevieweeUserId",
                        column: x => x.RevieweeUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_ReviewerUserId",
                        column: x => x.ReviewerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatThreads_ChatThreadId",
                        column: x => x.ChatThreadId,
                        principalTable: "ChatThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatThreadId_CreatedAt",
                table: "ChatMessages",
                columns: new[] { "ChatThreadId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderUserId",
                table: "ChatMessages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatThreads_DriverUserId",
                table: "ChatThreads",
                column: "DriverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatThreads_PassengerUserId",
                table: "ChatThreads",
                column: "PassengerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatThreads_RideApplicationId",
                table: "ChatThreads",
                column: "RideApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeUserId",
                table: "Reviews",
                column: "RevieweeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerUserId",
                table: "Reviews",
                column: "ReviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RideApplicationId_ReviewerUserId",
                table: "Reviews",
                columns: new[] { "RideApplicationId", "ReviewerUserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications",
                column: "PassengerTrajectoryId",
                principalTable: "Trajectories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ChatThreads");

            migrationBuilder.AddForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications",
                column: "PassengerTrajectoryId",
                principalTable: "Trajectories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
