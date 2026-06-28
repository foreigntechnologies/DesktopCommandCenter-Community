using DesktopCommandCenter.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IProcessService
{
    Task<IEnumerable<ProcessInfo>> GetProcessesAsync();
    Task KillProcessTreeAsync(int processId);
}
