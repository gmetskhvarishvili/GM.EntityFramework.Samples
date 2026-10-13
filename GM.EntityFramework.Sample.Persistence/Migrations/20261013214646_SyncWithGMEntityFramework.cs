using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.EntityFramework.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncWithGMEntityFramework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "domainEvents");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "sample",
                table: "SampleItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "sample",
                table: "SampleItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "sample",
                table: "Sample",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                schema: "sample",
                table: "Sample",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DomainEvents",
                schema: "domainEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateType = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "text", nullable: true),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChannelId = table.Column<string>(type: "text", nullable: true),
                    Culture = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    CorrelationId = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_AggregateType_AggregateId_OccurredOn",
                schema: "domainEvents",
                table: "DomainEvents",
                columns: new[] { "AggregateType", "AggregateId", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_CorrelationId",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_TenantId",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_UserId",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents",
                schema: "domainEvents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "sample",
                table: "SampleItem");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "sample",
                table: "SampleItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "sample",
                table: "Sample");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "sample",
                table: "Sample");
        }
    }
}
