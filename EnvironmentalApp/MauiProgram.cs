using Microsoft.Extensions.Logging;
using System;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EnvironmentalApp.Data;
using EnvironmentalApp.Services;
using EnvironmentalApp.ViewModels;

namespace EnvironmentalApp
{
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

#if WINDOWS
        Debug.WriteLine("Windows");
        builder.UseMauiCommunityToolkitMaps("1g5lpw6it9LDTBpHeAQzM9nPcgtOYHr3lEvSuZ4G62HRjBovPneXJQQJ99BCACi5YpzcXOIAAAAgAZMP2wy5");
#endif
            builder.Logging.AddDebug();
            // Database initialization (must be awaited, therefore we move it to a separate scope)
            // Register DatabaseService as a singleton
            builder.Services.AddSingleton<Data.DatabaseService>();
=======

#if DEBUG
            builder.Logging.AddDebug();
#endif


            // Register the DbContext for Dependency Injection
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=environmentaldata.db"); // Use a descriptive name!
            });

            // Register the DatabaseService
            builder.Services.AddSingleton<DatabaseService>();

            // Register ViewModels (Transient or Singleton based on usage)
            builder.Services.AddSingleton<MainPageViewModel>();

            // Register the ILogger for MainPage (and other pages if needed)
            builder.Services.AddSingleton<ILogger<MainPage>>();

            return builder.Build();
        }
    }
}