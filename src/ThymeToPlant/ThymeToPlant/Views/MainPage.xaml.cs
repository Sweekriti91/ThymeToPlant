using ThymeToPlant.Services;

namespace ThymeToPlant.Views;

public partial class MainPage : ContentPage
{
	private readonly PlantZoneService plantZoneService;

	public MainPage()
	{
		InitializeComponent();
		this.plantZoneService = MauiProgram.Services.GetService<PlantZoneService>();
	}

    async void FindPlantZoneButton_Clicked(System.Object sender, System.EventArgs e)
    {
		//TODO Add Null Check
		var zipValue = ZipCodeEntry.Text;
		var result = await plantZoneService.GetZoneByZip(zipValue);
		//TODO Move to ViewModel, update via Binding or on MainThread
		SearchResult.Text = result.Zone;
    }
}


