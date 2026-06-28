using System;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Application.Interfaces;

public interface ITerminalService : IDisposable
{
    event EventHandler<string> OutputDataReceived;
    event EventHandler ProcessExited;
    
    Task StartAsync(string commandLine, int columns, int rows);
    Task WriteInputAsync(string data);
    void Resize(int columns, int rows);
    void Stop();
}
