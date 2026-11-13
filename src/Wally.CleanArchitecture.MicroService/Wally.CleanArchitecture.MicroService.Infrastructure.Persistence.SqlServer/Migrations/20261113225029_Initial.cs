using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer.Migrations
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
                    ApplicationNamespaceKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeedKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SeedOwnerNamespace = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SeedManifestEpoch = table.Column<long>(type: "bigint", nullable: true),
                    DefinitionRevision = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    SeedLastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetirementRequestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeedWasEnabledBeforeRetirement = table.Column<bool>(type: "bit", nullable: true),
                    Chain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expression = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Request = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Retries = table.Column<int>(type: "int", nullable: false),
                    RetryIntervals = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkipIfAlreadyRunning = table.Column<bool>(type: "bit", nullable: false),
                    CatchUpMode = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemPaused = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OnStale = table.Column<int>(type: "int", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: true),
                    Function = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestContractVersion = table.Column<int>(type: "int", nullable: true),
                    RequestContractFingerprint = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SchedulePolicyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SchedulePolicyVersion = table.Column<long>(type: "bigint", nullable: true),
                    BlockedOccurrenceBehavior = table.Column<int>(type: "int", nullable: true),
                    MissedExecutionAction = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaximumLateness = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronTickers", x => new { x.ApplicationNamespaceKey, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
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
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "TickerQNodes",
                schema: "MicroService",
                columns: table => new
                {
                    ApplicationNamespaceKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NodeKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExecutionOwnerId = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    NodeIdentifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DashboardUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    MaxConcurrency = table.Column<int>(type: "int", nullable: false),
                    FunctionCount = table.Column<int>(type: "int", nullable: false),
                    ActiveExecutions = table.Column<int>(type: "int", nullable: false),
                    QueuedExecutions = table.Column<int>(type: "int", nullable: false),
                    ActiveWorkflows = table.Column<int>(type: "int", nullable: false),
                    WaitingForFunction = table.Column<int>(type: "int", nullable: false),
                    RetryingExecutions = table.Column<int>(type: "int", nullable: false),
                    ActivationEpoch = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickerQNodes", x => new { x.ApplicationNamespaceKey, x.NodeKey });
                });

            migrationBuilder.CreateTable(
                name: "TimeTickers",
                schema: "MicroService",
                columns: table => new
                {
                    ApplicationNamespaceKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Function = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestContractVersion = table.Column<int>(type: "int", nullable: true),
                    RequestContractFingerprint = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SchedulePolicyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SchedulePolicyVersion = table.Column<long>(type: "bigint", nullable: true),
                    BlockedOccurrenceBehavior = table.Column<int>(type: "int", nullable: true),
                    MissedExecutionAction = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaximumLateness = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LockHolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Request = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduleAdjustmentReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkippedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElapsedTime = table.Column<long>(type: "bigint", nullable: false),
                    ProgressPercent = table.Column<double>(type: "float", nullable: true),
                    ProgressMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProgressUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Retries = table.Column<int>(type: "int", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    RetryIntervals = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RunCondition = table.Column<int>(type: "int", nullable: true),
                    DeleteBehavior = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LeaseUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcquisitionToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChainRootId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OriginCronTickerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChainGeneration = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChainRetryFromId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChainRetryPreviousStatus = table.Column<int>(type: "int", nullable: true),
                    OnStale = table.Column<int>(type: "int", nullable: false),
                    StaleRestartCount = table.Column<int>(type: "int", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTickers", x => new { x.ApplicationNamespaceKey, x.Id });
                    table.ForeignKey(
                        name: "FK_TimeTickers_TimeTickers_ApplicationNamespaceKey_ParentId",
                        columns: x => new { x.ApplicationNamespaceKey, x.ParentId },
                        principalSchema: "MicroService",
                        principalTable: "TimeTickers",
                        principalColumns: new[] { "ApplicationNamespaceKey", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "MicroService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    ApplicationNamespaceKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LockHolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CronTickerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChainRootId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefinitionRevision = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkippedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElapsedTime = table.Column<long>(type: "bigint", nullable: false),
                    ProgressPercent = table.Column<double>(type: "float", nullable: true),
                    ProgressMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProgressUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaseUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaleRestartCount = table.Column<int>(type: "int", nullable: false),
                    AcquisitionToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronTickerOccurrences", x => new { x.ApplicationNamespaceKey, x.Id });
                    table.ForeignKey(
                        name: "FK_CronTickerOccurrences_CronTickers_ApplicationNamespaceKey_CronTickerId",
                        columns: x => new { x.ApplicationNamespaceKey, x.CronTickerId },
                        principalSchema: "MicroService",
                        principalTable: "CronTickers",
                        principalColumns: new[] { "ApplicationNamespaceKey", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                schema: "MicroService",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnqueueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_CronOccurrence_DefinitionRevision_Status",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "ApplicationNamespaceKey", "CronTickerId", "DefinitionRevision", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_CronTickerId",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "ApplicationNamespaceKey", "CronTickerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_ExecutionTime",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "ApplicationNamespaceKey", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_Status_ExecutedAt",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "ApplicationNamespaceKey", "Status", "ExecutedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickerOccurrence_Status_ExecutionTime",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "ApplicationNamespaceKey", "Status", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "UQ_CronTickerId_ExecutionTime",
                schema: "MicroService",
                table: "CronTickerOccurrences",
                columns: new[] { "ApplicationNamespaceKey", "CronTickerId", "ExecutionTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CronTickers_Expression",
                schema: "MicroService",
                table: "CronTickers",
                columns: new[] { "ApplicationNamespaceKey", "Expression" });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickers_SchedulePolicy",
                schema: "MicroService",
                table: "CronTickers",
                columns: new[] { "ApplicationNamespaceKey", "SchedulePolicyName" });

            migrationBuilder.CreateIndex(
                name: "IX_CronTickers_SeedOwner_Epoch",
                schema: "MicroService",
                table: "CronTickers",
                columns: new[] { "ApplicationNamespaceKey", "SeedOwnerNamespace", "SeedManifestEpoch" });

            migrationBuilder.CreateIndex(
                name: "IX_Function_Expression",
                schema: "MicroService",
                table: "CronTickers",
                columns: new[] { "ApplicationNamespaceKey", "Function", "Expression" });

            migrationBuilder.CreateIndex(
                name: "UX_CronTickers_SeedKey",
                schema: "MicroService",
                table: "CronTickers",
                columns: new[] { "ApplicationNamespaceKey", "SeedKey" },
                unique: true,
                filter: "[SeedKey] IS NOT NULL");

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
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                schema: "MicroService",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                schema: "MicroService",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_TickerQNodes_ApplicationNamespaceKey_LastSeenAtUtc",
                schema: "MicroService",
                table: "TickerQNodes",
                columns: new[] { "ApplicationNamespaceKey", "LastSeenAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_ChainRootId",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "ChainRootId" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_ExecutionTime",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_SchedulePolicy",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "SchedulePolicyName" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_Status_ExecutedAt",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "Status", "ExecutedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTicker_Status_ExecutionTime",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "Status", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTickers_ApplicationNamespaceKey_OriginCronTickerId_Status",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "OriginCronTickerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTickers_ApplicationNamespaceKey_ParentId",
                schema: "MicroService",
                table: "TimeTickers",
                columns: new[] { "ApplicationNamespaceKey", "ParentId" });

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
                name: "TickerQNodes",
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
