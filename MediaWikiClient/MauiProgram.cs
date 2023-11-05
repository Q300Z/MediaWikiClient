using MediaWikiClient.Factories;
using MediaWikiClient.Services;
using Microsoft.Extensions.Logging;

namespace MediaWikiClient;

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
        builder.Services.AddSingleton<IDataService, DataService>();
        builder.Services.AddSingleton<MediaWikiApi>();

        builder.Services.AddSingleton<SearchPage>();
        builder.Services.AddTransient<DetailsArticlePage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}