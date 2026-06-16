using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace HRApplicantSystem.UI.Converters
{
    public class StringEqualsConverter : IValueConverter
    {
        public static readonly StringEqualsConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                ? parameter?.ToString()
                : Avalonia.Data.BindingOperations.DoNothing;
        }
    }
}