using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using System.Diagnostics;

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
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
