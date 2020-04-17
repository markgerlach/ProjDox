using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.Utils.Drawing;

using mgCustom;
using mgModel;

namespace mgControls
{
	public class mgDevX_GridControl : DevExpress.XtraGrid.GridControl //, IXtraResizableControl
	{
		[Serializable]
		public struct RowInfo
		{
			public object Id;
			public int level;
		};

		private bool _showDetailButtons = false;
		private bool _allowGrouping = false;
		private string _layoutCaptionField = string.Empty;
		private long _dbCount = -1;
		private List<string> _setCaptions = new List<string>();
		private bool _isUpdating = false;
		private bool _repositionLinkColumnsAtEnd = true;
		private List<string> _visibleColumns = new List<string>();
		private string _keyFieldName = string.Empty;
		
		private bool? _oldValInCheckColumn = null;
		private bool _checkProcessingRunning = false;
		private DevExpress.XtraEditors.BaseEdit _chkEditor = null;
		private GridView _chkView = null;
		private bool _checkUncheckByClickingColumnHeader = true;

		private ToolTipController _toolTipController = new ToolTipController();
		//private bool _hintShowing = false;
		private Size? _customizationFormSize = null;
		private string _defaultGroupFormat = "{0}: [#image]{1} {2}";
		private List<string> _disabledLinkColumns = new List<string>();

		private Dictionary<GridLinkType, Image> _imageLibrary = new Dictionary<GridLinkType,Image>();

		private mgDevX_GridControl_LinkCollection _linkCollection = new mgDevX_GridControl_LinkCollection();
		private mgDevX_GridControl_ButtonCollection _buttonCollection = new mgDevX_GridControl_ButtonCollection();

		private List<string> _customDrawGridViewOverrides = new List<string>();

		//private ImageCollection _imageCollection = new ImageCollection();

		//private Dictionary<int, GridLinkType> _linkTypes = new Dictionary<int, GridLinkType>();
		//private Dictionary<int, Image> _linkImages = new Dictionary<int, Image>();
		//private Dictionary<int, string> _linkFieldNames = new Dictionary<int, string>();

		#region Remember Layout Properties
		// Layout remembered
		private bool _layoutRemembered = false;

		private Dictionary<GridView, string> _layoutStreams = new Dictionary<GridView, string>();

		//// Store the name and visible index of any visible columns
		//private Dictionary<GridView, ssModel.SearchFavoriteColumnCollection> _layoutVisibleColumns =
		//    new Dictionary<GridView, ssModel.SearchFavoriteColumnCollection>();

		//// Listing of grouped columns and their group index
		//private Dictionary<GridView, Dictionary<string, int>> _layoutGroupedColumns = new Dictionary<GridView, Dictionary<string, int>>();

		//// Store a listing of filter values
		//private Dictionary<GridView, Dictionary<string, string>> _layoutFilterValues = 
		//    new Dictionary<GridView, Dictionary<string, string>>();

		//// Store a listing of expanded group rows
		//private Dictionary<GridView, List<RowInfo>> _layoutExpandedGroupValues =
		//    new Dictionary<GridView, List<RowInfo>>();

		//// The layout visible row index of the current row
		//private int _layoutVisibleRowIndex = -1;
		#endregion Remember Layout Properties


		public mgDevX_GridControl()
			: base()
		{
			// When the control is created, set the properties accordingly
			this.ToolTipController = _toolTipController;
			_toolTipController.GetActiveObjectInfo += new 
				ToolTipControllerGetActiveObjectInfoEventHandler(_toolTipController_GetActiveObjectInfo);
			_toolTipController.BeforeShow += new ToolTipControllerBeforeShowEventHandler(_toolTipController_BeforeShow);
			_toolTipController.CustomDraw += new ToolTipControllerCustomDrawEventHandler(_toolTipController_CustomDraw);

			this.ShowOnlyPredefinedDetails = true;		// Show only pre-defined details in the grid
		}

		/// <summary>
		/// Override the create control so we can set some properties the first time this control gets set on the form
		/// </summary>
		protected override void OnCreateControl()
		{
			base.OnCreateControl();		// Run the base method

			// Modify the grouping
			// HA!  This works!!!
			if (!_allowGrouping)
			{
				if (this.GridView != null)
				{
					((GridView)this.GridView).OptionsView.ShowGroupPanel = _allowGrouping;
					((GridView)this.GridView).OptionsCustomization.AllowGroup = _allowGrouping;
				}
				else if (this.LayoutView != null)
				{
					((LayoutView)this.LayoutView).OptionsView.ShowHeaderPanel = false;
					((LayoutView)this.LayoutView).OptionsView.ShowCardExpandButton = false;
					((LayoutView)this.LayoutView).OptionsView.ViewMode = LayoutViewMode.MultiRow;
					((LayoutView)this.LayoutView).OptionsBehavior.ScrollVisibility = ScrollVisibility.Never;
					((LayoutView)this.LayoutView).OptionsBehavior.AllowExpandCollapse = false;
					((LayoutView)this.LayoutView).OptionsView.ShowCardExpandButton = false;
					((LayoutView)this.LayoutView).OptionsCustomization.AllowSort = false;
					((LayoutView)this.LayoutView).OptionsCustomization.AllowFilter = false;
				}
				//else
				//{
				//    MessageBox.Show("Gridview null");
				//}
			}
		}

		private void mgDevX_GridControl_ViewRemoved(object sender, ViewOperationEventArgs e)
		{
			//if (e.View is GridView)
			//{
			//    WireUpEventsForViews((GridView)e.View, false);		// Wire up the events for the grid
			//}
		}

		private void mgDevX_GridControl_ViewRegistered(object sender, ViewOperationEventArgs e)
		{
			//if (e.View is GridView)
			//{
			//    WireUpEventsForViews((GridView)e.View, true);		// Wire up the events for the grid
			//}
		}

		/// <summary>
		/// Provides a list of custom draw overrides - this means that the internal custom draw Cell event will not fire on these views
		/// so that they can be handled on the outside
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<string> CustomDrawGridViewOverrides
		{
			get { return _customDrawGridViewOverrides; }
			set { _customDrawGridViewOverrides = value; }
		}

		/// <summary>
		/// Provides a count of matching rows in the database
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public long DBCount
		{
			get { return _dbCount; }
			set { _dbCount = value; }
		}

		/// <summary>
		/// Tells if the user can check/uncheck the column header in the grid to check all the values in the grid
		/// </summary>
		public bool CheckUncheckByClickingColumnHeader
		{
			get { return _checkUncheckByClickingColumnHeader; }
			set { _checkUncheckByClickingColumnHeader = value; }
		}

		/// <summary>
		/// The collection of links in the grid control
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public mgDevX_GridControl_LinkCollection LinkCollection
		{
			get { return _linkCollection; }
		}

		/// <summary>
		/// The collection of buttons in the grid control
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public mgDevX_GridControl_ButtonCollection ButtonCollection
		{
			get { return _buttonCollection; }
		}

		/// <summary>
		/// Reposition the link columns at the end of the grid
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool RepositionLinkColumnsAtEnd
		{
			get { return _repositionLinkColumnsAtEnd;  }
			set { _repositionLinkColumnsAtEnd = value; }
		}

		/// <summary>
		/// The Key field name to use on the control
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string KeyFieldName
		{
			get { return _keyFieldName; }
			set { _keyFieldName = value; }
		}

		/// <summary>
		/// Provides a size for the customization form
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Size? CustomizationFormSize
		{
			get { return _customizationFormSize; }
			set { _customizationFormSize = value; }
		}

		/// <summary>
		/// Gives a list of the disabled link columns
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public List<string> DisabledLinkColumns
		{
			get { return _disabledLinkColumns; }
			set { _disabledLinkColumns = value; }
		}

		/// <summary>
		/// Gives a list of the disabled link columns
		/// </summary>
		public bool IsLinkColumnEnabled(string columnName)
		{
			bool rtv = true;
			if (mgCustom.Utils.ListContainsString(_disabledLinkColumns, columnName)) { rtv = false; }
			return rtv;
		}

		/// <summary>
		/// Gives a list of the disabled link columns
		/// </summary>
		public void EnableLinkColumn(string columnName)
		{
			if (mgCustom.Utils.ListContainsString(_disabledLinkColumns, columnName)) 
			{
				for (int i = _disabledLinkColumns.Count - 1; i >= 0; i--)
				{
					if (_disabledLinkColumns[i].ToLower() == columnName.ToLower()) { _disabledLinkColumns.RemoveAt(i); }
				}
			}
		}

		/// <summary>
		/// Integer value specifying tooltip delay in milliseconds. The default is 5000.
		/// </summary>
		[DefaultValue(5000)]
		[Browsable(true)]
		public int ToolTipPopupDelay
		{
			get { return _toolTipController.AutoPopDelay; }
			set { _toolTipController.AutoPopDelay = value; }
		}

		public new BaseView MainView
		{
			get { return base.MainView; }
			set 
			{ 
				base.MainView = value;
				if (this.DesignMode) { return; }

				// Get all the child views as well
				foreach (BaseView view in base.Views)
				{
					if (view is GridView)
					{
						WireUpEventsForViews(((GridView)view), true);

						// Go through the level tree to find child relations
						foreach (GridLevelNode node in base.LevelTree.Nodes)
						{
							if (node.LevelTemplate is GridView)
							{
								WireUpEventsForViews((GridView)node.LevelTemplate, true);
							}
						}
					}
					else if (view is LayoutView)
					{
						WireUpEventsForViews(((LayoutView)view), true);

						//// Go through the level tree to find child relations
						//foreach (GridLevelNode node in base.LevelTree.Nodes)
						//{
						//    if (node.LevelTemplate is GridView)
						//    {
						//        WireUpEventsForViews((GridView)node.LevelTemplate, true);
						//    }
						//}
					}
				}
			}
		}

		#region Remember Layout Properties
		/// <summary>
		/// Remembers the layout prior to a refresh
		/// </summary>
		public void RememberLayout()
		{
			//_keyFieldName = keyFieldName;

			_layoutRemembered = true;

			_layoutStreams = new Dictionary<GridView, string>();
			foreach (BaseView view in this.Views)
			{
				if (view is GridView)
				{
					GridView vw = (GridView)view;

					using (System.IO.MemoryStream strm = new System.IO.MemoryStream())
					{
						OptionsLayoutGrid opt = new OptionsLayoutGrid();
						opt.LayoutVersion = Application.ProductVersion;
						opt.StoreAllOptions = true;
						opt.StoreVisualOptions = true;
						opt.StoreDataSettings = true;
						opt.Columns.StoreAllOptions = true;
						opt.Columns.StoreLayout = true;
						opt.Columns.StoreAppearance = true;
						opt.Columns.RemoveOldColumns = false;
						opt.Columns.AddNewColumns = true;
						vw.SaveLayoutToStream(strm);
						strm.Position = 0;

						StreamReader sr = new StreamReader(strm);
						string val = sr.ReadToEnd();
						sr.Close();

						// Save the stream to memory
						_layoutStreams.Add(vw, val);
					}
				}
			}

			//// Take care of the visible columns
			//_layoutVisibleColumns = new Dictionary<GridView, SearchFavoriteColumnCollection>();
			//_layoutGroupedColumns = new Dictionary<GridView, Dictionary<string, int>>();
			//_layoutExpandedGroupValues = new Dictionary<GridView, List<RowInfo>>();
			//foreach (BaseView view in this.Views)
			//{
			//    if (view is GridView)
			//    {
			//        GridView vw = (GridView)view;
			//        _layoutVisibleColumns.Add(vw, new SearchFavoriteColumnCollection());
			//        _layoutGroupedColumns.Add(vw, new Dictionary<string, int>());
			//        _layoutExpandedGroupValues.Add(vw, new List<RowInfo>());

			//        // Set the visible columns
			//        foreach (DevExpress.XtraGrid.Columns.GridColumn col in vw.VisibleColumns)
			//        {
			//            _layoutVisibleColumns[vw].Add(new
			//                ssModel.SearchFavoriteColumn(col.FieldName, col.Caption, col.SortOrder, col.SortIndex));
			//        }

			//        // Set the grouped columns
			//        foreach (DevExpress.XtraGrid.Columns.GridColumn col in vw.GroupedColumns)
			//        {
			//            _layoutGroupedColumns[vw].Add(col.FieldName, col.GroupIndex);
			//        }

			//        // Save the expansion view information
			//        if (vw.GroupedColumns.Count == 0) return;
			//        GridColumn column = vw.Columns[_keyFieldName];
			//        //for (int i = -1; i > int.MinValue; i--)
			//        for (int i = 0; i < vw.RowCount; i++)
			//        {
			//            //if (!vw.IsValidRowHandle(i)) break;
			//            if (vw.IsGroupRow(i) && vw.GetRowExpanded(i))
			//            {
			//                RowInfo rowInfo;
			//                int dataRowHandle = vw.GetDataRowHandleByGroupRowHandle(i);
			//                rowInfo.Id = vw.GetRowCellValue(dataRowHandle, column);
			//                rowInfo.level = vw.GetRowLevel(i);
			//                _layoutExpandedGroupValues[vw].Add(rowInfo);
			//            }
			//        }
			//    }
			//}


			//if (this.MainView is GridView)
			//{
			//    GridView mainView = (GridView)this.MainView;

			//    // Save the expanded rows
			//    //if (mainView.GridControl.Views.Count == 1) return;
			//    //list.Clear();
			//    //GridColumn column = view.Columns[keyFieldName];
			//    //for (int i = 0; i < view.DataRowCount; i++)
			//    //{
			//    //    if (view.GetMasterRowExpanded(i))
			//    //    {
			//    //        list.Add(view.GetRowCellValue(i, column));
			//    //        _layoutExpandedGroupValues
			//    //    }
			//    //}

				

			//    // Save the selection view information
			//    //list.Clear();
			//    //GridColumn column = view.Columns[keyFieldName];
			//    //RowInfo rowInfo;
			//    //int[] selectionArray = view.GetSelectedRows();
			//    //if (selectionArray != null)  // otherwise we have a single focused but not selected row
			//    //    for (int i = 0; i < selectionArray.Length; i++)
			//    //    {
			//    //        int dataRowHandle = selectionArray[i];
			//    //        rowInfo.level = view.GetRowLevel(dataRowHandle);
			//    //        if (dataRowHandle < 0) // group row
			//    //            dataRowHandle = view.GetDataRowHandleByGroupRowHandle(dataRowHandle);
			//    //        rowInfo.Id = view.GetRowCellValue(dataRowHandle, column);
			//    //        list.Add(rowInfo);
			//    //    }
			//    //rowInfo.Id = view.GetRowCellValue(view.FocusedRowHandle, column);
			//    //rowInfo.level = view.GetRowLevel(view.FocusedRowHandle);
			//    //list.Add(rowInfo);

			//    // Save the visible row index
			//    _layoutVisibleRowIndex = mainView.GetVisibleIndex(mainView.FocusedRowHandle) - mainView.TopRowIndex;
			//}

			//// Save the filter information
			//_layoutFilterValues = new Dictionary<GridView, Dictionary<string, string>>();
			//foreach (BaseView view in this.Views)
			//{
			//    if (view is GridView)
			//    {
			//        GridView vw = (GridView)view;
			//        _layoutFilterValues.Add(vw, new Dictionary<string, string>());
			//        foreach (ViewColumnFilterInfo filterInfo in vw.ActiveFilter)
			//        {
			//            foreach (GridColumn col in vw.Columns)
			//            {
			//                if (vw.GetRowCellValue(GridControl.AutoFilterRowHandle, col) != null &&
			//                    !String.IsNullOrEmpty(vw.GetRowCellValue(GridControl.AutoFilterRowHandle, col).ToString()))
			//                {
			//                    _layoutFilterValues[vw].Add(col.FieldName,
			//                        vw.GetRowCellValue(GridControl.AutoFilterRowHandle, col).ToString());
			//                }
			//            }
			//        }
			//    }
			//}
		}

		/// <summary>
		/// Remembers the layout prior to a refresh
		/// </summary>
		public void MemorizeLayout()
		{
			RememberLayout();		// Just call the other function
		}

		/// <summary>
		/// Restore the layout from the captured settings
		/// </summary>
		public void RestoreLayout()
		{
			if (!_layoutRemembered) { return; }
			_layoutRemembered = false;

			foreach (BaseView view in this.Views)
			{
				if (view is GridView)
				{
					GridView vw = (GridView)view;
					if (_layoutStreams[vw] != null)
					{
						MemoryStream strm = new MemoryStream();
						StreamWriter w = new StreamWriter(strm);
						w.AutoFlush = true;
						w.Write(_layoutStreams[vw]);
						strm.Position = 0;
						try
						{
							view.RestoreLayoutFromStream(strm);		// Restore the values
						}
						catch
						{
							//int test = 1;
						}
					}
				}
			}

			//// Take care of the visible columns
			//foreach (KeyValuePair<GridView, SearchFavoriteColumnCollection> kvp in _layoutVisibleColumns)
			//{
			//    // Turn off all the columns in the view
			//    for (int colIndex = kvp.Key.Columns.Count - 1; colIndex >= 0; colIndex--)
			//    {
			//        kvp.Key.Columns[colIndex].VisibleIndex = -1;
			//    }

			//    // Put them back in there
			//    int count = 0;
			//    foreach (SearchFavoriteColumn col in kvp.Value)
			//    {
			//        foreach (GridColumn c in kvp.Key.Columns)
			//        {
			//            if (col.FieldName.ToLower() == c.FieldName.ToLower())
			//            {
			//                c.VisibleIndex = count;
			//                c.Caption = col.Caption;
			//                c.SortOrder = col.Sort;
			//                c.SortIndex = col.SortIndex;
			//                count++;
			//                break;
			//            }
			//        }
			//    }
			//}

			//// Take care of the groupings
			//foreach (KeyValuePair<GridView, Dictionary<string, int>> kvp in _layoutGroupedColumns)
			//{
			//    // Turn off all the groupings
			//    kvp.Key.ClearGrouping();

			//    // Put them all back
			//    Dictionary<string, int> sortedDict = Utils.SortDictionary<string, int>(kvp.Value);
			//    foreach (KeyValuePair<string, int> dict in sortedDict)
			//    {
			//        kvp.Key.Columns[dict.Key].GroupIndex = dict.Value;
			//    }
			//}

			//// Save the expansion view information
			//foreach (KeyValuePair<GridView, List<RowInfo>> kvp in _layoutExpandedGroupValues)
			//{
			//    kvp.Key.BeginUpdate();
			//    try
			//    {
			//        kvp.Key.CollapseAllGroups();
			//        foreach (RowInfo info in kvp.Value)
			//        {
			//            int dataRowHandle = kvp.Key.LocateByValue(0, kvp.Key.Columns[_keyFieldName], info.Id);
			//            if (dataRowHandle != GridControl.InvalidRowHandle)
			//            {
			//                int parentRowHandle = kvp.Key.GetParentRowHandle(dataRowHandle);
			//                while (kvp.Key.GetRowLevel(parentRowHandle) != info.level)
			//                {
			//                    parentRowHandle = kvp.Key.GetParentRowHandle(parentRowHandle);
			//                }

			//                //int parentRowHandle = FindParentRowHandle(info, dataRowHandle);
			//                kvp.Key.SetRowExpanded(parentRowHandle, true, false);
			//            }
			//        }
			//    }
			//    finally
			//    {
			//        kvp.Key.EndUpdate();
			//    }
			//}

			//if (this.MainView is GridView)
			//{
			//    GridView mainView = (GridView)this.MainView;

			//    // Save the expanded rows
			//    //if (mainView.GridControl.Views.Count == 1) return;
			//    //list.Clear();
			//    //GridColumn column = view.Columns[keyFieldName];
			//    //for (int i = 0; i < view.DataRowCount; i++)
			//    //{
			//    //    if (view.GetMasterRowExpanded(i))
			//    //    {
			//    //        list.Add(view.GetRowCellValue(i, column));
			//    //    }
			//    //}

			//    // Save the expansion view information
			//    //if (view.GroupedColumns.Count == 0) return;
			//    //list.Clear();
			//    //GridColumn column = view.Columns[keyFieldName];
			//    //for (int i = -1; i > int.MinValue; i--)
			//    //{
			//    //    if (!view.IsValidRowHandle(i)) break;
			//    //    if (view.GetRowExpanded(i))
			//    //    {
			//    //        RowInfo rowInfo;
			//    //        int dataRowHandle = view.GetDataRowHandleByGroupRowHandle(i);
			//    //        rowInfo.Id = view.GetRowCellValue(dataRowHandle, column);
			//    //        rowInfo.level = view.GetRowLevel(i);
			//    //        list.Add(rowInfo);
			//    //    }
			//    //}

			//    // Save the selection view information
			//    //list.Clear();
			//    //GridColumn column = view.Columns[keyFieldName];
			//    //RowInfo rowInfo;
			//    //int[] selectionArray = view.GetSelectedRows();
			//    //if (selectionArray != null)  // otherwise we have a single focused but not selected row
			//    //    for (int i = 0; i < selectionArray.Length; i++)
			//    //    {
			//    //        int dataRowHandle = selectionArray[i];
			//    //        rowInfo.level = view.GetRowLevel(dataRowHandle);
			//    //        if (dataRowHandle < 0) // group row
			//    //            dataRowHandle = view.GetDataRowHandleByGroupRowHandle(dataRowHandle);
			//    //        rowInfo.Id = view.GetRowCellValue(dataRowHandle, column);
			//    //        list.Add(rowInfo);
			//    //    }
			//    //rowInfo.Id = view.GetRowCellValue(view.FocusedRowHandle, column);
			//    //rowInfo.level = view.GetRowLevel(view.FocusedRowHandle);
			//    //list.Add(rowInfo);

			//    // Set the visible row index
			//    mainView.FocusedRowHandle = _layoutVisibleRowIndex;
			//}

			//// Load the filter information
			//foreach (KeyValuePair<GridView, Dictionary<string, string>> kvp in _layoutFilterValues)
			//{
			//    foreach (KeyValuePair<string, string> dict in kvp.Value)
			//    {
			//        kvp.Key.SetRowCellValue(GridControl.AutoFilterRowHandle, dict.Key, dict.Value);
			//    }
			//}
		}
		#endregion Remember Layout Properties

		/// <summary>
		/// Wire up certain events for each of the views
		/// </summary>
		/// <param name="view">The view to wire the events for</param>
		public void WireUpEventsForViews(GridView view, bool wireUp)
		{
			if (wireUp)
			{
				// Format the grid
				view.Appearance.EvenRow.BackColor = System.Drawing.Color.White;
				view.Appearance.EvenRow.Options.UseBackColor = true;
				view.Appearance.GroupRow.ForeColor = Color.Black;
				view.Appearance.GroupRow.Options.UseForeColor = true;
				view.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
				view.Appearance.HeaderPanel.Options.UseFont = true;
				view.Appearance.HeaderPanel.Options.UseTextOptions = true;
				view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
				view.Appearance.OddRow.BackColor = System.Drawing.Color.White;
				view.Appearance.OddRow.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
				view.Appearance.OddRow.Options.UseBackColor = true;
				view.AppearancePrint.EvenRow.BackColor = System.Drawing.Color.White;
				view.AppearancePrint.EvenRow.Options.UseBackColor = true;
				view.AppearancePrint.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
				view.AppearancePrint.HeaderPanel.Options.UseFont = true;
				view.AppearancePrint.OddRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(255)))));
				view.AppearancePrint.OddRow.Options.UseBackColor = true;

				view.OptionsPrint.UsePrintStyles = true;
				view.OptionsSelection.EnableAppearanceFocusedCell = false;
				view.OptionsView.EnableAppearanceEvenRow = true;
				view.OptionsView.EnableAppearanceOddRow = true;

				// Set up the group format for the view
				view.GroupFormat = _defaultGroupFormat;
			}

			view.CustomDrawCell -= new RowCellCustomDrawEventHandler(view_CustomDrawCell);
			view.CustomDrawGroupPanel -= new CustomDrawEventHandler(view_CustomDrawGroupPanel);
			view.DragObjectStart -= new DragObjectStartEventHandler(view_DragObjectStart);
			view.DragObjectDrop -= new DragObjectDropEventHandler(view_DragObjectDrop);
			view.ShowCustomizationForm -= new EventHandler(view_ShowCustomizationForm);
			view.MouseMove -= new System.Windows.Forms.MouseEventHandler(view_MouseMove);
			view.MouseUp -= new System.Windows.Forms.MouseEventHandler(view_MouseUp);
			view.CustomDrawColumnHeader -= new ColumnHeaderCustomDrawEventHandler(view_CustomDrawColumnHeader);
			view.ShowingEditor -= new CancelEventHandler(view_ShowingEditor);
			view.ShownEditor -= new EventHandler(view_ShownEditor);
			view.HiddenEditor -= new EventHandler(view_HiddenEditor);
			
			if (wireUp)
			{
				view.CustomDrawCell += new RowCellCustomDrawEventHandler(view_CustomDrawCell);
				view.CustomDrawGroupPanel += new CustomDrawEventHandler(view_CustomDrawGroupPanel);
				view.DragObjectStart += new DragObjectStartEventHandler(view_DragObjectStart);
				view.DragObjectDrop += new DragObjectDropEventHandler(view_DragObjectDrop);
				view.ShowCustomizationForm += new EventHandler(view_ShowCustomizationForm);
				view.MouseMove += new System.Windows.Forms.MouseEventHandler(view_MouseMove);
				view.MouseUp += new System.Windows.Forms.MouseEventHandler(view_MouseUp);
				view.CustomDrawColumnHeader += new ColumnHeaderCustomDrawEventHandler(view_CustomDrawColumnHeader);
				view.ShowingEditor += new CancelEventHandler(view_ShowingEditor);
				view.ShownEditor += new EventHandler(view_ShownEditor);
				view.HiddenEditor += new EventHandler(view_HiddenEditor);
			}
		}

		/// <summary>
		/// Wire up certain events for each of the views
		/// </summary>
		/// <param name="view">The view to wire the events for</param>
		public void WireUpEventsForViews(LayoutView view, bool wireUp)
		{
			if (wireUp)
			{
				// Format the grid
				view.OptionsPrint.UsePrintStyles = true;
			}

			view.CustomDrawCardCaption -= new LayoutViewCustomDrawCardCaptionEventHandler(view_CustomDrawCardCaption);
			view.CustomDrawCardFieldCaption -= new RowCellCustomDrawEventHandler(view_CustomDrawCardFieldCaption);
			view.CustomDrawCardFieldValue -= new RowCellCustomDrawEventHandler(view_CustomDrawCardFieldValue);
			//view.CustomCardLayout += new LayoutViewCustomCardLayoutEventHandler(view_CustomCardLayout);
			view.ShownEditor -= new EventHandler(view_ShownEditor);

			if (wireUp)
			{
				view.CustomDrawCardCaption += new LayoutViewCustomDrawCardCaptionEventHandler(view_CustomDrawCardCaption);
				view.CustomDrawCardFieldCaption += new RowCellCustomDrawEventHandler(view_CustomDrawCardFieldCaption);
				view.CustomDrawCardFieldValue += new RowCellCustomDrawEventHandler(view_CustomDrawCardFieldValue);
				view.ShownEditor += new EventHandler(view_ShownEditor);
			}
		}

		private void view_CustomDrawCardFieldValue(object sender, RowCellCustomDrawEventArgs e)
		{
			// Figure out if the caption is part of the fields caption
			//foreach (mgDevX_GridControl_Button button in _buttonCollection)
			//{
			//    if (button.FieldName.ToLower() == e.Column.FieldName.ToLower())
			//    {
			//        e.DisplayText = string.Empty;
			//        break;
			//    }
			//}
			foreach (mgDevX_GridControl_Button btn in _buttonCollection)
			{
				if (btn.FieldName.ToLower() == e.Column.FieldName.ToLower())
				{
					e.Appearance.Image = btn.ButtonImage;
					
					//e.Bounds.Height = btn.ButtonImage.Height + 12;
					break;
				}
			}
		}

		private void view_CustomDrawCardFieldCaption(object sender, RowCellCustomDrawEventArgs e)
		{
			// Figure out if the caption is part of the fields caption
			//foreach (mgDevX_GridControl_Button button in _buttonCollection)
			//{
			//    if (button.FieldName.ToLower() == e.Column.FieldName.ToLower())
			//    {
			//        e.DisplayText = string.Empty;
			//        break;
			//    }
			//}
		}

		private void view_CustomDrawCardCaption(object sender, LayoutViewCustomDrawCardCaptionEventArgs e)
		{
			// If they have a value set, get the value from the underlying set
			if (!String.IsNullOrEmpty(_layoutCaptionField))
			{
				LayoutView view = sender as LayoutView;
				string val = (view.GetRowCellValue(e.RowHandle, _layoutCaptionField) != null ?
					view.GetRowCellValue(e.RowHandle, _layoutCaptionField).ToString() : string.Empty);
				e.CardCaption = val;
			}
		}

		private void view_ShowingEditor(object sender, CancelEventArgs e)
		{
			// Check to see if we've disabled something
			GridView view = ((GridView)sender);
			if (this.DataSource != null &&
				this.DataSource is IBindingList)
			{
				IBindingList list = (IBindingList)this.DataSource;
				if (list.Count > 0 &&
					list[0] is mgModel_BaseObject)
				{
					// Get the row we're trying to handle here
					mgModel_BaseObject obj = (mgModel_BaseObject)view.GetRow(view.FocusedRowHandle);
					if (obj != null &&
						(obj.GridCustom_8.ToLower() == "donotshowcheck" ||
						obj.GridCustom_9.ToLower() == "donotshowcheck"))
					{
						// Don't allow the editor
						e.Cancel = true;
						return;
					}
				}
			}
			if (this.DataSource != null &&
				this.DataSource is DataTable)
			{
				//DataTable list = (DataTable)this.DataSource;
				DataRowView row = (DataRowView)view.GetRow(view.FocusedRowHandle);
				if (row.Row.Table.Columns.Contains("GridCustom_8") &&
					row["GridCustom_8"] != DBNull.Value &&
					row["GridCustom_8"].ToString().ToLower() == "donotshowcheck")
				{
					e.Cancel = true;
					return;
				}
				if (row.Row.Table.Columns.Contains("GridCustom_9") &&
					row["GridCustom_9"] != DBNull.Value &&
					row["GridCustom_9"].ToString().ToLower() == "donotshowcheck")
				{
					e.Cancel = true;
					return;
				}
			}
		}

		private void view_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			// Draw the CheckedInGrid column header differently
			if (e.Column != null &&
				e.Column.FieldName == "CheckedInGrid" &&
				!e.Info.CustomizationForm)
			{
				Rectangle rect = e.Bounds;

				//e.Column.OptionsColumn.ShowCaption = false;
				e.Info.Caption = string.Empty;
				e.Painter.DrawObject(e.Info); // <-----

				Brush brush =
					e.Cache.GetGradientBrush(rect, e.Column.AppearanceHeader.BackColor,
					e.Column.AppearanceHeader.BackColor2, e.Column.AppearanceHeader.GradientMode);
				rect.Inflate(-1, -1);

				// Fill column headers with the specified colors.
				e.Graphics.FillRectangle(brush, rect);
				e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);

				// Draw the filter and sort buttons.
				//foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
				//{
				//    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter,
				//        info.ElementInfo);
				//}

				// Find the center point of the column and draw the image
				//Image img = global::_BaseWinProject.Win.Common.res16x16.Check_Header;
				//Rectangle centerRect = new Rectangle(((e.Bounds.Width - img.Width) / 2) + e.Bounds.X,
				//	((e.Bounds.Height - img.Height) / 2) + e.Bounds.Y,
				//	img.Width, img.Height);
				//e.Graphics.DrawImage(img, centerRect);
				e.Handled = true;
			}
		}

		private void view_DragObjectDrop(object sender, DragObjectDropEventArgs e)
		{
			if ((e.DragObject is GridColumn) &&
				(e.DropInfo is DevExpress.XtraGrid.Dragging.ColumnPositionInfo))
			{
				if (sender is GridView)
				{
					GridView view = sender as GridView;
					if (view.VisibleColumns.Count != _visibleColumns.Count)
					{
						this.OnVisibleColumnsChanged();
						return;
					}

					// Let's see if the counts are the same, but the contents have changed
					foreach (GridColumn col in (sender as GridView).VisibleColumns)
					{
						if (!_visibleColumns.Contains(col.FieldName))
						{
							this.OnVisibleColumnsChanged();
							break;
						}
					}
				}
			}
		}

		private void view_DragObjectStart(object sender, DragObjectStartEventArgs e)
		{
			// When they start the drag/drop get a list of the visible columns
			_visibleColumns = new List<string>();
			if (sender is GridView)
			{
				foreach (GridColumn col in (sender as GridView).VisibleColumns)
				{
					if (!_visibleColumns.Contains(col.FieldName)) { _visibleColumns.Add(col.FieldName); }
				}
			}
		}

		private void view_ShowCustomizationForm(object sender, EventArgs e)
		{
			(sender as GridView).CustomizationForm.Resize -= new EventHandler(CustomizationForm_Resize);
			(sender as GridView).CustomizationForm.Resize += new EventHandler(CustomizationForm_Resize);

			// Go through and remove any fields that have weird names or shouldn't be avaiable to the grid
			//foreach (GridColumn col in (sender as GridView).Columns)
			for (int i = (sender as GridView).Columns.Count - 1; i >= 0; i--)
			{
				GridColumn col = (sender as GridView).Columns[i];
				switch (col.FieldName)
				{
					case "BrokenRules":
					case "ChangedProps":
					case "DateTimeObjectPopulated":
					case "RecordStatus":

					case "IsDisposable":
					case "IsLoaded":
					case "IsValid":

					case "OriginalValues":
					case "RefreshRate":
					case "Rules":

					case "GridCustom0":
					case "GridCustom1":
					case "GridCustom2":
					case "GridCustom3":
					case "GridCustom4":

					case "GridCustom5":
					case "GridCustom6":
					case "GridCustom7":
					case "GridCustom8":
					case "GridCustom9":
						col.OptionsColumn.ShowInCustomizationForm = false;
						(sender as GridView).CustomizationForm.View.Columns.Remove(col);
						break;
				}
				if (col.FieldName.EndsWith("Item") && 
					col.FieldName != "Item")
				{
					col.OptionsColumn.ShowInCustomizationForm = false;
					(sender as GridView).CustomizationForm.View.Columns.Remove(col);
				}
			}

			//Console.Clear();
			//foreach (GridColumn col in (sender as GridView).CustomizationForm.View.Columns)
			//{
			//    Console.WriteLine(col.FieldName);
			//}

			if (_customizationFormSize.HasValue)
			{
			    (sender as GridView).CustomizationForm.Size = _customizationFormSize.Value;
			}
			else
			{
				int width = 130;
				int height = Math.Max((sender as GridView).GridControl.Height - 100, 300);
				int left = (sender as GridView).GridControl.PointToScreen(new Point(0, 0)).X +
					(sender as GridView).GridControl.Width - 220;
				int top = ((sender as GridView).GridControl.PointToScreen(new Point(0, 0)).Y) + 30;

				(sender as GridView).CustomizationForm.Size = new Size(width, height);
				(sender as GridView).CustomizationForm.Location = new Point(left, top);
			}
		}

		private void CustomizationForm_Resize(object sender, EventArgs e)
		{
			_customizationFormSize = (sender as DevExpress.XtraGrid.Views.Grid.Customization.CustomizationForm).Size;
		}

		//private void view_MouseDown(object sender, MouseEventArgs e)
		//{
		//    if (this.DesignMode) { return; }
		//    try
		//    {
		//        DevExpress.XtraGrid.Views.Grid.GridView gridView = ((DevExpress.XtraGrid.Views.Grid.GridView)sender);
		//        GridHitInfo ghi = gridView.CalcHitInfo(new Point(e.X, e.Y));
		//        if ((sender is GridView) &&
		//            ghi.InRowCell &&
		//            ghi.Column != null &&
		//            ghi.Column.ColumnType.ToString().Contains("System.Boolean"))
		//        {
		//            _oldValInCheckColumn = (bool)((GridView)sender).GetRowCellValue(ghi.RowHandle, ghi.Column);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        ssException error = new ssException(ex);
		//    }
		//}

		private void view_HiddenEditor(object sender, EventArgs e)
		{
			if (_chkEditor != null)
			{
				_chkEditor.EditValueChanged -= new EventHandler(ColumnEdit_EditValueChanged);
				_chkEditor = null;
				_chkView = null;
			}
			_oldValInCheckColumn = null;
		}

		private void view_ShownEditor(object sender, EventArgs e)
		{
			if (sender is GridView)
			{
				if (((GridView)sender).FocusedColumn.ColumnType.ToString().Contains("System.Boolean"))
				{
					_chkView = ((GridView)sender);

					// Check to make sure they have the right row
					if (_chkView.FocusedRowHandle == GridControl.InvalidRowHandle ||
						_chkView.FocusedRowHandle == GridControl.AutoFilterRowHandle)
					{
						_chkView = null;
						return;
					}

					_chkEditor = ((GridView)sender).ActiveEditor;
					_chkEditor.EditValueChanged += new EventHandler(ColumnEdit_EditValueChanged);
					_oldValInCheckColumn = (bool)_chkView.GetRowCellValue(_chkView.FocusedRowHandle, _chkView.FocusedColumn);
				}
			}
			else if (sender is LayoutView)
			{
				//LayoutView view = (LayoutView)sender;
				//foreach (mgDevX_GridControl_Button button in _buttonCollection)
				//{
				//    if (button.FieldName.ToLower() == view.FocusedColumn.FieldName.ToLower())
				//    {
				//        ButtonEdit ed = (ButtonEdit)view.ActiveEditor;
				//        ed.Properties.Buttons[0].Caption = view.GetFocusedDisplayText();
				//        break;
				//    }
				//}
			}
		}

		private void ColumnEdit_EditValueChanged(object sender, EventArgs e)
		{
			// When the filter changes, reset the count
			if (_checkProcessingRunning) { return; }
			if (_chkView != null)
			{
				bool cancel = false;
				this.OnGridCheckClicking(_chkView,
					_chkView.FocusedColumn,
					_chkView.FocusedRowHandle,
					(bool)_chkView.EditingValue,
					_oldValInCheckColumn.Value,
					ref cancel);
				if (!cancel)
				{
					_chkView.PostEditor();
					if (_oldValInCheckColumn.HasValue &&
						_oldValInCheckColumn.Value != (bool)_chkView.GetRowCellValue(_chkView.FocusedRowHandle, _chkView.FocusedColumn))
					{
						bool chkd = (bool)_chkView.GetRowCellValue(_chkView.FocusedRowHandle, _chkView.FocusedColumn);
						this.OnGridCheckClicked(_chkView,
							_chkView.FocusedColumn,
							_chkView.FocusedRowHandle,
							chkd,
							_oldValInCheckColumn.Value);
						_oldValInCheckColumn = chkd;
					}
				}
				else
				{
					// Change the value back to the old value
					_checkProcessingRunning = true;
					_chkView.EditingValue = _oldValInCheckColumn.Value;
					_checkProcessingRunning = false;
				}
			}
		}

		private void view_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.DesignMode) { return; }
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView gridView = ((DevExpress.XtraGrid.Views.Grid.GridView)sender);
				GridHitInfo ghi = gridView.CalcHitInfo(new Point(e.X, e.Y));
				if (ghi.InRowCell &&
					_linkCollection.ContainsFieldName(gridView, ghi.Column.FieldName))
				{
					// Throw an event
					mgDevX_GridControl_Link link = _linkCollection.GetByViewAndFieldName(gridView, ghi.Column.FieldName);
					if (gridView.GetRowCellValue(ghi.RowHandle, ghi.Column) == null ||
						String.IsNullOrEmpty(gridView.GetRowCellValue(ghi.RowHandle, ghi.Column).ToString().Trim()))
					{ return; }
					if (link != null)
					{
						this.OnGridLinkClicked(link.GridView, 
							ghi.Column, 
							ghi.RowHandle, 
							link.LinkType,
							gridView.GetRow(ghi.RowHandle));
						((DXMouseEventArgs)e).Handled = true;
					}
				}
				if (ghi.InColumnPanel &&
					ghi.Column.FieldName == "CheckedInGrid")
				{
					ghi.Column.OptionsColumn.AllowSort = DefaultBoolean.False;

					if (_checkUncheckByClickingColumnHeader)
					{
						// Run the code for check/uncheck all
						this.CheckUncheckVisible(gridView);
					}
				}
			}
			catch 
			{
			}
		}

		/// <summary>
		/// Clear the filter on the grid's active view
		/// </summary>
		public void ClearFilter()
		{
			ClearFilter(this.MainView);
		}

		/// <summary>
		/// Clear the filter on the specified view
		/// </summary>
		public void ClearFilter(BaseView view)
		{
			if (view is GridView)
			{
				((GridView)view).ActiveFilter.Clear();
			}
		}

		/// <summary>
        /// Clear the checked column
        /// </summary>
        private void ClearChecked()
        {
			if (this.DataSource != null &&
				this.DataSource is IBindingList)
			{
				IBindingList list = (IBindingList)this.DataSource;
				if (list.Count > 0 &&
					list[0] is mgModel_BaseObject)
				{
					foreach (mgModel_BaseObject obj in list)
					{
						obj.CheckedInGrid = false;
					}
				}
			}

			if (this.DataSource != null &&
				this.DataSource is DataTable)
			{
				DataTable dt = (DataTable)this.DataSource;
				int columnIndex = -1;
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					if (dt.Columns[i].ColumnName.ToLower() == "checkedingrid")
					{
						columnIndex = i;
						break;
					}
				}
				if (columnIndex > -1)
				{
					foreach (DataRow row in dt.Rows)
					{
						row[columnIndex] = false;
					}
				}
			}
		}

		/// <summary>
		/// See how many we have checked
		/// </summary>
		public int CheckedCount
		{
			get
			{
				int rtv = 0;
				if (this.DataSource != null &&
					this.DataSource is IBindingList)
				{
					IBindingList list = (IBindingList)this.DataSource;
					if (list.Count > 0 &&
						list[0] is mgModel_BaseObject)
					{
						foreach (mgModel_BaseObject obj in list)
						{
							if (obj.CheckedInGrid) { rtv++; }
						}
					}
					else if (list.Count > 0)
					{
						System.Reflection.PropertyInfo[] props = list[0].GetType().GetProperties();
						foreach (System.Reflection.PropertyInfo prop in props)
						{
							if (prop.Name.ToLower() == "checkedingrid")
							{
								foreach (object o in list)
								{
									if ((bool)o.GetType().GetProperty(prop.Name).GetValue(o, null))
									{
										rtv++;
									}
								}
								break;
							}
						}
					}
				}
				else if (this.DataSource != null &&
					this.DataSource is DataTable)
				{
					DataTable dt = (DataTable)this.DataSource;
					int columnIndex = -1;
					for (int i = 0; i < dt.Columns.Count; i++)
					{
						if (dt.Columns[i].ColumnName.ToLower() == "checkedingrid")
						{
							columnIndex = i;
							break;
						}
					}
					if (columnIndex > -1)
					{
						foreach (DataRow row in dt.Rows)
						{
							if ((bool)row[columnIndex]) { rtv++; }
						}
					}
				}
				return rtv;
			}
		}

		///// <summary>
		///// See how many we have checked
		///// </summary>
		//public int CheckedChildrenCount
		//{
		//    get
		//    {
		//        int rtv = 0;

		//        // Go out and get the count from the corresponding children by the dataset in memory


		//        GridView detailView = null;
		//        for (int i = 0; i < ((GridView)this.MainView).RowCount; i++)
		//        {
		//            for (int relCount = 0; relCount < ((GridView)this.MainView).GetRelationCount(i); relCount++)
		//            {
		//                detailView = (GridView)((GridView)this.MainView).GetDetailView(i, relCount);
		//                if (detailView == null)
		//                {
		//                    DataRowView rowView = (((DevExpress.XtraGrid.Views.Grid.GridView)this.MainView).GetRow(i) as DataRowView);
		//                    DataView dv = rowView.CreateChildView(((DevExpress.XtraGrid.Views.Grid.GridView)this.MainView).GetRelationName(i, relCount));
		//                    int test = 1;
		//                }
		//                if (detailView != null &&
		//                    detailView.DataRowCount > 0 &&
		//                    detailView.Columns.Count > 0 &&
		//                    detailView.Columns.ColumnByFieldName("CheckedInGrid") != null)
		//                {
		//                    for (int detailIndex = 0; detailIndex < detailView.RowCount; detailIndex++)
		//                    {
		//                        if (detailView.GetRowCellValue(detailIndex, "CheckedInGrid") != null &&
		//                            bool.Parse(detailView.GetRowCellValue(detailIndex, "CheckedInGrid").ToString()))
		//                        {
		//                            rtv++;
		//                        }
		//                    }
		//                }
		//            }
		//        }
		//        return rtv;
		//    }
		//}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="view">The view to set against</param>
		/// <param name="chkd">True if checked, otherwise, false</param>
		public void CheckUncheckAll(BaseView view, bool chkd)
		{
			if (view is GridView)
			{
				if (((GridView)view).DataRowCount > 0 &&
					((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					this.BeginUpdate();		// Begin the update process

					ClearChecked();		// Clear all the checkmarks

					if (this.DataSource != null &&
						this.DataSource is IBindingList)
					{
						IBindingList list = (IBindingList)this.DataSource;
						if (list.Count > 0 &&
							list[0] is mgModel_BaseObject)
						{
							foreach (mgModel_BaseObject obj in list)
							{
								if (!String.IsNullOrEmpty(obj.GridCustom_8) &&
									obj.GridCustom_8.ToLower() == "donotshowcheck")
								{
									// Do nothing
								}
								else if (!String.IsNullOrEmpty(obj.GridCustom_9) &&
									obj.GridCustom_9.ToLower() == "donotshowcheck")
								{
									// Do nothing
								}
								else
								{
									obj.CheckedInGrid = chkd;
								}
							}
						}
						else if (list.Count > 0)
						{
							System.Reflection.PropertyInfo[] props = list[0].GetType().GetProperties();
							foreach (System.Reflection.PropertyInfo prop in props)
							{
								if (prop.Name.ToLower() == "checkedingrid")
								{
									foreach (object o in list)
									{
										o.GetType().GetProperty(prop.Name).SetValue(o, chkd, null);
									}
									break;
								}
							}
						}
					}

					if (this.DataSource != null &&
						this.DataSource is DataTable)
					{
						DataTable dt = (DataTable)this.DataSource;
						int checkedInGridColumnIndex = -1;
						int gridCustom8ColumnIndex = -1;
						int gridCustom9ColumnIndex = -1;
						for (int i = 0; i < dt.Columns.Count; i++)
						{
							if (dt.Columns[i].ColumnName.ToLower() == "checkedingrid") { checkedInGridColumnIndex = i; }
							if (dt.Columns[i].ColumnName.ToLower() == "gridcustom_8") { gridCustom8ColumnIndex = i; }
							if (dt.Columns[i].ColumnName.ToLower() == "gridcustom_9") { gridCustom9ColumnIndex = i; }
						}
						if (checkedInGridColumnIndex > -1)
						{
							foreach (DataRow row in dt.Rows)
							{
								if (gridCustom8ColumnIndex > -1 &&
									row[gridCustom8ColumnIndex] != DBNull.Value &&
									row[gridCustom8ColumnIndex].ToString().ToLower() == "donotshowcheck")
								{
									// Do nothing
								}
								else if (gridCustom9ColumnIndex > -1 &&
									row[gridCustom9ColumnIndex] != DBNull.Value &&
									row[gridCustom9ColumnIndex].ToString().ToLower() == "donotshowcheck")
								{
									// Do nothing
								}
								else
								{
									row[checkedInGridColumnIndex] = chkd;
								}
							}
						}
					}

					((GridView)view).RefreshData();
					//Application.DoEvents();
					this.EndUpdate();		// Begin the update process

					// Throw an event to the outside
					this.OnGridCheckClicked(((GridView)view),
						((GridView)view).Columns["CheckedInGrid"],
						-1,
						chkd,
						!chkd);
				}
			}
		}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="view">The view to set against</param>
		public void CheckUncheckAll(BaseView view)
		{
			if (view is GridView)
			{
				// Find the first value
				bool chkd = false;
				if (((GridView)view).DataRowCount > 0 &&
					((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					for (int i = 0; i < view.RowCount; i++)
					{
						if (((GridView)view).Columns.ColumnByFieldName("GridCustom_8") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_8") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_8").ToString().ToLower() != "donotshowcheck")
							{
								chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
								break;
							}
						}
						else if (((GridView)view).Columns.ColumnByFieldName("GridCustom_9") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_9") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_9").ToString().ToLower() != "donotshowcheck")
							{
								chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
								break;
							}
						}
						else
						{
							chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
							break;
						}
					}
					//chkd = bool.Parse(((GridView)view).GetRowCellValue(0, "CheckedInGrid").ToString());
				}

				// Check/Uncheck the values
				CheckUncheckAll(view, !chkd);
			}
		}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="chkd">True if checked, otherwise, false</param>
		public void CheckUncheckAll(bool chkd)
		{
			CheckUncheckAll(this.MainView, chkd);
		}

		/// <summary>
		/// Sets all the elements in the grid to checked/unchecked based on a toggle of what's found in the first row
		/// </summary>
		public void CheckUncheckAll()
		{
			CheckUncheckAll(this.MainView);
		}

		/// <summary>
		/// Check/Uncheck all the visible rows in the grid
		/// </summary>
		/// <param name="view">The view to set against</param>
		/// <param name="chkd">True if checked, otherwise, false</param>
		public void CheckUncheckVisible(BaseView view, bool chkd)
		{
			if (view is GridView)
			{
				if (((GridView)view).DataRowCount > 0 &&
					((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					this.BeginUpdate();		// Begin the update process

					if (view.Name == this.MainView.Name)
					{
						ClearChecked();		// Clear all the checkmarks
					}

					// Go through the visible rows
					// and just check those off
					for (int i = 0; i < ((GridView)view).RowCount; i++)
					{
						if (((GridView)view).Columns.ColumnByFieldName("GridCustom_8") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_8") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_8").ToString().ToLower() != "donotshowcheck")
							{
								((GridView)view).SetRowCellValue(i, "CheckedInGrid", chkd);
							}
						}
						else if (((GridView)view).Columns.ColumnByFieldName("GridCustom_9") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_9") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_9").ToString().ToLower() != "donotshowcheck")
							{
								((GridView)view).SetRowCellValue(i, "CheckedInGrid", chkd);
							}
						}
						else
						{
							((GridView)view).SetRowCellValue(i, "CheckedInGrid", chkd);
						}
					}

					// Take care of the child views
					GridView detailView = null;
					for (int i = 0; i < ((GridView)view).RowCount; i++)
					{
						for (int relCount = 0; relCount < ((GridView)view).GetRelationCount(i); relCount++)
						{
							detailView = (GridView)((GridView)view).GetDetailView(i, relCount);
							if (detailView != null &&
								detailView.DataRowCount > 0 &&
								detailView.Columns.Count > 0 &&
								detailView.Columns.ColumnByFieldName("CheckedInGrid") != null)
							{
								for (int detailIndex = 0; detailIndex < detailView.RowCount; detailIndex++)
								{
									detailView.SetRowCellValue(detailIndex, "CheckedInGrid", chkd);
								}
							}
						}
					}

					((GridView)view).RefreshData();
					//Application.DoEvents();
					this.EndUpdate();		// Begin the update process

					// Throw an event to the outside
					this.OnGridCheckClicked(((GridView)view),
						((GridView)view).Columns["CheckedInGrid"],
						-1,
						chkd,
						!chkd);
				}
			}
		}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="view">The view to set against</param>
		public void CheckUncheckVisible(BaseView view)
		{
			if (view is GridView)
			{
				// Find the first value
				bool chkd = false;
				if (((GridView)view).DataRowCount > 0 &&
					((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					for (int i = 0; i < ((GridView)view).RowCount; i++)
					{
						if (((GridView)view).Columns.ColumnByFieldName("GridCustom_8") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_8") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_8").ToString().ToLower() != "donotshowcheck")
							{
								chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
								break;
							}
						}
						else if (((GridView)view).Columns.ColumnByFieldName("GridCustom_9") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_9") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_9").ToString().ToLower() != "donotshowcheck")
							{
								chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
								break;
							}
						}
						else
						{
							chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
							break;
						}
					}
					//chkd = bool.Parse(((GridView)view).GetRowCellValue(0, "CheckedInGrid").ToString());
				}

				// Check/Uncheck the values
				CheckUncheckVisible(view, !chkd);
			}
		}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="chkd">True if checked, otherwise, false</param>
		public void CheckUncheckVisible(bool chkd)
		{
			CheckUncheckVisible(this.MainView, chkd);
		}

		/// <summary>
		/// Sets all the elements in the grid to checked/unchecked based on a toggle of what's found in the first row
		/// </summary>
		public void CheckUncheckVisible()
		{
			CheckUncheckVisible(this.MainView);
		}


		/// <summary>
		/// Check/Uncheck all the children rows in the grid
		/// </summary>
		/// <param name="view">The view to set against</param>
		/// <param name="chkd">True if checked, otherwise, false</param>
		public void CheckUncheckChildren(BaseView view, bool chkd)
		{
			if (view is GridView)
			{
				if (((GridView)view).DataRowCount > 0 &&
					((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					this.BeginUpdate();		// Begin the update process

					if (view.Name == this.MainView.Name)
					{
						ClearChecked();		// Clear all the checkmarks
					}

					// Take care of the child views
					GridView detailView = null;
					for (int i = ((GridView)view).FocusedRowHandle; i <= ((GridView)view).FocusedRowHandle; i++)
					{
						for (int relCount = 0; relCount < ((GridView)view).GetRelationCount(i); relCount++)
						{
							detailView = (GridView)((GridView)view).GetDetailView(i, relCount);
							if (detailView != null &&
								detailView.DataRowCount > 0 &&
								detailView.Columns.Count > 0 &&
								detailView.Columns.ColumnByFieldName("CheckedInGrid") != null)
							{
								for (int detailIndex = 0; detailIndex < detailView.RowCount; detailIndex++)
								{
									detailView.SetRowCellValue(detailIndex, "CheckedInGrid", chkd);
								}
							}
						}
					}

					((GridView)view).RefreshData();
					this.EndUpdate();		// Begin the update process

					// Throw an event to the outside
					this.OnGridCheckClicked(((GridView)view),
						((GridView)view).Columns["CheckedInGrid"],
						-1,
						chkd,
						!chkd);
				}
			}
		}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="view">The view to set against</param>
		public void CheckUncheckChildren(BaseView view)
		{
			if (view is GridView)
			{
				// Find the first value
				bool chkd = false;
				if (((GridView)view).DataRowCount > 0 &&
					((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					for (int i = 0; i < ((GridView)view).RowCount; i++)
					{
						if (((GridView)view).Columns.ColumnByFieldName("GridCustom_8") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_8") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_8").ToString().ToLower() != "donotshowcheck")
							{
								chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
								break;
							}
						}
						else if (((GridView)view).Columns.ColumnByFieldName("GridCustom_9") != null)
						{
							// See if we have a do not show check in this row
							if (((GridView)view).GetRowCellValue(i, "GridCustom_9") == null ||
								((GridView)view).GetRowCellValue(i, "GridCustom_9").ToString().ToLower() != "donotshowcheck")
							{
								chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
								break;
							}
						}
						else
						{
							chkd = bool.Parse(((GridView)view).GetRowCellValue(i, "CheckedInGrid").ToString());
							break;
						}
					}
					//chkd = bool.Parse(((GridView)view).GetRowCellValue(0, "CheckedInGrid").ToString());
				}

				// Check/Uncheck the values
				CheckUncheckChildren(view, !chkd);
			}
		}

		/// <summary>
		/// Check/Uncheck all the values in the grid
		/// </summary>
		/// <param name="chkd">True if checked, otherwise, false</param>
		public void CheckUncheckChildren(bool chkd)
		{
			CheckUncheckChildren(this.MainView, chkd);
		}

		/// <summary>
		/// Sets all the elements in the grid to checked/unchecked based on a toggle of what's found in the first row
		/// </summary>
		public void CheckUncheckChildren()
		{
			CheckUncheckChildren(this.MainView);
		}


		private void view_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.DesignMode) { return; }
			DevExpress.XtraGrid.Views.Grid.GridView gridView = ((DevExpress.XtraGrid.Views.Grid.GridView)sender);
			DevExpress.XtraGrid.GridControl gridControl = gridView.GridControl;
			gridControl.Cursor = System.Windows.Forms.Cursors.Default;
			GridHitInfo ghi = gridView.CalcHitInfo(new Point(e.X, e.Y));
			if (ghi.InRowCell)
			{
				mgDevX_GridControl_Link link = _linkCollection.GetByViewAndFieldName(gridView, ghi.Column.FieldName);
				if (link != null)
				{
					if (gridView.GetRowCellValue(ghi.RowHandle, ghi.Column) == null ||
						String.IsNullOrEmpty(gridView.GetRowCellValue(ghi.RowHandle, ghi.Column).ToString().Trim()))
					{ return; }
					if (gridView.GetRowCellValue(ghi.RowHandle, ghi.Column).ToString().Contains("{NoHand}")) { return; }
					if (gridView.GetRowCellValue(ghi.RowHandle, ghi.Column).ToString().Contains("-999999")) { return; }
					gridControl.Cursor = System.Windows.Forms.Cursors.Hand;
				}
			}
		}

		private string GetCellHintText(GridView view, int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
		{
			//string ret = view.GetRowCellDisplayText(rowHandle, column);
			//foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
			//{
			//    if (col.VisibleIndex < 0)
			//    {
			//        ret += string.Format("\r\n {0}: {1}", col.Caption, view.GetRowCellDisplayText(rowHandle, col));
			//    }
			//}
			return view.GetRowCellDisplayText(rowHandle, column);
		}

		public delegate void ToolTipBeforeShowEventHandler(object sender, ToolTipBeforeShow_EventArgs e);
		public event ToolTipBeforeShowEventHandler ToolTipBeforeShow;
		protected void OnToolTipBeforeShow(ToolTipControllerShowEventArgs e,
			GridView view,
			int rowHandle,
			GridColumn column)
		{
			if (ToolTipBeforeShow != null)
			{
				ToolTipBeforeShow_EventArgs args = new ToolTipBeforeShow_EventArgs(e, view, column, rowHandle);
				ToolTipBeforeShow(this, args);
			}
		}

		private void _toolTipController_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
		{
			// Find out the location in the grid that was clicked
			ToolTipController controller = sender as ToolTipController;
			if (controller.ActiveObject is DevExpress.XtraGrid.Views.Base.CellToolTipInfo)
			{
				DevExpress.XtraGrid.Views.Base.CellToolTipInfo info = 
					controller.ActiveObject as DevExpress.XtraGrid.Views.Base.CellToolTipInfo;
				if (info == null ||
					info.Column == null) { return; }

				this.OnToolTipBeforeShow(e, (GridView)info.Column.View, info.RowHandle, info.Column);
			}
		}

		private void _toolTipController_CustomDraw(object sender, ToolTipControllerCustomDrawEventArgs e)
		{
		}

		private void _toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
		{
			ToolTipControlInfo info = null;
			try
			{
				GridView view = this.GetViewAt(e.ControlMousePosition) as GridView;
				if (view == null) return;
				GridHitInfo hi = view.CalcHitInfo(e.ControlMousePosition);
				if (!hi.InRowCell &&
					hi.HitTest != GridHitTest.RowIndicator)
				{
					return;
				}
				if (hi.InRowCell)
				{
					// Handle any error text we might have in the cell
					GridCellInfo cellInfo = (view.GetViewInfo() as GridViewInfo).GetGridCellInfo(hi);
					Rectangle errorIconRect = new Rectangle(cellInfo.Bounds.Location, 
						new Size(cellInfo.Bounds.Height, cellInfo.Bounds.Height));
					if (view.GetRow(hi.RowHandle) is DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo)
					{
						DevExpress.XtraEditors.DXErrorProvider.ErrorInfo errorInfo = new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo();
						((DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo)view.GetRow(hi.RowHandle)).GetPropertyError(hi.Column.FieldName, errorInfo);
						if (errorIconRect.Contains(e.ControlMousePosition) &&
							!String.IsNullOrEmpty(errorInfo.ErrorText))
						{
							string title = string.Empty;
							ToolTipIconType iconType = ToolTipIconType.None;
							switch (errorInfo.ErrorType)
							{
								case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical:
								//case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default:
									title = "Critical Error";
									iconType = ToolTipIconType.Error;
									break;
								case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning:
									title = "Warning";
									iconType = ToolTipIconType.Warning;
									break;
								case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Information:
									title = "Information";
									iconType = ToolTipIconType.Information;
									break;
								default:
									title = "";
									iconType = ToolTipIconType.None;
									break;
							}
							if (!String.IsNullOrEmpty(title.Trim()))
							{
								info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell"),
									errorInfo.ErrorText,
									title,
									iconType);
							}
							else
							{
								info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell"),
									errorInfo.ErrorText,
									iconType);
							}
							return;
						}
					}

					// Then handle overage for the link cells
					mgDevX_GridControl_Link link = _linkCollection.GetByViewAndFieldName(view, hi.Column.FieldName);
					if (link != null)
					{
						info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell"),
							GetCellHintText(view, hi.RowHandle, hi.Column));
						//_toolTipController.HideHint();
						//_toolTipController.ShowHint(info);
						//e.Info = info;
						return;
					}

					if (info == null)
					{
						// Just set the default tool tip
						string ret = view.GetRowCellDisplayText(hi.RowHandle, hi.Column);
						//foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
						//{
						//    if (col.VisibleIndex < 0)
						//    {
						//        ret += string.Format("\r\n {0}: {1}", col.GetTextCaption(), view.GetRowCellDisplayText(hi.RowHandle, col));
						//    }
						//}
						info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell"), ret);
					}
				}
				//if (hi.Column != null)
				//{
				//    info = new ToolTipControlInfo(hi.Column, GetColumnHintText(hi.Column));
				//    return;
				//}
				//if (hi.HitTest == GridHitTest.GroupPanel)
				//{
				//    info = new ToolTipControlInfo(hi.HitTest, "Group panel");
				//    return;
				//}
				if (hi.HitTest == GridHitTest.RowIndicator &&
					view.GetRow(hi.RowHandle) is DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo)
				{
					// Handle any error text we might have in the cell
					DevExpress.XtraEditors.DXErrorProvider.ErrorInfo errorInfo = new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo();
					((DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo)view.GetRow(hi.RowHandle)).GetError(errorInfo);
					if (!String.IsNullOrEmpty(errorInfo.ErrorText))
					{
						string title = string.Empty;
						ToolTipIconType iconType = ToolTipIconType.None;
						switch (errorInfo.ErrorType)
						{
							case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical:
								//case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default:
								title = "Critical Error";
								iconType = ToolTipIconType.Error;
								break;
							case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning:
								title = "Warning";
								iconType = ToolTipIconType.Warning;
								break;
							case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Information:
								title = "Information";
								iconType = ToolTipIconType.Information;
								break;
							default:
								title = "";
								iconType = ToolTipIconType.None;
								break;
						}
						if (!String.IsNullOrEmpty(title.Trim()))
						{
							info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell"),
								errorInfo.ErrorText,
								title,
								iconType);
						}
						else
						{
							info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell"),
								errorInfo.ErrorText,
								iconType);
						}
					}
					return;
				}
			}
			finally
			{
				e.Info = info;
			}
		}

		/// <summary>
		/// Tells if the column exists in the view
		/// </summary>
		/// <param name="view">The view to find the column in</param>
		/// <param name="fieldName">The field name</param>
		/// <returns>True if the column exists</returns>
		public bool ColumnExistsInView(GridView view, string fieldName)
		{
			bool rtv = false;
			foreach (GridColumn col in view.Columns)
			{
				if (col.FieldName.ToLower() == fieldName.ToLower())
				{
					rtv = true;
					break;
				}
			}
			return rtv;
		}

		/// <summary>
		/// Custom draw the group panel
		/// </summary>
		public void CustomDrawGroupPanel(object sender, CustomDrawEventArgs e)
		{
			Color startColor = Color.FromArgb(208, 209, 255);
			//Color startColor = Color.White;
			//Color endColor = Color.MediumSeaGreen;
			//Color endColor = Color.LightCyan;
			Color endColor = Color.FromArgb(233, 234, 239);

			System.Drawing.Drawing2D.LinearGradientBrush paintBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, startColor, endColor, -45F);

			System.Drawing.Drawing2D.Blend b = new System.Drawing.Drawing2D.Blend();
			b.Positions = new float[] { 0, 0.2F, 1 };
			b.Factors = new float[] { 0, .3F, 1 };
			paintBrush.Blend = b;

			e.Graphics.FillRectangle(paintBrush, e.Bounds);
			
			e.Handled = true;
		}

		/// <summary>
		/// Call the custom method to paint the cell
		/// </summary>
		public void CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			// Draw the link columns in the grid
			if (this.DesignMode) { return; }
			if (e.RowHandle == GridControl.AutoFilterRowHandle) { return; }
			if (e.CellValue == null ||
				String.IsNullOrEmpty(e.CellValue.ToString())) { return; }
			GridView gridView = sender as GridView;

			// Tells if this row is a selected row
			bool selectedRow = (e.RowHandle == gridView.FocusedRowHandle);

			// Take care of the Checked In Grid Column - make it disappear if 
			// GridCustom_8 or GridCustom_9 has "DoNotShowCheck" in the field value
			if (e.Column.FieldName == "CheckedInGrid" &&
				((ColumnExistsInView(gridView, "GridCustom_8") &&
					gridView.GetRowCellValue(e.RowHandle, "GridCustom_8") != null && 
					gridView.GetRowCellValue(e.RowHandle, "GridCustom_8").ToString().ToLower() == "donotshowcheck") ||
				(ColumnExistsInView(gridView, "GridCustom_9") && 
					gridView.GetRowCellValue(e.RowHandle, "GridCustom_9") != null && 
					gridView.GetRowCellValue(e.RowHandle, "GridCustom_9").ToString().ToLower() == "donotshowcheck")))
			{
				e.Appearance.BackColor2 = e.Appearance.BackColor;
				e.Appearance.FillRectangle(e.Cache, e.Bounds);
				e.Handled = true;
			}

			//if (_linkFieldNames.ContainsValue(e.Column.FieldName))
			mgDevX_GridControl_Link link = _linkCollection.GetByViewAndFieldName(gridView, e.Column.FieldName);
			if (link != null)
			{
				Rectangle r = e.Bounds;
				int totalWidth = 16;
				//int imageWidth = 15;

				// MG - Leave the row below blanked
				// If you want to draw things in the cell, copy the code from this method into your form
				//e.Cache.FillRectangle(new SolidBrush(e.Appearance.BackColor), 
				//    new Rectangle(r.Left - 1, r.Top - 1, r.Width + 2, r.Height + 2));

				SizeF size = e.Graphics.MeasureString(e.CellValue.ToString(), e.Appearance.Font);
				totalWidth += 6 + (int)size.Width;
				int left = r.Left + ((r.Width - totalWidth) / 2);
				int top = r.Top;

				if (link.LinkImage != null)
				{
					//imageWidth = 15;
					Image img = new Bitmap(link.LinkImage, 15, 15);
					Rectangle imgRect = new Rectangle(), imgRectCrop = new Rectangle();
					if (totalWidth < r.Width)
					{
						int crop = (left - r.Left < 0 ? left - r.Left : 0);
						imgRect = new Rectangle(left - crop, top, 15 + crop, 15);
						imgRectCrop = new Rectangle(0 - crop, 0, 15 + crop, 15);
					}
					else
					{
						left = r.Left + ((r.Width - img.Width) / 2);
						int crop = (left - r.Left < 0 ? left - r.Left : 0);
						imgRect = new Rectangle(left - crop, top, 15 + crop, 15);
						imgRectCrop = new Rectangle(0 - crop, 0, 15 + crop, 15);
					}
					if (mgCustom.Utils.ListContainsString(_disabledLinkColumns, e.Column.FieldName)) { img = UserInterface.GetGrayscaleImage(img); }
					e.Graphics.DrawImage(img, imgRect, imgRectCrop, GraphicsUnit.Pixel);
				}
				//else
				//{
				//	imageWidth = 0;
				//}

				// Draw the text
				if (totalWidth < r.Width)
				{
					Rectangle textRect = new Rectangle(left + 16 + 6, r.Top, (int)size.Width, (int)size.Height);
					StringFormat format = new StringFormat();

					Color textColor = mgCustom.Utils.GetTextColorBasedOnBackgroundColor(e.Appearance.BackColor, Color.White, Color.Blue);
					if (selectedRow) { textColor = Color.Blue; }

					// Get the text color for a disabled link
					Font f = new Font(e.Appearance.Font.Name, e.Appearance.Font.Size, FontStyle.Underline);
					if (mgCustom.Utils.ListContainsString(_disabledLinkColumns, e.Column.FieldName)) 
					{
						textColor = Color.DimGray;
						f = new Font(e.Appearance.Font.Name, e.Appearance.Font.Size);
					}
					e.Cache.DrawString(e.CellValue.ToString(), f, new SolidBrush(textColor), textRect, format);
				}
				e.Handled = true;
			}
		}

		private void view_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (_customDrawGridViewOverrides != null && 
				!_customDrawGridViewOverrides.Contains(((BaseView)sender).Name))
			{
				CustomDrawCell(sender, e);		// Call the custom event handler
			}
		}

		private void view_CustomDrawGroupPanel(object sender, CustomDrawEventArgs e)
		{
			CustomDrawGroupPanel(sender, e);		// Call the custom event handler
		}

		/// <summary>
		/// Get the group display format string based on the type sent in
		/// </summary>
		/// <param name="format">The format to be sent in</param>
		/// <returns>The string that's the formatted string</returns>
		public string GetGroupDisplayFormatString(GridGroupSummaryDisplayFormat format)
		{
			string rtv = string.Empty;

			switch (format)
			{
				case GridGroupSummaryDisplayFormat.Currency: rtv = "c2"; break;
				case GridGroupSummaryDisplayFormat.CurrencyNoRounding: rtv = "\"$\"###,###,###,##0.00#########"; break;
				case GridGroupSummaryDisplayFormat.Decimal1Place: rtv = "n1"; break;
				case GridGroupSummaryDisplayFormat.Decimal2Places: rtv = "n2"; break;
				case GridGroupSummaryDisplayFormat.Decimal3Places: rtv = "n3"; break;
				case GridGroupSummaryDisplayFormat.Decimal4Places: rtv = "n4"; break;
				case GridGroupSummaryDisplayFormat.Integer: rtv = "n0"; break;
				case GridGroupSummaryDisplayFormat.Percentage: rtv = "##0.####\"%\""; break;
				case GridGroupSummaryDisplayFormat.Percentage1Decimal: rtv = "##0.0\"%\""; break;
				case GridGroupSummaryDisplayFormat.Percentage2Decimals: rtv = "##0.00\"%\""; break;
				case GridGroupSummaryDisplayFormat.Percentage3Decimals: rtv = "##0.000\"%\""; break;
				case GridGroupSummaryDisplayFormat.Percentage4Decimals: rtv = "##0.0000\"%\""; break;
			}

			return rtv;
		}

		/// <summary>
		/// Apply the sum and counts to groups in the grid
		/// </summary>
		/// <param name="sumOnFieldName">The field to sum on</param>
		/// <param name="countOnFieldName">The field to count on</param>
		public void ApplySumCountToGroup(string sumOnFieldName,
			string countOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplaySum,
			GridGroupSummaryDisplayFormat formatDisplayCount)
		{
			ApplySumCountToGroup(this.MainView, sumOnFieldName, countOnFieldName, formatDisplaySum, formatDisplayCount);
		}
		
		/// <summary>
		/// Apply the sum and counts to groups in the grid
		/// </summary>
		/// <param name="view">The view to set the elements on</param>
		/// <param name="sumOnFieldName">The field to sum on</param>
		/// <param name="countOnFieldName">The field to count on</param>
		public void ApplySumCountToGroup(BaseView view, 
			string sumOnFieldName, 
			string countOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplaySum,
			GridGroupSummaryDisplayFormat formatDisplayCount)
		{
			if (view is GridView)
			{
				((GridView)view).GroupSummary.Clear();

				// Add the Sum
				GridSummaryItem item1 = ((GridView)view).GroupSummary.Add();
				item1.FieldName = sumOnFieldName;
				item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
				item1.Tag = 1;
				item1.DisplayFormat = "(Sum = {0:" + GetGroupDisplayFormatString(formatDisplaySum) + "})";

				// Add the Count
				GridSummaryItem item2 = ((GridView)view).GroupSummary.Add();
				item2.FieldName = countOnFieldName;
				item2.SummaryType = DevExpress.Data.SummaryItemType.Count;
				item2.Tag = 2;
				item2.DisplayFormat = "(Count = {0:" + GetGroupDisplayFormatString(formatDisplayCount) + "})";
			}
		}

		/// <summary>
		/// Apply the sum and counts to groups in the grid
		/// </summary>
		/// <param name="sumOnFieldName">The field to sum on</param>
		/// <param name="countOnFieldName">The field to count on</param>
		public void ApplyCountSumToGroup(string countOnFieldName,
			string sumOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplayCount,
			GridGroupSummaryDisplayFormat formatDisplaySum)
		{
			ApplyCountSumToGroup(this.MainView, countOnFieldName, sumOnFieldName, formatDisplayCount, formatDisplaySum);
		}

		/// <summary>
		/// Apply the sum and counts to groups in the grid
		/// </summary>
		/// <param name="view">The view to set the elements on</param>
		/// <param name="sumOnFieldName">The field to sum on</param>
		/// <param name="countOnFieldName">The field to count on</param>
		public void ApplyCountSumToGroup(BaseView view,
			string countOnFieldName,
			string sumOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplayCount,
			GridGroupSummaryDisplayFormat formatDisplaySum)
		{
			if (view is GridView)
			{
				((GridView)view).GroupSummary.Clear();

				// Add the Count
				GridSummaryItem item2 = ((GridView)view).GroupSummary.Add();
				item2.FieldName = countOnFieldName;
				item2.SummaryType = DevExpress.Data.SummaryItemType.Count;
				item2.Tag = 1;
				item2.DisplayFormat = "(Count = {0:" + GetGroupDisplayFormatString(formatDisplayCount) + "})";

				// Add the Sum
				GridSummaryItem item1 = ((GridView)view).GroupSummary.Add();
				item1.FieldName = sumOnFieldName;
				item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
				item1.Tag = 2;
				item1.DisplayFormat = "(Sum = {0:" + GetGroupDisplayFormatString(formatDisplaySum) + "})";
			}
		}

		/// <summary>
		/// Apply the counts to groups in the grid
		/// </summary>
		/// <param name="countOnFieldName">The field to count on</param>
		public void ApplyCountToGroup(string countOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplayCount)
		{
			ApplyCountToGroup(this.MainView, countOnFieldName, formatDisplayCount);
		}

		/// <summary>
		/// Apply the counts to groups in the grid
		/// </summary>
		/// <param name="view">The view to set the elements on</param>
		/// <param name="countOnFieldName">The field to count on</param>
		public void ApplyCountToGroup(BaseView view,
			string countOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplayCount)
		{
			if (view is GridView)
			{
				((GridView)view).GroupSummary.Clear();

				// Add the Count
				GridSummaryItem item2 = ((GridView)view).GroupSummary.Add();
				item2.FieldName = countOnFieldName;
				item2.SummaryType = DevExpress.Data.SummaryItemType.Count;
				item2.Tag = 1;
				item2.DisplayFormat = "(Count = {0:" + GetGroupDisplayFormatString(formatDisplayCount) + "})";
			}
		}

		/// <summary>
		/// Apply the sums to groups in the grid
		/// </summary>
		/// <param name="sumOnFieldName">The field to sum on</param>
		public void ApplySumToGroup(string sumOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplaySum)
		{
			ApplySumToGroup(this.MainView, sumOnFieldName, formatDisplaySum);
		}

		/// <summary>
		/// Apply the sums to groups in the grid
		/// </summary>
		/// <param name="view">The view to set the elements on</param>
		/// <param name="sumOnFieldName">The field to sum on</param>
		public void ApplySumToGroup(BaseView view,
			string sumOnFieldName,
			GridGroupSummaryDisplayFormat formatDisplaySum)
		{
			if (view is GridView)
			{
				((GridView)view).GroupSummary.Clear();

				// Add the Count
				GridSummaryItem item2 = ((GridView)view).GroupSummary.Add();
				item2.FieldName = sumOnFieldName;
				item2.SummaryType = DevExpress.Data.SummaryItemType.Sum;
				item2.Tag = 1;
				item2.DisplayFormat = "(Sum = {0:" + GetGroupDisplayFormatString(formatDisplaySum) + "})";
			}
		}

		/// <summary>
		/// Tells if the specified view contains the field name
		/// </summary>
		private bool ViewContainsFieldName(GridView view, string columnFieldName)
		{
			bool exists = false;

			foreach (GridColumn col in view.Columns)
			{
				if (col.FieldName == columnFieldName)
				{
					exists = true;
					break;
				}
			}

			return exists;
		}

		#region ScrollToColumn
		/// <summary>
		/// Scroll to column
		/// </summary>
		public void ScrollToColumn(string columnName)
		{
			ScrollToColumn(this.MainView, columnName);
		}

		/// <summary>
		/// Scroll to the column
		/// </summary>
		public void ScrollToColumn(GridColumn column)
		{
			ScrollToColumn(this.MainView, column);
		}

		/// <summary>
		/// Scroll to column
		/// </summary>
		public void ScrollToColumn(BaseView view, string columnName)
		{
			if (view is GridView)
			{
				ScrollToColumn(view, ((GridView)view).Columns[columnName]);
			}
		}

		/// <summary>
		/// Scroll to the column
		/// </summary>
		public void ScrollToColumn(BaseView view, GridColumn column)
		{
			if (view is GridView)
			{
				GridViewInfo info = new GridViewInfo((GridView)view);
				int offsetWidth = 0;
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					if (col.Fixed == FixedStyle.Left &&
						col.GroupIndex < 0)
					{
						offsetWidth += col.Width;
					}
				}
				((GridView)view).LeftCoord = info.GetColumnLeftCoord(column) - offsetWidth;
			}
		}
		#endregion ScrollToColumn

		#region Scroll to Top/Bottom
		/// <summary>
		/// Scroll the grid to the top
		/// </summary>
		public void ScrollToTop()
		{
			if (((GridView)this.MainView).RowCount > 0)
			{
				((GridView)this.MainView).TopRowIndex = 0;
				((GridView)this.MainView).SelectRow(0);
				((GridView)this.MainView).FocusedRowHandle = 0;
			}
		}

		/// <summary>
		/// Scroll the grid to the bottom
		/// </summary>
		public void ScrollToBottom()
		{
			if (((GridView)this.MainView).RowCount > 0)
			{
				int gridBottomRow = ((GridView)this.MainView).RowCount - 1;
				((GridView)this.MainView).TopRowIndex = gridBottomRow;
				((GridView)this.MainView).SelectRow(gridBottomRow);
				((GridView)this.MainView).FocusedRowHandle = gridBottomRow;
			}
		}
		#endregion Scroll to Top/Bottom

		#region Expand/Collapse Groups
		/// <summary>
		/// Expand all the groups in the grid
		/// </summary>
		public void ExpandAllGroups()
		{
			ExpandAllGroups(this.MainView);
		}

		/// <summary>
		/// Expand all the groups in the grid
		/// </summary>
		public void ExpandAllGroups(BaseView view)
		{
			if (view is GridView)
			{
				UserInterface.GridUIAction((GridView)view, GridUIActionType.ExpandAllGroups);
			}
		}

		/// <summary>
		/// Collapse all the groups in the grid
		/// </summary>
		public void CollapseAllGroups()
		{
			CollapseAllGroups(this.MainView);
		}
		
		/// <summary>
		/// Collapse all the groups in the grid
		/// </summary>
		public void CollapseAllGroups(BaseView view)
		{
			if (view is GridView)
			{
				UserInterface.GridUIAction((GridView)view, GridUIActionType.CollapseAllGroups);
			}
		}
		#endregion Expand/Collapse Groups

		#region Resize Boolean Columns
		/// <summary>
		/// Resize the boolean column
		/// </summary>
		/// <param name="columnName">The name of the column in the collection to resize</param>
		public void ResizeBooleanColumn(string columnName)
		{
			ResizeBooleanColumn(this.MainView, columnName);
		}

		/// <summary>
		/// Resize the boolean column
		/// For the Checked In Grid column use the ResizeCheckedInGridColumn method
		/// </summary>
		/// <param name="columnName">The name of the column in the collection to resize</param>
		public void ResizeBooleanColumn(BaseView view, string columnName)
		{
			if (view is GridView)
			{
				if (((GridView)view).Columns.ColumnByFieldName(columnName) != null &&
					columnName.ToLower() != "checkedingrid")
				{
					((GridView)view).Columns[columnName].OptionsColumn.FixedWidth = true;
					((GridView)view).Columns[columnName].Width = 40;
				}
			}
		}
		#endregion Resize Boolean Columns

		#region Resize Link Columns
		/// <summary>
		/// Resize the link column
		/// </summary>
		public void ResizeLinkColumns()
		{
			ResizeLinkColumns(this.MainView);
		}

		/// <summary>
		/// Resize the link column
		/// </summary>
		public void ResizeLinkColumns(BaseView view)
		{
			ResizeLinkColumn(view, "GridCustom_0");
			ResizeLinkColumn(view, "GridCustom_1");
			ResizeLinkColumn(view, "GridCustom_2");
			ResizeLinkColumn(view, "GridCustom_3");
			ResizeLinkColumn(view, "GridCustom_4");

			ResizeLinkColumn(view, "GridCustom_5");
			ResizeLinkColumn(view, "GridCustom_6");
			ResizeLinkColumn(view, "GridCustom_7");
			ResizeLinkColumn(view, "GridCustom_8");
			ResizeLinkColumn(view, "GridCustom_9");
		}

		/// <summary>
		/// Resize the link column
		/// </summary>
		/// <param name="columnName">The name of the column in the collection to resize</param>
		public void ResizeLinkColumn(string columnName)
		{
			ResizeLinkColumn(this.MainView, columnName);
		}

		/// <summary>
		/// Resize the link column
		/// </summary>
		/// <param name="columnName">The name of the column in the collection to resize</param>
		public void ResizeLinkColumn(BaseView view, string columnName)
		{
			if (view is GridView)
			{
				if (((GridView)view).Columns.ColumnByFieldName(columnName) != null &&
					columnName.ToLower().StartsWith("gridcustom_"))
				{
					string text = string.Empty;
					if (this.DataSource != null &&
						this.DataSource is IBindingList)
					{
						IBindingList list = (IBindingList)this.DataSource;
						if (list.Count > 0 &&
							list[0] is mgModel_BaseObject)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (columnName == "GridCustom_0" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_0)) { text = ((mgModel_BaseObject)list[i]).GridCustom_0; break; }
								if (columnName == "GridCustom_1" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_1)) { text = ((mgModel_BaseObject)list[i]).GridCustom_1; break; }
								if (columnName == "GridCustom_2" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_2)) { text = ((mgModel_BaseObject)list[i]).GridCustom_2; break; }
								if (columnName == "GridCustom_3" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_3)) { text = ((mgModel_BaseObject)list[i]).GridCustom_3; break; }
								if (columnName == "GridCustom_4" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_4)) { text = ((mgModel_BaseObject)list[i]).GridCustom_4; break; }

								if (columnName == "GridCustom_5" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_5)) { text = ((mgModel_BaseObject)list[i]).GridCustom_5; break; }
								if (columnName == "GridCustom_6" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_6)) { text = ((mgModel_BaseObject)list[i]).GridCustom_6; break; }
								if (columnName == "GridCustom_7" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_7)) { text = ((mgModel_BaseObject)list[i]).GridCustom_7; break; }
								if (columnName == "GridCustom_8" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_8)) { text = ((mgModel_BaseObject)list[i]).GridCustom_8; break; }
								if (columnName == "GridCustom_9" && !String.IsNullOrEmpty(((mgModel_BaseObject)list[i]).GridCustom_9)) { text = ((mgModel_BaseObject)list[i]).GridCustom_9; break; }
							}
						}
					}
					if (String.IsNullOrEmpty(text) &&
						this.DataSource != null &&
						this.DataSource is DataTable)
					{
						foreach (DataRow row in ((DataTable)this.DataSource).Rows)
						{
							if (row[columnName] != DBNull.Value && !String.IsNullOrEmpty(row[columnName].ToString())) 
							{ text = row[columnName].ToString(); break; }
						}
					}
					if (!String.IsNullOrEmpty(text))
					{
						// Get the size of the string
						Graphics g = this.CreateGraphics();
						SizeF sz = g.MeasureString(text, this.Font);
						g.Dispose();

						// Set the width
						((GridView)view).Columns[columnName].OptionsColumn.FixedWidth = true;
						((GridView)view).Columns[columnName].Width = ((int)sz.Width + 1) + 32 + 8;
					}
				}
			}
		}
		#endregion Resize Link Columns

		#region Public Properties

		public bool ShowDetailButtons
		{
			get { return _showDetailButtons; }
			set 
			{ 
				_showDetailButtons = value;
				SetDetailButtons();		// Run the method to reset this in the grid
			}
		}

		[DefaultValue(false)]
		public bool AllowGroup
		{
			get { return _allowGrouping; }
			set 
			{ 
				_allowGrouping = value;
				SetGrouping();		// Run the method to reset this in the grid
			}
		}

		[DefaultValue("")]
		[Browsable(false)]
		public string LayoutCaptionField
		{
			get { return _layoutCaptionField; }
			set 
			{ 
				_layoutCaptionField = value;
			}
		}

		public GridView GridView
		{
			get 
			{
				if (this.MainView is GridView)
				{
					GridView view = this.MainView as GridView;
					return view;
				}
				else
				{
					return null;
				}
			}
		}

		public CardView CardView
		{
			get
			{
				if (this.MainView is CardView)
				{
					CardView view = this.MainView as CardView;
					return view;
				}
				else
				{
					return null;
				}
			}
		}

		public LayoutView LayoutView
		{
			get
			{
				if (this.MainView is LayoutView)
				{
					LayoutView view = this.MainView as LayoutView;
					return view;
				}
				else
				{
					return null;
				}
			}
		}

		#endregion Public Properties

		#region Link Columns
		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void AddLinkColumn(string linkColumnFieldName, GridLinkType type)
		{
			LinkColumnAdd(this.MainView, linkColumnFieldName, type);
		}

		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void AddLinkColumn(string linkColumnFieldName, GridLinkType type, Image img)
		{
			LinkColumnAdd(this.MainView, linkColumnFieldName, type, img);
		}

		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void LinkColumnAdd(string linkColumnFieldName, GridLinkType type)
		{
			LinkColumnAdd(this.MainView, linkColumnFieldName, type, null);
		}

		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void AddLinkColumn(BaseView view, string linkColumnFieldName, GridLinkType type)
		{
			LinkColumnAdd(view, linkColumnFieldName, type);
		}

		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void AddLinkColumn(BaseView view, string linkColumnFieldName, GridLinkType type, Image img)
		{
			LinkColumnAdd(view, linkColumnFieldName, type, img);
		}

		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void LinkColumnAdd(BaseView view, string linkColumnFieldName, GridLinkType type)
		{
			LinkColumnAdd(view, linkColumnFieldName, type, null);
		}

		/// <summary>
		/// Add a link column to the grid
		/// </summary>
		public void LinkColumnAdd(BaseView view, string linkColumnFieldName, GridLinkType type, Image img)
		{
			mgDevX_GridControl_Link newLink = new mgDevX_GridControl_Link();

			//int newIndex = _linkTypes.Count;
			//_linkTypes.Add(newIndex, type);
			//_linkFieldNames.Add(newIndex, linkColumnFieldName);

			newLink.LinkType = type;
			newLink.LinkFieldName = linkColumnFieldName;

			switch (type)
			{
				case GridLinkType.Add:
				case GridLinkType.Delete:
				case GridLinkType.Edit:
				case GridLinkType.Print:
					//_linkImages.Add(newIndex, _imageLibrary[type]);
					newLink.LinkImage = _imageLibrary[type];
					break;
				case GridLinkType.Custom1:
				case GridLinkType.Custom2:
				case GridLinkType.Custom3:
				case GridLinkType.Custom4:
				case GridLinkType.Custom5:
					//_linkImages.Add(newIndex, img);
					newLink.LinkImage = img;
					break;
			}

			// Turn the column to visible
			//if (this.MainView is GridView)
			if (view is GridView)
			{
				newLink.GridView = (GridView)view;
				//GridView view = this.MainView as GridView;
				if (!ViewContainsFieldName((GridView)view, linkColumnFieldName))
				{
					//// The column doesn't exist in the collection
					//throw new Exception("The column \"" + linkColumnFieldName + "\" does not exist in the current grid.");
				}
				else
				{
					if (_repositionLinkColumnsAtEnd)
					{
						((GridView)view).Columns[linkColumnFieldName].VisibleIndex = ((GridView)view).Columns.Count;
					}
					((GridView)view).Columns[linkColumnFieldName].OptionsColumn.AllowEdit = false;

					// Set the size on the column
					switch (type)
					{
						case GridLinkType.Add:
							((GridView)view).Columns[linkColumnFieldName].OptionsColumn.FixedWidth = true;
							((GridView)view).Columns[linkColumnFieldName].Width = 70;
							break;
						case GridLinkType.Delete:
						case GridLinkType.Edit:
							((GridView)view).Columns[linkColumnFieldName].OptionsColumn.FixedWidth = true;
							((GridView)view).Columns[linkColumnFieldName].Width = 80;
							break;
						case GridLinkType.Print:
							((GridView)view).Columns[linkColumnFieldName].OptionsColumn.FixedWidth = true;
							((GridView)view).Columns[linkColumnFieldName].Width = 90;
							break;
					}
				}

				_linkCollection.Add(newLink);		// Add the item to the collection
			}
		}
		#endregion Link Columns

		#region Buttons
		/// <summary>
		/// Add a button to the control (mostly for the layout view)
		/// </summary>
		public void ButtonColumnAdd(BaseView view,
			Font font,
			Image buttonImage,
			string fieldName, 
			DevExpress.XtraEditors.ImageLocation location)
		{
			if (view is LayoutView)
			{
				try
				{
					mgDevX_GridControl_Button btn = new mgDevX_GridControl_Button((LayoutView)view, font,
						buttonImage, fieldName, location);
					_buttonCollection.Add(btn);

					// Try to add a pictureedit
					RepositoryItemPictureEdit picEdit = new RepositoryItemPictureEdit();
					picEdit.SizeMode = PictureSizeMode.Zoom;
					picEdit.InitialImage = buttonImage;
					((LayoutView)view).Columns[fieldName].ColumnEdit = picEdit;
					((LayoutView)view).TemplateCard.Add(((LayoutView)view).Columns[fieldName].LayoutViewField);

					////RepositoryItem_BaseWinProject_ButtonEdit riButtonEdit = this.RepositoryItems.Add("_BaseWinProject_ButtonEdit") as RepositoryItem_BaseWinProject_ButtonEdit;
					//RepositoryItem_BaseWinProject_ButtonEdit riButtonEdit = new RepositoryItem_BaseWinProject_ButtonEdit();
					////RepositoryItemButtonEdit riButtonEdit = this.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;

					//riButtonEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
					//riButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
					//riButtonEdit.Buttons[0].Caption = string.Empty;
					//riButtonEdit.AutoHeight = false;
					//riButtonEdit.Buttons[0].Appearance.Font = font;
					//riButtonEdit.Buttons[0].Image = buttonImage;
					//riButtonEdit.Buttons[0].ImageLocation = location;
					
					////riButtonEdit.ButtonsHeight = 90;

					//((LayoutView)view).Columns[fieldName].OptionsColumn.AllowEdit = true;
					//((LayoutView)view).Columns[fieldName].Visible = true;
					//((LayoutView)view).Columns[fieldName].ColumnEdit = riButtonEdit;
					//((LayoutView)view).Columns[fieldName].ShowButtonMode = ShowButtonModeEnum.ShowAlways;

					////((LayoutView)view).Columns[fieldName].OptionsColumn.ShowInCustomizationForm = true;
					//((LayoutView)view).Columns[fieldName].VisibleIndex = ((LayoutView)view).VisibleColumns.Count + 1;
					//((LayoutView)view).Columns[fieldName].LayoutViewField.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
					//((LayoutView)view).Columns[fieldName].OptionsColumn.AllowSort = DefaultBoolean.False;
					//((LayoutView)view).Columns[fieldName].OptionsFilter.AllowFilter = false;
					//((LayoutView)view).Columns[fieldName].LayoutViewField.TextVisible = false;

					//// Figure out the height of the cards
					//decimal height = 0.00M;
					//Font fCaption = ((LayoutView)view).Appearance.FieldCaption.Font;
					//Font fValue = ((LayoutView)view).Appearance.FieldValue.Font;
					//Graphics g = this.CreateGraphics();
					//SizeF szCaption = g.MeasureString("Testing", fCaption);
					//SizeF szValue = g.MeasureString("Testing", fValue);
					//decimal maxHeight = (decimal)Math.Max(szCaption.Height, szValue.Height);
					
					//foreach (string s in _visibleColumns)
					//{
					//    // Get the height of the current field
					//    SizeF szDetail = g.MeasureString("Testing", ((LayoutView)view).Columns[s].AppearanceCell.Font);
					//    height += (decimal)Math.Max((float)maxHeight, szDetail.Height);
					//}
					//g.Dispose();		// Dispose of the graphics collection

					//// Set up the fields in layout view
					////((LayoutView)view).TemplateCard.Add(((LayoutView)view).Columns[fieldName].LayoutViewField);
					
					//// Add in the height of the image
					//height += buttonImage.Height + 8;
					
					//// Set the height
					////((LayoutView)view).TemplateCard.Height = (int)(height + 1);

					//((LayoutView)view).CardMinSize = new Size(100, (int)height);

					//SimpleButton btn2 = new SimpleButton();
					//btn2.Text = string.Empty;
					//btn2.Image = buttonImage;
					//((LayoutView)view).TemplateCard.AddItem(string.Empty, btn2);

					// Add the button
					//DevExpress.XtraEditors.SimpleButton newButton = new DevExpress.XtraEditors.SimpleButton();
					//newButton.Appearance.Font = font;
					//newButton.Image = buttonImage;
					//newButton.ImageLocation = location;
					//newButton.Location = new System.Drawing.Point(5, 5);
					//newButton.MaximumSize = new System.Drawing.Size(120, 120);
					//newButton.MinimumSize = new System.Drawing.Size(120, 120);
					//newButton.Name = "btn" + fieldName;
					//newButton.Size = new System.Drawing.Size(120, 120);
					//newButton.TabIndex = 291;
					//newButton.Text = "Run";
					//newButton.Click += new EventHandler(newButton_Click);

					//// Add the layout control item
					//DevExpress.XtraLayout.LayoutControlItem lci = new DevExpress.XtraLayout.LayoutControlItem();
					//lci.Control = newButton;
					//lci.CustomizationFormText = "NewLayoutControlItem";
					//lci.Location = new System.Drawing.Point(0, 0);
					//lci.Name = "lci" + fieldName;
					//lci.Size = new System.Drawing.Size(130, 130);
					//lci.Text = "NewLayoutControlItem";
					//lci.TextSize = new System.Drawing.Size(0, 0);
					//lci.TextToControlDistance = 0;
					//lci.TextVisible = false;

					//((LayoutView)view).TemplateCard.AddItem(lci);
				}
				catch
				{
				}
			}
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			// Implement the click for the button
			//int test = 1;
			//this.OnGridButtonClicked(
		}
		#endregion Buttons

		#region Random Functions
		/// <summary>
		/// Set the editor format for the given column
		/// </summary>
		public void SetEditorFormat(string columnFieldName, GridEditorFormat format)
		{
			this.SetEditorFormat(this.MainView, columnFieldName, format);
		}

		/// <summary>
		/// Set the editor format for the given column
		/// </summary>
		public void SetEditorFormat(BaseView view, string columnFieldName, GridEditorFormat format)
		{
			if (view is GridView)
			{
				if (!ViewContainsFieldName((GridView)view, columnFieldName))
				{
					// The column doesn't exist in the collection
					throw new Exception("The column \"" + columnFieldName + "\" does not exist in the current grid.");
				}
				else
				{
					// Set the editor format on the given column
					switch (format)
					{
						case GridEditorFormat.PhoneNumber:
							RepositoryItemTextEdit phoneEdit = new RepositoryItemTextEdit();

							phoneEdit.Mask.EditMask = "(\\(\\d\\d\\d\\)) \\d{1,3}-\\d\\d\\d\\d ?\\d{0,5}";//@"(###) ###-####";
							phoneEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;//DevExpress.XtraEditors.Mask.MaskType.Simple;
							phoneEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = phoneEdit;
							break;

						case GridEditorFormat.SSN:
							RepositoryItemTextEdit ssnEdit = new RepositoryItemTextEdit();

							ssnEdit.Mask.EditMask = @"###-##-####";
							ssnEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
							ssnEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = ssnEdit;
							break;

						case GridEditorFormat.Currency:
							RepositoryItemTextEdit currencyEdit = new RepositoryItemTextEdit();

							currencyEdit.Mask.EditMask = "c2";
							currencyEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							currencyEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = currencyEdit;
							break;

						case GridEditorFormat.CurrencyNoRounding:
							RepositoryItemTextEdit currencyEditNoRounding = new RepositoryItemTextEdit();

							currencyEditNoRounding.Mask.EditMask = "\"$\"###,###,###,##0.00#########";
							currencyEditNoRounding.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							currencyEditNoRounding.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = currencyEditNoRounding;
							break;

						case GridEditorFormat.ConciseDateTime:
							RepositoryItemTextEdit datetimeEditConcise = new RepositoryItemTextEdit();

							datetimeEditConcise.Mask.EditMask = "MM/dd/yy HH:mm";
							datetimeEditConcise.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Custom;
							datetimeEditConcise.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = datetimeEditConcise;
							break;

						case GridEditorFormat.DateTime:
							RepositoryItemTextEdit datetimeEdit = new RepositoryItemTextEdit();

							datetimeEdit.Mask.EditMask = mgCustom.Utils.GetDateAndTimeFormatAsString();
							datetimeEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
							datetimeEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = datetimeEdit;
							
							break;

						case GridEditorFormat.Date:
							RepositoryItemTextEdit dateEdit = new RepositoryItemTextEdit();

							dateEdit.Mask.EditMask = mgCustom.Utils.GetDateFormatAsString();
							dateEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
							dateEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = dateEdit;

							break;

						case GridEditorFormat.Time:
							RepositoryItemTextEdit timeEdit = new RepositoryItemTextEdit();

							timeEdit.Mask.EditMask = mgCustom.Utils.GetTimeFormatAsString();
							timeEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
							timeEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = timeEdit;

							break;

						case GridEditorFormat.FullDateTime:
							RepositoryItemTextEdit datetimeEditFull = new RepositoryItemTextEdit();

							datetimeEditFull.Mask.EditMask = "MM/dd/yyyy hh:mm:ss tt";
							datetimeEditFull.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
							datetimeEditFull.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = datetimeEditFull;

							break;

						case GridEditorFormat.Integer:
							RepositoryItemTextEdit integerEdit = new RepositoryItemTextEdit();

							integerEdit.Mask.EditMask = "n0";
							integerEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							integerEdit.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = integerEdit;
							break;

						case GridEditorFormat.Decimal1Place:
							RepositoryItemTextEdit decimal1Place = new RepositoryItemTextEdit();

							decimal1Place.Mask.EditMask = "n1";
							decimal1Place.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							decimal1Place.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = decimal1Place;

							break;

						case GridEditorFormat.Decimal2Places:
							RepositoryItemTextEdit decimal2Place = new RepositoryItemTextEdit();

							decimal2Place.Mask.EditMask = "n2";
							decimal2Place.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							decimal2Place.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = decimal2Place;

							break;

						case GridEditorFormat.Decimal3Places:
							RepositoryItemTextEdit decimal3Place = new RepositoryItemTextEdit();

							decimal3Place.Mask.EditMask = "n3";
							decimal3Place.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							decimal3Place.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = decimal3Place;

							break;

						case GridEditorFormat.Decimal4Places:
							RepositoryItemTextEdit decimal4Place = new RepositoryItemTextEdit();

							decimal4Place.Mask.EditMask = "n4";
							decimal4Place.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							decimal4Place.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = decimal4Place;

							break;

						case GridEditorFormat.Percentage:
							RepositoryItemTextEdit percentage = new RepositoryItemTextEdit();

							percentage.Mask.EditMask = "##0.####\"%\"";
							percentage.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage;

							break;

						case GridEditorFormat.Percentage1Decimal:
							RepositoryItemTextEdit percentage1Decimal = new RepositoryItemTextEdit();

							percentage1Decimal.Mask.EditMask = "##0.0\"%\"";
							percentage1Decimal.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage1Decimal.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage1Decimal;

							break;

						case GridEditorFormat.Percentage2Decimals:
							RepositoryItemTextEdit percentage2Decimal = new RepositoryItemTextEdit();

							percentage2Decimal.Mask.EditMask = "##0.00\"%\"";
							percentage2Decimal.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage2Decimal.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage2Decimal;

							break;

						case GridEditorFormat.Percentage3Decimals:
							RepositoryItemTextEdit percentage3Decimal = new RepositoryItemTextEdit();

							percentage3Decimal.Mask.EditMask = "##0.000\"%\"";
							percentage3Decimal.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage3Decimal.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage3Decimal;

							break;

						case GridEditorFormat.Percentage4Decimals:
							RepositoryItemTextEdit percentage4Decimal = new RepositoryItemTextEdit();

							percentage4Decimal.Mask.EditMask = "##0.0000\"%\"";
							percentage4Decimal.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage4Decimal.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage4Decimal;

							break;

						case GridEditorFormat.Decimal1PlaceWithPlaceholder:
							RepositoryItemTextEdit percentage1DecimalWithPlaceholder = new RepositoryItemTextEdit();

							percentage1DecimalWithPlaceholder.Mask.EditMask = "##0.#";
							percentage1DecimalWithPlaceholder.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage1DecimalWithPlaceholder.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage1DecimalWithPlaceholder;

							break;

						case GridEditorFormat.Decimal2PlacesWithPlaceholder:
							RepositoryItemTextEdit percentage2DecimalWithPlaceholder = new RepositoryItemTextEdit();

							percentage2DecimalWithPlaceholder.Mask.EditMask = "##0.##";
							percentage2DecimalWithPlaceholder.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage2DecimalWithPlaceholder.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage2DecimalWithPlaceholder;

							break;

						case GridEditorFormat.Decimal3PlacesWithPlaceholder:
							RepositoryItemTextEdit percentage3DecimalWithPlaceholder = new RepositoryItemTextEdit();

							percentage3DecimalWithPlaceholder.Mask.EditMask = "##0.###";
							percentage3DecimalWithPlaceholder.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage3DecimalWithPlaceholder.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage3DecimalWithPlaceholder;

							break;

						case GridEditorFormat.Decimal4PlacesWithPlaceholder:
							RepositoryItemTextEdit percentage4DecimalWithPlaceholder = new RepositoryItemTextEdit();

							percentage4DecimalWithPlaceholder.Mask.EditMask = "##0.####";
							percentage4DecimalWithPlaceholder.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
							percentage4DecimalWithPlaceholder.Mask.UseMaskAsDisplayFormat = true;

							((GridView)view).Columns[columnFieldName].ColumnEdit = percentage4DecimalWithPlaceholder;

							break;

						case GridEditorFormat.Multiline:
							//RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();
							//memo.WordWrap = true;
							//((GridView)view).OptionsView.RowAutoHeight = true;
							//((GridView)view).Columns[columnFieldName].ColumnEdit = memo;

							//// Go through each column and make sure they're aligned properly
							//foreach (GridColumn col in ((GridView)view).Columns)
							//{
							//    col.AppearanceCell.TextOptions.VAlignment = VertAlignment.Top;
							//}
							break;
					}
				}
			}
		}

		/// <summary>
		/// Set the display format for the specified columns
		/// </summary>
		/// <param name="columnFieldName">The column to set the display for</param>
		/// <param name="format">The format to show</param>
		public void SetDisplayFormat(string columnFieldName, GridDisplayFormat format)
		{
			this.SetDisplayFormat(this.MainView, columnFieldName, format);
		}

		/// <summary>
		/// Set the display format for the specified columns
		/// </summary>
		/// <param name="columnFieldName">The column to set the display for</param>
		/// <param name="format">The format to show</param>
		public void SetDisplayFormat(BaseView view, string columnFieldName, GridDisplayFormat format)
		{
			if (view is GridView)
			{
				if (!ViewContainsFieldName((GridView)view, columnFieldName))
				{
					// The column doesn't exist in the collection
					throw new Exception("The column \"" + columnFieldName + "\" does not exist in the current grid.");
				}
				else
				{
					// Set the editor format on the given column
					switch (format)
					{
						case GridDisplayFormat.PhoneNumber:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = @"(###) ###-####";
							break;

						case GridDisplayFormat.SSN:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = @"###-##-####";
							break;

						case GridDisplayFormat.Currency:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "c2";
							break;

						case GridDisplayFormat.CurrencyNoRounding:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "\"$\"###,###,###,##0.00#########";
							break;

						case GridDisplayFormat.ConciseDateTime:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "MM/dd/yy HH:mm";
							break;

						case GridDisplayFormat.DateTime:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = mgCustom.Utils.GetDateAndTimeFormatAsString();
							break;

						case GridDisplayFormat.Date:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = mgCustom.Utils.GetDateFormatAsString();
							break;

						case GridDisplayFormat.Time:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = mgCustom.Utils.GetTimeFormatAsString();
							break;

						case GridDisplayFormat.FullDateTime:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Custom;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "MM/dd/yyyy hh:mm:ss tt";
							break;

						case GridDisplayFormat.Integer:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "n0";
							break;

						case GridDisplayFormat.Decimal1Place:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "n1";
							break;

						case GridDisplayFormat.Decimal2Places:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "n2";
							break;

						case GridDisplayFormat.Decimal3Places:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "n3";
							break;

						case GridDisplayFormat.Decimal4Places:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "n4";
							break;

						case GridDisplayFormat.Percentage:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.####\"%\"";
							break;

						case GridDisplayFormat.Percentage1Decimal:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.0\"%\"";
							break;

						case GridDisplayFormat.Percentage2Decimals:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.00\"%\"";
							break;

						case GridDisplayFormat.Percentage3Decimals:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.000\"%\"";
							break;

						case GridDisplayFormat.Percentage4Decimals:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.0000\"%\"";
							break;

						case GridDisplayFormat.Decimal1PlaceWithPlaceholder:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.#";
							break;

						case GridDisplayFormat.Decimal2PlacesWithPlaceholder:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.##";
							break;

						case GridDisplayFormat.Decimal3PlacesWithPlaceholder:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.###";
							break;

						case GridDisplayFormat.Decimal4PlacesWithPlaceholder:
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatType = FormatType.Numeric;
							((GridView)view).Columns[columnFieldName].DisplayFormat.FormatString = "##0.####";
							break;

						case GridDisplayFormat.Multiline:
							RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();
							memo.WordWrap = true;
							((GridView)view).OptionsView.RowAutoHeight = true;
							((GridView)view).Columns[columnFieldName].ColumnEdit = memo;
							
							// Go through each column and make sure they're aligned properly
							foreach (GridColumn col in ((GridView)view).Columns)
							{
								col.AppearanceCell.TextOptions.VAlignment = VertAlignment.Top;
							}
							break;
					}
				}
			}
		}

		/// <summary>
		/// Turns on or off the detail buttons
		/// </summary>
		public void SetDetailButtons()
		{
			this.SetDetailButtons(this.MainView);
		}

		/// <summary>
		/// Turns on or off the detail buttons
		/// </summary>
		public void SetDetailButtons(BaseView view)
		{
			if (view is GridView)
			{
				((GridView)view).OptionsDetail.ShowDetailTabs = _showDetailButtons;
				((GridView)view).OptionsView.ShowDetailButtons = _showDetailButtons;
			}
		}

		/// <summary>
		/// Set the grouping on the grid
		/// </summary>
		public void SetGrouping()
		{
			this.SetGrouping(this.MainView);
		}

		/// <summary>
		/// Set the grouping on the grid
		/// </summary>
		public void SetGrouping(BaseView view)
		{
			if (view is GridView)
			{
				((GridView)view).OptionsView.ShowGroupPanel = _allowGrouping;
				((GridView)view).OptionsCustomization.AllowGroup = _allowGrouping;
			}
		}

		/// <summary>
		/// Align all the date columns in the set to be center aligned
		/// </summary>
		public void AlignDateColumns()
		{
			this.AlignDateColumns(this.MainView);
		}

		/// <summary>
		/// Align all the date columns in the set to be center aligned
		/// </summary>
		public void AlignDateColumns(BaseView view)
		{
			if (view is GridView)
			{
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					//if (col.ColumnType == typeof(System.DateTime))		// This doesn't work if the column is of a nullable type
					if (col.ColumnType.ToString().Contains("System.DateTime"))
					{
						col.AppearanceCell.TextOptions.VAlignment = VertAlignment.Top;
						col.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
					}
				}
			}
		}

		/// <summary>
		/// Align all the number columns to be right justified
		/// </summary>
		public void AlignNumberColumns()
		{
			this.AlignNumberColumns(this.MainView);
		}

		/// <summary>
		/// Align all the number columns to be right justified
		/// </summary>
		public void AlignNumberColumns(BaseView view)
		{
			if (view is GridView)
			{
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					//if (col.ColumnType == typeof(System.Decimal) ||
					//    col.ColumnType == typeof(System.Int16) ||
					//    col.ColumnType == typeof(System.Int32) ||
					//    col.ColumnType == typeof(System.Int64) ||
					//    col.ColumnType == typeof(System.Double))
					if (col.ColumnType.ToString().Contains("System.Decimal") ||
						col.ColumnType.ToString().Contains("System.Int16") ||
						col.ColumnType.ToString().Contains("System.Int32") ||
						col.ColumnType.ToString().Contains("System.Int64") ||
						col.ColumnType.ToString().Contains("System.Double"))
					{
						col.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
					}
				}
			}
		}
		#endregion Random Functions

		#region SetEditor
		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemComboBox(string columnFieldName, List<string> itemList)
		{
			this.SetEditor_RepositoryItemComboBox(this.MainView, columnFieldName, itemList);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemComboBox(BaseView view, string columnFieldName, List<string> itemList)
		{
			RepositoryItemComboBox cbo = new RepositoryItemComboBox();
			cbo.Items.Clear();
			foreach (string s in itemList)
			{
				cbo.Items.Add(s);
			}
			cbo.ImmediatePopup = true;
			SetEditor_RepositoryItemComboBox(view, columnFieldName, cbo);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemComboBox(string columnFieldName, RepositoryItemComboBox cbo)
		{
			this.SetEditor_RepositoryItemComboBox(this.MainView, columnFieldName, cbo);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemComboBox(BaseView view, string columnFieldName, RepositoryItemComboBox cbo)
		{
			if (view is GridView)
			{
				if (!ViewContainsFieldName((GridView)view, columnFieldName))
				{
					// The column doesn't exist in the collection
					throw new Exception("The column \"" + columnFieldName + "\" does not exist in the current grid.");
				}
				else
				{
					((GridView)view).Columns[columnFieldName].ColumnEdit = cbo;
				}
			}
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(string columnFieldName, List<string> itemList, string columnCaption)
		{
			this.SetEditor_RepositoryItemLookUpEdit(this.MainView, columnFieldName, itemList, columnCaption);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(BaseView view, 
			string columnFieldName, 
			List<string> itemList,
			string columnCaption)
		{
			RepositoryItemLookUpEdit lue = new RepositoryItemLookUpEdit();

			DataTable dt = new DataTable();
			dt.Columns.Add("Key", typeof(System.String));
			dt.Columns.Add("Value", typeof(System.String));
			dt.Columns.Add("ExtendedValue", typeof(System.String));

			foreach (string s in itemList)
			{
				DataRow newRow = dt.NewRow();
				newRow["Key"] = s;
				newRow["Value"] = s;
				dt.Rows.Add(newRow);
			}

			lue.DataSource = dt;
            lue.DisplayMember = "Value";
            lue.ValueMember = "Key";
            lue.PopulateColumns();
            lue.Columns["ExtendedValue"].Visible = false;
            lue.Columns["Key"].Visible = false;
            lue.ImmediatePopup = true;
			lue.PopupWidth = 300;
			//lue.ShowFooter = false;
			//lue.ShowHeader = false;
            lue.NullText = string.Empty;
			SetEditor_RepositoryItemLookUpEdit(view, columnFieldName, lue, columnCaption, columnCaption);
		}

		///// <summary>
		///// Set an editor up on a column
		///// </summary>
		//public void SetEditor_RepositoryItemLookUpEdit(string columnFieldName, 
		//    GenericListItemCollection itemList,
		//    string columnCaption0,
		//    string columnCaption1)
		//{
		//    this.SetEditor_RepositoryItemLookUpEdit(this.MainView, columnFieldName, itemList, columnCaption0, columnCaption1);
		//}

		///// <summary>
		///// Set an editor up on a column
		///// </summary>
		//public void SetEditor_RepositoryItemLookUpEdit(BaseView view, 
		//    string columnFieldName, 
		//    GenericListItemCollection itemList,
		//    string columnCaption0,
		//    string columnCaption1)
		//{
		//    RepositoryItemLookUpEdit lue = new RepositoryItemLookUpEdit();

		//    DataTable dt = new DataTable();
		//    dt.Columns.Add("Key", typeof(System.String));
		//    dt.Columns.Add("Value", typeof(System.String));
		//    dt.Columns.Add("ExtendedValue", typeof(System.String));

		//    foreach (GenericListItem kvp in itemList)
		//    {
		//        DataRow newRow = dt.NewRow();
		//        newRow["Key"] = kvp.Key;
		//        newRow["Value"] = kvp.Value;
		//        newRow["ExtendedValue"] = kvp.ExtendedValue;
		//        dt.Rows.Add(newRow);
		//    }

		//    lue.DataSource = dt;
		//    lue.DisplayMember = "Value";
		//    lue.ValueMember = "Key";
		//    lue.PopulateColumns();
		//    lue.Columns["ExtendedValue"].Visible = false;
		//    lue.Columns["Key"].Visible = false;
		//    lue.ImmediatePopup = true;
		//    lue.PopupWidth = 300;
		//    //lue.ShowFooter = false;
		//    //lue.ShowHeader = false;
		//    lue.NullText = string.Empty;
		//    SetEditor_RepositoryItemLookUpEdit(view, columnFieldName, lue, columnCaption0, columnCaption1);
		//}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(string columnFieldName, 
			Dictionary<string, string> itemList,
			string columnCaption0,
			string columnCaption1)
		{
			this.SetEditor_RepositoryItemLookUpEdit(this.MainView, columnFieldName, itemList, columnCaption0, columnCaption1);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(BaseView view, 
			string columnFieldName,
			Dictionary<string, string> itemList,
			string columnCaption0,
			string columnCaption1)
		{
			RepositoryItemLookUpEdit lue = new RepositoryItemLookUpEdit();

			DataTable dt = new DataTable();
			dt.Columns.Add("Key", typeof(System.String));
			dt.Columns.Add("Value", typeof(System.String));
			dt.Columns.Add("ExtendedValue", typeof(System.String));

			foreach (KeyValuePair<string, string> kvp in itemList)
			{
				DataRow newRow = dt.NewRow();
				newRow["Key"] = kvp.Key;
				newRow["Value"] = kvp.Value;
				dt.Rows.Add(newRow);
			}

			lue.DataSource = dt;
			lue.DisplayMember = "Value";
			lue.ValueMember = "Key";
			lue.PopulateColumns();
			lue.Columns["ExtendedValue"].Visible = false;
			lue.Columns["Key"].Visible = false;
			lue.ImmediatePopup = true;
			lue.PopupWidth = 300;
			//lue.ShowFooter = false;
			//lue.ShowHeader = false;
			lue.NullText = string.Empty;
			SetEditor_RepositoryItemLookUpEdit(view, columnFieldName, lue, columnCaption0, columnCaption1);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(string columnFieldName, 
			Dictionary<string, int> itemList,
			string columnCaption0,
			string columnCaption1)
		{
			this.SetEditor_RepositoryItemLookUpEdit(this.MainView, columnFieldName, itemList, columnCaption0, columnCaption1);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(BaseView view, 
			string columnFieldName, 
			Dictionary<string, int> itemList,
			string columnCaption0,
			string columnCaption1)
		{
			RepositoryItemLookUpEdit lue = new RepositoryItemLookUpEdit();

			DataTable dt = new DataTable();
			dt.Columns.Add("Key", typeof(System.String));
			dt.Columns.Add("Value", typeof(System.Int64));
			dt.Columns.Add("ExtendedValue", typeof(System.String));

			foreach (KeyValuePair<string, int> kvp in itemList)
			{
				DataRow newRow = dt.NewRow();
				newRow["Key"] = kvp.Key;
				newRow["Value"] = kvp.Value;
				dt.Rows.Add(newRow);
			}

			lue.DataSource = dt;
			lue.DisplayMember = "Value";
			lue.ValueMember = "Key";
			lue.PopulateColumns();
			lue.Columns["ExtendedValue"].Visible = false;
			lue.Columns["Key"].Visible = false;
			lue.ImmediatePopup = true;
			lue.PopupWidth = 300;
			//lue.ShowFooter = false;
			//lue.ShowHeader = false;
			lue.NullText = string.Empty;

			//lue.Items.Clear();
			//foreach (KeyValuePair<string, int> kvp in itemList)
			//{
			//    lue.Items.Add(new GenericListItem(kvp.Key, kvp.Value.ToString()));
			//}
			SetEditor_RepositoryItemLookUpEdit(view, columnFieldName, lue, columnCaption0, columnCaption1);
		}

		/// <summary>
		/// Set an editor up on a column
		/// </summary>
		public void SetEditor_RepositoryItemLookUpEdit(BaseView view, 
			string columnFieldName,
			RepositoryItemLookUpEdit lue, 
			string columnCaption0,
			string columnCaption1)
		{
			if (view is GridView)
			{
				if (!ViewContainsFieldName((GridView)view, columnFieldName))
				{
					// The column doesn't exist in the collection
					throw new Exception("The column \"" + columnFieldName + "\" does not exist in the current grid.");
				}
				else
				{
					lue.Columns[0].Caption = columnCaption0;
					if (lue.Columns.Count > 1) { lue.Columns[1].Caption = columnCaption1; }
					((GridView)view).Columns[columnFieldName].ColumnEdit = lue;
				}
			}
		}
		#endregion SetEditor

		#region SetCaptions
		/// <summary>
		/// Set the captions on the columns
		/// First column should be column name and second column should be column caption
		/// </summary>
		/// <param name="captions">The captions collection</param>
		public void SetCaptions(Dictionary<string, string> captions)
		{
			SetCaptions(this.MainView, captions);
		}

		/// <summary>
		/// Set the captions on the columns
		/// First column should be column name and second column should be column caption
		/// </summary>
		/// <param name="view">The view to use</param>
		/// <param name="captions">The captions collection</param>
		public void SetCaptions(BaseView view, Dictionary<string, string> captions)
		{
			if (view is GridView)
			{
				foreach (KeyValuePair<string, string> kvp in captions)
				{
					if (!ViewContainsFieldName((GridView)view, kvp.Key))
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + kvp.Key + "\" does not exist in the current grid.");
					}
					else
					{
						((GridView)view).Columns[kvp.Key].Caption = kvp.Value;
						_setCaptions.Add(kvp.Key);
					}
				}
			}
		}

		/// <summary>
		/// Sets the captions on all fields based on the field names in the collection
		/// </summary>
		public void SetCaptionsAuto()
		{
			SetCaptionsAuto(this.MainView);
		}

		/// <summary>
		/// Sets the captions on all fields based on the field names in the collection
		/// </summary>
		public void SetCaptionsAuto(BaseView view)
		{
			if (view is GridView)
			{
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					if (!_setCaptions.Contains(col.FieldName))
					{
						// Set the caption
						col.Caption = mgCustom.Utils.AlterCaptionFromFieldName(col.FieldName);
					}
				}
			}
		}
		#endregion SetCaptions

		#region SetCheckColumnSizeBasedOnCaption
		/// <summary>
		/// Set the check column sizes based on their captions
		/// </summary>
		public void SetCheckColumnSizeBasedOnCaption()
		{
			SetCheckColumnSizeBasedOnCaption(this.MainView);
		}

		/// <summary>
		/// Set the check column sizes based on their captions
		/// </summary>
		/// <param name="view">The view to set it on</param>
		public void SetCheckColumnSizeBasedOnCaption(BaseView view)
		{
			if (view is GridView)
			{
				GridView vw = view as GridView;
				foreach (GridColumn col in vw.Columns)
				{
					if (((col.ColumnType.IsGenericType &&
							col.ColumnType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
							System.Type.GetType(col.ColumnType.GetGenericArguments()[0].FullName) == typeof(System.Boolean)) ||
						col.ColumnType == typeof(System.Boolean)) &&
						col.FieldName != "CheckedInGrid")
					{
						vw.Columns[col.FieldName].OptionsColumn.FixedWidth = true;
						vw.Columns[col.FieldName].Width =
							(int)TextRenderer.MeasureText(vw.Columns[col.FieldName].Caption,
							vw.Columns[col.FieldName].AppearanceHeader.Font).Width + 36;
					}
				}
			}
		}
		#endregion SetCheckColumnSizeBasedOnCaption

		#region GroupColumns
		/// <summary>
		/// Group the columns in the grid
		/// </summary>
		public void GroupColumns(string[] columns)
		{
			GroupColumns(this.MainView, columns);
		}

		/// <summary>
		/// Group the columns in the grid
		/// </summary>
		public void GroupColumns(BaseView view, string[] columns)
		{
			if (view is GridView)
			{
				((GridView)view).ClearGrouping();		// Clear the grouping on the grid
				this.AllowGroup = true;
				int count = 0;
				for (int i = 0; i < columns.Length; i++)
				{
					if (!String.IsNullOrEmpty(columns[i].Trim()))
					{
						foreach (GridColumn c in ((GridView)view).Columns)
						{
							if (c.FieldName.ToLower() == columns[i].ToLower())
							{
								c.GroupIndex = count;
								count++;
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Group the columns in the grid
		/// </summary>
		public void GroupColumns(GridColumn[] columns)
		{
			GroupColumns(this.MainView, columns);
		}

		/// <summary>
		/// Group the columns in the grid
		/// </summary>
		public void GroupColumns(BaseView view, GridColumn[] columns)
		{
			// Reset all the columns to be invisible
			List<string> cols = new List<string>();
			for (int i = 0; i < columns.Length; i++)
			{
				cols.Add(columns[i].FieldName);
			}
			GroupColumns(view, cols.ToArray());
		}
		#endregion GroupColumns

		#region SetVisibleColumns
		/// <summary>
		/// Show all the columns
		/// </summary>
		public void SetVisibleColumns()
		{
			SetVisibleColumns(this.MainView);
		}

		/// <summary>
		/// Show all the columns
		/// </summary>
		public void SetVisibleColumns(BaseView view)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				SetVisibleColumns(view, cols.ToArray());		// Call the other to visible columns method
				SetDetailButtons(view);		// Set the detail buttons
			}

			AlignDateColumns(view);			// Align the date time columns
			AlignNumberColumns(view);		// Align the number columns
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetVisibleColumns(string[] columns)
		{
			SetVisibleColumns(this.MainView, columns);
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetVisibleColumns(BaseView view, string[] columns)
		{
			// Reset all the columns to be invisible
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				SetHiddenColumns(view);		// Hide all the columns first

				if (((GridView)view).Columns.Count == 0) { return; }

				// Show the columns in the list
				int count = 0;
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((GridView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.ShowInCustomizationForm = true;
							col.VisibleIndex = ++count;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					ResizeCheckedInGridColumn(view);		// Run the local method
				}
			}
			else if (view is CardView)
			{
				SetHiddenColumns(view);		// Hide all the columns first

				if (((CardView)view).Columns.Count == 0) { return; }

				// Show the columns in the list
				int count = 0;
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((CardView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.ShowInCustomizationForm = true;
							col.VisibleIndex = ++count;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (((CardView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					ResizeCheckedInGridColumn(view);		// Run the local method
				}
			}
			else if (view is LayoutView)
			{
				SetHiddenColumns(view);		// Hide all the columns first

				if (((LayoutView)view).Columns.Count == 0) { return; }

				List<LayoutViewField> fields = new List<LayoutViewField>();

				((LayoutView)view).TemplateCard.Clear();

				// Show the columns in the list
				int count = 0;
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (LayoutViewColumn col in ((LayoutView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.ShowInCustomizationForm = true;
							col.VisibleIndex = ++count;
							col.LayoutViewField.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
							fields.Add(col.LayoutViewField);
							exists = true;

							// Set up the fields in layout view
							((LayoutView)view).TemplateCard.Add(col.LayoutViewField);
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				//SetDetailButtons(view);		// Set the detail buttons

				//if (((LayoutView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				//{
				//    ResizeCheckedInGridColumn(view);		// Run the local method
				//}

				//((LayoutView)view).CardMinSize = new Size(350, 200);
			}

			if (!(view is LayoutView))
			{
				AlignDateColumns(view);			// Align the date time columns
				AlignNumberColumns(view);		// Align the number columns
			}
		}

		/// <summary>
		/// Resize the checked in grid column
		/// </summary>
		public void ResizeCheckedInGridColumn()
		{
			ResizeCheckedInGridColumn(this.MainView);
		}

		/// <summary>
		/// Resize the checked in grid column
		/// </summary>
		public void ResizeCheckedInGridColumn(BaseView view)
		{
			if (view is GridView)
			{
				if (((GridView)view).Columns.Count > 0 &&
					((GridView)view).Columns.ColumnByFieldName("CheckedInGrid") != null)
				{
					((GridView)view).Columns["CheckedInGrid"].OptionsColumn.AllowSort = DefaultBoolean.False;
					((GridView)view).Columns["CheckedInGrid"].OptionsFilter.AllowFilter = false;
					((GridView)view).Columns["CheckedInGrid"].OptionsFilter.AllowAutoFilter = false;
					((GridView)view).Columns["CheckedInGrid"].OptionsColumn.ShowCaption = false;
					((GridView)view).Columns["CheckedInGrid"].OptionsColumn.FixedWidth = true;
					((GridView)view).Columns["CheckedInGrid"].Width = 60;
				}
			}
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetVisibleColumns(GridColumn[] columns)
		{
			SetVisibleColumns(this.MainView, columns);
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetVisibleColumns(BaseView view, GridColumn[] columns)
		{
			// Reset all the columns to be invisible
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				SetVisibleColumns(view, cols.ToArray());		// Call the other to visible columns method
				SetDetailButtons(view);		// Set the detail buttons
			}
		}
		#endregion SetVisibleColumns

		#region SetShowInCustomizationForm
		/// <summary>
		/// Show all the columns
		/// </summary>
		public void SetShowInCustomizationForm(bool visible)
		{
			this.SetShowInCustomizationForm(this.MainView, visible);
		}

		/// <summary>
		/// Show all the columns
		/// </summary>
		public void SetShowInCustomizationForm(BaseView view, bool visible)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				SetShowInCustomizationForm(view, cols.ToArray(), visible);		// Call the other to visible columns method
			}
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetShowInCustomizationForm(string[] columns, bool visible)
		{
			SetShowInCustomizationForm(this.MainView, columns, visible);
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetShowInCustomizationForm(BaseView view, string[] columns, bool visible)
		{
			// Reset all the columns to be invisible
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Show the columns in the list
				//int count = 0;
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((GridView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.ShowInCustomizationForm = visible;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}
			}
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetShowInCustomizationForm(GridColumn[] columns, bool visible)
		{
			SetShowInCustomizationForm(this.MainView, columns, visible);
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetShowInCustomizationForm(BaseView view, GridColumn[] columns, bool visible)
		{
			// Reset all the columns to be invisible
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				SetShowInCustomizationForm(view, cols.ToArray(), visible);		// Call the other to visible columns method
			}
		}
		#endregion SetShowInCustomizationForm

		#region SetHiddenColumns
		/// <summary>
		/// Set the hidden columns
		/// </summary>
		public void SetHiddenColumns()
		{
			SetHiddenColumns(this.MainView);
		}

		/// <summary>
		/// Set the hidden columns
		/// </summary>
		public void SetHiddenColumns(BaseView view)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				if (!_isUpdating) { base.BeginUpdate(); }
				for (int colIndex = ((GridView)view).Columns.Count - 1; colIndex >= 0; colIndex--)
				{
					((GridView)view).Columns[colIndex].VisibleIndex = -1;
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (!_isUpdating) { base.EndUpdate(); }
			}
			else if (view is CardView)
			{
				if (!_isUpdating) { base.BeginUpdate(); }
				for (int colIndex = ((CardView)view).Columns.Count - 1; colIndex >= 0; colIndex--)
				{
					((CardView)view).Columns[colIndex].VisibleIndex = -1;
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (!_isUpdating) { base.EndUpdate(); }
			}
			else if (view is LayoutView)
			{
				if (!_isUpdating) { base.BeginUpdate(); }
				for (int colIndex = ((LayoutView)view).Columns.Count - 1; colIndex >= 0; colIndex--)
				{
					((LayoutView)view).Columns[colIndex].LayoutViewField.Visibility = 
						DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
				}

				//SetDetailButtons(view);		// Set the detail buttons

				if (!_isUpdating) { base.EndUpdate(); }
			}
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetHiddenColumns(string[] columns)
		{
			SetHiddenColumns(this.MainView, columns);
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetHiddenColumns(BaseView view, string[] columns)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				if (!_isUpdating) { base.BeginUpdate(); }

				SetVisibleColumns(view);		// Show all the columns

				// Hide the columns in the list
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					for (int colIndex = ((GridView)view).Columns.Count - 1; colIndex >= 0; colIndex--)
					{
						if (((GridView)view).Columns[colIndex].FieldName == columns[i])
						{
							((GridView)view).Columns[colIndex].VisibleIndex = -1;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (!_isUpdating) { base.EndUpdate(); }
			}
			else if (view is CardView)
			{
				if (!_isUpdating) { base.BeginUpdate(); }

				SetVisibleColumns(view);		// Show all the columns

				// Hide the columns in the list
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					for (int colIndex = ((CardView)view).Columns.Count - 1; colIndex >= 0; colIndex--)
					{
						if (((CardView)view).Columns[colIndex].FieldName == columns[i])
						{
							((CardView)view).Columns[colIndex].VisibleIndex = -1;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (!_isUpdating) { base.EndUpdate(); }
			}
			else if (view is LayoutView)
			{
				if (!_isUpdating) { base.BeginUpdate(); }

				SetVisibleColumns(view);		// Show all the columns

				// Hide the columns in the list
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					for (int colIndex = ((LayoutView)view).Columns.Count - 1; colIndex >= 0; colIndex--)
					{
						if (((LayoutView)view).Columns[colIndex].FieldName == columns[i])
						{
							((LayoutView)view).Columns[colIndex].VisibleIndex = -1;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				SetDetailButtons(view);		// Set the detail buttons

				if (!_isUpdating) { base.EndUpdate(); }
			}
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetHiddenColumns(GridColumn[] columns)
		{
			SetHiddenColumns(this.MainView, columns);
		}

		/// <summary>
		/// Set the visible columns using a string array of column field names
		/// If the column specified in the passed array doesn't exist 
		/// in the collection, an error is thrown
		/// </summary>
		public void SetHiddenColumns(BaseView view, GridColumn[] columns)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView ||
				view is CardView ||
				view is LayoutView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				SetHiddenColumns(view, cols.ToArray());		// Call the other to visible columns method

				SetDetailButtons(view);		// Set the detail buttons
			}
		}
		#endregion SetHiddenColumns

		#region AllowEdit
		/// <summary>
		/// Turn on or off the allow edit element of each column
		/// </summary>
		/// <param name="allowed">Tells if editing is allowed</param>
		public void AllowEdit(bool allowed)
		{
			AllowEdit(this.MainView, allowed);
		}

		/// <summary>
		/// Turn on or off the allow edit element of each column
		/// </summary>
		/// <param name="allowed">Tells if editing is allowed</param>
		public void AllowEdit(BaseView view, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Set all the edits
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				AllowEdit(view, cols.ToArray(), allowed);
			}
		}

		/// <summary>
		/// Turn on or off the allow edit element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if editing is allowed</param>
		public void AllowEdit(string[] columns, bool allowed)
		{
			AllowEdit(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow edit element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if editing is allowed</param>
		public void AllowEdit(BaseView view, string[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Do the opposite to all columns first
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					col.OptionsColumn.AllowEdit = !allowed;
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((GridView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowEdit = allowed;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				foreach (GridColumn col in ((GridView)view).Columns)
				{
					if (col.ColumnType.ToString().Contains("System.DateTime") &&
						col.OptionsColumn.AllowEdit)
					{
						if (col.ColumnEdit == null)
						{
							// Create the repository item editor
							//RepositoryItemDateEdit dtpEditor = new RepositoryItemDateEdit();
							mgDevX_RepositoryItemDateEdit dtpEditor = new mgDevX_RepositoryItemDateEdit();
							//dtpEditor.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
							col.ColumnEdit = dtpEditor;
						}
						else
						{
							//if (col.ColumnEdit is RepositoryItemDateEdit)
							//{
							//    ((RepositoryItemDateEdit)col.ColumnEdit).Mask.MaskType =
							//        DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
							//}
						}
					}
				}
			}
			else if (view is LayoutView)
			{
				// Do the opposite to all columns first
				foreach (LayoutViewColumn col in ((LayoutView)view).Columns)
				{
					col.OptionsColumn.AllowEdit = !allowed;
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (LayoutViewColumn col in ((LayoutView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowEdit = allowed;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}

				foreach (LayoutViewColumn col in ((LayoutView)view).Columns)
				{
					if (col.ColumnType.ToString().Contains("System.DateTime") &&
						col.OptionsColumn.AllowEdit)
					{
						if (col.ColumnEdit == null)
						{
							// Create the repository item editor
							mgDevX_RepositoryItemDateEdit dtpEditor = new mgDevX_RepositoryItemDateEdit();
							col.ColumnEdit = dtpEditor;
						}
					}
				}
			}
		}

		/// <summary>
		/// Turn on or off the allow edit element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if editing is allowed</param>
		public void AllowEdit(GridColumn[] columns, bool allowed)
		{
			AllowEdit(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow edit element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if editing is allowed</param>
		public void AllowEdit(BaseView view, GridColumn[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				AllowEdit(view, cols.ToArray(), allowed);		// Call the other to visible columns method
			}
		}
		#endregion AllowEdit

		#region AllowGrouping
		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowGrouping(bool allowed)
		{
			this.AllowGroup = allowed;
			AllowGrouping(this.MainView, allowed);
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowGrouping(BaseView view, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Set all the edits
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				AllowGrouping(view, cols.ToArray(), allowed);
			}
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowGrouping(string[] columns, bool allowed)
		{
			AllowGrouping(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowGrouping(BaseView view, string[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Do the opposite to all columns first
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					col.OptionsColumn.AllowGroup = (!allowed ? DefaultBoolean.True : DefaultBoolean.False);
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((GridView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowGroup = (allowed ? DefaultBoolean.True : DefaultBoolean.False);
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}
			}
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowGrouping(GridColumn[] columns, bool allowed)
		{
			AllowGrouping(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowGrouping(BaseView view, GridColumn[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				AllowGrouping(view, cols.ToArray(), allowed);		// Call the other to visible columns method
			}
		}
		#endregion AllowGrouping

		#region AllowSort
		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowSort(bool allowed)
		{
			AllowSort(this.MainView, allowed);
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowSort(BaseView view, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Set all the edits
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				AllowSort(view, cols.ToArray(), allowed);
			}
			else if (view is CardView)
			{
				// Set all the edits
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((CardView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				AllowSort(view, cols.ToArray(), allowed);
			}
			else if (view is LayoutView)
			{
				// Set all the edits
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((LayoutView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				AllowSort(view, cols.ToArray(), allowed);
			}
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowSort(string[] columns, bool allowed)
		{
			AllowSort(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowSort(BaseView view, string[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Do the opposite to all columns first
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					col.OptionsColumn.AllowSort = (!allowed ? DefaultBoolean.True : DefaultBoolean.False);
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((GridView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowSort = (allowed ? DefaultBoolean.True : DefaultBoolean.False);
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}
			}
			else if (view is CardView)
			{
				// Do the opposite to all columns first
				foreach (GridColumn col in ((CardView)view).Columns)
				{
					col.OptionsColumn.AllowSort = (!allowed ? DefaultBoolean.True : DefaultBoolean.False);
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((CardView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowSort = (allowed ? DefaultBoolean.True : DefaultBoolean.False);
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}
			}
			else if (view is LayoutView)
			{
				// Do the opposite to all columns first
				foreach (GridColumn col in ((LayoutView)view).Columns)
				{
					col.OptionsColumn.AllowSort = (!allowed ? DefaultBoolean.True : DefaultBoolean.False);
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((LayoutView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowSort = (allowed ? DefaultBoolean.True : DefaultBoolean.False);
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}
			}
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowSort(GridColumn[] columns, bool allowed)
		{
			AllowSort(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow grouping element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if grouping is allowed</param>
		public void AllowSort(BaseView view, GridColumn[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView ||
				view is CardView ||
				view is LayoutView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				AllowSort(view, cols.ToArray(), allowed);		// Call the other to visible columns method
			}
		}
		#endregion AllowSort

		#region AllowMove
		/// <summary>
		/// Turn on or off the allow moving of columns element of each column
		/// </summary>
		/// <param name="allowed">Tells if moving of columns is allowed</param>
		public void AllowMove(bool allowed)
		{
			AllowMove(this.MainView, allowed);
		}

		/// <summary>
		/// Turn on or off the allow moving of columns element of each column
		/// </summary>
		/// <param name="allowed">Tells if moving of columns is allowed</param>
		public void AllowMove(BaseView view, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Set all the edits
				List<string> cols = new List<string>();
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					cols.Add(col.FieldName);
				}
				AllowMove(view, cols.ToArray(), allowed);
			}
		}


		/// <summary>
		/// Turn on or off the allow moving of columns element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if moving of columns is allowed</param>
		public void AllowMove(string[] columns, bool allowed)
		{
			AllowMove(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow moving of columns element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if moving of columns is allowed</param>
		public void AllowMove(BaseView view, string[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				// Do the opposite to all columns first
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					col.OptionsColumn.AllowMove = !allowed;
				}

				// Now do the action
				bool exists = false;
				for (int i = 0; i < columns.Length; i++)
				{
					exists = false;
					foreach (GridColumn col in ((GridView)view).Columns)
					{
						if (col.FieldName == columns[i])
						{
							col.OptionsColumn.AllowMove = allowed;
							exists = true;
						}
					}
					if (!exists)
					{
						// The column doesn't exist in the collection
						throw new Exception("The column \"" + columns[i] +
							"\" does not exist in the current grid.");
					}
				}
			}
		}

		/// <summary>
		/// Turn on or off the allow moving of columns element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if moving of columns is allowed</param>
		public void AllowMove(GridColumn[] columns, bool allowed)
		{
			AllowMove(this.MainView, columns, allowed);
		}

		/// <summary>
		/// Turn on or off the allow moving of columns element of each column
		/// </summary>
		/// <param name="columns">The columns collection to work with</param>
		/// <param name="allowed">Tells if moving of columns is allowed</param>
		public void AllowMove(BaseView view, GridColumn[] columns, bool allowed)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				List<string> cols = new List<string>();
				for (int i = 0; i < columns.Length; i++)
				{
					cols.Add(columns[i].FieldName);
				}
				AllowMove(view, cols.ToArray(), allowed);		// Call the other to visible columns method
			}
		}
		#endregion AllowMove

		#region GroupSummaryItems
		/// <summary>
		/// Sets a group summary item on all columns in the view
		/// </summary>
		public void SetGroupSummaryItems()
		{
			SetGroupSummaryItems(this.MainView);
		}

		/// <summary>
		/// Sets a group summary item on all columns in the view
		/// </summary>
		/// <param name="view">The target view</param>
		public void SetGroupSummaryItems(BaseView view)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				((GridView)view).GroupSummary.Clear();
				((GridView)view).GroupSummary.Add(DevExpress.Data.SummaryItemType.Count, ((GridView)view).Columns[0].FieldName);
			}
		}
		#endregion GroupSummaryItems

		#region FormatHeaders
		/// <summary>
		/// Format the headers in the grid
		/// </summary>
		public void FormatHeaders()
		{
			FormatHeaders(this.MainView);
		}

		/// <summary>
		/// Format the headers in the grid
		/// </summary>
		public void FormatHeaders(BaseView view)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				if (((GridView)view).Columns.Count > 0)
				{
					//AppearanceObject appearance = ((GridView)view).Columns[0].AppearanceHeader;
					AppearanceObject appearance = ((GridView)view).Appearance.HeaderPanel;
					appearance.TextOptions.HAlignment = HorzAlignment.Center;
					//appearance.Font = new Font(appearance.Font.Name, 
					//    (appearance.FontHeight > 0 ? (appearance.FontHeight < 50 ? appearance.FontHeight : 8.25F) : 8.25F), 
					//    FontStyle.Bold);
					appearance.Font = new Font(appearance.Font.Name, appearance.Font.Size, FontStyle.Bold);
					FormatHeaders(view, appearance);
				}
			}
		}

		/// <summary>
		/// Format the headers in the grid
		/// </summary>
		public void FormatHeaders(AppearanceObject appearance)
		{
			FormatHeaders(this.MainView, appearance);
		}

		/// <summary>
		/// Format the headers in the grid
		/// </summary>
		public void FormatHeaders(BaseView view, AppearanceObject appearance)
		{
			if (this.DataSource == null) { return; }
			if (view is GridView)
			{
				foreach (GridColumn col in ((GridView)view).Columns)
				{
					col.AppearanceHeader.TextOptions.HAlignment = appearance.TextOptions.HAlignment;
					col.AppearanceHeader.TextOptions.HotkeyPrefix = appearance.TextOptions.HotkeyPrefix;
					col.AppearanceHeader.TextOptions.Trimming = appearance.TextOptions.Trimming;
					col.AppearanceHeader.TextOptions.VAlignment = appearance.TextOptions.VAlignment;
					col.AppearanceHeader.TextOptions.WordWrap = appearance.TextOptions.WordWrap;

					col.AppearanceHeader.Font = appearance.Font;
				}
			}
		}
		#endregion FormatHeaders

		#region GroupFormat
		/// <summary>
		/// The default is "{0}: [#image]{1} {2}"
		/// The text pattern can include static text plus predefined placeholders: {0}, {1}, {2} and [#image]. 
		/// The {0} character sequence is the placeholder for the grouping column's caption. The [#image] sequence 
		///		is the placeholder for the image which is displayed within the group row if data is grouped by a 
		///		column that uses an image combobox in-place editor. 
		/// The {1} sequence is the placeholder for the grouping column's display value which corresponds to the 
		///		current group row. The column's display value is a formatted representation of the column's raw data
		/// </summary>
		/// <param name="format">The format string to apply</param>
		public void SetGroupFormat(string format)
		{
			SetGroupFormat(this.MainView, format);
		}

		/// <summary>
		/// The default is "{0}: [#image]{1} {2}"
		/// The text pattern can include static text plus predefined placeholders: {0}, {1}, {2} and [#image]. 
		/// The {0} character sequence is the placeholder for the grouping column's caption. The [#image] sequence 
		///		is the placeholder for the image which is displayed within the group row if data is grouped by a 
		///		column that uses an image combobox in-place editor. 
		/// The {1} sequence is the placeholder for the grouping column's display value which corresponds to the 
		///		current group row. The column's display value is a formatted representation of the column's raw data
		/// </summary>
		/// <param name="format">The format string to use</param>
		/// <param name="view">The view to use</param>
		public void SetGroupFormat(BaseView view, string format)
		{
			// Reset the group formatting
			if (view is GridView)
			{
				((GridView)view).GroupFormat = format;
			}
		}

		///// <summary>
		///// The default is "{0}: [#image]{1} {2}"
		///// The text pattern can include static text plus predefined placeholders: {0}, {1}, {2} and [#image]. 
		///// The {0} character sequence is the placeholder for the grouping column's caption. The [#image] sequence 
		/////		is the placeholder for the image which is displayed within the group row if data is grouped by a 
		/////		column that uses an image combobox in-place editor. 
		///// The {1} sequence is the placeholder for the grouping column's display value which corresponds to the 
		/////		current group row. The column's display value is a formatted representation of the column's raw data
		///// </summary>
		///// <param name="columnName">The column to apply the formatting to</param>
		///// <param name="format">The format string to use</param>
		//public void SetGroupFormatColumn(string columnName, string format)
		//{
		//    SetGroupFormatColumn(this.MainView, columnName, format);
		//}

		///// <summary>
		///// The default is "{0}: [#image]{1} {2}"
		///// The text pattern can include static text plus predefined placeholders: {0}, {1}, {2} and [#image]. 
		///// The {0} character sequence is the placeholder for the grouping column's caption. The [#image] sequence 
		/////		is the placeholder for the image which is displayed within the group row if data is grouped by a 
		/////		column that uses an image combobox in-place editor. 
		///// The {1} sequence is the placeholder for the grouping column's display value which corresponds to the 
		/////		current group row. The column's display value is a formatted representation of the column's raw data
		///// </summary>
		///// <param name="columnName">The column to apply the formatting to</param>
		///// <param name="format">The format string to use</param>
		///// <param name="view">The view to use</param>
		//public void SetGroupFormatColumn(BaseView view, string columnName, string format)
		//{
		//    // Reset the group formatting for the column
		//    if (view is GridView)
		//    {
		//        GridColumn col = ((GridView)view).Columns[columnName];
		//        SetGroupFormatColumn(view, col, format);
		//    }
		//}

		///// <summary>
		///// The default is "{0}: [#image]{1} {2}"
		///// The text pattern can include static text plus predefined placeholders: {0}, {1}, {2} and [#image]. 
		///// The {0} character sequence is the placeholder for the grouping column's caption. The [#image] sequence 
		/////		is the placeholder for the image which is displayed within the group row if data is grouped by a 
		/////		column that uses an image combobox in-place editor. 
		///// The {1} sequence is the placeholder for the grouping column's display value which corresponds to the 
		/////		current group row. The column's display value is a formatted representation of the column's raw data
		///// </summary>
		///// <param name="column">The column to apply the formatting to</param>
		///// <param name="format">The format string to use</param>
		//public void SetGroupFormatColumn(GridColumn column, string format)
		//{
		//    SetGroupFormatColumn(this.MainView, column, format);
		//}

		///// <summary>
		///// The default is "{0}: [#image]{1} {2}"
		///// The text pattern can include static text plus predefined placeholders: {0}, {1}, {2} and [#image]. 
		///// The {0} character sequence is the placeholder for the grouping column's caption. The [#image] sequence 
		/////		is the placeholder for the image which is displayed within the group row if data is grouped by a 
		/////		column that uses an image combobox in-place editor. 
		///// The {1} sequence is the placeholder for the grouping column's display value which corresponds to the 
		/////		current group row. The column's display value is a formatted representation of the column's raw data
		///// </summary>
		///// <param name="column">The column to apply the formatting to</param>
		///// <param name="format">The format string to use</param>
		///// <param name="view">The view to use</param>
		//public void SetGroupFormatColumn(BaseView view, GridColumn column, string format)
		//{
		//    // Reset the group formatting for the column
		//    if (view is GridView)
		//    {
		//        column.GroupFormat = format;
		//    }
		//}
		#endregion GroupFormat

		#region SetCaptionsBasedOnScreenOverrides
		/// <summary>
		/// Set the captions based on the screen caption overrides in the database
		/// </summary>
		/// <param name="screenName">The screen to update</param>
		public void SetCaptionsBasedOnScreenOverrides(string screenName)
		{
			SetCaptionsBasedOnScreenOverrides(this.MainView, screenName);
		}

		/// <summary>
		/// Set the captions based on the screen caption overrides in the database
		/// </summary>
		/// <param name="screenName">The screen to update</param>
		public void SetCaptionsBasedOnScreenOverrides(BaseView view, string screenName)
		{
			// Reset all the columns to be invisible
			if (view is GridView)
			{
				//foreach (GridColumn col in ((GridView)view).Columns)
				//{
				//    col.Caption = Utils.ScreenCaptions.AlterText(col.Caption, screenName);
				//}
			}
		}
		#endregion SetCaptionsBasedOnScreenOverrides

		/// <summary>
		/// Override the begin updating method
		/// </summary>
		public override void BeginUpdate()
		{
			base.BeginUpdate();
			_isUpdating = true;
		}

		/// <summary>
		/// Override the end updating method
		/// </summary>
		public override void EndUpdate()
		{
			// Redo the screen captioning
			this.SetCaptionsBasedOnScreenOverrides(string.Empty);

			base.EndUpdate();
			_isUpdating = false;
		}

		/// <summary>
		/// Tells if the grid is in the middle of an update
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdating
		{
			get { return _isUpdating; }
		}

		#region Event Handlers
		public delegate void GridLinkClickedEventHandler(object sender, GridLinkClicked_EventArgs e);
		public event GridLinkClickedEventHandler GridLinkClicked;
		protected void OnGridLinkClicked(DevExpress.XtraGrid.Views.Grid.GridView gridView,
			GridColumn column, 
			int rowHandle, 
			GridLinkType linkType,
			object rowObject)
		{
			if (GridLinkClicked != null)
			{
				GridLinkClicked_EventArgs e = new GridLinkClicked_EventArgs(gridView, column, rowHandle, linkType, rowObject);
				GridLinkClicked(this, e);
			}
		}

		public delegate void GridButtonClickedEventHandler(object sender, GridButtonClicked_EventArgs e);
		public event GridButtonClickedEventHandler GridButtonClicked;
		protected void OnGridButtonClicked(DevExpress.XtraGrid.Views.Layout.LayoutView layoutView,
			int rowHandle,
			object rowObject)
		{
			if (GridButtonClicked != null)
			{
				GridButtonClicked_EventArgs e = new GridButtonClicked_EventArgs(layoutView, rowHandle, rowObject);
				GridButtonClicked(this, e);
			}
		}

		public delegate void GridCheckClickingEventHandler(object sender, GridCheckClicked_EventArgs e);
		public event GridCheckClickingEventHandler GridCheckClicking;
		protected void OnGridCheckClicking(DevExpress.XtraGrid.Views.Grid.GridView gridView,
			GridColumn column,
			int rowHandle,
			bool newValue,
			bool oldValue,
			ref bool cancel)
		{
			cancel = false;
			if (GridCheckClicking != null)
			{
				GridCheckClicked_EventArgs e = new GridCheckClicked_EventArgs(gridView, column, rowHandle, newValue, oldValue);
				GridCheckClicking(this, e);
				cancel = e.Cancel;
			}
		}

		public delegate void GridCheckClickedEventHandler(object sender, GridCheckClicked_EventArgs e);
		public event GridCheckClickedEventHandler GridCheckClicked;
		protected void OnGridCheckClicked(DevExpress.XtraGrid.Views.Grid.GridView gridView,
			GridColumn column,
			int rowHandle,
			bool newValue,
			bool oldValue)
		{
			if (GridCheckClicked != null)
			{
				GridCheckClicked_EventArgs e = new GridCheckClicked_EventArgs(gridView, column, rowHandle, newValue, oldValue);
				GridCheckClicked(this, e);
			}
		}

		public delegate void VisibleColumnsChangedEventHandler(object sender, EventArgs e);
		public event VisibleColumnsChangedEventHandler VisibleColumnsChanged;
		protected void OnVisibleColumnsChanged()
		{
			if (VisibleColumnsChanged != null)
			{
				EventArgs e = new EventArgs();
				VisibleColumnsChanged(this, e);
			}
		}
		#endregion Event Handlers

		#region IXtraResizableControl Members

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CS0067")]
		public new event EventHandler SizeChanged { add { } remove { } }

		public bool IsCaptionVisible
		{
			get { return false; }
		}

		public Size MaxSize
		{
			get { return new Size(0, 0); }
		}

		public Size MinSize
		{
			get { return new Size(0, 0); }
		}

		#endregion
	}

	public class mgDevX_GridControl_Column
	{

	}

	public class mgDevX_GridControl_ColumnCollection : List<mgDevX_GridControl_Column>
	{

	}

	/// <summary>
	/// Element for the links
	/// </summary>
	public class mgDevX_GridControl_Link
	{
		private GridView _view = null;
		private GridLinkType _linkType = GridLinkType.None;
		private Image _linkImage = null;
		private string _linkFieldName = string.Empty;

		public mgDevX_GridControl_Link(GridView view,
			GridLinkType linkType,
			Image linkImage,
			string linkFieldName)
		{
			_view = view;
			_linkType = linkType;
			_linkImage = linkImage;
			_linkFieldName = linkFieldName;
		}

		public mgDevX_GridControl_Link()
		{
		}

		public GridView GridView
		{
			get { return _view; }
			set { _view = value; }
		}


		public GridLinkType LinkType
		{
			get { return _linkType; }
			set { _linkType = value; }
		}

		public Image LinkImage
		{
			get { return _linkImage; }
			set { _linkImage = value; }
		}

		public string LinkFieldName
		{
			get { return _linkFieldName; }
			set { _linkFieldName = value; }
		}
	}

	public class mgDevX_GridControl_LinkCollection : List<mgDevX_GridControl_Link>
	{
		/// <summary>
		/// See if this collection contains an element for the view and fieldname passed in
		/// </summary>
		public bool ContainsFieldName(GridView view, string fieldName)
		{
			bool exists = false;

			foreach (mgDevX_GridControl_Link item in this)
			{
				if (item.GridView.Name == view.Name &&
					item.LinkFieldName == fieldName)
				{
					exists = true;
					break;
				}
			}

			return exists;
		}

		/// <summary>
		/// Get the link where the view and fieldname match
		/// </summary>
		public mgDevX_GridControl_Link GetByViewAndFieldName(GridView view, string fieldName)
		{
			mgDevX_GridControl_Link link = null;

			foreach (mgDevX_GridControl_Link item in this)
			{
				if (item.GridView.Name == view.Name &&
					item.LinkFieldName == fieldName)
				{
					link = item;
					break;
				}
			}

			return link;
		}
	}

	/// <summary>
	/// Element for the buttons (layout/card view)
	/// </summary>
	public class mgDevX_GridControl_Button
	{
		private LayoutView _view = null;
		private Font _font = null;
		private Image _buttonImage = null;
		private string _fieldName = string.Empty;
		private DevExpress.XtraEditors.ImageLocation _imageLocation = DevExpress.XtraEditors.ImageLocation.Default;

		public mgDevX_GridControl_Button(LayoutView view,
			Font font,
			Image buttonImage,
			string fieldName,
			DevExpress.XtraEditors.ImageLocation imageLocation)
		{
			_view = view;
			_font = font;
			_buttonImage = buttonImage;
			_fieldName = fieldName;
			_imageLocation = imageLocation;
		}

		public mgDevX_GridControl_Button()
		{
		}

		public LayoutView LayoutView
		{
			get { return _view; }
			set { _view = value; }
		}

		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		public Image ButtonImage
		{
			get { return _buttonImage; }
			set { _buttonImage = value; }
		}

		public string FieldName
		{
			get { return _fieldName; }
			set { _fieldName = value; }
		}

		public DevExpress.XtraEditors.ImageLocation ImageLocation
		{
			get { return _imageLocation; }
			set { _imageLocation = value; }
		}
	}

	public class mgDevX_GridControl_ButtonCollection : List<mgDevX_GridControl_Button>
	{
		/// <summary>
		/// See if this collection contains an element for the view and fieldname passed in
		/// </summary>
		public bool ContainsFieldName(GridView view, string fieldName)
		{
			bool exists = false;

			foreach (mgDevX_GridControl_Button item in this)
			{
				if (item.LayoutView.Name == view.Name &&
					item.FieldName == fieldName)
				{
					exists = true;
					break;
				}
			}

			return exists;
		}

		/// <summary>
		/// Get the link where the view and fieldname match
		/// </summary>
		public mgDevX_GridControl_Button GetByViewAndFieldName(GridView view, string fieldName)
		{
			mgDevX_GridControl_Button link = null;

			foreach (mgDevX_GridControl_Button item in this)
			{
				if (item.LayoutView.Name == view.Name &&
					item.FieldName == fieldName)
				{
					link = item;
					break;
				}
			}

			return link;
		}
	}

	//#region Custom ButtonEditor
	//public class _BaseWinProject_ButtonEdit : ButtonEdit
	//{
	//    static _BaseWinProject_ButtonEdit()
	//    {
	//        RepositoryItem_BaseWinProject_ButtonEdit.Register();
	//    }


	//    public override string EditorTypeName
	//    {
	//        get { return RepositoryItem_BaseWinProject_ButtonEdit.EditorName; }
	//    }
	//    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	//    public new RepositoryItem_BaseWinProject_ButtonEdit Properties
	//    {
	//        get { return base.Properties as RepositoryItem_BaseWinProject_ButtonEdit; }
	//    }
	//}

	//public class _BaseWinProject_ButtonEditPainter : ButtonEditPainter
	//{

	//    public _BaseWinProject_ButtonEditPainter()
	//    {

	//    }

	//    protected override void DrawContent(ControlGraphicsInfoArgs info)
	//    {
	//        Color backColor = info.ViewInfo.Appearance.GetBackColor();
	//        if (backColor == Color.Empty) backColor = Color.White;
	//        Brush brush = new SolidBrush(backColor);
	//        info.Graphics.FillRectangle(brush, info.Bounds);
	//        base.DrawContent(info);
	//    }
	//}

	//public class _BaseWinProject_ButtonEditViewInfo : ButtonEditViewInfo
	//{
	//    public RepositoryItem_BaseWinProject_ButtonEdit _BaseWinProject_RepositoryItem
	//    {
	//        get { return this.Item as RepositoryItem_BaseWinProject_ButtonEdit; }
	//    }

	//    public _BaseWinProject_ButtonEditViewInfo(RepositoryItem item)
	//        : base(item)
	//    {

	//    }

	//    Rectangle CalcButtonsBounds(Rectangle buttonBounds)
	//    {
	//        Rectangle r = buttonBounds;
	//        int buttonsHeight = _BaseWinProject_RepositoryItem.ButtonsHeight;
	//        if (buttonsHeight > 0 && buttonsHeight < buttonBounds.Height)
	//            r.Height = buttonsHeight;
	//        if (buttonsHeight > 0)
	//            r.Height = buttonsHeight;

	//        if (_BaseWinProject_RepositoryItem.ButtonsAlignment == DevExpress.Utils.VertAlignment.Bottom)
	//            r.Y = buttonBounds.Bottom - buttonsHeight;
	//        if (_BaseWinProject_RepositoryItem.ButtonsAlignment == DevExpress.Utils.VertAlignment.Center)
	//            r.Y = buttonBounds.Top + (buttonBounds.Height - buttonsHeight) / 2;
	//        return r;
	//    }

	//    void CalcButtonsBoundsCore(EditorButtonObjectCollection collection)
	//    {
	//        for (int n = collection.Count - 1; n >= 0; n--)
	//        {
	//            EditorButtonObjectInfoArgs button = collection[n];
	//            button.Bounds = CalcButtonsBounds(button.Bounds);
	//        }
	//    }

	//    protected override Rectangle CalcButtons(DevExpress.Utils.Drawing.GraphicsCache cache)
	//    {
	//        Rectangle result = base.CalcButtons(cache);
	//        CalcButtonsBoundsCore(this.LeftButtons);
	//        CalcButtonsBoundsCore(this.RightButtons);
	//        return result;
	//    }
	//}

	//[UserRepositoryItem("Register")]
	//public class RepositoryItem_BaseWinProject_ButtonEdit : RepositoryItemButtonEdit
	//{
	//    static RepositoryItem_BaseWinProject_ButtonEdit()
	//    {
	//        Register();
	//    }
	//    public RepositoryItem_BaseWinProject_ButtonEdit() { }

	//    internal const string EditorName = "_BaseWinProject_ButtonEdit";

	//    public static void Register()
	//    {
	//        EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(_BaseWinProject_ButtonEdit),
	//            typeof(RepositoryItem_BaseWinProject_ButtonEdit), typeof(_BaseWinProject_ButtonEditViewInfo),
	//            new _BaseWinProject_ButtonEditPainter(), true, null));
	//    }
	//    public override string EditorTypeName
	//    {
	//        get { return EditorName; }
	//    }

	//    private int _ButtonHeight;

	//    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	//    public int ButtonsHeight
	//    {
	//        get { return _ButtonHeight; }
	//        set { _ButtonHeight = value; }
	//    }

	//    private VertAlignment _ButtonAlignment;

	//    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	//    public VertAlignment ButtonsAlignment
	//    {
	//        get { return _ButtonAlignment; }
	//        set { _ButtonAlignment = value; }
	//    }
	//}
	//#endregion Custom ButtonEditor
}
