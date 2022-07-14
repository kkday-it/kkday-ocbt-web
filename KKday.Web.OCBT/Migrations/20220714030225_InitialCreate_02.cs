using Microsoft.EntityFrameworkCore.Migrations;

namespace KKday.Web.OCBT.Migrations
{
    public partial class InitialCreate_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_booking_mst_is_callback",
                table: "booking_mst",
                column: "is_callback");

            migrationBuilder.CreateIndex(
                name: "idx_booking_mst_order_mid",
                table: "booking_mst",
                column: "order_mid");

            migrationBuilder.CreateIndex(
                name: "idx_booking_dtl_booking_mst_xid",
                table: "booking_dtl",
                column: "booking_mst_xid");

            migrationBuilder.CreateIndex(
                name: "idx_booking_dtl_order_mid",
                table: "booking_dtl",
                column: "order_mid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_booking_mst_is_callback",
                table: "booking_mst");

            migrationBuilder.DropIndex(
                name: "idx_booking_mst_order_mid",
                table: "booking_mst");

            migrationBuilder.DropIndex(
                name: "idx_booking_dtl_booking_mst_xid",
                table: "booking_dtl");

            migrationBuilder.DropIndex(
                name: "idx_booking_dtl_order_mid",
                table: "booking_dtl");
        }
    }
}
