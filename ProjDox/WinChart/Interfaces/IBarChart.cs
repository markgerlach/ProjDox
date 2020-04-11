using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;

namespace mgWinChart.Interfaces
{
    public interface IBarChart
    {
        Bar3DModel ModelType { get; set; }
        BarSeriesLabelPosition BarLabelPosition { get; set; }
    }
}
