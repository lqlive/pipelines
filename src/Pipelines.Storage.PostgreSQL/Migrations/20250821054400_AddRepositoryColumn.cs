using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pipelines.Storage.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Repositories",
                newName: "CloneUrl");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Repositories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "Repositories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HtmlUrl",
                table: "Repositories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SshUrl",
                table: "Repositories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "Branch",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "HtmlUrl",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "SshUrl",
                table: "Repositories");

            migrationBuilder.RenameColumn(
                name: "CloneUrl",
                table: "Repositories",
                newName: "Url");
        }
    }
}
