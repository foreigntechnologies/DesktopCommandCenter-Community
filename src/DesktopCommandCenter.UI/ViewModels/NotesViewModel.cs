using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using DesktopCommandCenter.Domain.Entities;

using CommunityToolkit.Mvvm.Input;
using MediatR;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Features.Notes.Queries;
using DesktopCommandCenter.Application.Features.Notes.Commands;

namespace DesktopCommandCenter.UI.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private ObservableCollection<Note> _notes = new();

    public NotesViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RelayCommand]
    public async Task LoadNotesAsync()
    {
        var result = await _mediator.Send(new GetNotesQuery());
        Notes.Clear();
        foreach (var note in result)
        {
            Notes.Add(note);
        }
    }

    [RelayCommand]
    public async Task AddTestNoteAsync()
    {
        var note = await _mediator.Send(new CreateNoteCommand("Nova Nota", "Conteúdo de teste via MediatR!", "Geral"));
        Notes.Add(note);
    }
}

