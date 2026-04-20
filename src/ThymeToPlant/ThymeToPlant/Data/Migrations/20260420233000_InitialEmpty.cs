using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ThymeToPlant.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260420233000_InitialEmpty")]
public partial class InitialEmpty : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 64);
#pragma warning restore 612, 618
    }
}
