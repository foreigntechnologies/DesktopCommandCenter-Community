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
        }
        catch
        {
            // Ignora acessos negados, pois outro app pode estar segurando a lock do clipboard.
        }
    }
}
