using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace mgControls
{
	public class mgDevX_ProgressBarControl : DevExpress.XtraEditors.ProgressBarControl
	{
		public mgDevX_ProgressBarControl()
			: base()
		{
			// Set some of the basic properties
			this.ForeColor = Color.White;
			this.Font = new Font(this.Font.Name, this.Font.Size, FontStyle.Bold);

			// Set some of the skinning on the control
			DevExpress.Skins.Skin currentSkin = null;
			//DevExpress.Skins.SkinElement element, elementBackground, emptyTextColor, filledTextColor;
			DevExpress.Skins.SkinElement element, elementBackground;

			DevExpress.LookAndFeel.DefaultLookAndFeel df = new DevExpress.LookAndFeel.DefaultLookAndFeel();
			df.LookAndFeel.SkinName = "Black";
			currentSkin = DevExpress.Skins.EditorsSkins.GetSkin(df.LookAndFeel);

			element = currentSkin[DevExpress.Skins.EditorsSkins.SkinProgressChunk];
			elementBackground = currentSkin[DevExpress.Skins.EditorsSkins.SkinProgressBorder];
			//emptyTextColor = currentSkin[DevExpress.Skins.EditorsSkins.SkinProgressBarEmptyTextColor];
			//filledTextColor = currentSkin[DevExpress.Skins.EditorsSkins.SkinProgressBarFilledTextColor];

			element.Color.BackColor = Color.DarkGreen;
			element.Color.BackColor2 = Color.LimeGreen;
			element.Color.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;

			elementBackground.Color.BackColor = Color.Black;
			elementBackground.Color.BackColor2 = Color.DarkGray;
			elementBackground.Color.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;

			// Don't set the empty at this time
			//empty

			//filledTextColor.Color.ForeColor = Color.White;
			currentSkin.Colors["ProgressBarEmptyTextColor"] = Color.White;
			currentSkin.Colors["ProgressBarFilledTextColor"] = Color.White;

			if (element.Image.Image != null)
			{
				element.Image.Image = null;
			}
			if (elementBackground.Image.Image != null)
			{
				elementBackground.Image.Image = null;
			}
		}
	}
}
