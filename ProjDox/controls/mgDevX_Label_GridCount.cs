using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;

namespace mgControls
{
	public class mgDevX_Label_GridCount : DevExpress.XtraEditors.LabelControl
	{
		private mgDevX_GridControl _gridControl = null;
		//private DevExpress.XtraGrid.Views.Grid.GridView _gridView = null;
		private string _defaultText = string.Empty;
		private List<GridView> _gridViews = new List<GridView>();
		private List<CardView> _cardViews = new List<CardView>();
		private long _dbCount = -1;		// Find out what the DB Count is and include it in the text

		private string _dbCountText = "Rows In Database";
		private string _loadedCountText = "Rows Loaded";
		private string _visibleCountText = "Visible";
		private string _checkedCountText = "Selected";

		private DevExpress.XtraEditors.BaseEdit _chkEditor = null;
		private GridView _chkGridView = null;
		private CardView _chkCardView = null;

		public mgDevX_Label_GridCount()
			: base()
		{
			// When the control is created, set the properties accordingly
			if (this.DesignMode)
			{
				this.Text = "< Design Mode - No Count >";
			}
			else
			{
				this.Text = string.Empty;
			}
			this.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		}

		public override string Text
		{
			get
			{
				// Find out if we're down a view and need to add them
				if (_gridControl != null &&
					_gridViews.Count == 0 &&
					_gridControl.Views.Count > _gridViews.Count)
				{
					foreach (BaseView view in _gridControl.Views)
					{
						if (view is GridView)
						{
							((GridView)view).ColumnFilterChanged -= new EventHandler(view_ColumnFilterChanged);
							((GridView)view).DataSourceChanged -= new EventHandler(view_DataSourceChanged);

							((GridView)view).ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
							((GridView)view).DataSourceChanged += new EventHandler(view_DataSourceChanged);
						}
						else if (view is CardView)
						{
							((CardView)view).ColumnFilterChanged -= new EventHandler(view_ColumnFilterChanged);
							((CardView)view).DataSourceChanged -= new EventHandler(view_DataSourceChanged);

							((CardView)view).ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
							((CardView)view).DataSourceChanged += new EventHandler(view_DataSourceChanged);
						}
					}
				}

				return base.Text;
			}
			set
			{
				if (!this.DesignMode &&
					value.ToLower().Contains(" no count"))
				{
					value = string.Empty;
				}
				base.Text = value;
			}
		}

		/// <summary>
		/// Gives the Database Count Text for the grid
		/// </summary>
		public string DBCountText
		{
			get { return _dbCountText; }
			set { _dbCountText = value; }
		}

		/// <summary>
		/// Gives the Loaded Count Text for the grid
		/// </summary>
		public string LoadedCountText
		{
			get { return _loadedCountText; }
			set { _loadedCountText = value; }
		}

		/// <summary>
		/// Gives the Visible Count Text for the grid
		/// </summary>
		public string VisibleCountText
		{
			get { return _visibleCountText; }
			set { _visibleCountText = value; }
		}

		/// <summary>
		/// Gives the Checked Count Text for the grid
		/// Not visible if the checked column isn't visible in the grid
		/// </summary>
		public string CheckedCountText
		{
			get { return _checkedCountText; }
			set { _checkedCountText = value; }
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
					this.Text = "< Design Mode - No Count >";
				}
				else
				{
					this.Text = string.Empty;
				}

				// When they set the control, if the control isn't null then wire up the events to that grid
				if (_gridControl != null)
				{
					foreach (BaseView view in _gridControl.Views)
					{
						if (view is GridView)
						{
							((GridView)view).ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
							((GridView)view).DataSourceChanged += new EventHandler(view_DataSourceChanged);

							((GridView)view).ShownEditor += new EventHandler(view_ShownEditor);
							((GridView)view).HiddenEditor += new EventHandler(view_HiddenEditor);

							_gridViews.Add((GridView)view);
						}
						else if (view is CardView)
						{
							((CardView)view).ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
							((CardView)view).DataSourceChanged += new EventHandler(view_DataSourceChanged);

							((CardView)view).ShownEditor += new EventHandler(view_ShownEditor);
							((CardView)view).HiddenEditor += new EventHandler(view_HiddenEditor);

							_cardViews.Add((CardView)view);
						}
					}
				}
			}
		}

		private void view_HiddenEditor(object sender, EventArgs e)
		{
			if (_chkEditor != null)
			{
				_chkEditor.EditValueChanged -= new EventHandler(ColumnEdit_EditValueChanged);
				_chkEditor = null;
				_chkGridView = null;
				_chkCardView = null;
			}
		}

		private void view_ShownEditor(object sender, EventArgs e)
		{
			if (sender is GridView)
			{
				if (((GridView)sender).FocusedColumn.FieldName == "CheckedInGrid")
				{
					_chkGridView = ((GridView)sender);
					_chkCardView = null;
					_chkEditor = ((GridView)sender).ActiveEditor;
					_chkEditor.EditValueChanged += new EventHandler(ColumnEdit_EditValueChanged);
				}
			}
			else if (sender is CardView)
			{
				if (((CardView)sender).FocusedColumn.FieldName == "CheckedInGrid")
				{
					_chkGridView = null;
					_chkCardView = ((CardView)sender);
					_chkEditor = ((CardView)sender).ActiveEditor;
					_chkEditor.EditValueChanged += new EventHandler(ColumnEdit_EditValueChanged);
				}
			}
		}

		private void ColumnEdit_EditValueChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			if (_chkGridView != null)
			{
				_chkGridView.PostEditor();
				ResetFilterText(_chkGridView);
			}
			else if (_chkCardView != null)
			{
				_chkCardView.PostEditor();
				ResetFilterText(_chkCardView);
			}
		}

		private void view_DataSourceChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			if (sender is GridView)
			{
				ResetFilterText((GridView)sender);

				((GridView)sender).ShownEditor -= new EventHandler(view_ShownEditor);
				((GridView)sender).HiddenEditor -= new EventHandler(view_HiddenEditor);

				((GridView)sender).ShownEditor += new EventHandler(view_ShownEditor);
				((GridView)sender).HiddenEditor += new EventHandler(view_HiddenEditor);
			}
			else if (sender is CardView)
			{
				ResetFilterText((CardView)sender);

				((CardView)sender).ShownEditor -= new EventHandler(view_ShownEditor);
				((CardView)sender).HiddenEditor -= new EventHandler(view_HiddenEditor);

				((CardView)sender).ShownEditor += new EventHandler(view_ShownEditor);
				((CardView)sender).HiddenEditor += new EventHandler(view_HiddenEditor);
			}
		}

		private void view_ColumnFilterChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			if (sender is GridView)
			{
				ResetFilterText((GridView)sender);
			}
			else if (sender is CardView)
			{
				ResetFilterText((CardView)sender);
			}
		}

		/// <summary>
		/// Reset the filter text in the label
		/// </summary>
		public void ResetFilterText(BaseView view)
		{
			if (this.DesignMode)
			{
				this.Text = "< Design Mode - No Count >";
				return;
			}

			StringBuilder itemText = new StringBuilder();
			try
			{
				int visibleRows = 0, totalRows = 0;
				bool checkPresent = false;
				int checkedRows = -1;
				if (view is GridView)
				{
					GridView vw = (GridView)view;
					// When the filter control changes, change the text at the bottom
					visibleRows = vw.DataRowCount;
					totalRows = (vw.DataSource != null ? (vw.DataSource as IList).Count : 0); //((System.Data.DataView)view.DataSource).Count;
					checkPresent = (vw.VisibleColumns.Count > 0 &&
						vw.VisibleColumns[0].ColumnType == typeof(System.Boolean)); // _dt.Columns.Contains("check");
					checkedRows = -1;
					if (checkPresent)
					{
						// Reset the null data if need be
						int count = 0;
						for (int i = 0; i < view.RowCount; i++)
						{
							if (vw.VisibleColumns.Count > 0 &&
								vw.GetRowCellValue(i, vw.VisibleColumns[0]) != null &&
								vw.VisibleColumns[0].ColumnType.ToString().Contains("System.Boolean") &&
								!String.IsNullOrEmpty(vw.GetRowCellValue(i, vw.VisibleColumns[0]).ToString()) &&
								bool.Parse(vw.GetRowCellValue(i, vw.VisibleColumns[0]).ToString()))
							{
								count++;
							}
						}
						checkedRows = count;
					}
				}
				else if (view is CardView)
				{
					CardView vw = (CardView)view;
					// When the filter control changes, change the text at the bottom
					visibleRows = vw.DataRowCount;
					totalRows = (vw.DataSource != null ? (vw.DataSource as IList).Count : 0); //((System.Data.DataView)view.DataSource).Count;
					checkPresent = (vw.VisibleColumns.Count > 0 &&
						vw.VisibleColumns[0].ColumnType == typeof(System.Boolean)); // _dt.Columns.Contains("check");
					checkedRows = -1;
					if (checkPresent)
					{
						// Reset the null data if need be
						int count = 0;
						for (int i = 0; i < view.RowCount; i++)
						{
							if (vw.VisibleColumns.Count > 0 &&
								vw.GetRowCellValue(i, vw.VisibleColumns[0]) != null &&
								vw.VisibleColumns[0].ColumnType.ToString().Contains("System.Boolean") &&
								!String.IsNullOrEmpty(vw.GetRowCellValue(i, vw.VisibleColumns[0]).ToString()) &&
								bool.Parse(vw.GetRowCellValue(i, vw.VisibleColumns[0]).ToString()))
							{
								count++;
							}
						}
						checkedRows = count;
					}
				}
				else if (view is LayoutView)
				{
					LayoutView vw = (LayoutView)view;
					// When the filter control changes, change the text at the bottom
					visibleRows = vw.DataRowCount;
					totalRows = (vw.DataSource != null ? (vw.DataSource as IList).Count : 0); //((System.Data.DataView)view.DataSource).Count;
					checkPresent = (vw.VisibleColumns.Count > 0 &&
						vw.VisibleColumns[0].ColumnType == typeof(System.Boolean)); // _dt.Columns.Contains("check");
					checkedRows = -1;
					if (checkPresent)
					{
						// Reset the null data if need be
						int count = 0;
						for (int i = 0; i < view.RowCount; i++)
						{
							if (vw.VisibleColumns.Count > 0 &&
								vw.GetRowCellValue(i, vw.VisibleColumns[0]) != null &&
								vw.VisibleColumns[0].ColumnType.ToString().Contains("System.Boolean") &&
								!String.IsNullOrEmpty(vw.GetRowCellValue(i, vw.VisibleColumns[0]).ToString()) &&
								bool.Parse(vw.GetRowCellValue(i, vw.VisibleColumns[0]).ToString()))
							{
								count++;
							}
						}
						checkedRows = count;
					}
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
			this.Text = itemText.ToString();
		}
	}
}
