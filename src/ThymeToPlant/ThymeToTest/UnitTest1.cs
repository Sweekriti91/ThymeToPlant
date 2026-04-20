using ThymeToPlant.Services;
using ThymeToPlant.Models;
using ThymeToPlant.ViewModels;

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
    }

    [Test]
    public async Task FindPlantZoneCommand_HandlesEmptyZipAndNullServiceResult()
    {
        var fakeService = new FakePlantZoneService(null);
        var vm = new MainPageViewModel(fakeService);

        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(fakeService.Calls, Is.EqualTo(0));

        vm.ZipCode = "00000";
        await vm.FindPlantZoneCommand.ExecuteAsync(null);

        Assert.That(vm.SearchResult, Is.EqualTo(string.Empty));
        Assert.That(fakeService.Calls, Is.EqualTo(1));
        Assert.That(fakeService.LastZip, Is.EqualTo("00000"));
    }

}

internal sealed class FakePlantZoneService : PlantZoneService
{
    private readonly PlantZoneDataItem? response;

    public FakePlantZoneService(PlantZoneDataItem? response)
    {
        this.response = response;
    }

    public int Calls { get; private set; }
    public string LastZip { get; private set; } = string.Empty;

    public override Task<PlantZoneDataItem> GetZoneByZip(string zipCode)
    {
        Calls++;
        LastZip = zipCode;
        return Task.FromResult(response)!;
    }
}
