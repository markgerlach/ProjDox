namespace mgControls
{
	partial class mgDevX_FilterControl
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
			this.layoutMain = new mgLayoutControl();
			this.lblExistingFilter = new DevExpress.XtraEditors.LabelControl();
			this.filterControl1 = new DevExpress.XtraEditors.FilterControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutExistingFilter = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
			this.layoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutExistingFilter)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutMain
			// 
			//this.layoutMain.AllowCustomizationMenu = false;
			this.layoutMain.Controls.Add(this.lblExistingFilter);
			this.layoutMain.Controls.Add(this.filterControl1);
			this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutMain.Location = new System.Drawing.Point(0, 0);
			this.layoutMain.Margin = new System.Windows.Forms.Padding(0);
			this.layoutMain.Name = "layoutMain";
			this.layoutMain.Root = this.layoutControlGroup1;
			this.layoutMain.Size = new System.Drawing.Size(395, 301);
			this.layoutMain.TabIndex = 1;
			this.layoutMain.Text = "mgLayoutControl1";
			// 
			// lblExistingFilter
			// 
			this.lblExistingFilter.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.lblExistingFilter.Appearance.Options.UseFont = true;
			this.lblExistingFilter.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblExistingFilter.Location = new System.Drawing.Point(6, 6);
			this.lblExistingFilter.Name = "lblExistingFilter";
			this.lblExistingFilter.Size = new System.Drawing.Size(384, 13);
			this.lblExistingFilter.StyleController = this.layoutMain;
			this.lblExistingFilter.TabIndex = 4;
			this.lblExistingFilter.Text = "Existing Filter:";
			// 
			// filterControl1
			// 
			this.filterControl1.Location = new System.Drawing.Point(6, 30);
			this.filterControl1.Name = "filterControl1";
			this.filterControl1.Size = new System.Drawing.Size(384, 266);
			this.filterControl1.TabIndex = 0;
			this.filterControl1.Text = "filterControl1";
			this.filterControl1.MouseLeave += new System.EventHandler(this.filterControl1_MouseLeave);
			this.filterControl1.Click += new System.EventHandler(this.filterControl1_Click);
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutExistingFilter});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(395, 301);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.Control = this.filterControl1;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(395, 277);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			// 
			// layoutExistingFilter
			// 
			this.layoutExistingFilter.Control = this.lblExistingFilter;
			this.layoutExistingFilter.CustomizationFormText = "Existing Filter:";
			this.layoutExistingFilter.Location = new System.Drawing.Point(0, 0);
			this.layoutExistingFilter.MaxSize = new System.Drawing.Size(0, 24);
			this.layoutExistingFilter.MinSize = new System.Drawing.Size(21, 24);
			this.layoutExistingFilter.Name = "layoutExistingFilter";
			this.layoutExistingFilter.Size = new System.Drawing.Size(395, 24);
			this.layoutExistingFilter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutExistingFilter.Text = "Existing Filter:";
			this.layoutExistingFilter.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutExistingFilter.TextSize = new System.Drawing.Size(0, 0);
			this.layoutExistingFilter.TextToControlDistance = 0;
			this.layoutExistingFilter.TextVisible = false;
			this.layoutExistingFilter.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			// 
			// mgDevX_FilterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutMain);
			this.Name = "mgDevX_FilterControl";
			this.Size = new System.Drawing.Size(395, 301);
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
			this.layoutMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutExistingFilter)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraEditors.FilterControl filterControl1;
		private mgLayoutControl layoutMain;
		private DevExpress.XtraEditors.LabelControl lblExistingFilter;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutExistingFilter;
	}
}
