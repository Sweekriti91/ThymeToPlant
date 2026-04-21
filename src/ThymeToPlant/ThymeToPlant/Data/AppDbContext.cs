using Microsoft.EntityFrameworkCore;
using ThymeToPlant.Models;

namespace ThymeToPlant.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "thymetoplant.db");

    public DbSet<Seed> Seeds => Set<Seed>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seed>()
            .Property(seed => seed.CommonName)
            .IsRequired();
    }

    public static string GetDbPath(string baseDirectory)
        => Path.Combine(baseDirectory, "thymetoplant.db");
}
