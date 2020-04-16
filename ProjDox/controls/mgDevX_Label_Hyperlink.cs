using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Controls;

using mgCustom;

namespace mgControls
{
	public partial class mgDevX_Label_Hyperlink : UserControl //, IXtraResizableControl
	{
		private mgDevX_GridControl _gridControl = null;
		private DevExpress.XtraEditors.Controls.ImageSlider _imgSlider = null;
		private string _defaultText = string.Empty;
		private List<GridView> _gridViews = new List<GridView>();
		//private long _dbCount = -1;		// Find out what the DB Count is and include it in the text

		private string _dbCountText = "Rows In Database";
		private string _loadedCountText = "Rows Loaded";
		private string _visibleCountText = "Visible";
		private string _checkedCountText = "Selected";
		private bool _boldFont = false;

		private DevExpress.XtraEditors.BaseEdit _chkEditor = null;
		private GridView _chkView = null;

		private DateTime _last1Clicked = DateTime.Now;
		private DateTime _last2Clicked = DateTime.Now;
		private DateTime _last3Clicked = DateTime.Now;
		//private bool _enabled = true;

		private bool _hyperlink1Visible = true;
		private bool _hyperlink2Visible = true;
		private bool _hyperlink3Visible = true;

		public mgDevX_Label_Hyperlink()
		{
			InitializeComponent();

			// When the control is created, set the properties accordingly
			lblCount.Text = "< Design Mode - No Count >";
			lblHyperlink1.Text = "Check/Uncheck Visible";
			lblHyperlink2.Text = "Check/Uncheck All";
			lblHyperlink3.Text = string.Empty;

			PlaceElements();		// Place the elements on the form
		}

		/// <summary>
		/// Sets the text in the 1st link
		/// </summary>
		public string Hyperlink1Text
		{
			get { return lblHyperlink1.Text; }
			set 
			{ 
				lblHyperlink1.Text = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Sets the text in the 2nd link
		/// </summary>
		public string Hyperlink2Text
		{
			get { return lblHyperlink2.Text; }
			set 
			{ 
				lblHyperlink2.Text = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Sets the text in the 3rd link
		/// </summary>
		public string Hyperlink3Text
		{
			get { return lblHyperlink3.Text; }
			set
			{
				lblHyperlink3.Text = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Tells if the 1st hyperlink is visible
		/// </summary>
		public bool Hyperlink1Visible
		{
			get { return _hyperlink1Visible; }
			set
			{
				lblHyperlink1.Visible = _hyperlink1Visible = value;
			}
		}

		/// <summary>
		/// Tells if the 2nd hyperlink is visible
		/// </summary>
		public bool Hyperlink2Visible
		{
			get { return _hyperlink2Visible; }
			set
			{
				lblHyperlink2.Visible = _hyperlink2Visible = value;
			}
		}

		/// <summary>
		/// Tells if the 3rd hyperlink is visible
		/// </summary>
		public bool Hyperlink3Visible
		{
			get { return _hyperlink3Visible; }
			set
			{
				lblHyperlink3.Visible = _hyperlink3Visible = value;
			}
		}

		/// <summary>
		/// Sets the count text
		/// </summary>
		public string CountText
		{
			get 
			{
				return lblCount.Text; 
			}
			set
			{
				if (!this.DesignMode &&
					value.ToLower().Contains(" no count"))
				{
					value = string.Empty;
				}

				lblCount.Text = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Sets the text
		/// </summary>
		public override string Text
		{
			get
			{
				return lblCount.Text;
			}
			set
			{
				lblCount.Text = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Sets if the font is bold or not
		/// </summary>
		public bool BoldFont
		{
			get { return _boldFont; }
			set 
			{ 
				_boldFont = value; 

				// Set the font on the control
				if (_boldFont)
				{
					lblCount.Appearance.Font = new Font(lblCount.Appearance.Font.Name, lblCount.Appearance.Font.Size, FontStyle.Bold);
				}
				else
				{
					lblCount.Appearance.Font = new Font(lblCount.Appearance.Font.Name, lblCount.Appearance.Font.Size);
				}
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Set the super tip for the first control
		/// </summary>
		public DevExpress.Utils.SuperToolTip Hyperlink1SuperTip
		{
			get { return ttController.GetSuperTip(lblHyperlink1); }
			set 
			{ 
				ttController.SetSuperTip(lblHyperlink1, value);
				PlaceElements();		// Place the elements on the form
			}
		}

		public DevExpress.Utils.SuperToolTip Hyperlink2SuperTip
		{
			get { return ttController.GetSuperTip(lblHyperlink2); }
			set 
			{ 
				ttController.SetSuperTip(lblHyperlink2, value);
				PlaceElements();		// Place the elements on the form
			}
		}

		public DevExpress.Utils.SuperToolTip Hyperlink3SuperTip
		{
			get { return ttController.GetSuperTip(lblHyperlink3); }
			set 
			{
				ttController.SetSuperTip(lblHyperlink3, value);
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Gives the Database Count Text for the grid
		/// </summary>
		public string DBCountText
		{
			get { return _dbCountText; }
			set
			{
				_dbCountText = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Gives the Loaded Count Text for the grid
		/// </summary>
		public string LoadedCountText
		{
			get { return _loadedCountText; }
			set 
			{ 
				_loadedCountText = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Gives the Visible Count Text for the grid
		/// </summary>
		public string VisibleCountText
		{
			get { return _visibleCountText; }
			set 
			{ 
				_visibleCountText = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Gives the Checked Count Text for the grid
		/// Not visible if the checked column isn't visible in the grid
		/// </summary>
		public string CheckedCountText
		{
			get { return _checkedCountText; }
			set
			{
				_checkedCountText = value;
				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// Place the elements on the form
		/// </summary>
		public void PlaceElements()
		{
			//Application.DoEvents();
			int spacer = 12;
			int top = 2;
			int left = 2;
			
			lblCount.Location = new Point(left, top);
			left += lblCount.Width + spacer;
			if (!String.IsNullOrEmpty(lblHyperlink1.Text))
			{
				lblHyperlink1.Visible = true;
				lblHyperlink1.Location = new Point(left, top);
				left += lblHyperlink1.Width + spacer;
			}
			else
			{
				lblHyperlink1.Visible = false;
			}
			if (!_hyperlink1Visible) { lblHyperlink1.Visible = false; }

			if (!String.IsNullOrEmpty(lblHyperlink2.Text))
			{
				lblHyperlink2.Visible = true;
				lblHyperlink2.Location = new Point(left, top);
				left += lblHyperlink2.Width + spacer;
			}
			else
			{
				lblHyperlink2.Visible = false;
			}
			if (!_hyperlink2Visible) { lblHyperlink2.Visible = false; }

			if (!String.IsNullOrEmpty(lblHyperlink3.Text))
			{
				lblHyperlink3.Visible = true;
				lblHyperlink3.Location = new Point(left, top);
				left += lblHyperlink3.Width + spacer;
			}
			else
			{
				lblHyperlink3.Visible = false;
			}
			if (!_hyperlink3Visible) { lblHyperlink3.Visible = false; }

			// Set up the events for any views that haven't been set
			if (_gridControl != null)
			{
				foreach (GridView view in _gridControl.Views)
				{
					if (!_gridViews.Contains(view))
					{
						view.ColumnFilterChanged -= new EventHandler(view_ColumnFilterChanged);
						view.DataSourceChanged -= new EventHandler(view_DataSourceChanged);

						view.ShownEditor -= new EventHandler(view_ShownEditor);
						view.HiddenEditor -= new EventHandler(view_HiddenEditor);

						view.ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
						view.DataSourceChanged += new EventHandler(view_DataSourceChanged);

						view.ShownEditor += new EventHandler(view_ShownEditor);
						view.HiddenEditor += new EventHandler(view_HiddenEditor);

						_gridViews.Add(view);
					}
				}
			}
		}

		/// <summary>
		/// The grid control to wire this control to
		/// </summary>
		public mgDevX_GridControl GridControl
		{
			get { return _gridControl; }
			set
			{
				_gridControl = value;

				if (this.DesignMode)
				{
					lblCount.Text = "< Design Mode - No Count >";
				}
				else
				{
					if (value == null)
					{
						// Do nothing
					}
					else
					{
						lblCount.Text = string.Empty;
					}
				}

				// When they set the control, if the control isn't null then wire up the events to that grid
				if (_gridControl != null)
				{
					// Set up the checked changed event
					_gridControl.GridCheckClicked -= new mgDevX_GridControl.GridCheckClickedEventHandler(_gridControl_GridCheckClicked);
					_gridControl.GridCheckClicked += new mgDevX_GridControl.GridCheckClickedEventHandler(_gridControl_GridCheckClicked);
				}

				PlaceElements();		// Place the elements on the form
			}
		}

		/// <summary>
		/// The image slider
		/// </summary>
		public DevExpress.XtraEditors.Controls.ImageSlider ImageSlider
		{
			get { return _imgSlider; }
			set
			{
				_imgSlider = value;

				if (this.DesignMode)
				{
					lblCount.Text = "< Design Mode - No Count >";
				}
				else
				{
					if (value == null)
					{
						// Do nothing
					}
					else
					{
						lblCount.Text = string.Empty;
					}
				}

				// When they set the control, if the control isn't null then wire up the events to that grid
				//if (_imgSlider != null)
				//{
				//    // Set up the checked changed event
				//    _imgSlider.
				//    _gridControl.GridCheckClicked -= new ssDevX_GridControl.GridCheckClickedEventHandler(_gridControl_GridCheckClicked);
				//    _gridControl.GridCheckClicked += new ssDevX_GridControl.GridCheckClickedEventHandler(_gridControl_GridCheckClicked);
				//}

				PlaceElements();		// Place the elements on the form
			}
		}

		private void _gridControl_GridCheckClicked(object sender, GridCheckClicked_EventArgs e)
		{
			ResetFilterText((GridView)e.GridView.GridControl.MainView);
		}

		private void view_HiddenEditor(object sender, EventArgs e)
		{
			if (_chkEditor != null)
			{
				_chkEditor.EditValueChanged -= new EventHandler(ColumnEdit_EditValueChanged);
				_chkEditor = null;
				_chkView = null;
			}
		}

		private void view_ShownEditor(object sender, EventArgs e)
		{
			if (((GridView)sender).FocusedColumn.FieldName == "CheckedInGrid")
			{
				_chkView = ((GridView)sender);
				_chkEditor = ((GridView)sender).ActiveEditor;
				_chkEditor.EditValueChanged += new EventHandler(ColumnEdit_EditValueChanged);
			}
		}

		private void ColumnEdit_EditValueChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			_chkView.PostEditor();
			ResetFilterText(_chkView);
		}

		private void view_DataSourceChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			ResetFilterText((GridView)sender);

			//((GridView)sender).ShownEditor -= new EventHandler(view_ShownEditor);
			//((GridView)sender).HiddenEditor -= new EventHandler(view_HiddenEditor);

			//((GridView)sender).ShownEditor += new EventHandler(view_ShownEditor);
			//((GridView)sender).HiddenEditor += new EventHandler(view_HiddenEditor);
		}

		private void view_ColumnFilterChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			ResetFilterText((GridView)sender);
		}

		/// <summary>
		/// Reset the filter text in the label
		/// </summary>
		public void ResetFilterText(GridView view)
		{
			if (this.GridControl == null)
			{
				throw new Exception("There is no grid control associated to this count control.");
			}

			//if (view == null) { return; }
			if (this.DesignMode)
			{
				lblCount.Text = "< Design Mode - No Count >";
				PlaceElements();		// Place the elements on the form
				return;
			}

			StringBuilder itemText = new StringBuilder();
			try
			{
				// When the filter control changes, change the text at the bottom
				int visibleRows = view.DataRowCount;
				int totalRows = (view.DataSource != null ? (view.DataSource as IList).Count : 0); //((System.Data.DataView)view.DataSource).Count;
				bool checkPresent = (view.VisibleColumns.Count > 0 &&
					view.VisibleColumns[0].ColumnType == typeof(System.Boolean)); // _dt.Columns.Contains("check");
				int checkedRows = -1;
				if (checkPresent)
				{
					// Reset the null data if need be
					int count = 0;

					if (this.GridControl != null &&
						this.GridControl.DataSource != null &&
						this.GridControl.DataSource is IBindingList)
					{
						IBindingList list = (IBindingList)this.GridControl.DataSource;
						if (list.Count > 0 &&
							list[0] is mgModel.mgModel_BaseObject)
						{
							foreach (mgModel.mgModel_BaseObject obj in list)
							{
								if (obj.CheckedInGrid) { count++; }
							}
						}
					}
					else if (this.GridControl != null &&
						this.GridControl.DataSource != null &&
						this.GridControl.DataSource is DataTable)
					{
						DataTable list = (DataTable)this.GridControl.DataSource;
						if (list.Rows.Count > 0)
						{
							foreach (DataRow row in list.Rows)
							{
								if (list.Columns.Contains("CheckedInGrid") &&
									(bool)row["CheckedInGrid"]) 
								{ count++; }
							}
						}
					}

					checkedRows = count;
				}

				if (view.GridControl is mgDevX_GridControl &&
					((mgDevX_GridControl)view.GridControl).DBCount > -1)
				{
					_loadedCountText = _loadedCountText.Replace("Rows ", "");
					itemText.Append(((mgDevX_GridControl)view.GridControl).DBCount.ToString("###,###,##0") +
						" " + _dbCountText + ", ");
				}
				itemText.Append(totalRows.ToString("###,###,##0") + " " + _loadedCountText + ", " +
					visibleRows.ToString("###,###,##0") + " " + _visibleCountText + "" +
					(checkPresent ? ", " + checkedRows.ToString("###,###,##0") + " " + _checkedCountText + "" : ""));
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message + "\r\n" +
					ex.StackTrace, "Error",
					System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Error);
			}
			lblCount.Text = itemText.ToString();
			PlaceElements();		// Place the elements on the form
		}

		/// <summary>
		/// Reset the filter text
		/// </summary>
		public void ResetFilterTextForImageSlider()
		{
			if (_imgSlider == null)
			{
				throw new Exception("There is no image slider control associated to this count control.");
			}

			if (this.DesignMode)
			{
				lblCount.Text = "< Design Mode - No Count >";
				PlaceElements();		// Place the elements on the form
				return;
			}

			StringBuilder itemText = new StringBuilder();
			try
			{
				// Get the count of images in the box
				if (_imgSlider.Images.Count == 0)
				{
					itemText.Append("<No Images Present...>");
				}
				else
				{
					itemText.Append("Image: " + (_imgSlider.Images.IndexOf(_imgSlider.CurrentImage) + 1) + 
						" of " + _imgSlider.Images.Count.ToString());
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message + "\r\n" +
					ex.StackTrace, "Error",
					System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Error);
			}
			lblCount.Text = itemText.ToString();
			PlaceElements();		// Place the elements on the form
		}

		#region Events
		public delegate void Hyperlink1ClickedEventHandler(object sender, EventArgs e);
		public event Hyperlink1ClickedEventHandler Hyperlink1Clicked;
		protected void OnHyperlink1Clicked()
		{
			if (Hyperlink1Clicked != null)
			{
				EventArgs e = new EventArgs();
				Hyperlink1Clicked(this, e);
			}
		}

		public delegate void Hyperlink2ClickedEventHandler(object sender, EventArgs e);
		public event Hyperlink2ClickedEventHandler Hyperlink2Clicked;
		protected void OnHyperlink2Clicked()
		{
			if (Hyperlink2Clicked != null)
			{
				EventArgs e = new EventArgs();
				Hyperlink2Clicked(this, e);
			}
		}

		public delegate void Hyperlink3ClickedEventHandler(object sender, EventArgs e);
		public event Hyperlink3ClickedEventHandler Hyperlink3Clicked;
		protected void OnHyperlink3Clicked()
		{
			if (Hyperlink3Clicked != null)
			{
				EventArgs e = new EventArgs();
				Hyperlink3Clicked(this, e);
			}
		}
		#endregion Events

		private void lblHyperlink1_Click(object sender, EventArgs e)
		{
			if (((TimeSpan)DateTime.Now.Subtract(_last1Clicked)).TotalMilliseconds >
				SystemInformation.DoubleClickTime)
			{
				_last1Clicked = DateTime.Now;
				this.OnHyperlink1Clicked();
			}
		}

		private void lblHyperlink2_Click(object sender, EventArgs e)
		{
			if (((TimeSpan)DateTime.Now.Subtract(_last2Clicked)).TotalMilliseconds >
				SystemInformation.DoubleClickTime)
			{
				_last2Clicked = DateTime.Now;
				this.OnHyperlink2Clicked();
			}
		}

		private void lblHyperlink3_Click(object sender, EventArgs e)
		{
			if (((TimeSpan)DateTime.Now.Subtract(_last3Clicked)).TotalMilliseconds >
				SystemInformation.DoubleClickTime)
			{
				_last3Clicked = DateTime.Now;
				this.OnHyperlink3Clicked();
			}
		}

		#region IXtraResizableControl Members

		public event EventHandler SizeChanged;

		public bool IsCaptionVisible
		{
			get { return false; }
		}

		public Size MaxSize
		{
			get { return new Size(0, 17); }
		}

		public Size MinSize
		{
			get { return new Size(0, 17); }
		}

		#endregion

		private void mgDevX_Label_GridCount_Hyperlink_EnabledChanged(object sender, EventArgs e)
		{
			// Figure out what to do with the link columns
			if (this.Enabled)
			{
				// Color the controls
				this.lblHyperlink3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
				this.lblHyperlink3.Appearance.ForeColor = System.Drawing.Color.Blue;
				this.lblHyperlink3.Appearance.Options.UseFont = true;
				this.lblHyperlink3.Appearance.Options.UseForeColor = true;
				this.lblHyperlink3.Cursor = System.Windows.Forms.Cursors.Hand;

				this.lblHyperlink2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
				this.lblHyperlink2.Appearance.ForeColor = System.Drawing.Color.Blue;
				this.lblHyperlink2.Appearance.Options.UseFont = true;
				this.lblHyperlink2.Appearance.Options.UseForeColor = true;
				this.lblHyperlink2.Cursor = System.Windows.Forms.Cursors.Hand;

				this.lblHyperlink1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
				this.lblHyperlink1.Appearance.ForeColor = System.Drawing.Color.Blue;
				this.lblHyperlink1.Appearance.Options.UseFont = true;
				this.lblHyperlink1.Appearance.Options.UseForeColor = true;
				this.lblHyperlink1.Cursor = System.Windows.Forms.Cursors.Hand;
			}
			else
			{
				// Discolor them
				this.lblHyperlink3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F);
				this.lblHyperlink3.Appearance.ForeColor = System.Drawing.SystemColors.ControlDark;
				this.lblHyperlink3.Appearance.Options.UseFont = true;
				this.lblHyperlink3.Appearance.Options.UseForeColor = true;
				this.lblHyperlink3.Cursor = System.Windows.Forms.Cursors.Default;

				this.lblHyperlink2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F);
				this.lblHyperlink2.Appearance.ForeColor = System.Drawing.SystemColors.ControlDark;
				this.lblHyperlink2.Appearance.Options.UseFont = true;
				this.lblHyperlink2.Appearance.Options.UseForeColor = true;
				this.lblHyperlink2.Cursor = System.Windows.Forms.Cursors.Default;

				this.lblHyperlink1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F);
				this.lblHyperlink1.Appearance.ForeColor = System.Drawing.SystemColors.ControlDark;
				this.lblHyperlink1.Appearance.Options.UseFont = true;
				this.lblHyperlink1.Appearance.Options.UseForeColor = true;
				this.lblHyperlink1.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}

		private void ttController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
		{
			DevExpress.Utils.SuperToolTip superTip = ((DevExpress.Utils.ToolTipController)sender).GetSuperTip(e.SelectedControl);
			if (superTip != null)
			{
				DevExpress.Utils.ToolTipControlInfo info = new DevExpress.Utils.ToolTipControlInfo();
				info.Object = e.SelectedControl;
				info.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
				info.SuperTip = superTip;
				info.ImmediateToolTip = true;
				info.Text = "test";
				info.Title = "test";
				e.Info = info;
			}
		}

		private void ttController_CustomDraw(object sender, DevExpress.Utils.ToolTipControllerCustomDrawEventArgs e)
		{
		}

		private void ttController_BeforeShow(object sender, DevExpress.Utils.ToolTipControllerShowEventArgs e)
		{
		}
	}
}

