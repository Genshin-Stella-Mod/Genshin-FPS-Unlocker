using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using unlockfps_nc.Model;
using unlockfps_nc.Properties;
using unlockfps_nc.Utility;

namespace unlockfps_nc.Service;

public class ProcessService
{
	private static readonly uint[] PriorityClassMap =
	[
		0x100, // Realtime
		0x80, // High
		0x8000, // Above Normal
		0x20, // Normal
		0x4000, // Below Normal
		0x40 // Idle (Low)
	];

	private readonly Config _config;
	private readonly CancellationTokenSource _cts = new();

	private readonly IpcService _ipcService;
	private IntPtr _gameHandle = IntPtr.Zero;

	public ProcessService(ConfigService configService, IpcService ipcService)
	{
		_config = configService.Config;
		_ipcService = ipcService;

		_ = Task.Run(UnlockerPoll, _cts.Token);
	}

	internal bool StartGame()
	{
		Program.Logger.Info($"Starting game from path: {_config.GamePath}");

		if (!File.Exists(_config.GamePath))
		{
			Program.Logger.Error($"Game executable not found at path: {_config.GamePath}");
			MessageBox.Show(string.Format(Resources.ProcessService_StartGame_GameNotFound, _config.GamePath), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		if (IsGameRunning())
		{
			Program.Logger.Warn("Attempted to start game while another instance is already running");
			MessageBox.Show(Resources.ProcessService_StartGame_GameAlreadyRunning, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return false;
		}

		if (_gameHandle != IntPtr.Zero)
		{
			Native.CloseHandle(_gameHandle);
			_gameHandle = IntPtr.Zero;
		}

		if (_config.UseHDR)
		{
			var subKeyName = Path.GetFileName(_config.GamePath) == "YuanShen.exe" ? "原神" : "Genshin Impact";
			Program.Logger.Info($"Enabling HDR mode for {subKeyName}");
			try
			{
				using RegistryKey key = Registry.CurrentUser.CreateSubKey($@"Software\miHoYo\{subKeyName}");
				key.SetValue("WINDOWS_HDR_ON_h3132281285", 1);
				Program.Logger.Info("HDR registry setting applied successfully");
			}
			catch (Exception e)
			{
				Program.Logger.Error(e, "Failed to enable HDR registry setting");
				MessageBox.Show(string.Format(Resources.ProcessService_StartGame_FailedToEnableHDR, e.Message), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		STARTUPINFO si = new();
		var creationFlag = _config.SuspendLoad ? 4u : 0u;
		var gameFolder = Path.GetDirectoryName(_config.GamePath);

		var commandLine = BuildCommandLine();
		Program.Logger.Info($"Launching game with command line: {commandLine}");

		var commandLineBuffer = new StringBuilder(commandLine);
		if (!Native.CreateProcess(_config.GamePath, commandLineBuffer, IntPtr.Zero, IntPtr.Zero, false, creationFlag, IntPtr.Zero, gameFolder, ref si, out PROCESS_INFORMATION pi))
		{
			var error = Marshal.GetLastWin32Error();
			var errorMessage = Marshal.GetLastPInvokeErrorMessage();
			Program.Logger.Error($"CreateProcess failed with error code {error}: {errorMessage}");
			MessageBox.Show(string.Format(Resources.ProcessService_StartGame_CreateProcessFailed, error, errorMessage), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		if (_config.SuspendLoad)
		{
			Program.Logger.Info("Resuming suspended game thread");
			Native.ResumeThread(pi.hThread);
		}

		_gameHandle = pi.hProcess;
		Program.Logger.Info($"Game process started successfully with PID: {pi.dwProcessId}");

		var priorityClass = PriorityClassMap[_config.Priority];
		if (!Native.SetPriorityClass(_gameHandle, priorityClass))
			Program.Logger.Warn($"SetPriorityClass failed with error: {Marshal.GetLastWin32Error()}");
		else
			Program.Logger.Info($"Process priority set to index {_config.Priority} (class 0x{priorityClass:X})");

		Native.CloseHandle(pi.hThread);
		return true;
	}

	internal void OnFormClosing()
	{
		Program.Logger.Info("ProcessService shutting down");
		_cts.Cancel();
		Native.CloseHandle(_gameHandle);
	}

	internal bool IsGameRunning()
	{
		if (_gameHandle == IntPtr.Zero)
			return false;

		if (!Native.GetExitCodeProcess(_gameHandle, out var exitCode))
			return false;

		return exitCode == 259; // STILL_ACTIVE
	}

	private async Task UnlockerPoll()
	{
		while (!_cts.IsCancellationRequested)
		{
			await Task.Delay(1000, _cts.Token);
			using Process? process = FindGameProcess();
			if (process == null)
				continue;

			Program.Logger.Info($"Game process detected: {process.ProcessName} (PID: {process.Id})");

			while (!ProcessUtils.IsWindowDrawing(process.MainWindowHandle) && !_cts.IsCancellationRequested)
				await Task.Delay(1000, _cts.Token);

			if (!await _ipcService.StartAsync(process.Id))
			{
				Program.Logger.Error("Failed to start IPC service, terminating unlocker poll");
				return;
			}

			Program.Logger.Info("FPS unlocker successfully attached to game process");

			while (!process.HasExited && !_cts.IsCancellationRequested)
			{
				_ipcService.Update();
				await Task.Delay(62, _cts.Token);
			}

			if (_gameHandle != IntPtr.Zero && _config.AutoClose)
			{
				Program.Logger.Info("Game exited, auto-closing application");
				Application.OpenForms[0]?.Invoke(Application.Exit);
			}

			_ipcService.OnGameExit();
			Program.Logger.Info("Game process terminated, waiting for restart");
			await Task.Delay(5000, _cts.Token);
		}
	}

	private static Process? FindGameProcess()
	{
		Process[] processes = Process.GetProcessesByName("GenshinImpact");
		if (processes.Length == 0)
			processes = Process.GetProcessesByName("YuanShen");
		for (var i = 1; i < processes.Length; i++)
			processes[i].Dispose();
		return processes.FirstOrDefault();
	}

	private string BuildCommandLine()
	{
		return BuildCommandLine(_config);
	}

	internal static string BuildCommandLine(Config config)
	{
		var commandLine = $"\"{config.GamePath}\" ";

		commandLine += $"-monitor {ResolveMonitorNum(config)} ";

		if (config.UseCustomRes)
			commandLine += $"-screen-width {config.CustomResX} -screen-height {config.CustomResY} ";

		commandLine += $"-screen-fullscreen {(config.Fullscreen ? 1 : 0)} ";
		if (config.Fullscreen)
			commandLine += $"-window-mode {(config.IsExclusiveFullscreen ? "exclusive" : "borderless")} ";

		if (config.PopupWindow)
			commandLine += "-popupwindow ";

		if (!string.IsNullOrWhiteSpace(config.AdditionalCommandLine))
			commandLine += $"{config.AdditionalCommandLine.Trim()} ";
		return commandLine.TrimEnd();
	}

	private static int ResolveMonitorNum(Config config)
	{
		var screenCount = Screen.AllScreens.Length;

		if (!string.IsNullOrEmpty(config.MonitorId))
			for (var i = 0; i < screenCount; i++)
				if (MonitorUtils.GetMonitorInfo(i).DeviceId == config.MonitorId)
					return i + 1;

		return config.MonitorNum is >= 1 && config.MonitorNum <= screenCount ? config.MonitorNum : 1;
	}
}
