using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareYourRide.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStopOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Stops",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Stops");
        }
    }
}
