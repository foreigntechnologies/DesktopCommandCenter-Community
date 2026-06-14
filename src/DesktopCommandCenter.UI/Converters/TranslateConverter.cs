using Microsoft.UI.Xaml.Data;
using System;
using DesktopCommandCenter.UI.Helpers;

namespace DesktopCommandCenter.UI.Converters;

public class TranslateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string key)
        {
            return LocalizationHelper.Instance[key];
        }
        
        if (value is string valKey)
        {
            return LocalizationHelper.Instance[valKey];
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
