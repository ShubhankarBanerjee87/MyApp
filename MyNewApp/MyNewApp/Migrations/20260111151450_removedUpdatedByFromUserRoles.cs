using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNewApp.Migrations
{
    /// <inheritdoc />
    public partial class removedUpdatedByFromUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "UserRoles",
                type: "bigint",
                nullable: true);
        }
    }
}
