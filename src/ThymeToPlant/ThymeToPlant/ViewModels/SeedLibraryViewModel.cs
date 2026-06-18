#nullable enable

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThymeToPlant.Models;
using ThymeToPlant.Repositories;

namespace ThymeToPlant.ViewModels;

public partial class SeedLibraryViewModel : ObservableObject
{
    private readonly ISeedRepository seedRepository;
    private List<Seed> allSeeds = new();

    public SeedLibraryViewModel(ISeedRepository seedRepository)
    {
        this.seedRepository = seedRepository;
    }

    public ObservableCollection<Seed> Seeds { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private string searchText = string.Empty;

    public bool IsEmpty => !IsBusy && Seeds.Count == 0;

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            allSeeds = await seedRepository.ListAsync();
            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task Refresh() => LoadAsync();

    private void ApplyFilter()
    {
        var query = SearchText?.Trim() ?? string.Empty;

        IEnumerable<Seed> filtered = allSeeds;
        if (!string.IsNullOrEmpty(query))
        {
            filtered = allSeeds.Where(seed =>
                Contains(seed.CommonName, query) ||
                Contains(seed.Variety, query) ||
                Contains(seed.Brand, query));
        }

        Seeds.Clear();
        foreach (var seed in filtered)
        {
            Seeds.Add(seed);
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    private static bool Contains(string? source, string query)
    {
        if (string.IsNullOrEmpty(source))
        {
            return false;
        }

        return source.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
}
