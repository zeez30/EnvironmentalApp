using System.Globalization;

namespace EnvironmentalApp.Converters
{
    public enum ComparisonOperation
    {
        Equals,
        NotEquals,
        // Add LessThan, GreaterThan etc. if needed
    }

    public class ComparisonConverter : IValueConverter
    {
        public ComparisonOperation Operation { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Compares the bound value ('value') against the ConverterParameter ('parameter')
            switch (Operation)
            {
                case ComparisonOperation.Equals:
                    return object.Equals(value, parameter);
                case ComparisonOperation.NotEquals:
                default: // Default to NotEquals for enabling update button
                    return !object.Equals(value, parameter);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}