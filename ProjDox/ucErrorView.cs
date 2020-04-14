using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
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

using mgCustom;
using mgModel;

namespace ProjDox
{
	public partial class ucErrorView : ucCommonBase
	{
		private ClassGenExceptionCollection _coll = new ClassGenExceptionCollection();
		private bool _errorsOn = true;
		private bool _warningsOn = true;
		private bool _informationOn = true;

		private Image _userDef1 = null;
		private Image _userDef2 = null;
		private Image _userDef3 = null;

		private bool _continuableForm = false;
		private bool _cancelableForm = true;
		private bool _printableForm = true;
		private string _processDescription = string.Empty;

		private string _hyperlinkURL = string.Empty;
		private string _hyperlinkText = string.Empty;

		public delegate void ControlClosedEventHandler(object sender, EventArgs e);
		public event ControlClosedEventHandler ControlClosed;
		protected void OnControlClosed()
		{
			if (ControlClosed != null)
			{
				EventArgs e = new EventArgs();
				ControlClosed(this, e);
			}
		}

		public delegate void ContinueProcessingEventHandler(object sender, EventArgs e);
		public event ContinueProcessingEventHandler ContinueProcessing;
		protected void OnContinueProcessing()
		{
			if (ContinueProcessing != null)
			{
				EventArgs e = new EventArgs();
				ContinueProcessing(this, e);
			}
		}

		public delegate void CancelProcessingEventHandler(object sender, EventArgs e);
		public event CancelProcessingEventHandler CancelProcessing;
		protected void OnCancelProcessing()
		{
			if (CancelProcessing != null)
			{
				EventArgs e = new EventArgs();
				CancelProcessing(this, e);
			}
		}

		private bool _collectionContainsMoreThanOneRecordIndex = false;

		public ucErrorView(ClassGenExceptionCollection coll)
		{
			InitializeComponent();
			_coll = coll;
		}

		public ucErrorView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Tells if the form is part of a continuable process
		/// </summary>
		public bool ContinuableForm
		{
			get { return _continuableForm; }
			set 
			{ 
				_continuableForm = value;
				//if (this.DesignMode) { return; }
				SetContinuable();
			}
		}

		/// <summary>
		/// Tells if the form is part of a continuable process
		/// </summary>
		public bool CancelableForm
		{
			get { return _cancelableForm; }
			set
			{
				_cancelableForm = value;
				//if (this.DesignMode) { return; }
				SetCancelable();
			}
		}

		/// <summary>
		/// Tells if the form is part of a printable process
		/// </summary>
		public bool PrintableForm
		{
			get { return _printableForm; }
			set
			{
				_printableForm = value;
				//if (this.DesignMode) { return; }
				SetPrintable();
			}
		}

		/// <summary>
		/// The first user defined image
		/// </summary>
		public Image UserDef1Image
		{
			get { return _userDef1; }
			set { _userDef1 = value; }
		}

		/// <summary>
		/// The second user defined image
		/// </summary>
		public Image UserDef2Image
		{
			get { return _userDef2; }
			set { _userDef2 = value; }
		}

		/// <summary>
		/// The third user defined image
		/// </summary>
		public Image UserDef3Image
		{
			get { return _userDef3; }
			set { _userDef3 = value; }
		}

		/// <summary>
		/// The broken rule image
		/// </summary>
		public Image BrokenRuleImage
		{
			get { return _userDef3; }
			set { _userDef3 = value; }
		}

		/// <summary>
		/// The hyperlink URL
		/// </summary>
		public string HyperlinkURL
		{
			get { return _hyperlinkURL; }
			set 
			{
				_hyperlinkURL = value;
				linkField.EditValue = _hyperlinkURL;

				ResetHyperlinkControl();		// Reset the hyperlink Control
			}
		}

		/// <summary>
		/// The hyperlink Text
		/// </summary>
		public string HyperlinkText
		{
			get { return _hyperlinkText; }
			set 
			{ 
				_hyperlinkText = value;
				linkField.Properties.Caption = _hyperlinkText;

				ResetHyperlinkControl();		// Reset the hyperlink Control
			}
		}

		/// <summary>
		/// The hyperlink Text and URL
		/// </summary>
		public string HyperlinkTextAndURL
		{
			set 
			{ 
				_hyperlinkText = _hyperlinkURL = value;
				linkField.Properties.Caption = _hyperlinkText;
				linkField.EditValue = _hyperlinkURL;

				ResetHyperlinkControl();		// Reset the hyperlink Control
			}
		}

		/// <summary>
		/// Reset the hyperlink control visibility
		/// </summary>
		private void ResetHyperlinkControl()
		{
			if (!String.IsNullOrEmpty(_hyperlinkURL))
			{
				layoutSupportHyperlink.Visibility = LayoutVisibility.Always;
			}
			else
			{
				layoutSupportHyperlink.Visibility = LayoutVisibility.Never;
			}
		}

		/// <summary>
		/// A description for the process that was trying to run
		/// </summary>
		public string ProcessDescription
		{
			get { return _processDescription; }
			set 
			{ 
				_processDescription = value; 
			}
		}

		/// <summary>
		/// EFG Exception collection
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ClassGenExceptionCollection ClassGenExceptionCollection
		{
			get { return _coll; }
			set 
			{ 
				_coll = value;
				if (this.DesignMode) { return; }
				if (_coll.Count > 0)
				{
					ResetErrorCounts();		// Reset the error counts on the buttons
					RefreshGrid();
				}
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.P | Keys.Control))
			{
				PrintForm();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

        /// <summary>
        /// Set enabled property for print button
        /// </summary>
        public bool EnablePrintButton
        {
            set { btnPrint.Enabled = value; }
            get { return btnPrint.Enabled; }
        }

		/// <summary>
		/// Print the form
		/// </summary>
		public void PrintForm()
		{
			// Turn off the form
			layoutMain.Enabled = false;
			Application.DoEvents();


			// Reset the view
			foreach (GridColumn col in gridViewError.Columns)
			{
				if (col.FieldName.ToLower() == "ClassGenExceptionIconType".ToLower())
				{
					col.Width = 80;
					col.Caption = "Error Type";
				}
			}

			// Print the grid
			mgControls.mgDevX_GridControl_Print gridPrint = 
				new mgControls.mgDevX_GridControl_Print(gridViewError);
			gridPrint.ReportTitle = (!String.IsNullOrEmpty(_processDescription) ? _processDescription.Trim() + " - " : "") +
				"Error Listing...";
			DevExpress.XtraPrinting.Preview.PrintPreviewFormEx frm = gridPrint.ShowPreview(this.ParentForm);

			// Reset the columns
			foreach (GridColumn col in gridViewError.Columns)
			{
				if (col.FieldName.ToLower() == "ClassGenExceptionIconType".ToLower())
				{
					col.Width = 35;
					col.OptionsColumn.ShowCaption = false;
				}
			}

			layoutMain.Enabled = true;

			//// Check whether the XtraGrid control can be previewed.
			//if (!gridError.IsPrintingAvailable)
			//{
			//    MessageBox.Show("The 'DevExpress.XtraPrinting' library could not be found.", "Error");
			//    return;
			//}

			//// Turn off the form
			//layoutMain.Enabled = false;
			//Application.DoEvents();

			//gridViewError.OptionsPrint.EnableAppearanceEvenRow =
			//    gridViewError.OptionsPrint.EnableAppearanceOddRow =
			//    true;

			//PrintingSystem ps = new PrintingSystem();
			////ps.PreviewFormEx.MdiParent = mwsLEAD.common.UserInterface._mainMDIForm;

			//ps.PreviewFormEx.SaveState = false;
			//ps.PreviewFormEx.PrintBarManager.MainMenu.Visible = false;
			//ps.PreviewFormEx.SaveState = true;


			//PrintableComponentLink link = new PrintableComponentLink(ps);
			//link.Component = gridError;
			//link.CreateMarginalHeaderArea += new CreateAreaEventHandler(link_CreateMarginalHeaderArea);
			//link.CreateMarginalFooterArea += new CreateAreaEventHandler(link_CreateMarginalFooterArea);
			//link.CreateDocument();

			//// Show the columns
			//foreach (GridColumn col in gridViewError.Columns)
			//{
			//    if (col.FieldName.ToLower() == "ClassGenExceptionIconType".ToLower())
			//    {
			//        col.Width = 80;
			//        col.Caption = "Error Type";
			//    }
			//}

			//// Opens the Preview window.
			//link.ShowPreview();
			//ps.PreviewFormEx.MdiParent = mwsCommon.UserInterface.MainMDIForm;
			//mwsCommon.UserInterface.FormFillsClientArea(ps.PreviewFormEx);

			//// Reset the columns
			//foreach (GridColumn col in gridViewError.Columns)
			//{
			//    if (col.FieldName.ToLower() == "ClassGenExceptionIconType".ToLower())
			//    {
			//        col.Width = 35;
			//        col.OptionsColumn.ShowCaption = false;
			//    }
			//}

			//// Turn off the form
			//layoutMain.Enabled = true;
		}

		//private void link_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
		//{
		//    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
		//    e.Graph.Font = new Font("Tahoma", 14, FontStyle.Bold);
		//    RectangleF rec = new RectangleF(0, 10, e.Graph.ClientPageSize.Width / 2, 50);
		//    e.Graph.DrawString((!String.IsNullOrEmpty(_processDescription) ? _processDescription.Trim() + " - " : "") +
		//        "Error Listing...", Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);

		//    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Far);
		//    e.Graph.Font = new Font("Tahoma", 8, FontStyle.Bold);
		//    rec = new RectangleF(e.Graph.ClientPageSize.Width / 2, 45, e.Graph.ClientPageSize.Width / 2, 50);
		//    e.Graph.DrawString("Date Printed: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt"), Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);
		//}

		//private void link_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
		//{
		//    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Far);
		//    e.Graph.Font = new Font("Tahoma", 8);
		//    RectangleF rec = new RectangleF(0, 15, e.Graph.ClientPageSize.Width, 15);
		//    e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, "Page {0} of {1}", Color.Black, rec, DevExpress.XtraPrinting.BorderSide.None);
		//}

		/// <summary>
		/// Reset the error counts on the buttons
		/// </summary>
		private void ResetErrorCounts()
		{
			int errorCount = 0, warningCount = 0, infoCount = 0;
			if (_coll != null)
			{
				foreach (ClassGenException err in _coll)
				{
					if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Critical) { errorCount++; }
					if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Warning) { warningCount++; }
					if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Information) { infoCount++; }
				}
			}

			btnErrors.Text = "Errors (" + errorCount.ToString("###,##0") + ")";
			btnWarnings.Text = "Warnings (" + warningCount.ToString("###,##0") + ")";
			btnInformation.Text = "Information (" + infoCount.ToString("###,##0") + ")";
		}

		private void ucErrorView_Load(object sender, EventArgs e)
		{
			if (this.DesignMode) { return; }
			if (_coll == null) { return; }

			// Get the counts on the buttons
			ResetErrorCounts();

			RefreshGrid();		// Refresh the grid
		}

		private void gridViewError_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			//GridCellInfo cell = e.Cell as GridCellInfo;
			//GridView view = sender as GridView;

			// Draw on the cell
			if (e.Column.FieldName == "ClassGenExceptionIconType")
			{
				if (gridViewError.GetRowCellValue(e.RowHandle, "ClassGenExceptionIconType").ToString().ToLower() == "critical")
				{
					e.Graphics.DrawImage(ilErrors.Images[0], 
						new Point(e.Bounds.X + (e.Column.Width - ilErrors.Images[0].Width) / 2, 
						e.Bounds.Y + 1));
				}
				if (gridViewError.GetRowCellValue(e.RowHandle, "ClassGenExceptionIconType").ToString().ToLower() == "warning")
				{
					e.Graphics.DrawImage(ilErrors.Images[1],
						new Point(e.Bounds.X + (e.Column.Width - ilErrors.Images[0].Width) / 2,
						e.Bounds.Y + 1));
				}
				if (gridViewError.GetRowCellValue(e.RowHandle, "ClassGenExceptionIconType").ToString().ToLower() == "information")
				{
					e.Graphics.DrawImage(ilErrors.Images[2],
						new Point(e.Bounds.X + (e.Column.Width - ilErrors.Images[0].Width) / 2,
						e.Bounds.Y + 1));
				}
				e.Handled = true;
			}
		}

		/// <summary>
		/// Reset the column widths
		/// </summary>
		public void ResetColumnWidths()
		{
			gridViewError.Columns["ClassGenExceptionIconType"].Width = 35;
			gridViewError.Columns["RecordIndex"].Width = 35;
			gridViewError.Columns["RecordKey"].Width = 100;
		}

		private void RefreshGrid()
		{
			// Populate the grid with the errors
			// Copy the collection first
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
			if (_coll != null)
			{
				foreach (ClassGenException err in _coll)
				{
					if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Critical && _errorsOn) { errors.Add(err); }
					if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Warning && _warningsOn) { errors.Add(err); }
					if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Information && _informationOn) { errors.Add(err); }
				}
			}

			gridViewError.Columns.Clear();
			gridError.DataSource = errors;
			gridViewError.PopulateColumns();

			// Go through the collection to see if we have more than one record index
			int recIndex = -9999;
			_collectionContainsMoreThanOneRecordIndex = false;
			foreach (ClassGenException ex in errors)
			{
				if (ex.RecordIndex != recIndex)
				{
					if (recIndex == -9999)
					{
						recIndex = ex.RecordIndex;
					}
					else
					{
						_collectionContainsMoreThanOneRecordIndex = true;
						break;
					}
				}
			}

			// Take off all the columns
			//foreach (GridColumn col in gridViewError.Columns)
			//{
			//    col.VisibleIndex = -1;
			//    col.OptionsColumn.ShowInCustomizationForm = false;
			//}
			for (int colIndex = gridViewError.Columns.Count - 1; colIndex >= 0; colIndex--)
			{
				gridViewError.Columns[colIndex].VisibleIndex = -1;
				gridViewError.Columns[colIndex].OptionsColumn.ShowInCustomizationForm = false;
			}

			// Put them back in the order we want them
			int count = 0;

			gridViewError.OptionsView.RowAutoHeight = true;

			gridViewError.Columns["ClassGenExceptionIconType"].VisibleIndex = ++count;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.ShowInCustomizationForm = false;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.ShowCaption = false;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.AllowEdit = false;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.AllowMove = false;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.AllowSize = false;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsColumn.FixedWidth = true;
			gridViewError.Columns["ClassGenExceptionIconType"].OptionsFilter.AllowFilter = false;
			//gridViewError.Columns["ClassGenExceptionIconType"].Width = 35;

			if (_collectionContainsMoreThanOneRecordIndex)
			{
				gridViewError.Columns["RecordIndex"].VisibleIndex = ++count;
				gridViewError.Columns["RecordIndex"].OptionsColumn.ShowInCustomizationForm = true;
				gridViewError.Columns["RecordIndex"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
				gridViewError.Columns["RecordIndex"].Caption = "Row";
				gridViewError.Columns["RecordIndex"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
				gridViewError.Columns["RecordIndex"].OptionsColumn.AllowEdit = false;
				gridViewError.Columns["RecordIndex"].OptionsColumn.AllowMove = false;
				gridViewError.Columns["RecordIndex"].OptionsColumn.AllowSize = false;
				gridViewError.Columns["RecordIndex"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
				gridViewError.Columns["RecordIndex"].OptionsColumn.FixedWidth = true;
				gridViewError.Columns["RecordIndex"].OptionsFilter.AllowFilter = false;
				//gridViewError.Columns["RecordIndex"].Width = 35;
			}

			RepositoryItemMemoEdit memoEdit = gridError.RepositoryItems.Add("MemoEdit") as RepositoryItemMemoEdit;

			gridViewError.Columns["DescriptionWithException"].VisibleIndex = ++count;
			gridViewError.Columns["DescriptionWithException"].OptionsColumn.ShowInCustomizationForm = true;
			gridViewError.Columns["DescriptionWithException"].Caption = "Description";
			//gridViewError.Columns["DescriptionWithException"].OptionsColumn.AllowEdit = false;
			gridViewError.Columns["DescriptionWithException"].OptionsColumn.AllowEdit = true;
			gridViewError.Columns["DescriptionWithException"].OptionsColumn.AllowMove = false;
			gridViewError.Columns["DescriptionWithException"].OptionsColumn.AllowSize = false;
			gridViewError.Columns["DescriptionWithException"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
			gridViewError.Columns["DescriptionWithException"].OptionsFilter.AllowFilter = false;
			gridViewError.Columns["DescriptionWithException"].ColumnEdit = memoEdit;

			if (_collectionContainsMoreThanOneRecordIndex)
			{
				gridViewError.Columns["RecordKey"].VisibleIndex = ++count;
				gridViewError.Columns["RecordKey"].OptionsColumn.ShowInCustomizationForm = true;
				gridViewError.Columns["RecordKey"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
				gridViewError.Columns["RecordKey"].Caption = "Record Key";
				gridViewError.Columns["RecordKey"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
				gridViewError.Columns["RecordKey"].OptionsColumn.AllowEdit = false;
				gridViewError.Columns["RecordKey"].OptionsColumn.AllowMove = false;
				gridViewError.Columns["RecordKey"].OptionsColumn.AllowSize = false;
				gridViewError.Columns["RecordKey"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
				gridViewError.Columns["RecordKey"].OptionsColumn.FixedWidth = true;
				gridViewError.Columns["RecordKey"].OptionsFilter.AllowFilter = false;
				//gridViewError.Columns["RecordKey"].Width = 100;
			}
			
			gridViewError.Columns["PropertyName"].VisibleIndex = -1;
			gridViewError.Columns["PropertyName"].OptionsColumn.ShowInCustomizationForm = true;
			gridViewError.Columns["PropertyName"].Caption = "Property Name";
			gridViewError.Columns["PropertyName"].OptionsColumn.AllowEdit = false;
			gridViewError.Columns["PropertyName"].OptionsColumn.AllowMove = false;
			gridViewError.Columns["PropertyName"].OptionsColumn.AllowSize = false;
			gridViewError.Columns["PropertyName"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
			gridViewError.Columns["PropertyName"].OptionsFilter.AllowFilter = false;

			// Check the errors collection for critical exceptions
			if (errors.CriticalExceptionCount > 0 &&
				_continuableForm)
			{
				layoutContinueProcessing.Visibility = LayoutVisibility.Never;
			}
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			PrintForm();		// Print the form
			btnOK_Click(this, null);
		}

		private void btnWarnings_Click(object sender, EventArgs e)
		{
			_warningsOn = !_warningsOn;
			//btnWarnings.ButtonColor = (_warningsOn ? mgButtonGUIButtonColor.Green : mgButtonGUIButtonColor.Red);
			RefreshGrid();		// Refresh the grid
		}

		private void btnErrors_Click(object sender, EventArgs e)
		{
			_errorsOn = !_errorsOn;
			//btnErrors.ButtonColor = (_errorsOn ? mgButtonGUIButtonColor.Green : mgButtonGUIButtonColor.Red);
			RefreshGrid();		// Refresh the grid
		}

		private void btnInformation_Click(object sender, EventArgs e)
		{
			_informationOn = !_informationOn;
			//btnInformation.ButtonColor = (_informationOn ? mgButtonGUIButtonColor.Green : mgButtonGUIButtonColor.Red);
			RefreshGrid();		// Refresh the grid
		}

		/// <summary>
		/// Set if the form is continuable or not
		/// </summary>
		private void SetContinuable()
		{
			if (_continuableForm)
			{
				layoutContinueProcessing.Visibility = LayoutVisibility.Always;
				//btnCancelProcessing.ButtonColor = mgButtonGUIButtonColor.Red;
			}
			else
			{
				layoutContinueProcessing.Visibility = LayoutVisibility.Never;
				//btnCancelProcessing.ButtonColor = mgButtonGUIButtonColor.Blue;
			}
		}

		/// <summary>
		/// Set if the form is cancelable or not
		/// </summary>
		private void SetCancelable()
		{
			layoutCancelProcessing.Visibility =
				emptyRight.Visibility = 
				(_cancelableForm ? LayoutVisibility.Always : LayoutVisibility.Never);
		}

		/// <summary>
		/// Set if the form is printable or not
		/// </summary>
		private void SetPrintable()
		{
			layoutPrintErrors.Visibility =
				emptyLeft.Visibility =
				(_printableForm ? LayoutVisibility.Always : LayoutVisibility.Never);
		}

		private void btnContinueProcessing_Click(object sender, EventArgs e)
		{
			this.OnContinueProcessing();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (_continuableForm)
			{
				this.OnCancelProcessing();		// Call the cancel method
			}
			else
			{
				this.OnControlClosed();		// Just run the close method
			}
		}

		/// <summary>
		/// Dispose of the objects
		/// </summary>
		public override void DisposeObjects()
		{
			gridError.DataSource = null;
		}

		/// <summary>
		/// The print button text
		/// </summary>
		public string PrintButtonText
		{
			get { return btnPrint.Text; }
			set { btnPrint.Text = value; }
		}

		/// <summary>
		/// The cancel button text
		/// </summary>
		public string CancelButtonText
		{
			get { return btnCancelProcessing.Text; }
			set { btnCancelProcessing.Text = value; }
		}

		/// <summary>
		/// The continue button text
		/// </summary>
		public string ContinueButtonText
		{
			get { return btnContinueProcessing.Text; }
			set { btnContinueProcessing.Text = value; }
		}

		/// <summary>
		/// Gets/Sets the Print Button Color
		/// </summary>
		public mgButtonGUIButtonColor PrintButtonColor
		{
			get { return btnPrint.ButtonColor; }
			set { btnPrint.ButtonColor = value; }
		}

		/// <summary>
		/// Gets/Sets the Cancel Button Color
		/// </summary>
		public mgButtonGUIButtonColor CancelButtonColor
		{
			get { return btnCancelProcessing.ButtonColor; }
			set { btnCancelProcessing.ButtonColor = value; }
		}

		/// <summary>
		/// Gets/Sets the Continue Button Color
		/// </summary>
		public mgButtonGUIButtonColor ContinueButtonColor
		{
			get { return btnContinueProcessing.ButtonColor; }
			set { btnContinueProcessing.ButtonColor = value; }
		}

		private void linkField_OpenLink(object sender, OpenLinkEventArgs e)
		{
			// When they click on the link - fire the action
			if (_hyperlinkURL.ToLower().StartsWith("http"))
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = mgCustom.Utils.GetDefaultBrowser();
				proc.StartInfo.Arguments = _hyperlinkURL;
				proc.Start();		// Start the process
				e.Handled = true;
			}
			else
			{
				// Let's assume that this is a windows explorer window command
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				process.StartInfo.FileName = _hyperlinkURL;
				process.StartInfo.Verb = "explore";
				process.StartInfo.WindowStyle = linkField.Properties.BrowserWindowStyle;
				process.Start();
				e.Handled = true;
			}
		}
	}
}

