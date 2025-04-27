// --- START OF FILE MauiProgram.cs ---

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using System.Diagnostics;
using EnvironmentalApp.Data;
using System.Text;

namespace EnvironmentalApp
{
    /// <summary>
    /// Configures and creates the MauiApp instance.
    /// Sets up services, fonts, community toolkits, and platform-specific initializations.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the Maui application.
        /// </summary>
        /// <returns>The configured <see cref="MauiApp"/> instance.</returns>
        public static MauiApp CreateMauiApp()
        {
            // Required for ExcelDataReader potentially on some platforms
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Basic toolkit
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Platform-specific configuration for Maps using Bing Maps key
#if WINDOWS
                Debug.WriteLine("Windows platform detected, configuring Maps.");
                // IMPORTANT: Replace "YOUR_BING_MAPS_KEY" with your actual key from https://www.bingmapsportal.com/
                builder.UseMauiCommunityToolkitMaps("1g5lpw6it9LDTBpHeAQzM9nPcgtOYHr3lEvSuZ4G62HRjBovPneXJQQJ99BCACi5YpzcXOIAAAAgAZMP2wy5");
#endif

            // Enable debug logging in DEBUG builds
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register Services for Dependency Injection
            builder.Services.AddSingleton<FirebaseAuthService>(); // Authentication service (mock in this case)
            // Register pages that might be navigated to or used with DI
            builder.Services.AddTransient<MainPage>(); // Main page after login
            // Other pages like LoginPage, MapPage etc are typically instantiated directly via 'new' in this setup.
            // If using more complex navigation or MVVM, register ViewModels and other services here.

            return builder.Build();
        }
    }
}
// --- END OF FILE MauiProgram.cs ---