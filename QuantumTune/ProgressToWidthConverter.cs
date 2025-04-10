using System.Globalization;
using System.Windows.Data;

namespace WindowsOptimizer;

public class ProgressToWidthConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 4 ||
            !double.TryParse(values[0].ToString(), out var value) ||
            !double.TryParse(values[1].ToString(), out var minimum) ||
            !double.TryParse(values[2].ToString(), out var maximum) ||
            !double.TryParse(values[3].ToString(), out var width))
            return 0.0;

        if (maximum - minimum == 0)
            return 0.0;

        return (value - minimum) / (maximum - minimum) * width;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}