using DesktopCommandCenter.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Clipboard.Commands;

public class ClearClipboardCommandHandler : IRequestHandler<ClearClipboardCommand, bool>
{
    private readonly IClipboardRepository _repository;

    public ClearClipboardCommandHandler(IClipboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ClearClipboardCommand request, CancellationToken cancellationToken)
    {
        await _repository.ClearAsync();
        return true;
    }
}
