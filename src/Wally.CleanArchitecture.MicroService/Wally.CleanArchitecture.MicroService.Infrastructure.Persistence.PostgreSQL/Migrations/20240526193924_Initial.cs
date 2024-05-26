using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.PostgreSQL.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: "MicroService");
			
			migrationBuilder.CreateTable(
				name: "User",
				schema: "MicroService",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
					IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
					DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
					DeletedById = table.Column<Guid>(type: "uuid", nullable: true),
					CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
					ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
					ModifiedById = table.Column<Guid>(type: "uuid", nullable: true)
				},
				constraints: table => { table.PrimaryKey("PK_User", x => x.Id); });
			
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
