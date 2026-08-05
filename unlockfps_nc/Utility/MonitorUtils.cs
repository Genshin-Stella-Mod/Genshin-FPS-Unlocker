using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace unlockfps_nc.Utility;

internal static class MonitorUtils
{
	[DllImport("user32.dll")]
	private static extern bool EnumDisplayDevices(string? lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);

	internal static (string Name, int Width, int Height, int RefreshRate, bool IsPrimary, string DeviceId) GetMonitorInfo(int monitorIndex)
	{
		var device = new DisplayDevice { cb = Marshal.SizeOf<DisplayDevice>() };

		if (EnumDisplayDevices(null, (uint)monitorIndex, ref device, 0))
		{
			var monitorDevice = new DisplayDevice { cb = Marshal.SizeOf<DisplayDevice>() };

			if (EnumDisplayDevices(device.DeviceName, 0, ref monitorDevice, 0))
			{
				var realName = GetMonitorNameFromRegistry(monitorDevice.DeviceID);
				var name = !string.IsNullOrEmpty(realName) && !realName.Contains("Generic")
					? realName
					: monitorDevice.DeviceString;

				DevMode devMode = GetDeviceMode(device.DeviceName);
				var width = devMode.dmPelsWidth;
				var height = devMode.dmPelsHeight;
				var refreshRate = devMode.dmDisplayFrequency;
				var isPrimary = (device.StateFlags & 4) != 0;

				return (name, width, height, refreshRate, isPrimary, monitorDevice.DeviceID);
			}
		}

		return ($"Monitor {monitorIndex + 1}", 0, 0, 60, false, "");
	}

	[DllImport("user32.dll")]
	private static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DevMode devMode);

	private static DevMode GetDeviceMode(string deviceName)
	{
		var devMode = new DevMode { dmSize = (short)Marshal.SizeOf<DevMode>() };
		return EnumDisplaySettings(deviceName, -1, ref devMode) ? devMode : default;
	}

	private static string? GetMonitorNameFromRegistry(string deviceId)
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
				var friendlyName = subKey?.GetValue("FriendlyName")?.ToString();
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
