using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IColorPickerService
{
    /// <summary>
    /// Gets the color of the pixel exactly under the mouse cursor in HEX format (e.g., #FF5733)
    /// </summary>
    Task<string> GetColorAtCursorHexAsync();

    /// <summary>
    /// Gets the color of the pixel exactly under the mouse cursor in RGB format (e.g., 255, 87, 51)
    /// </summary>
    Task<string> GetColorAtCursorRgbAsync();
}
