using unlockfps_nc.Properties;

namespace unlockfps_nc.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			OptionsMenuStrip = new MenuStrip();
			openToolStripMenuItem = new ToolStripMenuItem();
			stellaLauncherToolStrip = new ToolStripMenuItem();
			checkHzToolStrip = new ToolStripMenuItem();
			systemInformationToolStripMenuItem = new ToolStripMenuItem();
			dxDiaxToolStripMenuItem = new ToolStripMenuItem();
			configFileToolStrip = new ToolStripMenuItem();
			linksToolStripMenuItem = new ToolStripMenuItem();
			officialWebsiteToolStripMenuItem = new ToolStripMenuItem();
			youTubeToolStripMenuItem = new ToolStripMenuItem();
			gitHubToolStripMenuItem = new ToolStripMenuItem();
			repositoresToolStripMenuItem = new ToolStripMenuItem();
			genshinImpactReShadeToolStripMenuItem = new ToolStripMenuItem();
			fPSUnlockerToolStripMenuItem = new ToolStripMenuItem();
			sefineksProfileToolStripMenuItem = new ToolStripMenuItem();
			optionsToolStripMenuItem = new ToolStripMenuItem();
			SettingsMenuItem = new ToolStripMenuItem();
			SetupMenuItem = new ToolStripMenuItem();
			aboutToolStrip = new ToolStripMenuItem();
			LabelFPS = new Label();
			InputFPS = new NumericUpDown();
			SliderFPS = new TrackBar();
			CBAutoStart = new CheckBox();
			BtnStartGame = new Button();
			ToolTipMain = new ToolTip(components);
			ContextNotify = new ContextMenuStrip(components);
			StartGameMenuItem = new ToolStripMenuItem();
			ExitMenuItem = new ToolStripMenuItem();
			pictureBox1 = new PictureBox();
			NotifyIconMain = new NotifyIcon(components);
			OptionsMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)InputFPS).BeginInit();
			((System.ComponentModel.ISupportInitialize)SliderFPS).BeginInit();
			ContextNotify.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// OptionsMenuStrip
			// 
			resources.ApplyResources(OptionsMenuStrip, "OptionsMenuStrip");
			OptionsMenuStrip.BackColor = Color.FromArgb(32, 34, 37);
			OptionsMenuStrip.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, linksToolStripMenuItem, optionsToolStripMenuItem, aboutToolStrip });
			OptionsMenuStrip.Name = "OptionsMenuStrip";
			ToolTipMain.SetToolTip(OptionsMenuStrip, resources.GetString("OptionsMenuStrip.ToolTip"));
			// 
			// openToolStripMenuItem
			// 
			resources.ApplyResources(openToolStripMenuItem, "openToolStripMenuItem");
			openToolStripMenuItem.BackColor = Color.Transparent;
			openToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { stellaLauncherToolStrip, checkHzToolStrip, configFileToolStrip });
			openToolStripMenuItem.ForeColor = Color.DarkGray;
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			// 
			// stellaLauncherToolStrip
			// 
			resources.ApplyResources(stellaLauncherToolStrip, "stellaLauncherToolStrip");
			stellaLauncherToolStrip.Name = "stellaLauncherToolStrip";
			stellaLauncherToolStrip.Click += OpenStella_Click;
			// 
			// checkHzToolStrip
			// 
			resources.ApplyResources(checkHzToolStrip, "checkHzToolStrip");
			checkHzToolStrip.DropDownItems.AddRange(new ToolStripItem[] { systemInformationToolStripMenuItem, dxDiaxToolStripMenuItem });
			checkHzToolStrip.Name = "checkHzToolStrip";
			// 
			// systemInformationToolStripMenuItem
			// 
			resources.ApplyResources(systemInformationToolStripMenuItem, "systemInformationToolStripMenuItem");
			systemInformationToolStripMenuItem.Name = "systemInformationToolStripMenuItem";
			systemInformationToolStripMenuItem.Click += SysInf_Click;
			// 
			// dxDiaxToolStripMenuItem
			// 
			resources.ApplyResources(dxDiaxToolStripMenuItem, "dxDiaxToolStripMenuItem");
			dxDiaxToolStripMenuItem.Name = "dxDiaxToolStripMenuItem";
			dxDiaxToolStripMenuItem.Click += DxDiag_Click;
			// 
			// configFileToolStrip
			// 
			resources.ApplyResources(configFileToolStrip, "configFileToolStrip");
			configFileToolStrip.Name = "configFileToolStrip";
			configFileToolStrip.Click += ViewConfig_Click;
			// 
			// linksToolStripMenuItem
			// 
			resources.ApplyResources(linksToolStripMenuItem, "linksToolStripMenuItem");
			linksToolStripMenuItem.BackColor = Color.Transparent;
			linksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { officialWebsiteToolStripMenuItem, youTubeToolStripMenuItem, gitHubToolStripMenuItem });
			linksToolStripMenuItem.ForeColor = Color.DarkGray;
			linksToolStripMenuItem.Name = "linksToolStripMenuItem";
			// 
			// officialWebsiteToolStripMenuItem
			// 
			resources.ApplyResources(officialWebsiteToolStripMenuItem, "officialWebsiteToolStripMenuItem");
			officialWebsiteToolStripMenuItem.Name = "officialWebsiteToolStripMenuItem";
			officialWebsiteToolStripMenuItem.Click += OfficialWebsite_Click;
			// 
			// youTubeToolStripMenuItem
			// 
			resources.ApplyResources(youTubeToolStripMenuItem, "youTubeToolStripMenuItem");
			youTubeToolStripMenuItem.Name = "youTubeToolStripMenuItem";
			youTubeToolStripMenuItem.Click += YouTube_Click;
			// 
			// gitHubToolStripMenuItem
			// 
			resources.ApplyResources(gitHubToolStripMenuItem, "gitHubToolStripMenuItem");
			gitHubToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { repositoresToolStripMenuItem, sefineksProfileToolStripMenuItem });
			gitHubToolStripMenuItem.Name = "gitHubToolStripMenuItem";
			// 
			// repositoresToolStripMenuItem
			// 
			resources.ApplyResources(repositoresToolStripMenuItem, "repositoresToolStripMenuItem");
			repositoresToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { genshinImpactReShadeToolStripMenuItem, fPSUnlockerToolStripMenuItem });
			repositoresToolStripMenuItem.Name = "repositoresToolStripMenuItem";
			// 
			// genshinImpactReShadeToolStripMenuItem
			// 
			resources.ApplyResources(genshinImpactReShadeToolStripMenuItem, "genshinImpactReShadeToolStripMenuItem");
			genshinImpactReShadeToolStripMenuItem.Name = "genshinImpactReShadeToolStripMenuItem";
			genshinImpactReShadeToolStripMenuItem.Click += GIReShade_Click;
			// 
			// fPSUnlockerToolStripMenuItem
			// 
			resources.ApplyResources(fPSUnlockerToolStripMenuItem, "fPSUnlockerToolStripMenuItem");
			fPSUnlockerToolStripMenuItem.Name = "fPSUnlockerToolStripMenuItem";
			fPSUnlockerToolStripMenuItem.Click += FpsUnlocker_Click;
			// 
			// sefineksProfileToolStripMenuItem
			// 
			resources.ApplyResources(sefineksProfileToolStripMenuItem, "sefineksProfileToolStripMenuItem");
			sefineksProfileToolStripMenuItem.Name = "sefineksProfileToolStripMenuItem";
			sefineksProfileToolStripMenuItem.Click += SefinGitHub_Click;
			// 
			// optionsToolStripMenuItem
			// 
			resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
			optionsToolStripMenuItem.BackColor = Color.Transparent;
			optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { SettingsMenuItem, SetupMenuItem });
			optionsToolStripMenuItem.ForeColor = Color.DarkGray;
			optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			// 
			// SettingsMenuItem
			// 
			resources.ApplyResources(SettingsMenuItem, "SettingsMenuItem");
			SettingsMenuItem.Name = "SettingsMenuItem";
			SettingsMenuItem.Click += SettingsMenuItem_Click;
			// 
			// SetupMenuItem
			// 
			resources.ApplyResources(SetupMenuItem, "SetupMenuItem");
			SetupMenuItem.Name = "SetupMenuItem";
			SetupMenuItem.Click += SetupMenuItem_Click;
			// 
			// aboutToolStrip
			// 
			resources.ApplyResources(aboutToolStrip, "aboutToolStrip");
			aboutToolStrip.ForeColor = Color.DarkGray;
			aboutToolStrip.Name = "aboutToolStrip";
			aboutToolStrip.Click += AboutMenuItem_Click;
			// 
			// LabelFPS
			// 
			resources.ApplyResources(LabelFPS, "LabelFPS");
			LabelFPS.ForeColor = Color.White;
			LabelFPS.Name = "LabelFPS";
			ToolTipMain.SetToolTip(LabelFPS, resources.GetString("LabelFPS.ToolTip"));
			// 
			// InputFPS
			// 
			resources.ApplyResources(InputFPS, "InputFPS");
			InputFPS.BackColor = Color.FromArgb(47, 49, 54);
			InputFPS.ForeColor = Color.White;
			InputFPS.Maximum = new decimal(new int[] { 540, 0, 0, 0 });
			InputFPS.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			InputFPS.Name = "InputFPS";
			ToolTipMain.SetToolTip(InputFPS, resources.GetString("InputFPS.ToolTip"));
			InputFPS.Value = new decimal(new int[] { 144, 0, 0, 0 });
			// 
			// SliderFPS
			// 
			resources.ApplyResources(SliderFPS, "SliderFPS");
			SliderFPS.Maximum = 540;
			SliderFPS.Minimum = 1;
			SliderFPS.Name = "SliderFPS";
			SliderFPS.TickStyle = TickStyle.None;
			ToolTipMain.SetToolTip(SliderFPS, resources.GetString("SliderFPS.ToolTip"));
			SliderFPS.Value = 120;
			// 
			// CBAutoStart
			// 
			resources.ApplyResources(CBAutoStart, "CBAutoStart");
			CBAutoStart.ForeColor = Color.White;
			CBAutoStart.Name = "CBAutoStart";
			ToolTipMain.SetToolTip(CBAutoStart, resources.GetString("CBAutoStart.ToolTip"));
			CBAutoStart.UseVisualStyleBackColor = true;
			// 
			// BtnStartGame
			// 
			resources.ApplyResources(BtnStartGame, "BtnStartGame");
			BtnStartGame.Name = "BtnStartGame";
			ToolTipMain.SetToolTip(BtnStartGame, resources.GetString("BtnStartGame.ToolTip"));
			BtnStartGame.UseVisualStyleBackColor = true;
			BtnStartGame.Click += BtnStartGame_Click;
			// 
			// ContextNotify
			// 
			resources.ApplyResources(ContextNotify, "ContextNotify");
			ContextNotify.Items.AddRange(new ToolStripItem[] { StartGameMenuItem, ExitMenuItem });
			ContextNotify.Name = "ContextNotify";
			ToolTipMain.SetToolTip(ContextNotify, resources.GetString("ContextNotify.ToolTip"));
			// 
			// StartGameMenuItem
			// 
			resources.ApplyResources(StartGameMenuItem, "StartGameMenuItem");
			StartGameMenuItem.Name = "StartGameMenuItem";
			StartGameMenuItem.Click += StartGameMenuItem_Click;
			// 
			// ExitMenuItem
			// 
			resources.ApplyResources(ExitMenuItem, "ExitMenuItem");
			ExitMenuItem.Name = "ExitMenuItem";
			ExitMenuItem.Click += ExitMenuItem_Click;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(pictureBox1, "pictureBox1");
			pictureBox1.Image = ImageResources.fps_unlocker;
			pictureBox1.Name = "pictureBox1";
			pictureBox1.TabStop = false;
			ToolTipMain.SetToolTip(pictureBox1, resources.GetString("pictureBox1.ToolTip"));
			// 
			// NotifyIconMain
			// 
			NotifyIconMain.BalloonTipIcon = ToolTipIcon.None;
			resources.ApplyResources(NotifyIconMain, "NotifyIconMain");
			NotifyIconMain.ContextMenuStrip = ContextNotify;
			NotifyIconMain.DoubleClick += NotifyIconMain_DoubleClick;
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.FromArgb(46, 48, 51);
			Controls.Add(pictureBox1);
			Controls.Add(BtnStartGame);
			Controls.Add(CBAutoStart);
			Controls.Add(SliderFPS);
			Controls.Add(InputFPS);
			Controls.Add(LabelFPS);
			Controls.Add(OptionsMenuStrip);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MainMenuStrip = OptionsMenuStrip;
			MaximizeBox = false;
			Name = "MainForm";
			ToolTipMain.SetToolTip(this, resources.GetString("$this.ToolTip"));
			FormClosing += MainForm_FormClosing;
			Load += MainForm_Load;
			Resize += MainForm_Resize;
			OptionsMenuStrip.ResumeLayout(false);
			OptionsMenuStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)InputFPS).EndInit();
			((System.ComponentModel.ISupportInitialize)SliderFPS).EndInit();
			ContextNotify.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip OptionsMenuStrip;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem SettingsMenuItem;
        private ToolStripMenuItem SetupMenuItem;
        private Label LabelFPS;
        private NumericUpDown InputFPS;
        private TrackBar SliderFPS;
        private CheckBox CBAutoStart;
        private Button BtnStartGame;
        private ToolTip ToolTipMain;
        private NotifyIcon NotifyIconMain;
        private ContextMenuStrip ContextNotify;
        private ToolStripMenuItem ExitMenuItem;
        private ToolStripMenuItem StartGameMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem linksToolStripMenuItem;
        private ToolStripMenuItem officialWebsiteToolStripMenuItem;
        private ToolStripMenuItem youTubeToolStripMenuItem;
        private ToolStripMenuItem gitHubToolStripMenuItem;
        private ToolStripMenuItem repositoresToolStripMenuItem;
        private ToolStripMenuItem genshinImpactReShadeToolStripMenuItem;
        private ToolStripMenuItem fPSUnlockerToolStripMenuItem;
        private ToolStripMenuItem sefineksProfileToolStripMenuItem;
        private ToolStripMenuItem stellaLauncherToolStrip;
        private ToolStripMenuItem checkHzToolStrip;
        private ToolStripMenuItem systemInformationToolStripMenuItem;
        private ToolStripMenuItem dxDiaxToolStripMenuItem;
		  private PictureBox pictureBox1;
		private ToolStripMenuItem aboutToolStrip;
		private ToolStripMenuItem configFileToolStrip;
	}
}
