using Microsoft.Extensions.Logging;
using ThymeToPlant.Services;

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

        // Services Registration
        builder.Services.AddSingleton<PlantZoneService>();

        App = builder.Build();

        return App;
    }

	//Helpers
    public static MauiApp App { get; private set; }
    public static IServiceProvider Services
    => App.Services;
}

