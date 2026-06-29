using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFOEX
    {
        public STARTUPINFO StartupInfo;
        public IntPtr lpAttributeList;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFO
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
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int CreatePseudoConsole(COORD size, IntPtr hInput, IntPtr hOutput, uint dwFlags, out IntPtr phPC);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ClosePseudoConsole(IntPtr hPC);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void DeleteProcThreadAttributeList(IntPtr lpAttributeList);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFOEX lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    public const int S_OK = 0;
    public const int EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
    public const int PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;

    static async Task Main()
    {
        string pipeInName = $"dcc_in_{Guid.NewGuid():N}";
        string pipeOutName = $"dcc_out_{Guid.NewGuid():N}";

        using var serverIn = new NamedPipeServerStream(pipeInName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        using var clientIn = new NamedPipeClientStream(".", pipeInName, PipeDirection.In, PipeOptions.None);
        
        using var serverOut = new NamedPipeServerStream(pipeOutName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        using var clientOut = new NamedPipeClientStream(".", pipeOutName, PipeDirection.Out, PipeOptions.None);

        _ = Task.Run(() => clientIn.Connect());
        serverIn.WaitForConnection();

        _ = Task.Run(() => clientOut.Connect());
        serverOut.WaitForConnection();

        Console.WriteLine("Pipes connected.");

        IntPtr hPc = IntPtr.Zero;
        try
        {
            var size = new COORD { X = 120, Y = 30 };
            int hr = CreatePseudoConsole(size, clientIn.SafePipeHandle.DangerousGetHandle(), clientOut.SafePipeHandle.DangerousGetHandle(), 0, out hPc);
            if (hr != S_OK) throw new Exception("CreatePseudoConsole failed");
            
            var startupInfo = new STARTUPINFOEX();
            startupInfo.StartupInfo.cb = Marshal.SizeOf<STARTUPINFOEX>();

            IntPtr attrListSize = IntPtr.Zero;
            InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref attrListSize);
            startupInfo.lpAttributeList = Marshal.AllocHGlobal(attrListSize);
            InitializeProcThreadAttributeList(startupInfo.lpAttributeList, 1, 0, ref attrListSize);

            UpdateProcThreadAttribute(startupInfo.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE, hPc, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

            var pSec = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>() };
            var tSec = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>() };

            string cmd = "powershell.exe";
            bool created = CreateProcess(null, cmd, ref pSec, ref tSec, false, EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, Environment.CurrentDirectory, ref startupInfo, out var pInfo);
            
            DeleteProcThreadAttributeList(startupInfo.lpAttributeList);
            Marshal.FreeHGlobal(startupInfo.lpAttributeList);

            if (!created) throw new Exception("CreateProcess failed");
            
            Console.WriteLine($"Process started {pInfo.dwProcessId}");

            // Close client ends of the pipes as they are duplicated in ConHost
            clientIn.Dispose();
            clientOut.Dispose();

            var reader = new StreamReader(serverOut, Encoding.UTF8);
            var writer = new StreamWriter(serverIn, new UTF8Encoding(false)) { AutoFlush = true };

            var readTask = Task.Run(async () => {
                var buffer = new char[1024];
                while (true) {
                    int count = await reader.ReadAsync(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    Console.WriteLine($"[OUT] {new string(buffer, 0, count)}");
                }
            });

            await Task.Delay(2000);
            await writer.WriteAsync("echo 'hello from conpty'\r");
            await Task.Delay(2000);
        }
        finally
        {
            if (hPc != IntPtr.Zero) ClosePseudoConsole(hPc);
        }
    }
}
