using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using unlockfps_nc.Model;
using unlockfps_nc.Properties;
using unlockfps_nc.Service;
using unlockfps_nc.Utility;

namespace unlockfps_nc.Forms;

public partial class MainForm : Form
{
	private readonly Config _config;
	private readonly ConfigService _configService;

	private readonly ProcessService _processService;
	private readonly UpdateCheckService _updateCheckService;
	private Icon? _appIcon;
	private string? _updateReleaseUrl;
	private Point _windowLocation;
	private Size _windowSize;

	public MainForm(ConfigService configService, ProcessService processService, UpdateCheckService updateCheckService)
	{
		InitializeComponent();
		_configService = configService;
		_config = _configService.Config;
		_processService = processService;
		_updateCheckService = updateCheckService;
		SetupBindings();
		RefreshFPSControls();
	}

	private void SettingsMenuItem_Click(object sender, EventArgs e)
	{
		Program.Logger.Info("Opening settings dialog");
		using var form = Program.ServiceProvider.GetRequiredService<SettingsForm>();
		form.ShowDialog();
		RefreshFPSControls();
	}

	private void RefreshFPSControls()
	{
		InputFPS.Value = _config.FPSTarget;
		SliderFPS.Value = _config.FPSTarget;
	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		Program.Logger.Info("Application closing, saving configuration and cleaning up");
		_configService.Save();
		_processService.OnFormClosing();
		NotifyIconMain.Visible = false;
		_appIcon?.Dispose();
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
		Program.Logger.Info("MainForm loaded");
		_windowLocation = Location;
		_windowSize = Size;

		_appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		if (_appIcon != null) Icon = _appIcon;

		NotifyIconMain.BalloonTipClicked += NotifyIconMain_BalloonTipClicked;
		_ = CheckForUpdatesAsync();

		if (_config.AutoStart)
		{
			Program.Logger.Info("Auto-start enabled, starting game automatically");
			BtnStartGame_Click(null, null);
		}
		else if (_config.StartMinimized)
		{
			WindowState = FormWindowState.Minimized;
		}
	}

	private void SetupBindings()
	{
		InputFPS.DataBindings.Add("Value", _config, nameof(_config.FPSTarget), true, DataSourceUpdateMode.OnPropertyChanged);
		SliderFPS.DataBindings.Add("Value", _config, nameof(_config.FPSTarget), true, DataSourceUpdateMode.OnPropertyChanged);
		CBAutoStart.DataBindings.Add("Checked", _config, nameof(_config.AutoStart), true, DataSourceUpdateMode.OnPropertyChanged);
	}

	private void SetupMenuItem_Click(object sender, EventArgs e)
	{
		Program.Logger.Info("Opening setup dialog");
		ShowSetupForm();
	}

	private void BtnStartGame_Click(object? sender, EventArgs? e)
	{
		Program.Logger.Info("User clicked Start Game button");

		if (!File.Exists(_config.GamePath))
		{
			Program.Logger.Info("Game path not configured, opening setup dialog");
			ShowSetupForm();
			if (!File.Exists(_config.GamePath)) return;
		}

		Screen[] screens = MonitorUtils.GetOrderedScreens();
		if (!MonitorUtils.IsSavedMonitorConnected(_config, screens))
		{
			var fallbackIndex = MonitorUtils.ResolveMonitorIndex(_config, screens);
			Program.Logger.Warn($"Saved monitor '{_config.MonitorId}' is not connected, falling back to monitor index {fallbackIndex} and updating configuration");

			_config.MonitorNum = fallbackIndex + 1;
			_configService.UpdateMonitorSettings(fallbackIndex);
			_configService.Save();
			RefreshFPSControls();

			NotifyMonitorNotConnected();
		}

		if (_processService.StartGame())
		{
			Program.Logger.Info("Game started successfully, minimizing to tray");
			WindowState = FormWindowState.Minimized;
		}
		else
		{
			Program.Logger.Warn("Game failed to start");
		}
	}

	private void NotifyMonitorNotConnected()
	{
		_updateReleaseUrl = null;
		ShowTrayIcon();
		NotifyIconMain.BalloonTipIcon = ToolTipIcon.Warning;
		NotifyIconMain.BalloonTipTitle = Resources.MainForm_MonitorNotConnected_Title;
		NotifyIconMain.BalloonTipText = Resources.MainForm_MonitorNotConnected_Text;
		NotifyIconMain.ShowBalloonTip(5000);
	}

	private async Task CheckForUpdatesAsync()
	{
		if (IsStellaModInstalled())
		{
			Program.Logger.Info("Genshin Stella Mod detected, skipping standalone update check");
			return;
		}

		if (DateTime.UtcNow - _config.LastUpdateCheckUtc < TimeSpan.FromHours(24))
		{
			Program.Logger.Info($"Skipping update check, last checked at {_config.LastUpdateCheckUtc:u}");
			return;
		}

		Program.Logger.Info("Checking for a new release on GitHub...");
		UpdateInfo? update = await _updateCheckService.CheckForUpdateAsync();
		_config.LastUpdateCheckUtc = DateTime.UtcNow;

		if (update == null)
		{
			Program.Logger.Info("No new release found, already up to date");
		}
		else if (update.Version.ToString() != _config.LastNotifiedUpdateVersion)
		{
			Program.Logger.Info($"New release available: v{update.Version}");
			_config.LastNotifiedUpdateVersion = update.Version.ToString();
			NotifyUpdateAvailable(update);
		}
		else
		{
			Program.Logger.Info($"New release v{update.Version} was already reported previously, skipping notification");
		}

		_configService.Save();
	}

	private void NotifyUpdateAvailable(UpdateInfo update)
	{
		_updateReleaseUrl = update.Url;
		ShowTrayIcon();
		NotifyIconMain.BalloonTipIcon = ToolTipIcon.None;
		NotifyIconMain.BalloonTipTitle = Resources.MainForm_UpdateAvailable_Title;
		NotifyIconMain.BalloonTipText = string.Format(Resources.MainForm_UpdateAvailable_Text, update.Version);
		NotifyIconMain.ShowBalloonTip(8000);
	}

	private void NotifyIconMain_BalloonTipClicked(object? sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(_updateReleaseUrl)) AboutForm.OpenLink(_updateReleaseUrl);
	}

	private void ShowTrayIcon()
	{
		NotifyIconMain.Icon = _appIcon;
		NotifyIconMain.Visible = true;
	}

	private static void ShowSetupForm()
	{
		using var form = Program.ServiceProvider.GetRequiredService<SetupForm>();
		form.ShowDialog();
	}

	private void ExitMenuItem_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void MainForm_Resize(object sender, EventArgs e)
	{
		if (WindowState == FormWindowState.Minimized && _processService.IsGameRunning()) NotifyAndHide();
	}

	private void NotifyAndHide()
	{
		_updateReleaseUrl = null;
		ShowTrayIcon();
		NotifyIconMain.Text = string.Format(Resources.MainForm_NotifyAndHide_GenshinFPSUnlockerCurrentLimit_, _config.FPSTarget);
		if (_configService.IsFirstRun)
			NotifyIconMain.ShowBalloonTip(500);

		ShowInTaskbar = false;
		Hide();
	}

	private void NotifyIconMain_DoubleClick(object sender, EventArgs e)
	{
		RestoreFromTray();
	}

	public void RestoreFromTray()
	{
		if (InvokeRequired)
		{
			Invoke(RestoreFromTray);
			return;
		}

		WindowState = FormWindowState.Normal;
		ShowInTaskbar = true;
		TopMost = true;
		Show();
		Activate();
		TopMost = false;

		Location = _windowLocation;
		Size = _windowSize;
	}

	private void AboutMenuItem_Click(object sender, EventArgs e)
	{
		using AboutForm aboutForm = new();
		aboutForm.ShowDialog();
	}

	private void StartGameMenuItem_Click(object? sender, EventArgs e)
	{
		BtnStartGame_Click(sender, e);
	}

	private void OpenStella_Click(object sender, EventArgs e)
	{
		using RegistryKey? key = Registry.CurrentUser.OpenSubKey(Program.REGISTRY_PATH);
		var stellaPath = key?.GetValue("StellaPath")?.ToString();

		if (string.IsNullOrEmpty(stellaPath))
		{
			MessageBox.Show(Resources.MainForm_OpenStella_Click_TheRegistryKeyStellaPathWasNotFoundAreYouSureGenshinStellaModIsInstalled, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		var exePath = Path.Combine(stellaPath, "Stella Mod Launcher.exe");
		if (!File.Exists(exePath))
		{
			MessageBox.Show(string.Format(Resources.MainForm_OpenStella_ExecutableNotFound, exePath), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		Process.Start(new ProcessStartInfo { FileName = exePath, WorkingDirectory = stellaPath });
	}

	private static bool IsStellaModInstalled()
	{
		using RegistryKey? key = Registry.CurrentUser.OpenSubKey(Program.REGISTRY_PATH);
		var stellaPath = key?.GetValue("StellaPath")?.ToString();
		return !string.IsNullOrEmpty(stellaPath) && File.Exists(Path.Combine(stellaPath, "Stella Mod Launcher.exe"));
	}

	private void SysInf_Click(object sender, EventArgs e)
	{
		Process.Start("msinfo32.exe");
	}

	private void DxDiag_Click(object sender, EventArgs e)
	{
		Process.Start("dxdiag.exe");
	}

	private void ViewConfig_Click(object sender, EventArgs e)
	{
		var cfgPath = ConfigService.ConfigPath;
		if (!File.Exists(cfgPath))
		{
			MessageBox.Show(Resources.MainForm_ViewCfg_TheUnlockerConfigJsonFileWasNotFound, Resources.MainForm_ViewCfg_FileNotFound, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		Process.Start(new ProcessStartInfo
		{
			FileName = cfgPath,
			UseShellExecute = true
		});
	}

	private void OfficialWebsite_Click(object sender, EventArgs e)
	{
		AboutForm.OpenLink("https://stella.sefinek.net/?referrer=OfficialWebsite_Click");
	}

	private void YouTube_Click(object sender, EventArgs e)
	{
		AboutForm.OpenLink("https://www.youtube.com/channel/UCfPJwxVkrfcJtTDRT7peNyg?referrer=YouTube_Click");
	}

	private void GIReShade_Click(object sender, EventArgs e)
	{
		AboutForm.OpenLink("https://github.com/Genshin-Stella-Mod/Genshin-Impact-ReShade?referrer=GIReShade_Click");
	}

	private void FpsUnlocker_Click(object sender, EventArgs e)
	{
		AboutForm.OpenLink("https://github.com/Genshin-Stella-Mod/Genshin-FPS-Unlocker?referrer=FpsUnlocker_Click");
	}

	private void SefinGitHub_Click(object sender, EventArgs e)
	{
		AboutForm.OpenLink("https://github.com/sefinek?referrer=SefinGitHub_Click");
	}
}
