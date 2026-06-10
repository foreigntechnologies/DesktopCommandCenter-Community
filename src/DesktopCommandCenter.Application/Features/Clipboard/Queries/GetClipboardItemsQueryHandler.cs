using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Clipboard.Queries;

public class GetClipboardItemsQueryHandler : IRequestHandler<GetClipboardItemsQuery, IEnumerable<ClipboardItem>>
{
    private readonly IClipboardRepository _repository;

    public GetClipboardItemsQueryHandler(IClipboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ClipboardItem>> Handle(GetClipboardItemsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
