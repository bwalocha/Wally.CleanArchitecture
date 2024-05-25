using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer.Migrations
{
	/// <inheritdoc />
	public partial class UserAudit : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "CreatedAt",
				table: "User",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<Guid>(
				name: "CreatedById",
				table: "User",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.AddColumn<DateTime>(name: "ModifiedAt", table: "User", type: "datetime2", nullable: true);

			migrationBuilder.AddColumn<Guid>(
				name: "ModifiedById",
				table: "User",
				type: "uniqueidentifier",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(name: "CreatedAt", table: "User");

			migrationBuilder.DropColumn(name: "CreatedById", table: "User");

			migrationBuilder.DropColumn(name: "ModifiedAt", table: "User");

			migrationBuilder.DropColumn(name: "ModifiedById", table: "User");
		}
	}
}
