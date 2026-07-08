using System;
using MediatR;

namespace DesktopCommandCenter.Application.Features.Notes.Commands;

public class DeleteNoteCommand : IRequest
{
    public Guid Id { get; }

    public DeleteNoteCommand(Guid id)
    {
        Id = id;
    }
}
