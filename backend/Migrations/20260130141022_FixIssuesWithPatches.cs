using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FixIssuesWithPatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Circuit_CircuitId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_Wire_Circuit_CircuitId",
                table: "Wire");

            migrationBuilder.AlterColumn<int>(
                name: "CircuitId",
                table: "Wire",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WireId",
                table: "Wire",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "CircuitId",
                table: "Component",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Circuit_CircuitId",
                table: "Component",
                column: "CircuitId",
                principalTable: "Circuit",
                principalColumn: "CircuitId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wire_Circuit_CircuitId",
                table: "Wire",
                column: "CircuitId",
                principalTable: "Circuit",
                principalColumn: "CircuitId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Component_Circuit_CircuitId",
                table: "Component");

            migrationBuilder.DropForeignKey(
                name: "FK_Wire_Circuit_CircuitId",
                table: "Wire");

            migrationBuilder.AlterColumn<int>(
                name: "CircuitId",
                table: "Wire",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "WireId",
                table: "Wire",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "CircuitId",
                table: "Component",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Component_Circuit_CircuitId",
                table: "Component",
                column: "CircuitId",
                principalTable: "Circuit",
                principalColumn: "CircuitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wire_Circuit_CircuitId",
                table: "Wire",
                column: "CircuitId",
                principalTable: "Circuit",
                principalColumn: "CircuitId");
        }
    }
}
