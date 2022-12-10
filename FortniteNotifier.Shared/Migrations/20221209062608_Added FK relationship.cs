using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FortniteNotifier.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedFKrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "UnsubscribeRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnsubscribeRequests_RecipientId",
                table: "UnsubscribeRequests",
                column: "RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnsubscribeRequests_Recipients_RecipientId",
                table: "UnsubscribeRequests",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "RecipientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnsubscribeRequests_Recipients_RecipientId",
                table: "UnsubscribeRequests");

            migrationBuilder.DropIndex(
                name: "IX_UnsubscribeRequests_RecipientId",
                table: "UnsubscribeRequests");

            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "UnsubscribeRequests",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
