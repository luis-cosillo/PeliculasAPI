using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace PeliculasAPI.Migrations
{
    public partial class SalaDeCine_Ubicacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Ubicacion",
                table: "SalasDeCines",
                type: "geography",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "SalasDeCines");
        }
    }
}
