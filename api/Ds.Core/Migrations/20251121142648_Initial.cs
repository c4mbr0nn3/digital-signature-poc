using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ds.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    customer_id = table.Column<int>(type: "INTEGER", nullable: false),
                    public_key = table.Column<byte[]>(type: "BLOB", nullable: false),
                    encrypted_private_key = table.Column<byte[]>(type: "BLOB", nullable: false),
                    private_key_salt = table.Column<byte[]>(type: "BLOB", nullable: false),
                    iv = table.Column<byte[]>(type: "BLOB", nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    superseded_at = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    active_key_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                    table.ForeignKey(
                        name: "FK_customers_customer_keys_active_key_id",
                        column: x => x.active_key_id,
                        principalTable: "customer_keys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trade_recommendations",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    customer_id = table.Column<int>(type: "INTEGER", nullable: false),
                    metadata = table.Column<string>(type: "TEXT", nullable: false),
                    signed_action = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    signed_at = table.Column<long>(type: "INTEGER", nullable: true),
                    signing_key_id = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trade_recommendations", x => x.id);
                    table.ForeignKey(
                        name: "FK_trade_recommendations_customer_keys_signing_key_id",
                        column: x => x.signing_key_id,
                        principalTable: "customer_keys",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_trade_recommendations_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_keys_customer_id",
                table: "customer_keys",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_active_key_id",
                table: "customers",
                column: "active_key_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_email",
                table: "customers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trade_recommendations_customer_id",
                table: "trade_recommendations",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_trade_recommendations_signing_key_id",
                table: "trade_recommendations",
                column: "signing_key_id");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_keys_customers_customer_id",
                table: "customer_keys",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_keys_customers_customer_id",
                table: "customer_keys");

            migrationBuilder.DropTable(
                name: "trade_recommendations");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "customer_keys");
        }
    }
}
