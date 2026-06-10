using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace DesktopCommandCenter.Application.Features.Clipboard.Queries;

public record GetClipboardItemsQuery() : IRequest<IEnumerable<ClipboardItem>>;
