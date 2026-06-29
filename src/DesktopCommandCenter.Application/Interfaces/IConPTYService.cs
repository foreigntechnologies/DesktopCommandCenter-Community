using System;

namespace DesktopCommandCenter.Application.Interfaces;

public interface IConPTYService : IDisposable
{
    event EventHandler<string> OutputDataReceived;
    event EventHandler ProcessExited;

    void Start(string command);
    void WriteInput(string data);
    void Resize(short columns, short rows);
}
