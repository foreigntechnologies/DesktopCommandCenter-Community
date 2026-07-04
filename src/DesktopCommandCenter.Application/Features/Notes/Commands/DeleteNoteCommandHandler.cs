using DesktopCommandCenter.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Notes.Commands;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
{
    private readonly INoteRepository _repository;

    public DeleteNoteCommandHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}
