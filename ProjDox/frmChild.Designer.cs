namespace ProjDox
{
    partial class frmChild
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChild));
            this.layoutMain = new mgControls.mgLayoutControl();
            this.ribbonMain = new mgControls.mgDevX_RibbonControl();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.splitTopBottom = new DevExpress.XtraEditors.SplitContainerControl();
            this.layoutLeftTop = new mgControls.mgLayoutControl();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutLeftBottom = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
            this.layoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitTopBottom)).BeginInit();
            this.splitTopBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutLeftTop)).BeginInit();
            this.layoutLeftTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutLeftBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // ucGrLbl
            // 
            this.ucGrLbl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ucGrLbl.BackgroundImage")));
            this.ucGrLbl.Size = new System.Drawing.Size(657, 24);
            // 
            // layoutMain
            // 
            this.layoutMain.AllowCustomization = false;
            this.layoutMain.BackColor = System.Drawing.Color.White;
            this.layoutMain.Controls.Add(this.ribbonMain);
            this.layoutMain.Controls.Add(this.splitTopBottom);
            this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutMain.Location = new System.Drawing.Point(0, 0);
            this.layoutMain.Name = "layoutMain";
            this.layoutMain.Root = this.Root;
            this.layoutMain.Size = new System.Drawing.Size(679, 532);
            this.layoutMain.TabIndex = 1;
            this.layoutMain.Text = " ";
            // 
            // ribbonMain
            // 
            this.ribbonMain.Dock = System.Windows.Forms.DockStyle.None;
            this.ribbonMain.ExpandCollapseItem.Id = 0;
            this.ribbonMain.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonMain.ExpandCollapseItem,
            this.ribbonMain.SearchEditItem});
            this.ribbonMain.Location = new System.Drawing.Point(101, 5);
            this.ribbonMain.MaxItemId = 1;
            this.ribbonMain.Name = "ribbonMain";
            this.ribbonMain.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonMain.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonMain.Size = new System.Drawing.Size(573, 126);
            this.ribbonMain.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // splitTopBottom
            // 
            this.splitTopBottom.Horizontal = false;
            this.splitTopBottom.Location = new System.Drawing.Point(5, 142);
            this.splitTopBottom.Name = "splitTopBottom";
            this.splitTopBottom.Panel1.Text = "Panel1";
            this.splitTopBottom.Panel2.Controls.Add(this.layoutLeftTop);
            this.splitTopBottom.Panel2.Controls.Add(this.layoutLeftBottom);
            this.splitTopBottom.Panel2.Text = "Panel2";
            this.splitTopBottom.Size = new System.Drawing.Size(669, 385);
            this.splitTopBottom.SplitterPosition = 149;
            this.splitTopBottom.TabIndex = 5;
            // 
            // layoutLeftTop
            // 
            this.layoutLeftTop.AllowCustomization = false;
            this.layoutLeftTop.BackColor = System.Drawing.Color.White;
            this.layoutLeftTop.Controls.Add(this.xtraTabControl1);
            this.layoutLeftTop.Location = new System.Drawing.Point(353, 59);
            this.layoutLeftTop.Name = "layoutLeftTop";
            this.layoutLeftTop.Root = this.layoutControlGroup1;
            this.layoutLeftTop.Size = new System.Drawing.Size(180, 120);
            this.layoutLeftTop.TabIndex = 0;
            this.layoutLeftTop.Text = "mgLayoutControl1";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Location = new System.Drawing.Point(101, 5);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(100, 93);
            this.xtraTabControl1.TabIndex = 4;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(98, 68);
            this.xtraTabPage1.Text = "xtraTabPage1";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(98, 68);
            this.xtraTabPage2.Text = "xtraTabPage2";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3});
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(206, 103);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.xtraTabControl1;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(206, 103);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(93, 13);
            // 
            // layoutLeftBottom
            // 
            this.layoutLeftBottom.Location = new System.Drawing.Point(52, 59);
            this.layoutLeftBottom.Name = "layoutLeftBottom";
            this.layoutLeftBottom.Root = this.layoutControlGroup2;
            this.layoutLeftBottom.Size = new System.Drawing.Size(180, 120);
            this.layoutLeftBottom.TabIndex = 0;
            this.layoutLeftBottom.Text = "layoutControl1";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(180, 120);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem4});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(679, 532);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.splitTopBottom;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 137);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(679, 395);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.ribbonMain;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(679, 137);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(93, 13);
            // 
            // frmChild
            // 
            this.ClientSize = new System.Drawing.Size(679, 532);
            this.Controls.Add(this.layoutMain);
            this.Name = "frmChild";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.SetChildIndex(this.ucGrLbl, 0);
            this.Controls.SetChildIndex(this.layoutMain, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
            this.layoutMain.ResumeLayout(false);
            this.layoutMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitTopBottom)).EndInit();
            this.splitTopBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutLeftTop)).EndInit();
            this.layoutLeftTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutLeftBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private mgControls.mgLayoutControl layoutMain;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SplitContainerControl splitTopBottom;
        private mgControls.mgLayoutControl layoutLeftTop;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControl layoutLeftBottom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private mgControls.mgDevX_RibbonControl ribbonMain;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}
