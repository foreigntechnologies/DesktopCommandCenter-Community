using DesktopCommandCenter.Domain.Entities;
using MediatR;

namespace DesktopCommandCenter.Application.Features.Clipboard.Commands;

public record CreateClipboardItemCommand(string Content, string Type = "Text") : IRequest<ClipboardItem>;
