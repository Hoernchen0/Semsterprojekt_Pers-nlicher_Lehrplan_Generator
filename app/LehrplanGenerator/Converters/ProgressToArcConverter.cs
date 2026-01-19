using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace LehrplanGenerator.Converters;

public class ProgressToArcConverter : IValueConverter
{
    // value = Progress zwischen 0.0 und 1.0
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double progress || progress <= 0)
            return Geometry.Parse(string.Empty);

        progress = Math.Clamp(progress, 0, 1);

        const double size = 220;
        const double radius = 100;
        const double center = size / 2;

        double angle = progress * 360;
        double radians = (Math.PI / 180) * (angle - 90);

        double x = center + radius * Math.Cos(radians);
        double y = center + radius * Math.Sin(radians);

        bool isLargeArc = angle > 180;

        var path =
            $"M {center},{center - radius} " +
            $"A {radius},{radius} 0 {(isLargeArc ? 1 : 0)} 1 {x},{y}";

        return Geometry.Parse(path);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => null;
}
