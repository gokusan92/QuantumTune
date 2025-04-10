using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WindowsOptimizer.ViewModels;

namespace WindowsOptimizer;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Collapsed;
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool needsOptimization)
            return needsOptimization
                ? new SolidColorBrush(Color.FromRgb(255, 165, 0)) // Orange
                : new SolidColorBrush(Color.FromRgb(0, 204, 102)); // Green
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class OptimizationStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool needsOptimization && needsOptimization
            ? "Your system needs optimization"
            : "Your system is optimized";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class OptimizationButtonTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool needsOptimization && needsOptimization
            ? "CLEAN"
            : "SCAN";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class OptimizationButtonSubtextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool needsOptimization && needsOptimization
            ? "fix all issues"
            : "your system now";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class OptimizationCommandConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is SystemStatusViewModel vm)
            return value is bool needsOpt && needsOpt
                ? vm.OptimizeCommand
                : vm.ScanCommand;

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ImpactToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string impact)
            return impact switch
            {
                "High Impact" => new SolidColorBrush(Color.FromRgb(255, 82, 82)), // Red
                "Medium Impact" => new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange
                "Essential" => new SolidColorBrush(Color.FromRgb(0, 204, 102)), // Green
                _ => new SolidColorBrush(Color.FromRgb(155, 89, 182)) // Purple
            };
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NavigationColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Determine if this is the current page
        if (value != null && parameter is string targetTypeName)
        {
            var currentTypeName = value.GetType().Name;
            if (currentTypeName == targetTypeName)
                return new SolidColorBrush(Color.FromRgb(155, 89, 182)); // Purple for selected
        }

        return new SolidColorBrush(Color.FromRgb(187, 187, 187)); // Light gray for non-selected
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}