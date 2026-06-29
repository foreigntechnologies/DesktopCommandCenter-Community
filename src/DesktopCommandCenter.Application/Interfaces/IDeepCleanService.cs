using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IDeepCleanService
{
    Task<List<string>> CleanLeftoversAsync(string appName, string publisherName);
}
