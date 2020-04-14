using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

using mgWinChart.Helpers;
using mgWinChart.Interfaces;
using mgWinChart.SerializationExample;
using System.Collections;

using mgModel;

namespace mgWinChart.usercontrols
{
    /// <summary>
    /// A user control for quickly setting up a good-looking chart.
    /// </summary>
    /// <remarks>
    /// When a DataSource is assigned to the WinChart control, it defaults to using the first available column as the argument column 
    /// (the X-Axis values that the other series will be charted against).  To use a different column, assign it's name to the 
    /// PrimaryXAxisColumnName property.   
    /// </remarks>
    /// <example>
    /// To set up a simple WinChart control (the "chart" variable):
    /// <code>
    /// DataTable table = new DataTable();
    /// table.Columns.Add(“Year”, typeof(System.String));
    /// table.Columns.Add(“Income”, typeof(System.Decimal));
    /// table.Columns.Add(“Expenditures”, typeof(System.Decimal));
    /// table.Rows.Add(“2006”, 2382234.00, 2481972.76);
    /// table.Rows.Add(“2007”, 2789035.12, 2765513.90);
    /// table.Rows.Add(“2008”, 2868504.00, 2618116.66);
    /// table.Rows.Add(“2009”, 2768012.31, 2529904.66);
    /// table.Rows.Add(“2010”, 2178323.29, 2948991.11);
    ///
    /// chart.DataSource = table;
    /// chart.AddSeries(“Income”);
    /// chart.AddSeries(“Expenditures”);
    /// chart.Render();
    /// </code>
    /// To use custom display names for the series:
    /// <code>
    /// DataTable table = new DataTable();
    /// table.Columns.Add(“sYearAsString”, typeof(System.String));
    /// table.Columns.Add(“mYearlyIncome”, typeof(System.Decimal));
    /// table.Columns.Add(“mYearlyExpenditure”, typeof(System.Decimal));
    /// table.Rows.Add(“2006”, 2382234.00, 2481972.76);
    /// table.Rows.Add(“2007”, 2789035.12, 2765513.90);
    /// table.Rows.Add(“2008”, 2868504.00, 2618116.66);
    /// table.Rows.Add(“2009”, 2768012.31, 2529904.66);
    /// table.Rows.Add(“2010”, 2178323.29, 2948991.11);
    ///
    /// chart.DataSource = table;
    /// chart.AddSeries(“Income”, "mYearlyIncome");
    /// chart.AddSeries(“Expenditures”, "mYearlyExpenditure");
    /// chart.Render();
    /// </code>
    /// To use a different column for the X-Axis:
    /// <code>
    /// DataTable table = new DataTable();
    /// table.Columns.Add("Region", typeof(System.String));
    /// table.Columns.Add(“Year”, typeof(System.String));
    /// table.Columns.Add(“Income”, typeof(System.Decimal));
    /// table.Columns.Add(“Expenditures”, typeof(System.Decimal));
    /// table.Rows.Add("New England", “2006”, 2382234.00, 2481972.76);
    /// table.Rows.Add("New England", “2007”, 2789035.12, 2765513.90);
    /// table.Rows.Add("New England", “2008”, 2868504.00, 2618116.66);
    /// table.Rows.Add("New England", “2009”, 2768012.31, 2529904.66);
    /// table.Rows.Add("New England", “2010”, 2178323.29, 2948991.11);
    ///
    /// // This form has a WinChart control named "chart".
    /// chart.DataSource = table;
    /// chart.PrimaryXAxisColumnName = "Year";
    /// chart.AddSeries(“Income”);
    /// chart.AddSeries(“Expenditures”);
    /// chart.Render();
    /// </code>
    /// </example>
    public partial class ucWinChart : UserControl
    {

        #region Private Variables

        private bool _isLoaded = false;
        private bool _enabled = true;
        private object _datasource = null;
        private string _primaryXAxisColumnName = string.Empty;
        private string _secondaryXAxisColumnName = string.Empty;

        private bool _secondaryXAxisVisible = false;

        private List<WinChartSeries> _series = new List<WinChartSeries>();
        //private Dictionary<string, string> _seriesSources = new Dictionary<string, string>();
        private ssChartPredicate _filter = new ssCompoundChartPredicate(PredicateConjunction.And);

        private List<SeriesUI> _seriesUIs = new List<SeriesUI>();

        private VisualSettingsObject _visualSettings = new VisualSettingsObject();
        private Palette _defaultPalette = Palettes.Apex;
        private ViewType _oldViewType = ViewType.Bar;

        private List<string> _seriesForSecondAxisX = new List<string>();
        private List<string> _seriesForSecondAxisY = new List<string>();

        // Ribbon Controls
        private bool _rcColorScheme = true;
        private bool _rcLegend = true;
        private bool _rcValueAsPercent = true;

        private Dictionary<UpdatableElement, bool> _dirtyElements = new Dictionary<UpdatableElement, bool>();
        private List<PropertyDescriptor> _dirtyProperties = new List<PropertyDescriptor>();
        private bool _holdCleaning = false;

        //private bool _progressVisible = false;
        private decimal _progressValue = 0;
        private Timer _timer = null;

        // A set of working days for use in the chart by default
        private Weekday _workingDays =
            //Weekday.Sunday |
            Weekday.Monday |
            Weekday.Tuesday |
            Weekday.Wednesday |
            Weekday.Thursday |
            Weekday.Friday;
        //| Weekday.Saturday;

        #endregion Private Variables

        #region Constructors

        public ucWinChart()
        {
            InitializeComponent();

            mgInit();
        }

        /// <summary>
        /// This version of the constructor collects enough information to draw the chart control.
        /// </summary>
        /// <param name="dataSource">The System.Data.DataTable object used to populate the chart.</param>
        /// <param name="xAxisColumnName">The name of the DataTable column the chart should use as the X axis.</param>
        /// <param name="seriesNames">A list of the column names the chart should populate Series objects from.  Creates one Series for each column.</param>
        /// <param name="chartType">The type of chart that should be drawn.  Default is "Bar."</param>
        public ucWinChart(object dataSource, string xAxisColumnName, List<string> seriesNames, ViewType chartType = ViewType.Bar)
        {
            InitializeComponent();

            mgInit();

            DataSource = dataSource;
            PrimaryXAxisColumnName = xAxisColumnName;

            foreach (string seriesName in seriesNames)
            {
                this.AddSeries(seriesName);
            }

            ChartType = chartType;

            Render();
        }

        private void mgInit()
        {
            //this.LabelsVisible = true;
            //cboViewType.Items.AddRange(ViewDictionary.Keys.ToArray());
            //cboColorScheme.Items.AddRange(AppearanceDictionary.Values.ToArray());
            this.SecondaryAxisY.Visible = false;

            List<UpdatableElement> allElements =
                new List<UpdatableElement>(Enum.GetValues(typeof(UpdatableElement)).Cast<UpdatableElement>());
            foreach (UpdatableElement element in allElements)
            {
                _dirtyElements.Add(element, false);
            }
        }

        private void WinChart_Load(object sender, EventArgs e)
        {
            // Init the chart types
            InitChartTypes();

            #region Color Schemes

            // Load the color schemes
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (KeyValuePair<Palette, string> kvp in AppearanceDictionary)
            {
                dt.Rows.Add(new object[] { kvp.Key.Name.ToString(), kvp.Value });
            }

            lueColorScheme.Columns.Clear();
            lueColorScheme.DataSource = null;
            lueColorScheme.DataSource = dt;
            lueColorScheme.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueColorScheme.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueColorScheme.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            barColorScheme.EditValue = (dt.Rows[0]["ValueField"] != DBNull.Value ?
                dt.Rows[0]["ValueField"].ToString() : "");

            #endregion Color Schemes

            #region Legend Positions

            // Set up the legend positions
            dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(LegendPosition)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueLegend.Columns.Clear();
            lueLegend.DataSource = null;
            lueLegend.DataSource = dt;
            lueLegend.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueLegend.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueLegend.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            // Select the first value 
            barLegend.EditValue = (dt.Rows[0]["ValueField"] != DBNull.Value ?
                dt.Rows[0]["ValueField"].ToString() : "");

            #endregion Legend Positions

            #region Bar Shapes

            // Set up the bar shapes
            dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(Bar3DModel)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueGeneralBarShape.Columns.Clear();
            lueGeneralBarShape.DataSource = null;
            lueGeneralBarShape.DataSource = dt;
            lueGeneralBarShape.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueGeneralBarShape.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueGeneralBarShape.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            #endregion Bar Shapes

            #region Label Position

            SetBarLabelPositionOptions(false);

            #endregion Label Position

            #region Line/Area Marker Options

            SetMarkerShapeOptions();
            SetMarkerSizeOptions();

            #endregion Line/Area Marker Options

            #region Hole Radius Options

            // Set up the legend positions
            dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(HoleRadius)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueHoleRadius.Columns.Clear();
            lueHoleRadius.DataSource = null;
            lueHoleRadius.DataSource = dt;
            lueHoleRadius.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueHoleRadius.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueHoleRadius.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            // Select the first value 
            barHoleRadius.EditValue = this.HoleRadius.ToString();

            #endregion Hole Radius Options

            #region Exploded Points

            SetExplodedPointsOptions();
            barExplodedDist.EditValue = this.ExplodeDistance.ToString();

            #endregion Exploded Points

            #region Ribbon Controls

            if (_rcColorScheme)
            {
                barColorScheme.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                barColorScheme.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            if (_rcLegend)
            {
                barLegend.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                barLegend.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            if (_rcValueAsPercent)
            {
                chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            #endregion Ribbon Controls

            _isLoaded = true;
        }

        /// <summary>
        /// Initialize the chart types on the form.
        /// </summary>
        /// <returns>Whether the current view type is included in the new menu.</returns>
        private bool InitChartTypes()
        {
            gddChartType.BeginUpdate();

            // Build a datatable for the objects that we can use to build the collection
            DataTable dt = new DataTable();
            dt.Columns.Add("GroupName", typeof(System.String));
            dt.Columns.Add("Icon", typeof(System.Drawing.Image));
            dt.Columns.Add("Caption", typeof(System.String));
            dt.Columns.Add("Hint", typeof(System.String));
            dt.Columns.Add("Tag", typeof(System.String));

            string groupName = string.Empty;

            // Bar Views
            groupName = "Bar Views";
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoBarSideBySide_Icon, "Bar", "Bar Chart", ViewType.Bar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoBarStacked_Icon, "Stacked Bar", "Stacked Bar Chart", ViewType.StackedBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoBarStackedFull_Icon, "Full-Stacked Bar", "Full-Stacked\r\nBar Chart", ViewType.FullStackedBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSideBySideBarStacked_Icon, "Stacked Bar Side-by-Side", "Stacked Bar\r\nSide-by-Side", ViewType.SideBySideStackedBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSideBySideBarStackedFull_Icon, "Full-Stacked Bar Side-by-Side", "Full-Stacked Bar\r\nSide-by-Side", ViewType.SideBySideFullStackedBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoBarSideBySide3D_Icon, "3D Bar", "3D Bar Chart", ViewType.Bar3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoManhattanBar_Icon, "3D Manhattan Bar", "3D Manhattan\r\nBar Chart", ViewType.ManhattanBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoBarStacked3D_Icon, "3D Stacked Bar", "3D Stacked\r\nBar Chart", ViewType.StackedBar3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoBarStackedFull3D_Icon, "3D Full-Stacked Bar", "3D Full-Stacked\r\nBar Chart", ViewType.FullStackedBar3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSideBySideBarStacked3D_Icon, "3D Stacked Bar Side-by-Side", "3D Stacked\r\nBar Chart", ViewType.SideBySideStackedBar3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSideBySideBarStackedFull3D_Icon, "3D Full-Stacked Bar Side-by-Side", "3D Full-Stacked Bar\r\nSide-by-Side", ViewType.SideBySideFullStackedBar3D.ToString() });

            // Point/Line Views
            groupName = "Point/Line Views";
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoPoint_Icon, "Point", "Point", ViewType.Point.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoLine_Icon, "Line", "Line", ViewType.Line.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoScatterLine_Icon, "Scatter Line", "Scatter Line", ViewType.ScatterLine.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoStepLine_Icon, "Step Line", "Step Line", ViewType.StepLine.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSpline_Icon, "Spline", "Spline", ViewType.Spline.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoLine3D_Icon, "3D Line", "3D Line", ViewType.Line3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoStepLine3D_Icon, "3D Step Line", "3D Step Line", ViewType.StepLine3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSpline3D_Icon, "3D Spline", "3D Spline", ViewType.Spline3D.ToString() });

            // Area Views
            groupName = "Area Views";
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoAreas_Icon, "Area", "Area", ViewType.Area.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoAreaStacked_Icon, "Stacked Area", "Stacked Area", ViewType.StackedArea.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoAreaStackedFull_Icon, "Full-Stacked Area", "Full-Stacked\r\nArea", ViewType.FullStackedArea.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSplineArea_Icon, "Spline Area", "Spline Area", ViewType.SplineArea.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSplineAreaStacked_Icon, "Stacked Spline Area", "Stacked Spline\r\nArea", ViewType.StackedSplineArea.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSplineAreaStackedFull_Icon, "Full-Stacked Spline Area", "Full-Stacked\r\nSpline Area", ViewType.FullStackedSplineArea.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoArea3D_Icon, "3D Area", "3D Area", ViewType.Area3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoAreaStacked3D_Icon, "3D Stacked Area", "3D Stacked Area", ViewType.StackedArea3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoAreaStackedFull3D_Icon, "3D Full-Stacked Area", "3D Full-Stacked Area", ViewType.FullStackedArea3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSplineArea3D_Icon, "3D Spline Area", "3D Spline Area", ViewType.SplineArea3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSplineAreaStacked3D_Icon, "3D Stacked Spline Area", "3D Stacked\r\nSpline Area", ViewType.StackedSplineArea3D.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSplineAreaStackedFull3D_Icon, "3D Full-stacked Spline Area", "3D Full-stacked\r\nSpline Area", ViewType.FullStackedSplineArea3D.ToString() });

            // Pie/Doughnut and Funnel views don't support values below zero, so conditionally omit them.
            bool valuesAreSafe = true;
            foreach (Series series in this.chartMain.Series)
            {
                foreach (SeriesPoint point in series.Points)
                {
                    foreach (double v in point.Values)
                    {
                        if (v < 0)
                        {
                            valuesAreSafe = false;
                            break;
                        }
                    }
                    if (!valuesAreSafe)
                    {
                        break;
                    }
                }
                if (!valuesAreSafe)
                {
                    break;
                }
            }

            if (valuesAreSafe)
            {
                // Pie/Doughnut Views
                groupName = "Pie/Doughnut Views";
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoPie_Icon, "Pie", "Pie", ViewType.Pie.ToString() });
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoDoughnut_Icon, "Doughnut", "Doughnut", ViewType.Doughnut.ToString() });
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoPie3D_Icon, "3D Pie", "3D Pie", ViewType.Pie3D.ToString() });
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoDoughnut3D_Icon, "3D Doughnut", "3D Doughnut", ViewType.Doughnut3D.ToString() });

                // Funnel Views
                groupName = "Funnel Views";
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoFunnel_Icon, "Funnel", "Funnel", ViewType.Funnel.ToString() });
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoFunnel3D_Icon, "3D Funnel", "3D Funnel", ViewType.Funnel3D.ToString() });
            }

            // Radar/Polar Views
            groupName = "Radar/Polar Views";
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoRadarPoint_Icon, "Radar Point", "Radar Point", ViewType.RadarPoint.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoRadarLine_Icon, "Radar Line", "Radar Line", ViewType.RadarLine.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoRadarArea_Icon, "Radar Area", "Radar Area", ViewType.RadarArea.ToString() });

            // Polar view require a certain value range for arguments, so conditionally omit them.
            valuesAreSafe = true;
            decimal d = 0m;
            foreach (Series series in this.chartMain.Series)
            {
                foreach (SeriesPoint point in series.Points)
                {
                    if (!(Decimal.TryParse(point.Argument, out d) && d >= 0 && d <= 360))
                    {
                        valuesAreSafe = false;
                        break;
                    }
                }
                if (!valuesAreSafe)
                {
                    break;
                }
            }

            if (valuesAreSafe)
            {
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoPolarPoint_Icon, "Polar Point", "Polar Point", ViewType.PolarPoint.ToString() });
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoPolarLine_Icon, "Polar Line", "Polar Line", ViewType.PolarLine.ToString() });
                //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoPolarArea_Icon, "Polar Area", "Polar Area", ViewType.PolarArea.ToString() });
            }

            //// Range Views
            //groupName = "Range Views";
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoRangeBar_Icon, "Range Bar", "Range Bar", ViewType.RangeBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSideBySideRangeBar_Icon, "Range Bar Side-by-Side", "Range Bar\r\nSide-by-Side", ViewType.SideBySideRangeBar.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoGantt_Icon, "Gantt", "Gantt", ViewType.Gantt.ToString() });
            //dt.Rows.Add(new object[] { groupName, global::_BaseWinProject.Win.Common.WinChart.resChart.ChartDemoSideBySideGantt_Icon, "Gantt Side-by-Side", "Gantt\r\nSide-by-Side", ViewType.SideBySideGantt.ToString() });

            // Remove all the elements, set up the text options, and go from there
            gddChartType.Gallery.Groups.Clear();
            gddChartType.Gallery.ShowItemText = true;

            bool containsCurrentViewType = false;
            string lastItemKey = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                if (lastItemKey.ToLower() != row["GroupName"].ToString().ToLower())
                {
                    GalleryItemGroup group = new GalleryItemGroup();
                    group.Caption = row["GroupName"].ToString();
                    gddChartType.Gallery.Groups.Add(group);
                }
                lastItemKey = row["GroupName"].ToString();

                // Add the item
                GalleryItem item = new GalleryItem();
                item.Image = (Image)row["Icon"];
                //item.Caption = row["Caption"].ToString();
                item.Description = row["Hint"].ToString();
                item.Hint = row["Hint"].ToString();
                item.Tag = row["Tag"].ToString();
                gddChartType.Gallery.Groups[gddChartType.Gallery.Groups.Count - 1].Items.Add(item);

                // Prepare the button.
                if (item.Tag.ToString().Equals(this.ChartType.ToString()))
                {
                    containsCurrentViewType = true;
                    btnChartType.LargeGlyph = item.Image;
                    btnChartType.Caption = item.Description;
                    btnChartType.Hint = item.Hint;
                }
            }

            gddChartType.EndUpdate();

            ChartGalleryEvents e = new ChartGalleryEvents(
                gddChartType, btnChartType.LargeGlyph, btnChartType.Caption, btnChartType.Hint);
            OnChartGalleryUpdated(e);

            return containsCurrentViewType;
        }

        #endregion Constructors

        #region Public Properties

        #region Access Control

        /// <summary>
        /// Returns the WinChart's DevExpress.XtraCharts.ChartControl object.
        /// </summary>
        public DevExpress.XtraCharts.ChartControl DevXChart
        {
            get
            {
                return chartMain;
            }
        }

        /// <summary>
        /// Allows the control to be enabled or not
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether the element is enabled.")]
        public new bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                // Handle the timer depending on what's done to the enabled
                if (!this.DesignMode)
                {
                    if (_enabled)
                    {
                        // Disable the timer
                        if (_timer != null)
                        {
                            _timer.Tick -= new EventHandler(_timer_Tick);
                            _timer.Enabled = false;
                        }
                        _timer = null;
                        Application.DoEvents();
                    }
                    else
                    {
                        // Reset the progress
                        prg.Properties.Minimum = 0;
                        prg.Properties.Maximum = 100;
                        _progressValue = 10;
                        prg.Position = (int)_progressValue;

                        // Enable the timer
                        _timer = new Timer();
                        _timer.Interval = 500;
                        _timer.Enabled = true;
                        _timer.Tick += new EventHandler(_timer_Tick);
                    }
                }

                // Turn on/off the controls
                layoutGroupUpdating.Visibility = (_enabled ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always);
                layoutGroupChart.Visibility = (_enabled ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
            }
        }

        #endregion Access Control

        #region Data Support

        /// <summary>
        /// The System.Data.DataTable used to populate the chart.
        /// </summary>
        [Category("Data")]
        [Description("The DataTable used to populate the chart with data.")]
        public object DataSource
        {
            get
            {
                return _datasource;
            }
            set
            {
                _datasource = value;

                // Make sure the XAxisColumn still exists.
                if (this.DataSourceAsTable != null &&
                    (this.PrimaryXAxisColumnName == null || !this.DataSourceAsTable.Columns.Contains(this.PrimaryXAxisColumnName)))
                {
                    if (this.DataSourceAsTable.Columns.Count > 0)
                    {
                        this.PrimaryXAxisColumnName = this.DataSourceAsTable.Columns[0].ColumnName;
                    }
                    else
                    {
                        this.PrimaryXAxisColumnName = string.Empty;
                    }
                }

                // Make sure the secondary exists as well.
                if (this.DataSourceAsTable != null &&
                    (this.SecondaryXAxisColumnName == null || !this.DataSourceAsTable.Columns.Contains(this.SecondaryXAxisColumnName)))
                {
                    this.SecondaryXAxisColumnName = string.Empty;
                }
            }
        }

        /// <summary>
        /// The DataSource converted to a DataTable object.  
        /// Returns null if the DataSource cannot be converted.
        /// </summary>
        public DataTable DataSourceAsTable
        {
            get
            {
                if (this.DataSource is DataTable)
                {
                    return (DataTable)DataSource;
                }
                if (this.DataSource is IClassGenClassGenerated)
                {
                    return ((IClassGenClassGenerated)DataSource).ToDataTable();
                }
                return null;
            }
        }

        /// <summary>
        /// A string representation of the column name used as the chart's X axis.
        /// </summary>
        [Category("Data")]
        [Description("The name of the column to use as the chart's X axis.")]
        public string PrimaryXAxisColumnName
        {
            get
            {
                return _primaryXAxisColumnName;
            }
            set
            {
                if (this.DataSourceAsTable != null && DataSourceAsTable.Columns.Contains(value))
                {
                    _primaryXAxisColumnName = value;
                }
                else
                {
                    //throw new error when we find the right one.
                }
            }
        }

        /// <summary>
        /// A string representation of the column name used as the chart's secondary X axis.
        /// </summary>
        public string SecondaryXAxisColumnName
        {
            get
            {
                return _secondaryXAxisColumnName;
            }
            set
            {
                if (this.DataSourceAsTable != null && DataSourceAsTable.Columns.Contains(value))
                {
                    _secondaryXAxisColumnName = value;
                }
                else
                {
                    //throw new error when we find the right one.
                }
            }
        }

        /// <summary>
        /// The Series in this WinChart.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<WinChartSeries> Series
        {
            get
            {
                return _series;
            }
            set
            {
                _series = value;
            }
        }

        /// <summary>
        /// DEPRECATED:
        /// A list of column names (keys) and the Series' names to display (values).
        /// </summary>
        [Category("Data")]
        [Description("The column names and the names of each series.")]
        public Dictionary<string, string> SeriesSources
        {
            get
            {
                Dictionary<string, string> seriesSources = new Dictionary<string, string>();
                foreach (WinChartSeries series in this.Series)
                {
                    seriesSources.Add(series.Name, series.DataSourceColumnName);
                }
                return seriesSources;
            }
            set
            {
                this.Series.Clear();
                foreach (KeyValuePair<string, string> pair in value)
                {
                    this.AddSeries(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// DEPRECATED:
        /// A list of the series' names.  Corresponds to the column names used to populate the series'.
        /// </summary>
        [Category("Data")]
        [Description("The names of the columns that populate each series.")]
        public List<string> SeriesNames
        {
            get
            {
                return new List<string>(this.SeriesSources.Keys);
            }
            set
            {
                this.Series.Clear();
                foreach (string name in value)
                {
                    this.AddSeries(name);
                }
            }
        }

        #endregion

        #region IChartingBase Support

        /// <summary>
        /// The title to display at the top of the chart.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("The title to display in the chart.")]
        public string ChartTitle
        {
            get
            {
                return _visualSettings.ChartTitle;
            }
            set
            {
                _visualSettings.ChartTitle = value;
                this.MarkAsDirty(UpdatableElement.ChartTitle);
            }
        }

        /// <summary>
        /// The type of chart.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(ViewType.Bar)]
        [Description("The type of chart.")]
        public ViewType ChartType
        {
            get
            {
                return _visualSettings.ViewType;
            }
            set
            {
                if (_visualSettings.ViewType != value)
                {
                    WinChartTypeChangeEventArgs e = new WinChartTypeChangeEventArgs(_visualSettings.ViewType, value);
                    this.OnBeforeChartTypeChange(e);
                    if (e.ShouldChange)
                    {
                        _oldViewType = _visualSettings.ViewType;
                        _visualSettings.ViewType = e.NewType;
                        this.MarkAsDirty(UpdatableElement.ChartType, UpdatableElement.ValueAsPercent,
                            UpdatableElement.BarLabelPosition, UpdatableElement.PieLabelPosition);
                    }
                }
            }
        }

        /// <summary>
        /// If true, series labels display their values as a percentage of the whole.  Not available for all chart types.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Whether labels display their values as percentages of the whole.  Not available for all chart types.")]
        public bool ValueAsPercent
        {
            get
            {
                return _visualSettings.ValueAsPercent;
            }
            set
            {
                _visualSettings.ValueAsPercent = value;
                this.MarkAsDirty(UpdatableElement.ValueAsPercent);
            }
        }

        /// <summary>
        /// The chart's color scheme.  NOTE: Currently, some of the options modify the old value instead of replacing it.
        /// </summary>
        [Category("Appearance")]
        //[DefaultValue(Palette.NATURE_COLORS)]
        [Description("The chart's color scheme.  NOTE: Currently, some options don't fully replace the prior scheme.")]
        public Palette ChartingAppearance
        {
            get
            {
                return _visualSettings.Palette;
            }
            set
            {
                if (value == null)
                {
                    _visualSettings.Palette = _defaultPalette;
                }
                else
                {
                    _visualSettings.Palette = value;
                }

                this.MarkAsDirty(UpdatableElement.ChartAppearance);
            }
        }

        /// <summary>
        /// Whether the labels for each Series are visible by default.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether series' labels are shown.")]
        public bool LabelsVisible
        {
            get
            {
                return _visualSettings.LabelsVisible;
            }
            set
            {
                bool newValue = value;
                if (value && this.DataSource != null && (GetNumberOfLabels() > 250))
                {
                    DialogResult result = MessageBox.Show(this.FindForm(),
                        string.Format("It will take a long time to draw {0} labels -- this program might appear unresponsive " +
                        "during that time.  Continue anyway?", GetNumberOfLabels()),
                        "Labels Visible", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    newValue = (result == DialogResult.Yes);
                }
                _visualSettings.LabelsVisible = newValue;
                this.MarkAsDirty(UpdatableElement.LabelsVisible);
            }
        }

        #endregion IChartingBase Support

        #region Legend

        /// <summary>
        /// Where in the chart the legend appears.  Choose "None" to hide the legend.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(LegendPosition.None)]
        [Description("Where to display the chart's legend.  Select None to hide the legend.")]
        public LegendPosition LegendPositioning
        {
            get
            {
                return _visualSettings.LegendPosition;
            }
            set
            {
                if (_visualSettings.LegendPosition != value)
                {
                    _visualSettings.LegendPosition = value;
                    MarkAsDirty(UpdatableElement.Legend);
                }
            }
        }

        /// <summary>
        /// Whether to include the value for each pie slice in the legend.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Whether to include the value for each pie slice in the legend.")]
        public bool ShowValueInLegend
        {
            get { return chkValueInLegend.Checked; }
            set { chkValueInLegend.Checked = value; }
        }

        /// <summary>
        /// Whether to include the percentage for each pie slice in the legend.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Whether to include the percentage for each pie slice in the legend.")]
        public bool ShowPercentageInLegend
        {
            get { return chkPercentInLegend.Checked; }
            set { chkPercentInLegend.Checked = value; }
        }

        #endregion Legend

        #region IAreaChart Support

        /// <summary>
        /// The angle of the line from the area or point series value to its label. 
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(LabelAngle.Angle_000)]
        [Description("The angle from each area or point series value to its label.")]
        [ReadOnly(true)]
        public LabelAngle LabelAngle
        {
            get
            {
                return _visualSettings.LabelAngle;
            }
            set
            {
                _visualSettings.LabelAngle = value;
                MarkAsDirty(UpdatableElement.AreaLineLabelAngle);
            }
        }

        /// <summary>
        /// The shape used to mark values in area and point series.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Helpers.MarkerKind.Circle)]
        [Description("The shape used to mark values in area and point series.")]
        [ReadOnly(true)]
        public mgWinChart.Helpers.MarkerKind MarkerKind
        {
            get
            {
                return _visualSettings.MarkerKind;
            }
            set
            {
                _visualSettings.MarkerKind = value;
                MarkAsDirty(UpdatableElement.MarkerKind);
            }
        }

        /// <summary>
        /// The size of area and point series markers.  Scale is similar to font scale. 
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(MarkerSize.Size_08)]
        [Description("The size of each marker in area and point series, measured in font sizes.")]
        [ReadOnly(true)]
        public MarkerSize MarkerSize
        {
            get
            {
                return _visualSettings.MarkerSize;
            }
            set
            {
                if (_visualSettings.MarkerSize != value)
                {
                    _visualSettings.MarkerSize = value;
                    this.MarkAsDirty(UpdatableElement.MarkerSize);
                }
            }
        }

        /// <summary>
        /// Whether area and point series markers should be visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether area and point series markers are visible.")]
        [ReadOnly(true)]
        public bool ShowMarkers
        {
            get
            {
                return _visualSettings.ShowMarkers;
            }
            set
            {
                if (_visualSettings.ShowMarkers != value)
                {
                    _visualSettings.ShowMarkers = value;
                    this.MarkAsDirty(UpdatableElement.ShowMarkers);
                }
            }
        }

        #endregion IAreaChart Support

        #region IBarChart Support

        /// <summary>
        /// The 3-dimensional shape to use when drawing values in a bar graph.  Options are Box, Cylinder, Pyramid, and Cone. 
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(Bar3DModel.Box)]
        [Description("The default shape to use for 3D bar graphs.")]
        public Bar3DModel ModelType
        {
            get
            {
                return _visualSettings.ModelType;
            }
            set
            {
                if (_visualSettings.ModelType != value)
                {
                    _visualSettings.ModelType = value;
                    this.MarkAsDirty(UpdatableElement.ModelType);
                }
            }
        }

        /// <summary>
        /// Where bar series labels are displayed.  Options are Top, Top Inside, Center, and Bottom Inside.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(BarSeriesLabelPosition.Top)]
        [Description("The default location for bar graph labels.")]
        public BarSeriesLabelPosition BarLabelPosition
        {
            get
            {
                return _visualSettings.BarLabelPosition;
            }

            set
            {
                if (_visualSettings.BarLabelPosition != value)
                {
                    _visualSettings.BarLabelPosition = value;
                    this.MarkAsDirty(UpdatableElement.BarLabelPosition);
                }
            }
        }

        #endregion IBarChart Support

        #region IPieDoughnutChart Support

        /// <summary>
        /// The distance, in percent of the pie chart's radius, to offset pie and doughnut series values that are "exploded."
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(5)]
        [Description("The distance exploded pie/doughnut segments are removed from the center (in percentage of the chart's radius).")]
        public uint ExplodeDistance
        {
            get
            {
                return _visualSettings.ExplodeDistance;
            }
            set
            {
                if (_visualSettings.ExplodeDistance != value)
                {
                    if (value > 100)
                    {
                        _visualSettings.ExplodeDistance = 100;
                    }
                    else
                    {
                        _visualSettings.ExplodeDistance = value;
                    }
                    this.MarkAsDirty(UpdatableElement.ExplodeDistance);
                }
            }
        }

        /// <summary>
        /// Which pie or doughnut values are "exploded."
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(PieExplodeMode.None)]
        [Description("Which points to explode.")]
        public PieExplodeMode ExplodeMode
        {
            get
            {
                return _visualSettings.ExplodeMode;
            }
            set
            {
                bool changed = false;
                if (_visualSettings.ExplodeMode != value)
                {
                    changed = true;
                }
                _visualSettings.ExplodeMode = value;
                if (changed)
                {
                    this.MarkAsDirty(UpdatableElement.ExplodeMode);
                }
            }
        }

        /// <summary>
        /// Which argument to explode if the ExplodeMode is set to UsePoints.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Which single argument to explode if the ExplodeMode is set to UsePoints")]
        public string ArgumentExploded
        {
            get
            {
                return _visualSettings.ArgumentExploded;
            }
            set
            {
                _visualSettings.ArgumentExploded = value;
                this.MarkAsDirty(UpdatableElement.ExplodeMode);
            }
        }

        /// <summary>
        /// How and where pie and doughnut series values display labels.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(PieSeriesLabelPosition.Radial)]
        [Description("The default location and orientation for pie/doughnut chart labels.")]
        public PieSeriesLabelPosition PieLabelPosition
        {
            get
            {
                return _visualSettings.PieLabelPosition;
            }
            set
            {
                if (_visualSettings.PieLabelPosition != value)
                {
                    _visualSettings.PieLabelPosition = value;
                    this.MarkAsDirty(UpdatableElement.PieLabelPosition);
                }
            }
        }

        /// <summary>
        /// The radius of the hole in the center of a doughnut chart, measured in percent of the chart's radius.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(HoleRadius.Radius_050)]
        [Description("The radius of the hole in the center of doughnut charts, as a percentage of the chart's radius.")]
        public HoleRadius HoleRadius
        {
            get
            {
                return _visualSettings.HoleRadius;
            }
            set
            {
                _visualSettings.HoleRadius = value;
                this.MarkAsDirty(UpdatableElement.HoleRadius);
            }
        }

        #endregion IPieDoughnutChart Support

        #region TopN Support

        /// <summary>
        /// Whether this chart shows all of the points in each series, as opposed to the top N points.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Whether this chart shows all of the points in each series, as opposed to the top N points.")]
        public bool ShowOnlyTopRecords
        {
            get
            {
                return _visualSettings.ShowOnlyTopRecords;
            }
            set
            {
                _visualSettings.ShowOnlyTopRecords = value;
                this.MarkAsDirty(UpdatableElement.ShowOnlyTopRecords);
            }
        }

        /// <summary>
        /// The number of records to show, counting from the greatest, before grouping everything else into 'Other'.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(5)]
        [Description("The number of records to show, counting from the greatest, before grouping everything else into 'Other'.")]
        public uint NumberOfRecordsToShow
        {
            get
            {
                return _visualSettings.NumberOfRecordsToShow;
            }
            set
            {
                _visualSettings.NumberOfRecordsToShow = value;
                this.MarkAsDirty(UpdatableElement.NumberOfRecordsToShow);
            }
        }

        /// <summary>
        /// Whether this chart shows the 'Other' value in Top N mode.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether this chart shows the 'Other' value in Top N mode.")]
        public bool ShowOther
        {
            get
            {
                return _visualSettings.ShowOther;
            }
            set
            {
                _visualSettings.ShowOther = value;
                this.MarkAsDirty(UpdatableElement.ShowOther);
            }
        }

        #endregion TopN Support

        #region Axes

        /// <summary>
        /// The Primary X Axis.
        /// </summary>
        [ReadOnly(true)]
        public AxisX PrimaryAxisX
        {
            get
            {
                if (this.chartMain.Diagram is XYDiagram)
                {
                    return ((XYDiagram)this.chartMain.Diagram).AxisX;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The chart's secondary X-axis.
        /// </summary>
        [Category("Behavior")]
        [Description("The chart's secondary X-axis.")]
        [ReadOnly(true)]
        public SecondaryAxisX SecondaryAxisX
        {
            get
            {
                return _visualSettings.SecondaryAxisX;
            }
        }

        /// <summary>
        /// The Chart's primary Y-Axis.
        /// </summary>
        [Category("Behavior")]
        [Description("The chart's primary Y-axis.")]
        [ReadOnly(true)]
        public AxisY PrimaryAxisY
        {
            get
            {
                if (this.chartMain.Diagram is XYDiagram)
                {
                    return ((XYDiagram)this.chartMain.Diagram).AxisY;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The chart's secondary Y-axis.
        /// </summary>
        [Category("Behavior")]
        [Description("The chart's secondary Y-axis.")]
        public SecondaryAxisY SecondaryAxisY
        {
            get
            {
                return _visualSettings.SecondaryAxisY;
            }
            set
            {
                _visualSettings.SecondaryAxisY = value;
                this.MarkAsDirty(UpdatableElement.SecondaryAxisY);
            }
        }

        /// <summary>
        /// Whether the chart's labels along the primary X axis are visible.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the chart's labels along the primary X axis are visible.")]
        public bool PrimaryAxisXLabelsVisible
        {
            get
            {
                return _visualSettings.PrimaryAxisXLabelsVisible;
            }
            set
            {
                _visualSettings.PrimaryAxisXLabelsVisible = value;
                this.MarkAsDirty(UpdatableElement.PrimaryAxisXLabel);
            }
        }

        /// <summary>
        /// The angle of the labels along the primary X Axis.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(LabelAngle.Angle_000)]
        [Description("The angle of the labels along the primary X Axis.")]
        public LabelAngle PrimaryAxisXLabelAngle
        {
            get
            {
                return _visualSettings.XAxisLabelAngle;
            }
            set
            {
                _visualSettings.XAxisLabelAngle = value;
                this.MarkAsDirty(UpdatableElement.PrimaryAxisXLabel);
            }
        }

        /// <summary>
        /// The space between Primary X-axis labels, measured in data points.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(1)]
        [Description("The space between Primary X-axis labels, measured in data points.")]
        [ReadOnly(true)]
        public int PrimaryAxisXLabelTickScale
        {
            get
            {
                return _visualSettings.XAxisLabelTickScale;
            }
            set
            {
                _visualSettings.XAxisLabelTickScale = value;
                this.MarkAsDirty(UpdatableElement.PrimaryAxisXLabel);
            }
        }

        /// <summary>
        /// The location of the Primary X Axis relative to the Primary Y Axis.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(AxisAlignment.Near)]
        [Description("The location of the Primary X Axis relative to the Primary Y Axis.")]
        public AxisAlignment PrimaryAxisXAlignment
        {
            get
            {
                return _visualSettings.PrimaryAxisXAlignment;
            }
            set
            {
                _visualSettings.PrimaryAxisXAlignment = value;
                this.MarkAsDirty(UpdatableElement.PrimaryAxisXAlignment);
            }
        }

        /// <summary>
        /// What scale to use for the secondary Y-axis, as a ratio to the primary axis.  Must be greater than zero.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(DateTimeScaleMode.AutomaticAverage)]
        [Description("The scale to use for date/time intervals.")]
        public DateTimeScaleMode PrimaryAxisXDateTimeScaleMode
        {
            get
            {
                return ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeScaleMode;
            }
            set
            {
                try
                {
                    ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeScaleMode = value;
                    //DateTimeOptions options = ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeOptions;
                    //options.Format = DateTimeFormat.Custom;
                    //options.FormatString = "MMMM, YY";
                    //((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeOptions = options;
                }
                catch (ArgumentOutOfRangeException x)
                {
                    throw x;
                }
            }
        }

        /// <summary>
        /// The unit to use when specifying a manual date/time spacing on the axis
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(DateTimeMeasurementUnit.Day)]
        [Description("The measuring unit to use for date/time intervals.")]
        public DateTimeMeasurementUnit PrimaryAxisXDateTimeMeasureUnit
        {
            get
            {
                return ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeMeasureUnit;
            }
            set
            {
                try
                {
                    ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeMeasureUnit = value;
                }
                catch (ArgumentOutOfRangeException x)
                {
                    throw x;
                }
            }
        }

        /// <summary>
        /// How the grid should be aligned on the axis when specifying a custom interval
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(DateTimeMeasurementUnit.Day)]
        [Description("The measuring unit to use for date/time intervals.")]
        public DateTimeMeasurementUnit PrimaryAxisXDateTimeGridAlignment
        {
            get
            {
                return ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeGridAlignment;
            }
            set
            {
                try
                {
                    ((XYDiagram)this.chartMain.Diagram).AxisX.DateTimeGridAlignment = value;
                }
                catch (ArgumentOutOfRangeException x)
                {
                    throw x;
                }
            }
        }

        /// <summary>
        /// Whether or not the Secondary X Axis is visible.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Whether or not the Secondary X Axis is visible.")]
        public bool SecondaryAxisXVisible
        {
            get
            {
                return _secondaryXAxisVisible;
            }
            set
            {
                _secondaryXAxisVisible = value;
                if (this.chartMain.Diagram is XYDiagram)
                {
                    if (_secondaryXAxisVisible)
                    {
                        ((XYDiagram)this.chartMain.Diagram).SecondaryAxesX.Add(this.SecondaryAxisX);
                    }
                    else
                    {
                        ((XYDiagram)this.chartMain.Diagram).SecondaryAxesX.Remove(this.SecondaryAxisX);
                    }
                }
            }
        }

        /// <summary>
        /// The Title displayed alongside the Primary Y-Axis.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("The Title displayed alongside the Primary Y-Axis.")]
        public string PrimaryAxisYTitle
        {
            get
            {
                return _visualSettings.PrimaryAxisYTitle;
            }
            set
            {
                _visualSettings.PrimaryAxisYTitle = value;
                this.MarkAsDirty(UpdatableElement.PrimaryAxisYTitle);
            }
        }

        /// <summary>
        /// The Title displayed alongside the Secondary Y-Axis.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("The Title displayed alongside the Secondary Y-Axis.")]
        public string SecondaryAxisYTitle
        {
            get
            {
                return _visualSettings.SecondaryAxisYTitle;
            }
            set
            {
                _visualSettings.SecondaryAxisYTitle = value;
                this.MarkAsDirty(UpdatableElement.SecondaryAxisYTitle);
            }
        }

        /// <summary>
        /// Whether the secondary y axis is shown.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether the secondary Y Axis is shown.")]
        public bool SecondaryAxisYVisible
        {
            get
            {
                return this.SecondaryAxisY.Visible;
            }
            set
            {
                this.SecondaryAxisY.Visible = value;
                if (value && this.chartMain.Diagram is XYDiagram && !((XYDiagram)this.chartMain.Diagram).SecondaryAxesY.Contains(this.SecondaryAxisY))
                {
                    XYDiagram diagram = (XYDiagram)this.chartMain.Diagram;
                    diagram.SecondaryAxesY.Add(this.SecondaryAxisY);
                }

                // When the initial loop was taking hold, it can't set the visibility because it's not fully loaded yet
                ((XYDiagram)this.chartMain.Diagram).SecondaryAxesY[0].SetVisibilityInPane(true, ((XYDiagram)this.chartMain.Diagram).DefaultPane);
                this.MarkAsDirty(UpdatableElement.SecondaryAxisY);
            }
        }

        /// <summary>
        /// The numeric format for the primary Y-axis.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(NumericFormat.General)]
        [Description("The numeric format for the primary Y-axis.")]
        public NumericFormat PrimaryAxisYFormat
        {
            get
            {
                return _visualSettings.PrimaryFormat;
            }
            set
            {
                _visualSettings.PrimaryFormat = value;
                //this.MarkAsDirty(UpdatableElement.?);
            }
        }

        /// <summary>
        /// The numeric format for the secondary Y-axis.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(NumericFormat.General)]
        [Description("The numeric format for the secondary Y-axis.")]
        public NumericFormat SecondaryAxisYFormat
        {
            get
            {
                return _visualSettings.SecondaryFormat;
            }
            set
            {
                _visualSettings.SecondaryFormat = value;
                this.MarkAsDirty(UpdatableElement.SecondaryAxisY);
            }
        }

        /// <summary>
        /// The decimal precision for labels assigned to the primary axis.  A number below 0 indicates the format's default precision.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(-1)]
        [Description("The decimal precision for labels assigned to the primary Y axis.  A number below 0 indicates the format's default precision.")]
        public int PrimaryAxisYFormatPrecision
        {
            get
            {
                return _visualSettings.PrimaryFormatPrecision;
            }
            set
            {
                _visualSettings.PrimaryFormatPrecision = value;
                //UpdateThisSetting("PrimaryFormatPrecision");
            }
        }

        /// <summary>
        /// The decimal precision for labels assigned to the primary axis.  A number below 0 indicates the format's default precision.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(-1)]
        [Description("The decimal precision for labels assigned to the secondary Y axis.  A number below 0 indicates the format's default precision.")]
        public int SecondaryAxisYFormatPrecision
        {
            get
            {
                return _visualSettings.SecondaryFormatPrecision;
            }
            set
            {
                _visualSettings.SecondaryFormatPrecision = value;
                //UpdateThisSetting("SecondaryFormatPrecision");
            }
        }

        /// <summary>
        /// Whether or not the two Y-axes are aligned.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Whether the secondary Y-axis is aligned to the first.")]
        public bool YAxesAligned
        {
            get
            {
                return _visualSettings.AxesAligned;
            }
            set
            {
                _visualSettings.AxesAligned = value;
                this.MarkAsDirty(UpdatableElement.SecondaryAxisY);
            }
        }

        /// <summary>
        /// What value should match when the two Y-axes are aligned.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(0.0)]
        [Description("What value matches when the two Y-axes are aligned.")]
        public double YAxesAlignedAtValue
        {
            get
            {
                return _visualSettings.AxesAlignedAtValue;
            }
            set
            {
                _visualSettings.AxesAlignedAtValue = value;
                this.MarkAsDirty(UpdatableElement.SecondaryAxisY);
            }
        }

        /// <summary>
        /// What scale to use for the secondary Y-axis, as a ratio to the primary axis.  Must be greater than zero.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(1.0)]
        [Description("What scale to use for the secondary Y-axis, as a ratio to the primary axis.  Must be greater than zero.")]
        public double AxesScaleY
        {
            get
            {
                return _visualSettings.AxesScale;
            }
            set
            {
                try
                {
                    _visualSettings.AxesScale = value;
                    this.MarkAsDirty(UpdatableElement.SecondaryAxisY);
                }
                catch (ArgumentOutOfRangeException x)
                {
                    throw x;
                }
            }
        }

        #endregion Axes

        #region Ribbon Controls

        /// <summary>
        /// Whether the ribbon menu is visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether the ribbon menu is visible.")]
        public bool RibbonMenuVisible
        {
            get
            {
                return (layoutRibbon.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always);
            }
            set
            {
                if (value == true)
                {
                    layoutRibbon.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    layoutRibbon.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
        }

        /// <summary>
        /// Whether the user can change the color scheme.
        /// </summary>
        [Category("Ribbon Controls")]
        [DefaultValue(true)]
        [Description("Whether the user can change the color scheme.")]
        public bool AllowColorSchemeChange
        {
            get
            {
                return _rcColorScheme;
            }
            set
            {
                _rcColorScheme = value;
                if (_isLoaded)
                {
                    if (_rcColorScheme)
                    {
                        barColorScheme.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        barColorScheme.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                }
            }
        }

        /// <summary>
        /// Whether the user can move and/or hide the chart's legend.
        /// </summary>
        [Category("Ribbon Controls")]
        [DefaultValue(true)]
        [Description("Whether the user can move and/or hide the chart's legend.")]
        public bool AllowLegendChange
        {
            get
            {
                return _rcLegend;
            }
            set
            {
                _rcLegend = value;
                if (_isLoaded)
                {
                    if (_rcLegend)
                    {
                        barLegend.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        barLegend.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                }
            }
        }

        /// <summary>
        /// Whether the user can set the chart to display percentages in place of actual values or vice versa.
        /// </summary>
        [Category("Ribbon Controls")]
        [DefaultValue(true)]
        [Description("Whether the user can set the chart to display percentages in place of actual values or vice versa.")]
        public bool ValueAsPercentageVisibility
        {
            get
            {
                return _rcValueAsPercent;
            }
            set
            {
                _rcValueAsPercent = value;
                if (_isLoaded)
                {
                    if (_rcValueAsPercent)
                    {
                        chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                }
            }
        }

        #region Buttons

        /// <summary>
        /// Whether the Chart Type button is visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether the Chart Type button is visible.")]
        public bool ChartTypeButtonVisible
        {
            get
            {
                return (btnChartType.Visibility == DevExpress.XtraBars.BarItemVisibility.Always);
            }
            set
            {
                if (value == true)
                {
                    btnChartType.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                else
                {
                    btnChartType.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            }
        }

        /// <summary>
        /// Whether or not the Save to File button is visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether or not the Save to File button is available.")]
        public bool ShowSaveButton
        {
            get
            {
                return btnSaveJpg.Visibility == DevExpress.XtraBars.BarItemVisibility.Always;
            }
            set
            {
                if (value == true)
                {
                    btnSaveJpg.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                else
                {
                    btnSaveJpg.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            }
        }

        /// <summary>
        /// Whether or not the Copy button is visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether or not the Copy button is available.")]
        public bool ShowCopyButton
        {
            get
            {
                return btnCopyJpg.Visibility == DevExpress.XtraBars.BarItemVisibility.Always;
            }
            set
            {
                if (value == true)
                {
                    btnCopyJpg.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                else
                {
                    btnCopyJpg.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            }
        }

        /// <summary>
        /// Whether or not the Print button is visible.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether or not the Print button is visible.")]
        public bool ShowPrintButton
        {
            get
            {
                return btnPrintChart.Visibility == DevExpress.XtraBars.BarItemVisibility.Always;
            }
            set
            {
                if (value == true)
                {
                    btnPrintChart.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                }
                else
                {
                    btnPrintChart.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            }
        }
        #endregion Buttons

        #endregion Ribbon Controls

        #region Printing
        /// <summary>
        /// The title displayed on the report when this chart is printed.  If blank, uses the form's title.
        /// </summary>
        [DefaultValue("")]
        [Category("Printing")]
        [Description("The title displayed on the report when this chart is printed.  If blank, uses the form's title.")]
        public string PrintingTitle
        {
            get
            {
                return _visualSettings.PrintingTitle;
            }
            set
            {
                _visualSettings.PrintingTitle = value;
            }
        }

        /// <summary>
        /// The string value to display as the filter when printed.  
        /// If left blank, uses the chart's ssChartPredicate filter.
        /// </summary>
        [Category("Printing")]
        [DefaultValue("")]
        [Description("The string value to display as the filter when printed.  If left blank, uses the chart's ssChartPredicate filter.")]
        public string PrintingFilter
        {
            get
            {
                return _visualSettings.PrintingFilter;
            }
            set
            {
                _visualSettings.PrintingFilter = value;
            }
        }

        /// <summary>
        /// The underlying chart rendered in an XRChart control for reporting.
        /// </summary>
        public XRChart ChartForReport
        {
            get
            {
                XRChart xchart = new XRChart();

                ((IChartContainer)xchart).Chart.Assign(((IChartContainer)this.DevXChart).Chart);

                return xchart;
            }
        }


        /// <summary>
        /// The data used for the chart, displayed in one or more XRTable controls for reporting.
        /// </summary>
        public List<XRTable> DataTablesForReport
        {
            get
            {
                // Set up some variables.
                List<XRTable> xtables = new List<XRTable>();
                XRTable primaryTable = XRTable.CreateTable(new RectangleF(0f, 0f, 250, 10), 1, 1);
                primaryTable.Borders = DevExpress.XtraPrinting.BorderSide.All;
                XRTable secondaryTable = null, currentTable = null;
                SortedList<string, string> primaryArguments = new SortedList<string, string>(),
                    secondaryArguments = new SortedList<string, string>(),
                    currentArguments = null;
                KeyValuePair<string, string> cellArgument = new KeyValuePair<string, string>(string.Empty, string.Empty);
                string dateFormat = string.Empty;
                List<int> weights = new List<int>(new int[] { 0, 0 });
                //int weightsIndex = 0;

                foreach (WinChartSeries series in this.Series)
                {
                    // Make sure we've got the right table.
                    if (series.UseSecondaryAxisX)
                    {
                        if (secondaryTable == null)
                        {
                            secondaryTable = XRTable.CreateTable(new RectangleF(0f, 0f, 250, 10), 1, 1);
                            secondaryTable.Borders = DevExpress.XtraPrinting.BorderSide.All;
                        }
                        currentTable = secondaryTable;
                        currentArguments = secondaryArguments;
                        //weightsIndex = 1;
                        dateFormat = this.ExtractDateFormat(this.SecondaryAxisX);
                    }
                    else
                    {
                        currentTable = primaryTable;
                        currentArguments = primaryArguments;
                        //weightsIndex = 2;

                        dateFormat = this.ExtractDateFormat(this.PrimaryAxisX);
                    }


                    // Update the weight for the series name.


                    // Create the series row.
                    XRTableRow row = currentTable.InsertRowBelow(null);
                    row.Cells[0].Font = new Font("Arial", 12f, FontStyle.Bold);
                    row.Cells[0].Text = series.Name;

                    foreach (SeriesPoint point in series.Points)
                    {
                        // Get the correct argument
                        switch (series.ArgumentScaleType)
                        {
                            case ScaleType.DateTime:
                                DateTime dt = point.DateTimeArgument;
                                cellArgument = new KeyValuePair<string, string>(dt.ToString("s"), dt.ToString(dateFormat));
                                break;
                            case ScaleType.Numerical:
                                double d = point.NumericalArgument;
                                cellArgument = new KeyValuePair<string, string>(d.ToString("0000000000000000"), Convert.ToString(d));
                                break;
                            case ScaleType.Qualitative:
                                cellArgument = new KeyValuePair<string, string>(point.Argument, point.Argument);
                                break;
                            default:
                                break;
                        }

                        // Make sure the argument is already in the column headers
                        if (!currentArguments.Contains(cellArgument))
                        {
                            currentArguments.Add(cellArgument.Key, cellArgument.Value);
                            // currentArguments.Sort();
                            int columnlocation = currentArguments.Keys.IndexOf(cellArgument.Key);

                            // Create the column and get the top cell all in one fell swoop.
                            currentTable.InsertColumnToRight(row.Cells[columnlocation]);
                            XRTableCell headerCell = currentTable.Rows[0].Cells[columnlocation + 1];
                            headerCell.TextAlignment = TextAlignment.BottomCenter;
                            headerCell.Font = new Font("Arial", 12f, FontStyle.Bold);
                            headerCell.Text = cellArgument.Value;
                        }

                        XRTableCell currentCell = row.Cells[currentArguments.Keys.IndexOf(cellArgument.Key) + 1];
                        if (currentCell.Text == null || currentCell.Text.Trim() == string.Empty)
                        {
                            currentCell.TextAlignment = TextAlignment.BottomRight;
                            currentCell.Font = new Font("Arial", 12f);
                            currentCell.Text = Convert.ToString(point.Values[0]);
                        }
                        else
                        {
                            currentCell.Text = string.Format("{0}, {1}", currentCell.Text, Convert.ToString(point.Values[0]));
                        }
                    }
                }

                xtables.Add(primaryTable);
                if (secondaryTable != null)
                {
                    xtables.Add(secondaryTable);
                }

                // Make all cells the same size.
                foreach (XRTable table in xtables)
                {
                    // table.SuspendLayout();
                    table.BeginInit();
                    table.WidthF = 100 * (table.Rows[0].Cells.Count);
                    foreach (XRTableRow row in table.Rows)
                    {
                        foreach (XRTableCell cell in row.Cells)
                        {
                            cell.WidthF = 100;

                        }
                    }
                    // table.ResumeLayout();
                    table.EndInit();
                }

                return xtables;
            }
        }

        #endregion Printing

        #endregion Public Properties

        #region Public Static Properties

        /// <summary>
        /// STATIC READ-ONLY: A dictionary for converting the custom ChartAppearance enum values into the strings actually used by the native chart control.
        /// </summary>
        private Dictionary<Palette, string> AppearanceDictionary
        {
            get
            {
                Dictionary<Palette, string> rtv = new Dictionary<Palette, string>();
                foreach (string p in chartMain.GetPaletteNames())
                {
					rtv.Add(chartMain.PaletteRepository[p], mgCustom.Utils.AlterCaptionFromFieldName(p.Replace("_", "")));
                }
                //rtv.Sort();
				Dictionary<Palette, string> sorted = mgCustom.Utils.SortDictionary<Palette, string>(rtv);
                return sorted;		// Return the list
            }
        }

        /// <summary>
        /// STATIC READ-ONLY: a Dictionary for converting HoleRadius Enum values to integers.
        /// </summary>
        public static Dictionary<HoleRadius, int> HoleRadiusDictionary
        {
            get
            {
                Dictionary<HoleRadius, int> rtv = new Dictionary<HoleRadius, int>();
                foreach (string s in Enum.GetValues(typeof(HoleRadius)))
                {
                    rtv.Add((HoleRadius)Enum.Parse(typeof(HoleRadius), s), int.Parse(s));
                }
                return rtv;

                //if (_holeRadiusDictionary == null)
                //{
                //    _holeRadiusDictionary = new Dictionary<HoleRadius, int>();

                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_0, 0);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_15, 15);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_30, 30);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_50, 50);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_60, 60);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_75, 75);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_90, 90);
                //    _holeRadiusDictionary.Add(HoleRadius.RADIUS_100, 100);
                //}
                //return _holeRadiusDictionary;
            }
        }

        #endregion Public Static Properties

        #region Public Methods

        /// <summary>
        /// Marks the beginning of a large update, and holds off cleaning properties until it is over.
        /// NOTE: Make sure you call EndUpdate() later.
        /// </summary>
        public void BeginUpdate()
        {
            _holdCleaning = true;
        }

        /// <summary>
        /// Marks the end of a large update, and cleans up the properties.
        /// </summary>
        public void EndUpdate()
        {
            _holdCleaning = false;
            this.CleanAllElements();
        }

        /// <summary>
        /// Assigns the datasource, adds series for each column after the first, and renders the chart.
        /// </summary>
        /// <param name="datasource">The object to use as a data source.  Must be a DataTable or IClassGenClassGenerated object.</param>
        /// <param name="filter">A filter to apply to the generated series.</param>
        public void AutoRender(object datasource, ssChartPredicate filter)
        {
            this.DataSource = datasource;
            DataTable dt = this.DataSourceAsTable;
            if (dt.Columns.Count < 2)
            {
                throw new ArgumentException("The data source must have at least two columns/properties in order to AutoRender.", "datasource");
            }

            this.Series.Clear();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                DataColumn column = dt.Columns[i];
                this.AddSeries(column.ColumnName);
            }

            this.Render(filter);
        }

        /// <summary>
        /// Assigns the datasource, adds series for each column after the first, and renders the chart.
        /// </summary>
        /// <param name="datasource">The object to use as a data source.  Must be a DataTable or IClassGenClassGenerated object.</param>
        public void AutoRender(object datasource)
        {
            this.AutoRender(datasource, null);
        }

        /// <summary>
        /// Reloads the data from the DataTable.  Does nothing and exits early if any of the DataTable, SeriesNames, or ArgumentName properties are null.
        /// </summary>
        public void Render()
        {
            Render(null);
        }

        /// <summary>
        /// Reloads the data from the DataTable, applying a filter to all series in the process.  Does nothing and exits early if any of the DataTable, SeriesNames, or ArgumentName properties are null.
        /// </summary>
        /// <param name="filter">The filter to apply to the series points.</param>
        public void Render(ssChartPredicate filter)
        {
            this.BeginUpdate();

            if (DataSource == null || this.Series == null || PrimaryXAxisColumnName == null)
            {
                //throw an error here?
                return;
            }

            this.chartMain.Series.Clear();
            this.chartMain.AnnotationRepository.Clear();
            _seriesUIs.Clear();
            //tabSeriesControls.TabPages.Clear();

            // For large numbers of points, drawing all those labels takes a long time and just obscures things.  So hide them.
            if (this.Series.Count * this.DataSourceAsTable.Rows.Count > 200)
            {
                this.LabelsVisible = false;
            }

            // Manage the filter.
            _filter = filter;
            if (_filter == null)
            {
                _filter = new ssCompoundChartPredicate(PredicateConjunction.And);
            }

            // Add each series to the table, then populate them with points.
            chartMain.Series.AddRange(this.Series.ToArray());

            foreach (WinChartSeries series in this.Series)
            {
                if (series.UseSecondaryAxisX == true)
                {
                    if (this.chartMain.Diagram is XYDiagram &&
                        !((XYDiagram)this.chartMain.Diagram).SecondaryAxesX.Contains(this.SecondaryAxisX))
                    {
                        ((XYDiagram)this.chartMain.Diagram).SecondaryAxesX.Add(this.SecondaryAxisX);
                    }
                    series.PrepareDataPoints(this.DataSource, this.SecondaryXAxisColumnName, _filter);
                }
                else
                {
                    series.PrepareDataPoints(this.DataSource, this.PrimaryXAxisColumnName, _filter);
                }
                series.ChangeView(this.ChartType);
                series.Label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
            }

            bool chartTypeIncluded = InitChartTypes();

            //RibbonPage templatePage = (RibbonPage)pageGeneral.Clone();

            foreach (Series series in this.Series)
            {
                SeriesUI ui = new SeriesUI();
                _seriesUIs.Add(ui);
                ui.ParentChart = this;

                //ribbonMain.Pages.Add(ui.Page);

                ui.SetDataMemberChoices(SeriesNames);
                ui.SetGroupingChoices(SeriesNames.Count);

                ui.Series = series;
                ui.Title = series.Name;
                ui.UpdateSeries();

                ui.SeriesDataMemberChanged += new EventHandler(ui_SeriesDataMemberChanged);
                ui.SeriesGroupChanged += new EventHandler(ui_SeriesGroupChanged);
            }

            SetExplodedPointsOptions();

            if (!chartTypeIncluded)
            {
                // Then we need to revert to something safe.
                this.ChartType = _oldViewType;
                _oldViewType = this.ChartType;
                if (!InitChartTypes())
                {
                    // Uh-oh.  Still not safe.  Go to good old bar view.  
                    this.ChartType = DevExpress.XtraCharts.ViewType.Bar;
                    _oldViewType = DevExpress.XtraCharts.ViewType.Bar;
                    InitChartTypes();
                }
            }

            this.MarkAsDirty(new List<UpdatableElement>(_dirtyElements.Keys));
            this.EndUpdate();
        }

        /// <summary>
        /// Remove the X Axis strips
        /// </summary>
        public void RemoveXStrips()
        {
            XYDiagram diagram = (XYDiagram)chartMain.Diagram;
            diagram.AxisX.Strips.Clear();
        }

        /// <summary>
        /// Remove the Y Axis strips
        /// </summary>
        public void RemoveYStrips()
        {
            XYDiagram diagram = (XYDiagram)chartMain.Diagram;
            diagram.AxisY.Strips.Clear();
        }

        /// <summary>
        /// Set the primary axis Date/Time Scale
        /// </summary>
        /// <param name="scaleMode">The scale mode to use</param>
        public void SetPrimaryAxisXDateTimeScale(DateTimeScaleMode scaleMode)
        {
            this.PrimaryAxisXDateTimeScaleMode = scaleMode;
        }

        /// <summary>
        /// Set the primary axis Date/Time Scale
        /// This is used for manual scale modes to specify additional options
        /// The scale mode on this option is automatically set to manual
        /// </summary>
        /// <param name="gridAlignment">The grid alignment to use</param>
        /// <param name="measureUnit">The measurement unit to use</param>
        public void SetPrimaryAxisXDateTimeScale(DateTimeMeasurementUnit measureUnit,
            DateTimeMeasurementUnit gridAlignment)
        {
            SetPrimaryAxisXDateTimeScale(measureUnit, gridAlignment, false);
        }

        /// <summary>
        /// Set the primary axis Date/Time Scale
        /// This is used for manual scale modes to specify additional options
        /// The scale mode on this option is automatically set to manual
        /// </summary>
        /// <param name="gridAlignment">The grid alignment to use</param>
        /// <param name="measureUnit">The measurement unit to use</param>
        /// <param name="onlyIncludeWorkDays">Only includes the working days (M-F) if true</param>
        public void SetPrimaryAxisXDateTimeScale(DateTimeMeasurementUnit measureUnit,
            DateTimeMeasurementUnit gridAlignment,
            bool onlyIncludeWorkDays)
        {
            this.PrimaryAxisXDateTimeScaleMode = DateTimeScaleMode.Manual;
            this.PrimaryAxisXDateTimeMeasureUnit = measureUnit;
            this.PrimaryAxisXDateTimeGridAlignment = gridAlignment;
            this.PrimaryAxisX.WorkdaysOnly = onlyIncludeWorkDays;
            if (onlyIncludeWorkDays)
            {
                this.PrimaryAxisX.WorkdaysOptions.Workdays = _workingDays;
            }
        }

        /// <summary>
        /// Set the working days for the grid to use
        /// </summary>
        /// <param name="workingDays">The working days to use</param>
        public void SetWorkingDays(Weekday workingDays)
        {
            _workingDays = workingDays;
        }

        /// <summary>
        /// Print the chart (returns an object that allows the user to work with the printed chart).
        /// </summary>
        /// <returns></returns>
        public ssWinChart_Print PrintChart()
        {
            this.chartMain.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;

            ssWinChart_Print printChart = new ssWinChart_Print(this);
            printChart.Orientation = mgPrintOrientation.Landscape;

            this.OnPrintButtonClicked(new EventArgs());

            if (string.IsNullOrEmpty(this.PrintingTitle))
            {
                printChart.ReportTitle = this.FindForm().Text;
            }
            else
            {
                printChart.ReportTitle = this.PrintingTitle;
            }

            if (string.IsNullOrEmpty(this.PrintingFilter))
            {
                printChart.FilterString = _filter.ToString();
            }
            else
            {
                printChart.FilterString = this.PrintingFilter;
            }

            return printChart;
        }

        /// <summary>
        /// Add an X strip to the chart
        /// </summary>
        /// <param name="name">The name of the strip</param>
        /// <param name="minValue">The minimum value</param>
        /// <param name="maxValue">The maximum value</param>
        /// <param name="axisLabelText">The axis label text</param>
        /// <param name="legendText">The legend text</param>
        /// <param name="color">The color</param>
        public void AddXStrip(string name, object minValue, object maxValue, string axisLabelText, string legendText, Color color)
        {
            XYDiagram diagram = (XYDiagram)chartMain.Diagram;
            Strip s = null;
            if (minValue is DateTime)
            {
                s = new Strip(name, (DateTime)minValue, (DateTime)maxValue);
            }
            else if (minValue is string)
            {
                s = new Strip(name, (string)minValue, (string)maxValue);
            }
            else if (minValue is double)
            {
                s = new Strip(name, (double)minValue, (double)maxValue);
            }
            else if (minValue is decimal) { s = new Strip(name, (double)((decimal)minValue), (double)((decimal)maxValue)); }
            else if (minValue is Int16) { s = new Strip(name, (double)((Int16)minValue), (double)((Int16)maxValue)); }
            else if (minValue is Int32) { s = new Strip(name, (double)((Int32)minValue), (double)((Int32)maxValue)); }
            else if (minValue is Int64) { s = new Strip(name, (double)((Int64)minValue), (double)((Int64)maxValue)); }

            if (s != null)
            {
                diagram.AxisX.Strips.Add(s);

                // Customize the behavior
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].Visible = true;
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].ShowAxisLabel = (!String.IsNullOrEmpty(axisLabelText));
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].AxisLabelText = axisLabelText;
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].ShowInLegend = (!String.IsNullOrEmpty(legendText));
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].LegendText = legendText;

                // Customize the appearance
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].Color = color;
                diagram.AxisX.Strips[diagram.AxisX.Strips.Count - 1].FillStyle.FillMode = FillMode.Empty;
            }
        }

        /// <summary>
        /// Add a Y strip to the chart
        /// </summary>
        /// <param name="name">The name of the strip</param>
        /// <param name="minValue">The minimum value</param>
        /// <param name="maxValue">The maximum value</param>
        /// <param name="axisLabelText">The axis label text</param>
        /// <param name="legendText">The legend text</param>
        /// <param name="color">The color</param>
        public void AddYStrip(string name, object minValue, object maxValue, string axisLabelText, string legendText, Color color)
        {
            XYDiagram diagram = (XYDiagram)chartMain.Diagram;
            Strip s = null;
            if (minValue is DateTime)
            {
                s = new Strip(name, (DateTime)minValue, (DateTime)maxValue);
            }
            else if (minValue is string)
            {
                s = new Strip(name, (string)minValue, (string)maxValue);
            }
            else if (minValue is double)
            {
                s = new Strip(name, (double)minValue, (double)maxValue);
            }
            else if (minValue is decimal) { s = new Strip(name, (double)((decimal)minValue), (double)((decimal)maxValue)); }
            else if (minValue is Int16) { s = new Strip(name, (double)((Int16)minValue), (double)((Int16)maxValue)); }
            else if (minValue is Int32) { s = new Strip(name, (double)((Int32)minValue), (double)((Int32)maxValue)); }
            else if (minValue is Int64) { s = new Strip(name, (double)((Int64)minValue), (double)((Int64)maxValue)); }

            if (s != null)
            {
                diagram.AxisY.Strips.Add(s);

                // Customize the behavior
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].Visible = true;
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].ShowAxisLabel = (!String.IsNullOrEmpty(axisLabelText));
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].AxisLabelText = axisLabelText;
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].ShowInLegend = (!String.IsNullOrEmpty(legendText));
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].LegendText = legendText;

                // Customize the appearance
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].Color = color;
                diagram.AxisY.Strips[diagram.AxisY.Strips.Count - 1].FillStyle.FillMode = FillMode.Empty;
            }
        }

        /// <summary>
        /// Adds a series to the chart.
        /// </summary>
        /// <param name="seriesName">The name of the series, as displayed on the chart.</param>
        /// <param name="columnName">The column with which to populate the series.</param>
        /// <param name="addSeriesToSecondaryAxisX">Whether to add the series to the secondary X axis instead of the primary.</param>
        public void AddSeries(string seriesName, string columnName, bool addSeriesToSecondaryAxisX)
        {
            WinChartSeries series = new WinChartSeries(this, seriesName, columnName, this.ChartType);
            this.Series.Add(series);
            series.UseSecondaryAxisX = addSeriesToSecondaryAxisX;
        }

        /// <summary>
        /// Adds a series to the chart.
        /// </summary>
        /// <param name="seriesName">The name of the series, as displayed on the chart.</param>
        /// <param name="columnName">The column with which to populate the series.</param>
        public void AddSeries(string seriesName, string columnName)
        {
            this.AddSeries(seriesName, columnName, false);
        }

        /// <summary>
        /// Adds a series to the chart.
        /// </summary>
        /// <param name="seriesName">The column with which to populate the series.</param>
        /// <param name="addSeriesToSecondaryAxisX">Whether to add the series to the secondary X axis instead of the primary.</param>
        public void AddSeries(string seriesName, bool addSeriesToSecondaryAxisX)
        {
            WinChartSeries series = new WinChartSeries(this, seriesName, this.ChartType);
            this.Series.Add(series);
            series.UseSecondaryAxisX = addSeriesToSecondaryAxisX;
        }

        /// <summary>
        /// Adds a series to the chart.
        /// </summary>
        /// <param name="seriesName">The column with which to populate the series.</param>
        public void AddSeries(string seriesName)
        {
            this.AddSeries(seriesName, false);
        }

        /// <summary>
        /// Returns a series from the table with the given name, or a null value if the table doesn't contain such a series.
        /// </summary>
        /// <param name="seriesName"></param>
        /// <returns></returns>
        public WinChartSeries GetSeries(string seriesName)
        {
            foreach (WinChartSeries series in this.Series)
            {
                if (series.Name.Equals(seriesName))
                {
                    return series;
                }
            }
            return null;
        }

        /// <summary>
        /// Applies property changes to the internal chart's series.
        /// </summary>
        public void UpdateAllSeries()
        {
            foreach (SeriesUI ui in _seriesUIs)
            {
                ui.UpdateSeries();
            }
        }

        /// <summary>
        /// Sets each individual series's properties to match the VisualSettingsObject's properties.
        /// </summary>
        public void AlignAllSeries()
        {
            foreach (SeriesUI ui in _seriesUIs)
            {
                ui.AlignToVisualSettings(_visualSettings);
            }
        }

        /// <summary>
        /// Serializes the visual settings into an XML Document for later use.
        /// </summary>
        /// <returns>An XML Document that stores the visual settings.</returns>
        public XmlDocument SerializeChartSettingsToXML()
        {
            /*
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<SettingConfig version='1.0' />");

            foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(this, new Attribute[] { new SerializeAppearanceAttribute(true) }))
            {
                
                XmlAttribute xmlAtt = xmlDoc.CreateAttribute(p.Name);
                xmlAtt.Value = p.GetValue(this).ToString();
                xmlDoc.DocumentElement.SetAttributeNode(xmlAtt);
            }

            return xmlDoc;
             */
            string xml = GenericSerializer<VisualSettingsObject>.Serialize(_visualSettings);
#if DEBUG_EVAN
            Console.WriteLine("Serialized XML: " + xml);
#endif

            //Stupid XmlDocument.LoadXML() fails when the xml has a header.  
            //So we first encode it in a memory stream and then use the Load() method.
            byte[] encodedXML = Encoding.UTF8.GetBytes(xml);
            MemoryStream ms = new MemoryStream(encodedXML);
            ms.Flush();
            ms.Position = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(ms);
            ms.Dispose();
            //Add the Version Number
            XmlElement version = doc.CreateElement("version");
            XmlText versionNumber = doc.CreateTextNode(VisualSettingsObject.CURRENT_VERSION.ToString());
            version.AppendChild(versionNumber);
            XmlNode visObj = doc.GetElementsByTagName("VisualSettingsObject")[0];
            visObj.PrependChild(version);
            //Serve with some kidney beans and a fine chianti.
            return doc;
        }

        /// <summary>
        /// Restores previous visual settings from an XML Document.
        /// </summary>
        /// <param name="xml">The XML Document created from SerializeChartSettingsToXML()</param>
        public void DeserializeChartSettingsFromXML(XmlDocument xml)
        {
            /*
            //switch (xml.DocumentElement.GetAttribute("version")){
            //  Modify legacy documents to support newest version.
            //}

            foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(this, new Attribute[] { new SerializeAppearanceAttribute(true) }))
            {
                if (xml.DocumentElement.HasAttribute(p.Name))
                {
                    string newValue = xml.DocumentElement.GetAttribute(p.Name);
                    if (p.PropertyType.IsEnum)
                    {
                        if (Enum.IsDefined(p.PropertyType, newValue))
                        {
                            p.SetValue(this, Enum.Parse(p.PropertyType, newValue));
                        }
                        else
                        {
                            //throw appropriate error here
                            Console.WriteLine("{0} is not a member of {1}", newValue, p.PropertyType);
                        }
                    }
                    else
                    {
                        p.SetValue(this, Convert.ChangeType(newValue, p.PropertyType)); 
                    }
                }
                else
                {
                    p.ResetValue(this);
                }
            }
             */

            _visualSettings = VisualSettingsObject.DeserializeNewObjectFromXML(xml.OuterXml);
            this.MarkAsDirty(new List<UpdatableElement>(_dirtyElements.Keys));
            CleanAllElements();
        }

        /// <summary>
        /// Restores the default visual settings.
        /// </summary>
        public void RestoreDefaultAppearance()
        {
            _visualSettings = new VisualSettingsObject();
        }

        /// <summary>
        /// Assigns a series to the Secondary X-Axis (when there is one).
        /// </summary>
        /// <param name="seriesDisplayName">The series' name that is displayed in the chart.</param>
        public void AssignSeriesToSecondaryAxisX(string seriesDisplayName)
        {
            if (!_seriesForSecondAxisX.Contains(seriesDisplayName))
            {
                _seriesForSecondAxisX.Add(seriesDisplayName);
            }
        }

        /// <summary>
        /// Assigns a group of series to the Secondary X-Axis (when there is one).
        /// </summary>
        /// <param name="seriesDisplayNames">A list of the series' names, as displayed in the chart.</param>
        public void AssignSeriesToSecondaryAxisX(List<string> seriesDisplayNames)
        {
            foreach (string name in seriesDisplayNames)
            {
                AssignSeriesToSecondaryAxisX(name);
            }
        }

        /// <summary>
        /// Removes a series from being assigned to the Secondary X-Axis.
        /// </summary>
        /// <param name="seriesDisplayName">The series' name that is displayed in the chart.</param>
        public void RemoveSeriesFromSecondaryAxisX(string seriesDisplayName)
        {
            if (_seriesForSecondAxisX.Contains(seriesDisplayName))
            {
                _seriesForSecondAxisX.Remove(seriesDisplayName);
            }
        }

        /// <summary>
        /// Removes a set of series from being assigned to the Secondary X-Axis.
        /// </summary>
        /// <param name="seriesDisplayNames">The series' names, as displayed in the chart.</param>
        public void RemoveSeriesFromSecondaryAxisX(List<string> seriesDisplayNames)
        {
            foreach (string name in seriesDisplayNames)
            {
                RemoveSeriesFromSecondaryAxisX(name);
            }
        }

        /// <summary>
        /// Assigns a series to the Secondary Y-Axis (when there is one).
        /// </summary>
        /// <param name="seriesDisplayName">The series' name that is displayed in the chart.</param>
        public void AssignSeriesToSecondaryAxisY(string seriesDisplayName)
        {
            if (!_seriesForSecondAxisY.Contains(seriesDisplayName))
            {
                _seriesForSecondAxisY.Add(seriesDisplayName);
            }
        }

        /// <summary>
        /// Assigns a group of series to the Secondary Y-Axis (when there is one).
        /// </summary>
        /// <param name="seriesDisplayNames">The series' names, as displayed in the chart.</param>
        public void AssignSeriesToSecondaryAxisY(List<string> seriesDisplayNames)
        {
            foreach (string name in seriesDisplayNames)
            {
                AssignSeriesToSecondaryAxisY(name);
            }
        }

        /// <summary>
        /// Removes a series from being assigned to the Secondary Y-Axis.
        /// </summary>
        /// <param name="seriesDisplayName">The series' name that is displayed in the chart.</param>
        public void RemoveSeriesFromSecondaryAxisY(string seriesDisplayName)
        {
            if (_seriesForSecondAxisY.Contains(seriesDisplayName))
            {
                _seriesForSecondAxisY.Remove(seriesDisplayName);
            }
        }

        /// <summary>
        /// Removes a group of series from being assigned to the Secondary Y-Axis.
        /// </summary>
        /// <param name="seriesDisplayNames">The series' names, as displayed in the chart.</param>
        public void RemoveSeriesFromSecondaryAxisY(List<string> seriesDisplayNames)
        {
            foreach (string name in seriesDisplayNames)
            {
                RemoveSeriesFromSecondaryAxisY(name);
            }
        }

        /// <summary>
        /// Removes all series from the secondary Y axis.
        /// </summary>
        public void RemoveAllSeriesFromSecondaryAxisY()
        {
            _seriesForSecondAxisY.Clear();
        }

        /// <summary>
        /// Sets up the secondary Y axis.
        /// </summary>
        /// <param name="seriesName">The name of the series to assign to the secondary Y axis.</param>
        public void SetUpSecondaryYAxis(string seriesName)
        {
            this.BeginUpdate();

            this.RemoveAllSeriesFromSecondaryAxisY();
            if (this.SecondaryAxisY == null)
            {
                this.SecondaryAxisY = new SecondaryAxisY();
            }
            this.SecondaryAxisYVisible = true;
            this.AssignSeriesToSecondaryAxisY(seriesName);
            this.YAxesAligned = false;

            this.EndUpdate();
        }

        /// <summary>
        /// Sets up the secondary Y axis.
        /// </summary>
        /// <param name="seriesNames">The names of the series to assign to the secondary Y axis.</param>
        public void SetUpSecondaryYAxis(params string[] seriesNames)
        {
            this.BeginUpdate();

            this.RemoveAllSeriesFromSecondaryAxisY();
            if (this.SecondaryAxisY == null)
            {
                this.SecondaryAxisY = new SecondaryAxisY();
            }
            this.SecondaryAxisYVisible = true;
            this.AssignSeriesToSecondaryAxisY(new List<string>(seriesNames));
            this.YAxesAligned = false;

            this.EndUpdate();
        }

        /// <summary>
        /// Sets up the secondary Y axis, aligning it to the primary Y axis.
        /// </summary>
        /// <param name="alignmentPoint">The point at which both Y-axes should have the same value.</param>
        /// <param name="scale">The ratio of the secondary axis values to the primary axis values.  Must be greater than zero.</param>
        /// <param name="seriesName">The name of the series to assign to the secondary Y axis.</param>
        public void SetUpSecondaryYAxis(double alignmentPoint, double scale, string seriesName)
        {
            this.BeginUpdate();

            this.RemoveAllSeriesFromSecondaryAxisY();
            if (this.SecondaryAxisY == null)
            {
                this.SecondaryAxisY = new SecondaryAxisY();
            }
            this.SecondaryAxisYVisible = true;
            this.AssignSeriesToSecondaryAxisY(seriesName);
            this.YAxesAligned = true;
            this.YAxesAlignedAtValue = alignmentPoint;
            this.AxesScaleY = scale;

            this.EndUpdate();
        }

        /// <summary>
        /// Sets up the secondary Y axis, aligning it to the primary Y axis.
        /// </summary>
        /// <param name="alignmentPoint">The point at which both Y-axes should have the same value.</param>
        /// <param name="scale">The ratio of the secondary axis values to the primary axis values.  Must be greater than zero.</param>
        /// <param name="seriesNames">The names of the series to assign to the secondary Y axis.</param>
        public void SetUpSecondaryYAxis(double alignmentPoint, double scale, params string[] seriesNames)
        {
            this.BeginUpdate();

            this.RemoveAllSeriesFromSecondaryAxisY();
            if (this.SecondaryAxisY == null)
            {
                this.SecondaryAxisY = new SecondaryAxisY();
            }
            this.SecondaryAxisYVisible = true;
            this.AssignSeriesToSecondaryAxisY(new List<string>(seriesNames));
            this.YAxesAligned = true;
            this.YAxesAlignedAtValue = alignmentPoint;
            this.AxesScaleY = scale;

            this.EndUpdate();
        }

        #endregion Public Methods

        #region Public Static Methods

        /// <summary>
        /// Build a DataTable suitable for WinChart.AutoRender() from a row-per-point DataTable.
        /// Of the columns returned, the first one will be named "Argument", and the rest will be the series' names.
        /// </summary>
        /// <param name="pDT">The row-per-point DataTable.</param>
        /// <param name="columnNameForArguments">The column name to use for X arguments.</param>
        /// <param name="columnNameForSeries">The column name to use for Series Names (Y arguments).</param>
        /// <param name="columnNameForValues">The column name that holds the values.  Must use a numerical data type.</param>
        /// <returns>A new DataTable ready to be assigned as a WinChart data source or passed as a dataSource argument in WinChart.AutoRender().</returns>

        public static DataTable ConvertRowPerPointTable(DataTable pDT,
            string columnNameForArguments,
            string columnNameForSeries,
            string columnNameForValues)
        {
            Dictionary<string, string> throwaway = new Dictionary<string, string>();
            return ConvertRowPerPointTable(pDT, columnNameForArguments, columnNameForSeries, columnNameForValues, ref throwaway);
        }

        /// <summary>
        /// NOTE: this overload is deprecated.
        /// Build a DataTable suitable for WinChart.AutoRender() from a DataTable with X,Y-style arguments.
        /// Of the columns returned, the first one will be named "Argument", and the rest will be the series' names.
        /// </summary>
        /// <param name="pDT">The X,Y-style DataTable.</param>
        /// <param name="columnNameForArguments">The column name to use for X arguments.</param>
        /// <param name="columnNameForSeries">The column name to use for Series Names (Y arguments).</param>
        /// <param name="columnNameForValues">The column name that holds the values.  Must use a numerical data type.</param>
        /// <param name="seriesSources">An empty Dictionary of string keys and values.  Will come back with the new Series Names.</param>
        /// <returns>A new DataTable ready to be assigned as a WinChart data source.</returns>
        public static DataTable ConvertRowPerPointTable(DataTable pDT,
            string columnNameForArguments,
            string columnNameForSeries,
            string columnNameForValues,
            ref Dictionary<string, string> seriesSources)
        {
            // Throw an error if we don't find the column names in the datatable
            if (!pDT.Columns.Contains(columnNameForArguments))
            {
                throw new Exception("The column name for arguments: " + columnNameForArguments + " does not exist in the data table passed.");
            }
            if (!pDT.Columns.Contains(columnNameForSeries))
            {
                throw new Exception("The column name for series: " + columnNameForSeries + " does not exist in the data table passed.");
            }
            if (!pDT.Columns.Contains(columnNameForValues))
            {
                throw new Exception(string.Format("The column name for values: {0} does note exist in the data table passed.", columnNameForValues));
            }
            DataColumn col = pDT.Columns[columnNameForValues];
            if (!(col.DataType == typeof(System.Int16) ||
                    col.DataType == typeof(System.Int32) ||
                    col.DataType == typeof(System.Int64) ||
                    col.DataType == typeof(System.Decimal) ||
                    col.DataType == typeof(System.Double)))
            {
                throw new Exception(string.Format("The column for values: {0} is not a numerical data type.", columnNameForValues));
            }

            // Get the column names for the series
            // Basically, get a listing of column names that we'll have to use for the columns
            // Use a prefix for the columns ("col_") - this will help us if we have a value that starts with a number
            // Also want to remove key characters (replace slashes with underscores, etc.)
            Dictionary<string, string> seriesColumns = new Dictionary<string, string>();
            string colName = string.Empty, colKey = string.Empty;
            foreach (DataRow row in pDT.Rows)
            {
                if (row[columnNameForSeries] == DBNull.Value)
                {
                    colName = "col_NULL";
                    colKey = string.Empty;
                }
                else
                {
                    colName = row[columnNameForSeries].ToString().Replace("/", "_");
                    colKey = row[columnNameForSeries].ToString();
                }
                if (!seriesColumns.ContainsKey(colKey)) { seriesColumns.Add(colKey, colName); }
            }
			seriesColumns = mgCustom.Utils.SortDictionary<string, string>(seriesColumns);

            // Build out the new datatable
            DataTable dt = new DataTable();
            seriesSources.Clear();
            dt.Columns.Add("Argument", typeof(System.String));
            foreach (KeyValuePair<string, string> kvp in seriesColumns)
            {
                dt.Columns.Add(kvp.Value, typeof(System.Decimal));
                seriesSources.Add(kvp.Value, kvp.Value);
            }

            // Initialize all the values to zero so we don't have to do it later
            List<string> arguments = new List<string>();
            //Type T = pDT.Columns[columnNameForArguments].GetType();
            foreach (DataRow row in pDT.Rows)
            {
                if (!arguments.Contains(row[columnNameForArguments].ToString()))
                {
                    DataRow newRow = dt.NewRow();
                    newRow[0] = row[columnNameForArguments].ToString();
                    arguments.Add(row[columnNameForArguments].ToString());
                    dt.Rows.Add(newRow);
                }
            }
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    row[dt.Columns[i]] = 0;
                }
            }

            // Add the values to the dataset
            foreach (DataRow row in pDT.Rows)
            {
                foreach (DataRow newRow in dt.Rows)
                {
                    if (newRow[0].ToString() == row[columnNameForArguments].ToString())
                    {
                        newRow[seriesColumns[row[columnNameForSeries].ToString()]] =
                            double.Parse(newRow[seriesColumns[row[columnNameForSeries].ToString()]].ToString()) + Convert.ToDouble(row[columnNameForValues]);
                        break;
                    }
                }
            }

            return dt;
        }

        #endregion Public Static Methods

        #region Internal Methods

        /// <summary>
        /// Adds a WinChartSeries element to the underlying DevX chart if it isn't already there.  
        /// </summary>
        /// <remarks>
        /// Currently this method only supports adding DevX series objects.
        /// </remarks>
        /// <param name="element">The WinChartSeries element to add.  NOTE: Currently, this method can only add an element if it is a DevX series.</param>
        internal void AddSeriesElement(object element)
        {
            if (element is DevExpress.XtraCharts.Series)
            {
                Series elementAsSeries = (Series)element;
                if (!this.chartMain.Series.Contains(elementAsSeries))
                {
                    this.chartMain.Series.Add(elementAsSeries);
                }
            }
        }

        internal void RemoveSeriesElement(object element)
        {
            if (element is DevExpress.XtraCharts.Series)
            {
                this.chartMain.Series.Remove((Series)element);
            }
        }

        internal void AddAnnotations(List<Annotation> annotations)
        {
            this.chartMain.AnnotationRepository.AddRange(annotations.ToArray());
        }

        #endregion Internal Methods

        #region Event Handlers

        #region Enabled Responders

        private void WinChart_EnabledChanged(object sender, EventArgs e)
        {
            chartMain.Enabled = this.Enabled;
            ribbonMain.Enabled = this.Enabled;
        }

        private void chartMain_EnabledChanged(object sender, EventArgs e)
        {
            if (chartMain.Enabled)
            {
                chartMain.BackColor = Color.White;
            }
            else
            {
                chartMain.BackColor = Color.Gray;
            }
        }

        #endregion Enabled Responders

        #region Timer Handlers

        private void _timer_Tick(object sender, EventArgs e)
        {
            // Update the progress bar
            if (!_enabled) { return; }

            // Update the progress bar
            _progressValue++;
            prg.Position = (int)_progressValue;
        }

        #endregion Timer Handlers

        #region Ribbon Menu Handlers

        #region Chart Type group

        private void gddChartType_GalleryItemClick(object sender, GalleryItemClickEventArgs e)
        {
            // When they click on an item, change out the view
            // Parse the tag as a view type
            //ViewType viewType = (ViewType)Enum.Parse(typeof(ViewType), e.Item.Tag.ToString(), true);
            btnChartType.LargeGlyph = e.Item.Image;
            btnChartType.Caption = e.Item.Hint;
            if (!this.ChartType.ToString().Equals(e.Item.Tag.ToString()))
            {
                this.ChartType = (ViewType)Enum.Parse(typeof(ViewType), e.Item.Tag.ToString());
            }
        }

        private void barColorScheme_EditValueChanged(object sender, EventArgs e)
        {
            if (barColorScheme.EditValue != null)
            {
                string paletteName = barColorScheme.EditValue.ToString();
                Palette appearance = this.chartMain.GetPaletteNames().Contains(paletteName) ?
                    this.chartMain.PaletteRepository[paletteName] : _defaultPalette;
                ChartingAppearance = appearance;
            }
        }

        private void chkPercent_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ValueAsPercent = chkPercent.Checked;
        }

        #endregion Chart Type group

        #region Legend group

        private void barLegend_EditValueChanged(object sender, EventArgs e)
        {
            LegendPosition lp = LegendPosition.None;
            if (Enum.TryParse<LegendPosition>(barLegend.EditValue.ToString(), out lp))
            {
                this.LegendPositioning = lp;
            }
        }

        private void chkValueInLegend_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.MarkAsDirty(UpdatableElement.ValueInLegend);
        }

        private void chkPercentInLegend_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.MarkAsDirty(UpdatableElement.PercentageInLegend);
        }

        #endregion Legend group

        #region General Config group

        private void barGeneralBarShape_EditValueChanged(object sender, EventArgs e)
        {
            this.ModelType = (Bar3DModel)Enum.Parse(typeof(Bar3DModel), barGeneralBarShape.EditValue.ToString());
        }

        private void barSeriesLabelPosition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (GetSampleSeries().Label is BarSeriesLabel)
                {
                    this.BarLabelPosition =
                        (BarSeriesLabelPosition)Enum.Parse(typeof(BarSeriesLabelPosition), barSeriesLabelPosition.EditValue.ToString());
                }
                else if (GetSampleSeries().Label is PieSeriesLabel)
                {
                    this.PieLabelPosition =
                        (PieSeriesLabelPosition)Enum.Parse(typeof(PieSeriesLabelPosition), barSeriesLabelPosition.EditValue.ToString());
                }
                else if (GetSampleSeries().Label is PointSeriesLabel)
                {
                    this.LabelAngle = (LabelAngle)Enum.Parse(typeof(LabelAngle), barSeriesLabelPosition.EditValue.ToString());
                }
                else
                {
                    // Do nothing.
                }
            }
            //catch (System.ArgumentException error)
            catch 
            {
#if DEBUG_EVAN
                Console.WriteLine("Error from barSeriesLabelPosition_EditValueChanged: '{0}'", error.Message);
#endif
            }
        }

        private void chkSeriesLabelVisible_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.LabelsVisible = chkSeriesLabelVisible.Checked;
        }

        #endregion General Config group

        #region Line & Area group

        private void chkPointMarkers_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowMarkers = chkPointMarkers.Checked;
        }

        private void barMarkerShape_EditValueChanged(object sender, EventArgs e)
        {
            Helpers.MarkerKind markerkind = this.MarkerKind;
            if (Enum.TryParse<Helpers.MarkerKind>(barMarkerShape.EditValue.ToString(), out markerkind))
            {
                this.MarkerKind = markerkind;
            }
        }

        private void barMarkerSize_EditValueChanged(object sender, EventArgs e)
        {
            Helpers.MarkerSize markersize = this.MarkerSize;
            if (Enum.TryParse<MarkerSize>(barMarkerSize.EditValue.ToString(), out markersize))
            {
                this.MarkerSize = markersize;
            }
        }

        #endregion Line & Area group

        #region Pie & Doughnut group

        private void barHoleRadius_EditValueChanged(object sender, EventArgs e)
        {
            this.HoleRadius = (HoleRadius)Enum.Parse(typeof(HoleRadius), barHoleRadius.EditValue.ToString());
        }

        private void barExplodedPoints_EditValueChanged(object sender, EventArgs e)
        {
            if (Enum.GetNames(typeof(PieExplodeMode)).Contains(barExplodedPoints.EditValue.ToString()))
            {
                this.ExplodeMode =
                    (PieExplodeMode)Enum.Parse(typeof(PieExplodeMode), barExplodedPoints.EditValue.ToString());
            }
            else if (barExplodedPoints.EditValue.ToString().StartsWith("val_"))
            {
                _holdCleaning = true;
                this.ExplodeMode = PieExplodeMode.UsePoints;
                _holdCleaning = false;
                this.ArgumentExploded = barExplodedPoints.EditValue.ToString().Substring(4);
            }
            else
            {
                // Do nothing.
            }
        }

        private void barExplodedDist_EditValueChanged(object sender, EventArgs e)
        {
            this.ExplodeDistance = Convert.ToUInt32(barExplodedDist.EditValue.ToString());
        }

        #endregion Pie & Doughnut group

        #region Top Options group

        private void chkShowOnlyTop_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowOnlyTopRecords = chkShowOnlyTop.Checked;
        }

        private void barShowTop_EditValueChanged(object sender, EventArgs e)
        {
            int editValue = Convert.ToInt32(barShowTop.EditValue);
            if (editValue < 0)
            {
                this.NumberOfRecordsToShow = 0;
            }
            else
            {
                this.NumberOfRecordsToShow = Convert.ToUInt32(editValue);
            }
        }

        private void chkShowOther_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowOther = chkShowOther.Checked;
        }

        #endregion Top Options group

        #region Print Copy & Save group

        private void btnCopyJpg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool successful = false;
            MemoryStream ms = new MemoryStream();

            try
            {
                this.chartMain.ExportToImage(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);

                Bitmap bump = new Bitmap(ms);
                Clipboard.SetImage(bump);
                successful = true;
            }
            finally
            {
                ms.Close();
            }

            this.OnCopyButtonClicked(new WinChartCopyEventArgs(successful));
        }

        private void btnSaveJpg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != string.Empty)
            {
                string fullPathName = Path.GetFullPath(saveFileDialog1.FileName);
                this.chartMain.ExportToImage(fullPathName, System.Drawing.Imaging.ImageFormat.Jpeg);
                this.OnSaveButtonClicked(new WinChartSaveEventArgs(fullPathName));
            }
        }

        private void btnPrintChart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //this.chartMain.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;

            //ssWinChart_Print printalicious = new ssWinChart_Print(this);
            //printalicious.Orientation = _BaseWinProjectPrintOrientation.Landscape;

            //this.OnPrintButtonClicked(new EventArgs());

            //if (string.IsNullOrEmpty(this.PrintingTitle))
            //{
            //    printalicious.ReportTitle = this.FindForm().Text;
            //}
            //else
            //{
            //    printalicious.ReportTitle = this.PrintingTitle;
            //}

            //if (string.IsNullOrEmpty(this.PrintingFilter))
            //{
            //    printalicious.FilterString = _filter.ToString();
            //}
            //else
            //{
            //    printalicious.FilterString = this.PrintingFilter;
            //}

            PrintChart().ShowPreview(this.ParentForm, true);
        }

        #endregion Print Copy & Save group

        #region Individual Series Controls (Not Currently Implemented)

        private void ui_SeriesDataMemberChanged(object sender, EventArgs e)
        {
            WinChartSeries series = (WinChartSeries)((SeriesUI)sender).Series;
            //series.PrepareDataPoints(series, this.SeriesSources[series.Name], DataSource, XAxisColumnName, _filter);
        }

        private void ui_SeriesGroupChanged(object sender, EventArgs e)
        {
            PrepareGroupings();
        }

        #endregion Individual Series Controls (Not Currently Implemented)

        #region Obsolete

        // Not used.
        private void cboViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ViewType = ViewDictionary[(string)cboViewType.SelectedItem];
        }

        private void lueColorScheme_EditValueChanged(object sender, EventArgs e)
        {
            //// When they change the color scheme, change the appearance
            //if (lueColorScheme.OwnerEdit != null &&
            //    lueColorScheme.OwnerEdit.EditValue != null)
            //{
            //    ChartAppearance appearance = (ChartAppearance)Enum.Parse(typeof(ChartAppearance), lueColorScheme.OwnerEdit.EditValue.ToString(), true);
            //    ChartingAppearance = appearance;
            //}
        }

        private void lueLegend_EditValueChanged(object sender, EventArgs e)
        {
            //if (lueLegend.OwnerEdit != null &&
            //    lueLegend.OwnerEdit.EditValue != null)
            //{
            //    if (lueLegend.OwnerEdit.EditValue.ToString().ToLower() == "none") { LegendPositioning = LegendPosition.None; }
            //    else if (lueLegend.OwnerEdit.EditValue.ToString().ToLower().Contains("top")) { LegendPositioning = LegendPosition.UpperLeft; }
            //    else if (lueLegend.OwnerEdit.EditValue.ToString().ToLower().Contains("middle")) { LegendPositioning = LegendPosition.CenteredLeft; }
            //}
        }

        private void barLegend_HiddenEditor(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barLegend_ShownEditor(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        #endregion Obsolete

        #endregion Ribbon Menu Handlers

        #region Custom Draw handlers

        private void chartMain_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {

        }

        private void chartMain_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            WinChartSeries series = e.Series as WinChartSeries;
            if (series == null)
            {
                return;
            }

            // Handle legend options.  For now, this only works with Pie/Doughnut views.
            if (e.Series.View is PieSeriesViewBase)
            {
                // Get the right argument
                string pointArgument = string.Empty;
                switch (series.ArgumentScaleType)
                {
                    case ScaleType.DateTime:
                        if (series.UseSecondaryAxisX)
                        {
                            pointArgument = e.SeriesPoint.DateTimeArgument.ToString(this.SecondaryAxisX.DateTimeOptions.FormatString);
                        }
                        else
                        {
                            pointArgument = e.SeriesPoint.DateTimeArgument.ToString(this.SecondaryAxisX.DateTimeOptions.FormatString);
                        }
                        break;
                    case ScaleType.Numerical:
                        pointArgument = Convert.ToString(e.SeriesPoint.NumericalArgument);
                        break;
                    case ScaleType.Qualitative:
                        pointArgument = e.SeriesPoint.Argument;
                        break;
                    default:
                        break;
                }

                if (series.IncludeValueInLegend && series.IncludePercentageInLegend)
                {
                    double percentage = (e.SeriesPoint.Values[0] / series.TotalValue) * 100.0;
                    e.LegendText = string.Format("{0}: {1} ({2}; {3:0.00}% of total)", series.Name, pointArgument, e.SeriesPoint.Values[0], percentage);
                }
                else if (series.IncludePercentageInLegend)
                {
                    double percentage = (e.SeriesPoint.Values[0] / series.TotalValue) * 100.0;
                    e.LegendText = string.Format("{0}: {1} ({2:0.00}% of total)", series.Name, pointArgument, percentage);
                }
                else if (series.IncludeValueInLegend)
                {
                    e.LegendText = string.Format("{0}: {1} ({2})", series.Name, pointArgument, e.SeriesPoint.Values[0]);
                }
                else
                {
                    e.LegendText = string.Format("{0}: {1}", series.Name, pointArgument);
                }
            }

            // Handle thresholds.
            ThresholdColorScheme colorScheme = series.ThresholdCollection.GetColorForValue(e.SeriesPoint.Values[0]);
            if (colorScheme.Color1 != null)
            {
                e.SeriesDrawOptions.Color = (Color)colorScheme.Color1;
                e.LegendDrawOptions.Color = (Color)colorScheme.Color1;
            }

            // Corner case city again.  Three different inherited classes that implement the exact same property the exact same way and don't share an interface.
            if (colorScheme.Color2 != null)
            {
                if (e.SeriesDrawOptions is BarDrawOptions && ((BarDrawOptions)e.SeriesDrawOptions).FillStyle.Options is FillOptionsColor2Base)
                {
                    FillOptionsColor2Base options = (FillOptionsColor2Base)((BarDrawOptions)e.SeriesDrawOptions).FillStyle.Options;
                    options.Color2 = (Color)colorScheme.Color2;
                }
                else if (e.SeriesDrawOptions is PieDrawOptions && ((PieDrawOptions)e.SeriesDrawOptions).FillStyle.Options is FillOptionsColor2Base)
                {
                    FillOptionsColor2Base options = (FillOptionsColor2Base)((PieDrawOptions)e.SeriesDrawOptions).FillStyle.Options;
                    FillOptionsColor2Base legendOptions = (FillOptionsColor2Base)((PieDrawOptions)e.LegendDrawOptions).FillStyle.Options;
                    options.Color2 = legendOptions.Color2 = (Color)colorScheme.Color2;
                }
                else if (e.SeriesDrawOptions is FunnelDrawOptions && ((FunnelDrawOptions)e.SeriesDrawOptions).FillStyle.Options is FillOptionsColor2Base)
                {
                    FillOptionsColor2Base options = (FillOptionsColor2Base)((FunnelDrawOptions)e.SeriesDrawOptions).FillStyle.Options;
                    FillOptionsColor2Base legendOptions = (FillOptionsColor2Base)((FunnelDrawOptions)e.LegendDrawOptions).FillStyle.Options;
                    options.Color2 = legendOptions.Color2 = (Color)colorScheme.Color2;
                }
            }
        }

        #endregion Custom Draw handlers

        #region Mouse Event handlers

        private void chartMain_MouseClick(object sender, MouseEventArgs e)
        {
            // CalcHitInfo is not supported in 3D
            if (chartMain.Diagram is Diagram3D)
            {
                return;
            }

            ChartHitInfo hi = this.chartMain.CalcHitInfo(e.X, e.Y);

            WinChartSeries series = hi.Series as WinChartSeries;
            SeriesPoint point = hi.SeriesPoint;
            if (series != null && point != null)
            {
                this.OnSeriesPointClicked(new WinChartMouseEventArgs(e, series, point));
            }
        }

        private void chartMain_MouseHover(object sender, EventArgs e)
        {

        }

        private void chartMain_MouseMove(object sender, MouseEventArgs e)
        {
            // CalcHitInfo is not supported in 3D
            if (chartMain.Diagram is Diagram3D)
            {
                return;
            }

            ChartHitInfo hi = this.chartMain.CalcHitInfo(e.X, e.Y);

            WinChartSeries series = hi.Series as WinChartSeries;
            SeriesPoint point = hi.SeriesPoint;
            WinChartMouseEventArgs eventargs = new WinChartMouseEventArgs(e, series, point);
            this.OnSeriesPointMouseOver(eventargs);
        }

        #endregion Mouse Event handlers

        #endregion Event Handlers

        #region Private Methods

        /// <summary>
        /// Gets a sample series to test for ViewType qualities.
        /// </summary>
        /// <returns>A series with the same ViewType as the Chart's.</returns>
        private Series GetSampleSeries()
        {
            if (this.Series.Count > 0)
            {
                return this.Series[0];
            }
            else
            {
                return new WinChartSeries(this, "sample", this.ChartType);
            }
        }

        ///// <summary>
        ///// Updates the chart and control settings to reflect the changes made to a property of the VisualSettingsObject.
        ///// </summary>
        ///// <param name="settingName">The name of the property that needs updating.</param>
        //private void UpdateThisSetting(string settingName)
        //{
        //    List<string> list = new List<string>();
        //    list.Add(settingName);
        //    UpdateThisSetting(list);
        //}

        //        /// <summary>
        //        /// Updates the chart and control settings to reflect changes made to one or more properties of the VisualSettingsObject.
        //        /// </summary>
        //        /// <param name="settingNames">The list of settings to update, by their names.</param>
        //        private void UpdateThisSetting(List<string> settingNames)
        //        {
        //            foreach (string settingName in settingNames)
        //            {
        //                PropertyDescriptor property = TypeDescriptor.GetProperties(_visualSettings).Find(settingName, true);
        //                if (property == null)
        //                {
        //#if DEBUG_EVAN
        //                    Console.WriteLine("Property {0} not found in VisualSettingsObject.", settingName);
        //#endif
        //                    //throw appropriate error here.
        //                    return;
        //                }
        //                _dirtyProperties.Add(property);
        //            }
        //            CleanProperties();
        //        }

        /// <summary>
        /// Sets the barSeriesLabelPosition control's options to BarSeriesLabelPosition options. 
        /// </summary>
        private void SetBarLabelPositionOptions(bool excludeTop)
        {
            // Set up the bar shapes
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(BarSeriesLabelPosition)))
            {
                if (!excludeTop || string.Compare(s, BarSeriesLabelPosition.Top.ToString()) != 0)
                {
					dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
                }
            }

            lueSeriesLabelPosition.Columns.Clear();
            lueSeriesLabelPosition.DataSource = null;
            lueSeriesLabelPosition.DataSource = dt;
            lueSeriesLabelPosition.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueSeriesLabelPosition.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueSeriesLabelPosition.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            // Set the current value.
            barSeriesLabelPosition.EditValue = this.BarLabelPosition.ToString();
        }

        /// <summary>
        /// Sets the barSeriesLabelPosition control's options to PieSeriesLabelPosition options. 
        /// </summary>
        private void SetPieLabelPositionOptions()
        {
            // Set up the bar shapes
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(PieSeriesLabelPosition)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueSeriesLabelPosition.Columns.Clear();
            lueSeriesLabelPosition.DataSource = null;
            lueSeriesLabelPosition.DataSource = dt;
            lueSeriesLabelPosition.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueSeriesLabelPosition.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueSeriesLabelPosition.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            // Set the current value.
            barSeriesLabelPosition.EditValue = this.PieLabelPosition.ToString();
        }

        /// <summary>
        /// Sets the barSeriesLabelPosition control's options to PointSeriesLabelPosition options. 
        /// </summary>
        private void SetPointLabelPositionOptions()
        {
            // Set up the data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(LabelAngle)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueSeriesLabelPosition.Columns.Clear();
            lueSeriesLabelPosition.DataSource = null;
            lueSeriesLabelPosition.DataSource = dt;
            lueSeriesLabelPosition.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueSeriesLabelPosition.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueSeriesLabelPosition.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Name"));

            // Set the current value.
            barSeriesLabelPosition.EditValue = this.LabelAngle.ToString();
        }

        /// <summary>
        /// Sets the barExplodedPoints control's options.
        /// </summary>
        private void SetExplodedPointsOptions()
        {
            // Set up the legend positions
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            dt.Rows.Add(new object[] { PieExplodeMode.None.ToString(), PieExplodeMode.None.ToString() });
            dt.Rows.Add(new object[] { PieExplodeMode.All.ToString(), PieExplodeMode.All.ToString() });
            dt.Rows.Add(new object[] { PieExplodeMode.MinValue.ToString(), PieExplodeMode.MinValue.ToString() });
            dt.Rows.Add(new object[] { PieExplodeMode.MaxValue.ToString(), PieExplodeMode.MaxValue.ToString() });

            // Load `em up
            if (chartMain.Series.Count > 0)
            {
                foreach (SeriesPoint point in GetSampleSeries().Points)
                {
                    dt.Rows.Add(new object[] { "val_" + point.Argument, point.Argument });
                }
            }

            lueExplodedPoints.Columns.Clear();
            lueExplodedPoints.DataSource = null;
            lueExplodedPoints.DataSource = dt;
            lueExplodedPoints.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueExplodedPoints.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueExplodedPoints.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Option"));

            // Select the correct value 
            if (this.ExplodeMode != PieExplodeMode.UsePoints)
            {
                barExplodedPoints.EditValue = this.ExplodeMode.ToString();
            }
            else
            {
                barExplodedPoints.EditValue = "val_" + this.ArgumentExploded;
            }
        }

        /// <summary>
        /// Sets the barMarkerShape control's options.
        /// </summary>
        private void SetMarkerShapeOptions()
        {
            // Set up the legend positions
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(Helpers.MarkerKind)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueMarkerShape.Columns.Clear();
            lueMarkerShape.DataSource = null;
            lueMarkerShape.DataSource = dt;
            lueMarkerShape.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueMarkerShape.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueMarkerShape.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Option"));

            // Set the current value.
            barMarkerShape.EditValue = this.MarkerKind.ToString();
        }

        /// <summary>
        /// Sets the barMarkerSize control's options.
        /// </summary>
        private void SetMarkerSizeOptions()
        {
            // Set up the legend positions
            DataTable dt = new DataTable();
            dt.Columns.Add("ValueField", typeof(System.String));
            dt.Columns.Add("DisplayField", typeof(System.String));

            // Load `em up
            foreach (string s in Enum.GetNames(typeof(Helpers.MarkerSize)))
            {
				dt.Rows.Add(new object[] { s, mgCustom.Utils.AlterCaptionFromFieldName(s.Replace("_", "")) });
            }

            lueMarkerSize.Columns.Clear();
            lueMarkerSize.DataSource = null;
            lueMarkerSize.DataSource = dt;
            lueMarkerSize.ValueMember = dt.Columns["ValueField"].ColumnName;
            lueMarkerSize.DisplayMember = dt.Columns["DisplayField"].ColumnName;
            lueMarkerSize.Columns.Add(new LookUpColumnInfo(dt.Columns["DisplayField"].ColumnName, 200, "Option"));

            // Set the current value.
            barMarkerSize.EditValue = this.MarkerSize.ToString();
        }

        /// <summary>
        /// Marks a list of chart elements as dirty and needing an update.
        /// </summary>
        /// <param name="elements">The list of chart elements to mark as dirty.</param>
        private void MarkAsDirty(List<UpdatableElement> elements)
        {
            foreach (UpdatableElement element in elements)
            {
                this._dirtyElements[element] = true;
            }

            if (!_holdCleaning)
            {
                this.CleanAllElements();
            }
        }

        /// <summary>
        /// Marks one or more chart elements as dirty and needing of an update.
        /// </summary>
        /// <param name="elements">The elements to mark as dirty.</param>
        private void MarkAsDirty(params UpdatableElement[] elements)
        {
            this.MarkAsDirty(new List<UpdatableElement>(elements));
        }

        /// <summary>
        /// Updates all elements marked as dirty.
        /// </summary>
        private void CleanAllElements()
        {
            if (_holdCleaning)
            {
                return;
            }

            _holdCleaning = true;

            List<UpdatableElement> elements = new List<UpdatableElement>(_dirtyElements.Keys);
            foreach (UpdatableElement element in elements)
            {
                if (_dirtyElements[element] == true)
                {
                    CleanElement(element);
                    _dirtyElements[element] = false;
                }
            }

            _holdCleaning = false;
        }

        /// <summary>
        /// Updates a chart element.
        /// </summary>
        /// <param name="element">The element to update.</param>
        private void CleanElement(UpdatableElement element)
        {
            switch (element)
            {
                case UpdatableElement.ChartType:
                    #region ChartType

                    try
                    {
                        foreach (WinChartSeries series in this.Series)
                        {
                            series.ChangeView(ChartType);
                        }
                    }
                    //catch (ArgumentException argex)
                    catch 
                    {
                        // The data source isn't sufficient to plot this type of chart.
                        // Hold onto the bad view type for reporting, and make sure both ViewType and _oldViewType are the most recent safe type.
                        ViewType badViewType = this.ChartType;
                        this.ChartType = _oldViewType;
                        _oldViewType = this.ChartType;

                        // Put the chart back the way it was.
                        foreach (Series series in chartMain.Series)
                        {
                            series.ChangeView(this.ChartType);
                        }

                        // Tell the user what was wrong.
                        MessageBox.Show(this.FindForm(), string.Format("The data source does not support the chart type {0}.", badViewType.ToString()),
                            "Chart Type Doesn't Match Data Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Turn on Rotate and Zoom options for 3D view types.
                    if (chartMain.Diagram is Diagram3D)
                    {
                        Diagram3D diagram = (Diagram3D)chartMain.Diagram;
                        diagram.RuntimeRotation = true;
                        diagram.RuntimeZooming = true;
                    }

                    // Set the Primary and Secondary Y-Axis
                    if (this.chartMain.Diagram is XYDiagram)
                    {
                        ((XYDiagram)chartMain.Diagram).AxisX.Label.Angle = (int)this.PrimaryAxisXLabelAngle;
                        ((XYDiagram)chartMain.Diagram).AxisX.GridSpacing = (double)this.PrimaryAxisXLabelTickScale;

                        UpdateSecondaryAxisX();
                        UpdateSecondaryAxisY();

                        ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Format = this.PrimaryAxisYFormat;
                        if (this.PrimaryAxisYFormatPrecision >= 0)
                        {
                            ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Precision = this.PrimaryAxisYFormatPrecision;
                        }
                    }

                    SeriesViewBase sampleView = this.GetSampleSeries().View;

                    // Set the Label position options.
                    if (sampleView is BarSeriesView)
                    {
                        this.barSeriesLabelPosition.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        this.SetBarLabelPositionOptions(sampleView is StackedBarSeriesView || sampleView is StackedBar3DSeriesView);
                    }
                    else if (sampleView is PieSeriesView)
                    {
                        this.barSeriesLabelPosition.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        this.SetPieLabelPositionOptions();
                    }
                    else if (sampleView is PointSeriesViewBase)
                    {
                        this.barSeriesLabelPosition.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        this.SetPointLabelPositionOptions();
                    }
                    else
                    {
                        this.barSeriesLabelPosition.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    // Stacked bar groups don't support BarSeriesLabelPosition.Top, so if the new view is stacked, change the Top to Top Inside.
                    if (BarLabelPosition == BarSeriesLabelPosition.Top &&
                        (sampleView is StackedBarSeriesView || sampleView is StackedBar3DSeriesView))
                    {
                        BarLabelPosition = BarSeriesLabelPosition.TopInside;
                    }

                    // Set up the Ribbon menu.
                    if (sampleView is PieSeriesViewBase)
                    {
                        groupPieDoughnut.Visible = true;
                        groupTopOptions.Visible = true;
                    }
                    else
                    {
                        groupPieDoughnut.Visible = false;
                        groupTopOptions.Visible = false;
                        this.ShowOnlyTopRecords = false;
                    }

                    if (sampleView is PointSeriesViewBase)
                    {
                        groupLineArea.Visible = true;
                    }
                    else
                    {
                        groupLineArea.Visible = false;
                    }

                    if (sampleView is Bar3DSeriesView)
                    {
                        barGeneralBarShape.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        barGeneralBarShape.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    SetOverlappingMode();

                    //Update the combobox to match.
                    //foreach (string key in ViewDictionary.Keys)
                    //{
                    //    if (ViewDictionary[key] == ViewType && cboViewType.Items.Contains(key))
                    //    {
                    //        cboViewType.SelectedItem = key;
                    //        break;
                    //    }
                    //}

                    //Enable or disable the Value as Percent checkbox to match the view type.
                    Series sample = this.GetSampleSeries();
                    if (sample.PointOptions is SimplePointOptions || sample.PointOptions is FullStackedPointOptions)
                    {
                        chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        chkPercent.Checked = ValueAsPercent;
                    }
                    else
                    {
                        ValueAsPercent = false;
                        chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    // Set the button Glyph
                    bool itemFound = false;
                    foreach (GalleryItemGroup group in gddChartType.Gallery.Groups)
                    {
                        foreach (GalleryItem item in group.Items)
                        {
                            if (this.ChartType.ToString().Equals(item.Tag))
                            {
                                btnChartType.LargeGlyph = item.Image;
                                btnChartType.Caption = item.Hint;
                                btnChartType.Hint = item.Hint;
                                itemFound = true;
                                break;
                            }
                        }
                        if (itemFound)
                        {
                            break;
                        }
                    }

                    #endregion ChartType
                    break;
                case UpdatableElement.LabelsVisible:
                    #region LabelsVisible

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.Label.Visible = this.LabelsVisible;
                    }

                    SetOverlappingMode();

                    #endregion LabelsVisible
                    break;

                case UpdatableElement.ChartTitle:
                    #region ChartTitle

                    if (String.Compare(ChartTitle, string.Empty, false) == 0)
                    {
                        chartMain.Titles.Clear();
                    }
                    else
                    {
                        if (chartMain.Titles.Count > 0)
                        {
                            chartMain.Titles[0].Text = ChartTitle;
                        }
                        else
                        {
                            ChartTitle chartTitle = new ChartTitle();
                            chartTitle.Text = ChartTitle;
                            chartMain.Titles.Add(chartTitle);
                        }
                    }

                    #endregion ChartTitle
                    break;
                case UpdatableElement.ValueAsPercent:
                    #region ValueAsPercent

                    chkPercent.Checked = ValueAsPercent;
                    foreach (WinChartSeries series in this.Series)
                    {
                        //Welcome to corner case city.  
                        //To begin with, two separate descendents of PointOptions support PercentOptions.  These two descendents
                        //do not share an interface, so we have to handle them separately.
                        if (series.Label.PointOptions is SimplePointOptions)
                        {
                            ((SimplePointOptions)series.Label.PointOptions).PercentOptions.ValueAsPercent = ValueAsPercent;
                            if (ValueAsPercent)
                            {
                                ((SimplePointOptions)series.Label.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
                                ((SimplePointOptions)series.Label.PointOptions).ValueNumericOptions.Precision = 0;
                            }
                            else
                            {
                                SetSeriesFormat(series);
                            }
                        }
                        else if (series.Label.PointOptions is FullStackedPointOptions)
                        {
                            ((FullStackedPointOptions)series.Label.PointOptions).PercentOptions.ValueAsPercent = ValueAsPercent;
                            if (ValueAsPercent)
                            {
                                ((FullStackedPointOptions)series.Label.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
                                ((FullStackedPointOptions)series.Label.PointOptions).ValueNumericOptions.Precision = 0;
                            }
                            else
                            {
                                SetSeriesFormat(series);
                            }
                        }
                        else
                        {
                            //PercentOptions is not supported, so make sure the format is correct.
                            SetSeriesFormat(series);
                        }
                    }

                    #endregion ValueAsPercent
                    break;
                case UpdatableElement.Legend:
                    #region LegendPosition

                    //Set the Legend Position in the chart.
                    if (LegendPositioning == LegendPosition.None)
                    {
                        chartMain.Legend.Visible = false;
                        //cboLegend.SelectedItem = "None";
                        barLegend.EditValue = "None";
                    }
                    else
                    {
                        chartMain.Legend.Visible = true;

                        // Set the Horizontal Alignment
                        if (this.LegendPositioning.ToString().ToLower().Contains("left"))
                        {
                            chartMain.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
                        }
                        else if (this.LegendPositioning.ToString().ToLower().Contains("right"))
                        {
                            chartMain.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
                        }
                        else
                        {
                            chartMain.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
                        }

                        // Set the Vertical Alignment
                        if (this.LegendPositioning == LegendPosition.TopCenter)
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
                        }
                        else if (this.LegendPositioning == LegendPosition.BottomCenter)
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
                        }
                        else if (this.LegendPositioning.ToString().ToLower().Contains("upper"))
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.Top;
                        }
                        else if (this.LegendPositioning.ToString().ToLower().Contains("lower"))
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.Bottom;
                        }
                        else
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.Center;
                        }

                        // Set the direction of the legend
                        if (this.LegendPositioning.ToString().ToLower().EndsWith("center"))
                        {
                            chartMain.Legend.Direction = LegendDirection.LeftToRight;
                        }
                        else
                        {
                            chartMain.Legend.Direction = LegendDirection.TopToBottom;
                        }

                        barLegend.EditValue = this.LegendPositioning.ToString();
                    }

                    #endregion LegendPosition
                    break;
                case UpdatableElement.ValueInLegend:
                    #region ShowValueInLegend

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.IncludeValueInLegend = this.ShowValueInLegend;
                    }

                    chartMain.Refresh();

                    #endregion ShowValueInLegend
                    break;
                case UpdatableElement.PercentageInLegend:
                    #region ShowPercentageInLegend

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.IncludePercentageInLegend = this.ShowPercentageInLegend;
                    }

                    chartMain.Refresh();

                    #endregion ShowPercentageInLegend
                    break;
                case UpdatableElement.ChartAppearance:
                    #region ChartAppearance

                    chartMain.PaletteName = this.ChartingAppearance != null ?
                        this.ChartingAppearance.ToString() : _defaultPalette.ToString();

                    if (barColorScheme.EditValue != null &&
                        string.Compare(barColorScheme.EditValue.ToString(), this.ChartingAppearance.ToString()) != 0)
                    {
                        barColorScheme.EditValue = this.ChartingAppearance.ToString();
                    }

                    #endregion ChartAppearance
                    break;

                case UpdatableElement.ShowOnlyTopRecords:
                    #region ShowOnlyTopRecords

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.TopNOptions.Enabled = this.ShowOnlyTopRecords;
                    }

                    if (GetNumberOfLabels() > 250)
                    {
                        this.LabelsVisible = false;
                    }

                    chkShowOnlyTop.Checked = barShowTop.Enabled = chkShowOther.Enabled = this.ShowOnlyTopRecords;

                    #endregion ShowOnlyTopRecords
                    break;
                case UpdatableElement.NumberOfRecordsToShow:
                    #region NumberOfRecordsToShow

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.TopNOptions.Count = Convert.ToInt32(this.NumberOfRecordsToShow);
                    }

                    chkShowOnlyTop.Caption = string.Format("Show Only Top {0} Records", this.NumberOfRecordsToShow);
                    barShowTop.EditValue = this.NumberOfRecordsToShow;

                    #endregion NumberOfRecordsToShow
                    break;
                case UpdatableElement.ShowOther:
                    #region ShowOther

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.TopNOptions.ShowOthers = this.ShowOther;
                    }

                    chkShowOther.Checked = this.ShowOther;

                    #endregion ShowOther
                    break;

                case UpdatableElement.AreaLineLabelAngle:
                    #region LabelAngle

					int angle = int.Parse(mgCustom.Utils.ExtractDigits(this.LabelAngle.ToString()));

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.Label is PointSeriesLabel)
                        {
                            ((PointSeriesLabel)series.Label).Angle = angle;
                        }
                    }

                    #endregion LabelAngle
                    break;
                case UpdatableElement.MarkerKind:
                    #region MarkerKind

                    DevExpress.XtraCharts.MarkerKind kind = DevExpress.XtraCharts.MarkerKind.Circle;
                    int starPoints = 0;
                    if (this.MarkerKind.ToString().StartsWith("Star"))
                    {
                        kind = DevExpress.XtraCharts.MarkerKind.Star;
						starPoints = int.Parse(mgCustom.Utils.ExtractDigits(this.MarkerKind.ToString()));
                    }
                    else
                    {
                        kind = (DevExpress.XtraCharts.MarkerKind)Enum.Parse(typeof(DevExpress.XtraCharts.MarkerKind), this.MarkerKind.ToString());
                    }

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is PointSeriesView)
                        {
                            ((PointSeriesView)series.View).PointMarkerOptions.Kind = kind;
                            if (kind == DevExpress.XtraCharts.MarkerKind.Star)
                            {
                                ((PointSeriesView)series.View).PointMarkerOptions.StarPointCount = starPoints;
                            }
                        }
                    }
                    barMarkerShape.EditValue = this.MarkerKind.ToString();

                    #endregion MarkerKind
                    break;
                case UpdatableElement.MarkerSize:
                    #region MarkerSize

					int size = int.Parse(mgCustom.Utils.ExtractDigits(this.MarkerSize.ToString()));
                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is PointSeriesView)
                        {
                            ((PointSeriesView)series.View).PointMarkerOptions.Size = size;
                        }
                    }

                    barMarkerSize.EditValue = this.MarkerSize.ToString();

                    #endregion MarkerSize
                    break;
                case UpdatableElement.ShowMarkers:
                    #region ShowMarkers

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is LineSeriesView)
                        {
                            ((LineSeriesView)series.View).LineMarkerOptions.Visible = this.ShowMarkers;
                        }
                    }
                    chkPointMarkers.Checked = this.ShowMarkers;
                    barMarkerShape.Enabled = barMarkerSize.Enabled = this.ShowMarkers;

                    #endregion ShowMarkers
                    break;

                case UpdatableElement.PrimaryAxisXLabel:
                    #region PrimaryAxisXLabel

                    if (chartMain.Diagram is XYDiagram)
                    {
                        AxisX axisX = ((XYDiagram)chartMain.Diagram).AxisX;
                        axisX.Label.Angle = (int)this.PrimaryAxisXLabelAngle;
                        axisX.Label.Visible = this.PrimaryAxisXLabelsVisible;
                        axisX.GridSpacing = (double)this.PrimaryAxisXLabelTickScale;
                    }

                    #endregion PrimaryAxisXLabel
                    break;
                case UpdatableElement.PrimaryAxisXAlignment:
                    #region PrimaryAxisXAlignment

                    if (this.PrimaryAxisX != null)
                    {
                        this.PrimaryAxisX.Alignment = _visualSettings.PrimaryAxisXAlignment;
                    }

                    #endregion PrimaryAxisXAlignment
                    break;
                case UpdatableElement.SecondaryAxisX:
                    UpdateSecondaryAxisX();
                    break;
                case UpdatableElement.SecondaryAxisY:
                    UpdateSecondaryAxisY();
                    break;
                case UpdatableElement.PrimaryAxisYTitle:
                    #region PrimaryAxisYTitle

                    if (this.PrimaryAxisY != null)
                    {
                        this.PrimaryAxisY.Title.Text = _visualSettings.PrimaryAxisYTitle;
                        if (_visualSettings.PrimaryAxisYTitle.Trim().Length > 0)
                        {
                            this.PrimaryAxisY.Title.Visible = true;
                        }
                        else
                        {
                            this.PrimaryAxisY.Title.Visible = false;
                        }
                    }

                    #endregion
                    break;
                case UpdatableElement.SecondaryAxisYTitle:
                    #region SecondaryAxisYTitle

                    if (this.SecondaryAxisY != null)
                    {
                        this.SecondaryAxisY.Title.Text = _visualSettings.SecondaryAxisYTitle;
                        if (_visualSettings.SecondaryAxisYTitle.Trim().Length > 0)
                        {
                            this.SecondaryAxisY.Title.Visible = true;
                        }
                        else
                        {
                            this.SecondaryAxisY.Title.Visible = false;
                        }
                    }
                    #endregion
                    break;

                case UpdatableElement.ModelType:
                    #region ModelType

                    foreach (WinChartSeries series in this.Series)
                    {
                        Bar3DSeriesView view = series.View as Bar3DSeriesView;
                        if (view != null)
                        {
                            view.Model = this.ModelType;
                        }
                    }
                    barGeneralBarShape.EditValue = ModelType.ToString();

                    #endregion ModelType
                    break;
                case UpdatableElement.BarLabelPosition:
                    #region BarLabelPosition

                    foreach (WinChartSeries series in this.Series)
                    {
                        BarSeriesLabel label = series.Label as BarSeriesLabel;
                        if (label != null)
                        {
                            label.Position = this.BarLabelPosition;
                        }
                    }

                    if (this.GetSampleSeries().Label is BarSeriesLabel)
                    {
                        barSeriesLabelPosition.EditValue = BarLabelPosition.ToString();
                    }

                    #endregion BarLabelPosition
                    break;

                case UpdatableElement.ExplodeDistance:
                    #region ExplodeDistance

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is PieSeriesViewBase)
                        {
                            ((PieSeriesViewBase)series.View).ExplodedDistancePercentage = Convert.ToDouble(this.ExplodeDistance);
                        }
                    }

                    #endregion ExplodeDistance
                    break;
                case UpdatableElement.ExplodeMode:
                    #region ExplodeMode

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is PieSeriesViewBase)
                        {
                            PieSeriesViewBase pieView = (PieSeriesViewBase)series.View;
                            pieView.ExplodeMode = this.ExplodeMode;
                            if (this.ExplodeMode == PieExplodeMode.UsePoints)
                            {
                                pieView.ExplodedPoints.Clear();
                                foreach (SeriesPoint point in series.Points)
                                {
                                    if (point.Argument == this.ArgumentExploded)
                                    {
                                        pieView.ExplodedPoints.Add(point);
                                    }
                                }
                            }
                        }
                    }

                    #endregion ExplodeMode
                    break;
                case UpdatableElement.PieLabelPosition:
                    #region PieLabelPosition

                    foreach (WinChartSeries series in this.Series)
                    {
                        PieSeriesLabel label = series.Label as PieSeriesLabel;
                        if (label != null)
                        {
                            label.Position = this.PieLabelPosition;
                        }
                    }

                    if (GetSampleSeries().Label is PieSeriesLabel)
                    {
                        barSeriesLabelPosition.EditValue = PieLabelPosition.ToString();
                    }

                    #endregion PieLabelPosition
                    break;
                case UpdatableElement.HoleRadius:
                    #region HoleRadius

                    barHoleRadius.EditValue = this.HoleRadius.ToString();

                    #endregion HoleRadius
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Updates internal settings to match a change to a visual property.
        /// </summary>
        /// <param name="property">The property that changed.</param>
        protected bool CleanProperty(PropertyDescriptor property)
        {
            bool shouldUpdateUI = false;
            switch (property.Name)
            {
                case "ViewType":
                    #region ViewType

                    try
                    {
                        foreach (WinChartSeries series in this.Series)
                        {
                            series.ChangeView(ChartType);
                        }
                    }
                    catch 
                    {
                        // The data source isn't sufficient to plot this type of chart.
                        // Hold onto the bad view type for reporting, and make sure both ViewType and _oldViewType are the most recent safe type.
                        ViewType badViewType = this.ChartType;
                        this.ChartType = _oldViewType;
                        _oldViewType = this.ChartType;

                        // Put the chart back the way it was.
                        foreach (Series series in chartMain.Series)
                        {
                            series.ChangeView(this.ChartType);
                        }

                        // Tell the user what was wrong.
                        MessageBox.Show(this.FindForm(), string.Format("The data source does not support the chart type {0}.", badViewType.ToString()),
                            "Chart Type Doesn't Match Data Source", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Turn on Rotate and Zoom options for 3D view types.
                    if (chartMain.Diagram is Diagram3D)
                    {
                        Diagram3D diagram = (Diagram3D)chartMain.Diagram;
                        diagram.RuntimeRotation = true;
                        diagram.RuntimeZooming = true;
                    }

                    // Set the Primary and Secondary Y-Axis
                    if (this.chartMain.Diagram is XYDiagram)
                    {
                        ((XYDiagram)chartMain.Diagram).AxisX.Label.Angle = (int)this.PrimaryAxisXLabelAngle;
                        ((XYDiagram)chartMain.Diagram).AxisX.GridSpacing = (double)this.PrimaryAxisXLabelTickScale;

                        UpdateSecondaryAxisX();
                        UpdateSecondaryAxisY();

                        ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Format = this.PrimaryAxisYFormat;
                        if (this.PrimaryAxisYFormatPrecision >= 0)
                        {
                            ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Precision = this.PrimaryAxisYFormatPrecision;
                        }
                    }

                    //Stacked bar groups don't support BarSeriesLabelPosition.Top, so if the new view is stacked, change the Top to Top Inside.
                    if (BarLabelPosition == BarSeriesLabelPosition.Top &&
                        (this.GetSampleSeries().View is StackedBarSeriesView || this.GetSampleSeries().View is ISupportStackedGroup))
                    {
                        BarLabelPosition = BarSeriesLabelPosition.TopInside;
                    }

                    if (GetSampleSeries().View is PieSeriesViewBase)
                    {
                        groupPieDoughnut.Visible = true;
                        groupTopOptions.Visible = true;
                    }
                    else
                    {
                        groupPieDoughnut.Visible = false;
                        groupTopOptions.Visible = false;
                        this.ShowOnlyTopRecords = false;
                    }

                    SetOverlappingMode();

                    //Update the combobox to match.
                    //foreach (string key in ViewDictionary.Keys)
                    //{
                    //    if (ViewDictionary[key] == ViewType && cboViewType.Items.Contains(key))
                    //    {
                    //        cboViewType.SelectedItem = key;
                    //        break;
                    //    }
                    //}

                    //Enable or disable the Value as Percent checkbox to match the view type.
                    Series sample = this.GetSampleSeries();
                    if (sample.PointOptions is SimplePointOptions || sample.PointOptions is FullStackedPointOptions)
                    {
                        chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        chkPercent.Checked = ValueAsPercent;
                    }
                    else
                    {
                        ValueAsPercent = false;
                        chkPercent.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    //Update the SeriesUIs
                    shouldUpdateUI = true;

                    // Set the button Glyph
                    bool itemFound = false;
                    foreach (GalleryItemGroup group in gddChartType.Gallery.Groups)
                    {
                        foreach (GalleryItem item in group.Items)
                        {
                            if (this.ChartType.ToString().Equals(item.Tag))
                            {
                                btnChartType.LargeGlyph = item.Image;
                                btnChartType.Caption = item.Hint;
                                btnChartType.Hint = item.Hint;
                                itemFound = true;
                                break;
                            }
                        }
                        if (itemFound)
                        {
                            break;
                        }
                    }

                    #endregion ViewType
                    break;
                case "LabelsVisible":
                    #region LabelsVisible

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.Label.Visible = this.LabelsVisible;
                    }

                    SetOverlappingMode();
                    shouldUpdateUI = true;

                    #endregion LabelsVisible
                    break;

                case "ChartTitle":
                    #region ChartTitle

                    if (String.Compare(ChartTitle, string.Empty, false) == 0)
                    {
                        chartMain.Titles.Clear();
                    }
                    else
                    {
                        if (chartMain.Titles.Count > 0)
                        {
                            chartMain.Titles[0].Text = ChartTitle;
                        }
                        else
                        {
                            ChartTitle chartTitle = new ChartTitle();
                            chartTitle.Text = ChartTitle;
                            chartMain.Titles.Add(chartTitle);
                        }
                    }

                    #endregion ChartTitle
                    break;
                case "ValueAsPercent":
                    #region ValueAsPercent

                    chkPercent.Checked = ValueAsPercent;
                    foreach (WinChartSeries series in this.Series)
                    {
                        //Welcome to corner case city.  
                        //To begin with, two separate descendents of PointOptions support PercentOptions.  These two descendents
                        //do not share an interface, so we have to handle them separately.
                        if (series.PointOptions is SimplePointOptions)
                        {
                            ((SimplePointOptions)series.PointOptions).PercentOptions.ValueAsPercent = ValueAsPercent;
                            if (ValueAsPercent)
                            {
                                ((SimplePointOptions)series.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
                                ((SimplePointOptions)series.PointOptions).ValueNumericOptions.Precision = 0;
                            }
                            else
                            {
                                SetSeriesFormat(series);
                            }
                        }
                        else if (series.PointOptions is FullStackedPointOptions)
                        {
                            ((FullStackedPointOptions)series.PointOptions).PercentOptions.ValueAsPercent = ValueAsPercent;
                            if (ValueAsPercent)
                            {
                                ((FullStackedPointOptions)series.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
                                ((FullStackedPointOptions)series.PointOptions).ValueNumericOptions.Precision = 0;
                            }
                            else
                            {
                                SetSeriesFormat(series);
                            }
                        }
                        else
                        {
                            //PercentOptions is not supported, so make sure the format is correct.
                            SetSeriesFormat(series);
                        }
                    }

                    #endregion ValueAsPercent
                    break;
                case "LegendPosition":
                    #region LegendPosition

                    //Set the Legend Position in the chart.
                    if (LegendPositioning == LegendPosition.None)
                    {
                        chartMain.Legend.Visible = false;
                        //cboLegend.SelectedItem = "None";
                        barLegend.EditValue = "None";
                    }
                    else
                    {
                        chartMain.Legend.Visible = true;
                        //The only Horizontal Alignment allowed by our custom enum is Left Outside
                        chartMain.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
                        if (LegendPositioning == LegendPosition.CenteredLeft)
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.Center;
                            //cboLegend.SelectedItem = "Middle Left";
                            barLegend.EditValue = LegendPosition.CenteredLeft.ToString();
                        }
                        else
                        {
                            chartMain.Legend.AlignmentVertical = LegendAlignmentVertical.Top;
                            //cboLegend.SelectedItem = "Top Left";
                            barLegend.EditValue = LegendPosition.UpperLeft.ToString();
                        }
                    }

                    #endregion LegendPosition
                    break;
                case "Palette":
                    #region ChartAppearance

                    chartMain.PaletteName = this.ChartingAppearance != null ?
                        this.ChartingAppearance.ToString() : _defaultPalette.ToString();

                    if (barColorScheme.EditValue != null &&
                        string.Compare(barColorScheme.EditValue.ToString(), this.ChartingAppearance.ToString()) != 0)
                    {
                        barColorScheme.EditValue = this.ChartingAppearance.ToString();
                    }

                    #endregion ChartAppearance
                    break;

                case "ShowOnlyTopRecords":
                    #region ShowOnlyTopRecords

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.TopNOptions.Enabled = this.ShowOnlyTopRecords;
                    }

                    if (GetNumberOfLabels() > 250)
                    {
                        this.LabelsVisible = false;
                    }

                    chkShowOnlyTop.Checked = barShowTop.Enabled = chkShowOther.Enabled = this.ShowOnlyTopRecords;

                    #endregion ShowOnlyTopRecords
                    break;
                case "NumberOfRecordsToShow":
                    #region NumberOfRecordsToShow

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.TopNOptions.Count = Convert.ToInt32(this.NumberOfRecordsToShow);
                    }

                    chkShowOnlyTop.Caption = string.Format("Show Only Top {0} Records", this.NumberOfRecordsToShow);
                    barShowTop.EditValue = this.NumberOfRecordsToShow;

                    #endregion NumberOfRecordsToShow
                    break;
                case "ShowOther":
                    #region ShowOther

                    foreach (WinChartSeries series in this.Series)
                    {
                        series.TopNOptions.ShowOthers = this.ShowOther;
                    }

                    chkShowOther.Checked = this.ShowOther;

                    #endregion ShowOther
                    break;

                case "LabelAngle":
                    #region LabelAngle

                    #endregion LabelAngle
                    break;
                case "MarkerKind":
                    #region MarkerKind

                    #endregion MarkerKind
                    break;
                case "MarkerSize":
                    #region MarkerSize

                    #endregion MarkerSize
                    break;
                case "ShowMarkers":
                    #region ShowMarkers

                    #endregion ShowMarkers
                    break;

                case "XAxisLabelAngle":
                    #region XAxisLabelAngle

                    if (chartMain.Diagram is XYDiagram)
                    {
                        ((XYDiagram)chartMain.Diagram).AxisX.Label.Angle = (int)this.PrimaryAxisXLabelAngle;
                    }

                    #endregion XAxisLabelAngle
                    break;
                case "XAxisLabelTickScale":
                    #region XAxisLabelTickScale

                    if (chartMain.Diagram is XYDiagram)
                    {
                        ((XYDiagram)chartMain.Diagram).AxisX.GridSpacing = (double)this.PrimaryAxisXLabelTickScale;
                    }
                    break;

                    #endregion XAxisLabelTickScale
                case "PrimaryAxisXAlignment":
                    #region PrimaryAxisXAlignment

                    if (this.PrimaryAxisX != null)
                    {
                        this.PrimaryAxisX.Alignment = _visualSettings.PrimaryAxisXAlignment;
                    }

                    #endregion PrimaryAxisXAlignment
                    break;
                case "SecondaryAxisX":
                    UpdateSecondaryAxisX();
                    break;
                case "SecondaryAxisY":
                case "AxesAligned":
                case "AxesAlignedAtValue":
                case "AxesScaleY":
                    UpdateSecondaryAxisY();
                    break;
                case "PrimaryAxisYTitle":
                    #region PrimaryAxisYTitle

                    if (this.PrimaryAxisY != null)
                    {
                        this.PrimaryAxisY.Title.Text = _visualSettings.PrimaryAxisYTitle;
                        if (_visualSettings.PrimaryAxisYTitle.Trim().Length > 0)
                        {
                            this.PrimaryAxisY.Title.Visible = true;
                        }
                        else
                        {
                            this.PrimaryAxisY.Title.Visible = false;
                        }
                    }

                    #endregion
                    break;
                case "SecondaryAxisYTitle":
                    #region SecondaryAxisYTitle

                    if (this.SecondaryAxisY != null)
                    {
                        this.SecondaryAxisY.Title.Text = _visualSettings.SecondaryAxisYTitle;
                        if (_visualSettings.SecondaryAxisYTitle.Trim().Length > 0)
                        {
                            this.SecondaryAxisY.Title.Visible = true;
                        }
                        else
                        {
                            this.SecondaryAxisY.Title.Visible = false;
                        }
                    }
                    #endregion
                    break;

                case "ModelType":
                    #region ModelType

                    barGeneralBarShape.EditValue = ModelType.ToString();
                    shouldUpdateUI = true;

                    #endregion ModelType
                    break;
                case "BarLabelPosition":
                    #region BarLabelPosition

                    barSeriesLabelPosition.EditValue = BarLabelPosition.ToString();
                    shouldUpdateUI = true;

                    #endregion BarLabelPosition
                    break;

                case "ExplodeDistance":
                    #region ExplodeDistance

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is PieSeriesViewBase)
                        {
                            ((PieSeriesViewBase)series.View).ExplodedDistancePercentage = Convert.ToDouble(this.ExplodeDistance);
                        }
                    }

                    shouldUpdateUI = true;

                    #endregion ExplodeDistance
                    break;
                case "ExplodeMode":
                case "ArgumentExploded":
                    #region ExplodeMode

                    foreach (WinChartSeries series in this.Series)
                    {
                        if (series.View is PieSeriesViewBase)
                        {
                            PieSeriesViewBase pieView = (PieSeriesViewBase)series.View;
                            pieView.ExplodeMode = this.ExplodeMode;
                            if (this.ExplodeMode == PieExplodeMode.UsePoints)
                            {
                                pieView.ExplodedPoints.Clear();
                                foreach (SeriesPoint point in series.Points)
                                {
                                    if (point.Argument == this.ArgumentExploded)
                                    {
                                        pieView.ExplodedPoints.Add(point);
                                    }
                                }
                            }
                        }
                    }

                    shouldUpdateUI = true;

                    #endregion ExplodeMode
                    break;
                case "PieLabelPosition":
                    #region PieLabelPosition

                    if (GetSampleSeries().Label is PieSeriesLabel)
                    {
                        shouldUpdateUI = true;
                        barSeriesLabelPosition.EditValue = PieLabelPosition.ToString();
                    }

                    #endregion PieLabelPosition
                    break;
                case "HoleRadius":
                    #region HoleRadius

                    shouldUpdateUI = true;
                    barHoleRadius.EditValue = this.HoleRadius.ToString();

                    #endregion HoleRadius
                    break;

                default:
#if DEBUG_EVAN
                    Console.WriteLine("The property '{0}' is not supported in WinChart.CleanProperty().", property.Name);
#endif
                    break;
            }
            return shouldUpdateUI;
            #region Update PrimaryFormat

            //if (properties.Find("PrimaryFormat", true) != null)
            //{
            //    if (this.chartMain.Diagram is XYDiagram)
            //    {
            //        ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Format = this.PrimaryFormat;
            //    }
            //    foreach (Series series in this.chartMain.Series)
            //    {
            //        SetSeriesFormat(series);
            //    }
            //}

            //#endregion Update PrimaryFormat
            //#region Update SecondaryFormat

            //if (properties.Find("SecondaryFormat", true) != null)
            //{
            //    this.SecondaryAxisY.NumericOptions.Format = this.SecondaryFormat;
            //    foreach (Series series in this.chartMain.Series)
            //    {
            //        SetSeriesFormat(series);
            //    }
            //}

            //#endregion Update SecondaryFormat
            //#region Update PrimaryFormatPrecision

            //if (properties.Find("PrimaryFormatPrecision", true) != null)
            //{
            //    if (this.chartMain.Diagram is XYDiagram)
            //    {
            //        if (this.PrimaryFormatPrecision < 0)
            //        {
            //            // Reset the precision.
            //            ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Format = this.PrimaryFormat;
            //        }
            //        else
            //        {
            //            ((XYDiagram)this.chartMain.Diagram).AxisY.NumericOptions.Precision = this.PrimaryFormatPrecision;
            //        }
            //    }
            //    foreach (Series series in chartMain.Series)
            //    {
            //        SetSeriesFormat(series);
            //    }
            //}

            //#endregion Update PrimaryFormatPrecision
            //#region Update SecondaryFormatPrecision

            //if (properties.Find("SecondaryFormatPrecision", true) != null)
            //{
            //    if (this.SecondaryFormatPrecision < 0)
            //    {
            //        // Reset the precision.
            //        this.SecondaryAxisY.NumericOptions.Format = this.SecondaryFormat;
            //    }
            //    else
            //    {
            //        this.SecondaryAxisY.NumericOptions.Precision = this.SecondaryFormatPrecision;
            //    }

            //    foreach (Series series in this.chartMain.Series)
            //    {
            //        SetSeriesFormat(series);
            //    }
            //}

            #endregion Update SecondaryFormatPrecision

        }

        //        /// <summary>
        //        /// Updates the chart and control settings to reflect the VisualSettingsObject.
        //        /// </summary>
        //        private void CleanProperties()
        //        {
        //            if (_holdCleaning)
        //            {
        //                return;
        //            }

        //            _holdCleaning = true;
        //            bool shouldUpdateSeries = false;

        //            while (_dirtyProperties.Count > 0)
        //            {
        //                PropertyDescriptor property = _dirtyProperties[0];
        //                shouldUpdateSeries = (this.CleanProperty(property) || shouldUpdateSeries);
        //                _dirtyProperties.Remove(property);
        //            }

        //            if (shouldUpdateSeries)
        //            {
        //                #region Update UI

        //                if (chkSeriesLabelVisible.Checked != this.LabelsVisible)
        //                {
        //                    chkSeriesLabelVisible.Checked = this.LabelsVisible;
        //                }

        //                if (GetSampleSeries().View is Bar3DSeriesView)
        //                {
        //                    barGeneralBarShape.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        //                }
        //                else
        //                {
        //                    barGeneralBarShape.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        //                }

        //                barSeriesLabelPosition.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        //                if (GetSampleSeries().Label is BarSeriesLabel && !(GetSampleSeries().View is Bar3DSeriesView))
        //                {
        //                    bool excludeTop = GetSampleSeries().View is StackedBarSeriesView || GetSampleSeries().View is ISupportStackedGroup;
        //                    SetBarLabelPositionOptions(excludeTop);
        //                }
        //                else if (GetSampleSeries().Label is PieSeriesLabel)
        //                {
        //                    SetPieLabelPositionOptions();
        //                }
        //                else
        //                {
        //                    barSeriesLabelPosition.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        //                }

        //                barSeriesLabelPosition.Enabled = this.LabelsVisible;

        //                barHoleRadius.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        //                if (GetSampleSeries().View is DoughnutSeriesView)
        //                {
        //                    foreach (Series series in chartMain.Series)
        //                    {
        //                        int radius =
        //                            Convert.ToInt32(Utils.ExtractDigits(this.HoleRadius.ToString()));
        //                        ((DoughnutSeriesView)series.View).HoleRadiusPercent = radius;
        //                    }
        //                    barHoleRadius.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        //                }
        //                if (GetSampleSeries().View is Doughnut3DSeriesView)
        //                {
        //                    foreach (Series series in chartMain.Series)
        //                    {
        //                        ((Doughnut3DSeriesView)series.View).HoleRadiusPercent =
        //                            Convert.ToInt32(Utils.ExtractDigits(this.HoleRadius.ToString()));
        //                    }
        //                    barHoleRadius.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        //                }

        //                if (this.ExplodeMode == PieExplodeMode.UsePoints)
        //                {
        //                    try
        //                    {
        //                        barExplodedPoints.EditValue = "val_" + this.ArgumentExploded;
        //                    }
        //                    catch (Exception e)
        //                    {
        //#if DEBUG_EVAN
        //                        Console.WriteLine("Error setting barExplodedPoints.EditValue: {0}", e.Message);
        //#endif
        //                    }
        //                }
        //                else
        //                {
        //                    barExplodedPoints.EditValue = this.ExplodeMode.ToString();
        //                }

        //                barExplodedDist.EditValue = this.ExplodeDistance.ToString();

        //                #endregion Update UI

        //                //if (_generalUI != null)
        //                //{
        //                //    _generalUI.UpdateToSeries(this.GetSampleSeries());
        //                //    _generalUI.ModelType = ModelType;
        //                //}
        //                UpdateAllSeries();
        //            }

        //            _holdCleaning = false;
        //        }

        /// <summary>
        /// Figures out and applies the proper ResolveOverlappingMode to each series' labels.  
        /// </summary>
        private void SetOverlappingMode()
        {
            // Stacked bar groups also don't do well displaying labels in default mode.
            foreach (WinChartSeries series in this.Series)
            {
                if (series.View is StackedBarSeriesView)
                {
                    series.Label.ResolveOverlappingMode = ResolveOverlappingMode.HideOverlapped;
                }
                else if (GetNumberOfLabels() > 400 && series.View is PointSeriesViewBase && !(series.View is BubbleSeriesView))
                {
                    series.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAroundPoint;
                }
                else if (series.Label is FunnelSeriesLabel)
                {
                    series.Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
                }
                else
                {
                    series.Label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
                }
            }
        }

        /// <summary>
        /// Updates the Secondary X-Axis.
        /// </summary>
        private void UpdateSecondaryAxisX()
        {
            if (!(this.chartMain.Diagram is XYDiagram) || this.SecondaryAxisX == null)
            {
                return;
            }

            //if (!((XYDiagram)this.chartMain.Diagram).SecondaryAxesX.Contains(this.SecondaryAxisX))
            //{
            //    ((XYDiagram)this.chartMain.Diagram).SecondaryAxesX.Add(this.SecondaryAxisX);
            //}

            //// Set each series to its axis.
            //foreach (WinChartSeries series in this.Series)
            //{
            //    if (series.View is XYDiagramSeriesViewBase)
            //    {
            //        if (_seriesForSecondAxisX.Contains(series.Name))
            //        {
            //            ((XYDiagramSeriesViewBase)series.View).AxisX = this.SecondaryAxisX;
            //        }
            //        else
            //        {
            //            ((XYDiagramSeriesViewBase)series.View).AxisX = ((XYDiagram)this.chartMain.Diagram).AxisX;
            //        }
            //    }
            //}

            //((XYDiagram)this.chartMain.Diagram).AxisX.Range.Auto = true;
            //this.SecondaryAxisX.Range.Auto = true;

            ((XYDiagram)this.chartMain.Diagram).AxisX.VisualRange.Auto =
                ((XYDiagram)this.chartMain.Diagram).AxisX.WholeRange.Auto =
                true;
            this.SecondaryAxisX.VisualRange.Auto =
                this.SecondaryAxisX.WholeRange.Auto =
                true;
        }

        /// <summary>
        /// Updates the secondary Y-axis.  
        /// </summary>
        private void UpdateSecondaryAxisY()
        {
            if (!(this.chartMain.Diagram is XYDiagram) || this.SecondaryAxisY == null)
            {
                return;
            }

            if (!((XYDiagram)this.chartMain.Diagram).SecondaryAxesY.Contains(this.SecondaryAxisY))
            {
                ((XYDiagram)this.chartMain.Diagram).SecondaryAxesY.Add(this.SecondaryAxisY);
            }

            // Set each series to its axis.
            foreach (WinChartSeries series in this.Series)
            {
                if (series.View is XYDiagramSeriesViewBase)
                {
                    if (_seriesForSecondAxisY.Contains(series.Name))
                    {
                        ((XYDiagramSeriesViewBase)series.View).AxisY = this.SecondaryAxisY;
                    }
                    else
                    {
                        ((XYDiagramSeriesViewBase)series.View).AxisY = ((XYDiagram)this.chartMain.Diagram).AxisY;
                    }
                }
            }

            if (YAxesAligned)
            {
                AlignAxes(this.YAxesAlignedAtValue, this.AxesScaleY);
            }
            else
            {
                //((XYDiagram)this.chartMain.Diagram).AxisY.Range.Auto = true;
                //this.SecondaryAxisY.Range.Auto = true;

                ((XYDiagram)this.chartMain.Diagram).AxisY.VisualRange.Auto = true;
                this.SecondaryAxisY.VisualRange.Auto = true;
            }
        }

        /// <summary>
        /// Creates and assigns unique objects for each group.
        /// </summary>
        private void PrepareGroupings()
        {
            Dictionary<string, object> groupDictionary = new Dictionary<string, object>();
            foreach (SeriesUI ui in _seriesUIs)
            {
                //Make sure there's a unique object for each group.
                if (!(groupDictionary.ContainsKey(ui.SelectedGroup)))
                {
                    groupDictionary.Add(ui.SelectedGroup, new object());
                }
                if (ui.Series.View is ISupportStackedGroup)
                {
                    ((ISupportStackedGroup)ui.Series.View).StackedGroup = groupDictionary[ui.SelectedGroup];
                }
            }
        }

        /// <summary>
        /// Aligns the two axes so that they have a consistent anchor and scale.
        /// </summary>
        /// <param name="matchingValue">The value that should match from one scale to the other.</param>
        /// <param name="scale">The ratio of value/distance between the two axes.  
        /// A value of 2 will cause the secondary axis to increment 2 units for each 1 unit on the primary axis.</param>
        private void AlignAxes(double matchingValue, double scale)
        {
            if (!(this.chartMain.Diagram is XYDiagram))
            {
                return;
            }

            // Get the min and max value for each axis.
            double minPrimary = 0.0, maxPrimary = 0.0, minSecondary = 0.0, maxSecondary = 0.0;
            bool primaryStartingPointFound = false, secondaryStartingPointFound = false;

            foreach (WinChartSeries series in this.Series)
            {
                if (_seriesForSecondAxisY.Contains(series.Name))
                {
                    foreach (SeriesPoint point in series.Points)
                    {
                        foreach (double val in point.Values)
                        {
                            if (!secondaryStartingPointFound)
                            {
                                minSecondary = maxSecondary = val;
                                secondaryStartingPointFound = true;
                            }
                            else
                            {
                                if (point.Values[0] < minSecondary)
                                {
                                    minSecondary = val;
                                }
                                else if (point.Values[0] > maxSecondary)
                                {
                                    maxSecondary = val;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (SeriesPoint point in series.Points)
                    {
                        foreach (double val in point.Values)
                        {
                            if (!primaryStartingPointFound)
                            {
                                minPrimary = maxPrimary = val;
                                primaryStartingPointFound = true;
                            }
                            else
                            {
                                if (point.Values[0] < minPrimary)
                                {
                                    minPrimary = val;
                                }
                                else if (point.Values[0] > maxPrimary)
                                {
                                    maxPrimary = val;
                                }
                            }
                        }
                    }
                }
            }

            // Get the min and max in terms of the primary axis
            double convertedMinSecondary = matchingValue + ((minSecondary - matchingValue) / scale);
            double convertedMaxSecondary = matchingValue + ((maxSecondary - matchingValue) / scale);
            double scaleMin = minPrimary, scaleMax = maxPrimary;
            if (convertedMinSecondary < scaleMin)
            {
                scaleMin = convertedMinSecondary;
            }
            if (convertedMaxSecondary > scaleMax)
            {
                scaleMax = convertedMaxSecondary;
            }

            // Add in a margin
            double marginSize = (scaleMax - scaleMin) / 8;
            if (marginSize == 0)
            {
                marginSize = 1;
            }

            scaleMin -= marginSize;
            scaleMax += marginSize;

            // Find the corresponding values on the secondary axis
            double scaleMinSecondary = (scale * (scaleMin - matchingValue)) + matchingValue;
            double scaleMaxSecondary = (scale * (scaleMax - matchingValue)) + matchingValue;

            // Set the values on the Axes
            //((XYDiagram)this.chartMain.Diagram).AxisY.Range.SetInternalMinMaxValues(scaleMin, scaleMax);
            //this.SecondaryAxisY.Range.SetInternalMinMaxValues(scaleMinSecondary, scaleMaxSecondary);

            ((XYDiagram)this.chartMain.Diagram).AxisY.VisualRange.SetMinMaxValues(scaleMin, scaleMax);
            this.SecondaryAxisY.VisualRange.SetMinMaxValues(scaleMinSecondary, scaleMaxSecondary);
        }

        /// <summary>
        /// Sets the numeric format of the series' points to match its assigned Y-axis.
        /// </summary>
        /// <param name="series">The series to format.</param>
        private void SetSeriesFormat(WinChartSeries series)
        {
            series.LegendPointOptions.PointView = PointView.Argument;
            if (_seriesForSecondAxisY.Contains(series.Name))
            {
                series.PointOptions.ValueNumericOptions.Format = this.SecondaryAxisYFormat;
                if (this.SecondaryAxisYFormatPrecision >= 0)
                {
                    series.PointOptions.ValueNumericOptions.Precision = this.SecondaryAxisYFormatPrecision;
                }
            }
            else
            {
                series.PointOptions.ValueNumericOptions.Format = this.PrimaryAxisYFormat;
                if (this.PrimaryAxisYFormatPrecision >= 0)
                {
                    series.PointOptions.ValueNumericOptions.Precision = this.PrimaryAxisYFormatPrecision;
                }
            }
        }

        /// <summary>
        /// Figures out how many labels need to be rendered.
        /// </summary>
        /// <returns>The number of labels in all series.</returns>
        private int GetNumberOfLabels()
        {
            int labelsPerSeries = GetSampleSeries().Points.Count;
            if (this.ShowOnlyTopRecords && (this.NumberOfRecordsToShow < labelsPerSeries))
            {
                labelsPerSeries = Convert.ToInt32(this.NumberOfRecordsToShow);
                if (this.ShowOther)
                {
                    labelsPerSeries++;
                }
            }
            return this.chartMain.Series.Count * labelsPerSeries;
        }

        /// <summary>
        /// Extracts the actual DateTime format string used by an X-Axis's labels, once regional settings are accounted for.
        /// </summary>
        /// <param name="axis">The AxisX object to extract the format strings from.</param>
        /// <returns>A format string that should match the one used by the axis at runtime.</returns>
        private string ExtractDateFormat(AxisXBase axis)
        {
            if (axis == null)
            {
                return "yyyy-MM-dd";
            }

            System.Globalization.DateTimeFormatInfo currentInfo = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;

            switch (axis.DateTimeOptions.Format)
            {
                case DateTimeFormat.Custom:
                    return axis.DateTimeOptions.FormatString;
                case DateTimeFormat.General:
                    return string.Format("{0} {1}", currentInfo.ShortDatePattern, currentInfo.ShortTimePattern);
                case DateTimeFormat.LongDate:
                    return currentInfo.LongDatePattern;
                case DateTimeFormat.LongTime:
                    return currentInfo.LongTimePattern;
                case DateTimeFormat.MonthAndDay:
                    return currentInfo.MonthDayPattern;
                case DateTimeFormat.MonthAndYear:
                    return currentInfo.YearMonthPattern;
                case DateTimeFormat.QuarterAndYear:
                    return "QQ yyyy";
                case DateTimeFormat.ShortDate:
                    return currentInfo.ShortDatePattern;
                case DateTimeFormat.ShortTime:
                    return currentInfo.ShortTimePattern;
                default:
                    return "yyyy-MM-dd";
            }
        }

        #endregion Private Methods

        #region Class Events

        [Browsable(true)]
        public event EventHandler ChartGalleryUpdated;
        protected virtual void OnChartGalleryUpdated(ChartGalleryEvents e)
        {
            if (ChartGalleryUpdated != null)
            {
                ChartGalleryUpdated(this, e);
            }
        }

        public event EventHandler BeforeChartTypeChange;
        protected virtual void OnBeforeChartTypeChange(WinChartTypeChangeEventArgs e)
        {
            if (BeforeChartTypeChange != null)
            {
                BeforeChartTypeChange(this, e);
            }
        }

        public event EventHandler PrintButtonClicked;
        protected virtual void OnPrintButtonClicked(EventArgs e)
        {
            if (PrintButtonClicked != null)
            {
                PrintButtonClicked(this, e);
            }
        }

        public event EventHandler CopyButtonClicked;
        protected virtual void OnCopyButtonClicked(WinChartCopyEventArgs e)
        {
            if (CopyButtonClicked != null)
            {
                CopyButtonClicked(this, e);
            }
        }

        public event EventHandler SaveButtonClicked;
        protected virtual void OnSaveButtonClicked(WinChartSaveEventArgs e)
        {
            if (SaveButtonClicked != null)
            {
                SaveButtonClicked(this, e);
            }
        }

        /// <summary>
        /// Fires when a user clicks a series point.  
        /// The event args extend MouseEventArgs, and include the Series and SeriesPoint clicked in the WinChartMouseEventArgs.Series and WinChartMouseEventArgs.Point properties.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<WinChartMouseEventArgs> SeriesPointClicked;
        protected virtual void OnSeriesPointClicked(WinChartMouseEventArgs e)
        {
            if (SeriesPointClicked != null)
            {
                SeriesPointClicked(this, e);
            }
        }

        public event EventHandler<WinChartMouseEventArgs> SeriesPointMouseOver;
        protected virtual void OnSeriesPointMouseOver(WinChartMouseEventArgs e)
        {
            if (SeriesPointMouseOver != null)
            {
                SeriesPointMouseOver(this, e);
            }
        }

        #endregion Class Events

        #region Class Enum

        /// <summary>
        /// Chart elements that can be marked as dirty and updated.
        /// </summary>
        private enum UpdatableElement
        {
            ChartType,
            LabelsVisible,
            ChartTitle,
            ValueAsPercent,
            Legend,
            ValueInLegend,
            PercentageInLegend,
            ChartAppearance,
            ShowOnlyTopRecords,
            NumberOfRecordsToShow,
            ShowOther,
            AreaLineLabelAngle,
            MarkerKind,
            MarkerSize,
            ShowMarkers,
            ModelType,
            BarLabelPosition,
            PieLabelPosition,
            ExplodeMode,
            ExplodeDistance,
            HoleRadius,
            PrimaryAxisXAlignment,
            PrimaryAxisXLabel,
            SecondaryAxisX,
            SecondaryAxisY,
            PrimaryAxisYTitle,
            SecondaryAxisYTitle,
        }

        #endregion Class Enum

        #region IPrintable Members

        //public void AcceptChanges()
        //{
        //    ((IPrintable)chartMain).AcceptChanges();
        //}

        //public bool CreatesIntersectedBricks
        //{
        //    get { return ((IPrintable)chartMain).CreatesIntersectedBricks; }
        //}

        //public bool HasPropertyEditor()
        //{
        //    return ((IPrintable)chartMain).HasPropertyEditor();
        //}

        //public UserControl PropertyEditorControl
        //{
        //    get { return ((IPrintable)chartMain).PropertyEditorControl; }
        //}

        //public void RejectChanges()
        //{
        //    ((IPrintable)chartMain).RejectChanges();
        //}

        //public void ShowHelp()
        //{
        //    ((IPrintable)chartMain).ShowHelp();
        //}

        //public bool SupportsHelp()
        //{
        //    return ((IPrintable)chartMain).SupportsHelp();
        //}

        #endregion

        #region IBasePrintable Members

        //public void CreateArea(string areaName, IBrickGraphics graph)
        //{
        //    ((IPrintable)chartMain).CreateArea(areaName, graph);
        //}

        //public void Finalize(IPrintingSystem ps, ILink link)
        //{
        //    ((IPrintable)chartMain).Finalize(ps, link);
        //}

        //public void Initialize(IPrintingSystem ps, ILink link)
        //{
        //    ((IPrintable)chartMain).Initialize(ps, link);
        //}

        #endregion
    }

    #region Event Argument Classes

    /// <summary>
    /// Used to pass the contents of the gallery drop down to other classes which have their own chart type galleries, to ensure that everyone has the same list of chart types.
    /// </summary>
    public class ChartGalleryEvents : EventArgs
    {
        private GalleryDropDown _galleryDropDown = null;
        private Image _glyph = null;
        private string _caption = string.Empty;
        private string _hint = string.Empty;

        public GalleryDropDown GalleryDropDown
        {
            get
            {
                return _galleryDropDown;
            }
        }

        public Image Glyph
        {
            get
            {
                return _glyph;
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
        }

        public string Hint
        {
            get
            {
                return _hint;
            }
        }

        public ChartGalleryEvents(GalleryDropDown gdd, Image glyph, string caption, string hint)
        {
            _galleryDropDown = gdd;
            _glyph = glyph;
            _caption = caption;
            _hint = hint;
        }
    }

    /// <summary>
    /// Holds the current ChartType, the potential ChartType, and boolean that indicates whether they should change.
    /// </summary>
    public class WinChartTypeChangeEventArgs : EventArgs
    {
        private ViewType _currentType = ViewType.Bar;
        private ViewType _newType = ViewType.Bar;
        private bool _shouldChange = true;

        public ViewType CurrentType
        {
            get { return _currentType; }
            set { _currentType = value; }
        }

        public ViewType NewType
        {
            get { return _newType; }
            set { _newType = value; }
        }

        public bool ShouldChange
        {
            get { return _shouldChange; }
            set { _shouldChange = value; }
        }

        public WinChartTypeChangeEventArgs(ViewType currentType, ViewType newType)
        {
            _currentType = currentType;
            _newType = newType;
        }
    }

    /// <summary>
    /// Passes whether or not the chart successfully copied to the clipboard.
    /// </summary>
    public class WinChartCopyEventArgs : EventArgs
    {
        private bool _successful = false;

        public bool Successful
        {
            get { return _successful; }
            set { _successful = value; }
        }

        public WinChartCopyEventArgs(bool successful)
        {
            _successful = successful;
        }

    }

    /// <summary>
    /// Passes the full file path saved to.
    /// </summary>
    public class WinChartSaveEventArgs : EventArgs
    {
        private string _filePath = "";

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public WinChartSaveEventArgs(string filePath)
        {
            _filePath = filePath;
        }
    }

    /// <summary>
    /// Event arguments for Mouse events within WinChart.
    /// </summary>
    public class WinChartMouseEventArgs : MouseEventArgs, IWinChartPointEventArgs
    {
        /// <summary>
        /// The WinChartSeries the mouse is over.  Null if the mouse is not over a WinChartSeries object.
        /// </summary>
        public WinChartSeries Series { get; set; }

        /// <summary>
        /// The SeriesPoint the mouse is over.  Null if the mouse is not over a SeriesPoint.
        /// </summary>
        public SeriesPoint Point { get; set; }

        /// <summary>
        /// Event arguments for Mouse events within WinChart, derived from other MouseEventArgs.
        /// </summary>
        /// <param name="args">The MouseEventArgs of the event that triggered this event.</param>
        /// <param name="series">The WinChartSeries the mouse is over.</param>
        /// <param name="point">The SeriesPoint the mouse is over.</param>
        public WinChartMouseEventArgs(MouseEventArgs args, WinChartSeries series, SeriesPoint point)
            : base(args.Button, args.Clicks, args.X, args.Y, args.Delta)
        {
            this.Series = series;
            this.Point = point;
        }
    }

    /// <summary>
    /// Arguments for WinChart events centered on a WinChartSeries or SeriesPoint.
    /// </summary>
    public class WinChartPointEventArgs : EventArgs, IWinChartPointEventArgs
    {
        /// <summary>
        /// The WinChartSeries the event is centered on.
        /// </summary>
        public WinChartSeries Series { get; set; }

        /// <summary>
        /// The SeriesPoint the event is centered on.
        /// </summary>
        public SeriesPoint Point { get; set; }

        /// <summary>
        /// Arguments for WinChart events centered on a WinChartSeries or SeriesPoint.
        /// </summary>
        /// <param name="series">The WinChartSeries the event is centered on.</param>
        /// <param name="point">The SeriesPoint the event is centered on.</param>
        public WinChartPointEventArgs(WinChartSeries series, SeriesPoint point)
        {
            this.Series = series;
            this.Point = point;
        }
    }

    /// <summary>
    /// An interface for WinChartEvents that center on a WinChartSeries or SeriesPoint.
    /// </summary>
    public interface IWinChartPointEventArgs
    {
        /// <summary>
        /// The WinChartSeries the event is centered on.
        /// </summary>
        WinChartSeries Series { get; set; }

        /// <summary>
        /// The SeriesPoint the event is centered on.
        /// </summary>
        SeriesPoint Point { get; set; }
    }

    #endregion Event Argument Classes

}