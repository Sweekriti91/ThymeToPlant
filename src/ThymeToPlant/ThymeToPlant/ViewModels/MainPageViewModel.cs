using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThymeToPlant.Services;

namespace ThymeToPlant.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly PlantZoneService plantZoneService;

    [ObservableProperty]
    private string zipCode;

    [ObservableProperty]
    private string searchResult;

    public MainPageViewModel(PlantZoneService plantZoneService)
    {
        this.plantZoneService = plantZoneService;
    }

    [RelayCommand]
    private async Task FindPlantZone()
    {
        var result = await plantZoneService.GetZoneByZip(ZipCode);
        SearchResult = result?.Zone;
    }
}
