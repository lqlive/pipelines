using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pipelines.Storage.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Repositories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "RawId",
                table: "Repositories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Repositories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Repositories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "RawId",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Repositories");
        }
    }
}
