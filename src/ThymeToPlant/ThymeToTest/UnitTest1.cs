using ThymeToPlant.Services;
using ThymeToPlant.Models;
using ThymeToPlant.ViewModels;
using ThymeToPlant.Data;

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
        var fakeService = new FakePlantZoneService(new PlantZoneDataItem { Zone = "6B" });
        var vm = new MainPageViewModel(fakeService)
        {
            ZipCode = "97214"
        };

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo("6B"));
        Assert.That(fakeService.LastZip, Is.EqualTo("97214"));
        Assert.That(fakeService.Calls, Is.EqualTo(1));
        Assert.That(vm.ErrorMessage, Is.EqualTo(string.Empty));
        Assert.That(vm.HasError, Is.False);
    }

    [Test]
    public async Task FindPlantZoneCommand_HandlesEmptyZipAndNullServiceResult()
    {
        var fakeService = new FakePlantZoneService((PlantZoneDataItem?)null);
        var vm = new MainPageViewModel(fakeService);

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(vm.ErrorMessage, Is.EqualTo("Please enter a valid ZIP code."));
        Assert.That(vm.HasError, Is.True);
        Assert.That(fakeService.Calls, Is.EqualTo(0));

        vm.ZipCode = "00000";
        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(vm.ErrorMessage, Is.EqualTo("Could not find a plant zone. Check the ZIP code and try again."));
        Assert.That(vm.HasError, Is.True);
        Assert.That(fakeService.Calls, Is.EqualTo(1));
        Assert.That(fakeService.LastZip, Is.EqualTo("00000"));
    }

    [Test]
    public async Task FindPlantZoneCommand_SetsError_WhenServiceThrows()
    {
        var fakeService = new FakePlantZoneService(_ => throw new HttpRequestException("network down"));
        var vm = new MainPageViewModel(fakeService)
        {
            ZipCode = "97214"
        };

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(vm.ErrorMessage, Is.EqualTo("Unable to look up plant zone right now. Please try again."));
        Assert.That(vm.HasError, Is.True);
        Assert.That(vm.IsBusy, Is.False);
    }

    [Test]
    public async Task FindPlantZoneCommand_IsBusyWhileLookupInProgress()
    {
        var lookupGate = new TaskCompletionSource<PlantZoneDataItem?>();
        var fakeService = new FakePlantZoneService(_ => lookupGate.Task);
        var vm = new MainPageViewModel(fakeService)
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
    public void GetDbPath_UsesProvidedBaseDirectory()
    {
        var basePath = "/tmp/testdb";
        var dbPath = AppDbContext.GetDbPath(basePath);

        Assert.That(dbPath, Is.EqualTo(Path.Combine(basePath, "thymetoplant.db")));
    }
}

internal sealed class FakePlantZoneService : PlantZoneService
{
    private readonly PlantZoneDataItem? response;
    private readonly Func<string, Task<PlantZoneDataItem?>>? responseFactory;

    public FakePlantZoneService(PlantZoneDataItem? response)
    {
        this.response = response;
    }

    public FakePlantZoneService(Func<string, Task<PlantZoneDataItem?>> responseFactory)
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
            return responseFactory(zipCode)!;
        }

        return Task.FromResult(response!);
    }
}
