using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc.Migrations
{
    /// <inheritdoc />
    public partial class editTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "BusinessType",
                table: "Businesses",
                newName: "PackageId");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionType",
                table: "Checkouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_PackageId",
                table: "Businesses",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Packages_PackageId",
                table: "Businesses",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Packages_PackageId",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_PackageId",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "SubscriptionType",
                table: "Checkouts");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "Businesses",
                newName: "BusinessType");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Packages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
