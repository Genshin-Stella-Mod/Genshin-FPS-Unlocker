using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using unlockfps_nc.Properties;
using unlockfps_nc.Utility;

namespace unlockfps_nc.Service;

internal enum IpcStatus : sbyte
{
	None = 0,
	Error,
	Ready
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct IpcData
{
	internal IpcStatus Status;
	internal int FrameRate;
	internal bool PowerSave;
	internal bool UseMobileUI;
}

public class IpcService(ConfigService configService) : IDisposable
{
	private MemoryMappedFile? _sharedMemory;
	private MemoryMappedViewAccessor? _sharedMemoryAccessor;
	private ModuleGuard _stubModule = IntPtr.Zero;
	private string _stubPath = string.Empty;
	private IntPtr _wndHook = IntPtr.Zero;

	public void Dispose()
	{
		_sharedMemoryAccessor?.Dispose();
		_sharedMemory?.Dispose();
		_stubModule.Dispose();
	}

	internal async Task<bool> StartAsync(int processId)
	{
		Program.Logger.Info($"Starting IPC service for process ID: {processId}");

		_sharedMemory ??= MemoryMappedFile.CreateOrOpen(@"Global\2DE95FDC-6AB7-4593-BFE6-760DD4AB422B", 4096, MemoryMappedFileAccess.ReadWrite);
		_sharedMemoryAccessor ??= _sharedMemory.CreateViewAccessor();
		if (_sharedMemoryAccessor == null)
		{
			Program.Logger.Error("Failed to create shared memory accessor");
			MessageBox.Show(Resources.IpcService_Start_FailedToCreateSharedMemory, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		Update();

		_sharedMemoryAccessor.Read(0, out IpcData ipcData);
		switch (ipcData.Status)
		{
			case IpcStatus.Ready:
				Program.Logger.Info("IPC service already running and ready");
				return true;
			case IpcStatus.Error:
				Program.Logger.Error("IPC service reported error status");
				return false;
		}

		_stubPath = GetUnlockerStubPath();
		Program.Logger.Info($"Loading stub module from: {_stubPath}");
		_stubModule = Native.LoadLibrary(_stubPath);
		if (_stubModule == IntPtr.Zero)
		{
			var error = Marshal.GetLastWin32Error();
			var errorMessage = Marshal.GetLastPInvokeErrorMessage();
			Program.Logger.Error($"LoadLibrary failed with error code {error}: {errorMessage}");
			MessageBox.Show(string.Format(Resources.IpcService_Start_FailedToLoadStubModule, error, errorMessage), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		var stubWndProc = Native.GetProcAddress(_stubModule, "WndProc");
		var targetWindow = ProcessUtils.GetWindowFromProcessId(processId);
		if (targetWindow == IntPtr.Zero)
		{
			var error = Marshal.GetLastWin32Error();
			var errorMessage = Marshal.GetLastPInvokeErrorMessage();
			Program.Logger.Error($"Failed to find game window for process ID: {processId}");
			MessageBox.Show(string.Format(Resources.IpcService_Start_FailedToSetWindowHook, error, errorMessage), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		var threadId = Native.GetWindowThreadProcessId(targetWindow, out _);

		_wndHook = Native.SetWindowsHookEx(3, stubWndProc, _stubModule, threadId);
		if (_wndHook == IntPtr.Zero)
		{
			var error = Marshal.GetLastWin32Error();
			var errorMessage = Marshal.GetLastPInvokeErrorMessage();
			Program.Logger.Error($"SetWindowsHookEx failed with error code {error}: {errorMessage}");
			MessageBox.Show(string.Format(Resources.IpcService_Start_FailedToSetWindowHook, error, errorMessage), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		if (!Native.PostThreadMessage(threadId, 0, IntPtr.Zero, IntPtr.Zero))
		{
			var error = Marshal.GetLastWin32Error();
			var errorMessage = Marshal.GetLastPInvokeErrorMessage();
			Program.Logger.Error($"PostThreadMessage failed with error code {error}: {errorMessage}");
			MessageBox.Show(string.Format(Resources.IpcService_Start_FailedToPostThreadMessage, error, errorMessage), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		var retryCount = 0;
		while (true)
		{
			_sharedMemoryAccessor.Read(0, out ipcData);

			if (ipcData.Status == IpcStatus.Ready)
			{
				Program.Logger.Info("IPC service initialization completed successfully");
				break;
			}

			if (ipcData.Status == IpcStatus.Error)
			{
				Program.Logger.Error("IPC service initialization failed with error status");
				return false;
			}

			if (retryCount >= 20)
			{
				Program.Logger.Error($"IPC initialization timeout after {retryCount} retries");
				MessageBox.Show(Resources.IpcService_Start_FailedToStartUnlocker, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			retryCount++;
			await Task.Delay(1000);
		}

		return true;
	}

	internal void OnGameExit()
	{
		Program.Logger.Info("Game exit detected, cleaning up IPC service");
		var ipcData = new IpcData
		{
			Status = IpcStatus.None
		};

		_sharedMemoryAccessor?.Write(0, ref ipcData);
	}

	internal void Update()
	{
		var ipcData = new IpcData
		{
			FrameRate = configService.Config.FPSTarget,
			PowerSave = configService.Config.UsePowerSave,
			UseMobileUI = configService.Config.UseMobileUI
		};

		_sharedMemoryAccessor?.Write(0, ref ipcData);
	}

	private static string GetUnlockerStubPath()
	{
		var assembly = Assembly.GetExecutingAssembly();
		using Stream stream = assembly.GetManifestResourceStream("unlockfps_nc.Resources.UnlockerStub.dll")!;

		var filePath = Path.Combine(AppContext.BaseDirectory, "UnlockerStub.dll");

		try
		{
			using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
			stream.CopyTo(fileStream);
		}
		catch (Exception ex)
		{
			Program.Logger.Error(ex);
		}

		return filePath;
	}
}
