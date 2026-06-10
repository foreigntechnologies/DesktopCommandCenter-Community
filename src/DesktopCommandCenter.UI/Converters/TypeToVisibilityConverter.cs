using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace DesktopCommandCenter.UI.Converters;

public class TypeToVisibilityConverter : IValueConverter
{
    public string TargetType { get; set; } = "Text"; // Text or Image

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string type)
        {
            return type == TargetType ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
