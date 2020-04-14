using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraCharts;

using mgWinChart.Helpers;
using ProjDox;

namespace mgWinChart.Test_Forms
{
    public partial class frmDevXChartTester : frm_ProjDox
	{
        public frmDevXChartTester()
        {
            InitializeComponent();
        }

        private void frmDevXChartTester_Load(object sender, EventArgs e)
        {
            SecondaryAxisY axisY2 = new SecondaryAxisY();
            ((XYDiagram)chart.Diagram).SecondaryAxesY.Add(axisY2);
            ((BarSeriesView)chart.Series[0].View).AxisY = axisY2;
            ((XYDiagram)chart.Diagram).AxisX.Alignment = AxisAlignment.Zero;
        }
    }
}
