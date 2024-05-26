using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.MySql.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: "MicroService");
			
			migrationBuilder.AlterDatabase()
				.Annotation("MySql:CharSet", "utf8mb4");
			
			migrationBuilder.CreateTable(
					name: "User",
					schema: "MicroService",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
						Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
							.Annotation("MySql:CharSet", "utf8mb4"),
						IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
						DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
						DeletedById = table.Column<Guid>(type: "char(36)", nullable: true,
							collation: "ascii_general_ci"),
						CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
						CreatedById = table.Column<Guid>(type: "char(36)", nullable: false,
							collation: "ascii_general_ci"),
						ModifiedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
						ModifiedById = table.Column<Guid>(type: "char(36)", nullable: true,
							collation: "ascii_general_ci")
					},
					constraints: table => { table.PrimaryKey("PK_User", x => x.Id); })
				.Annotation("MySql:CharSet", "utf8mb4");
			
			migrationBuilder.CreateIndex(
				name: "IX_User_Name",
				schema: "MicroService",
				table: "User",
				column: "Name",
				unique: true,
				filter: "IsDeleted != 1");
		}
		
		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "User",
				schema: "MicroService");
		}
	}
}
