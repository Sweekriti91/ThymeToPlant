using Microsoft.EntityFrameworkCore;

namespace ThymeToPlant.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "thymetoplant.db");

    public static string GetDbPath(string baseDirectory)
        => Path.Combine(baseDirectory, "thymetoplant.db");
}
