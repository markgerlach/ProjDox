using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using System.Windows.Forms;

using mgModel;

namespace mgCustom
{
	#region GridCheckClicked_EventArgs
	/// <summary>
	/// Custom class for GridCheckClicked event args 
	/// </summary>
	[Serializable]
	public class GridCheckClicked_EventArgs : EventArgs
	{
		private DevExpress.XtraGrid.Views.Grid.GridView _gridView = null;
		private int _rowHandle = -1;
		private GridColumn _column = new GridColumn();
		private bool _newValue = false;
		private bool _oldValue = false;
		private bool _cancel = false;

		public GridCheckClicked_EventArgs(DevExpress.XtraGrid.Views.Grid.GridView gridView,
			GridColumn column,
			int rowHandle,
			bool newValue,
			bool oldValue)
		{
			_column = column;
			_rowHandle = rowHandle;
			_gridView = gridView;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public GridCheckClicked_EventArgs()
		{
		}

		public DevExpress.XtraGrid.Views.Grid.GridView GridView
		{
			get { return _gridView; }
		}

		public GridColumn Column
		{
			get { return _column; }
		}

		public int RowHandle
		{
			get { return _rowHandle; }
		}

		public bool NewValue
		{
			get { return _newValue; }
		}

		public bool OldValue
		{
			get { return _oldValue; }
		}

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
	#endregion GridCheckClicked_EventArgs

	#region GridLinkClicked_EventArgs
	/// <summary>
	/// Custom class for GridLinkClicked event args such as population
	/// </summary>
	[Serializable]
	public class GridLinkClicked_EventArgs : EventArgs
	{
		private DataRowView _rowDataRowView = null;
		private string _filterString = string.Empty;
		private string _appliedFilterGUID = string.Empty;
		private Dictionary<string, string> _basicSearchParams = new Dictionary<string, string>();
		private string _rowGUID = string.Empty;
		private DevExpress.XtraGrid.Views.Grid.GridView _gridView = null;

		//private string _linkText = string.Empty;
		//private string _columnName = string.Empty;
		private object _rowObject = null;
		private int _rowHandle = -1;
		private GridColumn _column = new GridColumn();
		private GridLinkType _linkType = GridLinkType.None;

		public GridLinkClicked_EventArgs(DevExpress.XtraGrid.Views.Grid.GridView gridView,
			DataRowView rowDataRowView,
			string filterString,
			string appliedFilterGUID,
			Dictionary<string, string> basicSearchParams)
		{
			_rowDataRowView = rowDataRowView;
			_filterString = filterString;
			_appliedFilterGUID = appliedFilterGUID;
			_basicSearchParams = basicSearchParams;
			_gridView = gridView;
		}

		public GridLinkClicked_EventArgs(DataRowView rowDataRowView,
			string filterString,
			string appliedFilterGUID,
			Dictionary<string, string> basicSearchParams)
		{
			_rowDataRowView = rowDataRowView;
			_filterString = filterString;
			_appliedFilterGUID = appliedFilterGUID;
			_basicSearchParams = basicSearchParams;
		}

		public GridLinkClicked_EventArgs(DevExpress.XtraGrid.Views.Grid.GridView gridView,
			GridColumn column,
			int rowHandle,
			GridLinkType linkType,
			object rowObject)
		{
			//_linkText = linkText;
			//_rowObject = rowObject;
			_column = column;
			//_columnName = column.FieldName;
			_linkType = linkType;
			_rowHandle = rowHandle;
			_gridView = gridView;
			_rowObject = rowObject;
		}

		public GridLinkClicked_EventArgs(string rowGUID)
		{
			_rowGUID = rowGUID;
		}

		public GridLinkClicked_EventArgs()
		{
		}

		public DataRowView RowDataRowView
		{
			get { return _rowDataRowView; }
		}

		public string AppliedFilterGUID
		{
			get { return _appliedFilterGUID; }
		}

		public string FilterString
		{
			get { return _filterString; }
		}

		public DevExpress.XtraGrid.Views.Grid.GridView GridView
		{
			get { return _gridView; }
		}

		public Dictionary<string, string> BasicSearchParams
		{
			get { return _basicSearchParams; }
		}

		public string RowGUID
		{
			get { return _rowGUID; }
		}

		//public string LinkText
		//{
		//    get { return _linkText; }
		//}

		//public string ColumnName
		//{
		//    get { return _columnName; }
		//}

		public object RowObject
		{
			get { return _rowObject; }
		}

		public GridColumn Column
		{
			get { return _column; }
		}

		public GridLinkType LinkType
		{
			get { return _linkType; }
		}

		public int RowHandle
		{
			get { return _rowHandle; }
		}
	}
	#endregion GridLinkClicked_EventArgs

	#region GridButtonClicked_EventArgs
	/// <summary>
	/// Custom class for GridButtonClicked event args such as population
	/// </summary>
	[Serializable]
	public class GridButtonClicked_EventArgs : EventArgs
	{
		private DevExpress.XtraGrid.Views.Layout.LayoutView _layoutView = null;
		private object _rowObject = null;
		private int _rowHandle = -1;

		public GridButtonClicked_EventArgs(DevExpress.XtraGrid.Views.Layout.LayoutView layoutView,
			int rowHandle,
			object rowObject)
		{
			_rowHandle = rowHandle;
			_layoutView = layoutView;
			_rowObject = rowObject;
		}

		public GridButtonClicked_EventArgs()
		{
		}

		public DevExpress.XtraGrid.Views.Layout.LayoutView GridView
		{
			get { return _layoutView; }
		}

		public object RowObject
		{
			get { return _rowObject; }
		}

		public int RowHandle
		{
			get { return _rowHandle; }
		}
	}
	#endregion GridButtonClicked_EventArgs

	#region ToolTipBeforeShow_EventArgs
	/// <summary>
	/// Custom class for ToolTip event args such as population
	/// this.OnGridLinkClicked(link.GridView, ghi.Column, ghi.RowHandle, link.LinkType);
	/// </summary>
	[Serializable]
	public class ToolTipBeforeShow_EventArgs : EventArgs
	{
		private DevExpress.Utils.ToolTipControllerShowEventArgs _ttControllerEventArgs = null;
		private DevExpress.XtraGrid.Views.Grid.GridView _gridView = null;
		private int _rowHandle = -1;
		private GridColumn _column = new GridColumn();

		public ToolTipBeforeShow_EventArgs(
			DevExpress.Utils.ToolTipControllerShowEventArgs ttControllerEventArgs,
			DevExpress.XtraGrid.Views.Grid.GridView gridView,
			GridColumn column,
			int rowHandle)
		{
			_ttControllerEventArgs = ttControllerEventArgs;
			_column = column;
			_rowHandle = rowHandle;
			_gridView = gridView;
		}

		public ToolTipBeforeShow_EventArgs()
		{
		}

		public DevExpress.Utils.ToolTipControllerShowEventArgs ToolTipControllerShowEventArgs
		{
			get { return _ttControllerEventArgs; }
		}

		public DevExpress.XtraGrid.Views.Grid.GridView GridView
		{
			get { return _gridView; }
		}

		public int RowHandle
		{
			get { return _rowHandle; }
		}

		public GridColumn Column
		{
			get { return _column; }
		}
	}
	#endregion ToolTipBeforeShow_EventArgs

	#region SingleRecEventArgs
	/// <summary>
	/// Custom class event Args
	/// </summary>
	public class SingleRecEventArgs : EventArgs
	{
		private string _recGUID;

		public SingleRecEventArgs(string recGUID)
		{
			_recGUID = recGUID;
		}

		public SingleRecEventArgs()
		{
		}

		public string RecGUID
		{
			get { return _recGUID; }
		}
	}
	#endregion SingleRecEventArgs

	#region LoadingProgressEventArgs
	/// <summary>
	/// Custom class event Args
	/// </summary>
	public class LoadingProgressEventArgs : EventArgs
	{
		private int _currentIndex = 0;
		private int _totalIndex = 0;

		public LoadingProgressEventArgs(int currentIndex, int totalIndex)
		{
			_currentIndex = currentIndex;
			_totalIndex = totalIndex;
		}

		public LoadingProgressEventArgs()
		{
		}

		public int CurrentIndex
		{
			get { return _currentIndex; }
		}

		public int TotalIndex
		{
			get { return _totalIndex; }
		}
	}
	#endregion LoadingProgressEventArgs

	#region LookupEditEventArgs
	/// <summary>
	/// Custom class event Args
	/// </summary>
	public class LookupEditEventArgs : EventArgs
	{
		private string _newText;

		public LookupEditEventArgs(string newText)
		{
			_newText = newText;
		}

		public LookupEditEventArgs()
		{
		}

		public string NewText
		{
			get { return _newText; }
		}
	}
	#endregion LookupEditEventArgs
	
	#region FilterEventArgs
	/// <summary>
	/// Custom class for Search event args that allow the 
	/// passing back of the datarow for the selected row in the grid
	/// </summary>
	public class FilterEventArgs : EventArgs
	{
		private string _newText = string.Empty;

		public FilterEventArgs(string newText)
		{
			_newText = newText;
		}

		public FilterEventArgs()
		{
		}

		public string NewText
		{
			get { return _newText; }
		}
	}
	#endregion FilterEventArgs

	#region ButtonClickEventArgs
	/// <summary>
	/// Custom class event Args
	/// </summary>
	public class ButtonClickEventArgs : EventArgs
	{
		private string _newText;
		private bool _cancel = false;

		public ButtonClickEventArgs(string newText)
		{
			_newText = newText;
		}

		public ButtonClickEventArgs()
		{
		}

		public string NewText
		{
			get { return _newText; }
		}

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
	#endregion ButtonClickEventArgs

	#region mwsRTFEditTextChangedEventArgs
	/// <summary>
	/// Custom class for RTF Edit passing back the text and the RTF of the control
	/// </summary>
	public class mwsRTFEditTextChangedEventArgs : EventArgs
	{
		private string _rtf;
		private string _text;

		//public mwsRTFEditTextChangedEventArgs(RichTextBox rtf)
		//{
		//    _rtf = rtf.Rtf;
		//    _text = rtf.Text;
		//}

		public mwsRTFEditTextChangedEventArgs(DevExpress.XtraRichEdit.RichEditControl rtf)
		{
			_rtf = rtf.RtfText;
			_text = rtf.Text;
		}

		public mwsRTFEditTextChangedEventArgs()
		{
		}

		public string Rtf
		{
			get { return _rtf; }
		}

		public string Text
		{
			get { return _text; }
		}
	}
	#endregion mwsRTFEditTextChangedEventArgs

	#region FavoritesLinkClicked_EventArgs
	/// <summary>
	/// Custom class for GridLinkClicked event args such as population
	/// </summary>
	[Serializable]
	public class FavoritesLinkClicked_EventArgs : EventArgs
	{
		private string _searchGUID = string.Empty;

		public FavoritesLinkClicked_EventArgs(string searchGUID)
		{
			_searchGUID = searchGUID;
		}

		public FavoritesLinkClicked_EventArgs()
		{
		}

		public string SearchGUID
		{
			get { return _searchGUID; }
		}
	}
	#endregion FavoritesLinkClicked_EventArgs

	#region CustomUserControlEventArgs

	public class CustomUserControlEventArgs : EventArgs
	{
		private string _entityGUID = string.Empty;

		public CustomUserControlEventArgs()
		{ }

		public CustomUserControlEventArgs(string entityGUID)
		{
			_entityGUID = entityGUID;
		}

		public string EntityGUID
		{
			get { return _entityGUID; }
		}
	}

	#endregion CustomUserControlEventArgs

	#region DateRangeChangedEventArgs

	public class DateRangeChangedEventArgs : EventArgs
	{
		private DateTime _dtStart = DateTime.MinValue;
		private DateTime _dtEnd = DateTime.MaxValue;

		public DateRangeChangedEventArgs()
		{
		}

		public DateRangeChangedEventArgs(DateTime dtStart, DateTime dtEnd)
		{
			_dtStart = dtStart;
			_dtEnd = dtEnd;
		}

		public DateTime DateStart
		{
			get { return _dtStart; }
		}

		public DateTime DateEnd
		{
			get { return _dtEnd; }
		}
	}

	#endregion DateRangeChangedEventArgs

	#region CalcButtonClickedEventArgs
	/// <summary>
	/// Custom class event Args
	/// </summary>
	public class CalcButtonClickedEventArgs : EventArgs
	{
		private CalcButton _button;

		public CalcButtonClickedEventArgs(CalcButton button)
		{
			_button = button;
		}

		public CalcButtonClickedEventArgs()
		{
		}

		public CalcButton Button
		{
			get { return _button; }
		}
	}
	#endregion CalcButtonClickedEventArgs

	#region JobClickedEventArgs
	/// <summary>
	/// Custom class event Args
	/// </summary>
	public class JobClickedEventArgs : EventArgs
	{
		private MotionStudySelectedJob _job = MotionStudySelectedJob.Job1;
		private string _jobNum;
		private string _jobGUID;

		public JobClickedEventArgs(MotionStudySelectedJob job, string jobNum, string jobGUID)
		{
			_jobGUID = jobGUID;
			_jobNum = jobNum;
			_job = job;
		}

		public JobClickedEventArgs()
		{
		}

		public string JobNum
		{
			get { return _jobNum; }
		}

		public string JobGUID
		{
			get { return _jobGUID; }
		}

		public MotionStudySelectedJob Job
		{
			get { return _job; }
		}
	}
	#endregion JobClickedEventArgs
}
