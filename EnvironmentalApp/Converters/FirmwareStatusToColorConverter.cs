using System.Globalization;
using Microsoft.Maui.Graphics;

namespace EnvironmentalApp.Converters
{
    public class FirmwareStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status.ToLowerInvariant())
                {
                    case "updating...": return Colors.Orange;
                    case "update available": return Colors.DodgerBlue;
                    case "up to date": return Colors.Green;
                    case "update failed": return Colors.Red;
                    case "no update needed": return Colors.Gray;
                    default: return Colors.Black; // Idle or unknown
                }
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}