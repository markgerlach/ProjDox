namespace mgWinChart.usercontrols
{
    partial class ucRealTimeChart
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
            DevExpress.XtraCharts.SwiftPlotDiagram swiftPlotDiagram1 = new DevExpress.XtraCharts.SwiftPlotDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SwiftPlotSeriesView swiftPlotSeriesView1 = new DevExpress.XtraCharts.SwiftPlotSeriesView();
            DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SwiftPlotSeriesView swiftPlotSeriesView2 = new DevExpress.XtraCharts.SwiftPlotSeriesView();
            DevExpress.XtraCharts.SwiftPlotSeriesView swiftPlotSeriesView3 = new DevExpress.XtraCharts.SwiftPlotSeriesView();
            this.layoutcontrolMain = new mgControls.mgLayoutControl();
            this.chartMain = new DevExpress.XtraCharts.ChartControl();
            this.ribbonMenu = new mgControls.mgDevX_RibbonControl();
            this.barTimeRange = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.groupDisplay = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.layoutgroupMain = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutMenu = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutChart = new DevExpress.XtraLayout.LayoutControlItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutcontrolMain)).BeginInit();
            this.layoutcontrolMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotSeriesView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotSeriesView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotSeriesView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutgroupMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutChart)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutcontrolMain
            // 
            this.layoutcontrolMain.AllowCustomizationMenu = false;
            this.layoutcontrolMain.BackColor = System.Drawing.Color.White;
            this.layoutcontrolMain.Controls.Add(this.chartMain);
            this.layoutcontrolMain.Controls.Add(this.ribbonMenu);
            this.layoutcontrolMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutcontrolMain.Location = new System.Drawing.Point(0, 0);
            this.layoutcontrolMain.Name = "layoutcontrolMain";
            this.layoutcontrolMain.Root = this.layoutgroupMain;
            this.layoutcontrolMain.Size = new System.Drawing.Size(900, 568);
            this.layoutcontrolMain.TabIndex = 0;
            this.layoutcontrolMain.Text = "mgLayoutControl1";
            // 
            // chartMain
            // 
            swiftPlotDiagram1.AxisX.DateTimeGridAlignment = DevExpress.XtraCharts.DateTimeMeasurementUnit.Millisecond;
            swiftPlotDiagram1.AxisX.DateTimeMeasureUnit = DevExpress.XtraCharts.DateTimeMeasurementUnit.Millisecond;
            swiftPlotDiagram1.AxisX.DateTimeOptions.Format = DevExpress.XtraCharts.DateTimeFormat.Custom;
            swiftPlotDiagram1.AxisX.DateTimeOptions.FormatString = "mm:ss";
            swiftPlotDiagram1.AxisX.GridLines.Visible = true;
            swiftPlotDiagram1.AxisX.GridSpacing = 1000D;
            swiftPlotDiagram1.AxisX.GridSpacingAuto = false;
            swiftPlotDiagram1.AxisX.Label.MaxWidth = 50;
            swiftPlotDiagram1.AxisX.Label.TextAlignment = System.Drawing.StringAlignment.Far;
            swiftPlotDiagram1.AxisX.MinorCount = 9;
            swiftPlotDiagram1.AxisX.Range.Auto = false;
            swiftPlotDiagram1.AxisX.Range.MaxValueSerializable = "07/21/2011 17:19:26.000";
            swiftPlotDiagram1.AxisX.Range.MinValueSerializable = "07/21/2011 17:19:24.000";
            swiftPlotDiagram1.AxisX.Range.ScrollingRange.SideMarginsEnabled = true;
            swiftPlotDiagram1.AxisX.Range.SideMarginsEnabled = true;
            swiftPlotDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            swiftPlotDiagram1.AxisY.Interlaced = true;
            swiftPlotDiagram1.AxisY.Range.ScrollingRange.SideMarginsEnabled = true;
            swiftPlotDiagram1.AxisY.Range.SideMarginsEnabled = true;
            swiftPlotDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            swiftPlotDiagram1.Margins.Left = 25;
            swiftPlotDiagram1.Margins.Right = 25;
            this.chartMain.Diagram = swiftPlotDiagram1;
            this.chartMain.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right;
            this.chartMain.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.TopOutside;
            this.chartMain.Legend.Direction = DevExpress.XtraCharts.LegendDirection.LeftToRight;
            this.chartMain.Location = new System.Drawing.Point(5, 111);
            this.chartMain.Name = "chartMain";
            series1.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.DateTime;
            series1.Name = "Series 1";
            series1.View = swiftPlotSeriesView1;
            series2.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.DateTime;
            series2.Name = "Series 2";
            series2.View = swiftPlotSeriesView2;
            this.chartMain.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};
            this.chartMain.SeriesTemplate.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.DateTime;
            this.chartMain.SeriesTemplate.View = swiftPlotSeriesView3;
            this.chartMain.Size = new System.Drawing.Size(890, 452);
            this.chartMain.TabIndex = 5;
            // 
            // ribbonMenu
            // 
            this.ribbonMenu.ApplicationButtonText = null;
            this.ribbonMenu.Dock = System.Windows.Forms.DockStyle.None;
            // 
            // 
            // 
            this.ribbonMenu.ExpandCollapseItem.Id = 0;
            this.ribbonMenu.ExpandCollapseItem.Name = "";
            this.ribbonMenu.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonMenu.ExpandCollapseItem,
            this.barTimeRange});
            this.ribbonMenu.Location = new System.Drawing.Point(5, 5);
            this.ribbonMenu.MaxItemId = 2;
            this.ribbonMenu.Name = "ribbonMenu";
            this.ribbonMenu.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonMenu.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinEdit1});
            this.ribbonMenu.SelectedPage = this.ribbonPage1;
            this.ribbonMenu.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.ShowOnMultiplePages;
            this.ribbonMenu.Size = new System.Drawing.Size(890, 96);
            this.ribbonMenu.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // barTimeRange
            // 
            this.barTimeRange.Caption = "Time Span to Show:";
            this.barTimeRange.Edit = this.repositoryItemSpinEdit1;
            this.barTimeRange.EditValue = 30;
            this.barTimeRange.Id = 1;
            this.barTimeRange.Name = "barTimeRange";
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemSpinEdit1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.repositoryItemSpinEdit1.MinValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.groupDisplay});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // groupDisplay
            // 
            this.groupDisplay.ItemLinks.Add(this.barTimeRange);
            this.groupDisplay.Name = "groupDisplay";
            this.groupDisplay.ShowCaptionButton = false;
            this.groupDisplay.Text = "Display";
            // 
            // layoutgroupMain
            // 
            this.layoutgroupMain.CustomizationFormText = "layoutGroupMain";
            this.layoutgroupMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutgroupMain.GroupBordersVisible = false;
            this.layoutgroupMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutMenu,
            this.layoutChart});
            this.layoutgroupMain.Location = new System.Drawing.Point(0, 0);
            this.layoutgroupMain.Name = "layoutgroupMain";
            this.layoutgroupMain.Size = new System.Drawing.Size(900, 568);
            this.layoutgroupMain.Text = "layoutgroupMain";
            this.layoutgroupMain.TextVisible = false;
            // 
            // layoutMenu
            // 
            this.layoutMenu.Control = this.ribbonMenu;
            this.layoutMenu.CustomizationFormText = "layoutMenu";
            this.layoutMenu.Location = new System.Drawing.Point(0, 0);
            this.layoutMenu.MaxSize = new System.Drawing.Size(0, 106);
            this.layoutMenu.MinSize = new System.Drawing.Size(27, 106);
            this.layoutMenu.Name = "layoutMenu";
            this.layoutMenu.Size = new System.Drawing.Size(900, 106);
            this.layoutMenu.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutMenu.Text = "layoutMenu";
            this.layoutMenu.TextSize = new System.Drawing.Size(0, 0);
            this.layoutMenu.TextToControlDistance = 0;
            this.layoutMenu.TextVisible = false;
            // 
            // layoutChart
            // 
            this.layoutChart.Control = this.chartMain;
            this.layoutChart.CustomizationFormText = "layoutChart";
            this.layoutChart.Location = new System.Drawing.Point(0, 106);
            this.layoutChart.Name = "layoutChart";
            this.layoutChart.Size = new System.Drawing.Size(900, 462);
            this.layoutChart.Text = "layoutChart";
            this.layoutChart.TextSize = new System.Drawing.Size(0, 0);
            this.layoutChart.TextToControlDistance = 0;
            this.layoutChart.TextVisible = false;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // ucRealTimeChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutcontrolMain);
            this.Name = "ucRealTimeChart";
            this.Size = new System.Drawing.Size(900, 568);
            ((System.ComponentModel.ISupportInitialize)(this.layoutcontrolMain)).EndInit();
            this.layoutcontrolMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(swiftPlotDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotSeriesView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(swiftPlotSeriesView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutgroupMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private mgControls.mgLayoutControl layoutcontrolMain;
        private DevExpress.XtraCharts.ChartControl chartMain;
        private mgControls.mgDevX_RibbonControl ribbonMenu;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupDisplay;
        private DevExpress.XtraLayout.LayoutControlGroup layoutgroupMain;
        private DevExpress.XtraLayout.LayoutControlItem layoutMenu;
        private DevExpress.XtraLayout.LayoutControlItem layoutChart;
        private DevExpress.XtraBars.BarEditItem barTimeRange;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private System.Windows.Forms.Timer timer;

    }
}
