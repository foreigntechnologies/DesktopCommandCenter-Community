using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace DesktopCommandCenter.Application.Features.Notes.Queries;

public record GetNotesQuery : IRequest<IEnumerable<Note>>;
