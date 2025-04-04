using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc.Migrations
{
    /// <inheritdoc />
    public partial class editCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Checkouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Checkouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_BusinessId",
                table: "Checkouts",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_PackageId",
                table: "Checkouts",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Businesses_BusinessId",
                table: "Checkouts",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Packages_PackageId",
                table: "Checkouts",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Businesses_BusinessId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Packages_PackageId",
                table: "Checkouts");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_BusinessId",
                table: "Checkouts");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_PackageId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Checkouts");
        }
    }
}
