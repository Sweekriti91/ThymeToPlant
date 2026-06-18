using ThymeToPlant.Models;
using ThymeToPlant.Repositories;
using ThymeToPlant.ViewModels;

namespace ThymeToTest;

public class AddSeedViewModelTests
{
    [Test]
    public void SaveCommand_CannotExecute_WhenCommonNameIsEmpty()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        viewModel.CommonName = string.Empty;
        var canSave = viewModel.SaveCommand.CanExecute(null);

        Assert.That(canSave, Is.False);
    }

    [Test]
    public void SaveCommand_CanExecute_WhenCommonNameIsProvided()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        viewModel.CommonName = "Basil";
        var canSave = viewModel.SaveCommand.CanExecute(null);

        Assert.That(canSave, Is.True);
    }

    [Test]
    public void SaveCommand_CannotExecute_WhenCommonNameIsWhitespace()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        viewModel.CommonName = "   ";
        var canSave = viewModel.SaveCommand.CanExecute(null);

        Assert.That(canSave, Is.False);
    }

    [Test]
    public void ResetForm_ClearsAllFields()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository)
        {
            CommonName = "Basil",
            Variety = "Genovese",
            Brand = "Seed Co",
            Category = "Herb",
            Notes = "Some notes"
        };

        viewModel.ResetForm();

        Assert.That(viewModel.CommonName, Is.Empty);
        Assert.That(viewModel.Variety, Is.Empty);
        Assert.That(viewModel.Brand, Is.Empty);
        Assert.That(viewModel.Category, Is.Empty);
        Assert.That(viewModel.Notes, Is.Empty);
        Assert.That(viewModel.CommonNameError, Is.Empty);
    }

    [Test]
    public void HasCommonNameError_IsFalse_Initially()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        Assert.That(viewModel.HasCommonNameError, Is.False);
    }

    [Test]
    public void CommonNameError_NotifyChangesHasCommonNameError()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        var hasErrorValues = new List<bool>();
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(viewModel.HasCommonNameError))
            {
                hasErrorValues.Add(viewModel.HasCommonNameError);
            }
        };

        viewModel.CommonNameError = "Common name is required.";

        Assert.That(hasErrorValues, Contains.Item(true));
    }

    private sealed class FakeSeedRepository : ISeedRepository
    {
        private readonly List<Seed> seeds = [];

        public Task<List<Seed>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(seeds.ToList());

        public Task<Seed?> GetAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(seeds.FirstOrDefault(s => s.Id == id));

        public Task<Seed> AddAsync(Seed seed, CancellationToken cancellationToken = default)
        {
            seed.Id = Guid.NewGuid();
            seed.CreatedUtc = DateTime.UtcNow;
            seed.UpdatedUtc = DateTime.UtcNow;
            seeds.Add(seed);
            return Task.FromResult(seed);
        }

        public Task UpdateAsync(Seed seed, CancellationToken cancellationToken = default)
        {
            var existing = seeds.FirstOrDefault(s => s.Id == seed.Id);
            if (existing is not null)
            {
                seeds.Remove(existing);
                seeds.Add(seed);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            seeds.RemoveAll(s => s.Id == id);
            return Task.CompletedTask;
        }

        public Task<List<Seed>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
            => Task.FromResult(seeds.Where(s => s.CommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList());
    }
}
