namespace mgWinChart.usercontrols
{
    partial class ucWinChart
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
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel2 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel3 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup1 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucWinChart));
            this.chartMain = new DevExpress.XtraCharts.ChartControl();
            this.layoutMain = new mgControls.mgLayoutControl();
            this.lblProcessing = new DevExpress.XtraEditors.LabelControl();
            this.prg = new DevExpress.XtraEditors.ProgressBarControl();
            this.ribbonMain = new mgControls.mgDevX_RibbonControl();
            this.btnChartType = new DevExpress.XtraBars.BarButtonItem();
            this.gddChartType = new DevExpress.XtraBars.Ribbon.GalleryDropDown(this.components);
            this.images = new DevExpress.Utils.ImageCollection(this.components);
            this.barColorScheme = new DevExpress.XtraBars.BarEditItem();
            this.lueColorScheme = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.chkPercent = new DevExpress.XtraBars.BarCheckItem();
            this.barLegend = new DevExpress.XtraBars.BarEditItem();
            this.lueLegend = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.barGeneralBarShape = new DevExpress.XtraBars.BarEditItem();
            this.lueGeneralBarShape = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.barSeriesSource = new DevExpress.XtraBars.BarEditItem();
            this.lueSeriesSource = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.barSeriesGroup = new DevExpress.XtraBars.BarEditItem();
            this.lueSeriesGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.chkSeriesLabelVisible = new DevExpress.XtraBars.BarCheckItem();
            this.barSeriesLabelPosition = new DevExpress.XtraBars.BarEditItem();
            this.lueSeriesLabelPosition = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.barExplodedDist = new DevExpress.XtraBars.BarEditItem();
            this.spinExplodedDist = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barHoleRadius = new DevExpress.XtraBars.BarEditItem();
            this.lueHoleRadius = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.barExplodedPoints = new DevExpress.XtraBars.BarEditItem();
            this.lueExplodedPoints = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.btnCopyJpg = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveJpg = new DevExpress.XtraBars.BarButtonItem();
            this.chkShowOnlyTop = new DevExpress.XtraBars.BarCheckItem();
            this.barShowTop = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.chkShowOther = new DevExpress.XtraBars.BarCheckItem();
            this.barMarkerShape = new DevExpress.XtraBars.BarEditItem();
            this.lueMarkerShape = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.repositoryItemSpinEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.chkPointMarkers = new DevExpress.XtraBars.BarCheckItem();
            this.btnPrintChart = new DevExpress.XtraBars.BarButtonItem();
            this.chkValueInLegend = new DevExpress.XtraBars.BarCheckItem();
            this.chkPercentInLegend = new DevExpress.XtraBars.BarCheckItem();
            this.pageGeneral = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.groupChartType = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupLegend = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupGeneral = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupLineArea = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupPieDoughnut = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupTopOptions = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupCopySave = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.lueSeriesBarShape = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.spinHoleRadius = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutRibbon = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutGroupChart = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutChart = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutGroupUpdating = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.barMarkerSize = new DevExpress.XtraBars.BarEditItem();
            this.lueMarkerSize = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
            this.layoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.prg.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gddChartType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.images)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueColorScheme)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueLegend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueGeneralBarShape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesLabelPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExplodedDist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueHoleRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueExplodedPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueMarkerShape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesBarShape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinHoleRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutRibbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutGroupChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutGroupUpdating)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueMarkerSize)).BeginInit();
            this.SuspendLayout();
            // 
            // chartMain
            // 
            xyDiagram1.AxisX.Label.Angle = -90;
            //xyDiagram1.AxisX.Range.ScrollingRange.SideMarginsEnabled = true;
            //xyDiagram1.AxisX.Range.SideMarginsEnabled = true;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            //xyDiagram1.AxisY.Range.ScrollingRange.SideMarginsEnabled = true;
            //xyDiagram1.AxisY.Range.SideMarginsEnabled = true;
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            this.chartMain.Diagram = xyDiagram1;
            //this.chartMain.Legend.Visible = false;
            this.chartMain.Location = new System.Drawing.Point(5, 111);
            this.chartMain.Name = "chartMain";
            //sideBySideBarSeriesLabel1.LineVisible = true;
            series1.Label = sideBySideBarSeriesLabel1;
            series1.Name = "Series 1";
            //sideBySideBarSeriesLabel2.LineVisible = true;
            series2.Label = sideBySideBarSeriesLabel2;
            series2.Name = "Series 2";
            this.chartMain.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};
            //sideBySideBarSeriesLabel3.LineVisible = true;
            this.chartMain.SeriesTemplate.Label = sideBySideBarSeriesLabel3;
            this.chartMain.SideBySideEqualBarWidth = true;
            this.chartMain.Size = new System.Drawing.Size(1121, 255);
            this.chartMain.TabIndex = 0;
            this.chartMain.CustomDrawSeries += new DevExpress.XtraCharts.CustomDrawSeriesEventHandler(this.chartMain_CustomDrawSeries);
            this.chartMain.CustomDrawSeriesPoint += new DevExpress.XtraCharts.CustomDrawSeriesPointEventHandler(this.chartMain_CustomDrawSeriesPoint);
            this.chartMain.EnabledChanged += new System.EventHandler(this.chartMain_EnabledChanged);
            this.chartMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chartMain_MouseClick);
            this.chartMain.MouseHover += new System.EventHandler(this.chartMain_MouseHover);
            this.chartMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chartMain_MouseMove);
            // 
            // layoutMain
            // 
            //this.layoutMain.AllowCustomizationMenu = false;
            this.layoutMain.BackColor = System.Drawing.Color.White;
            this.layoutMain.Controls.Add(this.lblProcessing);
            this.layoutMain.Controls.Add(this.prg);
            this.layoutMain.Controls.Add(this.ribbonMain);
            this.layoutMain.Controls.Add(this.chartMain);
            this.layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutMain.Location = new System.Drawing.Point(0, 0);
            this.layoutMain.Name = "layoutMain";
            this.layoutMain.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutMain.Root = this.layoutControlGroup1;
            this.layoutMain.Size = new System.Drawing.Size(1131, 517);
            this.layoutMain.TabIndex = 15;
            this.layoutMain.Text = "mgLayoutControl1";
            // 
            // lblProcessing
            // 
            this.lblProcessing.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblProcessing.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblProcessing.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProcessing.Location = new System.Drawing.Point(75, 432);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(985, 14);
            this.lblProcessing.StyleController = this.layoutMain;
            this.lblProcessing.TabIndex = 8;
            this.lblProcessing.Text = "Please wait...  Refreshing Data...";
            // 
            // prg
            // 
            this.prg.EditValue = 50;
            this.prg.Location = new System.Drawing.Point(75, 456);
            this.prg.MenuManager = this.ribbonMain;
            this.prg.Name = "prg";
            this.prg.Size = new System.Drawing.Size(985, 18);
            this.prg.StyleController = this.layoutMain;
            this.prg.TabIndex = 7;
            // 
            // ribbonMain
            // 
            this.ribbonMain.ApplicationButtonText = null;
            this.ribbonMain.Dock = System.Windows.Forms.DockStyle.None;
            this.ribbonMain.ExpandCollapseItem.Id = 0;
            this.ribbonMain.ExpandCollapseItem.Name = "";
            this.ribbonMain.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonMain.ExpandCollapseItem,
            this.btnChartType,
            this.barColorScheme,
            this.chkPercent,
            this.barLegend,
            this.barGeneralBarShape,
            this.barSeriesSource,
            this.barSeriesGroup,
            this.chkSeriesLabelVisible,
            this.barSeriesLabelPosition,
            this.barExplodedDist,
            this.barHoleRadius,
            this.barExplodedPoints,
            this.btnCopyJpg,
            this.btnSaveJpg,
            this.chkShowOnlyTop,
            this.barShowTop,
            this.chkShowOther,
            this.barMarkerShape,
            this.chkPointMarkers,
            this.btnPrintChart,
            this.chkValueInLegend,
            this.chkPercentInLegend,
            this.barMarkerSize});
            this.ribbonMain.Location = new System.Drawing.Point(5, 5);
            this.ribbonMain.MaxItemId = 49;
            this.ribbonMain.Name = "ribbonMain";
            this.ribbonMain.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.pageGeneral});
            this.ribbonMain.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.lueColorScheme,
            this.lueLegend,
            this.lueGeneralBarShape,
            this.lueSeriesSource,
            this.lueSeriesGroup,
            this.lueSeriesBarShape,
            this.lueSeriesLabelPosition,
            this.spinHoleRadius,
            this.spinExplodedDist,
            this.lueHoleRadius,
            this.lueExplodedPoints,
            this.repositoryItemSpinEdit1,
            this.repositoryItemLookUpEdit1,
            this.lueMarkerShape,
            this.repositoryItemSpinEdit2,
            this.lueMarkerSize});
            this.ribbonMain.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.ribbonMain.Size = new System.Drawing.Size(1121, 96);
			this.ribbonMain.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // btnChartType
            // 
            this.btnChartType.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.btnChartType.Caption = "Chart Type";
            this.btnChartType.DropDownControl = this.gddChartType;
            this.btnChartType.Id = 1;
            this.btnChartType.ItemAppearance.Normal.Options.UseTextOptions = true;
            this.btnChartType.ItemAppearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnChartType.LargeWidth = 160;
            this.btnChartType.Name = "btnChartType";
            this.btnChartType.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // gddChartType
            // 
            // 
            // gddChartType
            // 
            this.gddChartType.Gallery.AllowFilter = false;
            galleryItemGroup1.Caption = "Group1";
            this.gddChartType.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
            galleryItemGroup1});
            this.gddChartType.Gallery.Images = this.images;
            this.gddChartType.Name = "gddChartType";
            this.gddChartType.Ribbon = this.ribbonMain;
            this.gddChartType.GalleryItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.gddChartType_GalleryItemClick);
            // 
            // images
            // 
            this.images.ImageSize = new System.Drawing.Size(32, 32);
            this.images.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("images.ImageStream")));
            // 
            // barColorScheme
            // 
            this.barColorScheme.Caption = "Color Scheme:";
            this.barColorScheme.Edit = this.lueColorScheme;
            this.barColorScheme.Id = 3;
            this.barColorScheme.Name = "barColorScheme";
            this.barColorScheme.Width = 80;
            this.barColorScheme.EditValueChanged += new System.EventHandler(this.barColorScheme_EditValueChanged);
            // 
            // lueColorScheme
            // 
            this.lueColorScheme.AutoHeight = false;
            this.lueColorScheme.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueColorScheme.Name = "lueColorScheme";
            this.lueColorScheme.EditValueChanged += new System.EventHandler(this.lueColorScheme_EditValueChanged);
            // 
            // chkPercent
            // 
            this.chkPercent.Caption = "Value As Percent (%)";
            this.chkPercent.Id = 4;
            this.chkPercent.Name = "chkPercent";
            this.chkPercent.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkPercent_CheckedChanged);
            // 
            // barLegend
            // 
            this.barLegend.Caption = "Legend:";
            this.barLegend.Edit = this.lueLegend;
            this.barLegend.Id = 5;
            this.barLegend.Name = "barLegend";
            this.barLegend.Width = 110;
            this.barLegend.EditValueChanged += new System.EventHandler(this.barLegend_EditValueChanged);
            this.barLegend.HiddenEditor += new DevExpress.XtraBars.ItemClickEventHandler(this.barLegend_HiddenEditor);
            this.barLegend.ShownEditor += new DevExpress.XtraBars.ItemClickEventHandler(this.barLegend_ShownEditor);
            // 
            // lueLegend
            // 
            this.lueLegend.AutoHeight = false;
            this.lueLegend.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueLegend.DropDownRows = 9;
            this.lueLegend.Name = "lueLegend";
            this.lueLegend.EditValueChanged += new System.EventHandler(this.lueLegend_EditValueChanged);
            // 
            // barGeneralBarShape
            // 
            this.barGeneralBarShape.Caption = "Bar Shape:";
            this.barGeneralBarShape.Edit = this.lueGeneralBarShape;
            this.barGeneralBarShape.Id = 6;
            this.barGeneralBarShape.Name = "barGeneralBarShape";
            this.barGeneralBarShape.Width = 106;
            this.barGeneralBarShape.EditValueChanged += new System.EventHandler(this.barGeneralBarShape_EditValueChanged);
            // 
            // lueGeneralBarShape
            // 
            this.lueGeneralBarShape.AutoHeight = false;
            this.lueGeneralBarShape.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueGeneralBarShape.Name = "lueGeneralBarShape";
            // 
            // barSeriesSource
            // 
            this.barSeriesSource.Caption = "Series Source:";
            this.barSeriesSource.Edit = this.lueSeriesSource;
            this.barSeriesSource.Id = 8;
            this.barSeriesSource.Name = "barSeriesSource";
            this.barSeriesSource.Width = 80;
            // 
            // lueSeriesSource
            // 
            this.lueSeriesSource.AutoHeight = false;
            this.lueSeriesSource.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueSeriesSource.Name = "lueSeriesSource";
            // 
            // barSeriesGroup
            // 
            this.barSeriesGroup.Caption = "Series Group:";
            this.barSeriesGroup.Edit = this.lueSeriesGroup;
            this.barSeriesGroup.Id = 9;
            this.barSeriesGroup.Name = "barSeriesGroup";
            this.barSeriesGroup.Width = 80;
            // 
            // lueSeriesGroup
            // 
            this.lueSeriesGroup.AutoHeight = false;
            this.lueSeriesGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueSeriesGroup.Name = "lueSeriesGroup";
            // 
            // chkSeriesLabelVisible
            // 
            this.chkSeriesLabelVisible.Caption = "Series Label";
            this.chkSeriesLabelVisible.Checked = true;
            this.chkSeriesLabelVisible.Id = 11;
            this.chkSeriesLabelVisible.Name = "chkSeriesLabelVisible";
            this.chkSeriesLabelVisible.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkSeriesLabelVisible_CheckedChanged);
            // 
            // barSeriesLabelPosition
            // 
            this.barSeriesLabelPosition.Caption = "Label Position:";
            this.barSeriesLabelPosition.Edit = this.lueSeriesLabelPosition;
            this.barSeriesLabelPosition.Id = 12;
            this.barSeriesLabelPosition.Name = "barSeriesLabelPosition";
            this.barSeriesLabelPosition.Width = 90;
            this.barSeriesLabelPosition.EditValueChanged += new System.EventHandler(this.barSeriesLabelPosition_EditValueChanged);
            // 
            // lueSeriesLabelPosition
            // 
            this.lueSeriesLabelPosition.AutoHeight = false;
            this.lueSeriesLabelPosition.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueSeriesLabelPosition.Name = "lueSeriesLabelPosition";
            // 
            // barExplodedDist
            // 
            this.barExplodedDist.Caption = "Exploded Distance:";
            this.barExplodedDist.Edit = this.spinExplodedDist;
            this.barExplodedDist.Id = 15;
            this.barExplodedDist.Name = "barExplodedDist";
            this.barExplodedDist.EditValueChanged += new System.EventHandler(this.barExplodedDist_EditValueChanged);
            // 
            // spinExplodedDist
            // 
            this.spinExplodedDist.AutoHeight = false;
            this.spinExplodedDist.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinExplodedDist.Name = "spinExplodedDist";
            // 
            // barHoleRadius
            // 
            this.barHoleRadius.Caption = "Hole Radius:";
            this.barHoleRadius.Edit = this.lueHoleRadius;
            this.barHoleRadius.Id = 20;
            this.barHoleRadius.Name = "barHoleRadius";
            this.barHoleRadius.Width = 100;
            this.barHoleRadius.EditValueChanged += new System.EventHandler(this.barHoleRadius_EditValueChanged);
            // 
            // lueHoleRadius
            // 
            this.lueHoleRadius.AutoHeight = false;
            this.lueHoleRadius.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueHoleRadius.Name = "lueHoleRadius";
            // 
            // barExplodedPoints
            // 
            this.barExplodedPoints.Caption = "Exploded Points:";
            this.barExplodedPoints.Edit = this.lueExplodedPoints;
            this.barExplodedPoints.Id = 21;
            this.barExplodedPoints.Name = "barExplodedPoints";
            this.barExplodedPoints.Width = 80;
            this.barExplodedPoints.EditValueChanged += new System.EventHandler(this.barExplodedPoints_EditValueChanged);
            // 
            // lueExplodedPoints
            // 
            this.lueExplodedPoints.AutoHeight = false;
            this.lueExplodedPoints.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueExplodedPoints.Name = "lueExplodedPoints";
            // 
            // btnCopyJpg
            // 
            this.btnCopyJpg.Caption = "Copy";
            this.btnCopyJpg.Id = 23;
            this.btnCopyJpg.LargeWidth = 80;
            this.btnCopyJpg.Name = "btnCopyJpg";
            this.btnCopyJpg.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.btnCopyJpg.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCopyJpg_ItemClick);
            // 
            // btnSaveJpg
            // 
            this.btnSaveJpg.Caption = "Save to File";
            this.btnSaveJpg.Id = 24;
            this.btnSaveJpg.LargeWidth = 80;
            this.btnSaveJpg.Name = "btnSaveJpg";
            this.btnSaveJpg.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.btnSaveJpg.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveJpg_ItemClick);
            // 
            // chkShowOnlyTop
            // 
            this.chkShowOnlyTop.Caption = "Show Only Top 5 Records";
            this.chkShowOnlyTop.Id = 26;
            this.chkShowOnlyTop.Name = "chkShowOnlyTop";
            this.chkShowOnlyTop.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkShowOnlyTop_CheckedChanged);
            // 
            // barShowTop
            // 
            this.barShowTop.Caption = "    Show Top";
            this.barShowTop.Edit = this.repositoryItemSpinEdit1;
            this.barShowTop.EditValue = 5;
            this.barShowTop.Enabled = false;
            this.barShowTop.Id = 27;
            this.barShowTop.Name = "barShowTop";
            this.barShowTop.EditValueChanged += new System.EventHandler(this.barShowTop_EditValueChanged);
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // chkShowOther
            // 
            this.chkShowOther.Caption = "   Show \'Other\'   ";
            this.chkShowOther.Enabled = false;
            this.chkShowOther.Id = 28;
            this.chkShowOther.Name = "chkShowOther";
            this.chkShowOther.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkShowOther_CheckedChanged);
            // 
            // barMarkerShape
            // 
            this.barMarkerShape.Caption = "    Marker Shape:";
            this.barMarkerShape.Edit = this.lueMarkerShape;
            this.barMarkerShape.Id = 32;
            this.barMarkerShape.Name = "barMarkerShape";
            this.barMarkerShape.Width = 80;
            this.barMarkerShape.EditValueChanged += new System.EventHandler(this.barMarkerShape_EditValueChanged);
            // 
            // lueMarkerShape
            // 
            this.lueMarkerShape.AutoHeight = false;
            this.lueMarkerShape.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueMarkerShape.Name = "lueMarkerShape";
            // 
            // repositoryItemSpinEdit2
            // 
            this.repositoryItemSpinEdit2.AutoHeight = false;
            this.repositoryItemSpinEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemSpinEdit2.Name = "repositoryItemSpinEdit2";
            // 
            // chkPointMarkers
            // 
            this.chkPointMarkers.Caption = "Point Markers";
            this.chkPointMarkers.Id = 35;
            this.chkPointMarkers.Name = "chkPointMarkers";
            this.chkPointMarkers.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkPointMarkers_CheckedChanged);
            // 
            // btnPrintChart
            // 
            this.btnPrintChart.Caption = "Print";
            this.btnPrintChart.Id = 39;
            this.btnPrintChart.LargeWidth = 80;
            this.btnPrintChart.Name = "btnPrintChart";
            this.btnPrintChart.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.btnPrintChart.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPrintChart_ItemClick);
            // 
            // chkValueInLegend
            // 
            this.chkValueInLegend.Caption = "Show Value";
            this.chkValueInLegend.Id = 46;
            this.chkValueInLegend.Name = "chkValueInLegend";
            this.chkValueInLegend.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkValueInLegend_CheckedChanged);
            // 
            // chkPercentInLegend
            // 
            this.chkPercentInLegend.Caption = "Show Percentage";
            this.chkPercentInLegend.Id = 47;
            this.chkPercentInLegend.Name = "chkPercentInLegend";
            this.chkPercentInLegend.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.chkPercentInLegend_CheckedChanged);
            // 
            // pageGeneral
            // 
            this.pageGeneral.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.groupChartType,
            this.groupLegend,
            this.groupGeneral,
            this.groupLineArea,
            this.groupPieDoughnut,
            this.groupTopOptions,
            this.groupCopySave});
            this.pageGeneral.Name = "pageGeneral";
            this.pageGeneral.Text = "General";
            // 
            // groupChartType
            // 
            this.groupChartType.ItemLinks.Add(this.btnChartType);
            this.groupChartType.ItemLinks.Add(this.barColorScheme);
            this.groupChartType.ItemLinks.Add(this.chkPercent);
            this.groupChartType.Name = "groupChartType";
            this.groupChartType.ShowCaptionButton = false;
            this.groupChartType.Text = "Chart Type";
            // 
            // groupLegend
            // 
            this.groupLegend.ItemLinks.Add(this.barLegend);
            this.groupLegend.ItemLinks.Add(this.chkValueInLegend);
            this.groupLegend.ItemLinks.Add(this.chkPercentInLegend);
            this.groupLegend.Name = "groupLegend";
            this.groupLegend.Text = "Legend";
            // 
            // groupGeneral
            // 
            this.groupGeneral.ItemLinks.Add(this.chkSeriesLabelVisible);
            this.groupGeneral.ItemLinks.Add(this.barSeriesLabelPosition);
            this.groupGeneral.ItemLinks.Add(this.barGeneralBarShape);
            this.groupGeneral.Name = "groupGeneral";
            this.groupGeneral.ShowCaptionButton = false;
            this.groupGeneral.Text = "General Config.";
            // 
            // groupLineArea
            // 
            this.groupLineArea.ItemLinks.Add(this.chkPointMarkers);
            this.groupLineArea.ItemLinks.Add(this.barMarkerShape);
            this.groupLineArea.ItemLinks.Add(this.barMarkerSize);
            this.groupLineArea.Name = "groupLineArea";
            this.groupLineArea.ShowCaptionButton = false;
            this.groupLineArea.Text = "Line && Area";
            this.groupLineArea.Visible = false;
            // 
            // groupPieDoughnut
            // 
            this.groupPieDoughnut.ItemLinks.Add(this.barHoleRadius);
            this.groupPieDoughnut.ItemLinks.Add(this.barExplodedPoints);
            this.groupPieDoughnut.ItemLinks.Add(this.barExplodedDist);
            this.groupPieDoughnut.Name = "groupPieDoughnut";
            this.groupPieDoughnut.ShowCaptionButton = false;
            this.groupPieDoughnut.Text = "Pie && Doughnut";
            // 
            // groupTopOptions
            // 
            this.groupTopOptions.ItemLinks.Add(this.chkShowOnlyTop);
            this.groupTopOptions.ItemLinks.Add(this.barShowTop);
            this.groupTopOptions.ItemLinks.Add(this.chkShowOther);
            this.groupTopOptions.Name = "groupTopOptions";
            this.groupTopOptions.ShowCaptionButton = false;
            this.groupTopOptions.Text = "Top Options";
            // 
            // groupCopySave
            // 
            this.groupCopySave.ItemLinks.Add(this.btnPrintChart);
            this.groupCopySave.ItemLinks.Add(this.btnCopyJpg);
            this.groupCopySave.ItemLinks.Add(this.btnSaveJpg);
            this.groupCopySave.Name = "groupCopySave";
            this.groupCopySave.ShowCaptionButton = false;
            this.groupCopySave.Text = "Print, Copy && Save";
            // 
            // lueSeriesBarShape
            // 
            this.lueSeriesBarShape.AutoHeight = false;
            this.lueSeriesBarShape.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueSeriesBarShape.Name = "lueSeriesBarShape";
            // 
            // spinHoleRadius
            // 
            this.spinHoleRadius.AutoHeight = false;
            this.spinHoleRadius.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinHoleRadius.Name = "spinHoleRadius";
            // 
            // repositoryItemLookUpEdit1
            // 
            this.repositoryItemLookUpEdit1.AutoHeight = false;
            this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutRibbon,
            this.layoutGroupChart,
            this.layoutGroupUpdating});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1131, 517);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutRibbon
            // 
            this.layoutRibbon.Control = this.ribbonMain;
            this.layoutRibbon.CustomizationFormText = "layoutRibbon";
            this.layoutRibbon.Location = new System.Drawing.Point(0, 0);
            this.layoutRibbon.MaxSize = new System.Drawing.Size(0, 106);
            this.layoutRibbon.MinSize = new System.Drawing.Size(207, 106);
            this.layoutRibbon.Name = "layoutRibbon";
            this.layoutRibbon.Size = new System.Drawing.Size(1131, 106);
            this.layoutRibbon.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutRibbon.Text = "layoutRibbon";
            this.layoutRibbon.TextSize = new System.Drawing.Size(0, 0);
            this.layoutRibbon.TextToControlDistance = 0;
            this.layoutRibbon.TextVisible = false;
            // 
            // layoutGroupChart
            // 
            this.layoutGroupChart.CustomizationFormText = "layoutGroupChart";
            this.layoutGroupChart.GroupBordersVisible = false;
            this.layoutGroupChart.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutChart});
            this.layoutGroupChart.Location = new System.Drawing.Point(0, 106);
            this.layoutGroupChart.Name = "layoutGroupChart";
            this.layoutGroupChart.Size = new System.Drawing.Size(1131, 265);
            this.layoutGroupChart.Text = "layoutGroupChart";
            // 
            // layoutChart
            // 
            this.layoutChart.Control = this.chartMain;
            this.layoutChart.CustomizationFormText = "layoutChart";
            this.layoutChart.Location = new System.Drawing.Point(0, 0);
            this.layoutChart.Name = "layoutChart";
            this.layoutChart.Size = new System.Drawing.Size(1131, 265);
            this.layoutChart.Text = "layoutChart";
            this.layoutChart.TextSize = new System.Drawing.Size(0, 0);
            this.layoutChart.TextToControlDistance = 0;
            this.layoutChart.TextVisible = false;
            // 
            // layoutGroupUpdating
            // 
            this.layoutGroupUpdating.CustomizationFormText = "layoutGroupUpdating";
            this.layoutGroupUpdating.GroupBordersVisible = false;
            this.layoutGroupUpdating.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem3,
            this.emptySpaceItem4,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutControlItem2,
            this.layoutControlItem1});
            this.layoutGroupUpdating.Location = new System.Drawing.Point(0, 371);
            this.layoutGroupUpdating.Name = "layoutGroupUpdating";
            this.layoutGroupUpdating.Size = new System.Drawing.Size(1131, 146);
            this.layoutGroupUpdating.Text = "layoutGroupUpdating";
            this.layoutGroupUpdating.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(1131, 56);
            this.emptySpaceItem3.Text = "emptySpaceItem3";
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem4";
            this.emptySpaceItem4.Location = new System.Drawing.Point(0, 56);
            this.emptySpaceItem4.MaxSize = new System.Drawing.Size(70, 0);
            this.emptySpaceItem4.MinSize = new System.Drawing.Size(70, 10);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(70, 52);
            this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem4.Text = "emptySpaceItem4";
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 108);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(1131, 38);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            this.emptySpaceItem2.Location = new System.Drawing.Point(1065, 56);
            this.emptySpaceItem2.MaxSize = new System.Drawing.Size(66, 0);
            this.emptySpaceItem2.MinSize = new System.Drawing.Size(66, 10);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(66, 52);
            this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem2.Text = "emptySpaceItem2";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.lblProcessing;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(70, 56);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(995, 24);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.prg;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(70, 80);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 28);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(60, 28);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(995, 28);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "JPEG files|*.jpg|All files|*.*";
            this.saveFileDialog1.InitialDirectory = "C:\\";
            this.saveFileDialog1.Title = "Save Chart as JPEG";
            // 
            // barMarkerSize
            // 
            this.barMarkerSize.Caption = "    Marker Size:";
            this.barMarkerSize.Edit = this.lueMarkerSize;
            this.barMarkerSize.Id = 48;
            this.barMarkerSize.Name = "barMarkerSize";
            this.barMarkerSize.Width = 91;
            this.barMarkerSize.EditValueChanged += new System.EventHandler(this.barMarkerSize_EditValueChanged);
            // 
            // lueMarkerSize
            // 
            this.lueMarkerSize.AutoHeight = false;
            this.lueMarkerSize.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueMarkerSize.Name = "lueMarkerSize";
            // 
            // ucWinChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutMain);
            this.Name = "ucWinChart";
            this.Size = new System.Drawing.Size(1131, 517);
            this.Load += new System.EventHandler(this.WinChart_Load);
            this.EnabledChanged += new System.EventHandler(this.WinChart_EnabledChanged);
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
            this.layoutMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.prg.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gddChartType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.images)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueColorScheme)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueLegend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueGeneralBarShape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesLabelPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinExplodedDist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueHoleRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueExplodedPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueMarkerShape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeriesBarShape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinHoleRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutRibbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutGroupChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutGroupUpdating)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueMarkerSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private DevExpress.XtraCharts.ChartControl chartMain;
		private mgControls.mgLayoutControl layoutMain;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private mgControls.mgDevX_RibbonControl ribbonMain;
		private DevExpress.XtraBars.Ribbon.RibbonPage pageGeneral;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupChartType;
		private DevExpress.XtraLayout.LayoutControlItem layoutChart;
		private DevExpress.XtraLayout.LayoutControlItem layoutRibbon;
		private DevExpress.XtraBars.BarButtonItem btnChartType;
		private DevExpress.XtraBars.Ribbon.GalleryDropDown gddChartType;
		private DevExpress.Utils.ImageCollection images;
		private DevExpress.XtraBars.BarEditItem barColorScheme;
		private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueColorScheme;
		private DevExpress.XtraBars.BarCheckItem chkPercent;
		private DevExpress.XtraBars.BarEditItem barLegend;
		private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueLegend;
		private DevExpress.XtraBars.BarEditItem barGeneralBarShape;
		private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueGeneralBarShape;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupGeneral;
		private DevExpress.XtraBars.BarEditItem barSeriesSource;
		private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueSeriesSource;
		private DevExpress.XtraBars.BarEditItem barSeriesGroup;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueSeriesGroup;
		private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueSeriesBarShape;
		private DevExpress.XtraBars.BarCheckItem chkSeriesLabelVisible;
		private DevExpress.XtraBars.BarEditItem barSeriesLabelPosition;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueSeriesLabelPosition;
		private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit spinHoleRadius;
		private DevExpress.XtraBars.BarEditItem barExplodedDist;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit spinExplodedDist;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupPieDoughnut;
        private DevExpress.XtraBars.BarEditItem barHoleRadius;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueHoleRadius;
        private DevExpress.XtraBars.BarEditItem barExplodedPoints;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueExplodedPoints;
        private DevExpress.XtraBars.BarButtonItem btnCopyJpg;
        private DevExpress.XtraBars.BarButtonItem btnSaveJpg;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupCopySave;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private DevExpress.XtraEditors.LabelControl lblProcessing;
		private DevExpress.XtraEditors.ProgressBarControl prg;
		private DevExpress.XtraLayout.LayoutControlGroup layoutGroupChart;
		private DevExpress.XtraLayout.LayoutControlGroup layoutGroupUpdating;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarCheckItem chkShowOnlyTop;
        private DevExpress.XtraBars.BarEditItem barShowTop;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraBars.BarCheckItem chkShowOther;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupTopOptions;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupLineArea;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit1;
        private DevExpress.XtraBars.BarEditItem barMarkerShape;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueMarkerShape;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit2;
        private DevExpress.XtraBars.BarCheckItem chkPointMarkers;
        private DevExpress.XtraBars.BarButtonItem btnPrintChart;
        private DevExpress.XtraBars.BarCheckItem chkValueInLegend;
        private DevExpress.XtraBars.BarCheckItem chkPercentInLegend;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup groupLegend;
        private DevExpress.XtraBars.BarEditItem barMarkerSize;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lueMarkerSize;
    }
}
