using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pipelines.Storage.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryAccessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccessId",
                table: "Repositories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccessId1",
                table: "Repositories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RepositoryAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminUsers = table.Column<List<string>>(type: "text[]", nullable: true),
                    WriteUsers = table.Column<List<string>>(type: "text[]", nullable: true),
                    ReadUsers = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryAccess", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_AccessId1",
                table: "Repositories",
                column: "AccessId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_RepositoryAccess_AccessId1",
                table: "Repositories",
                column: "AccessId1",
                principalTable: "RepositoryAccess",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_RepositoryAccess_AccessId1",
                table: "Repositories");

            migrationBuilder.DropTable(
                name: "RepositoryAccess");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_AccessId1",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "AccessId",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "AccessId1",
                table: "Repositories");
        }
    }
}
