using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LehrplanGenerator.Logic.Utils.Converter;

public class BoolToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? 1.0 : 0.5;

        return 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}
