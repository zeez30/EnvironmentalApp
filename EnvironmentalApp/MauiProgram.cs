
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Xaml;
using EnvironmentalApp.Data; // Make sure to include this
using Microsoft.Extensions.Logging;
using System;


namespace EnvironmentalApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if WINDOWS
        Debug.WriteLine("Windows");
        builder.UseMauiCommunityToolkitMaps("1g5lpw6it9LDTBpHeAQzM9nPcgtOYHr3lEvSuZ4G62HRjBovPneXJQQJ99BCACi5YpzcXOIAAAAgAZMP2wy5");
#endif

            builder.Logging.AddDebug();

#endif
            // Database initialization (must be awaited, therefore we move it to a separate scope)
            // Register DatabaseService as a singleton
            builder.Services.AddSingleton<Data.DatabaseService>();

            // Register MainPage with dependency injection (logger)
            builder.Services.AddSingleton<MainPage>();

            // Register your services here:
            builder.Services.AddSingleton<MapPage>(); //Example
            // You may have to register your viewmodels, add singleton scope

            return builder.Build();
        }
    }
}