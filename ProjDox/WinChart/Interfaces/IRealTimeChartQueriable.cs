using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using mgWinChart.Helpers;

namespace mgWinChart.Interfaces
{
    public interface IRealTimeChartQueriable
    {
        /// <summary>
        /// Returns the latest data points for each series, in the order those series appeared in when SetSeriesNames() was called.
        /// </summary>
        List<RealTimeDataEntry> GetLatestData();
    }
}
