using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionRH.Migrations
{
    /// <inheritdoc />
    public partial class Entretientri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entretiens_AspNetUsers_UtilisateurId",
                table: "Entretiens");

            migrationBuilder.AddForeignKey(
                name: "FK_Entretiens_AspNetUsers_UtilisateurId",
                table: "Entretiens",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entretiens_AspNetUsers_UtilisateurId",
                table: "Entretiens");

            migrationBuilder.AddForeignKey(
                name: "FK_Entretiens_AspNetUsers_UtilisateurId",
                table: "Entretiens",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
