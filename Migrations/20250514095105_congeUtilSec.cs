using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionRH.Migrations
{
    /// <inheritdoc />
    public partial class congeUtilSec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DemandesConge_Employes_EmployeId",
                table: "DemandesConge");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeId",
                table: "DemandesConge",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DemandesConge_Employes_EmployeId",
                table: "DemandesConge",
                column: "EmployeId",
                principalTable: "Employes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DemandesConge_Employes_EmployeId",
                table: "DemandesConge");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeId",
                table: "DemandesConge",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DemandesConge_Employes_EmployeId",
                table: "DemandesConge",
                column: "EmployeId",
                principalTable: "Employes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
