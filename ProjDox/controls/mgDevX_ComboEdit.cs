using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;

using mgCustom;

namespace mgControls
{
	public class mgDevX_ComboEdit : ComboBoxEdit
	{
		//private SupportDropDown _dropDownType = SupportDropDown.Empty;
		private GenericListItemCollection _list = new GenericListItemCollection();
		private bool _addBlankRow = false;
		private string _blankRowText = string.Empty;
		private string _valueFieldName = "sValue";
		private string _displayFieldName = "sDescription";

		public mgDevX_ComboEdit()
		{
			mgInit();
		}

		//public mgDevX_ComboEdit(SupportDropDown dropDownType, bool addBlankRow)
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

			this.Properties.AppearanceReadOnly.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Properties.AppearanceDisabled.BackColor = System.Drawing.SystemColors.ControlLight;		// Take care of the disabled and read only
		}

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

		/// <summary>
		/// Clear out the control
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			this.Properties.Items.Clear();
		}

		public bool AddBlankRow
		{
			get { return _addBlankRow; }
			set { _addBlankRow = value; }
		}

		protected override void OnPopupClosing(CloseUpEventArgs e)
		{
			if (e.Value != null)
			{
				e.AcceptValue = (FindItem(e.Value.ToString(), 0) > -1);
			}
			base.OnPopupClosing(e);
		}

		protected override void ProcessAutoSearchChar(KeyPressEventArgs e)
		{
			//base.ProcessAutoSearchChar(e);
			if (Properties.ReadOnly) { return; }
			char charCode = e.KeyChar;
			if (Properties.CharacterCasing != CharacterCasing.Normal) 
			{
				charCode = (Properties.CharacterCasing == CharacterCasing.Lower ? Char.ToLower(e.KeyChar) : Char.ToUpper(e.KeyChar));
			}
			if (Char.IsControl(charCode) && charCode != '\b') { return; }
			//if (Char.IsControl(charCode) && charCode != '\b') { int stop = 1; }
			if (IsMaskBoxAvailable && Properties.Mask.MaskType != MaskType.None) { return; }
			KeyPressHelper helper = (IsMaskBoxAvailable ? new KeyPressHelper(MaskBox.MaskBoxText, 
				SelectionStart, SelectionLength, Properties.MaxLength) :
				new KeyPressHelper(AutoSearchText, Properties.MaxLength));
			//if (Char.IsControl(charCode) && charCode == '\b') { int stop = 1; }
			//if (charCode == 'e') { int stop = 1; } 
			helper.ProcessChar(e.KeyChar);
			//if (Char.IsControl(charCode) && charCode == '\b')
			//{
			//    if (helper.SelectionStart > 0) { helper.SelectionStart--; }
			//    if (helper.SelectionLength < MaskBox.MaskBoxText.Length) { helper.SelectionLength++; }
			//}
			AutoSearchText = helper.Text;
			e.Handled = true;
			ProcessFindItem(helper, charCode);
		}

		protected override void ProcessFindItem(KeyPressHelper helper, char pressedKey)
		{
			if (Properties.ReadOnly) return;
			int selectionStart = helper.SelectionStart;
			string searchText = AutoSearchText;
			int itemIndex = FindItem(searchText, 0);
			bool isOpened = IsPopupOpen;
			if (IsImmediatePopup)
			{
				DoImmediatePopup(itemIndex, pressedKey);
				if (!isOpened && IsPopupOpen)
					itemIndex = FindItem(searchText, 0);
			}
			if (IsPopupOpen)
			{
				FindUpdatePopupSelectedItem(itemIndex);
			}
			if (itemIndex != -1)
			{
				FindUpdateEditValue(itemIndex, false);
				if (IsMaskBoxAvailable)
				{
					UpdateMaskBoxDisplayText();
					selectionStart = helper.GetCorrectedAutoSearchSelectionStart(Text, pressedKey);
					SelectionStart = selectionStart;
					SelectionLength = Text.Length - selectionStart;
				}
			}
			else
			{
				if (IsMaskBoxAvailable)
				{
					FindUpdateEditValueAutoSearchText();
					UpdateMaskBoxDisplayText();
					SelectionStart = Math.Max(0, selectionStart);
					SelectionLength = 0;
				}
				else
					AutoSearchText = AutoSearchText.Length > 1 ? AutoSearchText.Substring(0, AutoSearchText.Length - 1) : string.Empty;
			}
			LayoutChanged();
		}

		//protected override void ProcessFindItem(KeyPressHelper helper, char pressedKey)
		//{
		//    //base.ProcessFindItem(helper, pressedKey);
		//    if (Properties.ReadOnly) return;
		//    string searchText = AutoSearchText;
		//    int itemIndex = FindItem(searchText, 0);
		//    bool isOpened = IsPopupOpen;
		//    if (IsImmediatePopup)
		//    {
		//        DoImmediatePopup(itemIndex, pressedKey);
		//        if (!isOpened && IsPopupOpen)
		//        {
		//            itemIndex = FindItem(searchText, 0);
		//        }
		//    }
		//    if (IsPopupOpen)
		//    {
		//        FindUpdatePopupSelectedItem(itemIndex);
		//    }
		//    if (itemIndex != -1)
		//    {
		//        FindUpdateEditValue(itemIndex, false);
		//        if (IsMaskBoxAvailable)
		//        {
		//            UpdateMaskBoxDisplayText();
		//            //SelectionStart = selectionStart;
		//            //SelectionLength = Text.Length - selectionStart;
		//            SelectionStart = helper.SelectionStart;
		//            SelectionLength = Text.Length - helper.SelectionStart;
		//        }
		//    }
		//    else
		//    {
		//        if (IsMaskBoxAvailable)
		//        {
		//            FindUpdateEditValueAutoSearchText();
		//            UpdateMaskBoxDisplayText();
		//            //SelectionStart = Math.Max(0, selectionStart);
		//            SelectionStart = Math.Max(0, helper.SelectionStart);
		//            SelectionLength = 0;
		//        }
		//        else
		//        {
		//            AutoSearchText = (AutoSearchText.Length > 1 ? AutoSearchText.Substring(0, AutoSearchText.Length - 1) : string.Empty);
		//        }
		//    }
		//    LayoutChanged();
		//}

		//public void Load(SupportDropDown ddType)
		//{
		//    _dropDownType = ddType;
		//    DataTable dt = Utils.GetDataTableBasedOnDropDownType(_dropDownType);

		//    GenericListItemCollection coll = new GenericListItemCollection();
		//    foreach (DataRow row in dt.Rows)
		//    {
		//        coll.Add(new GenericListItem((row[0] != DBNull.Value ? row[0].ToString() : ""),
		//            (row[1] != DBNull.Value ? row[1].ToString() : ""),
		//            (dt.Columns.Count > 2 && row[2] != DBNull.Value ? row[2].ToString() : "")));
		//    }
		//    Load(coll);
		//}

		public void Load(List<string> list)
		{
			GenericListItemCollection coll = new GenericListItemCollection();
			foreach (string s in list)
			{
				coll.Add(new GenericListItem(s, s, s));
			}

			Load(coll);
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
			GenericListItemCollection coll = new GenericListItemCollection();
			foreach (DataRow row in dt.Rows)
			{
				coll.Add(new GenericListItem((row[0] != DBNull.Value ? row[0].ToString() : ""),
					(row[1] != DBNull.Value ? row[1].ToString() : ""),
					(dt.Columns.Count > 2 && row[2] != DBNull.Value ? row[2].ToString() : "")));
			}
			Load(coll);
		}

		public void Load(GenericListItemCollection coll)
		{
			_list = coll;
			if (_addBlankRow)
			{
				_list.Insert(0, new GenericListItem("", _blankRowText, ""));
			}

			this.Properties.Items.Clear();
			this.Properties.Items.AddRange(_list);
		}

		/// <summary>
		/// Select the item by the key specified
		/// </summary>
		/// <param name="key">The key to find</param>
		public void SelectByKey(string key)
		{
			bool found = false;
			for (int i = 0; i < this.Properties.Items.Count; i++)
			{
				if (((GenericListItem)this.Properties.Items[i]).Key.ToLower() == key.ToLower())
				{
					this.SelectedIndex = i;
					found = true;
					break;
				}
			}
			if (!found) { this.SelectedIndex = -1; }
		}

		/// <summary>
		/// Select the item by the value specified
		/// </summary>
		/// <param name="value">The value to find</param>
		public void SelectByValue(string value)
		{
			bool found = false;
			for (int i = 0; i < this.Properties.Items.Count; i++)
			{
				if (((GenericListItem)this.Properties.Items[i]).Value.ToLower() == value.ToLower())
				{
					this.SelectedIndex = i;
					found = true;
					break;
				}
			}
			if (!found) { this.SelectedIndex = -1; }
		}

		/// <summary>
		/// Select the first value in the lookup edit
		/// </summary>
		public void SelectFirstValue()
		{
			if (this.Properties.Items.Count == 0) { this.SelectedIndex = -1; }
			this.SelectedIndex = 0;
		}
	}

	public class mgDevX_ComboEditStrings : ComboBoxEdit
	{
		private List<string> _list = new List<string>(); 
		private bool _addBlankRow = false;
		private string _blankRowText = string.Empty;

		public mgDevX_ComboEditStrings()
		{
			mgInit();
		}

		public void mgInit()
		{
			this.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
			this.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Properties.ImmediatePopup = true;
			this.Properties.NullText = "";
		}

		public string BlankRowText
		{
			get { return _blankRowText; }
			set { _blankRowText = value; }
		}

		/// <summary>
		/// Clear out the control
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			this.Properties.Items.Clear();
		}

		public bool AddBlankRow
		{
			get { return _addBlankRow; }
			set { _addBlankRow = value; }
		}

		protected override void OnPopupClosing(CloseUpEventArgs e)
		{
			// Find out if the value exists
			bool exists = false;
			foreach (string s in this.Properties.Items)
			{
				if (!String.IsNullOrEmpty(s) &&
					s.ToLower() == this.Text.ToLower())
				{
					exists = true;
					break;
				}
			}
			e.AcceptValue = exists;
			base.OnPopupClosing(e);
		}

		protected override void ProcessAutoSearchChar(KeyPressEventArgs e)
		{
			//base.ProcessAutoSearchChar(e);
			if (Properties.ReadOnly) { return; }
			char charCode = e.KeyChar;
			if (Properties.CharacterCasing != CharacterCasing.Normal)
			{
				charCode = (Properties.CharacterCasing == CharacterCasing.Lower ? Char.ToLower(e.KeyChar) : Char.ToUpper(e.KeyChar));
			}
			if (Char.IsControl(charCode) && charCode != '\b') { return; }
			if (IsMaskBoxAvailable && Properties.Mask.MaskType != MaskType.None) { return; }
			KeyPressHelper helper = (IsMaskBoxAvailable ? new KeyPressHelper(MaskBox.MaskBoxText,
				SelectionStart, SelectionLength, Properties.MaxLength) :
				new KeyPressHelper(AutoSearchText, Properties.MaxLength));
			helper.ProcessChar(e.KeyChar);
			AutoSearchText = helper.Text;
			e.Handled = true;
			ProcessFindItem(helper, charCode);
		}

		protected override void ProcessFindItem(KeyPressHelper helper, char pressedKey)
		{
			//base.ProcessFindItem(helper, pressedKey);
			if (Properties.ReadOnly) return;
			string searchText = AutoSearchText;
			int itemIndex = FindItem(searchText, 0);
			bool isOpened = IsPopupOpen;
			if (IsImmediatePopup)
			{
				DoImmediatePopup(itemIndex, pressedKey);
				if (!isOpened && IsPopupOpen)
				{
					itemIndex = FindItem(searchText, 0);
				}
			}
			if (IsPopupOpen)
			{
				FindUpdatePopupSelectedItem(itemIndex);
			}
			if (itemIndex != -1)
			{
				FindUpdateEditValue(itemIndex, false);
				if (IsMaskBoxAvailable)
				{
					UpdateMaskBoxDisplayText();
					//SelectionStart = selectionStart;
					//SelectionLength = Text.Length - selectionStart;
					SelectionStart = helper.SelectionStart;
					SelectionLength = Text.Length - helper.SelectionStart;
				}
			}
			else
			{
				if (IsMaskBoxAvailable)
				{
					FindUpdateEditValueAutoSearchText();
					UpdateMaskBoxDisplayText();
					//SelectionStart = Math.Max(0, selectionStart);
					SelectionStart = Math.Max(0, helper.SelectionStart);
					SelectionLength = 0;
				}
				else
				{
					AutoSearchText = (AutoSearchText.Length > 1 ? AutoSearchText.Substring(0, AutoSearchText.Length - 1) : string.Empty);
				}
			}
			LayoutChanged();
		}

		//protected override void ProcessFindItem(int selectionStart, char pressedKey)
		//{
			//base.ProcessFindItem(selectionStart, pressedKey);
			//if (Properties.ReadOnly) return;
			//string searchText = AutoSearchText;
			//int itemIndex = FindItem(searchText, 0);
			//bool isOpened = IsPopupOpen;
			//if (IsImmediatePopup)
			//{
			//    DoImmediatePopup(itemIndex, pressedKey);
			//    if (!isOpened && IsPopupOpen)
			//    {
			//        itemIndex = FindItem(searchText, 0);
			//    }
			//}
			//if (IsPopupOpen)
			//{
			//    FindUpdatePopupSelectedItem(itemIndex);
			//}
			//if (itemIndex != -1)
			//{
			//    FindUpdateEditValue(itemIndex, false);
			//    if (IsMaskBoxAvailable)
			//    {
			//        UpdateMaskBoxDisplayText();
			//        SelectionStart = selectionStart;
			//        SelectionLength = Text.Length - selectionStart;
			//    }
			//}
			//else
			//{
			//    if (IsMaskBoxAvailable)
			//    {
			//        FindUpdateEditValueAutoSearchText();
			//        UpdateMaskBoxDisplayText();
			//        SelectionStart = Math.Max(0, selectionStart);
			//        SelectionLength = 0;
			//    }
			//    else
			//    {
			//        AutoSearchText = (AutoSearchText.Length > 1 ? AutoSearchText.Substring(0, AutoSearchText.Length - 1) : string.Empty);
			//    }
			//}
			//LayoutChanged();
		//}

		public void Load(List<string> list)
		{
			_list = list;
			this.Properties.Items.Clear();
			this.Properties.Items.AddRange(_list);
		}

		/// <summary>
		/// Select the item by the key specified
		/// </summary>
		/// <param name="key">The key to find</param>
		public void SelectByKey(string key)
		{
			bool found = false;
			for (int i = 0; i < this.Properties.Items.Count; i++)
			{
				if (((GenericListItem)this.Properties.Items[i]).Key.ToLower() == key.ToLower())
				{
					this.SelectedIndex = i;
					found = true;
					break;
				}
			}
			if (!found) { this.SelectedIndex = -1; }
		}

		/// <summary>
		/// Select the item by the value specified
		/// </summary>
		/// <param name="value">The value to find</param>
		public void SelectByValue(string value)
		{
			bool found = false;
			for (int i = 0; i < this.Properties.Items.Count; i++)
			{
				if (((GenericListItem)this.Properties.Items[i]).Value.ToLower() == value.ToLower())
				{
					this.SelectedIndex = i;
					found = true;
					break;
				}
			}
			if (!found) { this.SelectedIndex = -1; }
		}

		/// <summary>
		/// Select the first value in the lookup edit
		/// </summary>
		public void SelectFirstValue()
		{
			if (this.Properties.Items.Count == 0) { this.SelectedIndex = -1; }
			this.SelectedIndex = 0;
		}
	}

	[UserRepositoryItem("Register")]
	public class Repository_mgDevX_ComboEditStrings : RepositoryItemComboBox
	{
		private List<string> _list = new List<string>();
		private bool _addBlankRow = false;
		private string _blankRowText = string.Empty;

		public Repository_mgDevX_ComboEditStrings()
		{
			mgInit();
		}

		static Repository_mgDevX_ComboEditStrings()
		{
			Register();
		}

		public void mgInit()
		{
			this.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
			this.ImmediatePopup = true;
			this.NullText = "";
		}

		public static void Register()
		{
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("mgDevX_ComboEditStrings",
				typeof(mgDevX_ComboEditStrings),
				typeof(Repository_mgDevX_ComboEditStrings), 
				typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo),
				new DevExpress.XtraEditors.Drawing.ButtonEditPainter(),
				true));
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string EditorTypeName { get { return "mgDevX_ComboEditStrings"; } }

		public string BlankRowText
		{
			get { return _blankRowText; }
			set { _blankRowText = value; }
		}

		/// <summary>
		/// Clear out the control
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			this.Items.Clear();
		}

		public bool AddBlankRow
		{
			get { return _addBlankRow; }
			set { _addBlankRow = value; }
		}

		public void Load(List<string> list)
		{
			_list = list;
			this.Items.Clear();
			this.Items.AddRange(_list);
		}
	}
}
