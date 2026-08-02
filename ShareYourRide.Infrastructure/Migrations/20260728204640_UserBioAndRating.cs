using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareYourRide.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserBioAndRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Users",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Users",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleGroupId",
                table: "Trajectories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrajectoryWaypoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrajectoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrajectoryWaypoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrajectoryWaypoints_Stops_StopId",
                        column: x => x.StopId,
                        principalTable: "Stops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrajectoryWaypoints_Trajectories_TrajectoryId",
                        column: x => x.TrajectoryId,
                        principalTable: "Trajectories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trajectories_ScheduleGroupId",
                table: "Trajectories",
                column: "ScheduleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TrajectoryWaypoints_StopId",
                table: "TrajectoryWaypoints",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_TrajectoryWaypoints_TrajectoryId_Order",
                table: "TrajectoryWaypoints",
                columns: new[] { "TrajectoryId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrajectoryWaypoints");

            migrationBuilder.DropIndex(
                name: "IX_Trajectories_ScheduleGroupId",
                table: "Trajectories");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ScheduleGroupId",
                table: "Trajectories");
        }
    }
}
