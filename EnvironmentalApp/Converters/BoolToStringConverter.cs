using System.Globalization;

namespace EnvironmentalApp.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueString { get; set; } = "True";
        public string FalseString { get; set; } = "False";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueString : FalseString;
            }
            return string.Empty; // Or return FalseString as default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Equals(TrueString, StringComparison.OrdinalIgnoreCase);
            }
            return false; // Default
        }
    }
}