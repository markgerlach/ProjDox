namespace ProjDox
{
	partial class ucErrorView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucErrorView));
			this.layoutMain = new mgControls.mgLayoutControl();
			this.linkField = new DevExpress.XtraEditors.HyperLinkEdit();
			this.btnContinueProcessing = new mgControls.mgDevX_SimpleButton();
			this.btnCancelProcessing = new mgControls.mgDevX_SimpleButton();
			this.btnPrint = new mgControls.mgDevX_SimpleButton();
			this.btnWarnings = new mgControls.mgDevX_SimpleButton();
			this.btnInformation = new mgControls.mgDevX_SimpleButton();
			this.btnErrors = new mgControls.mgDevX_SimpleButton();
			this.gridError = new mgControls.mgDevX_GridControl();
			this.gridViewError = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutCancelProcessing = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutContinueProcessing = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptyRight = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptyLeft = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutPrintErrors = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutSupportHyperlink = new DevExpress.XtraLayout.LayoutControlItem();
			this.ilErrors = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
			this.layoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.linkField.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridError)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewError)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutCancelProcessing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutContinueProcessing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptyRight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptyLeft)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutPrintErrors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutSupportHyperlink)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutMain
			// 
			this.layoutMain.AllowCustomizationMenu = false;
			this.layoutMain.BackColor = System.Drawing.Color.White;
			this.layoutMain.Controls.Add(this.linkField);
			this.layoutMain.Controls.Add(this.btnContinueProcessing);
			this.layoutMain.Controls.Add(this.btnCancelProcessing);
			this.layoutMain.Controls.Add(this.btnPrint);
			this.layoutMain.Controls.Add(this.btnWarnings);
			this.layoutMain.Controls.Add(this.btnInformation);
			this.layoutMain.Controls.Add(this.btnErrors);
			this.layoutMain.Controls.Add(this.gridError);
			this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutMain.Location = new System.Drawing.Point(0, 0);
			this.layoutMain.Name = "layoutMain";
			this.layoutMain.Root = this.layoutControlGroup1;
			this.layoutMain.Size = new System.Drawing.Size(507, 480);
			this.layoutMain.TabIndex = 0;
			this.layoutMain.Text = "mgLayoutControl1";
			// 
			// linkField
			// 
			this.linkField.Location = new System.Drawing.Point(72, 419);
			this.linkField.Name = "linkField";
			this.linkField.Size = new System.Drawing.Size(430, 20);
			this.linkField.StyleController = this.layoutMain;
			this.linkField.TabIndex = 11;
			this.linkField.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(this.linkField_OpenLink);
			// 
			// btnContinueProcessing
			// 
			this.btnContinueProcessing.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.btnContinueProcessing.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(0)))));
			this.btnContinueProcessing.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnContinueProcessing.Appearance.ForeColor = System.Drawing.Color.White;
			this.btnContinueProcessing.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.btnContinueProcessing.Appearance.Options.UseBackColor = true;
			this.btnContinueProcessing.Appearance.Options.UseFont = true;
			this.btnContinueProcessing.Appearance.Options.UseForeColor = true;
			this.btnContinueProcessing.ButtonColor = mgModel.mgButtonGUIButtonColor.Green;
			this.btnContinueProcessing.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnContinueProcessing.Location = new System.Drawing.Point(167, 449);
			this.btnContinueProcessing.Name = "btnContinueProcessing";
			this.btnContinueProcessing.Size = new System.Drawing.Size(159, 26);
			this.btnContinueProcessing.TabIndex = 10;
			this.btnContinueProcessing.Text = "Continue Processing";
			this.btnContinueProcessing.Click += new System.EventHandler(this.btnContinueProcessing_Click);
			// 
			// btnCancelProcessing
			// 
			this.btnCancelProcessing.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.btnCancelProcessing.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.btnCancelProcessing.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnCancelProcessing.Appearance.ForeColor = System.Drawing.Color.White;
			this.btnCancelProcessing.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.btnCancelProcessing.Appearance.Options.UseBackColor = true;
			this.btnCancelProcessing.Appearance.Options.UseFont = true;
			this.btnCancelProcessing.Appearance.Options.UseForeColor = true;
			this.btnCancelProcessing.ButtonColor = mgModel.mgButtonGUIButtonColor.Red;
			this.btnCancelProcessing.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnCancelProcessing.Location = new System.Drawing.Point(382, 449);
			this.btnCancelProcessing.Name = "btnCancelProcessing";
			this.btnCancelProcessing.Size = new System.Drawing.Size(120, 26);
			this.btnCancelProcessing.TabIndex = 9;
			this.btnCancelProcessing.Text = "Cancel Processing";
			this.btnCancelProcessing.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnPrint
			// 
			this.btnPrint.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnPrint.Appearance.Options.UseFont = true;
			this.btnPrint.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnPrint.Location = new System.Drawing.Point(5, 449);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(103, 26);
			this.btnPrint.TabIndex = 8;
			this.btnPrint.Text = "Print Errors";
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// btnWarnings
			// 
			this.btnWarnings.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.btnWarnings.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(0)))));
			this.btnWarnings.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnWarnings.Appearance.ForeColor = System.Drawing.Color.White;
			this.btnWarnings.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.btnWarnings.Appearance.Options.UseBackColor = true;
			this.btnWarnings.Appearance.Options.UseFont = true;
			this.btnWarnings.Appearance.Options.UseForeColor = true;
			this.btnWarnings.ButtonColor = mgModel.mgButtonGUIButtonColor.Green;
			this.btnWarnings.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnWarnings.Location = new System.Drawing.Point(122, 5);
			this.btnWarnings.Name = "btnWarnings";
			this.btnWarnings.Size = new System.Drawing.Size(127, 20);
			this.btnWarnings.TabIndex = 7;
			this.btnWarnings.Text = "Warnings (500)";
			this.btnWarnings.Click += new System.EventHandler(this.btnWarnings_Click);
			// 
			// btnInformation
			// 
			this.btnInformation.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.btnInformation.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(0)))));
			this.btnInformation.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnInformation.Appearance.ForeColor = System.Drawing.Color.White;
			this.btnInformation.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.btnInformation.Appearance.Options.UseBackColor = true;
			this.btnInformation.Appearance.Options.UseFont = true;
			this.btnInformation.Appearance.Options.UseForeColor = true;
			this.btnInformation.ButtonColor = mgModel.mgButtonGUIButtonColor.Green;
			this.btnInformation.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnInformation.Location = new System.Drawing.Point(259, 5);
			this.btnInformation.Name = "btnInformation";
			this.btnInformation.Size = new System.Drawing.Size(131, 20);
			this.btnInformation.TabIndex = 6;
			this.btnInformation.Text = "Information (500)";
			this.btnInformation.Click += new System.EventHandler(this.btnInformation_Click);
			// 
			// btnErrors
			// 
			this.btnErrors.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.btnErrors.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(0)))));
			this.btnErrors.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnErrors.Appearance.ForeColor = System.Drawing.Color.White;
			this.btnErrors.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.btnErrors.Appearance.Options.UseBackColor = true;
			this.btnErrors.Appearance.Options.UseFont = true;
			this.btnErrors.Appearance.Options.UseForeColor = true;
			this.btnErrors.ButtonColor = mgModel.mgButtonGUIButtonColor.Green;
			this.btnErrors.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnErrors.Location = new System.Drawing.Point(5, 5);
			this.btnErrors.Name = "btnErrors";
			this.btnErrors.Size = new System.Drawing.Size(107, 20);
			this.btnErrors.TabIndex = 5;
			this.btnErrors.Text = "Errors (200)";
			this.btnErrors.Click += new System.EventHandler(this.btnErrors_Click);
			// 
			// gridError
			// 
			this.gridError.CheckUncheckByClickingColumnHeader = true;
			this.gridError.Location = new System.Drawing.Point(5, 35);
			this.gridError.MainView = this.gridViewError;
			this.gridError.Name = "gridError";
			this.gridError.ShowDetailButtons = false;
			this.gridError.ShowOnlyPredefinedDetails = true;
			this.gridError.Size = new System.Drawing.Size(497, 374);
			this.gridError.TabIndex = 4;
			this.gridError.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewError});
			// 
			// gridViewError
			// 
			this.gridViewError.Appearance.Row.Options.UseTextOptions = true;
			this.gridViewError.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.gridViewError.GridControl = this.gridError;
			this.gridViewError.Name = "gridViewError";
			this.gridViewError.OptionsCustomization.AllowGroup = false;
			this.gridViewError.OptionsDetail.ShowDetailTabs = false;
			this.gridViewError.OptionsView.RowAutoHeight = true;
			this.gridViewError.OptionsView.ShowDetailButtons = false;
			this.gridViewError.OptionsView.ShowGroupPanel = false;
			this.gridViewError.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridViewError_CustomDrawCell);
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.AppearanceGroup.BackColor = System.Drawing.Color.White;
			this.layoutControlGroup1.AppearanceGroup.Options.UseBackColor = true;
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutCancelProcessing,
            this.layoutContinueProcessing,
            this.emptyRight,
            this.emptyLeft,
            this.layoutPrintErrors,
            this.layoutSupportHyperlink});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(507, 480);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.Control = this.gridError;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 30);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(507, 384);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(395, 0);
			this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 30);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 30);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(112, 30);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.btnErrors;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.MaxSize = new System.Drawing.Size(117, 30);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(117, 30);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(117, 30);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.btnInformation;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.Location = new System.Drawing.Point(254, 0);
			this.layoutControlItem3.MaxSize = new System.Drawing.Size(141, 30);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(141, 30);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(141, 30);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			// 
			// layoutControlItem4
			// 
			this.layoutControlItem4.Control = this.btnWarnings;
			this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem4.Location = new System.Drawing.Point(117, 0);
			this.layoutControlItem4.MaxSize = new System.Drawing.Size(137, 30);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(137, 30);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(137, 30);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.Text = "layoutControlItem4";
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			// 
			// layoutCancelProcessing
			// 
			this.layoutCancelProcessing.Control = this.btnCancelProcessing;
			this.layoutCancelProcessing.CustomizationFormText = "layoutControlItem6";
			this.layoutCancelProcessing.Location = new System.Drawing.Point(377, 444);
			this.layoutCancelProcessing.MaxSize = new System.Drawing.Size(130, 36);
			this.layoutCancelProcessing.MinSize = new System.Drawing.Size(130, 36);
			this.layoutCancelProcessing.Name = "layoutCancelProcessing";
			this.layoutCancelProcessing.Size = new System.Drawing.Size(130, 36);
			this.layoutCancelProcessing.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutCancelProcessing.Text = "layoutCancelProcessing";
			this.layoutCancelProcessing.TextSize = new System.Drawing.Size(0, 0);
			this.layoutCancelProcessing.TextToControlDistance = 0;
			this.layoutCancelProcessing.TextVisible = false;
			// 
			// layoutContinueProcessing
			// 
			this.layoutContinueProcessing.Control = this.btnContinueProcessing;
			this.layoutContinueProcessing.CustomizationFormText = "layoutControlItem7";
			this.layoutContinueProcessing.Location = new System.Drawing.Point(162, 444);
			this.layoutContinueProcessing.MaxSize = new System.Drawing.Size(169, 36);
			this.layoutContinueProcessing.MinSize = new System.Drawing.Size(169, 36);
			this.layoutContinueProcessing.Name = "layoutContinueProcessing";
			this.layoutContinueProcessing.Size = new System.Drawing.Size(169, 36);
			this.layoutContinueProcessing.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutContinueProcessing.Text = "layoutContinueProcessing";
			this.layoutContinueProcessing.TextSize = new System.Drawing.Size(0, 0);
			this.layoutContinueProcessing.TextToControlDistance = 0;
			this.layoutContinueProcessing.TextVisible = false;
			// 
			// emptyRight
			// 
			this.emptyRight.AllowHotTrack = false;
			this.emptyRight.CustomizationFormText = "emptySpaceItem2";
			this.emptyRight.Location = new System.Drawing.Point(331, 444);
			this.emptyRight.MaxSize = new System.Drawing.Size(0, 36);
			this.emptyRight.MinSize = new System.Drawing.Size(10, 36);
			this.emptyRight.Name = "emptyRight";
			this.emptyRight.Size = new System.Drawing.Size(46, 36);
			this.emptyRight.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptyRight.Text = "emptyRight";
			this.emptyRight.TextSize = new System.Drawing.Size(0, 0);
			// 
			// emptyLeft
			// 
			this.emptyLeft.AllowHotTrack = false;
			this.emptyLeft.CustomizationFormText = "emptySpaceItem3";
			this.emptyLeft.Location = new System.Drawing.Point(113, 444);
			this.emptyLeft.MaxSize = new System.Drawing.Size(0, 36);
			this.emptyLeft.MinSize = new System.Drawing.Size(10, 36);
			this.emptyLeft.Name = "emptyLeft";
			this.emptyLeft.Size = new System.Drawing.Size(49, 36);
			this.emptyLeft.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptyLeft.Text = "emptyLeft";
			this.emptyLeft.TextSize = new System.Drawing.Size(0, 0);
			// 
			// layoutPrintErrors
			// 
			this.layoutPrintErrors.Control = this.btnPrint;
			this.layoutPrintErrors.CustomizationFormText = "layoutControlItem5";
			this.layoutPrintErrors.Location = new System.Drawing.Point(0, 444);
			this.layoutPrintErrors.MaxSize = new System.Drawing.Size(113, 36);
			this.layoutPrintErrors.MinSize = new System.Drawing.Size(113, 36);
			this.layoutPrintErrors.Name = "layoutPrintErrors";
			this.layoutPrintErrors.Size = new System.Drawing.Size(113, 36);
			this.layoutPrintErrors.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutPrintErrors.Text = "layoutPrintErrors";
			this.layoutPrintErrors.TextSize = new System.Drawing.Size(0, 0);
			this.layoutPrintErrors.TextToControlDistance = 0;
			this.layoutPrintErrors.TextVisible = false;
			// 
			// layoutSupportHyperlink
			// 
			this.layoutSupportHyperlink.Control = this.linkField;
			this.layoutSupportHyperlink.CustomizationFormText = "Support Link:";
			this.layoutSupportHyperlink.Location = new System.Drawing.Point(0, 414);
			this.layoutSupportHyperlink.Name = "layoutSupportHyperlink";
			this.layoutSupportHyperlink.Size = new System.Drawing.Size(507, 30);
			this.layoutSupportHyperlink.Text = "Support Link:";
			this.layoutSupportHyperlink.TextSize = new System.Drawing.Size(63, 13);
			this.layoutSupportHyperlink.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			// 
			// ilErrors
			// 
			this.ilErrors.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilErrors.ImageStream")));
			this.ilErrors.TransparentColor = System.Drawing.Color.Transparent;
			this.ilErrors.Images.SetKeyName(0, "error.png");
			this.ilErrors.Images.SetKeyName(1, "warning.png");
			this.ilErrors.Images.SetKeyName(2, "information.png");
			this.ilErrors.Images.SetKeyName(3, "Mail_Broken_16.png");
			// 
			// ucErrorView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.layoutMain);
			this.Name = "ucErrorView";
			this.Size = new System.Drawing.Size(507, 480);
			this.Load += new System.EventHandler(this.ucErrorView_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
			this.layoutMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.linkField.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridError)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewError)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutCancelProcessing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutContinueProcessing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptyRight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptyLeft)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutPrintErrors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutSupportHyperlink)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private mgControls.mgLayoutControl layoutMain;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private mgControls.mgDevX_SimpleButton btnErrors;
		private mgControls.mgDevX_GridControl gridError;
		private DevExpress.XtraGrid.Views.Grid.GridView gridViewError;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private mgControls.mgDevX_SimpleButton btnWarnings;
		private mgControls.mgDevX_SimpleButton btnInformation;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private mgControls.mgDevX_SimpleButton btnCancelProcessing;
		private mgControls.mgDevX_SimpleButton btnPrint;
		private DevExpress.XtraLayout.LayoutControlItem layoutPrintErrors;
		private DevExpress.XtraLayout.LayoutControlItem layoutCancelProcessing;
		private System.Windows.Forms.ImageList ilErrors;
		private mgControls.mgDevX_SimpleButton btnContinueProcessing;
		private DevExpress.XtraLayout.LayoutControlItem layoutContinueProcessing;
		private DevExpress.XtraLayout.EmptySpaceItem emptyRight;
		private DevExpress.XtraLayout.EmptySpaceItem emptyLeft;
		private DevExpress.XtraEditors.HyperLinkEdit linkField;
		private DevExpress.XtraLayout.LayoutControlItem layoutSupportHyperlink;
	}
}
