using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;

namespace DesktopCommandCenter.Infrastructure.Services;

public class ConPTYService : ITerminalService
{
    [StructLayout(LayoutKind.Sequential)]
    private struct COORD
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct STARTUPINFOEX
    {
        public STARTUPINFO StartupInfo;
        public IntPtr lpAttributeList;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int CreatePseudoConsole(COORD size, IntPtr hInput, IntPtr hOutput, uint dwFlags, out IntPtr phPC);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int ResizePseudoConsole(IntPtr hPC, COORD size);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void ClosePseudoConsole(IntPtr hPC);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void DeleteProcThreadAttributeList(IntPtr lpAttributeList);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateProcess(string? lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFOEX lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    private const int S_OK = 0;
    private const int EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
    private const int PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;

    private IntPtr _pseudoConsole;
    private PROCESS_INFORMATION _processInfo;
    private NamedPipeServerStream? _serverIn;
    private NamedPipeServerStream? _serverOut;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private CancellationTokenSource? _cts;

    public event EventHandler<string>? OutputDataReceived;
    public event EventHandler? ProcessExited;

    public Task StartAsync(string commandLine, int columns, int rows)
    {
        _cts = new CancellationTokenSource();

        string pipeInName = $"dcc_in_{Guid.NewGuid():N}";
        string pipeOutName = $"dcc_out_{Guid.NewGuid():N}";

        _serverIn = new NamedPipeServerStream(pipeInName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        var clientIn = new NamedPipeClientStream(".", pipeInName, PipeDirection.In, PipeOptions.None);
        
        _serverOut = new NamedPipeServerStream(pipeOutName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        var clientOut = new NamedPipeClientStream(".", pipeOutName, PipeDirection.Out, PipeOptions.None);

        clientIn.Connect();
        _serverIn.WaitForConnection();

        clientOut.Connect();
        _serverOut.WaitForConnection();

        var size = new COORD { X = (short)columns, Y = (short)rows };
        int hr = CreatePseudoConsole(size, clientIn.SafePipeHandle.DangerousGetHandle(), clientOut.SafePipeHandle.DangerousGetHandle(), 0, out _pseudoConsole);
        if (hr != S_OK)
        {
            throw new Exception($"Failed to create Pseudo Console. HR: {hr}");
        }

        var startupInfo = new STARTUPINFOEX();
        startupInfo.StartupInfo.cb = Marshal.SizeOf<STARTUPINFOEX>();

        IntPtr attrListSize = IntPtr.Zero;
        InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref attrListSize);
        startupInfo.lpAttributeList = Marshal.AllocHGlobal(attrListSize);
        InitializeProcThreadAttributeList(startupInfo.lpAttributeList, 1, 0, ref attrListSize);

        UpdateProcThreadAttribute(startupInfo.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE, _pseudoConsole, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

        var pSec = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>() };
        var tSec = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>() };

        string psPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe");
        
        string executable;
        if (commandLine.Equals("powershell.exe", StringComparison.OrdinalIgnoreCase))
        {
            // Injeta o alias 'ai' nativamente no PowerShell
            executable = $"{psPath} -NoExit -Command \"function ai {{ future.exe ai `$args }}\"";
        }
        else
        {
            executable = commandLine;
        }
        string cwd = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Injetar o future.exe no PATH
        string cliPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".build", "bin", "DesktopCommandCenter.CLI", "Debug", "net9.0-windows10.0.26100.0"));
        if (!Directory.Exists(cliPath))
        {
            // Fallback for release/other builds
            cliPath = Path.Combine(AppContext.BaseDirectory, "tools");
        }
        
        string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        if (!currentPath.Contains(cliPath, StringComparison.OrdinalIgnoreCase))
        {
            Environment.SetEnvironmentVariable("PATH", currentPath + ";" + cliPath);
        }

        bool created = CreateProcess(null, executable, ref pSec, ref tSec, false, EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, cwd, ref startupInfo, out _processInfo);
        
        DeleteProcThreadAttributeList(startupInfo.lpAttributeList);
        Marshal.FreeHGlobal(startupInfo.lpAttributeList);

        if (!created)
        {
            throw new Exception("Failed to create process attached to pseudo console.");
        }

        // Close our handles to the client ends as ConHost now owns them
        clientIn.Dispose();
        clientOut.Dispose();

        _reader = new StreamReader(_serverOut, Encoding.UTF8);
        _writer = new StreamWriter(_serverIn, new UTF8Encoding(false)) { AutoFlush = true };

        _ = Task.Run(() => ReadStreamLoop(_reader, _cts.Token));
        _ = Task.Run(() => MonitorProcess(_processInfo.hProcess, _cts.Token));

        return Task.CompletedTask;
    }

    private void MonitorProcess(IntPtr hProcess, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            uint result = WaitForSingleObject(hProcess, 500);
            if (result == 0) // WAIT_OBJECT_0
            {
                ProcessExited?.Invoke(this, EventArgs.Empty);
                break;
            }
        }
    }

    private async Task ReadStreamLoop(StreamReader reader, CancellationToken token)
    {
        try
        {
            char[] buffer = new char[4096];
            while (!token.IsCancellationRequested)
            {
                int count = await reader.ReadAsync(buffer, 0, buffer.Length);
                if (count == 0) break;

                string text = new string(buffer, 0, count);
                OutputDataReceived?.Invoke(this, text);
            }
        }
        catch (Exception)
        {
            // Stream closed or disposed
        }
    }

    public async Task WriteInputAsync(string data)
    {
        if (_writer != null)
        {
            try
            {
                await _writer.WriteAsync(data);
            }
            catch (Exception)
            {
                // Pipe closed
            }
        }
    }

    public void Resize(int columns, int rows)
    {
        if (_pseudoConsole != IntPtr.Zero)
        {
            var size = new COORD { X = (short)columns, Y = (short)rows };
            ResizePseudoConsole(_pseudoConsole, size);
        }
    }

    public void Stop()
    {
        _cts?.Cancel();

        if (_processInfo.hProcess != IntPtr.Zero)
        {
            // Optional: Terminate process if it hasn't exited
            CloseHandle(_processInfo.hThread);
            CloseHandle(_processInfo.hProcess);
            _processInfo = default;
        }

        if (_pseudoConsole != IntPtr.Zero)
        {
            ClosePseudoConsole(_pseudoConsole);
            _pseudoConsole = IntPtr.Zero;
        }

        _reader?.Dispose();
        _writer?.Dispose();
        _serverIn?.Dispose();
        _serverOut?.Dispose();
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
    }
}
