using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PaintShopIMS.Converters;

public class StringLengthToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return !string.IsNullOrWhiteSpace(str) ? Visibility.Visible : Visibility.Collapsed;
        }
        else if (value is int length)
        {
            return length > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
