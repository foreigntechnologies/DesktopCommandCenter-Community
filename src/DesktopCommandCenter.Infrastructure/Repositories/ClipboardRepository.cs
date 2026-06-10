using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using DesktopCommandCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Repositories;

public class ClipboardRepository : IClipboardRepository
{
    private readonly AppDbContext _context;

    public ClipboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClipboardItem>> GetAllAsync()
    {
        return await _context.ClipboardItems.ToListAsync();
    }

    public async Task<ClipboardItem> AddAsync(ClipboardItem item)
    {
        _context.ClipboardItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.ClipboardItems.FindAsync(id);
        if (item != null)
        {
            _context.ClipboardItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearAsync()
    {
        _context.ClipboardItems.RemoveRange(_context.ClipboardItems);
        await _context.SaveChangesAsync();
    }
}
