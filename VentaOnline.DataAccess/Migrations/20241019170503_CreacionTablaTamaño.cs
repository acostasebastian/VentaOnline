using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentaOnline.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionTablaTamaño : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tamanio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tamanio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tamanios_Nombre",
                table: "Tamanio",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tamanio");
        }
    }
}
