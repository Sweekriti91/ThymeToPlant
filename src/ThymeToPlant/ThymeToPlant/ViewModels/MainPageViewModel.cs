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
    private const string LastUpdatedDisplayFormat = "yyyy-MM-dd HH:mm:ss 'UTC'";

    private static readonly Dictionary<int, string> ZoneDescriptions = new()
    {
        { 1, "Arctic climates with extremely short growing seasons and severe winters." },
        { 2, "Very cold regions with long winters and limited frost-free periods." },
        { 3, "Cold northern areas with short summers and frequent deep freezes." },
        { 4, "Cool climates with moderate summers and cold winter temperatures." },
        { 5, "Cool-temperate regions with distinct seasons and regular winter freezes." },
        { 6, "Mild-temperate zones with moderate winters and broad plant options." },
        { 7, "Temperate climates with relatively mild winters and long growing seasons." },
        { 8, "Warm-temperate regions with mild winters and hot summers." },
        { 9, "Warm climates with rare hard freezes and long frost-free seasons." },
        { 10, "Subtropical regions with minimal frost and year-round growing potential." },
        { 11, "Tropical-subtropical climates with very warm temperatures year-round." },
        { 12, "Tropical climates that stay hot year-round with no freezing temperatures." },
        { 13, "Hottest tropical climates with consistently high heat year-round." }
    };

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

    [ObservableProperty]
    private string zoneCode = string.Empty;

    [ObservableProperty]
    private string zoneTemperatureRange = string.Empty;

    [ObservableProperty]
    private string zoneLastUpdated = string.Empty;

    [ObservableProperty]
    private string zoneDescription = string.Empty;

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
            ClearZoneDetails();
            ErrorMessage = "Please enter a valid ZIP code.";
            return;
        }

        try
        {
            IsBusy = true;
            var result = await plantZoneService.GetZoneByZip(ZipCode);
            var zone = result?.Zone ?? string.Empty;
            SearchResult = zone;

            if (string.IsNullOrWhiteSpace(zone))
            {
                ClearZoneDetails();
                ErrorMessage = "Could not find a plant zone. Check the ZIP code and try again.";
                return;
            }

            ApplyZoneDetails(result!);
            preferences.Set(CachedZipCodeKey, ZipCode);
            preferences.Set(CachedPlantZoneDataKey, JsonSerializer.Serialize(result));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            SearchResult = string.Empty;
            ClearZoneDetails();
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
            ClearZoneDetails();
            return;
        }

<<<<<<< HEAD
        try
        {
            var cachedZoneData = JsonSerializer.Deserialize<PlantZoneDataItem>(cachedZoneJson);
            if (cachedZoneData is null || string.IsNullOrWhiteSpace(cachedZoneData.Zone))
            {
                SearchResult = string.Empty;
                ClearZoneDetails();
                return;
            }

            SearchResult = cachedZoneData.Zone;
            ApplyZoneDetails(cachedZoneData);
        }
        catch (JsonException)
        {
            SearchResult = string.Empty;
            ClearZoneDetails();
        }
    }

    private void ApplyZoneDetails(PlantZoneDataItem zoneData)
    {
        ZoneCode = zoneData.Zone ?? string.Empty;
        ZoneTemperatureRange = zoneData.TemperatureRange ?? string.Empty;
        ZoneLastUpdated = DateTimeOffset.UtcNow.ToString(LastUpdatedDisplayFormat);
        ZoneDescription = GetZoneDescription(ZoneCode);
=======
        var result = await plantZoneService.GetZoneByZip(ZipCode);
        var zone = result?.Zone ?? string.Empty;
        SearchResult = zone;
        ZoneCode = zone;
        ZoneTemperatureRange = result?.TemperatureRange ?? string.Empty;
        ZoneLastUpdated = string.IsNullOrEmpty(zone) ? string.Empty : DateTimeOffset.UtcNow.ToString(LastUpdatedDisplayFormat);
        ZoneDescription = GetZoneDescription(zone);
>>>>>>> 857dd48 (chore: address validation feedback for zone card)
    }

    private static string GetZoneDescription(string zoneCode)
    {
        if (string.IsNullOrWhiteSpace(zoneCode))
        {
            return string.Empty;
        }

        var zoneDigits = new string(zoneCode.TakeWhile(char.IsDigit).ToArray());

        if (int.TryParse(zoneDigits, out var zoneNumber) && ZoneDescriptions.TryGetValue(zoneNumber, out var description))
        {
            return description;
        }

        return string.Empty;
    }

    private void ClearZoneDetails()
    {
        ZoneCode = string.Empty;
        ZoneTemperatureRange = string.Empty;
        ZoneLastUpdated = string.Empty;
        ZoneDescription = string.Empty;
    }
}
