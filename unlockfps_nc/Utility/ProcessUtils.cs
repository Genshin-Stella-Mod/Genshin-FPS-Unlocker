using System.Text;

namespace unlockfps_nc.Utility;

internal static class ProcessUtils
{
	internal static string GetProcessPath(IntPtr hProcess)
	{
		if (hProcess == IntPtr.Zero)
			return string.Empty;

		var sb = new StringBuilder(1024);
		uint bufferSize = (uint)sb.Capacity;
		return !Native.QueryFullProcessImageName(hProcess, 0, sb, ref bufferSize) ? string.Empty : sb.ToString();
	}

	internal static IntPtr GetWindowFromProcessId(int processId)
	{
		IntPtr windowHandle = IntPtr.Zero;

		Native.EnumWindows((hWnd, _) =>
		{
			Native.GetWindowThreadProcessId(hWnd, out uint pid);
			if (pid != processId) return true;

			windowHandle = hWnd;
			return false;

		}, IntPtr.Zero);

		return windowHandle;
	}

	internal static bool IsWindowDrawing(IntPtr hWnd)
	{
		if (!Native.IsWindowVisible(hWnd))
			return false;

		Native.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, 0x122); // RDW_INTERNALPAINT | RDW_NOERASE | RDW_UPDATENOW
		Native.UpdateWindow(hWnd);

		IntPtr hdc = Native.GetDC(hWnd);
		if (hdc == IntPtr.Zero)
			return false;

		Native.ReleaseDC(hWnd, hdc);
		return true;
	}
}
