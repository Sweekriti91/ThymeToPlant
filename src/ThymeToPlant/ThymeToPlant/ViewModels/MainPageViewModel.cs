using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThymeToPlant.Services;

namespace ThymeToPlant.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly PlantZoneService plantZoneService;

    [ObservableProperty]
    private string zipCode = string.Empty;

    [ObservableProperty]
    private string searchResult = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(FindPlantZoneCommand))]
    private bool isBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public MainPageViewModel(PlantZoneService plantZoneService)
    {
        this.plantZoneService = plantZoneService;
    }

    private bool CanFindPlantZone() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanFindPlantZone))]
    private async Task FindPlantZone()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(ZipCode))
        {
            SearchResult = string.Empty;
            ErrorMessage = "Please enter a valid ZIP code.";
            return;
        }

        try
        {
            IsBusy = true;
            var result = await plantZoneService.GetZoneByZip(ZipCode);
            SearchResult = result?.Zone ?? string.Empty;
            if (string.IsNullOrWhiteSpace(SearchResult))
            {
                ErrorMessage = "Could not find a plant zone. Check the ZIP code and try again.";
            }
        }
        catch (HttpRequestException)
        {
            SearchResult = string.Empty;
            ErrorMessage = "Unable to look up plant zone right now. Please try again.";
        }
        catch (TaskCanceledException)
        {
            SearchResult = string.Empty;
            ErrorMessage = "Unable to look up plant zone right now. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
