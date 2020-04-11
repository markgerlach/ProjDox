using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
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

using mgModel;

namespace mgControls
{
	public class mgDevX_GridControl_Print
	{
		private mgPrintOrientation _orientation = mgPrintOrientation.Portrait;
		private BaseView _view = null;
		private string _reportTitle = string.Empty;
		private bool _showDatePrinted = true;
		private bool _showPageNumbersInFooter = true;
		private string _filterString = string.Empty;
		private bool _showFilter = true;
		private System.Drawing.Printing.Margins _margins = null;

		//private bool _addFooterTotal = false;
		//private decimal _footerTotalValue = 0.00M;
		//private string _footerTotalCaption = string.Empty;
		//private System.Drawing.StringAlignment _footerTextAlignment = System.Drawing.StringAlignment.Far;

		public mgDevX_GridControl_Print(GridView view)
		{
			_view = view;
		}

		public mgDevX_GridControl_Print(CardView view)
		{
			_view = view;
		}

		public mgDevX_GridControl_Print(LayoutView view)
		{
			_view = view;
		}

		public mgDevX_GridControl_Print()
		{
		}

		/// <summary>
		/// Return the grid view that needs to be printed
		/// </summary>
		public GridView GridView
		{
			get 
			{
				if (_view is GridView) { return _view as GridView; }
				return null;
			}
			set { _view = value; }
		}

		/// <summary>
		/// Return the grid view that needs to be printed
		/// </summary>
		public CardView CardView
		{
			get
			{
				if (_view is CardView) { return _view as CardView; }
				return null;
			}
			set { _view = value; }
		}

		/// <summary>
		/// Return the grid view that needs to be printed
		/// </summary>
		public LayoutView LayoutView
		{
			get
			{
				if (_view is LayoutView) { return _view as LayoutView; }
				return null;
			}
			set { _view = value; }
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
			set { _margins = value; }
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

		///// <summary>
		///// Add the footer total
		///// </summary>
		//public bool AddFooterTotal
		//{
		//    get { return _addFooterTotal; }
		//    set { _addFooterTotal = value; }
		//}

		///// <summary>
		///// The footer total value
		///// </summary>
		//public decimal FooterTotalValue
		//{
		//    get { return _footerTotalValue; }
		//    set { _footerTotalValue = value; }
		//}

		///// <summary>
		///// The footer total caption
		///// </summary>
		//public string FooterTotalCaption
		//{
		//    get { return _footerTotalCaption; }
		//    set { _footerTotalCaption = value; }
		//}

		///// <summary>
		///// The footer alignment
		///// Default is System.Drawing.StringAlignment.Far (right)
		///// </summary>
		//public System.Drawing.StringAlignment FooterAlignment
		//{
		//    get { return _footerTextAlignment; }
		//    set { _footerTextAlignment = value; }
		//}

		/// <summary>
		/// Show the preview window based on how the control is set
		/// </summary>
		public DevExpress.XtraPrinting.Preview.PrintPreviewFormEx ShowPreview(System.Windows.Forms.Form mainForm)
		{
			// Check whether the XtraGrid control can be previewed.
			if (!_view.GridControl.IsPrintingAvailable)
			{
				MessageBox.Show("The 'DevExpress.XtraPrinting' library could not be found.", "Error");
				return null;
			}

			if (_view is GridView)
			{
				((GridView)_view).OptionsPrint.EnableAppearanceEvenRow =
					((GridView)_view).OptionsPrint.EnableAppearanceOddRow =
					true;
				((GridView)_view).OptionsPrint.UsePrintStyles = true;

				// Set the options on the footer
				((GridView)_view).OptionsPrint.PrintFooter = true;
				((GridView)_view).OptionsPrint.PrintGroupFooter = true;
				
				// Set the alignment of the cells (vertical)
				//((GridView)_view).OptionsPrint.UsePrintStyles = true;
				//foreach (GridColumn col in ((GridView)_view).Columns)
				//{
				//    col.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
				//}
				//((GridView)_view).AppearancePrint.
				//((GridView)_view).AppearancePrint.Preview.Options.UseTextOptions = true;
				//((GridView)_view).AppearancePrint.Preview.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
				//((GridView)_view).AppearancePrint.Row.Options.UseTextOptions = true;
				//((GridView)_view).AppearancePrint.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			}
			else if (_view is LayoutView)
			{
				//((LayoutView)_view).OptionsPrint.us =
			}

			PrintingSystem ps = new PrintingSystem();
			//ps.PreviewFormEx.MdiParent = mwsLEAD.common.UserInterface._mainMDIForm;

			ps.PreviewFormEx.SaveState = false;
			ps.PreviewFormEx.PrintBarManager.MainMenu.Visible = false;
			ps.PreviewFormEx.SaveState = true;
			ps.PreviewFormEx.Icon = mainForm.Icon;

			System.Drawing.Printing.Margins margins = ps.PageMargins;
			if (_margins != null)
			{				
				margins.Left = _margins.Left;
				margins.Top = _margins.Top;
				margins.Bottom = _margins.Bottom;
				margins.Right = _margins.Right;
			}
			else
			{
				margins.Left = 50;
				margins.Top = 137;
				margins.Bottom = 87;
				margins.Right = 50;
			}

			PrintableComponentLink link = new PrintableComponentLink(ps);
			link.Component = _view.GridControl;

			link.Margins = margins;
			//if (_margins != null)
			//{
			//    link.Margins = _margins;
			//}

			link.CreateMarginalHeaderArea -= new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
			link.CreateMarginalFooterArea -= new CreateAreaEventHandler(link_CreateMarginalFooterArea);

			link.CreateMarginalHeaderArea += new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
			link.CreateMarginalFooterArea += new CreateAreaEventHandler(link_CreateMarginalFooterArea);

			link.CreateDetailArea += new CreateAreaEventHandler(link_CreateDetailArea);
			link.CreateDocument();

			if (_orientation == mgPrintOrientation.Landscape) 
			{
				link.Landscape = true;
				link.PrintingSystem.PageSettings.Landscape = true; 
			}

			// Opens the Preview window.
			ps.PreviewFormEx.MdiParent = mainForm;
			link.ShowPreview();
			//mwsCommon.UserInterface.FormFillsClientArea(ps.PreviewFormEx);
			 //mwsCommon.UserInterface.SizeScreen(ps.PreviewFormEx, MWSScreenSize.FormFillsClientArea);
			return ps.PreviewFormEx;		// Return the form that was just created
		}

		private void link_CreateDetailArea(object sender, CreateAreaEventArgs e)
		{
			//MWSCreateAreaEventArgs newEvent = new MWSCreateAreaEventArgs();
			//newEvent.PageNum = e.Graph.PrintingSystem.Pages.Count;
			//newEvent.NumOfTotal = 100000;
			//this.OnDetailCreated(newEvent);
		}

		public delegate void DetailCreatedEventHandler(object sender, mgCreateAreaEventArgs e);
		public event DetailCreatedEventHandler DetailCreated;
		public void OnDetailCreated(mgCreateAreaEventArgs e)
		{
			if (DetailCreated != null)
			{
				DetailCreated(this, e);
			}
		}

		private void link_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
		{
			int top = 20, leftMargin = 1, rightMargin = (int)e.Graph.ClientPageSize.Width - 2;
			int titleTop = 0, lineSpacer = 4, lineIndent = 10;

			// For testing - draw some borders
			//e.Graph.DrawRect(new RectangleF(1, 1, e.Graph.ClientPageSize.Width - 2, e.Graph.PrintingSystem.PageMargins.Top - 16),
			//    DevExpress.XtraPrinting.BorderSide.All, Color.LightBlue, Color.Red);

			// Draw two lines at the top of the header
			e.Graph.DrawLine(new PointF(leftMargin, top), new PointF(rightMargin, top), Color.Black, 0.1F);
			top += lineSpacer;
			e.Graph.DrawLine(new PointF(leftMargin, top), new PointF(rightMargin, top), Color.Black, 0.1F);
			top += lineSpacer;

			// Draw the title
			titleTop = top;
			e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
			e.Graph.Font = new Font("Tahoma", 14, FontStyle.Bold | FontStyle.Italic);
			RectangleF rec = new RectangleF(lineIndent, titleTop, e.Graph.ClientPageSize.Width, 50);
			e.Graph.DrawString(_reportTitle, Color.Blue, rec, DevExpress.XtraPrinting.BorderSide.None);
			SizeF textSize = e.Graph.MeasureString(_reportTitle);
			top += (int)textSize.Height + (lineSpacer / 2);

			// Draw the date printed
			if (_showDatePrinted)
			{
				e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
				e.Graph.Font = new Font("Tahoma", 8, FontStyle.Italic);
				rec = new RectangleF(lineIndent * 2, top, e.Graph.ClientPageSize.Width, 50);
				e.Graph.DrawString("Date Printed: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt"), Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);
				textSize = e.Graph.MeasureString("Date Printed:");
				top += (int)textSize.Height;
			}

			// Show the filter if need be
			if (_showFilter &&
				!String.IsNullOrEmpty(_filterString))
			{
				top += lineSpacer;
				textSize = e.Graph.MeasureString(_filterString);
				e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
				e.Graph.Font = new Font("Tahoma", 8, FontStyle.Bold);
				rec = new RectangleF(lineIndent * 2, top, e.Graph.ClientPageSize.Width / 2, textSize.Height);
				e.Graph.DrawString("Filter: " + _filterString, Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);
				top += (int)textSize.Height;
			}

			// Draw the block off to the right - wait until you have the height of the square
			// Draw the black block
			e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Far);
			e.Graph.Font = new Font("Tahoma", 8, FontStyle.Bold);
			string agencyName = string.Empty; // UserInterface.GetCompanyName();
			textSize = e.Graph.MeasureString(agencyName);
			Image img = null; // UserInterface.GetCompanyImage();
			//img = UserInterface.SizeImageKeepAspectRatio(img, null, top - titleTop);
			int imgLeft = rightMargin - lineIndent - img.Width;
			int blockWidth = (int)textSize.Width + (int)img.Width + (lineIndent * 5);
			int blockLeft = rightMargin - lineIndent - blockWidth;

			RectangleF blockRect = new RectangleF(blockLeft, titleTop, blockWidth, top - titleTop);
			e.Graph.DrawRect(blockRect,
				DevExpress.XtraPrinting.BorderSide.All, Color.Black, Color.Black);

			// Draw the agency image
			e.Graph.DrawImage(img, 
				new RectangleF(imgLeft, titleTop, img.Width, top - titleTop), 
				DevExpress.XtraPrinting.BorderSide.All, Color.Black);
			
			// Draw the agency name
			e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Far);
			e.Graph.Font = new Font("Tahoma", 8, FontStyle.Bold);
			//string agencyName = UserInterface.GetCompanyName();
			textSize = e.Graph.MeasureString(agencyName);
			rec = new RectangleF(blockRect.Left, blockRect.Top + ((blockRect.Height - textSize.Height) / 2), 
				blockRect.Width - (lineSpacer * 3) - img.Width, (int)textSize.Height + 5);
			e.Graph.BackColor = Color.Black;
			e.Graph.DrawString(agencyName, Color.White, rec, DevExpress.XtraPrinting.BorderSide.All);
			e.Graph.BackColor = Color.White;

			// Draw the two lines at the bottom of the header
			top += lineSpacer;
			e.Graph.DrawLine(new PointF(leftMargin, top), new PointF(rightMargin, top), Color.Black, 0.1F);
			top += lineSpacer;
			e.Graph.DrawLine(new PointF(leftMargin, top), new PointF(rightMargin, top), Color.Black, 0.1F);
		}

		private void link_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
		{
			if (_showPageNumbersInFooter)
			{
				e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Far);
				e.Graph.Font = new Font("Tahoma", 8);
				RectangleF rec = new RectangleF(0, 15, e.Graph.ClientPageSize.Width, 15);
				e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, "Page {0} of {1}", Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);
			}
		}
	}

	public class mgCreateAreaEventArgs : EventArgs
	{
		private int _pageNum;
		private int _numOfTotal;

		public mgCreateAreaEventArgs() 
		{
		}

		public int PageNum
		{
			get { return _pageNum; }
			set { _pageNum = value; }
		}

		public int NumOfTotal
		{
			get { return _numOfTotal; }
			set { _numOfTotal = value; }
		}
	}
}
