#nullable enable

using ThymeToPlant.Models;

namespace ThymeToPlant.Repositories;

public interface ISeedRepository
{
    Task<List<Seed>> ListAsync(CancellationToken cancellationToken = default);
    Task<Seed?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Seed> AddAsync(Seed seed, CancellationToken cancellationToken = default);
    Task UpdateAsync(Seed seed, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Seed>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
