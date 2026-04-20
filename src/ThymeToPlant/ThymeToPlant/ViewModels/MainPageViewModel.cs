using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System.Text.Json;
using ThymeToPlant.Models;
using ThymeToPlant.Services;

namespace ThymeToPlant.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private const string CachedZipCodeKey = "home.cachedZipCode";
    private const string CachedPlantZoneDataKey = "home.cachedPlantZoneData";

    private readonly PlantZoneService plantZoneService;
    private readonly IPreferences preferences;

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

    public MainPageViewModel(PlantZoneService plantZoneService, IPreferences preferences)
    {
        this.plantZoneService = plantZoneService;
        this.preferences = preferences;
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
                return;
            }

            preferences.Set(CachedZipCodeKey, ZipCode);
            preferences.Set(CachedPlantZoneDataKey, JsonSerializer.Serialize(result));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            SearchResult = string.Empty;
            ErrorMessage = "Unable to look up plant zone right now. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void RestoreCachedZone()
    {
        ZipCode = preferences.Get(CachedZipCodeKey, string.Empty);

        var cachedZoneJson = preferences.Get(CachedPlantZoneDataKey, string.Empty);
        if (string.IsNullOrWhiteSpace(cachedZoneJson))
        {
            SearchResult = string.Empty;
            return;
        }

        try
        {
            var cachedZoneData = JsonSerializer.Deserialize<PlantZoneDataItem>(cachedZoneJson);
            SearchResult = cachedZoneData?.Zone ?? string.Empty;
        }
        catch (JsonException)
        {
            SearchResult = string.Empty;
        }
    }
}
