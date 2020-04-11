namespace mgControls
{
	partial class mgDevX_Label_Hyperlink
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.lblHyperlink2 = new DevExpress.XtraEditors.LabelControl();
			this.lblHyperlink1 = new DevExpress.XtraEditors.LabelControl();
			this.lblCount = new DevExpress.XtraEditors.LabelControl();
			this.lblHyperlink3 = new DevExpress.XtraEditors.LabelControl();
			this.ttController = new DevExpress.Utils.ToolTipController(this.components);
			this.SuspendLayout();
			// 
			// lblHyperlink2
			// 
			this.lblHyperlink2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
			this.lblHyperlink2.Appearance.ForeColor = System.Drawing.Color.Blue;
			this.lblHyperlink2.Appearance.Options.UseFont = true;
			this.lblHyperlink2.Appearance.Options.UseForeColor = true;
			this.lblHyperlink2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.lblHyperlink2.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblHyperlink2.Location = new System.Drawing.Point(276, 3);
			this.lblHyperlink2.Name = "lblHyperlink2";
			this.lblHyperlink2.Size = new System.Drawing.Size(63, 13);
			this.lblHyperlink2.TabIndex = 6;
			this.lblHyperlink2.Text = "labelControl3";
			this.lblHyperlink2.ToolTipController = this.ttController;
			this.lblHyperlink2.Click += new System.EventHandler(this.lblHyperlink2_Click);
			// 
			// lblHyperlink1
			// 
			this.lblHyperlink1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
			this.lblHyperlink1.Appearance.ForeColor = System.Drawing.Color.Blue;
			this.lblHyperlink1.Appearance.Options.UseFont = true;
			this.lblHyperlink1.Appearance.Options.UseForeColor = true;
			this.lblHyperlink1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.lblHyperlink1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblHyperlink1.Location = new System.Drawing.Point(174, 3);
			this.lblHyperlink1.Name = "lblHyperlink1";
			this.lblHyperlink1.Size = new System.Drawing.Size(63, 13);
			this.lblHyperlink1.TabIndex = 5;
			this.lblHyperlink1.Text = "labelControl2";
			this.lblHyperlink1.ToolTipController = this.ttController;
			this.lblHyperlink1.Click += new System.EventHandler(this.lblHyperlink1_Click);
			// 
			// lblCount
			// 
			this.lblCount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.lblCount.Location = new System.Drawing.Point(3, 3);
			this.lblCount.Name = "lblCount";
			this.lblCount.Size = new System.Drawing.Size(75, 13);
			this.lblCount.TabIndex = 4;
			this.lblCount.Text = "239 Ch53 Open";
			this.lblCount.ToolTipController = this.ttController;
			// 
			// lblHyperlink3
			// 
			this.lblHyperlink3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
			this.lblHyperlink3.Appearance.ForeColor = System.Drawing.Color.Blue;
			this.lblHyperlink3.Appearance.Options.UseFont = true;
			this.lblHyperlink3.Appearance.Options.UseForeColor = true;
			this.lblHyperlink3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.lblHyperlink3.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblHyperlink3.Location = new System.Drawing.Point(368, 8);
			this.lblHyperlink3.Name = "lblHyperlink3";
			this.lblHyperlink3.Size = new System.Drawing.Size(63, 13);
			this.lblHyperlink3.TabIndex = 7;
			this.lblHyperlink3.Text = "labelControl4";
			this.lblHyperlink3.ToolTipController = this.ttController;
			this.lblHyperlink3.Click += new System.EventHandler(this.lblHyperlink3_Click);
			// 
			// ttController
			// 
			this.ttController.AllowHtmlText = true;
			this.ttController.AutoPopDelay = 8000;
			this.ttController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.ttController_GetActiveObjectInfo);
			this.ttController.CustomDraw += new DevExpress.Utils.ToolTipControllerCustomDrawEventHandler(this.ttController_CustomDraw);
			this.ttController.BeforeShow += new DevExpress.Utils.ToolTipControllerBeforeShowEventHandler(this.ttController_BeforeShow);
			// 
			// mgDevX_Label_Hyperlink
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblHyperlink3);
			this.Controls.Add(this.lblCount);
			this.Controls.Add(this.lblHyperlink1);
			this.Controls.Add(this.lblHyperlink2);
			this.Name = "mgDevX_Label_Hyperlink";
			this.Size = new System.Drawing.Size(558, 124);
			this.EnabledChanged += new System.EventHandler(this.mgDevX_Label_GridCount_Hyperlink_EnabledChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DevExpress.XtraEditors.LabelControl lblHyperlink2;
		private DevExpress.XtraEditors.LabelControl lblHyperlink1;
		private DevExpress.XtraEditors.LabelControl lblCount;
		private DevExpress.XtraEditors.LabelControl lblHyperlink3;
		private DevExpress.Utils.ToolTipController ttController;
	}
}
