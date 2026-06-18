using ThymeToPlant.ViewModels;

namespace ThymeToPlant.Views;

public partial class AddSeedPage : ContentPage
{
    private readonly AddSeedViewModel viewModel;

    public AddSeedPage(AddSeedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.ResetForm();
    }
}
