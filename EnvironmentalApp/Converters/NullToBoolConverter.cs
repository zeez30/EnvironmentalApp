using System.Globalization;

namespace EnvironmentalApp.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        // Parameter allows inverting: True means null=true, False means null=false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool parameterBool = false;
            if (parameter != null && bool.TryParse(parameter.ToString(), out bool result))
            {
                parameterBool = result;
            }

            // If parameter is true, return true when value is null.
            // If parameter is false (default), return false when value is null.
            return (value == null) ? parameterBool : !parameterBool;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}