namespace mgWinChart.usercontrols
{
    partial class GeneralUI
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
			this.cboHoleRadius = new System.Windows.Forms.ComboBox();
			this.nudExplodedDist = new System.Windows.Forms.NumericUpDown();
			this.cboPosition = new System.Windows.Forms.ComboBox();
			this.chkLabelsVisible = new System.Windows.Forms.CheckBox();
			this.cboBarShape = new System.Windows.Forms.ComboBox();
			this.layoutMain = new mgControls.mgLayoutControlCommon();
			this.btnExplosion = new mgControls.mgDevX_SimpleButton();
			this.mgDevX_SimpleButton1 = new mgControls.mgDevX_SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.nudExplodedDist)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
			this.layoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			this.SuspendLayout();
			// 
			// cboHoleRadius
			// 
			this.cboHoleRadius.FormattingEnabled = true;
			this.cboHoleRadius.Location = new System.Drawing.Point(414, 95);
			this.cboHoleRadius.Name = "cboHoleRadius";
			this.cboHoleRadius.Size = new System.Drawing.Size(95, 21);
			this.cboHoleRadius.TabIndex = 5;
			this.cboHoleRadius.SelectedIndexChanged += new System.EventHandler(this.cboHoleRadius_SelectedIndexChanged);
			// 
			// nudExplodedDist
			// 
			this.nudExplodedDist.Location = new System.Drawing.Point(665, 95);
			this.nudExplodedDist.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudExplodedDist.Name = "nudExplodedDist";
			this.nudExplodedDist.Size = new System.Drawing.Size(59, 20);
			this.nudExplodedDist.TabIndex = 2;
			this.nudExplodedDist.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudExplodedDist.ValueChanged += new System.EventHandler(this.nudExplodedDist_ValueChanged);
			// 
			// cboPosition
			// 
			this.cboPosition.FormattingEnabled = true;
			this.cboPosition.Location = new System.Drawing.Point(480, 28);
			this.cboPosition.Name = "cboPosition";
			this.cboPosition.Size = new System.Drawing.Size(244, 21);
			this.cboPosition.TabIndex = 1;
			this.cboPosition.Text = "default";
			this.cboPosition.SelectedIndexChanged += new System.EventHandler(this.cboPosition_SelectedIndexChanged);
			// 
			// chkLabelsVisible
			// 
			this.chkLabelsVisible.Checked = true;
			this.chkLabelsVisible.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLabelsVisible.Location = new System.Drawing.Point(350, 28);
			this.chkLabelsVisible.Name = "chkLabelsVisible";
			this.chkLabelsVisible.Size = new System.Drawing.Size(74, 20);
			this.chkLabelsVisible.TabIndex = 0;
			this.chkLabelsVisible.Text = "Visible";
			this.chkLabelsVisible.UseVisualStyleBackColor = true;
			this.chkLabelsVisible.CheckedChanged += new System.EventHandler(this.chkLabelsVisible_CheckedChanged);
			// 
			// cboBarShape
			// 
			this.cboBarShape.FormattingEnabled = true;
			this.cboBarShape.Items.AddRange(new object[] {
            "Box",
            "Cylinder",
            "Pyramid",
            "Cone"});
			this.cboBarShape.Location = new System.Drawing.Point(69, 5);
			this.cboBarShape.Name = "cboBarShape";
			this.cboBarShape.Size = new System.Drawing.Size(268, 21);
			this.cboBarShape.TabIndex = 8;
			this.cboBarShape.Text = "Box";
			this.cboBarShape.SelectedIndexChanged += new System.EventHandler(this.cboBarShape_SelectedIndexChanged);
			// 
			// layoutMain
			// 
			this.layoutMain.Controls.Add(this.btnExplosion);
			this.layoutMain.Controls.Add(this.nudExplodedDist);
			this.layoutMain.Controls.Add(this.cboHoleRadius);
			this.layoutMain.Controls.Add(this.mgDevX_SimpleButton1);
			this.layoutMain.Controls.Add(this.cboPosition);
			this.layoutMain.Controls.Add(this.cboBarShape);
			this.layoutMain.Controls.Add(this.chkLabelsVisible);
			this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutMain.Location = new System.Drawing.Point(0, 0);
			this.layoutMain.Name = "layoutMain";
			this.layoutMain.Root = this.layoutControlGroup1;
			this.layoutMain.Size = new System.Drawing.Size(732, 194);
			this.layoutMain.TabIndex = 12;
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
			this.btnExplosion.Location = new System.Drawing.Point(519, 95);
			this.btnExplosion.Name = "btnExplosion";
			this.btnExplosion.Size = new System.Drawing.Size(62, 21);
			this.btnExplosion.TabIndex = 13;
			this.btnExplosion.Text = "Expl.";
			this.btnExplosion.Click += new System.EventHandler(this.btnExplosion_Click);
			// 
			// mgDevX_SimpleButton1
			// 
			this.mgDevX_SimpleButton1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.mgDevX_SimpleButton1.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(60)))));
			this.mgDevX_SimpleButton1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.mgDevX_SimpleButton1.Appearance.ForeColor = System.Drawing.Color.White;
			this.mgDevX_SimpleButton1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			this.mgDevX_SimpleButton1.Appearance.Options.UseBackColor = true;
			this.mgDevX_SimpleButton1.Appearance.Options.UseFont = true;
			this.mgDevX_SimpleButton1.Appearance.Options.UseForeColor = true;
			this.mgDevX_SimpleButton1.ButtonColor = mgModel.mgButtonGUIButtonColor.Blue;
			this.mgDevX_SimpleButton1.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.mgDevX_SimpleButton1.Location = new System.Drawing.Point(5, 36);
			this.mgDevX_SimpleButton1.Name = "mgDevX_SimpleButton1";
			this.mgDevX_SimpleButton1.Size = new System.Drawing.Size(332, 20);
			this.mgDevX_SimpleButton1.TabIndex = 12;
			this.mgDevX_SimpleButton1.Text = "Align Series Settings";
			this.mgDevX_SimpleButton1.Click += new System.EventHandler(this.btnAlignSeriesSettings_Click);
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.emptySpaceItem2,
            this.emptySpaceItem3,
            this.layoutControlGroup2,
            this.layoutControlGroup3});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(732, 194);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.Control = this.cboBarShape;
			this.layoutControlItem1.CustomizationFormText = "Bar Shape:";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 31);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(94, 31);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(342, 31);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.Text = "Bar Shape:";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(60, 13);
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 61);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(342, 133);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.mgDevX_SimpleButton1;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 31);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(342, 30);
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			// 
			// emptySpaceItem2
			// 
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(342, 124);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(390, 70);
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			// 
			// emptySpaceItem3
			// 
			this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
			this.emptySpaceItem3.Location = new System.Drawing.Point(342, 57);
			this.emptySpaceItem3.MaxSize = new System.Drawing.Size(0, 10);
			this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 10);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(390, 10);
			this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem3.Text = "emptySpaceItem3";
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			// 
			// layoutControlGroup2
			// 
			this.layoutControlGroup2.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.layoutControlGroup2.AppearanceGroup.Options.UseFont = true;
			this.layoutControlGroup2.CustomizationFormText = "Pie and Doughnut";
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7});
			this.layoutControlGroup2.Location = new System.Drawing.Point(342, 67);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Size = new System.Drawing.Size(390, 57);
			this.layoutControlGroup2.Text = "Pie and Doughnut";
			// 
			// layoutControlItem5
			// 
			this.layoutControlItem5.Control = this.cboHoleRadius;
			this.layoutControlItem5.CustomizationFormText = "Hole Radius:";
			this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(169, 31);
			this.layoutControlItem5.Text = "Hole Radius:";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(60, 13);
			// 
			// layoutControlItem6
			// 
			this.layoutControlItem6.Control = this.btnExplosion;
			this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
			this.layoutControlItem6.Location = new System.Drawing.Point(169, 0);
			this.layoutControlItem6.MaxSize = new System.Drawing.Size(72, 31);
			this.layoutControlItem6.MinSize = new System.Drawing.Size(72, 31);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(72, 31);
			this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem6.Text = "layoutControlItem6";
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextToControlDistance = 0;
			this.layoutControlItem6.TextVisible = false;
			// 
			// layoutControlItem7
			// 
			this.layoutControlItem7.Control = this.nudExplodedDist;
			this.layoutControlItem7.CustomizationFormText = "Exploded Dist.";
			this.layoutControlItem7.Location = new System.Drawing.Point(241, 0);
			this.layoutControlItem7.MaxSize = new System.Drawing.Size(143, 31);
			this.layoutControlItem7.MinSize = new System.Drawing.Size(143, 31);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(143, 31);
			this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem7.Text = "Exploded Dist.";
			this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
			this.layoutControlItem7.TextSize = new System.Drawing.Size(69, 13);
			this.layoutControlItem7.TextToControlDistance = 5;
			// 
			// layoutControlGroup3
			// 
			this.layoutControlGroup3.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.layoutControlGroup3.AppearanceGroup.Options.UseFont = true;
			this.layoutControlGroup3.CustomizationFormText = "Series Labels";
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem4});
			this.layoutControlGroup3.Location = new System.Drawing.Point(342, 0);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Size = new System.Drawing.Size(390, 57);
			this.layoutControlGroup3.Text = "Series Labels";
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.chkLabelsVisible;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.MaxSize = new System.Drawing.Size(84, 30);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(84, 30);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(84, 31);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			// 
			// layoutControlItem4
			// 
			this.layoutControlItem4.Control = this.cboPosition;
			this.layoutControlItem4.CustomizationFormText = "Position:";
			this.layoutControlItem4.Location = new System.Drawing.Point(84, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(300, 31);
			this.layoutControlItem4.Text = "Position:";
			this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(41, 13);
			this.layoutControlItem4.TextToControlDistance = 5;
			// 
			// GeneralUI
			// 
			this.Controls.Add(this.layoutMain);
			this.Name = "GeneralUI";
			this.Size = new System.Drawing.Size(732, 194);
			this.Load += new System.EventHandler(this.GeneralUI_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudExplodedDist)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
			this.layoutMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.NumericUpDown nudExplodedDist;
        private System.Windows.Forms.ComboBox cboPosition;
		private System.Windows.Forms.CheckBox chkLabelsVisible;
		private System.Windows.Forms.ComboBox cboBarShape;
        private System.Windows.Forms.ComboBox cboHoleRadius;
		private mgControls.mgLayoutControlCommon layoutMain;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private mgControls.mgDevX_SimpleButton mgDevX_SimpleButton1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private mgControls.mgDevX_SimpleButton btnExplosion;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
    }
}
