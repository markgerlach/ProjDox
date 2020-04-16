using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace mgControls
{
	public class mgLayoutControl : DevExpress.XtraLayout.LayoutControl
	{
		public mgLayoutControl()
			: base()
		{
			this.Root.GroupBordersVisible = false;

			DevExpress.Skins.Skin currentSkin = null;
			DevExpress.Skins.Skin tabSkin = null;
			DevExpress.Skins.Skin navBarSkin = null;
			DevExpress.Skins.Skin formSkin = null;

			DevExpress.Skins.SkinElement elementForm, elementPanel, elementPanelNoBorder,layoutItem;
			DevExpress.Skins.SkinElement panelBottom, panelTop, panelLeft, panelRight;
			DevExpress.Skins.SkinElement layoutItemPadding, layoutGroupPadding, layoutRootGroupPadding;
			DevExpress.Skins.SkinElement layoutGroupWithoutBordersPadding, layoutRootGroupWithoutBordersPadding;
			
            DevExpress.LookAndFeel.DefaultLookAndFeel df = new DevExpress.LookAndFeel.DefaultLookAndFeel();
            df.LookAndFeel.SkinName = "Black";
            currentSkin = DevExpress.Skins.CommonSkins.GetSkin(df.LookAndFeel);
			tabSkin = DevExpress.Skins.TabSkins.GetSkin(df.LookAndFeel);
			navBarSkin = DevExpress.Skins.NavBarSkins.GetSkin(df.LookAndFeel);
			formSkin = DevExpress.Skins.FormSkins.GetSkin(df.LookAndFeel);

			elementForm = currentSkin[DevExpress.Skins.CommonSkins.SkinForm];
            elementPanel = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanel];
            elementPanelNoBorder = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelNoBorder];
            layoutItem = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutItemBackground];
			panelBottom = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelBottom];
			panelTop = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelTop];
			panelLeft = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelLeft];
			panelRight = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelRight];

			layoutItemPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutItemPadding];
			layoutGroupPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutGroupPadding];
			layoutRootGroupPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutRootGroupPadding];
			layoutGroupWithoutBordersPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutGroupWithoutBordersPadding];
			layoutRootGroupWithoutBordersPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutRootGroupWithoutBordersPadding];

			// Change the padding on elements
			layoutItemPadding.ContentMargins.All = 5;
			layoutGroupPadding.ContentMargins.All = 0;
			layoutRootGroupPadding.ContentMargins.All = 0;
			layoutGroupWithoutBordersPadding.ContentMargins.All = 0;
			//layoutGroupWithoutBordersPadding.ContentMargins.Bottom = 12;
			layoutRootGroupWithoutBordersPadding.ContentMargins.All = 0;

			if (elementPanel.Image.Image != null)
			{
				elementPanel.Image.Image = null;
			}

			DevExpress.Skins.SkinElement element;
			element = tabSkin[DevExpress.Skins.TabSkins.SkinTabPane];

			DevExpress.Skins.SkinElement navbar = navBarSkin[DevExpress.Skins.NavBarSkins.SkinBackground];

			//layoutItem.Color.BackColor =
			//    elementForm.Color.BackColor =
			//    elementPanel.Color.BackColor =
			//    elementPanelNoBorder.Color.BackColor =
			//    panelBottom.Color.SolidImageCenterColor =
			//    panelTop.Color.SolidImageCenterColor =
			//    panelLeft.Color.SolidImageCenterColor =
			//    panelRight.Color.SolidImageCenterColor =
			//    element.Color.SolidImageCenterColor =
			//    navbar.Color.BackColor =
			//    Color.White;

			//if (elementPanel.Image.Image != null)
			//{
			//    elementPanel.Image.Image = null;
			//}

			this.BackColor = Color.White;
			SetLayoutBackColor();

            AllowCustomization = false;
		}

		/// <summary>
		/// Set the layout back color
		/// </summary>
		private void SetLayoutBackColor()
		{
			DevExpress.Skins.Skin currentSkin = null;
			DevExpress.Skins.Skin tabSkin = null;
			DevExpress.Skins.Skin navBarSkin = null;
			DevExpress.Skins.Skin formSkin = null;

			DevExpress.Skins.SkinElement elementForm, elementPanel, elementPanelNoBorder, layoutItem;
			DevExpress.Skins.SkinElement panelBottom, panelTop, panelLeft, panelRight;
			DevExpress.Skins.SkinElement layoutItemPadding, layoutGroupPadding, layoutRootGroupPadding;
			DevExpress.Skins.SkinElement layoutGroupWithoutBordersPadding, layoutRootGroupWithoutBordersPadding;

			DevExpress.LookAndFeel.DefaultLookAndFeel df = new DevExpress.LookAndFeel.DefaultLookAndFeel();
			df.LookAndFeel.SkinName = "Black";
			currentSkin = DevExpress.Skins.CommonSkins.GetSkin(df.LookAndFeel);
			tabSkin = DevExpress.Skins.TabSkins.GetSkin(df.LookAndFeel);
			navBarSkin = DevExpress.Skins.NavBarSkins.GetSkin(df.LookAndFeel);
			formSkin = DevExpress.Skins.FormSkins.GetSkin(df.LookAndFeel);

			elementForm = currentSkin[DevExpress.Skins.CommonSkins.SkinForm];
			elementPanel = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanel];
			elementPanelNoBorder = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelNoBorder];
			layoutItem = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutItemBackground];
			panelBottom = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelBottom];
			panelTop = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelTop];
			panelLeft = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelLeft];
			panelRight = currentSkin[DevExpress.Skins.CommonSkins.SkinGroupPanelRight];

			layoutItemPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutItemPadding];
			layoutGroupPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutGroupPadding];
			layoutRootGroupPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutRootGroupPadding];
			layoutGroupWithoutBordersPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutGroupWithoutBordersPadding];
			layoutRootGroupWithoutBordersPadding = currentSkin[DevExpress.Skins.CommonSkins.SkinLayoutRootGroupWithoutBordersPadding];

			// Change the padding on elements
			//layoutItemPadding.ContentMargins.All = 5;
			//layoutGroupPadding.ContentMargins.All = 0;
			//layoutRootGroupPadding.ContentMargins.All = 0;
			//layoutGroupWithoutBordersPadding.ContentMargins.All = 0;
			//layoutRootGroupWithoutBordersPadding.ContentMargins.All = 0;

			if (elementPanel.Image.Image != null)
			{
				elementPanel.Image.Image = null;
			}

			DevExpress.Skins.SkinElement element;
			element = tabSkin[DevExpress.Skins.TabSkins.SkinTabPane];

			DevExpress.Skins.SkinElement navbar = navBarSkin[DevExpress.Skins.NavBarSkins.SkinBackground];

			layoutItem.Color.BackColor =
				elementForm.Color.BackColor =
				elementPanel.Color.BackColor =
				elementPanelNoBorder.Color.BackColor =
				//panelBottom.Color.SolidImageCenterColor =
				//panelTop.Color.SolidImageCenterColor =
				//panelLeft.Color.SolidImageCenterColor =
				//panelRight.Color.SolidImageCenterColor =
				element.Color.SolidImageCenterColor =
				navbar.Color.BackColor =
				base.BackColor;

			if (elementPanel.Image.Image != null)
			{
				elementPanel.Image.Image = null;
			}
		}

		///// <summary>
		///// Use this to modify settings on a load in the designer
		///// </summary>
		//public override DevExpress.XtraLayout.BaseLayoutItem CreateLayoutItem(DevExpress.XtraLayout.LayoutGroup parent)
		//{
		//    DevExpress.XtraLayout.BaseLayoutItem item = base.CreateLayoutItem(parent);
		//    item.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
		//    return item;
		//}

		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				SetLayoutBackColor();		// Set the layout back color
			}
		}
	}
}


