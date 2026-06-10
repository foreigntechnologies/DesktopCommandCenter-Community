using DesktopCommandCenter.Application.Interfaces;
using System;
using Windows.ApplicationModel.DataTransfer;

using Microsoft.Extensions.DependencyInjection;
using MediatR;
using DesktopCommandCenter.Application.Features.Clipboard.Commands;

namespace DesktopCommandCenter.Infrastructure.Services;

public class WindowsClipboardService : IClipboardService
{
    private readonly IServiceProvider _serviceProvider;
    public event EventHandler<string>? TextCopied;

    public WindowsClipboardService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void StartMonitoring()
    {
        Clipboard.ContentChanged += OnClipboardContentChanged;
    }

    public void StopMonitoring()
    {
        Clipboard.ContentChanged -= OnClipboardContentChanged;
    }

    private async void OnClipboardContentChanged(object? sender, object e)
    {
        try
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();
                TextCopied?.Invoke(this, text);
                
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new CreateClipboardItemCommand(text));
            }
            else if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                var bitmapRef = await dataPackageView.GetBitmapAsync();
                using var stream = await bitmapRef.OpenReadAsync();
                
                // Save bitmap into a .png file
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var clipboardImagesFolder = await localFolder.CreateFolderAsync("ClipboardImages", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var fileName = $"img_{Guid.NewGuid()}.png";
                var file = await clipboardImagesFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                
                using (var destStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    await Windows.Storage.Streams.RandomAccessStream.CopyAndCloseAsync(
                        stream.GetInputStreamAt(0), 
                        destStream.GetOutputStreamAt(0));
                }

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                TextCopied?.Invoke(this, "Imagem");
                await mediator.Send(new CreateClipboardItemCommand("Imagem", "Image", file.Path));
            }
        }
        catch
        {
            // Ignore access denied errors since another app could be locking the clipboard.
        }
    }
}
