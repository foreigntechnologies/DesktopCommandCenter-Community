using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Features.Clipboard.Queries;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class ClipboardViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly IClipboardService _clipboardService;

    public ObservableCollection<ClipboardItem> Items { get; } = new();

    [ObservableProperty]
    private bool _isLoading;

    public ClipboardViewModel(IMediator mediator, IClipboardService clipboardService)
    {
        _mediator = mediator;
        _clipboardService = clipboardService;
        
        // Opcional: recarregar a lista quando um novo texto for copiado
        _clipboardService.TextCopied += OnTextCopied;
    }

    private void OnTextCopied(object? sender, string text)
    {
        // Precisamos marshalar de volta para a UI thread. 
        // No momento vamos apenas chamar o LoadAsync via DispatcherQueue.
        Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()?.TryEnqueue(async () =>
        {
            await LoadItemsAsync();
        });
    }

    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        try
        {
            IsLoading = true;
            Items.Clear();
            var items = await _mediator.Send(new GetClipboardItemsQuery());
            
            // Reverter a ordem para mostrar os mais recentes primeiro
            foreach (var item in items.Reverse())
            {
                Items.Add(item);
            }
        }
        catch (Exception)
        {
            // Tratamento de erro visual
        }
        finally
        {
            IsLoading = false;
        }
    }
}
