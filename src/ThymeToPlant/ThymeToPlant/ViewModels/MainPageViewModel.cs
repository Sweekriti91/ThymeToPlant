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

    public MainPageViewModel(PlantZoneService plantZoneService)
    {
        this.plantZoneService = plantZoneService;
    }

    [RelayCommand]
    private async Task FindPlantZone()
    {
        if (string.IsNullOrWhiteSpace(ZipCode))
        {
            SearchResult = string.Empty;
            return;
        }

        var result = await plantZoneService.GetZoneByZip(ZipCode);
        SearchResult = result?.Zone ?? string.Empty;
    }
}
