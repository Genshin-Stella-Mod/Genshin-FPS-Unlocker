using unlockfps_nc.Model;
using unlockfps_nc.Properties;
using unlockfps_nc.Service;
using unlockfps_nc.Utility;

namespace unlockfps_nc.Forms;

public partial class SettingsForm : Form
{
	private readonly Config _config;
	private readonly ConfigService _configService;
	private readonly string _configVersionTemplate;
	private readonly string _lastUpdatedTemplate;
	private bool _monitorOverrideActive;
	private NotifyIcon? _monitorOverrideNotifyIcon;

	public SettingsForm(ConfigService configService)
	{
		InitializeComponent();
		_configService = configService;
		_config = _configService.Config;
		_configVersionTemplate = LabelConfigVersion.Text;
		_lastUpdatedTemplate = LabelLastUpdated.Text;

		SetupBindings();
	}

	private void SetupBindings()
	{
		SetupDataBindings();
		SetupManualBindings();
		SetupMonitorCombo();
		SetupConfigInfoLabels();
		UpdateMonitorOverrideState();
	}

	private void SetupConfigInfoLabels()
	{
		LabelConfigVersion.Text = string.Format(_configVersionTemplate, _config.ConfigVersion);

		var lastUpdated = _config.LastModified == default
			? Resources.SettingsForm_LastUpdatedNever
			: _config.LastModified.ToString("g");
		LabelLastUpdated.Text = string.Format(_lastUpdatedTemplate, lastUpdated);
	}

	private void SetupDataBindings()
	{
		CBStartMinimized.DataBindings.Add("Checked", _config, nameof(_config.StartMinimized), true, DataSourceUpdateMode.OnPropertyChanged);
		CBAutoClose.DataBindings.Add("Checked", _config, nameof(_config.AutoClose), true, DataSourceUpdateMode.OnPropertyChanged);
		CBPowerSave.DataBindings.Add("Checked", _config, nameof(_config.UsePowerSave), true, DataSourceUpdateMode.OnPropertyChanged);
		CBHdr.DataBindings.Add("Checked", _config, nameof(_config.UseHDR), true, DataSourceUpdateMode.OnPropertyChanged);
		CBUseMobileUI.DataBindings.Add("Checked", _config, nameof(_config.UseMobileUI), true, DataSourceUpdateMode.OnPropertyChanged);

		ComboPriority.DataBindings.Add("SelectedIndex", _config, nameof(_config.Priority), true, DataSourceUpdateMode.OnPropertyChanged);
	}

	private void SetupManualBindings()
	{
		CBPopup.CheckedChanged -= CBPopup_CheckedChanged;
		CBFullscreen.CheckedChanged -= CBFullscreen_CheckedChanged;
		CBCustomRes.CheckedChanged -= CBCustomRes_CheckedChanged;

		CBPopup.Checked = _config.PopupWindow;
		CBFullscreen.Checked = _config.Fullscreen;
		CBCustomRes.Checked = _config.UseCustomRes;
		ComboFullscreenMode.SelectedIndex = _config.IsExclusiveFullscreen ? 1 : 0;
		InputResX.Value = _config.CustomResX;
		InputResY.Value = _config.CustomResY;
		TextBoxAdditionalCmdLine.Text = _config.AdditionalCommandLine;

		CBPopup.CheckedChanged += CBPopup_CheckedChanged;
		CBFullscreen.CheckedChanged += CBFullscreen_CheckedChanged;
		CBCustomRes.CheckedChanged += CBCustomRes_CheckedChanged;
	}

	private void UpdateControlState()
	{
		CBPopup.Enabled = !_config.Fullscreen;
		CBFullscreen.Enabled = !_config.PopupWindow;
		ComboFullscreenMode.Enabled = _config.Fullscreen && !_config.PopupWindow;

		InputResX.Enabled = _config.UseCustomRes;
		InputResY.Enabled = _config.UseCustomRes;
	}

	private void SettingsForm_Load(object sender, EventArgs e)
	{
		UpdateControlState();
		RefreshCommandPreview();
		ComboFullscreenMode.SelectedIndexChanged += (_, _) =>
		{
			_config.IsExclusiveFullscreen = ComboFullscreenMode.SelectedIndex == 1;
			RefreshCommandPreview();
		};
		InputResX.ValueChanged += (_, _) =>
		{
			_config.CustomResX = (int)InputResX.Value;
			RefreshCommandPreview();
		};
		InputResY.ValueChanged += (_, _) =>
		{
			_config.CustomResY = (int)InputResY.Value;
			RefreshCommandPreview();
		};
		TextBoxAdditionalCmdLine.TextChanged += (_, _) =>
		{
			_config.AdditionalCommandLine = TextBoxAdditionalCmdLine.Text;
			UpdateMonitorOverrideState();
			RefreshCommandPreview();
		};
	}

	private void UpdateMonitorOverrideState()
	{
		var isOverridden = ProcessService.HasManualMonitorOverride(_config);
		ComboMonitor.Enabled = !isOverridden;
		BtnRefreshMonitor.Enabled = !isOverridden;

		if (isOverridden == _monitorOverrideActive) return;
		_monitorOverrideActive = isOverridden;
		if (isOverridden) ShowMonitorOverrideNotification();
	}

	private void ShowMonitorOverrideNotification()
	{
		_monitorOverrideNotifyIcon ??= new NotifyIcon { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };
		_monitorOverrideNotifyIcon.Visible = true;
		_monitorOverrideNotifyIcon.BalloonTipIcon = ToolTipIcon.None;
		_monitorOverrideNotifyIcon.BalloonTipTitle = Resources.SettingsForm_MonitorOverridden_Title;
		_monitorOverrideNotifyIcon.BalloonTipText = Resources.SettingsForm_MonitorOverridden_Text;
		_monitorOverrideNotifyIcon.ShowBalloonTip(5000);
	}

	private void CBCustomRes_CheckedChanged(object? sender, EventArgs e)
	{
		_config.UseCustomRes = CBCustomRes.Checked;
		UpdateControlState();
		RefreshCommandPreview();
	}

	private void CBPopup_CheckedChanged(object? sender, EventArgs e)
	{
		_config.PopupWindow = CBPopup.Checked;
		if (_config is { PopupWindow: true, Fullscreen: true })
		{
			_config.Fullscreen = false;
			CBFullscreen.Checked = false;
		}

		UpdateControlState();
		RefreshCommandPreview();
	}

	private void CBFullscreen_CheckedChanged(object? sender, EventArgs e)
	{
		_config.Fullscreen = CBFullscreen.Checked;
		if (_config is { Fullscreen: true, PopupWindow: true })
		{
			_config.PopupWindow = false;
			CBPopup.Checked = false;
		}

		UpdateControlState();
		RefreshCommandPreview();
	}

	private void SetupMonitorCombo()
	{
		ComboMonitor.SelectedIndexChanged -= ComboMonitor_SelectedIndexChanged;

		ComboMonitor.Items.Clear();
		ComboMonitor.Items.Add(Resources.SettingsForm_MonitorUndefined);
		Screen[] screens = MonitorUtils.GetOrderedScreens();

		foreach (Screen screen in screens)
		{
			var (name, width, height, refreshRate, _) = MonitorUtils.GetMonitorInfo(screen);
			var displayName = $"{name}{(screen.Primary ? " (Main)" : "")} - {width}x{height}@{refreshRate}Hz";
			ComboMonitor.Items.Add(displayName);
		}

		ComboMonitor.SelectedIndex = string.IsNullOrEmpty(_config.MonitorId) ? 0 : MonitorUtils.ResolveMonitorIndex(_config) + 1;
		ComboMonitor.SelectedIndexChanged += ComboMonitor_SelectedIndexChanged;
	}

	private void BtnRefreshMonitor_Click(object? sender, EventArgs e)
	{
		SetupMonitorCombo();
	}

	private void ComboMonitor_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if (ComboMonitor.SelectedIndex == 0)
		{
			_config.MonitorId = "";
			RefreshCommandPreview();
			return;
		}

		var monitorIndex = ComboMonitor.SelectedIndex - 1;
		_config.MonitorNum = monitorIndex + 1;
		_configService.UpdateMonitorSettings(monitorIndex);

		InputResX.Value = _config.CustomResX;
		InputResY.Value = _config.CustomResY;
		RefreshCommandPreview();
	}

	private void RefreshCommandPreview()
	{
		TextBoxCommandLine.Text = ProcessService.BuildCommandLine(_config);
	}

	private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		_configService.Save();
		_monitorOverrideNotifyIcon?.Dispose();
	}
}
