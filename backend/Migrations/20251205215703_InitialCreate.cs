using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Circuit",
                columns: table => new
                {
                    CircuitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circuit", x => x.CircuitId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Component",
                columns: table => new
                {
                    ComponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ComponentType = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    CircuitId = table.Column<int>(type: "int", nullable: true),
                    Emf = table.Column<double>(type: "double", nullable: true),
                    Power = table.Column<double>(type: "double", nullable: true),
                    Resistance = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Component", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_Component_Circuit_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "Circuit",
                        principalColumn: "CircuitId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Wire",
                columns: table => new
                {
                    WireId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartId = table.Column<int>(type: "int", nullable: false),
                    EndId = table.Column<int>(type: "int", nullable: false),
                    CircuitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wire", x => x.WireId);
                    table.ForeignKey(
                        name: "FK_Wire_Circuit_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "Circuit",
                        principalColumn: "CircuitId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Component_CircuitId",
                table: "Component",
                column: "CircuitId");

            migrationBuilder.CreateIndex(
                name: "IX_Wire_CircuitId",
                table: "Wire",
                column: "CircuitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Component");

            migrationBuilder.DropTable(
                name: "Wire");

            migrationBuilder.DropTable(
                name: "Circuit");
        }
    }
}
