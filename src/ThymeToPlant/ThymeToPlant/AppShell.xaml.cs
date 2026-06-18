namespace ThymeToPlant;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Views.AddSeedPage), typeof(Views.AddSeedPage));
    }
}

