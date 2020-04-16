using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;
using mgWinChart.Interfaces;

namespace mgWinChart.usercontrols
{
    public partial class ucRealTimeChart : UserControl
    {

        #region Private Variables

        private IRealTimeChartQueriable _delegate = null;

        #endregion Private Variables

        #region Constructors

        public ucRealTimeChart()
        {
            InitializeComponent();

            mgInit();
        }

        public void mgInit()
        {
            // Do nothing for now.            
        }

        #endregion Constructors
        
        #region Public Properties

        /// <summary>
        /// The object that provides the data.
        /// </summary>
        public IRealTimeChartQueriable Delegate
        {
            get
            {
                return _delegate;
            }
            set
            {
                _delegate = value;
            }
        }

        /// <summary>
        /// How many milliseconds to wait between each update.  
        /// </summary>
        public int UpdateInterval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }
        
        /// <summary>
        /// The Time Span to display in the chart, in seconds.  
        /// </summary>
        [DefaultValue(30)]
        public int TimeSpanShown
        {
            get
            {
                return Convert.ToInt32(barTimeRange.EditValue);
            }
            set
            {
                barTimeRange.EditValue = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Replaces the current series collection with new series with the given names, in the order they should appear in the Legend.
        /// </summary>
        /// <param name="names"></param>
        public void SetSeriesNames(List<string> names)
        {
            // Make sure we never actually empty the Series collection.
            if (names.Count == 0)
            {
                this.chartMain.Visible = false;
                this.PauseRealTimeUpdates();
                return;
            }
            else
            {
                this.chartMain.Visible = true;
            }

           // If there are two many series, pare them down.
            while (this.chartMain.Series.Count > names.Count)
            {
                this.chartMain.Series.RemoveAt(0);
            }

            // Rename the existing series, adding new series as necessary.
            int seriesCounter = 0;
            foreach (string name in names)
            {
                if (seriesCounter < this.chartMain.Series.Count)
                {
                    Series series = this.chartMain.Series[seriesCounter];
                    series.Points.Clear();
                    series.Name = name;
                    seriesCounter++;
                }
                else
                {
                    Series series = new Series(name, ViewType.SwiftPlot);
                    series.ArgumentScaleType = ScaleType.DateTime;
                    this.chartMain.Series.Add(series);
                    seriesCounter++;
                }
            }
        }

        /// <summary>
        /// Start the timer, and begin updating the chart at steady intervals.  
        /// </summary>
        public void BeginRealTimeUpdates()
        {
            timer.Start();
        }

        /// <summary>
        /// Pause updating the chart.  
        /// </summary>
        public void PauseRealTimeUpdates()
        {
            timer.Stop();
        }

        #endregion Public Methods

        #region Event Handlers
        
        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateSeries();
        }
        
        #endregion Event Handlers

        #region Private Methods

        /// <summary>
        /// Pull data from the delegate and update the series.
        /// </summary>
        private void UpdateSeries()
        {
            if (_delegate == null)
            {
                return;
            }

            List<RealTimeDataEntry> newDataPoints = this.Delegate.GetLatestData();
            DateTime maxDate = DateTime.Now;
            DateTime minDate = maxDate.AddSeconds(-this.TimeSpanShown);

            if (newDataPoints.Count != 0)
            {
                foreach (RealTimeDataEntry entry in newDataPoints)
                {
                    Series series = this.chartMain.Series[entry.Name];
                    if (entry.Value != null && series != null)
                    {
                        int pointsToRemove = 0;
                        foreach (SeriesPoint point in series.Points)
                        {
                            if (point.DateTimeArgument < minDate)
                            {
                                pointsToRemove++;
                            }
                        }
                        if (pointsToRemove < series.Points.Count)
                        {
                            pointsToRemove--;
                        }
                        if (pointsToRemove > 0)
                        {
                            series.Points.RemoveRange(0, pointsToRemove);
                        }
                        series.Points.Add(new SeriesPoint(entry.Time, Convert.ToDouble(entry.Value)));
                    }
                }
            }

            if (AxisXRange != null)
            {
                AxisXRange.SetMinMaxValues(minDate, maxDate);
            }
        }

        #endregion Private Methods

        #region Private Properties

        /// <summary>
        /// The X-axis's range property.
        /// </summary>
        private AxisRange AxisXRange
        {
            get
            {
                SwiftPlotDiagram diagram = chartMain.Diagram as SwiftPlotDiagram;
                //if (diagram != null)
                //    return diagram.AxisX.Range;
                return null;
            }
        }

        #endregion Private Properties

    }
}
