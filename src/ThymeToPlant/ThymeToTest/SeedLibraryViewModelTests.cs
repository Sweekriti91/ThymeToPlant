#nullable enable

using ThymeToPlant.Models;
using ThymeToPlant.Repositories;
using ThymeToPlant.ViewModels;

namespace ThymeToTest;

public class SeedLibraryViewModelTests
{
    [Test]
    public async Task LoadAsync_PopulatesSeedsAndClearsEmpty()
    {
        var repo = new FakeSeedRepository(new[]
        {
            new Seed { CommonName = "Basil", Variety = "Genovese", Brand = "Burpee" },
            new Seed { CommonName = "Tomato", Variety = "Roma", Brand = "Botanical" }
        });
        var vm = new SeedLibraryViewModel(repo);

        Assert.That(vm.IsEmpty, Is.True);

        await vm.LoadAsync();

        Assert.That(vm.Seeds, Has.Count.EqualTo(2));
        Assert.That(vm.IsBusy, Is.False);
        Assert.That(vm.IsEmpty, Is.False);
    }

    [Test]
    public async Task SearchText_FiltersByCommonNameVarietyOrBrand_CaseInsensitive()
    {
        var repo = new FakeSeedRepository(new[]
        {
            new Seed { CommonName = "Basil", Variety = "Genovese", Brand = "Burpee" },
            new Seed { CommonName = "Tomato", Variety = "Roma", Brand = "Botanical" },
            new Seed { CommonName = "Carrot", Variety = "Nantes", Brand = "Burpee" }
        });
        var vm = new SeedLibraryViewModel(repo);
        await vm.LoadAsync();

        vm.SearchText = "burpee";
        Assert.That(vm.Seeds.Select(s => s.CommonName), Is.EquivalentTo(new[] { "Basil", "Carrot" }));

        vm.SearchText = "ROMA";
        Assert.That(vm.Seeds.Select(s => s.CommonName), Is.EquivalentTo(new[] { "Tomato" }));

        vm.SearchText = "basil";
        Assert.That(vm.Seeds.Select(s => s.CommonName), Is.EquivalentTo(new[] { "Basil" }));

        vm.SearchText = string.Empty;
        Assert.That(vm.Seeds, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task IsEmpty_FlipsTrue_WhenFilterMatchesNothing()
    {
        var repo = new FakeSeedRepository(new[]
        {
            new Seed { CommonName = "Basil" }
        });
        var vm = new SeedLibraryViewModel(repo);
        await vm.LoadAsync();

        Assert.That(vm.IsEmpty, Is.False);

        vm.SearchText = "no-such-seed";

        Assert.That(vm.Seeds, Is.Empty);
        Assert.That(vm.IsEmpty, Is.True);
    }

    [Test]
    public async Task IsEmpty_FalseWhileLoading_EvenWithNoSeeds()
    {
        var gate = new TaskCompletionSource<List<Seed>>();
        var repo = new FakeSeedRepository(_ => gate.Task);
        var vm = new SeedLibraryViewModel(repo);

        var loadTask = vm.LoadAsync();

        Assert.That(vm.IsBusy, Is.True);
        Assert.That(vm.IsEmpty, Is.False);

        gate.SetResult(new List<Seed>());
        await loadTask;

        Assert.That(vm.IsBusy, Is.False);
        Assert.That(vm.IsEmpty, Is.True);
    }
}

internal sealed class FakeSeedRepository : ISeedRepository
{
    private readonly List<Seed> seeds;
    private readonly Func<CancellationToken, Task<List<Seed>>>? listFactory;

    public FakeSeedRepository(IEnumerable<Seed> seeds)
    {
        this.seeds = seeds.ToList();
    }

    public FakeSeedRepository(Func<CancellationToken, Task<List<Seed>>> listFactory)
    {
        this.seeds = new List<Seed>();
        this.listFactory = listFactory;
    }

    public Task<List<Seed>> ListAsync(CancellationToken cancellationToken = default)
    {
        if (listFactory is not null)
        {
            return listFactory(cancellationToken);
        }

        return Task.FromResult(seeds.ToList());
    }

    public Task<Seed?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult<Seed?>(seeds.FirstOrDefault(s => s.Id == id));

    public Task<Seed> AddAsync(Seed seed, CancellationToken cancellationToken = default)
    {
        if (seed.Id == Guid.Empty)
        {
            seed.Id = Guid.NewGuid();
        }
        seeds.Add(seed);
        return Task.FromResult(seed);
    }

    public Task UpdateAsync(Seed seed, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        seeds.RemoveAll(s => s.Id == id);
        return Task.CompletedTask;
    }

    public Task<List<Seed>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => Task.FromResult(seeds.ToList());
}
