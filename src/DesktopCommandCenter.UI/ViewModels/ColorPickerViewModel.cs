using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Interfaces;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ColorPickerViewModel : ObservableObject
{
    private readonly IColorPickerService _colorPickerService;
    private DispatcherQueueTimer? _timer;

    [ObservableProperty]
    private string _currentHex = "#FFFFFF";

    [ObservableProperty]
    private string _currentRgb = "255, 255, 255";

    [ObservableProperty]
    private bool _isPicking;

    public ColorPickerViewModel(IColorPickerService colorPickerService)
    {
        _colorPickerService = colorPickerService;
    }

    [RelayCommand]
    public void TogglePicking()
    {
        IsPicking = !IsPicking;

        if (IsPicking)
        {
            StartPicking();
        }
        else
        {
            StopPicking();
        }
    }

    private void StartPicking()
    {
        if (_timer == null)
        {
            _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += async (s, e) => await UpdateColorAsync();
        }
        _timer.Start();
    }

    private void StopPicking()
    {
        _timer?.Stop();
    }

    private async Task UpdateColorAsync()
    {
        CurrentHex = await _colorPickerService.GetColorAtCursorHexAsync();
        CurrentRgb = await _colorPickerService.GetColorAtCursorRgbAsync();
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
