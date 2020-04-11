using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using mgWinChart.Helpers;

namespace mgWinChart.Interfaces
{
    public interface IAreaChart
    {
        LabelAngle LabelAngle { get; set; }
        MarkerKind MarkerKind { get; set; }
        MarkerSize MarkerSize { get; set; }
        bool ShowMarkers { get; set; }
    }
}
