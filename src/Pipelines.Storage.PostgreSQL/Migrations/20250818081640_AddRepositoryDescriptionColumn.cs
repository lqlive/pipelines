using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pipelines.Storage.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryDescriptionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Repositories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Repositories");
        }
    }
}
