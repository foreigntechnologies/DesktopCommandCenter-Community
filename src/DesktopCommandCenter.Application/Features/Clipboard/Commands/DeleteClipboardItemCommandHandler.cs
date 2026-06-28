using DesktopCommandCenter.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Clipboard.Commands;

public class DeleteClipboardItemCommandHandler : IRequestHandler<DeleteClipboardItemCommand>
{
    private readonly IClipboardRepository _repository;

    public DeleteClipboardItemCommandHandler(IClipboardRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteClipboardItemCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}
