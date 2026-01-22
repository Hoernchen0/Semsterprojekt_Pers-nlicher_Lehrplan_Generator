using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LehrplanGenerator.Logic.Utils.Converter;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i)
            return i > 0;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}
