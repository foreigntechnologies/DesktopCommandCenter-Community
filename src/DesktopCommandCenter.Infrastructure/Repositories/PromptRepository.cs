using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using DesktopCommandCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Repositories;

public class PromptRepository : IPromptRepository
{
    private readonly AppDbContext _context;

    public PromptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Prompt>> GetAllAsync()
    {
        return await _context.Prompts.ToListAsync();
    }

    public async Task<Prompt?> GetByIdAsync(Guid id)
    {
        return await _context.Prompts.FindAsync(id);
    }

    public async Task<Prompt> AddAsync(Prompt prompt)
    {
        _context.Prompts.Add(prompt);
        await _context.SaveChangesAsync();
        return prompt;
    }

    public async Task UpdateAsync(Prompt prompt)
    {
        _context.Entry(prompt).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var prompt = await GetByIdAsync(id);
        if (prompt != null)
        {
            _context.Prompts.Remove(prompt);
            await _context.SaveChangesAsync();
        }
    }
}
