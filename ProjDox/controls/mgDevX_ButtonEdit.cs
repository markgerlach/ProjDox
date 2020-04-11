using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace mgControls
{
	public class mgDevX_ButtonEdit : DevExpress.XtraEditors.ButtonEdit
	{
		public mgDevX_ButtonEdit()
			: base()
		{
			this.KeyUp -= new System.Windows.Forms.KeyEventHandler(mgDevX_ButtonEdit_KeyUp);
			this.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(mgDevX_ButtonEdit_ButtonClick);

			this.KeyUp += new System.Windows.Forms.KeyEventHandler(mgDevX_ButtonEdit_KeyUp);
			this.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(mgDevX_ButtonEdit_ButtonClick);
			
			this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
		}

		private void mgDevX_ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			// When they click on the right button, 
			if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
			{
				// Trigger the event
				this.OnButtonEllipsisClicked((this.EditValue != null ? this.EditValue.ToString() : string.Empty));
			}
		}

		private void mgDevX_ButtonEdit_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			// When they hit a key, see if it's the edit key
			if (e.KeyData == Keys.F12)
			{
				// Throw the edit event
				this.OnButtonEllipsisClicked((this.EditValue != null ? this.EditValue.ToString() : string.Empty));
			}
		}

		public delegate void ButtonEllipsisClickedEventHandler(object sender, ButtonEllipsisClicked_EventArgs e);
		public event ButtonEllipsisClickedEventHandler ButtonEllipsisClicked;
		protected void OnButtonEllipsisClicked(string existingValue)
		{
			if (ButtonEllipsisClicked != null)
			{
				ButtonEllipsisClicked_EventArgs e = new ButtonEllipsisClicked_EventArgs(existingValue);
				ButtonEllipsisClicked(this, e);
			}
		}
	}

	#region ButtonEllipsisClicked_EventArgs
	public class ButtonEllipsisClicked_EventArgs : EventArgs
	{
		private string _existingValue = string.Empty;

		public ButtonEllipsisClicked_EventArgs(string existingValue)
		{
			_existingValue = existingValue;
		}

		public ButtonEllipsisClicked_EventArgs()
		{
		}

		public string ExistingValue
		{
			get { return _existingValue; }
		}
	}
	#endregion ButtonEllipsisClicked_EventArgs
}

