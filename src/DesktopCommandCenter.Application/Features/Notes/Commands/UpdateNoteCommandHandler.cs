using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Notes.Commands;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Note>
{
    private readonly INoteRepository _repository;

    public UpdateNoteCommandHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Note> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(request.Note);
        return request.Note;
    }
}
