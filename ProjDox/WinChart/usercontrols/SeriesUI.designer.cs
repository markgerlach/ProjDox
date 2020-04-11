namespace mgWinChart.usercontrols
{
    partial class SeriesUI
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
			this.cboSeriesSource = new System.Windows.Forms.ComboBox();
			this.cboSeriesGroup = new System.Windows.Forms.ComboBox();
			this.cboPosition = new System.Windows.Forms.ComboBox();
			this.chkLabelsVisible = new System.Windows.Forms.CheckBox();
			this.cboBarShape = new System.Windows.Forms.ComboBox();
			this.nudExplodedDist = new System.Windows.Forms.NumericUpDown();
			this.nudHoleRadius = new System.Windows.Forms.NumericUpDown();
			this.layoutMain = new mgControls.mgLayoutControlCommon();
			this.btnExplosion = new mgControls.mgDevX_SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.nudExplodedDist)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudHoleRadius)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
			this.layoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			// 
			// cboSeriesSource
			// 
			this.cboSeriesSource.FormattingEnabled = true;
			this.cboSeriesSource.Location = new System.Drawing.Point(90, 5);
			this.cboSeriesSource.Name = "cboSeriesSource";
			this.cboSeriesSource.Size = new System.Drawing.Size(138, 21);
			this.cboSeriesSource.TabIndex = 0;
			this.cboSeriesSource.SelectedIndexChanged += new System.EventHandler(this.cboSeriesSource_SelectedIndexChanged);
			// 
			// cboSeriesGroup
			// 
			this.cboSeriesGroup.FormattingEnabled = true;
			this.cboSeriesGroup.Location = new System.Drawing.Point(90, 36);
			this.cboSeriesGroup.Name = "cboSeriesGroup";
			this.cboSeriesGroup.Size = new System.Drawing.Size(138, 21);
			this.cboSeriesGroup.TabIndex = 3;
			this.cboSeriesGroup.Text = "Group 1";
			this.cboSeriesGroup.SelectedIndexChanged += new System.EventHandler(this.cboSeriesGroup_SelectedIndexChanged);
			// 
			// cboPosition
			// 
			this.cboPosition.FormattingEnabled = true;
			this.cboPosition.Location = new System.Drawing.Point(361, 28);
			this.cboPosition.Name = "cboPosition";
			this.cboPosition.Size = new System.Drawing.Size(200, 21);
			this.cboPosition.TabIndex = 1;
			this.cboPosition.Text = "default";
			this.cboPosition.SelectedIndexChanged += new System.EventHandler(this.cboPosition_SelectedIndexChanged);
			// 
			// chkLabelsVisible
			// 
			this.chkLabelsVisible.Checked = true;
			this.chkLabelsVisible.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLabelsVisible.Location = new System.Drawing.Point(241, 28);
			this.chkLabelsVisible.Name = "chkLabelsVisible";
			this.chkLabelsVisible.Size = new System.Drawing.Size(64, 21);
			this.chkLabelsVisible.TabIndex = 0;
			this.chkLabelsVisible.Text = "Visible";
			this.chkLabelsVisible.UseVisualStyleBackColor = true;
			this.chkLabelsVisible.CheckedChanged += new System.EventHandler(this.chkLabelsVisible_CheckedChanged);
			// 
			// cboBarShape
			// 
			this.cboBarShape.FormattingEnabled = true;
			this.cboBarShape.Items.AddRange(new object[] {
            "default",
            "Box",
            "Cylinder",
            "Pyramid",
            "Cone"});
			this.cboBarShape.Location = new System.Drawing.Point(90, 67);
			this.cboBarShape.Name = "cboBarShape";
			this.cboBarShape.Size = new System.Drawing.Size(138, 21);
			this.cboBarShape.TabIndex = 6;
			this.cboBarShape.Text = "default";
			this.cboBarShape.SelectedIndexChanged += new System.EventHandler(this.cboBarShape_SelectedIndexChanged);
			// 
			// nudExplodedDist
			// 
			this.nudExplodedDist.Location = new System.Drawing.Point(509, 95);
			this.nudExplodedDist.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudExplodedDist.Name = "nudExplodedDist";
			this.nudExplodedDist.Size = new System.Drawing.Size(52, 20);
			this.nudExplodedDist.TabIndex = 2;
			this.nudExplodedDist.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudExplodedDist.ValueChanged += new System.EventHandler(this.nudExplodedDist_ValueChanged);
			// 
			// nudHoleRadius
			// 
			this.nudHoleRadius.Location = new System.Drawing.Point(326, 95);
			this.nudHoleRadius.Name = "nudHoleRadius";
			this.nudHoleRadius.Size = new System.Drawing.Size(20, 20);
			this.nudHoleRadius.TabIndex = 0;
			this.nudHoleRadius.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.nudHoleRadius.ValueChanged += new System.EventHandler(this.nudHoleRadius_ValueChanged);
			// 
			// layoutMain
			// 
			this.layoutMain.Controls.Add(this.btnExplosion);
			this.layoutMain.Controls.Add(this.nudExplodedDist);
			this.layoutMain.Controls.Add(this.cboSeriesSource);
			this.layoutMain.Controls.Add(this.cboPosition);
			this.layoutMain.Controls.Add(this.cboSeriesGroup);
			this.layoutMain.Controls.Add(this.chkLabelsVisible);
			this.layoutMain.Controls.Add(this.nudHoleRadius);
			this.layoutMain.Controls.Add(this.cboBarShape);
			this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutMain.Location = new System.Drawing.Point(0, 0);
			this.layoutMain.Name = "layoutMain";
			this.layoutMain.Root = this.layoutControlGroup1;
			this.layoutMain.Size = new System.Drawing.Size(569, 155);
			this.layoutMain.TabIndex = 9;
			this.layoutMain.Text = "mgLayoutControlCommon1";
			// 
			// btnExplosion
			// 
			this.btnExplosion.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.btnExplosion.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(60)))), ((int)(((byte)(0)))));
			this.btnExplosion.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.btnExplosion.Appearance.ForeColor = System.Drawing.Color.White;
			this.btnExplosion.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.btnExplosion.Appearance.Options.UseBackColor = true;
			this.btnExplosion.Appearance.Options.UseFont = true;
			this.btnExplosion.Appearance.Options.UseForeColor = true;
			this.btnExplosion.ButtonColor = mgModel.mgButtonGUIButtonColor.Green;
			this.btnExplosion.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.btnExplosion.Location = new System.Drawing.Point(356, 95);
			this.btnExplosion.Name = "btnExplosion";
			this.btnExplosion.Size = new System.Drawing.Size(69, 20);
			this.btnExplosion.TabIndex = 7;
			this.btnExplosion.Text = "Expl.";
			this.btnExplosion.Click += new System.EventHandler(this.btnExplosion_Click);
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.layoutControlGroup2,
            this.layoutControlGroup3,
            this.emptySpaceItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(569, 155);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.Control = this.cboSeriesSource;
			this.layoutControlItem1.CustomizationFormText = "Series Source:";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(233, 31);
			this.layoutControlItem1.Text = "Series Source:";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(81, 13);
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.cboSeriesGroup;
			this.layoutControlItem2.CustomizationFormText = "Series Group:";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 31);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(233, 31);
			this.layoutControlItem2.Text = "Series Group:";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(81, 13);
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.cboBarShape;
			this.layoutControlItem3.CustomizationFormText = "Bar Shape:";
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 62);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(233, 93);
			this.layoutControlItem3.Text = "Bar Shape:";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(81, 13);
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(233, 123);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(336, 32);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// layoutControlGroup2
			// 
			this.layoutControlGroup2.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.layoutControlGroup2.AppearanceGroup.Options.UseFont = true;
			this.layoutControlGroup2.CustomizationFormText = "Series Label";
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem5});
			this.layoutControlGroup2.Location = new System.Drawing.Point(233, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Size = new System.Drawing.Size(336, 57);
			this.layoutControlGroup2.Text = "Series Label";
			// 
			// layoutControlItem4
			// 
			this.layoutControlItem4.Control = this.chkLabelsVisible;
			this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem4.MaxSize = new System.Drawing.Size(74, 31);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(74, 31);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(74, 31);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.Text = "layoutControlItem4";
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			// 
			// layoutControlItem5
			// 
			this.layoutControlItem5.Control = this.cboPosition;
			this.layoutControlItem5.CustomizationFormText = "Position:";
			this.layoutControlItem5.Location = new System.Drawing.Point(74, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(256, 31);
			this.layoutControlItem5.Text = "Position:";
			this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
			this.layoutControlItem5.TextSize = new System.Drawing.Size(41, 13);
			this.layoutControlItem5.TextToControlDistance = 5;
			// 
			// layoutControlGroup3
			// 
			this.layoutControlGroup3.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.layoutControlGroup3.AppearanceGroup.Options.UseFont = true;
			this.layoutControlGroup3.CustomizationFormText = "Pie and Doughnut";
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8});
			this.layoutControlGroup3.Location = new System.Drawing.Point(233, 67);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Size = new System.Drawing.Size(336, 56);
			this.layoutControlGroup3.Text = "Pie and Doughnut";
			// 
			// layoutControlItem6
			// 
			this.layoutControlItem6.Control = this.nudHoleRadius;
			this.layoutControlItem6.CustomizationFormText = "Hole and Radius:";
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(115, 30);
			this.layoutControlItem6.Text = "Hole and Radius:";
			this.layoutControlItem6.TextSize = new System.Drawing.Size(81, 13);
			// 
			// layoutControlItem7
			// 
			this.layoutControlItem7.Control = this.btnExplosion;
			this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
			this.layoutControlItem7.Location = new System.Drawing.Point(115, 0);
			this.layoutControlItem7.MaxSize = new System.Drawing.Size(79, 30);
			this.layoutControlItem7.MinSize = new System.Drawing.Size(79, 30);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(79, 30);
			this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem7.Text = "layoutControlItem7";
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextToControlDistance = 0;
			this.layoutControlItem7.TextVisible = false;
			// 
			// layoutControlItem8
			// 
			this.layoutControlItem8.Control = this.nudExplodedDist;
			this.layoutControlItem8.CustomizationFormText = "Exploded Dist.";
			this.layoutControlItem8.Location = new System.Drawing.Point(194, 0);
			this.layoutControlItem8.MaxSize = new System.Drawing.Size(136, 30);
			this.layoutControlItem8.MinSize = new System.Drawing.Size(136, 30);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Size = new System.Drawing.Size(136, 30);
			this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem8.Text = "Exploded Dist.";
			this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
			this.layoutControlItem8.TextSize = new System.Drawing.Size(69, 13);
			this.layoutControlItem8.TextToControlDistance = 5;
			// 
			// emptySpaceItem2
			// 
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(233, 57);
			this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 10);
			this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 10);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(336, 10);
			this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			// 
			// SeriesUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutMain);
			this.Name = "SeriesUI";
			this.Size = new System.Drawing.Size(569, 155);
			this.Load += new System.EventHandler(this.SeriesUI_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudExplodedDist)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudHoleRadius)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
			this.layoutMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ComboBox cboSeriesSource;
		private System.Windows.Forms.ComboBox cboSeriesGroup;
        private System.Windows.Forms.ComboBox cboPosition;
        private System.Windows.Forms.CheckBox chkLabelsVisible;
		private System.Windows.Forms.ComboBox cboBarShape;
		private System.Windows.Forms.NumericUpDown nudExplodedDist;
        private System.Windows.Forms.NumericUpDown nudHoleRadius;
		private mgControls.mgLayoutControlCommon layoutMain;
		private mgControls.mgDevX_SimpleButton btnExplosion;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
