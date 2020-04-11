using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Security.Permissions;

using mgModel;

namespace mgControls
{
	public class mgDevX_SimpleButton : DevExpress.XtraEditors.SimpleButton
	{
		private mgButtonGUIButtonColor _buttonColor = mgButtonGUIButtonColor.Default;

		public mgDevX_SimpleButton()
			: base()
		{
			// Set up the button the way you want
			SetCustom();
		}

		/// <summary>
		/// Resets the button to a default view
		/// </summary>
		public void ResetToDefault()
		{
			this.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			this.Appearance.Font = new System.Drawing.Font(this.Appearance.Font.Name,
				this.Appearance.Font.Size);
		}

		/// <summary>
		/// Set the button up for a custom type
		/// </summary>
		public void SetCustom()
		{
			// Make sure you null out the style controller
			if (this.StyleController != null) { this.StyleController = null; }
				
			this.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
			this.Appearance.Font = new System.Drawing.Font(this.Appearance.Font.Name,
				this.Appearance.Font.Size,
				System.Drawing.FontStyle.Bold);
		}

		public override DevExpress.XtraEditors.IStyleController StyleController
		{
			get
			{
				return null;
			}
			set
			{
				base.StyleController = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(mgButtonGUIButtonColor.Default)]
		public mgButtonGUIButtonColor ButtonColor
		{
			get { return _buttonColor; }
			set
			{
				// Make sure you null out the style controller
				if (this.StyleController != null) { this.StyleController = null; }

				_buttonColor = value;
				SetCustom();		// Set the custom layout of the screen
				switch (_buttonColor)
				{
					case mgButtonGUIButtonColor.Red:
						//button1.BackgroundImage = lblEnabledNormalRed.Appearance.ImageBackground;
						this.Appearance.BackColor = Color.FromArgb(192, 0, 0);	// Firebrick
						this.Appearance.BackColor2 = Color.FromArgb(60, 0, 0);	// Darker red
						this.Appearance.ForeColor = Color.White;
						break;
					case mgButtonGUIButtonColor.Green:
						//button1.BackgroundImage = lblEnabledNormalGreen.Appearance.ImageBackground;
						this.Appearance.BackColor = Color.FromArgb(0, 192, 0);	// Dark Green
						this.Appearance.BackColor2 = Color.FromArgb(0, 60, 0);	// Darker Green
						this.Appearance.ForeColor = Color.White;
						break;
					case mgButtonGUIButtonColor.Yellow:
						//button1.BackgroundImage = lblEnabledNormalYellow.Appearance.ImageBackground;
						this.Appearance.BackColor = Color.Yellow;	// Yellow
						this.Appearance.BackColor2 = Color.FromArgb(192, 192, 0);	// Darker yellow
						this.Appearance.ForeColor = Color.Black;
						break;
					default:
						//button1.BackgroundImage = lblEnabledNormalBlue.Appearance.ImageBackground;
						this.Appearance.BackColor = Color.FromArgb(0, 0, 192);	// Navy
						this.Appearance.BackColor2 = Color.FromArgb(0, 0, 60);	// Darker blue
						this.Appearance.ForeColor = Color.White;
						break;
				}
				this.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			}
		}
	}
}
