using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Features.Clipboard.Queries;
using DesktopCommandCenter.Application.Features.Clipboard.Commands;
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

    private readonly Microsoft.UI.Dispatching.DispatcherQueue? _dispatcherQueue;

    public ClipboardViewModel(IMediator mediator, IClipboardService clipboardService)
    {
        _mediator = mediator;
        _clipboardService = clipboardService;
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
    }

    public void Activate()
    {
        _clipboardService.TextCopied += OnTextCopied;
    }

    public void Deactivate()
    {
        _clipboardService.TextCopied -= OnTextCopied;
    }

    private void OnTextCopied(object? sender, string text)
    {
        // Precisamos marshalar de volta para a UI thread. 
        if (_dispatcherQueue != null)
        {
            _dispatcherQueue.TryEnqueue(async () =>
            {
                await LoadItemsAsync();
            });
        }
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
    [RelayCommand]
    public async Task ClearHistoryAsync()
    {
        try
        {
            await _mediator.Send(new ClearClipboardCommand());
            Items.Clear();

            // Limpar imagens da pasta local também
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            if (await localFolder.TryGetItemAsync("ClipboardImages") is Windows.Storage.StorageFolder imgFolder)
            {
                await imgFolder.DeleteAsync();
            }
        }
        catch (Exception)
        {
            // Tratamento de erro
        }
    }

    [RelayCommand]
    public async Task DeleteItemAsync(ClipboardItem item)
    {
        try
        {
            if (item != null)
            {
                await _mediator.Send(new DeleteClipboardItemCommand(item.Id));
                Items.Remove(item);
            }
        }
        catch { }
    }
}
