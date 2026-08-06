using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareYourRide.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatCountWaypointsAndPricing21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideApplications_Trajectories_PassengerTrajectoryId",
                table: "RideApplications");

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
