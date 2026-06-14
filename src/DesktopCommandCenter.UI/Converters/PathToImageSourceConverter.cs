using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace DesktopCommandCenter.UI.Converters;

public class PathToImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string path && !string.IsNullOrWhiteSpace(path))
        {
            try
            {
                // Uri handles absolute paths. In Windows App SDK / WinUI 3, 
                // BitmapImage can be created using a local file URI (file:///)
                return new BitmapImage(new Uri(path));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting path to ImageSource: {ex.Message}");
                return null;
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
