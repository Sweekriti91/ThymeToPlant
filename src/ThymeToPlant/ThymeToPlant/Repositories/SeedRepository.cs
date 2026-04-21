#nullable enable

using Microsoft.EntityFrameworkCore;
using ThymeToPlant.Data;
using ThymeToPlant.Models;

namespace ThymeToPlant.Repositories;

public class SeedRepository : ISeedRepository
{
    private readonly AppDbContext dbContext;

    public SeedRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<List<Seed>> ListAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Seeds
            .AsNoTracking()
            .OrderBy(seed => seed.CommonName)
            .ThenBy(seed => seed.Variety)
            .ToListAsync(cancellationToken);
    }

    public Task<Seed?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Seeds
            .FirstOrDefaultAsync(seed => seed.Id == id, cancellationToken);
    }

    public async Task<Seed> AddAsync(Seed seed, CancellationToken cancellationToken = default)
    {
        if (seed.Id == Guid.Empty)
        {
            seed.Id = Guid.NewGuid();
        }

        var now = DateTime.UtcNow;
        seed.CreatedUtc = now;
        seed.UpdatedUtc = now;

        await dbContext.Seeds.AddAsync(seed, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return seed;
    }

    public async Task UpdateAsync(Seed seed, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Seeds.FirstOrDefaultAsync(item => item.Id == seed.Id, cancellationToken);
        if (existing is null)
        {
            return;
        }

        existing.CommonName = seed.CommonName;
        existing.Variety = seed.Variety;
        existing.Brand = seed.Brand;
        existing.Category = seed.Category;
        existing.Barcode = seed.Barcode;
        existing.PurchaseDate = seed.PurchaseDate;
        existing.ExpiryDate = seed.ExpiryDate;
        existing.QuantityRemaining = seed.QuantityRemaining;
        existing.PhotoPath = seed.PhotoPath;
        existing.Notes = seed.Notes;
        existing.UpdatedUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Seeds.FirstOrDefaultAsync(seed => seed.Id == id, cancellationToken);
        if (existing is null)
        {
            return;
        }

        dbContext.Seeds.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Seed>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            return ListAsync(cancellationToken);
        }

        return dbContext.Seeds
            .AsNoTracking()
            .Where(seed =>
                EF.Functions.Like(seed.CommonName, $"%{normalizedSearchTerm}%") ||
                (seed.Variety != null && EF.Functions.Like(seed.Variety, $"%{normalizedSearchTerm}%")) ||
                (seed.Brand != null && EF.Functions.Like(seed.Brand, $"%{normalizedSearchTerm}%")) ||
                (seed.Category != null && EF.Functions.Like(seed.Category, $"%{normalizedSearchTerm}%")) ||
                (seed.Barcode != null && EF.Functions.Like(seed.Barcode, $"%{normalizedSearchTerm}%")))
            .OrderBy(seed => seed.CommonName)
            .ThenBy(seed => seed.Variety)
            .ToListAsync(cancellationToken);
    }
}
