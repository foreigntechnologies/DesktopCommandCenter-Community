using DesktopCommandCenter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IPromptRepository
{
    Task<IEnumerable<Prompt>> GetAllAsync();
    Task<Prompt?> GetByIdAsync(Guid id);
    Task<Prompt> AddAsync(Prompt prompt);
    Task UpdateAsync(Prompt prompt);
    Task DeleteAsync(Guid id);
}
