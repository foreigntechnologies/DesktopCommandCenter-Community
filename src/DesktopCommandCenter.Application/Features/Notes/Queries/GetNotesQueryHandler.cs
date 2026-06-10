using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Features.Notes.Queries;

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, IEnumerable<Note>>
{
    private readonly INoteRepository _repository;

    public GetNotesQueryHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Note>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
