using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ThymeToPlant.Data;
using ThymeToPlant.Models;
using ThymeToPlant.Repositories;

namespace ThymeToTest;

public class SeedRepositoryTests
{
    [Test]
    public async Task AddAndGetAsync_PersistsSeedAndSetsTimestamps()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = CreateContext(connection);
        var repository = new SeedRepository(context);

        var created = await repository.AddAsync(new Seed
        {
            CommonName = "Basil",
            Variety = "Genovese",
            Brand = "Seed Co"
        });

        var fetched = await repository.GetAsync(created.Id);

        Assert.That(created.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(created.CreatedUtc, Is.Not.EqualTo(default(DateTime)));
        Assert.That(created.UpdatedUtc, Is.EqualTo(created.CreatedUtc));
        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched!.CommonName, Is.EqualTo("Basil"));
        Assert.That(fetched.Variety, Is.EqualTo("Genovese"));
    }

    [Test]
    public async Task SearchAsync_FiltersByConfiguredFields()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = CreateContext(connection);
        var repository = new SeedRepository(context);

        await repository.AddAsync(new Seed { CommonName = "Basil", Brand = "Green House" });
        await repository.AddAsync(new Seed { CommonName = "Tomato", Variety = "Beefsteak" });
        await repository.AddAsync(new Seed { CommonName = "Carrot", Category = "Root" });

        var varietyResult = await repository.SearchAsync("beef");
        var categoryResult = await repository.SearchAsync("root");

        Assert.That(varietyResult.Select(seed => seed.CommonName), Is.EquivalentTo(new[] { "Tomato" }));
        Assert.That(categoryResult.Select(seed => seed.CommonName), Is.EquivalentTo(new[] { "Carrot" }));
    }

    [Test]
    public async Task UpdateAndDeleteAsync_ModifiesAndRemovesSeed()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = CreateContext(connection);
        var repository = new SeedRepository(context);

        var seed = await repository.AddAsync(new Seed { CommonName = "Pepper", QuantityRemaining = 10 });
        var originalCreatedUtc = seed.CreatedUtc;

        seed.CommonName = "Bell Pepper";
        seed.QuantityRemaining = 8;
        await repository.UpdateAsync(seed);

        var updated = await repository.GetAsync(seed.Id);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.CommonName, Is.EqualTo("Bell Pepper"));
        Assert.That(updated.QuantityRemaining, Is.EqualTo(8));
        Assert.That(updated.CreatedUtc, Is.EqualTo(originalCreatedUtc));
        Assert.That(updated.UpdatedUtc, Is.GreaterThanOrEqualTo(originalCreatedUtc));

        await repository.DeleteAsync(seed.Id);
        var deleted = await repository.GetAsync(seed.Id);
        Assert.That(deleted, Is.Null);
    }

    private static AppDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
