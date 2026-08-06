using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareYourRide.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatCountWaypointsAndPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeatCount",
                table: "Trajectories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommonStopsCount",
                table: "RideApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PassengerTrajectoryId",
                table: "RideApplications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "RideApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RideApplications_PassengerTrajectoryId",
                table: "RideApplications",
                column: "PassengerTrajectoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications",
                column: "PassengerTrajectoryId",
                principalTable: "Trajectories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications");

            migrationBuilder.DropIndex(
                name: "IX_RideApplications_PassengerTrajectoryId",
                table: "RideApplications");

            migrationBuilder.DropColumn(
                name: "SeatCount",
                table: "Trajectories");

            migrationBuilder.DropColumn(
                name: "CommonStopsCount",
                table: "RideApplications");

            migrationBuilder.DropColumn(
                name: "PassengerTrajectoryId",
                table: "RideApplications");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "RideApplications");
        }
    }
}
