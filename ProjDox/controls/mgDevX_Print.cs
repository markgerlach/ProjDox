using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

using DevExpress.Data;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraGrid.Columns;

using mgCustom;
using mgModel;

namespace mgControls
{
	public class mgDevX_Print
	{
		#region Private Variables
		private mgPrintOrientation _orientation = mgPrintOrientation.Portrait;
		private string _reportTitle = string.Empty;
		private bool _showDatePrinted = true;
		private bool _showPageNumbersInFooter = true;
		private string _filterString = string.Empty;
		private bool _showFilter = true;
		private System.Drawing.Printing.Margins _margins = null;
		private List<object> _controlArray = new List<object>();
		#endregion Private Variables

		#region Constructor(s)

		public mgDevX_Print()
		{
			// Set the default margins
			ResetMargins();
		}

		#endregion Constructor(s)

		#region Public Properties
		/// <summary>
		/// The array of controls to work with
		/// </summary>
		public List<object> ControlArray
		{
			get { return _controlArray; }
			set { _controlArray = value; }
		}

		/// <summary>
		/// The orientation for the report
		/// </summary>
		public mgPrintOrientation Orientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}

		/// <summary>
		/// The report title to show on the report
		/// </summary>
		public string ReportTitle
		{
			get { return _reportTitle; }
			set { _reportTitle = value; }
		}

		/// <summary>
		/// Show the date field at the top right of the report
		/// </summary>
		public bool ShowDatePrinted
		{
			get { return _showDatePrinted; }
			set { _showDatePrinted = value; }
		}

		/// <summary>
		/// Show the page numbers in the footer
		/// </summary>
		public bool ShowPageNumbersInFooter
		{
			get { return _showPageNumbersInFooter; }
			set { _showPageNumbersInFooter = value; }
		}

		/// <summary>
		/// Get the margins for the report
		/// </summary>
		public System.Drawing.Printing.Margins Margins
		{
			get { return _margins; }
			set  { _margins = value; }
		}

		/// <summary>
		/// The filter string on the report
		/// </summary>
		public string FilterString
		{
			get { return _filterString; }
			set { _filterString = value; }
		}

		/// <summary>
		/// Show the filter string on the report
		/// </summary>
		public bool ShowFilter
		{
			get { return _showFilter; }
			set { _showFilter = value; }
		}
		#endregion Public Properties

		#region Public Methods
		/// <summary>
		/// Reset the margins
		/// </summary>
		public void ResetMargins()
		{
			_margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
		}
		
		/// <summary>
		/// Generate the report and pass it back to the calling function
		/// </summary>
		/// <returns>The report that's been generated</returns>
		public XtraReport GenerateReport()
		{
			// Declare some variables
			XtraReport rpt = new XtraReport();

			// Wire up an event handler for the print progress
			rpt.PrintProgress += new DevExpress.XtraPrinting.PrintProgressEventHandler(rpt_PrintProgress);

			// Set the landscape
			rpt.Landscape = (_orientation == mgPrintOrientation.Landscape ? true : false); 

			// Set the margins
			rpt.Margins = _margins;

			// Create a graphics object that we can work with
			Image img = new Bitmap(10, 10);
			Graphics g = Graphics.FromImage(img);

			// Generate the page header band
			rpt.Bands.Add(GeneratePageHeader(rpt, g));

			// Generate the page footer band
			rpt.Bands.Add(GeneratePageFooter(rpt));

			// Go through each control and generate the objects and place them in the detail
			if (rpt.Bands[BandKind.Detail] == null)
			{
				DetailBand detailBand = new DetailBand();
				rpt.Bands.Add(detailBand);
			}
			int top = 0, lineSpacer = 4;
			foreach (object cntrl in _controlArray)
			{
				if (cntrl is mgWinChart.usercontrols.ucWinChart)
				{
					#region Print Chart
					XRChart rptChart = new XRChart();
					IChartContainer cc = rptChart as IChartContainer;
					cc.Chart.Assign(((IChartContainer)((mgWinChart.usercontrols.ucWinChart)cntrl).DevXChart).Chart);
					rptChart.LocationF = new PointF(0, 0);
					rptChart.WidthF = rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right;
					rptChart.HeightF = rptChart.WidthF / 2;
					
					rpt.Bands[BandKind.Detail].Controls.Add(rptChart);

					top = (int)rptChart.BottomF + (lineSpacer * 5);
					#endregion Print Chart
				}
				else if (cntrl is mgDevX_GridControl)
				{
					#region Print Grid
					// Try to do this in a sub report
					XtraReport rptGrid = new XtraReport();
					rptGrid.Landscape = rpt.Landscape;

					mgDevX_GridControl gridDetail = (cntrl as mgDevX_GridControl);

					List<string> visibleFieldNames = new List<string>();
					rptGrid.DataSource = (IBindingList)gridDetail.DataSource;
					for (int i = gridDetail.GridView.GroupCount - 1; i >= 0; i--)
					{
						GroupHeaderBand gb = new GroupHeaderBand();
						gb.Height = 25;
						gb.RepeatEveryPage = true;
						XRLabel l = new XRLabel();
						l.DataBindings.Add("Text", null, gridDetail.GridView.GroupedColumns[i].Caption);
						l.Size = new Size(300, 25);
						l.Location = new Point(0 + i * 20, 0);
						l.BackColor = Color.Beige;
						gb.Controls.Add(l);
						GroupField gf;
						if (gridDetail.GridView.GroupedColumns[i].SortOrder == ColumnSortOrder.Ascending)
						{
							gf = new GroupField(gridDetail.GridView.GroupedColumns[i].FieldName, XRColumnSortOrder.Ascending);
						}
						else
						{
							gf = new GroupField(gridDetail.GridView.GroupedColumns[i].FieldName, XRColumnSortOrder.Descending);
						}
						gb.GroupFields.Add(gf);
						rptGrid.Bands.Add(gb);
					}
					Dictionary<string, int> fieldWidths = new Dictionary<string, int>();
					int gridViewWidth = gridDetail.Width;
					int pageWidth = (rptGrid.PageWidth - (rptGrid.Margins.Left + rptGrid.Margins.Right));
					for (int i = 0; i < gridDetail.GridView.Columns.Count; i++)
					{
						if (gridDetail.GridView.Columns[i].Visible &&
							gridDetail.GridView.Columns[i].GroupIndex < 0)
						{
							visibleFieldNames.Add(gridDetail.GridView.Columns[i].FieldName);
							fieldWidths.Add(gridDetail.GridView.Columns[i].FieldName,
								(int)((gridDetail.GridView.Columns[i].VisibleWidth / (decimal)gridViewWidth) * pageWidth));
						}
					}

					int colCount = visibleFieldNames.Count;

					XRTable table = new XRTable();
					XRTableRow row = new XRTableRow();
					XRTable table2 = new XRTable();
					XRTableRow row2 = new XRTableRow();

					// Set the style
					TextAlignment alignment = TextAlignment.TopLeft;
					XRControlStyle oddStyle = new XRControlStyle();
					oddStyle.Name = "OddStyleGrid";
					oddStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));

					rptGrid.StyleSheet.Add(oddStyle);

					int cellPadding = 5;
					for (int i = 0; i < colCount; i++)
					{
						// Get the column we're working with here
						GridColumn col = gridDetail.GridView.Columns[visibleFieldNames[i]];

						// Find out the format column
						string formatString = col.DisplayFormat.FormatString;
						if (String.IsNullOrEmpty(formatString))
						{
							if (col.ColumnType == typeof(System.Decimal))
							{
								formatString = "#,###,##0.###";
							}
							else if (col.ColumnType == typeof(System.Int16) ||
								col.ColumnType == typeof(System.Int32) ||
								col.ColumnType == typeof(System.Int64))
							{
								formatString = "#,###,##0";
							}
							else if (col.ColumnType == typeof(System.DateTime) ||
								col.ColumnType == typeof(System.DateTime?))
							{
								formatString = "MM/dd/yyyy";
							}
						}

						// Add the header
						XRTableCell cell = new XRTableCell();
						cell.Width = fieldWidths[col.FieldName];
						cell.Text = col.Caption;
						cell.BackColor = Color.Navy;
						cell.ForeColor = Color.White;
						cell.Font = gridDetail.GridView.Appearance.HeaderPanel.Font;
						cell.TextAlignment = TextAlignment.MiddleCenter;
						row.Cells.Add(cell);

						// Add the cell
						XRTableCell cell2 = new XRTableCell();
						cell2.Width = fieldWidths[col.FieldName];
						if (!String.IsNullOrEmpty(formatString))
						{
							cell2.DataBindings.Add("Text", null, col.FieldName, "{0:" + formatString + "}");
						}
						else
						{
							cell2.DataBindings.Add("Text", null, col.FieldName);
						}
						cell2.Font = col.AppearanceCell.Font;
						cell2.Borders = BorderSide.All;
						cell2.BorderColor = Color.LightGray;
						cell2.BorderWidth = 1;
						cell2.OddStyleName = "OddStyleGrid";
						cell2.TextAlignment = TextAlignment.MiddleLeft;
						cell2.HeightF = 10;
						cell2.Padding = new PaddingInfo(cellPadding, cellPadding, cellPadding, cellPadding);
						if (col.ColumnType == typeof(System.Decimal) ||
							col.ColumnType == typeof(System.Int16) ||
							col.ColumnType == typeof(System.Int32) ||
							col.ColumnType == typeof(System.Int64))
						{
							cell2.TextAlignment = TextAlignment.MiddleRight;
						}
						else if (col.ColumnType == typeof(System.DateTime) ||
							col.ColumnType == typeof(System.DateTime?))
						{
							cell2.TextAlignment = TextAlignment.MiddleCenter;
						}
						row2.Cells.Add(cell2);
					}

					table.Rows.Add(row);
					table.Width = pageWidth;
					table.Borders = BorderSide.Bottom;

					table2.Rows.Add(row2);
					table2.Width = pageWidth;

					if (rptGrid.Bands[BandKind.Detail] == null) { DetailBand detailBand = new DetailBand(); rptGrid.Bands.Add(detailBand); }
					if (rptGrid.Bands[BandKind.PageHeader] == null) { PageHeaderBand pageHeaderBand = new PageHeaderBand(); rptGrid.Bands.Add(pageHeaderBand); }

					rptGrid.Bands[BandKind.PageHeader].Controls.Add(table);
					rptGrid.Bands[BandKind.PageHeader].HeightF = table.Rows[0].Cells[0].HeightF;
					rptGrid.Bands[BandKind.PageHeader].KeepTogether = true;

					rptGrid.Bands[BandKind.Detail].Controls.Add(table2);
					rptGrid.Bands[BandKind.Detail].HeightF = table2.Rows[0].Cells[0].HeightF;
					rptGrid.Bands[BandKind.Detail].KeepTogether = true;

					XRSubreport subReport = new XRSubreport();
					subReport.LocationF = new PointF(0, top);
					subReport.SizeF = new SizeF(rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right, 50);
					subReport.ReportSource = rptGrid;
					subReport.CanGrow = true;
					subReport.CanShrink = false;
					rpt.Bands[BandKind.Detail].Controls.Add(subReport);

					// Add to the top
					top = (int)subReport.BottomF + (lineSpacer * 5);
					#endregion Print Grid
				}
				else
				{
					throw new Exception("The control type: " + cntrl.GetType().ToString() + " is not supported by this class.");
				}
			}

			// Dispose of the image and graphics element
			g.Dispose();
			img.Dispose();

			// Return the report
			return rpt;
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Generate the page header band
		/// </summary>
		/// <param name="rpt">The report for use with this Page Header</param>
		/// <param name="g">A graphics object that can be used for image generation and measuring</param>
		/// <returns>The page header band for the report</returns>
		private PageHeaderBand GeneratePageHeader(XtraReport rpt, Graphics g)
		{
			#region Page Header
			int top = 0, titleTop = 0, lineSpacer = 4, lineIndent = 10;

			// Set up the font objects
			Font titleFont = new Font("Tahoma", 14, FontStyle.Bold | FontStyle.Italic);
			Font dateFont = new Font("Tahoma", 8, FontStyle.Italic);
			Font filterFont = new Font("Tahoma", 8, FontStyle.Bold);

			PageHeaderBand phBand = new PageHeaderBand();

			// Draw two lines at the top of the header
			XRLine line1 = new XRLine();
			line1.LocationF = new PointF(0, top);
			line1.SizeF = new SizeF(rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right, 3);
			line1.ForeColor = Color.Black;
			line1.LineWidth = 1;

			top += lineSpacer;
			XRLine line2 = new XRLine();
			line2.LocationF = new PointF(0, top);
			line2.SizeF = new SizeF(rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right, 3);
			line2.ForeColor = Color.Black;
			line2.LineWidth = 1;

			top += lineSpacer;


			// Draw the title
			titleTop = top;
			XRLabel lblTitle = new XRLabel();
			lblTitle.Font = titleFont;
			lblTitle.Text = _reportTitle;
			SizeF textSize = g.MeasureString(_reportTitle, titleFont);
			lblTitle.LocationF = new PointF(lineIndent, top);
			lblTitle.SizeF = new SizeF(textSize.Width + 10, textSize.Height);
			lblTitle.ForeColor = Color.Blue;

			top += (int)Math.Ceiling(textSize.Height);

			// Draw the date
			XRLabel lblDate = new XRLabel();
			if (_showDatePrinted)
			{
				lblDate.Font = dateFont;
				string dateReportRun = "Date Printed: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt");
				lblDate.Text = dateReportRun;
				textSize = g.MeasureString(dateReportRun, dateFont);
				lblDate.LocationF = new PointF(lineIndent * 2, top);
				lblDate.SizeF = new SizeF(textSize.Width + 10, textSize.Height);

				top += (int)Math.Ceiling(textSize.Height);
			}

			// Draw the filter
			XRLabel lblFilter = new XRLabel();
			if (_showFilter)
			{
				lblFilter.Font = filterFont;
				lblFilter.Text = _filterString;
				textSize = g.MeasureString(_filterString, filterFont, (int)((rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right) * 0.7));
				lblFilter.Multiline = true;
				lblFilter.LocationF = new PointF(lineIndent * 2, top);
				lblFilter.SizeF = new SizeF(textSize.Width + 10, textSize.Height);

				top += (int)Math.Ceiling(textSize.Height) + lineSpacer;
			}

			// Draw the elemnt off to the right
			string agencyName = UserInterface.GetCompanyName();
			Image img = UserInterface.GetCompanyImage();
			img = UserInterface.SizeImageKeepAspectRatio(img, null, top - titleTop - (lineSpacer * 3));
			textSize = g.MeasureString(agencyName, new Font("Tahoma", 8, FontStyle.Italic));
			int blockWidth = (int)textSize.Width + (int)img.Width + (lineIndent * 7);
			int blockLeft = rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right - lineIndent - blockWidth;


			// Draw the black box
			XRLabel lblBlackBackground = new XRLabel();
			lblBlackBackground.BackColor = Color.Black;
			lblBlackBackground.Font = new Font("Tahoma", 8, FontStyle.Bold);
			lblBlackBackground.Text = string.Empty;
			lblBlackBackground.LocationF = new PointF(blockLeft, titleTop + (lineSpacer / 2));
			lblBlackBackground.SizeF = new SizeF(blockWidth, top - titleTop - (lineSpacer / 2));

			XRPictureBox picImage = new XRPictureBox();
			picImage.Image = img;
			picImage.Sizing = ImageSizeMode.CenterImage;
			picImage.LocationF = new PointF(lblBlackBackground.RightF - img.Width - lineIndent, lblBlackBackground.TopF);
			picImage.SizeF = new SizeF(img.Width, lblBlackBackground.HeightF);

			// Draw the white label
			XRLabel lblWhiteLabel = new XRLabel();
			lblWhiteLabel.Font = new Font("Tahoma", 8, FontStyle.Bold);
			lblWhiteLabel.TextAlignment = TextAlignment.MiddleCenter;
			lblWhiteLabel.Text = agencyName;
			lblWhiteLabel.ForeColor = Color.White;
			lblWhiteLabel.LocationF = new PointF(lblBlackBackground.LeftF, lblBlackBackground.TopF);
			lblWhiteLabel.SizeF = new SizeF(blockWidth - (lineSpacer * 3) - img.Width, lblBlackBackground.HeightF);

			// Draw two lines at the bottom of the header
			XRLine line3 = new XRLine();
			line3.LocationF = new PointF(0, top);
			line3.SizeF = new SizeF(rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right, 3);
			line3.ForeColor = Color.Black;
			line3.LineWidth = 1;

			top += lineSpacer;

			XRLine line4 = new XRLine();
			line4.LocationF = new PointF(0, top);
			line4.SizeF = new SizeF(rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right, 3);
			line4.ForeColor = Color.Black;
			line4.LineWidth = 1;

			top += lineSpacer;


			// Add all the controls
			// It renders top to back, so put the stuff on top in first
			phBand.Controls.AddRange(new XRControl[] {
						line1, 
						line2,	
						lblTitle,
						//lblDate,
						//lblFilter,
						picImage,
						lblWhiteLabel,
						lblBlackBackground,						
						line3,	
						line4,	
					});
			if (_showDatePrinted) { phBand.Controls.Add(lblDate); }
			if (_showFilter) { phBand.Controls.Add(lblFilter); }

			#endregion Page Header

			return phBand;
		}

		/// <summary>
		/// Generate the page footer for a report and pass it back
		/// </summary>
		/// <param name="rpt">The report to generate the footer for it</param>
		/// <returns>A page footer band</returns>
		private PageFooterBand GeneratePageFooter(XtraReport rpt)
		{
			#region Page Footer
			PageFooterBand pfBand = new PageFooterBand();

			XRPageInfo lblPageInfo = new XRPageInfo();
			if (_showPageNumbersInFooter)
			{
				lblPageInfo.Font = new Font("Tahoma", 8);
				lblPageInfo.TextAlignment = TextAlignment.MiddleRight;
				lblPageInfo.PageInfo = PageInfo.NumberOfTotal;
				lblPageInfo.LocationF = new PointF(rpt.PageWidth - rpt.Margins.Left - rpt.Margins.Right - 100, 20);
				lblPageInfo.SizeF = new SizeF(100, 13);
				lblPageInfo.Format = "Page {0} of {1}";
			}

			if (_showPageNumbersInFooter)
			{
				pfBand.HeightF = 33;
				pfBand.Controls.Add(lblPageInfo);
			}
			else
			{
				// Shrink it if we don't need it
				pfBand.HeightF = 5;
			}
			#endregion Page Footer

			return pfBand;
		}

		#endregion Private Methods

		#region Event Handlers
		private void rpt_PrintProgress(object sender, DevExpress.XtraPrinting.PrintProgressEventArgs e)
		{
			// Show the printing progress

		}
		#endregion Event Handlers
	}
}
