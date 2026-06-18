#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThymeToPlant.Models;
using ThymeToPlant.Repositories;

namespace ThymeToPlant.ViewModels;

public partial class AddSeedViewModel : ObservableObject
{
    private readonly ISeedRepository seedRepository;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCommonNameError))]
    private string commonName = string.Empty;

    [ObservableProperty]
    private string variety = string.Empty;

    [ObservableProperty]
    private string brand = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string notes = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCommonNameError))]
    private string commonNameError = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool isBusy;

    public bool HasCommonNameError => !string.IsNullOrEmpty(CommonNameError);

    public AddSeedViewModel(ISeedRepository seedRepository)
    {
        this.seedRepository = seedRepository;
    }

    private bool CanSave() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        CommonNameError = string.Empty;

        if (string.IsNullOrWhiteSpace(CommonName))
        {
            CommonNameError = "Common name is required.";
            return;
        }

        try
        {
            IsBusy = true;

            var seed = new Seed
            {
                CommonName = CommonName.Trim(),
                Variety = string.IsNullOrWhiteSpace(Variety) ? null : Variety.Trim(),
                Brand = string.IsNullOrWhiteSpace(Brand) ? null : Brand.Trim(),
                Category = string.IsNullOrWhiteSpace(Category) ? null : Category.Trim(),
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
            };

            await seedRepository.AddAsync(seed);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void ResetForm()
    {
        CommonName = string.Empty;
        Variety = string.Empty;
        Brand = string.Empty;
        Category = string.Empty;
        Notes = string.Empty;
        CommonNameError = string.Empty;
    }
}
