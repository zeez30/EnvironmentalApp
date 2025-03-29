using Microsoft.Extensions.Logging;
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