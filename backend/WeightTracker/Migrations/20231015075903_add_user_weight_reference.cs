using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeightTracker.Migrations
{
    /// <inheritdoc />
    public partial class add_user_weight_reference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weight_Users_UserId",
                table: "Weight");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weight",
                table: "Weight");

            migrationBuilder.RenameTable(
                name: "Weight",
                newName: "Weights");

            migrationBuilder.RenameIndex(
                name: "IX_Weight_UserId_Date",
                table: "Weights",
                newName: "IX_Weights_UserId_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weights",
                table: "Weights",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weights_Users_UserId",
                table: "Weights",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weights_Users_UserId",
                table: "Weights");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weights",
                table: "Weights");

            migrationBuilder.RenameTable(
                name: "Weights",
                newName: "Weight");

            migrationBuilder.RenameIndex(
                name: "IX_Weights_UserId_Date",
                table: "Weight",
                newName: "IX_Weight_UserId_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weight",
                table: "Weight",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weight_Users_UserId",
                table: "Weight",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
