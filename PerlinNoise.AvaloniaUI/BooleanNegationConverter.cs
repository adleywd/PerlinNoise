using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace PerlinNoise.AvaloniaUI;

public class BooleanNegationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return !booleanValue;
        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}