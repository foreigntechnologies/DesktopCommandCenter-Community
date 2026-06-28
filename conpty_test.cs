using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

class Program
{
    internal const int S_OK = 0;
    internal const int EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
    internal const int PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;

    [StructLayout(LayoutKind.Sequential)]
    internal struct COORD { public short X; public short Y; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct STARTUPINFOEX
    {
        public STARTUPINFO StartupInfo;
        public IntPtr lpAttributeList;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX; public int dwY; public int dwXSize; public int dwYSize;
        public int dwXCountChars; public int dwYCountChars; public int dwFillAttribute;
        public int dwFlags; public short wShowWindow; public short cbReserved2;
        public IntPtr lpReserved2; public IntPtr hStdInput; public IntPtr hStdOutput; public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_INFORMATION { public IntPtr hProcess; public IntPtr hThread; public int dwProcessId; public int dwThreadId; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES { public int nLength; public IntPtr lpSecurityDescriptor; public int bInheritHandle; }

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern int CreatePseudoConsole(COORD size, SafeFileHandle hInput, SafeFileHandle hOutput, uint dwFlags, out IntPtr phPC);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern void ClosePseudoConsole(IntPtr hPC);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool CreateProcess(string lpApplicationName, [In, Out] StringBuilder lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern void DeleteProcThreadAttributeList(IntPtr lpAttributeList);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr hObject);

    static void Main(string[] args)
    {
        var sa = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>(), bInheritHandle = 1 };
        CreatePipe(out var hInputRead, out var hInputWrite, ref sa, 0);
        CreatePipe(out var hOutputRead, out var hOutputWrite, ref sa, 0);

        CreatePseudoConsole(new COORD { X = 80, Y = 30 }, hInputRead, hOutputWrite, 0, out var pseudoConsole);

        var startupInfo = new STARTUPINFOEX();
        startupInfo.StartupInfo.cb = Marshal.SizeOf<STARTUPINFOEX>();

        IntPtr attrListSize = IntPtr.Zero;
        InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref attrListSize);
        startupInfo.lpAttributeList = Marshal.AllocHGlobal(attrListSize);
        InitializeProcThreadAttributeList(startupInfo.lpAttributeList, 1, 0, ref attrListSize);

        IntPtr hPcPtr = Marshal.AllocHGlobal(IntPtr.Size);
        Marshal.WriteIntPtr(hPcPtr, pseudoConsole);
        
        UpdateProcThreadAttribute(startupInfo.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE, hPcPtr, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);

        var pSec = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>() };
        var tSec = new SECURITY_ATTRIBUTES { nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>() };
        
        var cmd = new StringBuilder("powershell.exe");
        string cwd = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        bool created = CreateProcess(null, cmd, ref pSec, ref tSec, false, EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, cwd, ref startupInfo, out var pi);

        if (!created)
        {
            Console.WriteLine("CreateProcess failed: " + Marshal.GetLastWin32Error());
            return;
        }

        Console.WriteLine("Started process " + pi.dwProcessId);
        Thread.Sleep(2000); // Wait to see if it crashes with 0xc0000142
        Console.WriteLine("Finished");
    }
}
