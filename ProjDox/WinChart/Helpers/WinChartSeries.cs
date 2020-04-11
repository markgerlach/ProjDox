using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using DevExpress.XtraCharts;

//using _BaseWinProject;
//using _BaseWinProject.Win.Common.WinChart;
//using _BaseWinProject.Win.Common.WinChart.Interfaces;
//using _BaseWinProject.Win.Common.WinChart.usercontrols;

//using mgWinChart;
//using mgWinChart.Interfaces;
//using mgWinChart.usercontrols;

using mgModel;

namespace mgWinChart.Helpers
{
    /// <summary>
    /// A series of data points for use with WinChart.
    /// </summary>
    public class WinChartSeries : DevExpress.XtraCharts.Series
    {

        #region Private Variables

        private usercontrols.ucWinChart _winChart = null;
        private string _dataSourceColumnName = "";
        private string _annotationSourceColumnName = "";
        private string _annotationFormat = "{0}";
        private ssChartPredicate _filter = new ssCompoundChartPredicate(PredicateConjunction.And);

        private bool _useSecondaryAxisX = false;

        private bool _showMean = false;
        private Series _meanLine = new Series("mu", ViewType.Line);
        private Series _envelopeTop = new Series("etop", ViewType.Line);
        private Series _envelopeBottom = new Series("ebot", ViewType.Line);
        private double _envelopeSize = -1;

        private bool _includeValueInLegend = false;
        private bool _includePercentageInLegend = false;

        private ThresholdCollection _thresholdCollection = new ThresholdCollection();

        #endregion Private Variables

        #region Constructors

        /// <summary>
        /// Creates a new series for use with WinChart.
        /// </summary>
        /// <param name="winChart">The WinChart object that acts as parent.</param>
        /// <param name="name">The name of the column to pull data from (and also the name of the series as displayed in the chart).</param>
        /// <param name="viewType">The series's ViewType (Bar, Line, Pie, etc.).</param>
        public WinChartSeries(usercontrols.ucWinChart winChart, string name, ViewType viewType)
            : base(name, viewType)
        {
            _winChart = winChart;
            _dataSourceColumnName = name;

            this.SetUpVariables();
        }

        /// <summary>
        /// Creates a new series for use with WinChart.
        /// </summary>
        /// <param name="winChart">The WinChart object that acts as parent.</param>
        /// <param name="seriesName">The name of the series, as displayed in the chart.</param>
        /// <param name="columnName">The name of the column to pull data from.</param>
        /// <param name="viewType">The series's ViewType (Bar, Line, Pie, etc.).</param>
        public WinChartSeries(usercontrols.ucWinChart winChart, string seriesName, string columnName, ViewType viewType)
            : base(seriesName, viewType)
        {
            _winChart = winChart;
            _dataSourceColumnName = columnName;

            this.SetUpVariables();
        }

        private void SetUpVariables()
        {
            _meanLine.Label.Visible = _envelopeTop.Label.Visible = _envelopeBottom.Label.Visible = false;
            ((LineSeriesView)_meanLine.View).LineMarkerOptions.Visible = ((LineSeriesView)_envelopeBottom.View).LineMarkerOptions.Visible =
                ((LineSeriesView)_envelopeTop.View).LineMarkerOptions.Visible = false;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// This series's parent.
        /// </summary>
        public mgWinChart.usercontrols.ucWinChart WinChart
        {
            get
            {
                return _winChart;
            }
            set
            {
                _winChart = value;
            }
        }

        /// <summary>
        /// The name of the column in the data source this series represents.
        /// </summary>
        public string DataSourceColumnName
        {
            get
            {
                return _dataSourceColumnName;
            }
            set
            {
                _dataSourceColumnName = value;
            }
        }

        /// <summary>
        /// The column name to pull annotations from.
        /// </summary>
        public string AnnotationSourceColumnName
        {
            get { return _annotationSourceColumnName; }
            set { _annotationSourceColumnName = value; }
        }

        /// <summary>
        /// The 
        /// </summary>
        public string AnnotationFormat
        {
            get { return _annotationFormat; }
            set { _annotationFormat = value; }
        }

        /// <summary>
        /// A filter to apply to this series.
        /// </summary>
        /// <remarks>
        /// Note: Setting this property does not apply the filter immediately.  
        /// Call the WinChartSeries.PrepareDataPoints() method to update the series, including its filter.
        /// </remarks>
        public ssChartPredicate Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }

        /// <summary>
        /// If true, shows a line indicating the Series' mean value.
        /// </summary>
        public bool ShowMean
        {
            get
            {
                return _showMean;
            }
            set
            {
                _showMean = value;
                if (_showMean)
                {
                    double mu = this.GetMean();
                    if (this.ValueScaleType != DevExpress.XtraCharts.ScaleType.Numerical)
                    {
                        _showMean = false;
                    }
                    else
                    {
                        _meanLine.Name = string.Format("{0} Mean: {1}", this.Name, mu);

                        _meanLine.Points.Clear();
                        _meanLine.ArgumentScaleType = this.ArgumentScaleType;
                        foreach (SeriesPoint point in this.Points)
                        {
                            switch (this.ArgumentScaleType)
                            {
                                case DevExpress.XtraCharts.ScaleType.Qualitative:
                                    _meanLine.Points.Add(new SeriesPoint(point.Argument, mu));
                                    break;
                                case DevExpress.XtraCharts.ScaleType.Numerical:
                                    _meanLine.Points.Add(new SeriesPoint(point.NumericalArgument, mu));
                                    break;
                                case DevExpress.XtraCharts.ScaleType.DateTime:
                                    _meanLine.Points.Add(new SeriesPoint(point.DateTimeArgument, mu));
                                    break;
                            }
                        }

                        this.WinChart.AddSeriesElement(_meanLine);
                    }
                }
                else
                {
                    this.WinChart.RemoveSeriesElement(_meanLine);
                }

            }
        }

        /// <summary>
        /// The width of the envelope, measured in Standard Deviations from the mean.  
        /// A value of 0 or less hides the envelope.
        /// </summary>
        public double EnvelopeSize
        {
            get
            {
                return _envelopeSize;
            }
            set
            {
                _envelopeSize = value;
                if (_envelopeSize <= 0)
                {
                    this.WinChart.RemoveSeriesElement(_envelopeBottom);
                    this.WinChart.RemoveSeriesElement(_envelopeTop);
                }
                else
                {
                    double mu = this.GetMean();
                    double sigma = this.GetStandardDeviation();
                    double upperLimit = mu + (_envelopeSize * sigma);
                    double lowerLimit = mu - (_envelopeSize * sigma);

                    _envelopeBottom.Name = string.Format("{0}, {1} SD below Mean: {2}", this.Name, _envelopeSize, lowerLimit);
                    _envelopeTop.Name = string.Format("{0}, {1} SD above Mean: {2}", this.Name, _envelopeSize, upperLimit);

                    _envelopeBottom.ArgumentScaleType = this.ArgumentScaleType;
                    _envelopeTop.ArgumentScaleType = this.ArgumentScaleType;

                    _envelopeBottom.Points.Clear();
                    _envelopeTop.Points.Clear();

                    foreach (SeriesPoint point in this.Points)
                    {
                        switch (this.ArgumentScaleType)
                        {
                            case DevExpress.XtraCharts.ScaleType.Qualitative:
                                _envelopeTop.Points.Add(new SeriesPoint(point.Argument, upperLimit));
                                _envelopeBottom.Points.Add(new SeriesPoint(point.Argument, lowerLimit));
                                break;
                            case DevExpress.XtraCharts.ScaleType.Numerical:
                                _envelopeTop.Points.Add(new SeriesPoint(point.NumericalArgument, upperLimit));
                                _envelopeBottom.Points.Add(new SeriesPoint(point.NumericalArgument, lowerLimit));
                                break;
                            case DevExpress.XtraCharts.ScaleType.DateTime:
                                _envelopeTop.Points.Add(new SeriesPoint(point.DateTimeArgument, upperLimit));
                                _envelopeBottom.Points.Add(new SeriesPoint(point.DateTimeArgument, lowerLimit));
                                break;
                            default:
                                break;
                        }
                    }

                    this.WinChart.AddSeriesElement(_envelopeTop);
                    this.WinChart.AddSeriesElement(_envelopeBottom);
                }

            }
        }

        /// <summary>
        /// Whether to use the chart's secondary X axis in place of the primary.
        /// </summary>
        public bool UseSecondaryAxisX
        {
            get
            {
                return _useSecondaryAxisX;
            }
            set
            {
                _useSecondaryAxisX = value;
                //UpdateSecondaryAxisX();
            }
        }

        /// <summary>
        /// The sum total of the series's point values.
        /// </summary>
        public double TotalValue
        {
            get
            {
                double runningTotal = 0.0;
                foreach (SeriesPoint point in this.Points)
                {
                    runningTotal += point.Values.Sum();
                }
                return runningTotal;
            }
        }

        /// <summary>
        /// Whether to include each point's value in the legend (Pie/Doughnut series only).
        /// </summary>
        public bool IncludeValueInLegend
        {
            get { return _includeValueInLegend; }
            set { _includeValueInLegend = value; }
        }

        /// <summary>
        /// Whether to include each point's value as percent in the legend (Pie/Doughnut series only).
        /// </summary>
        public bool IncludePercentageInLegend
        {
            get { return _includePercentageInLegend; }
            set { _includePercentageInLegend = value; }
        }

        public ThresholdCollection ThresholdCollection
        {
            get { return _thresholdCollection; }
        }

        #endregion Public Properties

        #region Public Methods

        /// Refreshes the data points in the series to match the data source.
        /// </summary>
        /// <param name="dataSource">The data source to pull new data from.</param>
        /// <param name="argumentsColumnName">The name of the column to use as arguments (the X-axis).</param>
        public void PrepareDataPoints(object dataSource, string argumentsColumnName)
        {
            this.PrepareDataPoints(dataSource, argumentsColumnName, new ssCompoundChartPredicate(PredicateConjunction.And));
        }

        /// <summary>
        /// Refreshes the data points in the series to match the data source.
        /// </summary>
        /// <param name="dataSource">The data source to pull new data from.</param>
        /// <param name="argumentsColumnName">The name of the column to use as arguments (the X-axis).</param>
        /// <param name="filter">An additional filter to apply to the data (in addition to the WinChartSeries.Filter) property.</param>
        public void PrepareDataPoints(object dataSource, string argumentsColumnName, ssChartPredicate filter)
        {
            DataTable dt = null;
            if (dataSource is DataTable)
            {
                dt = (DataTable)dataSource;
            }
            else if (dataSource is IClassGenClassGenerated)
            {
                dt = ((IClassGenClassGenerated)dataSource).ToDataTable();
            }

            if (dt.Columns[_dataSourceColumnName] == null)
            {
                throw new Exception(string.Format("The data source does not contain a column or property named '{0}'.", _dataSourceColumnName));
            }

            this.Points.BeginUpdate();
            // Clear Annotations?
            this.Points.Clear();

            Type T = dt.Columns[argumentsColumnName].DataType;
            if (T == typeof(System.DateTime))
            {
                this.ArgumentScaleType = ScaleType.DateTime;
            }
            else if (T.Name.ToLower().Contains("int") ||
                T.Name.ToLower().Contains("decimal") ||
                T.Name.ToLower().Contains("double"))
            {
                this.ArgumentScaleType = ScaleType.Numerical;
            }
            else
            {
                this.ArgumentScaleType = ScaleType.Qualitative;
            }

            //List<object> arguments = ExtractArguments<object>(dt, argumentsColumnName);

            ssCompoundChartPredicate compoundfilter = new ssCompoundChartPredicate(PredicateConjunction.And);
            compoundfilter.Subpredicates.Add(filter);
            compoundfilter.Subpredicates.Add(this.Filter);

            //SeriesPoint[] spArray = new SeriesPoint[90];
            List<SeriesPoint> seriespointsList = new List<SeriesPoint>();
            Dictionary<Annotation, SeriesPoint> annotations = new Dictionary<Annotation, SeriesPoint>();
            foreach (DataRow row in dt.Rows)
            {
                object dataPoint = row[_dataSourceColumnName];
                object argument = row[argumentsColumnName];
                if (dataPoint != DBNull.Value && argument != DBNull.Value)
                {
                    double d = Convert.ToDouble(dataPoint);
                    SeriesPoint point = new SeriesPoint(argument, d);
                    if (compoundfilter.Evaluate(point))
                    {
                        seriespointsList.Add(point);
                        // Add the annotation as well.
                        if (!string.IsNullOrEmpty(this.AnnotationSourceColumnName))
                        {
                            string annotationString =
                                string.Format(this.AnnotationFormat, row[AnnotationSourceColumnName]);
                            annotations.Add(new TextAnnotation(string.Empty, annotationString), point);
                        }
                    }
                }
            }
            if (seriespointsList.Count > 0)
            {
                this.Points.AddRange(seriespointsList.ToArray());
            }

            if (annotations.Count > 0)
            {
                this.WinChart.AddAnnotations(new List<Annotation>(annotations.Keys));
            }

            foreach (Annotation annotation in annotations.Keys)
            {
                // Position each annotation.
                annotation.AnchorPoint = new SeriesPointAnchorPoint(annotations[annotation]);
                annotation.ShapePosition = new RelativePosition(-67, 85);
                annotation.RuntimeMoving = annotation.RuntimeResizing = true;
                annotation.Angle = 90;
            }

            if (this.ArgumentScaleType != ScaleType.Qualitative)
            {
                this.SeriesPointsSortingKey = SeriesPointKey.Argument;
                this.SeriesPointsSorting = SortingMode.Ascending;
            }

            this.UpdateSecondaryAxisX();

            this.Points.EndUpdate();
        }

        public void SetAnnotationSource(string annotationColumnName, string annotationFormat)
        {
            this.AnnotationSourceColumnName = annotationColumnName;
            this.AnnotationFormat = annotationFormat;
        }

        public void SetAnnotationSource(string annotationColumnName)
        {
            this.SetAnnotationSource(annotationColumnName, "{0}");
        }

        #endregion Public Methods

        #region Private Methods

        private double GetMean()
        {
            double sum = 0.0;
            foreach (SeriesPoint point in this.Points)
            {
                sum += point.Values[0];
            }

            if (this.Points.Count == 0)
            {
                return 0;
            }
            else
            {
                return sum / this.Points.Count;
            }
        }

        private double GetStandardDeviation()
        {
            if (this.Points.Count < 1)
            {
                return 0;
            }

            double mu = this.GetMean();
            double totalSquares = 0.0;

            foreach (SeriesPoint point in this.Points)
            {
                double dev = point.Values[0] - mu;
                totalSquares += (dev * dev);
            }

            double avgSquares = totalSquares / this.Points.Count;
            return Math.Sqrt(avgSquares);
        }

        private void UpdateSecondaryAxisX()
        {
            if (_useSecondaryAxisX)
            {
                if (this.WinChart.SecondaryAxisX != null && this.View is XYDiagramSeriesViewBase)
                {
                    ((XYDiagramSeriesViewBase)this.View).AxisX = ((XYDiagramSeriesViewBase)_meanLine.View).AxisX =
                        ((XYDiagramSeriesViewBase)_envelopeBottom.View).AxisX = ((XYDiagramSeriesViewBase)_envelopeTop.View).AxisX =
                        this.WinChart.SecondaryAxisX;
                }
                else
                {
                    _useSecondaryAxisX = false;
                }
            }
            else
            {
                if (this.WinChart.PrimaryAxisX != null && this.View is XYDiagramSeriesViewBase)
                {
                    ((XYDiagramSeriesViewBase)this.View).AxisX = ((XYDiagramSeriesViewBase)_meanLine.View).AxisX =
                        ((XYDiagramSeriesViewBase)_envelopeBottom.View).AxisX = ((XYDiagramSeriesViewBase)_envelopeTop.View).AxisX =
                        this.WinChart.PrimaryAxisX;
                }
            }
        }

        #endregion Private Methods

        #region Private Static Methods

        private static DataRow GetRowWithValue(DataTable dt, string columnName, object value)
        {
            DataRow correctRow = null;
            foreach (DataRow row in dt.Rows)
            {
                if (row[columnName].Equals(value))
                {
                    correctRow = row;
                    break;
                }
            }
            return correctRow;
        }

        private static List<T> ExtractArguments<T>(DataTable dt, string columnName)
        {
            List<T> argNames = new List<T>();
            foreach (DataRow row in dt.Select())
            {
                argNames.Add((T)row[columnName]);
            }
            return argNames;
        }

        #endregion Private Static Methods

    }

    #region Thresholds

    public class ThresholdColorScheme
    {
        public Color? Color1 = null;
        public Color? Color2 = null;

        /// <summary>
        /// An empty ColorScheme.  Both values are null.
        /// </summary>
        public ThresholdColorScheme()
        {
            Color1 = Color2 = null;
        }

        /// <summary>
        /// A solid-color ColorScheme.
        /// </summary>
        /// <param name="color">The color to paint the point.  Null means leave the point the default color.</param>
        public ThresholdColorScheme(Color? color)
        {
            Color1 = Color2 = color;
        }

        /// <summary>
        /// A gradient ColorScheme.
        /// </summary>
        /// <param name="color1">The color to paint one end.  Null means leave this end of the point the default color.</param>
        /// <param name="color2">The color to paint the other end.  Null means leave this end of the point the default color.</param>
        public ThresholdColorScheme(Color? color1, Color? color2)
        {
            Color1 = color1;
            Color2 = color2;
        }
    }

    public class ThresholdCollection : SortedList<double, ThresholdColorScheme>
    {
        /// <summary>
        /// Adds a new solid threshold to the series.
        /// </summary>
        /// <param name="value">The lower cut-off point for this threshold.</param>
        /// <param name="color">The color to paint the values above the threshold.  Null means leave it the default series color.</param>
        public void Add(double value, Color? color)
        {
            this.Add(value, new ThresholdColorScheme(color));
        }

        /// <summary>
        /// Adds a new gradient threshold to the series.
        /// </summary>
        /// <param name="value">The lower cut-off point for this threshold.</param>
        /// <param name="color1">The first color to paint the values above the threshold.  Null means leave this end the default series color.</param>
        /// <param name="color2">The second color to paint the values above the threshold.  Null means leave this end the default series color.</param>
        public void Add(double value, Color? color1, Color? color2)
        {
            this.Add(value, new ThresholdColorScheme(color1, color2));
        }

        /// <summary>
        /// Returns the set of colors to paint the point based on its value.
        /// </summary>
        /// <param name="value">The point's value.</param>
        /// <returns>The color scheme assigned to the point's threshold.</returns>
        public ThresholdColorScheme GetColorForValue(double value)
        {
            if (this.Count < 1 || value < this.Keys[0])
            {
                return new ThresholdColorScheme();
            }

            ThresholdColorScheme colorScheme = new ThresholdColorScheme();
            foreach (KeyValuePair<double, ThresholdColorScheme> kvp in this)
            {
                if (kvp.Key > value)
                {
                    break;
                }
                colorScheme = kvp.Value;
            }
            return colorScheme;
        }
    }

    #endregion Thresholds

}
