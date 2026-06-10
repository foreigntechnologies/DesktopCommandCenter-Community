using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Notes.Commands;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Note>
{
    private readonly INoteRepository _repository;

    public CreateNoteCommandHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Note> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            Category = request.Category
        };

        return await _repository.AddAsync(note);
    }
}
