using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionRH.Migrations
{
    /// <inheritdoc />
    public partial class MakeDepartementIdReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postes_Departements_DepartementId",
                table: "Postes");

            migrationBuilder.AlterColumn<int>(
                name: "DepartementId",
                table: "Postes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Postes_Departements_DepartementId",
                table: "Postes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Postes_Departements_DepartementId",
                table: "Postes");

            migrationBuilder.AlterColumn<int>(
                name: "DepartementId",
                table: "Postes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Postes_Departements_DepartementId",
                table: "Postes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id");
        }
    }
}
