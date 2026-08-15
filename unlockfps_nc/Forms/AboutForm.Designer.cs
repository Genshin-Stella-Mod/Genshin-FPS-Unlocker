namespace unlockfps_nc.Forms
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			LabelTitle = new Label();
			LabelDescription = new Label();
			linkLabel1 = new LinkLabel();
			linkLabel2 = new LinkLabel();
			label1 = new Label();
			SuspendLayout();
			// 
			// LabelTitle
			// 
			resources.ApplyResources(LabelTitle, "LabelTitle");
			LabelTitle.ForeColor = Color.White;
			LabelTitle.Name = "LabelTitle";
			// 
			// LabelDescription
			// 
			resources.ApplyResources(LabelDescription, "LabelDescription");
			LabelDescription.ForeColor = Color.White;
			LabelDescription.Name = "LabelDescription";
			// 
			// linkLabel1
			// 
			linkLabel1.ActiveLinkColor = Color.LightSkyBlue;
			resources.ApplyResources(linkLabel1, "linkLabel1");
			linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
			linkLabel1.LinkColor = Color.DodgerBlue;
			linkLabel1.Name = "linkLabel1";
			linkLabel1.TabStop = true;
			linkLabel1.LinkClicked += GitHub_LinkClicked;
			// 
			// linkLabel2
			// 
			linkLabel2.ActiveLinkColor = Color.LightSkyBlue;
			resources.ApplyResources(linkLabel2, "linkLabel2");
			linkLabel2.LinkBehavior = LinkBehavior.HoverUnderline;
			linkLabel2.LinkColor = Color.DodgerBlue;
			linkLabel2.Name = "linkLabel2";
			linkLabel2.TabStop = true;
			linkLabel2.LinkClicked += Website_LinkClicked;
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.ForeColor = Color.White;
			label1.Name = "label1";
			// 
			// AboutForm
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.FromArgb(19, 19, 19);
			Controls.Add(label1);
			Controls.Add(linkLabel2);
			Controls.Add(linkLabel1);
			Controls.Add(LabelDescription);
			Controls.Add(LabelTitle);
			DoubleBuffered = true;
			ForeColor = Color.White;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutForm";
			Load += AboutForm_Load;
			ResumeLayout(false);
		}

		#endregion

		private Label LabelTitle;
        private Label LabelDescription;
		private LinkLabel linkLabel1;
		private LinkLabel linkLabel2;
		private Label label1;
	}
}
