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
            migrationBuilder.EnsureSchema(
                name: "MicroService");

            migrationBuilder.CreateTable(
                name: "CronTickers",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Expression = table.Column<string>(type: "TEXT", nullable: true),
                    Request = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Retries = table.Column<int>(type: "INTEGER", nullable: false),
                    RetryIntervals = table.Column<string>(type: "TEXT", nullable: true),
                    Function = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    InitIdentifier = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronTickers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceiveCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Consumed = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Delivered = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                schema: "MicroService",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LockId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Delivered = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "TimeTickers",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Function = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    InitIdentifier = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LockHolder = table.Column<string>(type: "TEXT", nullable: true),
                    Request = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LockedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "TEXT", nullable: true),
                    SkippedReason = table.Column<string>(type: "TEXT", nullable: true),
                    ElapsedTime = table.Column<long>(type: "INTEGER", nullable: false),
                    Retries = table.Column<int>(type: "INTEGER", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RetryIntervals = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RunCondition = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTickers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeTickers_TimeTickers_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "MicroService",
                        principalTable: "TimeTickers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    DeletedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "TEXT", precision: 7, nullable: true),
                    ModifiedById = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CronTickerOccurrences",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LockHolder = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CronTickerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "TEXT", nullable: true),
                    SkippedReason = table.Column<string>(type: "TEXT", nullable: true),
                    ElapsedTime = table.Column<long>(type: "INTEGER", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronTickerOccurrences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CronTickerOccurrences_CronTickers_CronTickerId",
                        column: x => x.CronTickerId,
                        principalSchema: "MicroService",
                        principalTable: "CronTickers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                schema: "MicroService",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EnqueueTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SentTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Headers = table.Column<string>(type: "TEXT", nullable: true),
                    Properties = table.Column<string>(type: "TEXT", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OutboxId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    ConversationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RequestId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceAddress = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalSchema: "MicroService",
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalSchema: "MicroService",
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_CronTickerId",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                column: "CronTickerId");

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_ExecutionTime",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                column: "ExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_Status_ExecutionTime",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "Status", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "UQ_CronTickerId_ExecutionTime",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "CronTickerId", "ExecutionTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CronTickers_Expression",
                schema: "MicroService",
                table: "CronTickers",
                column: "Expression");

            migrationBuilder.CreateIndex(
                name: "IX_Function_Expression",
                schema: "MicroService",
                table: "CronTickers",
                columns: new[] { "Function", "Expression" });

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                schema: "MicroService",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                schema: "MicroService",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                schema: "MicroService",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                schema: "MicroService",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                schema: "MicroService",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                schema: "MicroService",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_ExecutionTime",
                schema: "MicroService",
                table: "TimeTickers",
                column: "ExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_Status_ExecutionTime",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "Status", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTickers_ParentId",
                schema: "MicroService",
                table: "TimeTickers",
                column: "ParentId");

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
                name: "CronTickerOccurrences",
                schema: "MicroService");

            migrationBuilder.DropTable(
                name: "OutboxMessage",
                schema: "MicroService");

            migrationBuilder.DropTable(
                name: "TimeTickers",
                schema: "MicroService");

            migrationBuilder.DropTable(
                name: "User",
                schema: "MicroService");

            migrationBuilder.DropTable(
                name: "CronTickers",
                schema: "MicroService");

            migrationBuilder.DropTable(
                name: "InboxState",
                schema: "MicroService");

            migrationBuilder.DropTable(
                name: "OutboxState",
                schema: "MicroService");
        }
    }
}
