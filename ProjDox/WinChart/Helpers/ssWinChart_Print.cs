using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraCharts;
using DevExpress.Utils.Drawing;

using DevExpress.XtraNavBar;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

using mgControls;
using mgCustom;
using mgModel;
using mgWinChart.usercontrols;

namespace mgWinChart.Helpers
{
    public class ssWinChart_Print
    {

        #region Private Variables

        private XtraReport _report = new XtraReport();
        private mgPrintOrientation _orientation = mgPrintOrientation.Portrait;
		private mgWinChart.usercontrols.ucWinChart _chart = null;
		private string _reportTitle = string.Empty;
		private bool _showDatePrinted = true;
		private bool _showPageNumbersInFooter = true;
		private string _filterString = string.Empty;
		private bool _showFilter = true;
        //private System.Drawing.Printing.Margins _margins = null;
		private string _column0HeaderText = string.Empty;

        #endregion Private Variables

        #region Constructors

        public ssWinChart_Print(mgWinChart.usercontrols.ucWinChart chart)
        {
            _chart = chart;

            _report.Margins.Left = 50;
            _report.Margins.Top = 50;
            _report.Margins.Bottom = 50;
            _report.Margins.Right = 50;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// The report to print.
        /// </summary>
        public XtraReport Report
        {
            get { return _report; }
            set { _report = value; }
        }

        /// <summary>
		/// Return the DevX chart this object prints.
		/// </summary>
		public ucWinChart Chart
		{
			get 
			{
                return _chart;
			}
            set
            {
                _chart = value;
            }
		}

		/// <summary>
		/// The first column's header text.
		/// </summary>
		public string Column0HeaderText
		{
			get { return _column0HeaderText; }
			set { _column0HeaderText = value; }
		}

		/// <summary>
		/// The report's orientation: Portrait or Landscape.
		/// </summary>
		public mgPrintOrientation Orientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}

		/// <summary>
		/// The report title.  Not the same as the WinChart.Title.
		/// </summary>
		public string ReportTitle
		{
			get { return _reportTitle; }
			set { _reportTitle = value; }
		}

		/// <summary>
		/// Show the date field at the top right of the report.
		/// </summary>
		public bool ShowDatePrinted
		{
			get { return _showDatePrinted; }
			set { _showDatePrinted = value; }
		}

		/// <summary>
		/// Show the page numbers in the footer.
		/// </summary>
		public bool ShowPageNumbersInFooter
		{
			get { return _showPageNumbersInFooter; }
			set { _showPageNumbersInFooter = value; }
		}

		/// <summary>
		/// The margins for the report.
		/// </summary>
		public System.Drawing.Printing.Margins Margins
		{
			get { return this.Report.Margins; }
			set { this.Report.Margins = value; }
		}

		/// <summary>
		/// The filter string on the report.
		/// </summary>
		public string FilterString
		{
			get { return _filterString; }
			set { _filterString = value; }
		}

		/// <summary>
		/// Show the filter string on the report.
		/// </summary>
		public bool ShowFilter
		{
			get { return _showFilter; }
			set { _showFilter = value; }
		}
        
        #endregion Public Properties

        #region Public Methods

        ///// <summary>
        ///// Generates a printing system object 
        ///// </summary>
        ///// <returns>The printing system object that you want to use for a report</returns>
        //public PrintingSystem GeneratePrintingSystem()
        //{
        //    PrintingSystem ps = new PrintingSystem();

        //    ps.PreviewFormEx.SaveState = false;
        //    ps.PreviewFormEx.PrintBarManager.MainMenu.Visible = false;
        //    ps.PreviewFormEx.SaveState = true;
        //    //ps.PreviewFormEx.Icon = mainMDIForm.Icon;

        //    //System.Drawing.Printing.Margins margins = ps.PageMargins;
        //    //if (_margins != null)
        //    //{
        //    //    margins.Left = _margins.Left;
        //    //    margins.Top = _margins.Top;
        //    //    margins.Bottom = _margins.Bottom;
        //    //    margins.Right = _margins.Right;
        //    //}
        //    //else
        //    //{
        //    //    margins.Left = 50;
        //    //    margins.Top = 137;
        //    //    margins.Bottom = 87;
        //    //    margins.Right = 50;
        //    //}

        //    if (_margins != null)
        //    {
        //        ps.PageSettings.LeftMargin = _margins.Left;
        //        ps.PageSettings.TopMargin = _margins.Top;
        //        ps.PageSettings.BottomMargin = _margins.Bottom;
        //        ps.PageSettings.RightMargin = _margins.Right;
        //    }
        //    else
        //    {
        //        ps.PageSettings.LeftMargin = 50;
        //        ps.PageSettings.TopMargin = 137;
        //        ps.PageSettings.BottomMargin = 87;
        //        ps.PageSettings.RightMargin = 50;
        //    }

        //    if (_orientation == _BaseWinProjectPrintOrientation.Landscape)
        //    {
        //        ps.PageSettings.Landscape = true;
        //    }

        //    return ps;		// Return the printing system
        //}

		/// <summary>
		/// Generate a component link for the report
		/// </summary>
		/// <returns>The printable component link object</returns>
        //public PrintableComponentLink GenerateComponentLink()
        //{
        //    //PrintingSystem ps = new PrintingSystem();

        //    //ps.PreviewFormEx.SaveState = false;
        //    //ps.PreviewFormEx.PrintBarManager.MainMenu.Visible = false;
        //    //ps.PreviewFormEx.SaveState = true;
        //    ////ps.PreviewFormEx.Icon = mainMDIForm.Icon;

        //    //System.Drawing.Printing.Margins margins = ps.PageMargins;
        //    //if (_margins != null)
        //    //{
        //    //    margins.Left = _margins.Left;
        //    //    margins.Top = _margins.Top;
        //    //    margins.Bottom = _margins.Bottom;
        //    //    margins.Right = _margins.Right;
        //    //}
        //    //else
        //    //{
        //    //    margins.Left = 50;
        //    //    margins.Top = 137;
        //    //    margins.Bottom = 87;
        //    //    margins.Right = 50;
        //    //}

        //    //PrintableComponentLink link = new PrintableComponentLink(ps);
        //    PrintingSystem ps = GeneratePrintingSystem();
        //    PrintableComponentLink link = new PrintableComponentLink(ps);
        //    link.Component = _chart.DevXChart;

        //    //link.CreateDocument();

        //    //link.Margins = margins;
        //    link.Margins = ps.PageMargins;

        //    link.CreateMarginalHeaderArea -= new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
        //    link.CreateReportFooterArea -= new CreateAreaEventHandler(link_CreateReportFooterArea);
        //    link.CreateMarginalFooterArea -= new CreateAreaEventHandler(link_CreateMarginalFooterArea);

        //    link.CreateMarginalHeaderArea += new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
        //    link.CreateReportFooterArea += new CreateAreaEventHandler(link_CreateReportFooterArea);
        //    link.CreateMarginalFooterArea += new CreateAreaEventHandler(link_CreateMarginalFooterArea);

        //    if (_orientation == _BaseWinProjectPrintOrientation.Landscape)
        //    {
        //        link.Landscape = true;
        //        //link.PrintingSystem.PageSettings.Landscape = true;
        //    }

        //    return link;
        //}

        /// <summary>
        /// Build the report and show it in the preview window.
        /// </summary>
        /// <param name="mainMDIForm">The form that the preview form should dock in (not currently implemented).</param>
        public void ShowPreview(System.Windows.Forms.Form mainMDIForm)
        {
            this.ShowPreview(mainMDIForm, false);
        }

        /// <summary>
        /// Build the report and show it in the preview window.
        /// </summary>
        /// <param name="mainMDIForm">The form that the preview form should dock in (not currently implemented).</param>
        /// <param name="flipTables">Whether to flip the tables: to use the Series' as the columns and Arguments for the rows rather than the other way around.</param>
        public void ShowPreview(System.Windows.Forms.Form mainMDIForm, bool flipTables)
        {
            // Check whether the XtraGrid control can be previewed.
            if (!_chart.DevXChart.IsPrintingAvailable)
            {
                MessageBox.Show("The 'DevExpress.XtraPrinting' library could not be found.", "Error");
                return;
            }

            // Set the page orientation.
            _report.Landscape = _report.PrintingSystem.PageSettings.Landscape = (_orientation == mgPrintOrientation.Landscape);

            // Detail band
            #region Detail Band

            DetailBand detailband = _report.Bands.GetBandByType(typeof(DevExpress.XtraReports.UI.DetailBand)) as DetailBand;
            if (detailband == null)
            {
                detailband = new DetailBand();
                _report.Bands.Add(detailband);
            }
            XRChart xrchart = _chart.ChartForReport;
            List<XRTable> xrtables = _chart.DataTablesForReport;
            detailband.Controls.Add(xrchart);
            _report.ReportUnit = ReportUnit.HundredthsOfAnInch;
            xrchart.WidthF = _report.PageWidth - (_report.Margins.Left + _report.Margins.Right);
            xrchart.Height = (_report.PageHeight - (_report.Margins.Top + _report.Margins.Bottom)) / 2;
            float tableTop = xrchart.BottomF;
            foreach (XRTable sourceTable in xrtables)
            {
                XRTable table = sourceTable;
                if (flipTables)
                {
                    table = ssWinChart_Print.FlipXRTable(sourceTable);
                }
                table.TopF = tableTop;
                tableTop = table.BottomF + 10;
                table.WidthF = xrchart.WidthF;
                detailband.Controls.Add(table);
            }

            #endregion Detail Band
            
            // Header band
            #region Header Band

            PageHeaderBand headerband = _report.Bands.GetBandByType(typeof(PageHeaderBand)) as PageHeaderBand;
            if (headerband == null)
            {
                headerband = new PageHeaderBand();
                _report.Bands.Add(headerband);
            }

            int top = 20, leftMargin = 1, rightMargin = _report.PageWidth - (_report.Margins.Left + _report.Margins.Right + 2);
            int titleTop = 0, lineSpacer = 4, lineIndent = 10;

            // Draw two lines at the top of the header
            headerband.Controls.Add(this.GetLine(position: new PointF(leftMargin, top), width: rightMargin - leftMargin));
            top += lineSpacer;
            headerband.Controls.Add(this.GetLine(position: new PointF(leftMargin, top), width: rightMargin - leftMargin));
            top += lineSpacer;

            // Draw the title
            titleTop = top;
            XRLabel lblTitle = new XRLabel();
            lblTitle.TextAlignment = TextAlignment.BottomLeft;
            lblTitle.Font = new Font("Tahoma", 14, FontStyle.Bold | FontStyle.Italic);
            lblTitle.LocationF = new PointF(lineIndent, titleTop);
            lblTitle.WidthF = rightMargin / 2;
            lblTitle.ForeColor = Color.Blue;
            lblTitle.Text = _reportTitle;
            headerband.Controls.Add(lblTitle);
            top += (int)lblTitle.HeightF + (lineSpacer / 2);

            // Draw the date printed
            if (_showDatePrinted)
            {
                XRLabel lblDatePrinted = new XRLabel();
                lblDatePrinted.Text = "Date Printed: " + DateTime.Now.ToString("MMM dd, yyy hh:mm tt");
                lblDatePrinted.TextAlignment = TextAlignment.BottomLeft;
                lblDatePrinted.Font = new Font("Tahoma", 8, FontStyle.Italic);
                lblDatePrinted.LocationF = new PointF(lineIndent * 2, top);
                lblDatePrinted.WidthF = rightMargin / 2;
                lblDatePrinted.ForeColor = Color.Black;
                headerband.Controls.Add(lblDatePrinted);
                top += (int)lblDatePrinted.HeightF;
            }

            // Show the filter if need be
            if (_showFilter &&
                !String.IsNullOrEmpty(_filterString))
            {
                top += lineSpacer;
                XRLabel lblFilter = new XRLabel();
                lblFilter.Text = "Filter: " + _filterString;
                lblFilter.TextAlignment = TextAlignment.BottomLeft;
                lblFilter.Font = new Font("Tahoma", 8, FontStyle.Bold);
                lblFilter.CanGrow = true;
                lblFilter.WordWrap = true;
                lblFilter.LocationF = new PointF(lineIndent * 2, top);
                lblFilter.WidthF = rightMargin / 2;
                lblFilter.ForeColor = Color.Black;
                //float labelwidth = e.Graph.ClientPageSize.Width / 2f;
                //textSize = e.Graph.MeasureString(fullFilterString);
                //decimal numOfLines = Math.Ceiling(Convert.ToDecimal(textSize.Width / labelwidth));
                //float labelheight = textSize.Height * (float)numOfLines;
                //e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
                //rec = new RectangleF(lineIndent * 2, top, labelwidth, labelheight);
                //e.Graph.DrawString(fullFilterString, Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);
                headerband.Controls.Add(lblFilter);
                top += (int)lblFilter.HeightF;
            }

            // Draw the block off to the right - wait until you have the height of the square
            // Draw the black block
            XRPictureBox pbxLogo = new XRPictureBox();
            pbxLogo.Sizing = ImageSizeMode.AutoSize;
            pbxLogo.BackColor = Color.Black;
            pbxLogo.Image = UserInterface.SizeImageKeepAspectRatio(UserInterface.GetCompanyImage(), null, top - titleTop);
            string agencyName = UserInterface.GetCompanyName();
            pbxLogo.LeftF = rightMargin - (lineIndent + pbxLogo.WidthF);
            pbxLogo.TopF = titleTop;
            pbxLogo.Borders = DevExpress.XtraPrinting.BorderSide.All;
            headerband.Controls.Add(pbxLogo);

            XRLabel lblCompanyName = new XRLabel();
            lblCompanyName.BackColor = Color.Black;
            lblCompanyName.ForeColor = Color.White;
            lblCompanyName.Text = UserInterface.GetCompanyName();
            lblCompanyName.TextAlignment = TextAlignment.MiddleRight;
            lblCompanyName.Font = new Font("Tahoma", 8, FontStyle.Bold);
            lblCompanyName.HeightF = pbxLogo.HeightF;
            lblCompanyName.WidthF += lineIndent * 5;
            lblCompanyName.TopF = pbxLogo.TopF;
            lblCompanyName.LeftF = pbxLogo.LeftF - lblCompanyName.WidthF;
            lblCompanyName.Borders = DevExpress.XtraPrinting.BorderSide.All;
            headerband.Controls.Add(lblCompanyName);

            // Draw the two lines at the bottom of the header
            top += lineSpacer;
            headerband.Controls.Add(this.GetLine(position: new PointF(leftMargin, top), width: rightMargin - leftMargin));
            top += lineSpacer;
            headerband.Controls.Add(this.GetLine(position: new PointF(leftMargin, top), width: rightMargin - leftMargin));

            #endregion Header Band

            // Footer band
            #region Footer Band

            PageFooterBand footerband = _report.Bands.GetBandByType(typeof(PageFooterBand)) as PageFooterBand;
            if (footerband == null)
            {
                footerband = new PageFooterBand();
                _report.Bands.Add(footerband);
            }

            if (_showPageNumbersInFooter)
            {
                XRPageInfo pgiPageInfo = new XRPageInfo();
                pgiPageInfo.TextAlignment = TextAlignment.TopRight;
                pgiPageInfo.Font = new Font("Tahoma", 8);
                pgiPageInfo.WidthF = 200;
                pgiPageInfo.LocationF = new PointF(rightMargin - 201, 0);
                pgiPageInfo.ForeColor = Color.Black;
                pgiPageInfo.Format = "Page {0} of {1}";
                pgiPageInfo.PageInfo = PageInfo.NumberOfTotal;
                footerband.Controls.Add(pgiPageInfo);
                footerband.HeightF = pgiPageInfo.HeightF + 1;
            }

            #endregion Footer Band

            _report.ShowPreview();

            //    //link.CreateMarginalHeaderArea -= new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
        //    //link.CreateReportFooterArea -= new CreateAreaEventHandler(link_CreateReportFooterArea);
        //    //link.CreateMarginalFooterArea -= new CreateAreaEventHandler(link_CreateMarginalFooterArea);

        //    //link.CreateMarginalHeaderArea += new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
        //    //link.CreateReportFooterArea += new CreateAreaEventHandler(link_CreateReportFooterArea);
        //    //link.CreateMarginalFooterArea += new CreateAreaEventHandler(link_CreateMarginalFooterArea);

        //    //if (_orientation == _BaseWinProjectPrintOrientation.Landscape) 
        //    //{
        //    //    link.Landscape = true;
        //    //    link.PrintingSystem.PageSettings.Landscape = true; 
        //    //}

        //    UserInterface.SetProgressSubValue(0.5m, "Drawing chart...");

        //    //link.CreateDetailArea += new CreateAreaEventHandler(link_CreateDetailArea);
        //    link.CreateDocument();

        //    UserInterface.SetProgressSubValue(0.8m, "Opening preview window...");

        //    // Opens the Preview window.
        //    link.PrintingSystem.PreviewFormEx.MdiParent = mainMDIForm;
        //    link.ShowPreview();

        //    UserInterface.SetProgressSubValue(100m, "Done");
        //    UserInterface.KillProgress();

        //    return link.PrintingSystem.PreviewFormEx;		// Return the form that was just created
        }

        #endregion Public Methods

        #region Public Static Methods

        /// <summary>
        /// Creates a new XRTable from an old one, swapping the rows with the columns.
        /// </summary>
        /// <param name="sourceTable">The old XRTable.</param>
        /// <returns>A new XRTable, with the same values but rows in place of columns and vice versa.</returns>
        public static XRTable FlipXRTable(XRTable sourceTable)
        {
            XRTable newTable = XRTable.CreateTable(sourceTable.BoundsF, sourceTable.Rows.FirstRow.Cells.Count, sourceTable.Rows.Count);
            newTable.Borders = DevExpress.XtraPrinting.BorderSide.All;
            for (int i = 0; i < sourceTable.Rows.FirstRow.Cells.Count; i++)
            {
                for (int j = 0; j < sourceTable.Rows.Count; j++)
                {
                    XRTableCell oldCell = sourceTable.Rows[j].Cells[i];
                    XRTableCell newCell = newTable.Rows[i].Cells[j];
                    newCell.Text = oldCell.Text;
                    newCell.Font = oldCell.Font;
                }
            }

            return newTable;
        }

        #endregion Public Static Methods

        #region Class Events

        public delegate void DetailCreatedEventHandler(object sender, mgCreateAreaEventArgs e);
		public event DetailCreatedEventHandler DetailCreated;
		public void OnDetailCreated(mgCreateAreaEventArgs e)
		{
			if (DetailCreated != null)
			{
				DetailCreated(this, e);
			}
		}

        #endregion Class Events

        #region Event Handlers

        private static void link_CreateDetailArea(object sender, CreateAreaEventArgs e)
		{
			//MWSCreateAreaEventArgs newEvent = new MWSCreateAreaEventArgs();
			//newEvent.PageNum = e.Graph.PrintingSystem.Pages.Count;
			//newEvent.NumOfTotal = 100000;
			//this.OnDetailCreated(newEvent);
		}

		private void link_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
		{
			
		}

        private void link_CreateReportFooterArea(object sender, CreateAreaEventArgs e)
        {
            //float topMargin = 10f, leftMargin = 1f;
            //float x = leftMargin, y = topMargin;

            //e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
            //e.Graph.Font = new Font("Tahoma", 11, FontStyle.Bold);

            //string headerColumn0Text = "Arguments";
            //if (!String.IsNullOrEmpty(_column0HeaderText)) { headerColumn0Text = _column0HeaderText; }

            //// Compile the list of arguments, and get the width of each column.
            //Dictionary<string, float> columnWidths = new Dictionary<string,float>();
            //columnWidths.Add("Arguments", e.Graph.MeasureString(headerColumn0Text).Width * 1.2f);
            //List<string> arguments = new List<string>();
            //foreach (WinChartSeries series in _chart.Series)
            //{
            //    // Add the series title to the dictionary.
            //    columnWidths.Add(series.Name, e.Graph.MeasureString(series.Name).Width * 1.2f);

            //    foreach (SeriesPoint point in series.Points)
            //    {
            //        // Add each argument to the list.
            //        if (!arguments.Contains(point.Argument))
            //        {
            //            arguments.Add(point.Argument);
            //            float newWidth = e.Graph.MeasureString(point.Argument).Width;
            //            columnWidths["Arguments"] = Math.Max(columnWidths["Arguments"], newWidth);
            //        }

            //        // Get the widest text-width.
            //        columnWidths[series.Name] = Math.Max(columnWidths[series.Name], 
            //            e.Graph.MeasureString(point.Values[0].ToString()).Width);
            //    }
            //}
            
            //// Draw the column headers.
            //RectangleF rec = new RectangleF();
            //float textHeight = e.Graph.MeasureString(headerColumn0Text).Height + 2f;

            //foreach (KeyValuePair<string, float> kvp in columnWidths)
            //{                
            //    rec = new RectangleF(x, y, kvp.Value, textHeight);
            //    e.Graph.DrawString(kvp.Key, Color.Black, rec, DevExpress.XtraPrinting.BorderSide.All);
            //    x += kvp.Value;
            //}

            //x = leftMargin;
            //y += textHeight;

            //// Draw the rows.
            //e.Graph.Font = new Font("Tahoma", 11f);
            //arguments.Sort();

            //foreach (string arg in arguments)
            //{
            //    rec = new RectangleF(x, y, columnWidths["Arguments"], textHeight);
            //    e.Graph.DrawString(arg, Color.Black, rec, DevExpress.XtraPrinting.BorderSide.All);
            //    x += columnWidths["Arguments"];

            //    foreach (WinChartSeries series in _chart.Series)
            //    {
            //        rec = new RectangleF(x, y, columnWidths[series.Name], textHeight);
            //        e.Graph.DrawString(ssWinChart_Print.GetValueForSeriesArgument(series, arg), 
            //            Color.Black, rec, DevExpress.XtraPrinting.BorderSide.All);
            //        x += columnWidths[series.Name];
            //    }

            //    x = leftMargin;
            //    y += textHeight;
            //}

        }

		private void link_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
		{
			
        }

        #endregion Event Handlers

        private XRLine GetLine(PointF position, float width)
        {
            XRLine line = new XRLine();

            line.LocationF = position;
            line.SizeF = new SizeF(width, 1);
            line.ForeColor = Color.Black;
            line.LineWidth = 1;

            return line;
        }

        //private static string GetValueForSeriesArgument(Series series, string argument)
        //{
        //    foreach (SeriesPoint point in series.Points)
        //    {
        //        if (point.Argument == argument)
        //        {
        //            return point.Values[0].ToString();
        //        }
        //    }
        //    return string.Empty;
        //}

    }
}