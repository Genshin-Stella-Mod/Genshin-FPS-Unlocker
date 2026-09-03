using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using unlockfps_nc.Model;
using unlockfps_nc.Utility;

namespace unlockfps_nc.Service;

public class ConfigService
{
	private const int CurrentConfigVersion = 1;
	internal static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "unlocker.config.json");
	private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };
	private readonly Lock _lock = new();

	public ConfigService()
	{
		IsFirstRun = !File.Exists(ConfigPath);

		Program.Logger.Info(IsFirstRun ? $"First run detected, creating new config at: {ConfigPath}" : $"Loading existing config from: {ConfigPath}");

		var loaded = Load();
		Sanitize();
		LoadGamePathFromRegistry();

		if (!loaded)
		{
			InitializePrimaryMonitor();
			Config.ConfigVersion = CurrentConfigVersion;
		}
		else
		{
			MigrateMonitorSettings();
		}
	}

	internal bool IsFirstRun { get; }
	internal Config Config { get; private set; } = new();

	private bool Load()
	{
		if (!File.Exists(ConfigPath)) return false;

		try
		{
			var json = File.ReadAllText(ConfigPath);
			var config = JsonSerializer.Deserialize<Config>(json);
			if (config == null) return false;

			Config = config;
			Program.Logger.Info("Configuration loaded successfully");
			return true;
		}
		catch (Exception e)
		{
			Program.Logger.Error(e, "Failed to load configuration file, reinitializing from monitor data");
			return false;
		}
	}

	private void Sanitize()
	{
		Config.FPSTarget = Math.Clamp(Config.FPSTarget, 1, 540);
		Config.Priority = Math.Clamp(Config.Priority, 0, 5);
		Config.CustomResX = Math.Clamp(Config.CustomResX, 200, 7680);
		Config.CustomResY = Math.Clamp(Config.CustomResY, 200, 4320);
		Config.MonitorNum = Math.Clamp(Config.MonitorNum, 1, 100);
	}

	private void LoadGamePathFromRegistry()
	{
		if (!string.IsNullOrEmpty(Config.GamePath)) return;

		using RegistryKey? key = Registry.CurrentUser.OpenSubKey(Program.REGISTRY_PATH);
		var gamePath = key?.GetValue("GamePath") as string;
		if (string.IsNullOrEmpty(gamePath) || !File.Exists(gamePath)) return;

		Config.GamePath = gamePath;
		Program.Logger.Info($"Game path loaded from registry: {gamePath}");
	}

	private void InitializePrimaryMonitor()
	{
		if (MonitorUtils.GetOrderedScreens().Length == 0)
		{
			Program.Logger.Warn("Could not detect primary monitor, using default settings");
			return;
		}

		Config.MonitorNum = 1;
		UpdateMonitorSettings(0);
		Program.Logger.Info($"Initialized primary monitor settings: {Config.CustomResX}x{Config.CustomResY} @{Config.FPSTarget}Hz");
	}

	private void MigrateMonitorSettings()
	{
		if (Config.ConfigVersion >= CurrentConfigVersion) return;

		Screen[] screens = Screen.AllScreens;
		var index = Config.MonitorNum - 1;
		if (index >= 0 && index < screens.Length)
		{
			var (_, width, height, _, deviceId) = MonitorUtils.GetMonitorInfo(screens[index]);
			Config.MonitorId = deviceId;

			if (Config.CustomResX == 1920 && Config.CustomResY == 1080)
			{
				if (width > 0) Config.CustomResX = width;
				if (height > 0) Config.CustomResY = height;
			}

			Program.Logger.Info($"Resynced monitor id/resolution for monitor index {Config.MonitorNum} during config migration");
		}

		Config.ConfigVersion = CurrentConfigVersion;
	}

	internal void UpdateMonitorSettings(int monitorIndex)
	{
		Screen[] screens = MonitorUtils.GetOrderedScreens();
		if (monitorIndex < 0 || monitorIndex >= screens.Length)
		{
			Config.MonitorId = "";
			return;
		}

		var (_, width, height, refreshRate, deviceId) = MonitorUtils.GetMonitorInfo(screens[monitorIndex]);
		Config.FPSTarget = refreshRate > 0 ? refreshRate : 60;
		Config.CustomResX = width > 0 ? width : 1920;
		Config.CustomResY = height > 0 ? height : 1080;
		Config.MonitorId = deviceId;
	}

	internal void Save()
	{
		lock (_lock)
		{
			try
			{
				Config.LastModified = DateTime.Now;
				var json = JsonSerializer.Serialize(Config, WriteOptions);

				var wasHidden = false;
				if (File.Exists(ConfigPath))
				{
					FileAttributes attributes = File.GetAttributes(ConfigPath);
					if ((attributes & FileAttributes.Hidden) != 0)
					{
						wasHidden = true;
						File.SetAttributes(ConfigPath, attributes & ~FileAttributes.Hidden);
					}
				}

				try
				{
					using var fs = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
					using var sw = new StreamWriter(fs, Encoding.UTF8);
					sw.Write(json);
				}
				finally
				{
					if (wasHidden && File.Exists(ConfigPath))
					{
						FileAttributes attributes = File.GetAttributes(ConfigPath);
						File.SetAttributes(ConfigPath, attributes | FileAttributes.Hidden);
					}
				}

				Program.Logger.Info("Configuration saved successfully");
			}
			catch (Exception e)
			{
				Program.Logger.Error(e, "Failed to save configuration file");
			}
		}
	}
}
