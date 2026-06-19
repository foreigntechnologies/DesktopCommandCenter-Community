using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Infrastructure.Native;

namespace DesktopCommandCenter.Infrastructure.Services;

public class ConPTYService : ITerminalService
{
    private IntPtr _pseudoConsole = IntPtr.Zero;
    private TerminalNativeMethods.PROCESS_INFORMATION _processInfo;
    
    private SafeFileHandle _hInputRead = null!;
    private SafeFileHandle _hInputWrite = null!;
    private SafeFileHandle _hOutputRead = null!;
    private SafeFileHandle _hOutputWrite = null!;

    private FileStream _inputStream = null!;
    private FileStream _outputStream = null!;
    
    private Thread _readerThread = null!;
    private CancellationTokenSource _cts = null!;

    public event EventHandler<string>? OutputDataReceived;
    public event EventHandler? ProcessExited;

    public Task StartAsync(string commandLine, int columns, int rows)
    {
        _cts = new CancellationTokenSource();

        CreatePipes();

        var size = new TerminalNativeMethods.COORD { X = (short)columns, Y = (short)rows };
        int hr = TerminalNativeMethods.CreatePseudoConsole(size, _hInputRead, _hOutputWrite, 0, out _pseudoConsole);
        if (hr != TerminalNativeMethods.S_OK)
        {
            throw new Exception($"CreatePseudoConsole failed with HRESULT {hr:X}");
        }

        var startupInfo = new TerminalNativeMethods.STARTUPINFOEX();
        startupInfo.StartupInfo.cb = Marshal.SizeOf<TerminalNativeMethods.STARTUPINFOEX>();

        IntPtr attrListSize = IntPtr.Zero;
        TerminalNativeMethods.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref attrListSize);
        
        startupInfo.lpAttributeList = Marshal.AllocHGlobal(attrListSize);
        TerminalNativeMethods.InitializeProcThreadAttributeList(startupInfo.lpAttributeList, 1, 0, ref attrListSize);

        IntPtr hPcPtr = Marshal.AllocHGlobal(IntPtr.Size);
        Marshal.WriteIntPtr(hPcPtr, _pseudoConsole);
        
        TerminalNativeMethods.UpdateProcThreadAttribute(
            startupInfo.lpAttributeList,
            0,
            (IntPtr)TerminalNativeMethods.PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE,
            hPcPtr,
            (IntPtr)IntPtr.Size,
            IntPtr.Zero,
            IntPtr.Zero);

        var pSec = new TerminalNativeMethods.SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<TerminalNativeMethods.SECURITY_ATTRIBUTES>() };
        var tSec = new TerminalNativeMethods.SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<TerminalNativeMethods.SECURITY_ATTRIBUTES>() };

        var cmdBuilder = new StringBuilder(commandLine);

        bool created = TerminalNativeMethods.CreateProcess(
            null!,
            cmdBuilder,
            ref pSec,
            ref tSec,
            false,
            TerminalNativeMethods.EXTENDED_STARTUPINFO_PRESENT,
            IntPtr.Zero,
            null!,
            ref startupInfo,
            out _processInfo);

        TerminalNativeMethods.DeleteProcThreadAttributeList(startupInfo.lpAttributeList);
        Marshal.FreeHGlobal(startupInfo.lpAttributeList);
        Marshal.FreeHGlobal(hPcPtr);

        if (!created)
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        // We can close our ends of the pipes that the pseudo console uses
        _hInputRead.Dispose();
        _hOutputWrite.Dispose();

        _inputStream = new FileStream(_hInputWrite, FileAccess.Write, 4096, true);
        _outputStream = new FileStream(_hOutputRead, FileAccess.Read, 4096, true);

        _readerThread = new Thread(ReadOutputLoop) { IsBackground = true };
        _readerThread.Start();

        return Task.CompletedTask;
    }

    private void CreatePipes()
    {
        var sa = new TerminalNativeMethods.SECURITY_ATTRIBUTES
        {
            nLength = Marshal.SizeOf<TerminalNativeMethods.SECURITY_ATTRIBUTES>(),
            bInheritHandle = 1,
            lpSecurityDescriptor = IntPtr.Zero
        };

        if (!TerminalNativeMethods.CreatePipe(out _hInputRead, out _hInputWrite, ref sa, 0))
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

        if (!TerminalNativeMethods.CreatePipe(out _hOutputRead, out _hOutputWrite, ref sa, 0))
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
    }

    private void ReadOutputLoop()
    {
        byte[] buffer = new byte[4096];
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                int bytesRead = _outputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // EOF

                string text = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                OutputDataReceived?.Invoke(this, text);
            }
        }
        catch (Exception)
        {
            // Usually means stream closed or task canceled
        }
        finally
        {
            ProcessExited?.Invoke(this, EventArgs.Empty);
            Stop();
        }
    }

    public async Task WriteInputAsync(string data)
    {
        if (_inputStream != null && _inputStream.CanWrite)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            await _inputStream.WriteAsync(bytes, 0, bytes.Length);
            await _inputStream.FlushAsync();
        }
    }

    public void Resize(int columns, int rows)
    {
        if (_pseudoConsole != IntPtr.Zero)
        {
            var size = new TerminalNativeMethods.COORD { X = (short)columns, Y = (short)rows };
            TerminalNativeMethods.ResizePseudoConsole(_pseudoConsole, size);
        }
    }

    public void Stop()
    {
        _cts?.Cancel();

        _inputStream?.Dispose();
        _outputStream?.Dispose();

        if (_processInfo.hProcess != IntPtr.Zero)
        {
            TerminalNativeMethods.CloseHandle(_processInfo.hThread);
            TerminalNativeMethods.CloseHandle(_processInfo.hProcess);
            _processInfo.hProcess = IntPtr.Zero;
        }

        if (_pseudoConsole != IntPtr.Zero)
        {
            TerminalNativeMethods.ClosePseudoConsole(_pseudoConsole);
            _pseudoConsole = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
