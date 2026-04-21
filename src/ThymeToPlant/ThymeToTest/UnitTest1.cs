using ThymeToPlant.Services;
using ThymeToPlant.Models;
using ThymeToPlant.ViewModels;
using ThymeToPlant.Data;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace ThymeToTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public void DummyTest()
    {
        var mathy = new FakeTestService();
        var result = mathy.AddMe(1, 2);

        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public async Task FindPlantZoneCommand_SetsSearchResult_WhenServiceReturnsZone()
    {
        var fakeService = new FakePlantZoneService(new PlantZoneDataItem { Zone = "6B", TemperatureRange = "-10°F to 0°F" });
        var fakePreferences = new FakePreferences();
        var vm = new MainPageViewModel(fakeService, fakePreferences, new FakeLocationZipService(LocationZipResult.Success("99999")))
        {
            ZipCode = "97214"
        };

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo("6B"));
        Assert.That(vm.ZoneCode, Is.EqualTo("6B"));
        Assert.That(vm.ZoneTemperatureRange, Is.EqualTo("-10°F to 0°F"));
        Assert.That(vm.ZoneDescription, Is.EqualTo("Mild-temperate zones with moderate winters and broad plant options."));
        Assert.That(vm.ZoneSearchedAt, Is.Not.Empty);
        Assert.That(fakeService.LastZip, Is.EqualTo("97214"));
        Assert.That(fakeService.Calls, Is.EqualTo(1));
        Assert.That(vm.ErrorMessage, Is.EqualTo(string.Empty));
        Assert.That(vm.HasError, Is.False);
        Assert.That(fakePreferences.Get("home.cachedZipCode", string.Empty), Is.EqualTo("97214"));
        var cachedZone = JsonSerializer.Deserialize<PlantZoneDataItem>(fakePreferences.Get("home.cachedPlantZoneData", string.Empty));
        Assert.That(cachedZone?.Zone, Is.EqualTo("6B"));
    }

    [Test]
    public async Task FindPlantZoneCommand_HandlesEmptyZipAndNullServiceResult()
    {
        var fakeService = new FakePlantZoneService((PlantZoneDataItem?)null);
        var fakePreferences = new FakePreferences();
        var vm = new MainPageViewModel(fakeService, fakePreferences, new FakeLocationZipService(LocationZipResult.Success("99999")));

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(vm.ErrorMessage, Is.EqualTo("Please enter a valid ZIP code."));
        Assert.That(vm.HasError, Is.True);
        Assert.That(vm.ZoneCode, Is.EqualTo(string.Empty));
        Assert.That(vm.ZoneTemperatureRange, Is.EqualTo(string.Empty));
        Assert.That(vm.ZoneSearchedAt, Is.EqualTo(string.Empty));
        Assert.That(vm.ZoneDescription, Is.EqualTo(string.Empty));
        Assert.That(fakeService.Calls, Is.EqualTo(0));

        vm.ZipCode = "00000";
        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(vm.ErrorMessage, Is.EqualTo("Could not find a plant zone. Check the ZIP code and try again."));
        Assert.That(vm.HasError, Is.True);
        Assert.That(vm.ZoneCode, Is.EqualTo(string.Empty));
        Assert.That(vm.ZoneTemperatureRange, Is.EqualTo(string.Empty));
        Assert.That(vm.ZoneSearchedAt, Is.EqualTo(string.Empty));
        Assert.That(vm.ZoneDescription, Is.EqualTo(string.Empty));
        Assert.That(fakeService.Calls, Is.EqualTo(1));
        Assert.That(fakeService.LastZip, Is.EqualTo("00000"));
        Assert.That(fakePreferences.ContainsKey("home.cachedZipCode"), Is.False);
        Assert.That(fakePreferences.ContainsKey("home.cachedPlantZoneData"), Is.False);
    }

    [Test]
    public void RestoreCachedZoneCommand_LoadsCachedZipAndZone()
    {
        var fakeService = new FakePlantZoneService((PlantZoneDataItem?)null);
        var fakePreferences = new FakePreferences();
        fakePreferences.Set("home.cachedZipCode", "97035");
        fakePreferences.Set("home.cachedPlantZoneData", JsonSerializer.Serialize(new PlantZoneDataItem { Zone = "8B", TemperatureRange = "15°F to 20°F" }));

        var vm = new MainPageViewModel(fakeService, fakePreferences, new FakeLocationZipService(LocationZipResult.Success("99999")));

        vm.RestoreCachedZoneCommand.Execute(null);

        Assert.That(vm.ZipCode, Is.EqualTo("97035"));
        Assert.That(vm.SearchResult, Is.EqualTo("8B"));
        Assert.That(vm.ZoneCode, Is.EqualTo("8B"));
        Assert.That(vm.ZoneTemperatureRange, Is.EqualTo("15°F to 20°F"));
        Assert.That(vm.ZoneDescription, Is.EqualTo("Warm-temperate regions with mild winters and hot summers."));
        Assert.That(fakeService.Calls, Is.EqualTo(0));
    }

    [Test]
    public async Task FindPlantZoneCommand_SetsError_WhenServiceThrows()
    {
        var fakeService = new FakePlantZoneService(_ => throw new HttpRequestException("network down"));
        var fakePreferences = new FakePreferences();
        var vm = new MainPageViewModel(fakeService, fakePreferences, new FakeLocationZipService(LocationZipResult.Success("99999")))
        {
            ZipCode = "97214"
        };

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(vm.ErrorMessage, Is.EqualTo("Unable to look up plant zone right now. Please try again."));
        Assert.That(vm.HasError, Is.True);
        Assert.That(vm.IsBusy, Is.False);
        Assert.That(vm.ZoneCode, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task FindPlantZoneCommand_IsBusyWhileLookupInProgress()
    {
        var lookupGate = new TaskCompletionSource<PlantZoneDataItem>();
        var fakeService = new FakePlantZoneService(_ => lookupGate.Task);
        var fakePreferences = new FakePreferences();
        var vm = new MainPageViewModel(fakeService, fakePreferences, new FakeLocationZipService(LocationZipResult.Success("99999")))
        {
            ZipCode = "97214"
        };

        var execution = vm.FindPlantZoneCommand.ExecuteAsync(null);

        await Task.Delay(50);
        Assert.That(vm.IsBusy, Is.True);
        Assert.That(vm.FindPlantZoneCommand.CanExecute(null), Is.False);

        lookupGate.SetResult(new PlantZoneDataItem { Zone = "6B" });
        await execution;

        Assert.That(vm.IsBusy, Is.False);
        Assert.That(vm.FindPlantZoneCommand.CanExecute(null), Is.True);
    }

    [Test]
    public async Task FindPlantZoneCommand_UsesZoneLookup_ForTwoDigitZones()
    {
        var fakeService = new FakePlantZoneService(new PlantZoneDataItem { Zone = "13A" });
        var fakePreferences = new FakePreferences();
        var vm = new MainPageViewModel(fakeService, fakePreferences, new FakeLocationZipService(LocationZipResult.Success("99999")))
        {
            ZipCode = "33101"
        };

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.ZoneDescription, Is.EqualTo("Hottest tropical climates with consistently high heat year-round."));
    }

    [Test]
    public void GetDbPath_UsesProvidedBaseDirectory()
    {
        var basePath = "/tmp/testdb";
        var dbPath = AppDbContext.GetDbPath(basePath);

        Assert.That(dbPath, Is.EqualTo(Path.Combine(basePath, "thymetoplant.db")));
    }

    [Test]
    public async Task UseMyLocationCommand_SetsZipAndSearches_WhenLocationLookupSucceeds()
    {
        var fakeService = new FakePlantZoneService(new PlantZoneDataItem { Zone = "7A" });
        var fakeLocationZipService = new FakeLocationZipService(LocationZipResult.Success("97214"));
        var vm = new MainPageViewModel(fakeService, new FakePreferences(), fakeLocationZipService);

        await vm.UseMyLocationCommand.ExecuteAsync(null);

        Assert.That(vm.ZipCode, Is.EqualTo("97214"));
        Assert.That(vm.SearchResult, Is.EqualTo("7A"));
        Assert.That(vm.LocationStatusMessage, Is.EqualTo(string.Empty));
        Assert.That(fakeService.Calls, Is.EqualTo(1));
        Assert.That(fakeService.LastZip, Is.EqualTo("97214"));
    }

    [Test]
    public async Task UseMyLocationCommand_SetsError_WhenPermissionDenied()
    {
        var fakeService = new FakePlantZoneService(new PlantZoneDataItem { Zone = "7A" });
        var fakeLocationZipService = new FakeLocationZipService(LocationZipResult.Failure("Location permission denied. Please enter a ZIP code."));
        var vm = new MainPageViewModel(fakeService, new FakePreferences(), fakeLocationZipService);

        await vm.UseMyLocationCommand.ExecuteAsync(null);

        Assert.That(vm.LocationStatusMessage, Is.EqualTo("Location permission denied. Please enter a ZIP code."));
        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(fakeService.Calls, Is.EqualTo(0));
    }
}

internal sealed class FakePreferences : IPreferences
{
    private readonly Dictionary<string, object> store = new();

    public bool ContainsKey(string key, string? sharedName = null) => store.ContainsKey(key);

    public void Remove(string key, string? sharedName = null) => store.Remove(key);

    public void Clear(string? sharedName = null) => store.Clear();

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        store[key] = value!;
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        if (store.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return defaultValue;
    }
}

internal sealed class FakePlantZoneService : PlantZoneService
{
    private readonly PlantZoneDataItem? response;
    private readonly Func<string, Task<PlantZoneDataItem>>? responseFactory;

    public FakePlantZoneService(PlantZoneDataItem? response)
    {
        this.response = response;
    }

    public FakePlantZoneService(Func<string, Task<PlantZoneDataItem>> responseFactory)
    {
        this.responseFactory = responseFactory;
    }

    public int Calls { get; private set; }
    public string LastZip { get; private set; } = string.Empty;

    public override Task<PlantZoneDataItem> GetZoneByZip(string zipCode)
    {
        Calls++;
        LastZip = zipCode;
        if (responseFactory is not null)
        {
            return responseFactory(zipCode);
        }

        return Task.FromResult(response!);
    }
}

internal sealed class FakeLocationZipService : LocationZipService
{
    private readonly LocationZipResult result;

    public FakeLocationZipService(LocationZipResult result)
    {
        this.result = result;
    }

    public override Task<LocationZipResult> GetCurrentZipAsync()
    {
        return Task.FromResult(result);
    }
}
