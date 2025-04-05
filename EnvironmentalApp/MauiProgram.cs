using Microsoft.Extensions.Logging;
using System;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EnvironmentalApp.Data;
using EnvironmentalApp.Services;
using EnvironmentalApp.ViewModels;
using System.IO; // Add this for Path.Combine

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
                .UseMauiCommunityToolkitMaps("YOUR_MAPS_API_KEY")
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            Debug.WriteLine("Windows");
#endif
            builder.Logging.AddDebug();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register the DbContext for Dependency Injection
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "environmentaldata.db");
                Console.WriteLine($"Database Path: {dbPath}"); // To help find the database
                options.UseSqlite($"Data Source={dbPath}");
            });

            // Register DatabaseService as a singleton (after AppDbContext)
            builder.Services.AddSingleton<DatabaseService>();

            // Register ViewModels (Transient or Singleton based on usage)
            builder.Services.AddSingleton<MainPageViewModel>();

            // Register the ILogger for MainPage (and other pages if needed)
            builder.Services.AddSingleton<ILogger<MainPage>>();

            return builder.Build();
        }
    }
}