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

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

	[DllImport("user32.dll")]
	internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

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

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern uint ResumeThread(IntPtr hThread);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

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

	[DllImport("psapi.dll", SetLastError = true)]
	internal static extern bool EnumProcessModules(IntPtr hProcess, [Out] IntPtr[] lphModule, uint cb, out uint lpcbNeeded);

	[DllImport("psapi.dll", SetLastError = true)]
	internal static extern bool EnumProcessModulesEx(IntPtr hProcess, [Out] IntPtr[] lphModule, uint cb, out uint lpcbNeeded, uint dwFilterFlag);

	[DllImport("psapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, uint nSize);

	[DllImport("psapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo, uint cb);

	[DllImport("ntdll.dll")]
	internal static extern uint RtlAdjustPrivilege(uint Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);

	internal static bool IsWine()
	{
		var ntdll = GetModuleHandle("ntdll.dll");
		var ver = GetProcAddress(ntdll, "wine_get_version");

		return ver != 0;
	}

	internal static uint GetModuleImageSize(IntPtr lpBaseAddress)
	{
		var dosHeader = Marshal.PtrToStructure<IMAGE_DOS_HEADER>(lpBaseAddress);
		var ntHeader = Marshal.PtrToStructure<IMAGE_NT_HEADERS>(lpBaseAddress + dosHeader.e_lfanew);

		return ntHeader.OptionalHeader.SizeOfImage;
	}

	internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	internal delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
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

internal static class AllocationType
{
	internal const uint COMMIT = 0x1000;
	internal const uint RESERVE = 0x2000;
	internal const uint RESET = 0x80000;
	internal const uint LARGE_PAGES = 0x20000000;
	internal const uint PHYSICAL = 0x400000;
	internal const uint TOP_DOWN = 0x100000;
	internal const uint WRITE_WATCH = 0x200000;
	internal const uint RESET_UNDO = 0x1000000;
}

internal static class FreeType
{
	internal const uint DECOMMIT = 0x4000;
	internal const uint RELEASE = 0x8000;
}

internal static class MemoryProtection
{
	internal const uint EXECUTE = 0x10;
	internal const uint EXECUTE_READ = 0x20;
	internal const uint EXECUTE_READWRITE = 0x40;
	internal const uint EXECUTE_WRITECOPY = 0x80;
	internal const uint NOACCESS = 0x01;
	internal const uint READONLY = 0x02;
	internal const uint READWRITE = 0x04;
	internal const uint WRITECOPY = 0x08;
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

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct IMAGE_DOS_HEADER
{
	internal ushort e_magic; // Magic number
	internal ushort e_cblp; // Bytes on last page of file
	internal ushort e_cp; // Pages in file
	internal ushort e_crlc; // Relocations
	internal ushort e_cparhdr; // Size of header in paragraphs
	internal ushort e_minalloc; // Minimum extra paragraphs needed
	internal ushort e_maxalloc; // Maximum extra paragraphs needed
	internal ushort e_ss; // Initial (relative) SS value
	internal ushort e_sp; // Initial SP value
	internal ushort e_csum; // Checksum
	internal ushort e_ip; // Initial IP value
	internal ushort e_cs; // Initial (relative) CS value
	internal ushort e_lfarlc; // File address of relocation table
	internal ushort e_ovno; // Overlay number
	internal fixed ushort e_res[4]; // Reserved words
	internal ushort e_oemid; // OEM identifier (for e_oeminfo)
	internal ushort e_oeminfo; // OEM information; e_oemid specific
	internal fixed ushort e_res2[10]; // Reserved words
	internal int e_lfanew; // File address of new exe header
}

[StructLayout(LayoutKind.Sequential)]
internal struct IMAGE_NT_HEADERS
{
	internal uint Signature;
	internal IMAGE_FILE_HEADER FileHeader;
	internal IMAGE_OPTIONAL_HEADER64 OptionalHeader;
}

[StructLayout(LayoutKind.Sequential)]
internal struct IMAGE_FILE_HEADER
{
	internal ushort Machine;
	internal ushort NumberOfSections;
	internal uint TimeDateStamp;
	internal uint PointerToSymbolTable;
	internal uint NumberOfSymbols;
	internal ushort SizeOfOptionalHeader;
	internal ushort Characteristics;
}

[StructLayout(LayoutKind.Sequential)]
internal struct IMAGE_OPTIONAL_HEADER64
{
	// Standard fields.
	internal ushort Magic;
	internal byte MajorLinkerVersion;
	internal byte MinorLinkerVersion;
	internal uint SizeOfCode;
	internal uint SizeOfInitializedData;
	internal uint SizeOfUninitializedData;
	internal uint AddressOfEntryPoint;
	internal uint BaseOfCode;

	// Specific to IMAGE_OPTIONAL_HEADER64
	internal ulong ImageBase;
	internal uint SectionAlignment;
	internal uint FileAlignment;
	internal ushort MajorOperatingSystemVersion;
	internal ushort MinorOperatingSystemVersion;
	internal ushort MajorImageVersion;
	internal ushort MinorImageVersion;
	internal ushort MajorSubsystemVersion;
	internal ushort MinorSubsystemVersion;
	internal uint Win32VersionValue;
	internal uint SizeOfImage;
	internal uint SizeOfHeaders;
	internal uint CheckSum;
	internal ushort Subsystem;
	internal ushort DllCharacteristics;
	internal ulong SizeOfStackReserve;
	internal ulong SizeOfStackCommit;
	internal ulong SizeOfHeapReserve;
	internal ulong SizeOfHeapCommit;
	internal uint LoaderFlags;
	internal uint NumberOfRvaAndSizes;

	// Directory Entries
	internal IMAGE_DATA_DIRECTORY ExportTable;
	internal IMAGE_DATA_DIRECTORY ImportTable;
	internal IMAGE_DATA_DIRECTORY ResourceTable;
	internal IMAGE_DATA_DIRECTORY ExceptionTable;
	internal IMAGE_DATA_DIRECTORY CertificateTable;
	internal IMAGE_DATA_DIRECTORY BaseRelocationTable;
	internal IMAGE_DATA_DIRECTORY Debug;
	internal IMAGE_DATA_DIRECTORY Architecture;
	internal IMAGE_DATA_DIRECTORY GlobalPtr;
	internal IMAGE_DATA_DIRECTORY TLSTable;
	internal IMAGE_DATA_DIRECTORY LoadConfigTable;
	internal IMAGE_DATA_DIRECTORY BoundImport;
	internal IMAGE_DATA_DIRECTORY IAT;
	internal IMAGE_DATA_DIRECTORY DelayImportDescriptor;
	internal IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
	internal IMAGE_DATA_DIRECTORY Reserved;
}

[StructLayout(LayoutKind.Sequential)]
internal struct IMAGE_DATA_DIRECTORY
{
	internal uint VirtualAddress;
	internal uint Size;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MODULEINFO
{
	internal IntPtr lpBaseOfDll;
	internal uint SizeOfImage;
	internal IntPtr EntryPoint;
}
