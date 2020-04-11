using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using mgModel;

namespace mgControls
{
	public class mgDevX_ErrorProvider : DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider, 
		IExtenderProvider, ISupportInitialize
	{
		private Image _userDefinedIcon1 = null;
		private Image _userDefinedIcon2 = null;
		private Image _userDefinedIcon3 = null;
		private System.Windows.Forms.BindingSource _bindingSource = new System.Windows.Forms.BindingSource();
		private bool _objectBound = false;

		public mgDevX_ErrorProvider()
			: base()
		{
			DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIcon +=
				  new DevExpress.XtraEditors.DXErrorProvider.GetErrorIconEventHandler(DXErrorProvider_GetErrorIcon);
		}

		public mgDevX_ErrorProvider(ContainerControl parentControl)
			: base(parentControl)
		{
			DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIcon +=
				new DevExpress.XtraEditors.DXErrorProvider.GetErrorIconEventHandler(DXErrorProvider_GetErrorIcon);
		}

		public mgDevX_ErrorProvider(IContainer container)
			: base(container)
		{
			DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIcon +=
				new DevExpress.XtraEditors.DXErrorProvider.GetErrorIconEventHandler(DXErrorProvider_GetErrorIcon);
		}

		public void DXErrorProvider_GetErrorIcon(DevExpress.XtraEditors.DXErrorProvider.GetErrorIconEventArgs e)
		{
			switch (e.ErrorType)
			{
				case DevExpress.XtraEditors.DXErrorProvider.ErrorType.User1:
					e.ErrorIcon = _userDefinedIcon1;
					break;
				case DevExpress.XtraEditors.DXErrorProvider.ErrorType.User2:
					e.ErrorIcon = _userDefinedIcon2;
					break;
				case DevExpress.XtraEditors.DXErrorProvider.ErrorType.User3:
					e.ErrorIcon = _userDefinedIcon3;
					break;
				//case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning:
				//    e.ErrorIcon = global::ssCommon.res16x16.warning;
				//    break;
				//case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Information:
				//    e.ErrorIcon = global::ssCommon.res16x16.information;
				//    break;
				//case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical:
				//case DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default:
				//default:
				//    e.ErrorIcon = global::ssCommon.res16x16.Error_3_16_ent_vl;
				//    break;
			}
		}

		[Browsable(false)]
		public new object DataSource
		{
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool ObjectBound
		{
			get { return _objectBound; }
			set { _objectBound = value; }
		}

		[Browsable(false)]
		public new string DataMember
		{
			get { return base.DataMember; }
			set { base.DataMember = value; }
		}

		public System.Windows.Forms.BindingSource XBindingSource
		{
			get { return _bindingSource; }
			set { _bindingSource = value; }
		}

		public Image UserDefinedIcon1
		{
			get { return _userDefinedIcon1; }
			set { _userDefinedIcon1 = value; }
		}

		public Image UserDefinedIcon2
		{
			get { return _userDefinedIcon2; }
			set { _userDefinedIcon2 = value; }
		}

		public Image UserDefinedIcon3
		{
			get { return _userDefinedIcon3; }
			set { _userDefinedIcon3 = value; }
		}

		public void BindToObjectCollection<T, K>(ClassGenBindingList<T, K> list) where T : mgModel_BaseObject
		{
			_objectBound = false;
			if (this.Container != null) { _bindingSource = new System.Windows.Forms.BindingSource(this.Container); }
			else { _bindingSource = new System.Windows.Forms.BindingSource(); }
			
			this.DataSource = _bindingSource;
			_bindingSource.DataSource = typeof(T);
			//_bindingSource.DataSource = list[0].GetType();
			foreach (object o in list)
			{
				_bindingSource.Add(o);
			}
			_objectBound = true;
		}

		public void BindToObject(object obj)
		{
			_objectBound = false;
			if (this.Container != null) { _bindingSource = new System.Windows.Forms.BindingSource(this.Container); }
			else { _bindingSource = new System.Windows.Forms.BindingSource(); }
			
			this.DataSource = _bindingSource;
			_bindingSource.DataSource = obj.GetType();
			_bindingSource.Add(obj);
			_objectBound = true;
		}

		public void ReplaceBoundObject(object obj)
		{
			ClearBindings();
			_objectBound = false;
			_bindingSource.Add(obj);
			_objectBound = true;
		}

		public void ClearBindings()
		{
			_bindingSource.Clear();
			_objectBound = false;
		}
	}
}
