using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public class ColorPickerService : IColorPickerService
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("gdi32.dll")]
    static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    public Task<string> GetColorAtCursorHexAsync()
    {
        var color = GetColorAtCursor();
        string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        return Task.FromResult(hex);
    }

    public Task<string> GetColorAtCursorRgbAsync()
    {
        var color = GetColorAtCursor();
        string rgb = $"{color.R}, {color.G}, {color.B}";
        return Task.FromResult(rgb);
    }

    private Color GetColorAtCursor()
    {
        IntPtr hdc = GetDC(IntPtr.Zero);
        GetCursorPos(out POINT pos);
        uint pixel = GetPixel(hdc, pos.X, pos.Y);
        ReleaseDC(IntPtr.Zero, hdc);

        Color color = Color.FromArgb(
            (int)(pixel & 0x000000FF),
            (int)(pixel & 0x0000FF00) >> 8,
            (int)(pixel & 0x00FF0000) >> 16);

        return color;
    }
}
