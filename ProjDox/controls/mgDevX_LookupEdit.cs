using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;

namespace mgControls
{
	public class mgDevX_LookupEdit : LookUpEdit
	{
		//private SupportDropDown _dropDownType = SupportDropDown.Empty;
		private DataTable _dt = null;
		private bool _addBlankRow = false;
		private string _blankRowText = string.Empty;

		private string _valueFieldName = "sValue";
		private string _displayFieldName = "sDescription";
		//private Type _valueType = null;

		public mgDevX_LookupEdit()
		{
			mgInit();
		}

		//public mgDevX_LookupEdit(SupportDropDown dropDownType, bool addBlankRow)
		//{
		//    mgInit();

		//    _dropDownType = dropDownType;
		//    _addBlankRow = addBlankRow;
		//    Load(Utils.GetDataTableBasedOnDropDownType(_dropDownType));		// Load the control
		//}

		public void mgInit()
		{
			this.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
			this.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Properties.ImmediatePopup = true;
			this.Properties.NullText = "";
			this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color

			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lue_KeyDown);
		}

		//protected override void RaiseEditValueChanged()
		//{
		//    base.RaiseEditValueChanged();
		//}

		//public SupportDropDown DropDownType
		//{
		//    get { return _dropDownType; }
		//    set { _dropDownType = value; }
		//}

		public string BlankRowText
		{
			get { return _blankRowText; }
			set { _blankRowText = value; }
		}

		public string ValueFieldName
		{
			get { return _valueFieldName; }
			set { _valueFieldName = value; }
		}

		public string DisplayFieldName
		{
			get { return _displayFieldName; }
			set { _displayFieldName = value; }
		}

		/// <summary>
		/// Clear out the control
		/// </summary>
		public void Clear()
		{
			this.Properties.Columns.Clear();
			this.Properties.DataSource = null;
		}
		
		public bool AddBlankRow
		{
			get { return _addBlankRow; }
			set { _addBlankRow = value; }
		}

		private void lue_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete ||
				e.KeyCode == Keys.Back)
			{
				this.EditValue = null;
				this.Text = string.Empty;
			}
		}

		//public void Load(SupportDropDown ddType)
		//{
		//    _dropDownType = ddType;
		//    //Load(Utils.GetDataTableBasedOnDropDownType(_dropDownType));		// This won't work as it's 
		//                                                                        // returning a GUID as the value member

		//    // Convert the datatable
		//    Dictionary<string, string> existingVals = new Dictionary<string, string>();
		//    DataTable dtTemp = Utils.GetDataTableBasedOnDropDownType(_dropDownType);

		//    DataTable dt = new DataTable();
		//    dt.Columns.Add(_valueFieldName, typeof(System.String));
		//    dt.Columns.Add(_displayFieldName, typeof(System.String));

		//    foreach (DataRow row in dtTemp.Rows)
		//    {
		//        if (row[0] != DBNull.Value &&
		//            !existingVals.ContainsKey(row[0].ToString()))
		//        {
		//            DataRow newRow = dt.NewRow();
		//            newRow[0] = (row[0] != DBNull.Value ? row[0].ToString() : "");
		//            newRow[1] = (row[1] != DBNull.Value ? row[1].ToString() : "");
		//            dt.Rows.Add(newRow);

		//            existingVals.Add((row[0] != DBNull.Value ? row[0].ToString() : ""),
		//                (row[1] != DBNull.Value ? row[1].ToString() : ""));
		//        }
		//    }

		//    Load(dt);
		//}

		/// <summary>
		/// Load the list based on a listing of enums
		/// </summary>
		/// <param name="enumType">The type to load the list from</param>
		public void Load(Type enumType)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(_valueFieldName, typeof(System.String));
			dt.Columns.Add(_displayFieldName, typeof(System.String));

			List<string> vals = new List<string>(Enum.GetNames(enumType));
			for (int i = 0; i < vals.Count; i++)
			{
				if (String.IsNullOrEmpty(vals[i]) || vals[i].ToLower() == "empty")
				{
					vals[i] = string.Empty;
				}
			}
			vals.Sort();
			foreach (string s in vals)
			{
				DataRow newRow = dt.NewRow();
				if (!String.IsNullOrEmpty(s) && s.ToLower() == "empty")
				{
					newRow[0] = s;
					newRow[1] = string.Empty;
				}
				else
				{
					newRow[0] = s;
					newRow[1] = mgCustom.Utils.AlterCaptionFromFieldName(s);
				}
				dt.Rows.Add(newRow);
			}

			//_valueType = enumType;		// Set the enum type

			//// Handle the parse event on the control
			//this.CustomDisplayText += new CustomDisplayTextEventHandler(mgDevX_LookupEdit_CustomDisplayText);

			////this.FormatEditValue -= new ConvertEditValueEventHandler(mgDevX_LookupEdit_FormatEditValue);
			//this.ParseEditValue -= new ConvertEditValueEventHandler(mgDevX_LookupEdit_ParseEditValue);

			////this.FormatEditValue += new ConvertEditValueEventHandler(mgDevX_LookupEdit_FormatEditValue);
			//this.ParseEditValue += new ConvertEditValueEventHandler(mgDevX_LookupEdit_ParseEditValue);

			//this.Validating += new CancelEventHandler(mgDevX_LookupEdit_Validating);

			Load(dt);
		}

		public void Load(List<string> list)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(_valueFieldName, typeof(System.String));
			dt.Columns.Add(_displayFieldName, typeof(System.String));

			foreach (string s in list)
			{
				DataRow newRow = dt.NewRow();
				newRow[0] = newRow[1] = s;
				dt.Rows.Add(newRow);
			}

			Load(dt);
		}

		public void Load(Dictionary<string, string> list)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(_valueFieldName, typeof(System.String));
			dt.Columns.Add(_displayFieldName, typeof(System.String));

			foreach (KeyValuePair<string, string> kvp in list)
			{
				DataRow newRow = dt.NewRow();
				newRow[0] = kvp.Key;
				newRow[1] = kvp.Value;
				dt.Rows.Add(newRow);
			}

			Load(dt);
		}

		public void Load(DataTable dt)
		{
			_dt = mgCustom.Utils.CopyDataTable(dt);
			_valueFieldName = _dt.Columns[0].ColumnName;
			_displayFieldName = _dt.Columns[1].ColumnName;
			Load(_valueFieldName, _displayFieldName);
		}

		public void Load(DataTable dt, string valueFieldName, string displayFieldName)
		{
			_dt = mgCustom.Utils.CopyDataTable(dt);
			_valueFieldName = valueFieldName;
			_displayFieldName = displayFieldName;
			Load(_valueFieldName, _displayFieldName);		// Load the table
		}

		public void Load(string valueFieldName, string displayFieldName)
		{
			if (_addBlankRow)
			{
				DataRow newRow = _dt.NewRow();
			
				_valueFieldName = valueFieldName;
				_displayFieldName = displayFieldName;

				//newRow[_valueFieldName] = DBNull.Value;
				newRow[_valueFieldName] = string.Empty;		// We tried to use a null, but putting these back into a collection doesn't seem to work
				newRow[_displayFieldName] = (!String.IsNullOrEmpty(_blankRowText) ? _blankRowText : string.Empty);
				_dt.Rows.InsertAt(newRow, 0);
			}

			//this.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.Properties.Columns.Clear();
			this.Properties.DataSource = null;
			this.Properties.DataSource = _dt;
			this.Properties.ValueMember = _dt.Columns[_valueFieldName].ColumnName;
			this.Properties.DisplayMember = _dt.Columns[_displayFieldName].ColumnName;
			this.Properties.Columns.Add(new LookUpColumnInfo(_dt.Columns[_displayFieldName].ColumnName, 200, "Name"));
		}

		public void LoadReverse(List<string> list)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(_valueFieldName, typeof(System.String));
			dt.Columns.Add(_displayFieldName, typeof(System.String));

			for (int i = list.Count - 1; i >= 0; i--)
			{
				DataRow newRow = dt.NewRow();
				newRow[0] = newRow[1] = list[i];
				dt.Rows.Add(newRow);
			}

			Load(dt);
		}

		public void LoadReverse(Dictionary<string, string> list)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(_valueFieldName, typeof(System.String));
			dt.Columns.Add(_displayFieldName, typeof(System.String));

			foreach (KeyValuePair<string, string> kvp in list)
			{
				DataRow newRow = dt.NewRow();
				newRow[0] = kvp.Key;
				newRow[1] = kvp.Value;
				dt.Rows.InsertAt(newRow, 0);
			}

			Load(dt);
		}

		public void AddColumn(string fieldName, int width, string caption)
		{
			this.AddColumn(fieldName, width, caption, DevExpress.Utils.FormatType.None, string.Empty);
		}

		public void AddColumn(string fieldName, int width, string caption, DevExpress.Utils.FormatType formatType, string formatString)
		{
			this.AddColumn(fieldName, width, caption, formatType, formatString, true, DevExpress.Utils.HorzAlignment.Default);
		}

		public void AddColumn(string fieldName, 
			int width, 
			string caption, 
			DevExpress.Utils.FormatType formatType, 
			string formatString,
			bool visible,
			DevExpress.Utils.HorzAlignment alignment)
		{
			this.Properties.Columns.Add(new LookUpColumnInfo(_dt.Columns[fieldName].ColumnName, 
				caption,
				width,
				formatType, 
				formatString,
				visible,
				alignment));
		}

		public LookUpColumnInfoCollection Columns
		{
			get { return this.Properties.Columns; }
		}

		public int RowCount
		{
			get { return (_dt == null ? 0 : _dt.Rows.Count); }
		}
		
		public int ItemCount
		{
			get { return RowCount; }
		}

		/// <summary>
		/// Select the item in the lookup edit by the value
		/// </summary>
		/// <param name="value"></param>
		public void SelectByKey(string key)
		{
			if (_dt == null) 
			{
				this.EditValue = null;
				return; 
			}

			bool found = false;
			foreach (DataRow row in _dt.Rows)
			{
				if (row[_valueFieldName] != DBNull.Value &&
					String.Compare(row[_valueFieldName].ToString(), key, true) == 0)
				{
					found = true;
					this.EditValue = (row[_valueFieldName] != DBNull.Value ?
						row[_valueFieldName].ToString() : "");
					break;
				}
			}
			if (!found)
			{
				this.EditValue = null;
			}
		}

		/// <summary>
		/// Select the item in the lookup edit by the display text
		/// </summary>
		/// <param name="displayText"></param>
		public void SelectByValue(string val)
		{
			if (_dt == null) { return; }
			foreach (DataRow row in _dt.Rows)
			{
				if (row[_displayFieldName] != DBNull.Value &&
					String.Compare(row[_displayFieldName].ToString(), val, true) == 0)
				{
					this.EditValue = (row[_valueFieldName] != DBNull.Value ?
						row[_valueFieldName].ToString() : "");
					break;
				}
			}
		}

		/// <summary>
		/// Select the first value in the lookup edit
		/// </summary>
		public void SelectFirstValue()
		{
			if (_dt == null || _dt.Rows.Count <= 0) { return; }
			this.EditValue = (_dt.Rows[0][_valueFieldName] != DBNull.Value ?
				_dt.Rows[0][_valueFieldName].ToString() : "");
		}

		/// <summary>
		/// Select the first non-blank value in the lookup edit
		/// </summary>
		public void SelectFirstNonBlankValue()
		{
			if (_dt == null || _dt.Rows.Count <= 0) { return; }
			foreach (DataRow row in _dt.Rows)
			{
				if (row[_displayFieldName] != DBNull.Value &&
					!String.IsNullOrEmpty(row[_displayFieldName].ToString()))
				{
					this.EditValue = row[_valueFieldName].ToString();
					break;
				}
			}
		}

		/// <summary>
		/// Returns true if the Text Exists
		/// </summary>
		public bool DisplayTextExists(string displayText)
		{
			bool rtv = false;
			if (_dt == null) { return false; }
			foreach (DataRow row in _dt.Rows)
			{
				if (row[_displayFieldName] != DBNull.Value &&
					row[_displayFieldName].ToString() == displayText)
				{
					rtv = true;
					break;
				}
			}
			return rtv;
		}

		/// <summary>
		/// Find out if the value exists
		/// </summary>
		/// <param name="value">The value to find</param>
		/// <returns></returns>
		public bool ValueExists(string value)
		{
			bool rtv = false;
			if (_dt == null) { return false; }
			foreach (DataRow row in _dt.Rows)
			{
				if (row[_valueFieldName] != DBNull.Value &&
					row[_valueFieldName].ToString() == value)
				{
					rtv = true;
					break;
				}
			}
			return rtv;
		}

		//public override string ToString()
		//{
		//    return (this.EditValue != null ? this.EditValue.ToString() : "");
		//    //return (_dt.Rows[this.Properties.ValueMember] != DBNull.Value ? _dt.Rows[this.Properties.ValueMember].ToString() : "");
		//    //return base.ToString();
		//}
	}

	//[UserRepositoryItem("Register")]
	//public class Repository_mgDevX_LookupEdit : RepositoryItemLookUpEdit
	//{
	//    private SupportDropDown _dropDownType = SupportDropDown.Empty;
	//    private DataTable _dt = null;
	//    private bool _addBlankRow = false;
	//    private string _blankRowText = string.Empty;

	//    private string _valueFieldName = "sValue";
	//    private string _displayFieldName = "sDescription";
		
	//    public Repository_mgDevX_LookupEdit() 
	//    {
	//        mgInit();
	//    }

	//    static Repository_mgDevX_LookupEdit()
	//    {
	//        Register();

	//        //mgInit();
	//    }

	//    public void mgInit()
	//    {
	//        this.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
	//        this.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
	//            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
	//        this.ImmediatePopup = true;
	//        this.NullText = "";

	//        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lue_KeyDown);
	//    }

	//    protected override void OnDataSourceChanged()
	//    {
	//        FieldInfo fieldInfo = typeof(RepositoryItemLookUpEditBase).GetField("lastProcessedNewValue", 
	//            BindingFlags.Instance | BindingFlags.NonPublic);
	//        fieldInfo.SetValue(this, null);
	//        base.OnDataSourceChanged();
	//    }

	//    protected override void RaiseProcessNewValue(DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
	//    {
	//        base.RaiseProcessNewValue(e);
	//    }

	//    public static void Register()
	//    {
	//        EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("mgDevX_LookupEdit", typeof(mgDevX_LookupEdit),
	//            typeof(Repository_mgDevX_LookupEdit), typeof(DevExpress.XtraEditors.ViewInfo.LookUpEditViewInfo),
	//            new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true));
	//    }

	//    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	//    public override string EditorTypeName { get { return "mgDevX_LookupEdit"; } }

	//    #region Properties/Events
	//    public SupportDropDown DropDownType
	//    {
	//        get { return _dropDownType; }
	//        set { _dropDownType = value; }
	//    }

	//    public string BlankRowText
	//    {
	//        get { return _blankRowText; }
	//        set { _blankRowText = value; }
	//    }

	//    public string ValueFieldName
	//    {
	//        get { return _valueFieldName; }
	//        set { _valueFieldName = value; }
	//    }

	//    public string DisplayFieldName
	//    {
	//        get { return _displayFieldName; }
	//        set { _displayFieldName = value; }
	//    }

	//    /// <summary>
	//    /// Clear out the control
	//    /// </summary>
	//    public void Clear()
	//    {
	//        this.Columns.Clear();
	//        this.DataSource = null;
	//    }

	//    public bool AddBlankRow
	//    {
	//        get { return _addBlankRow; }
	//        set { _addBlankRow = value; }
	//    }

	//    private void lue_KeyDown(object sender, KeyEventArgs e)
	//    {
	//        if (e.KeyCode == Keys.Delete ||
	//            e.KeyCode == Keys.Back)
	//        {
	//            //this.EditValue = null;
	//            //this.Text = string.Empty;
	//        }
	//    }
	//    #endregion Properties/Events

	//    #region Methods
	//    public void Load(SupportDropDown ddType)
	//    {
	//        _dropDownType = ddType;

	//        // Convert the datatable
	//        DataTable dtTemp = Utils.GetDataTableBasedOnDropDownType(_dropDownType);

	//        DataTable dt = new DataTable();
	//        dt.Columns.Add(_valueFieldName, typeof(System.String));
	//        dt.Columns.Add(_displayFieldName, typeof(System.String));

	//        foreach (DataRow row in dtTemp.Rows)
	//        {
	//            DataRow newRow = dt.NewRow();
	//            newRow[0] = (row[0] != DBNull.Value ? row[0].ToString() : "");
	//            newRow[1] = (row[1] != DBNull.Value ? row[1].ToString() : "");
	//            dt.Rows.Add(newRow);
	//        }

	//        Load(dt);
	//    }

	//    /// <summary>
	//    /// Load the list based on a listing of enums
	//    /// </summary>
	//    /// <param name="enumType">The type to load the list from</param>
	//    public void Load(Type enumType)
	//    {
	//        DataTable dt = new DataTable();
	//        dt.Columns.Add(_valueFieldName, typeof(System.String));
	//        dt.Columns.Add(_displayFieldName, typeof(System.String));

	//        foreach (string s in Enum.GetNames(enumType))
	//        {
	//            DataRow newRow = dt.NewRow();
	//            newRow[0] = newRow[1] = s;
	//            dt.Rows.Add(newRow);
	//        }

	//        Load(dt);
	//    }

	//    public void Load(List<string> list)
	//    {
	//        DataTable dt = new DataTable();
	//        dt.Columns.Add(_valueFieldName, typeof(System.String));
	//        dt.Columns.Add(_displayFieldName, typeof(System.String));

	//        foreach (string s in list)
	//        {
	//            DataRow newRow = dt.NewRow();
	//            newRow[0] = newRow[1] = s;
	//            dt.Rows.Add(newRow);
	//        }

	//        Load(dt);
	//    }

	//    public void Load(Dictionary<string, string> list)
	//    {
	//        DataTable dt = new DataTable();
	//        dt.Columns.Add(_valueFieldName, typeof(System.String));
	//        dt.Columns.Add(_displayFieldName, typeof(System.String));

	//        foreach (KeyValuePair<string, string> kvp in list)
	//        {
	//            DataRow newRow = dt.NewRow();
	//            newRow[0] = kvp.Key;
	//            newRow[1] = kvp.Value;
	//            dt.Rows.Add(newRow);
	//        }

	//        Load(dt);
	//    }

	//    public void Load(DataTable dt)
	//    {
	//        _dt = Utils.CopyDataTable(dt);
	//        _valueFieldName = _dt.Columns[0].ColumnName;
	//        _displayFieldName = _dt.Columns[1].ColumnName;
	//        Load(_valueFieldName, _displayFieldName);
	//    }

	//    public void Load(DataTable dt, string valueFieldName, string displayFieldName)
	//    {
	//        _dt = Utils.CopyDataTable(dt);
	//        _valueFieldName = valueFieldName;
	//        _displayFieldName = displayFieldName;
	//        Load(_valueFieldName, _displayFieldName);		// Load the table
	//    }

	//    public void Load(string valueFieldName, string displayFieldName)
	//    {
	//        if (_addBlankRow)
	//        {
	//            DataRow newRow = _dt.NewRow();

	//            _valueFieldName = valueFieldName;
	//            _displayFieldName = displayFieldName;

	//            newRow[_valueFieldName] = DBNull.Value;
	//            newRow[_displayFieldName] = (!String.IsNullOrEmpty(_blankRowText) ? _blankRowText : string.Empty);
	//            _dt.Rows.InsertAt(newRow, 0);
	//        }

	//        //this.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
	//        this.Columns.Clear();
	//        this.DataSource = null;
	//        this.DataSource = _dt;
	//        this.ValueMember = _dt.Columns[_valueFieldName].ColumnName;
	//        this.DisplayMember = _dt.Columns[_displayFieldName].ColumnName;
	//        this.Columns.Add(new LookUpColumnInfo(_dt.Columns[_displayFieldName].ColumnName, 200, "Name"));
	//    }

	//    //public LookUpColumnInfoCollection Columns
	//    //{
	//    //    get { return this.Columns; }
	//    //}

	//    public int RowCount
	//    {
	//        get { return (_dt == null ? 0 : _dt.Rows.Count); }
	//    }

	//    public int ItemCount
	//    {
	//        get { return RowCount; }
	//    }

	//    ///// <summary>
	//    ///// Select the item in the lookup edit by the value
	//    ///// </summary>
	//    ///// <param name="value"></param>
	//    //public void SelectByKey(string key)
	//    //{
	//    //    this.EditValue = key;
	//    //}

	//    ///// <summary>
	//    ///// Select the item in the lookup edit by the display text
	//    ///// </summary>
	//    ///// <param name="displayText"></param>
	//    //public void SelectByValue(string val)
	//    //{
	//    //    if (_dt == null) { return; }
	//    //    foreach (DataRow row in _dt.Rows)
	//    //    {
	//    //        if (row[_displayFieldName] != DBNull.Value &&
	//    //            String.Compare(row[_displayFieldName].ToString(), val, true) == 0)
	//    //        {
	//    //            this.EditValue = (row[_valueFieldName] != DBNull.Value ?
	//    //                row[_valueFieldName].ToString() : "");
	//    //            break;
	//    //        }
	//    //    }
	//    //}

	//    ///// <summary>
	//    ///// Select the first value in the lookup edit
	//    ///// </summary>
	//    //public void SelectFirstValue()
	//    //{
	//    //    if (_dt == null || _dt.Rows.Count <= 0) { return; }
	//    //    this.EditValue = (_dt.Rows[0][_valueFieldName] != DBNull.Value ?
	//    //        _dt.Rows[0][_valueFieldName].ToString() : "");
	//    //}

	//    /// <summary>
	//    /// Returns true if the Text Exists
	//    /// </summary>
	//    public bool DisplayTextExists(string displayText)
	//    {
	//        bool rtv = false;
	//        if (_dt == null) { return false; }
	//        foreach (DataRow row in _dt.Rows)
	//        {
	//            if (row[_displayFieldName] != DBNull.Value &&
	//                row[_displayFieldName].ToString() == displayText)
	//            {
	//                rtv = true;
	//                break;
	//            }
	//        }
	//        return rtv;
	//    }

	//    /// <summary>
	//    /// Find out if the value exists
	//    /// </summary>
	//    /// <param name="value">The value to find</param>
	//    /// <returns></returns>
	//    public bool ValueExists(string value)
	//    {
	//        bool rtv = false;
	//        if (_dt == null) { return false; }
	//        foreach (DataRow row in _dt.Rows)
	//        {
	//            if (row[_valueFieldName] != DBNull.Value &&
	//                row[_valueFieldName].ToString() == value)
	//            {
	//                rtv = true;
	//                break;
	//            }
	//        }
	//        return rtv;
	//    }
	//    #endregion Methods
	//}
}
