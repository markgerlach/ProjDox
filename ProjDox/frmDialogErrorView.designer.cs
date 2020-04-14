namespace ProjDox
{
	partial class frmDialogErrorView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDialogErrorView));
            this.layoutMain = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.ilErrors = new System.Windows.Forms.ImageList(this.components);
			this.ucErrorView = new ucErrorView();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
            this.layoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutMain
            // 
            this.layoutMain.Appearance.DisabledLayoutGroupCaption.ForeColor = System.Drawing.SystemColors.GrayText;
            this.layoutMain.Appearance.DisabledLayoutGroupCaption.Options.UseForeColor = true;
            this.layoutMain.Appearance.DisabledLayoutItem.ForeColor = System.Drawing.SystemColors.GrayText;
            this.layoutMain.Appearance.DisabledLayoutItem.Options.UseForeColor = true;
            this.layoutMain.Controls.Add(this.ucErrorView);
            this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutMain.Location = new System.Drawing.Point(0, 0);
            this.layoutMain.Name = "layoutMain";
            this.layoutMain.Root = this.layoutControlGroup1;
            this.layoutMain.Size = new System.Drawing.Size(534, 539);
            this.layoutMain.TabIndex = 0;
            this.layoutMain.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(534, 539);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // ilErrors
            // 
            this.ilErrors.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilErrors.ImageStream")));
            this.ilErrors.TransparentColor = System.Drawing.Color.Transparent;
            this.ilErrors.Images.SetKeyName(0, "");
            this.ilErrors.Images.SetKeyName(1, "");
            this.ilErrors.Images.SetKeyName(2, "");
            this.ilErrors.Images.SetKeyName(3, "");
            this.ilErrors.Images.SetKeyName(4, "");
            this.ilErrors.Images.SetKeyName(5, "");
            this.ilErrors.Images.SetKeyName(6, "");
            this.ilErrors.Images.SetKeyName(7, "");
            this.ilErrors.Images.SetKeyName(8, "");
            this.ilErrors.Images.SetKeyName(9, "");
            this.ilErrors.Images.SetKeyName(10, "");
            this.ilErrors.Images.SetKeyName(11, "");
            this.ilErrors.Images.SetKeyName(12, "");
            this.ilErrors.Images.SetKeyName(13, "");
            this.ilErrors.Images.SetKeyName(14, "");
            this.ilErrors.Images.SetKeyName(15, "");
            this.ilErrors.Images.SetKeyName(16, "");
            // 
            // ucErrorView
            // 
            this.ucErrorView.BrokenRuleImage = null;
            this.ucErrorView.CancelableForm = true;
			this.ucErrorView.CancelButtonColor = mgModel.mgButtonGUIButtonColor.Blue;
            this.ucErrorView.CancelButtonText = "Cancel Processing";
            this.ucErrorView.ContinuableForm = false;
			this.ucErrorView.ContinueButtonColor = mgModel.mgButtonGUIButtonColor.Green;
            this.ucErrorView.ContinueButtonText = "Continue Processing";
            this.ucErrorView.EnablePrintButton = true;
            this.ucErrorView.Location = new System.Drawing.Point(7, 7);
            this.ucErrorView.Name = "ucErrorView";
			this.ucErrorView.PrintButtonColor = mgModel.mgButtonGUIButtonColor.Default;
            this.ucErrorView.PrintButtonText = "Print Errors";
            this.ucErrorView.ProcessDescription = "";
            this.ucErrorView.Size = new System.Drawing.Size(521, 526);
            this.ucErrorView.TabIndex = 4;
            this.ucErrorView.UserDef1Image = null;
            this.ucErrorView.UserDef2Image = null;
            this.ucErrorView.UserDef3Image = null;
            this.ucErrorView.CancelProcessing += new ucErrorView.CancelProcessingEventHandler(this.ucErrorView_CancelProcessing);
			this.ucErrorView.ControlClosed += new ucErrorView.ControlClosedEventHandler(this.ucErrorView_ControlClosed);
			this.ucErrorView.ContinueProcessing += new ucErrorView.ContinueProcessingEventHandler(this.ucErrorView_ContinueProcessing);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.ucErrorView;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(532, 537);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmDialogErrorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 539);
            this.Controls.Add(this.layoutMain);
            this.KeyPreview = true;
            this.Name = "frmDialogErrorView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmDialogErrorView";
            this.Load += new System.EventHandler(this.frmDialogErrorView_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmDialogErrorView_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
            this.layoutMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraLayout.LayoutControl layoutMain;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private System.Windows.Forms.ImageList ilErrors;
		private ucErrorView ucErrorView;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
	}
}