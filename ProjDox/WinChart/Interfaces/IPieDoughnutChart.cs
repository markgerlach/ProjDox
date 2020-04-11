using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;

namespace mgWinChart.Interfaces
{
    public interface IPieDoughnutChart
    {
        int ExplodeDistance { get; set; }
        string ExplodedPoints { get; set; }
        PieSeriesLabelPosition PieLabelPosition { get; set; }
        HoleRadius HoleRadius { get; set; }
    }
}
