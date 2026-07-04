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

    public async Task CreateNoteAsync(string title, string content, string category)
    {
        var note = await _mediator.Send(new CreateNoteCommand(title, content, category));
        Notes.Insert(0, note); // Insert at top
    }

    public async Task UpdateNoteAsync(Note note, string newTitle, string newContent, string newCategory)
    {
        note.Title = newTitle;
        note.Content = newContent;
        note.Category = newCategory;
        await _mediator.Send(new UpdateNoteCommand(note));
        
        // Trigger UI update
        var index = Notes.IndexOf(note);
        if (index >= 0)
        {
            Notes[index] = note;
        }
    }

    public async Task DeleteNoteAsync(Note note)
    {
        await _mediator.Send(new DeleteNoteCommand(note.Id));
        Notes.Remove(note);
    }
}

