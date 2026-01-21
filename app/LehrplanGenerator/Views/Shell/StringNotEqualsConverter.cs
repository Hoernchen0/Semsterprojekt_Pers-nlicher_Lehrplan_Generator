using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LehrplanGenerator.Views.Shell;

public sealed class StringNotEqualsConverter : IValueConverter
{
    public static readonly StringNotEqualsConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString() != parameter?.ToString();

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
