using Microsoft.Extensions.Logging;
using ThymeToPlant.Services;
using ThymeToPlant.ViewModels;
using ThymeToPlant.Views;

namespace ThymeToPlant;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // MVVM + DI pattern: register services, then view models, then pages for constructor injection and reusable binding patterns.
        builder.Services.AddSingleton<PlantZoneService>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();

        App = builder.Build();

        return App;
    }

	//Helpers
    public static MauiApp App { get; private set; }
    public static IServiceProvider Services
    => App.Services;
}
