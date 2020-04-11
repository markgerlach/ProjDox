using System;
using System.Collections.Generic;
using System.Text;

namespace mgControls
{
	public class mgDevX_DateEdit : DevExpress.XtraEditors.DateEdit
	{
		public mgDevX_DateEdit()
			: base()
		{
			// When the control is created, set the properties accordingly
			this.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
			this.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.False;
			this.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;

			// Handle an event for a keystroke entered in the control
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(mgDevX_DateEdit_KeyUp);

			this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
		}

		private void mgDevX_DateEdit_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			// Handle the event and do some trick stuff with lettering
			if (!e.Control)
			{
				// They didn't hold the ctrl key down - don't put in the time
				switch (e.KeyCode)
				{
					case System.Windows.Forms.Keys.T:		// Today
						this.EditValue = DateTime.Parse(DateTime.Now.ToShortDateString());
						e.Handled = true;
						break;
					case System.Windows.Forms.Keys.Y:		// Yesterday
						this.EditValue = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());
						e.Handled = true;
						break;
					case System.Windows.Forms.Keys.N:		// Tomorrow
						this.EditValue = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString());
						e.Handled = true;
						break;
				}
			}
			else
			{
				// They held the ctrl key down - put in the time
				switch (e.KeyCode)
				{
					case System.Windows.Forms.Keys.T:		// Today
						this.EditValue = DateTime.Now;
						e.Handled = true;
						break;
					case System.Windows.Forms.Keys.Y:		// Yesterday
						this.EditValue = DateTime.Now.AddDays(-1);
						e.Handled = true;
						break;
					case System.Windows.Forms.Keys.N:		// Tomorrow
						this.EditValue = DateTime.Now.AddDays(1);
						e.Handled = true;
						break;
				}
			}
		}
	}
}
