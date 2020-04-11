using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mgWinChart.Interfaces
{
    /// <summary>
    /// Interface that inherits IChartingBase, IAreaChart, IBarChart, IPieDoughnutChart.
    /// </summary>
    public interface IMultiChart: IChartingBase, IAreaChart, IBarChart, IPieDoughnutChart
    {

    }
}