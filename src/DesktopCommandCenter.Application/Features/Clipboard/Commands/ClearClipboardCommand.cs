using MediatR;

namespace DesktopCommandCenter.Application.Features.Clipboard.Commands;

public record ClearClipboardCommand() : IRequest<bool>;
