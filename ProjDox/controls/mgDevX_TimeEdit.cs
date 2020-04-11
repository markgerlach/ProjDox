using System;
using System.Collections.Generic;
using System.Text;

namespace mgControls
{
	public class mgDevX_TimeEdit : mgControls.mgDevX_TextEdit
	{
		public mgDevX_TimeEdit()
			: base()
		{
			this.Properties.Mask.EditMask = "t";
			this.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
			this.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
		}

		/// <summary>
		/// Get's sets the date time on the control
		/// </summary>
		public DateTime? DateTime
		{
			get 
			{
				DateTime? datetime = null;
				if (this.EditValue == null) { return datetime; }
				DateTime testDate = System.DateTime.Now;
				bool rtv = System.DateTime.TryParse(this.EditValue.ToString(), out testDate);
				if (rtv) { datetime = testDate; }
				return datetime;
			}
			set 
			{ 
				if (!value.HasValue)
				{
					//this.Text = string.Empty;
					this.EditValue = null;
				}
				else
				{
					this.EditValue = value.Value.ToShortTimeString();
				}
			}
		}

		//public override object EditValue
		//{
		//    get
		//    {
		//        return base.EditValue;
		//    }
		//    set
		//    {
		//        base.EditValue = value;
		//    }
		//}
	}
}
