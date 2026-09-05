using System.Runtime.InteropServices;
using System.Text;

namespace unlockfps_nc.Utility;

internal static class Native
{

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll")]
	internal static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

	[DllImport("user32.dll")]
	internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll")]
	internal static extern IntPtr SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll")]
	internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern bool PostThreadMessage(uint idThread, uint Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	internal static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	internal static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	internal static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

	[DllImport("user32.dll")]
	internal static extern bool UpdateWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool CloseHandle(IntPtr hHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

	[DllImport("kernel32.dll")]
	internal static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	internal static extern bool CreateProcess(string lpApplicationName, StringBuilder lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string? lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern IntPtr LoadLibrary(string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

	[DllImport("kernel32.dll")]
	internal static extern void FreeLibrary(IntPtr handle);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("kernel32.dll")]
	internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

	[DllImport("kernel32.dll")]
	internal static extern bool SetPriorityClass(IntPtr hProcess, uint dwPriorityClass);

	internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
}

internal class ModuleGuard(IntPtr module) : IDisposable
{
	private IntPtr BaseAddress => module & ~3;

	public void Dispose()
	{
		if (this)
			Native.FreeLibrary(module);
	}

	public static implicit operator ModuleGuard(IntPtr module)
	{
		return new ModuleGuard(module);
	}

	public static implicit operator IntPtr(ModuleGuard guard)
	{
		return guard.BaseAddress;
	}

	public static implicit operator bool(ModuleGuard guard)
	{
		return guard.BaseAddress != IntPtr.Zero;
	}
}

internal static class ProcessAccess
{
	internal const uint TERMINATE = 0x0001;
	internal const uint CREATE_THREAD = 0x0002;
	internal const uint SET_SESSIONID = 0x0004;
	internal const uint VM_OPERATION = 0x0008;
	internal const uint VM_READ = 0x0010;
	internal const uint VM_WRITE = 0x0020;
	internal const uint DUP_HANDLE = 0x0040;
	internal const uint CREATE_PROCESS = 0x0080;
	internal const uint SET_QUOTA = 0x0100;
	internal const uint SET_INFORMATION = 0x0200;
	internal const uint QUERY_INFORMATION = 0x0400;
	internal const uint SUSPEND_RESUME = 0x0800;
	internal const uint QUERY_LIMITED_INFORMATION = 0x1000;
	internal const uint SET_LIMITED_INFORMATION = 0x2000;
	internal const uint ALL_ACCESS = 0x1FFFFF;
}

internal static class StandardAccess
{
	internal const uint DELETE = 0x00010000;
	internal const uint READ_CONTROL = 0x00020000;
	internal const uint WRITE_DAC = 0x00040000;
	internal const uint WRITE_OWNER = 0x00080000;
	internal const uint SYNCHRONIZE = 0x00100000;
	internal const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
	internal const uint STANDARD_RIGHTS_READ = READ_CONTROL;
	internal const uint STANDARD_RIGHTS_WRITE = READ_CONTROL;
	internal const uint STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
	internal const uint STANDARD_RIGHTS_ALL = 0x001F0000;
	internal const uint SPECIFIC_RIGHTS_ALL = 0x0000FFFF;
}

[StructLayout(LayoutKind.Sequential)]
internal struct PROCESS_INFORMATION
{
	internal IntPtr hProcess;
	internal IntPtr hThread;
	internal int dwProcessId;
	internal int dwThreadId;
}

[StructLayout(LayoutKind.Sequential)]
internal struct STARTUPINFO
{
	internal int cb;
	internal string lpReserved;
	internal string lpDesktop;
	internal string lpTitle;
	internal int dwX;
	internal int dwY;
	internal int dwXSize;
	internal int dwYSize;
	internal int dwXCountChars;
	internal int dwYCountChars;
	internal int dwFillAttribute;
	internal int dwFlags;
	internal short wShowWindow;
	internal short cbReserved2;
	internal IntPtr lpReserved2;
	internal IntPtr hStdInput;
	internal IntPtr hStdOutput;
	internal IntPtr hStdError;
}
