using System.Diagnostics;

namespace unlockfps_nc.Forms;

internal partial class AboutForm : Form
{
	internal AboutForm()
	{
		InitializeComponent();
	}

	private void AboutForm_Load(object sender, EventArgs e)
	{
		LabelTitle.Text = string.Format(LabelTitle.Text, Application.ProductVersion);
	}

	private void GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		OpenLink("https://github.com/Genshin-Stella-Mod/Genshin-FPS-Unlocker?referrer=GitHub_LinkClicked");
	}

	private void Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		OpenLink("https://stella.sefinek.net/?referrer=OfficialWebsite_LinkClicked");
	}

	internal static void OpenLink(string url)
	{
		ProcessStartInfo psi = new()
		{
			FileName = url,
			UseShellExecute = true
		};

		Process.Start(psi);
	}
}
