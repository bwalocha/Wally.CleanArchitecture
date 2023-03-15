using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wally.CleanArchitecture.MicroService.Persistence.MySql.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterDatabase()
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
					name: "User",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
						Name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
							.Annotation("MySql:CharSet", "utf8mb4"),
						CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
						CreatedById =
							table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
						ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
						ModifiedById = table.Column<Guid>(
							type: "char(36)",
							nullable: true,
							collation: "ascii_general_ci")
					},
					constraints: table => { table.PrimaryKey("PK_User", x => x.Id); })
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateIndex(name: "IX_User_Name", table: "User", column: "Name", unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "User");
		}
	}
}
