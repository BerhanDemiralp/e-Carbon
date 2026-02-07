using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCarbon.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityToSnapshotItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_snapshot_items_facility_id",
                table: "snapshot_items",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_snapshot_items_facilities_FacilityId",
                table: "snapshot_items",
                column: "FacilityId",
                principalTable: "facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_snapshot_items_facilities_FacilityId",
                table: "snapshot_items");

            migrationBuilder.DropIndex(
                name: "IX_snapshot_items_facility_id",
                table: "snapshot_items");
        }
    }
}
