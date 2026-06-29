using DesktopCommandCenter.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IClipboardRepository
{
    Task<IEnumerable<ClipboardItem>> GetAllAsync();
    Task<ClipboardItem> AddAsync(ClipboardItem item);
    Task DeleteAsync(Guid id);
    Task ClearAsync();
}
