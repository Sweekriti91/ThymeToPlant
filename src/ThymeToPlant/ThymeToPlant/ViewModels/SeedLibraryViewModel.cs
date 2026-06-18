#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ThymeToPlant.Models;
using ThymeToPlant.Repositories;
using ThymeToPlant.Views;

namespace ThymeToPlant.ViewModels;

public partial class SeedLibraryViewModel : ObservableObject
{
    private readonly ISeedRepository seedRepository;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private ObservableCollection<Seed> seeds = [];

    [ObservableProperty]
    private bool isBusy;

    public bool IsEmpty => Seeds.Count == 0;

    public SeedLibraryViewModel(ISeedRepository seedRepository)
    {
        this.seedRepository = seedRepository;
    }

    [RelayCommand]
    private async Task LoadSeeds()
    {
        try
        {
            IsBusy = true;
            var list = await seedRepository.ListAsync();
            Seeds = new ObservableCollection<Seed>(list);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddSeed()
    {
        await Shell.Current.GoToAsync(nameof(AddSeedPage));
    }
}

