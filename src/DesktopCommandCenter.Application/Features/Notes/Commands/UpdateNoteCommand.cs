using MediatR;
using DesktopCommandCenter.Domain.Entities;

namespace DesktopCommandCenter.Application.Features.Notes.Commands;

public class UpdateNoteCommand : IRequest<Note>
{
    public Note Note { get; }

    public UpdateNoteCommand(Note note)
    {
        Note = note;
    }
}
