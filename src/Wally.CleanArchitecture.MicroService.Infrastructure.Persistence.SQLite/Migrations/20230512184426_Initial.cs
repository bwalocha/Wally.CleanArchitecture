using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SQLite.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "User",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "TEXT", nullable: false),
					Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
					CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
					ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
					ModifiedById = table.Column<Guid>(type: "TEXT", nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_User", x => x.Id); });

			migrationBuilder.CreateIndex(name: "IX_User_Name", table: "User", column: "Name", unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "User");
		}
	}
}
