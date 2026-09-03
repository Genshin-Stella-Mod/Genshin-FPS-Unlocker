using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using unlockfps_nc.Model;

namespace unlockfps_nc.Utility;

internal static class MonitorUtils
{
	[DllImport("user32.dll")]
	private static extern bool EnumDisplayDevices(string? lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);

	internal static string GetDeviceId(Screen screen)
	{
		var device = new DisplayDevice { cb = Marshal.SizeOf<DisplayDevice>() };
		return EnumDisplayDevices(screen.DeviceName, 0, ref device, 0) ? device.DeviceID : "";
	}

	internal static int GetDisplayNumber(Screen screen)
	{
		var index = screen.DeviceName.IndexOf("DISPLAY", StringComparison.OrdinalIgnoreCase);
		if (index < 0) return 1;

		var suffix = screen.DeviceName[(index + "DISPLAY".Length)..];
		return int.TryParse(suffix, out var number) ? number : 1;
	}

	internal static Screen[] GetOrderedScreens()
	{
		return Screen.AllScreens.OrderByDescending(s => s.Primary).ToArray();
	}

	internal static int ResolveMonitorIndex(Config config)
	{
		return ResolveMonitorIndex(config, GetOrderedScreens());
	}

	internal static bool IsSavedMonitorConnected(Config config)
	{
		if (string.IsNullOrEmpty(config.MonitorId)) return true;

		Screen[] screens = GetOrderedScreens();
		return screens.Any(s => GetDeviceId(s) == config.MonitorId);
	}

	internal static int ResolveMonitorIndex(Config config, Screen[] screens)
	{
		if (!string.IsNullOrEmpty(config.MonitorId))
			for (var i = 0; i < screens.Length; i++)
				if (GetDeviceId(screens[i]) == config.MonitorId)
					return i;

		var fallback = config.MonitorNum - 1;
		return fallback >= 0 && fallback < screens.Length ? fallback : 0;
	}

	internal static (string Name, int Width, int Height, int RefreshRate, string DeviceId) GetMonitorInfo(Screen screen)
	{
		DevMode devMode = GetDeviceMode(screen.DeviceName);
		var width = devMode.dmPelsWidth > 0 ? devMode.dmPelsWidth : screen.Bounds.Width;
		var height = devMode.dmPelsHeight > 0 ? devMode.dmPelsHeight : screen.Bounds.Height;
		var refreshRate = devMode.dmDisplayFrequency > 0 ? devMode.dmDisplayFrequency : 60;

		var monitorDevice = new DisplayDevice { cb = Marshal.SizeOf<DisplayDevice>() };
		if (EnumDisplayDevices(screen.DeviceName, 0, ref monitorDevice, 0))
		{
			var name = GetMonitorName(monitorDevice.DeviceID) ?? monitorDevice.DeviceString;
			return (name, width, height, refreshRate, monitorDevice.DeviceID);
		}

		return (FormatFallbackName(screen.DeviceName), width, height, refreshRate, "");
	}

	private static string FormatFallbackName(string deviceName)
	{
		var index = deviceName.IndexOf("DISPLAY", StringComparison.OrdinalIgnoreCase);
		return index >= 0 ? $"Monitor {deviceName[(index + "DISPLAY".Length)..]}" : deviceName;
	}

	[DllImport("user32.dll")]
	private static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DevMode devMode);

	private static DevMode GetDeviceMode(string deviceName)
	{
		var devMode = new DevMode { dmSize = (short)Marshal.SizeOf<DevMode>() };
		return EnumDisplaySettings(deviceName, -1, ref devMode) ? devMode : default;
	}

	private static string? GetMonitorName(string deviceId)
	{
		try
		{
			var parts = deviceId.Split('\\');
			if (parts.Length < 2) return null;

			using RegistryKey? key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Enum\DISPLAY\{parts[1]}");
			if (key == null) return null;

			foreach (var subKeyName in key.GetSubKeyNames())
			{
				using RegistryKey? subKey = key.OpenSubKey(subKeyName);
				if (subKey == null) continue;

				if (subKey.OpenSubKey("Device Parameters")?.GetValue("EDID") is byte[] edid)
				{
					var edidName = ParseEdidMonitorName(edid);
					if (!string.IsNullOrEmpty(edidName)) return edidName;
				}

				var friendlyName = subKey.GetValue("FriendlyName")?.ToString();
				if (string.IsNullOrEmpty(friendlyName)) continue;

				var cleanName = CleanMonitorName(friendlyName);
				if (!string.IsNullOrEmpty(cleanName) && !cleanName.Contains("Generic"))
					return cleanName;
			}
		}
		catch (Exception ex)
		{
			Program.Logger.Error(ex);
		}

		return null;
	}

	private static string? ParseEdidMonitorName(byte[] edid)
	{
		if (edid.Length < 128) return null;

		for (var offset = 54; offset <= 108; offset += 18)
		{
			if (edid[offset] != 0 || edid[offset + 1] != 0 || edid[offset + 3] != 0xFC) continue;

			Span<byte> nameBytes = edid.AsSpan(offset + 5, 13);
			var terminator = nameBytes.IndexOf((byte)0x0A);
			var length = terminator >= 0 ? terminator : nameBytes.Length;
			var name = Encoding.ASCII.GetString(nameBytes[..length]).Trim();
			if (!string.IsNullOrEmpty(name)) return name;
		}

		return null;
	}

	private static string CleanMonitorName(string rawName)
	{
		if (rawName.Contains(';'))
		{
			var parts = rawName.Split(';');
			var lastPart = parts[^1];
			if (lastPart.StartsWith('(') && lastPart.EndsWith(')'))
				return lastPart.Trim('(', ')');
		}

		if (!rawName.StartsWith('@')) return rawName;

		var index = rawName.IndexOf(';');
		if (index > 0 && index < rawName.Length - 1)
			return rawName[(index + 1)..];

		return rawName;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	private struct DisplayDevice
	{
		[MarshalAs(UnmanagedType.U4)] internal int cb;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string DeviceName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string DeviceString;
		[MarshalAs(UnmanagedType.U4)] internal uint StateFlags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string DeviceID;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string DeviceKey;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct DevMode
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string dmDeviceName;
		internal short dmSpecVersion;
		internal short dmDriverVersion;
		internal short dmSize;
		internal short dmDriverExtra;
		internal int dmFields;
		internal int dmPositionX;
		internal int dmPositionY;
		internal int dmDisplayOrientation;
		internal int dmDisplayFixedOutput;
		internal short dmColor;
		internal short dmDuplex;
		internal short dmYResolution;
		internal short dmTTOption;
		internal short dmCollate;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string dmFormName;
		internal short dmLogPixels;
		internal int dmBitsPerPel;
		internal int dmPelsWidth;
		internal int dmPelsHeight;
		internal int dmDisplayFlags;
		internal int dmDisplayFrequency;
	}
}
