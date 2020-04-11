using System;
using System.Collections.Generic;
using System.Text;

namespace mgControls
{
	public class mgDevX_RepositoryItemDateEdit : DevExpress.XtraEditors.Repository.RepositoryItemDateEdit
	{
		public mgDevX_RepositoryItemDateEdit()
			: base()
		{
			// When the control starts, set some of the properties
			this.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;

			// Handle an event to deal with lettered keys
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(mgDevX_RepositoryItemDateEdit_KeyUp);
		}

		private void mgDevX_RepositoryItemDateEdit_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			// Handle the event and do some trick stuff with lettering
			if (sender is DevExpress.XtraEditors.DateEdit)
			{
				if (!e.Control)
				{
					// They didn't hold the ctrl key down - don't put in the time
					switch (e.KeyCode)
					{
						case System.Windows.Forms.Keys.T:		// Today
							((DevExpress.XtraEditors.DateEdit)sender).EditValue = DateTime.Parse(DateTime.Now.ToShortDateString());
							//this.OwnerEdit.EditValue = 
							break;
						case System.Windows.Forms.Keys.Y:		// Yesterday
							//this.OwnerEdit.EditValue = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());
							((DevExpress.XtraEditors.DateEdit)sender).EditValue = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());
							break;
						case System.Windows.Forms.Keys.N:		// Tomorrow
							//this.OwnerEdit.EditValue = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString());
							((DevExpress.XtraEditors.DateEdit)sender).EditValue = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString());
							break;
					}
				}
				else
				{
					// They held the ctrl key down - put in the time
					switch (e.KeyCode)
					{
						case System.Windows.Forms.Keys.T:		// Today
							//this.OwnerEdit.EditValue = DateTime.Now;
							((DevExpress.XtraEditors.DateEdit)sender).EditValue = DateTime.Now;
							break;
						case System.Windows.Forms.Keys.Y:		// Yesterday
							//this.OwnerEdit.EditValue = DateTime.Now.AddDays(-1);
							((DevExpress.XtraEditors.DateEdit)sender).EditValue = DateTime.Now.AddDays(-1);
							break;
						case System.Windows.Forms.Keys.N:		// Tomorrow
							//this.OwnerEdit.EditValue = DateTime.Now.AddDays(1);
							((DevExpress.XtraEditors.DateEdit)sender).EditValue = DateTime.Now.AddDays(1);
							break;
					}
				}
			}
		}
	}
}
