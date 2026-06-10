using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Clipboard.Commands;

public class CreateClipboardItemCommandHandler : IRequestHandler<CreateClipboardItemCommand, ClipboardItem>
{
    private readonly IClipboardRepository _repository;

    public CreateClipboardItemCommandHandler(IClipboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClipboardItem> Handle(CreateClipboardItemCommand request, CancellationToken cancellationToken)
    {
        var item = new ClipboardItem
        {
            Content = request.Content,
            Type = request.Type,
            ImagePath = request.ImagePath
        };

        return await _repository.AddAsync(item);
    }
}
