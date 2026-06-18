using ThymeToPlant.ViewModels;

namespace ThymeToPlant.Views;

public partial class SeedLibraryPage : ContentPage
{
    private readonly SeedLibraryViewModel viewModel;

    public SeedLibraryPage(SeedLibraryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadAsync();
    }
}
