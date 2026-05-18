using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empen.Migrations.PersonDb
{
    /// <inheritdoc />
    public partial class fixPersonStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "flim",
                table: "person_status",
                newName: "film");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "film",
                table: "person_status",
                newName: "flim");
        }
    }
}
