using DesktopCommandCenter.Domain.Entities;
using MediatR;

namespace DesktopCommandCenter.Application.Features.Notes.Commands;

public record CreateNoteCommand(string Title, string Content, string Category) : IRequest<Note>;
