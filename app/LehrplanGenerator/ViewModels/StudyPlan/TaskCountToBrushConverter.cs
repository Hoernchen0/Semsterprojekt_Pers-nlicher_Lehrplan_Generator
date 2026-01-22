using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LehrplanGenerator.ViewModels.StudyPlan
{

    public class TaskCountToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DayPlanViewModel day)
            {
                // Tage außerhalb des Monats: hellgrau
                if (!day.IsCurrentMonth)
                    return Brushes.LightGray;

                // Tage mit Tasks: grün
                return day.Tasks.Count > 0 ? Brushes.Green : Brushes.White;
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Avalonia.Data.BindingOperations.DoNothing;
        }
    }
}