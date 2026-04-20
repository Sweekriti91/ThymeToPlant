using ThymeToPlant.ViewModels;

namespace ThymeToPlant.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel viewModel;

	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        this.viewModel = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.RestoreCachedZoneCommand.Execute(null);
    }
}
