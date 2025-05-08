using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionRH.Migrations
{
    /// <inheritdoc />
    public partial class RendreChampsOptionnels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes");

            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Postes_PosteId",
                table: "Employes");

            migrationBuilder.AlterColumn<int>(
                name: "PosteId",
                table: "Employes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DepartementId",
                table: "Employes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Postes_PosteId",
                table: "Employes",
                column: "PosteId",
                principalTable: "Postes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes");

            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Postes_PosteId",
                table: "Employes");

            migrationBuilder.AlterColumn<int>(
                name: "PosteId",
                table: "Employes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DepartementId",
                table: "Employes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Departements_DepartementId",
                table: "Employes",
                column: "DepartementId",
                principalTable: "Departements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Postes_PosteId",
                table: "Employes",
                column: "PosteId",
                principalTable: "Postes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
