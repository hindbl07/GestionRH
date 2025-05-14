using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionRH.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartementToPoste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartementId",
                table: "Postes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Postes_DepartementId",
                table: "Postes",
                column: "DepartementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Postes_Departements_DepartementId",
                table: "Postes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postes_Departements_DepartementId",
                table: "Postes");

            migrationBuilder.DropIndex(
                name: "IX_Postes_DepartementId",
                table: "Postes");

            migrationBuilder.DropColumn(
                name: "DepartementId",
                table: "Postes");
        }
    }
}
