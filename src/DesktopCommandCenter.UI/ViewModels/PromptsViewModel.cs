using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class PromptsViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<Prompt> Prompts { get; } = new();

    public Visibility EmptyMessageVisibility => Prompts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public string FormTitle => IsEditing ? "Editar Prompt" : "Novo Prompt";

    [ObservableProperty]
    private string _editTitle = string.Empty;

    [ObservableProperty]
    private string _editCategory = string.Empty;

    [ObservableProperty]
    private string _editContent = string.Empty;

    [ObservableProperty] public partial Prompt? SelectedPrompt { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormTitle))]
    private bool _isEditing;

    public PromptsViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [RelayCommand]
    public async Task LoadPromptsAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPromptRepository>();
            var list = await repository.GetAllAsync();

            Prompts.Clear();
            foreach (var p in list)
            {
                Prompts.Add(p);
            }
            OnPropertyChanged(nameof(EmptyMessageVisibility));
        }
        catch (Exception)
        {
            // Fail-safe to prevent app crash
        }
    }

    [RelayCommand]
    private async Task SavePromptAsync()
    {
        if (string.IsNullOrWhiteSpace(EditTitle) || string.IsNullOrWhiteSpace(EditContent))
            return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPromptRepository>();

            if (IsEditing && SelectedPrompt != null)
            {
                var promptToUpdate = await repository.GetByIdAsync(SelectedPrompt.Id);
                if (promptToUpdate != null)
                {
                    promptToUpdate.Title = EditTitle.Trim();
                    promptToUpdate.Category = string.IsNullOrWhiteSpace(EditCategory) ? "Geral" : EditCategory.Trim();
                    promptToUpdate.Content = EditContent.Trim();

                    await repository.UpdateAsync(promptToUpdate);
                }
            }
            else
            {
                var newPrompt = new Prompt
                {
                    Title = EditTitle.Trim(),
                    Category = string.IsNullOrWhiteSpace(EditCategory) ? "Geral" : EditCategory.Trim(),
                    Content = EditContent.Trim()
                };
                await repository.AddAsync(newPrompt);
            }

            ClearForm();
            await LoadPromptsAsync();
        }
        catch (Exception)
        {
            // Fail-safe to prevent app crash
        }
    }

    [RelayCommand]
    private async Task DeletePromptAsync(Prompt prompt)
    {
        if (prompt == null) return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPromptRepository>();
            await repository.DeleteAsync(prompt.Id);

            if (SelectedPrompt?.Id == prompt.Id)
            {
                ClearForm();
            }

            await LoadPromptsAsync();
        }
        catch (Exception)
        {
            // Fail-safe to prevent app crash
        }
    }

    [RelayCommand]
    private void SelectPromptForEdit(Prompt prompt)
    {
        if (prompt == null) return;
        SelectedPrompt = prompt;
        EditTitle = prompt.Title;
        EditCategory = prompt.Category;
        EditContent = prompt.Content;
        IsEditing = true;
    }

    [RelayCommand]
    private void ClearForm()
    {
        EditTitle = string.Empty;
        EditCategory = string.Empty;
        EditContent = string.Empty;
        SelectedPrompt = null;
        IsEditing = false;
    }

    [RelayCommand]
    private void CopyPrompt(Prompt prompt)
    {
        if (prompt == null) return;
        try
        {
            var package = new DataPackage();
            package.SetText(prompt.Content);
            Clipboard.SetContent(package);
        }
        catch (Exception)
        {
            // Fail-safe
        }
    }
}

