using DesktopCommandCenter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync();
    Task<Note> AddAsync(Note note);
    Task UpdateAsync(Note note);
    Task DeleteAsync(Guid id);
}
