using ThymeToPlant.Models;
using ThymeToPlant.Repositories;
using ThymeToPlant.ViewModels;

namespace ThymeToTest;

public class AddSeedViewModelTests
{
    [Test]
    public void SaveCommand_CanExecute_WhenNotBusy()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        Assert.That(viewModel.SaveCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void SaveCommand_CannotExecute_WhenBusy()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository) { IsBusy = true };

        Assert.That(viewModel.SaveCommand.CanExecute(null), Is.False);
    }

    [Test]
    public async Task Save_SetsCommonNameError_WhenCommonNameIsEmpty()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        viewModel.CommonName = string.Empty;
        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.That(viewModel.CommonNameError, Is.Not.Empty);
        Assert.That(viewModel.HasCommonNameError, Is.True);
    }

    [Test]
    public async Task Save_SetsCommonNameError_WhenCommonNameIsWhitespace()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        viewModel.CommonName = "   ";
        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.That(viewModel.CommonNameError, Is.Not.Empty);
        Assert.That(viewModel.HasCommonNameError, Is.True);
        Assert.That(repository.Seeds, Is.Empty);
    }

    [Test]
    public async Task Save_DoesNotCallRepository_WhenCommonNameIsEmpty()
    {
        var repository = new FakeSeedRepository();
        var viewModel = new AddSeedViewModel(repository);

        viewModel.CommonName = string.Empty;
        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.That(repository.Seeds, Is.Empty);
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

    internal sealed class FakeSeedRepository : ISeedRepository
    {
        public List<Seed> Seeds { get; } = [];

        public Task<List<Seed>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Seeds.ToList());

        public Task<Seed?> GetAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Seeds.FirstOrDefault(s => s.Id == id));

        public Task<Seed> AddAsync(Seed seed, CancellationToken cancellationToken = default)
        {
            seed.Id = Guid.NewGuid();
            seed.CreatedUtc = DateTime.UtcNow;
            seed.UpdatedUtc = DateTime.UtcNow;
            Seeds.Add(seed);
            return Task.FromResult(seed);
        }

        public Task UpdateAsync(Seed seed, CancellationToken cancellationToken = default)
        {
            var existing = Seeds.FirstOrDefault(s => s.Id == seed.Id);
            if (existing is not null)
            {
                Seeds.Remove(existing);
                Seeds.Add(seed);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Seeds.RemoveAll(s => s.Id == id);
            return Task.CompletedTask;
        }

        public Task<List<Seed>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
            => Task.FromResult(Seeds.Where(s => s.CommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList());
    }
}
