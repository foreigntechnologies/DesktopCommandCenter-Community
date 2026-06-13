using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Interfaces;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using DesktopCommandCenter.Infrastructure.Services;
using DesktopCommandCenter.UI.Views;
using System.Runtime.InteropServices;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ColorPickerViewModel : ObservableObject
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X; public int Y; }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    private readonly IColorPickerService _colorPickerService;
    private DispatcherQueueTimer? _timer;
    private ColorLoupeWindow? _loupeWindow;
    private GlobalMouseHook? _mouseHook;

    [ObservableProperty]
    private string _currentHex = "#FFFFFF";

    [ObservableProperty]
    private string _currentRgb = "255, 255, 255";

    [ObservableProperty] public partial bool IsPicking { get; set; }

    public ColorPickerViewModel(IColorPickerService colorPickerService)
    {
        _colorPickerService = colorPickerService;
    }

    [RelayCommand]
    public void TogglePicking()
    {
        IsPicking = !IsPicking;
    }

    partial void OnIsPickingChanged(bool value)
    {
        if (value)
            StartPicking();
        else
            StopPicking();
    }

    private void StartPicking()
    {
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (_timer == null)
        {
            _timer = dispatcherQueue.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(30);
            _timer.Tick += async (s, e) => await UpdateColorAsync();
        }

        if (_loupeWindow == null)
        {
            _loupeWindow = new ColorLoupeWindow();
        }
        _loupeWindow.Activate();

        if (_mouseHook == null)
        {
            _mouseHook = new GlobalMouseHook();
            _mouseHook.OnLeftMouseDown += (s, e) =>
            {
                dispatcherQueue?.TryEnqueue(() => CopyHex());
            };
        }
        _mouseHook.Start();

        _timer.Start();
    }

    private void StopPicking()
    {
        _timer?.Stop();
        _mouseHook?.Stop();

        if (_loupeWindow != null)
        {
            _loupeWindow.Close();
            _loupeWindow = null;
        }
    }

    private async Task UpdateColorAsync()
    {
        CurrentHex = await _colorPickerService.GetColorAtCursorHexAsync();
        CurrentRgb = await _colorPickerService.GetColorAtCursorRgbAsync();

        if (_loupeWindow != null)
        {
            GetCursorPos(out POINT pos);
            _loupeWindow.MoveTo(pos.X, pos.Y);
            
            // Parse RGB para atualizar o visual
            var rgbParts = CurrentRgb.Split(',');
            if (rgbParts.Length == 3 && 
                byte.TryParse(rgbParts[0].Trim(), out byte r) && 
                byte.TryParse(rgbParts[1].Trim(), out byte g) && 
                byte.TryParse(rgbParts[2].Trim(), out byte b))
            {
                _loupeWindow.UpdateColor(CurrentHex, r, g, b);
            }
        }
    }

    [RelayCommand]
    public void CopyHex()
    {
        var package = new DataPackage();
        package.SetText(CurrentHex);
        Clipboard.SetContent(package);
        StopPicking();
        IsPicking = false;
    }

    [RelayCommand]
    public void CopyRgb()
    {
        var package = new DataPackage();
        package.SetText(CurrentRgb);
        Clipboard.SetContent(package);
        StopPicking();
        IsPicking = false;
    }
}

