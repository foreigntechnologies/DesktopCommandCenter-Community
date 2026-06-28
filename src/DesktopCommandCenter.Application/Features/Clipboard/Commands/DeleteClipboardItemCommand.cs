using MediatR;

namespace DesktopCommandCenter.Application.Features.Clipboard.Commands;

public record DeleteClipboardItemCommand(Guid Id) : IRequest;
