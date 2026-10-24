using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GM.EntityFramework.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "domainEvents");

            migrationBuilder.EnsureSchema(
                name: "sample");

            migrationBuilder.CreateTable(
                name: "DomainEvents",
                schema: "domainEvents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate_type = table.Column<string>(type: "text", nullable: true),
                    aggregate_id = table.Column<string>(type: "text", nullable: true),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    occurred_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    client_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    channel_id = table.Column<string>(type: "text", nullable: true),
                    culture = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<string>(type: "text", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    idempotency_key = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_domain_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sample",
                schema: "sample",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sample", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sample_item",
                schema: "sample",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    sample_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sample_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_sample_item_sample_sample_id",
                        column: x => x.sample_id,
                        principalSchema: "sample",
                        principalTable: "sample",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_aggregate_type_aggregate_id_occurred_on",
                schema: "domainEvents",
                table: "DomainEvents",
                columns: new[] { "aggregate_type", "aggregate_id", "occurred_on" });

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_correlation_id",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "correlation_id");

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_tenant_id",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_user_id",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sample_id",
                schema: "sample",
                table: "sample",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_sample_item_id",
                schema: "sample",
                table: "sample_item",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_sample_item_sample_id",
                schema: "sample",
                table: "sample_item",
                column: "sample_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents",
                schema: "domainEvents");

            migrationBuilder.DropTable(
                name: "sample_item",
                schema: "sample");

            migrationBuilder.DropTable(
                name: "sample",
                schema: "sample");
        }
    }
}
