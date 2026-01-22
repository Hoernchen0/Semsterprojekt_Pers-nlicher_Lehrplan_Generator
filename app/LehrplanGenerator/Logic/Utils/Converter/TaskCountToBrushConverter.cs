using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LehrplanGenerator.ViewModels.StudyPlan;

namespace LehrplanGenerator.Logic.Utils.Converter;

public class TaskCountToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DayPlanViewModel day)
        {
            if (!day.IsCurrentMonth)
                return Brushes.LightGray;
            return day.Tasks.Count > 0 ? Brushes.Green : Brushes.White;
        }

        return Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}
