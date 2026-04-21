using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using ThymeToPlant.Data;
using ThymeToPlant.Repositories;
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
        builder.Services.AddSingleton<IPreferences>(_ => Preferences.Default);
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={AppDbContext.DbPath}"));
        builder.Services.AddScoped<ISeedRepository, SeedRepository>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();

        App = builder.Build();
        InitializeDatabase(App.Services);

        return App;
    }

    private static void InitializeDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }

//Helpers
    public static MauiApp App { get; private set; }
    public static IServiceProvider Services
    => App.Services;
}
