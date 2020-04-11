using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;

namespace mgWinChart.Interfaces
{
    public interface IChartingBase
    {
        string ChartTitle { get; set; }
        bool ValueAsPercent { get; set; }
        //LabelPosition LabelPositioning { get; set; }
        LegendPosition LegendPositioning { get; set; }
		//ChartAppearance ChartingAppearance { get; set; }
		Palette Palette { get; set; }
    }
}