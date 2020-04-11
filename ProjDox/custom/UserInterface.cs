using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using mgModel;

namespace mgCustom
{
	public static class UserInterface
	{
		/// <summary>
		/// Gets the default application icon for the project
		/// </summary>
		/// <returns>The default Icon that the application will use</returns>
		public static Icon GetDefaultApplicationIcon()
		{
			try
			{
				return null;
				//return global::_BaseWinProject.Win.Common.resIcon._BaseWinProjectLogo_Base_120x120_Blue;
				//FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
				//string filePath = info.DirectoryName;
				//Assembly assy = Assembly.LoadFile(filePath + @"\mwsCommon.dll");

				//Bitmap bmp = new Bitmap(Image.FromStream(assy.GetManifestResourceStream("mwsCommon.images.icons.App.ico")));
				//Icon ico = Icon.FromHandle(bmp.GetHicon());
				//bmp.Dispose();
				//return ico;
			}
			catch
			{
				return null;
			}
		}

		#region Grid Interaction
		public static void GridUIAction(DevExpress.XtraGrid.Views.Grid.GridView gridView, GridUIActionType action)
		{
			gridView.BeginUpdate();
			switch (action)
			{
				case GridUIActionType.ExpandAllGroups:
					gridView.ExpandAllGroups();
					//for (int i = 0; i < gridView.RowCount; i++)
					//{
					//    gridView.ExpandGroupRow(i, true);
					//}
					break;
				case GridUIActionType.CollapseAllGroups:
					gridView.CollapseAllGroups();
					//for (int i = 0; i < gridView.RowCount; i++)
					//{
					//    gridView.CollapseGroupRow(i, true);
					//}
					break;
				case GridUIActionType.ExpandAllDetails:
					for (int i = 0; i < gridView.RowCount; i++)
					{
						gridView.ExpandMasterRow(i);
					}
					break;
				case GridUIActionType.CollapseAllDetails:
					for (int i = 0; i < gridView.RowCount; i++)
					{
						gridView.CollapseMasterRow(i);
					}
					break;
			}
			gridView.EndUpdate();
		}
		#endregion Grid Interaction

		/// <summary>
		/// Get a grayscale version of the image
		/// </summary>
		/// <param name="img">The image to convert</param>
		/// <returns>The new image (in grayscale)</returns>
		public static Image GetGrayscaleImage(Image img)
		{
			if (img == null) { return img; }

			Bitmap bm = new Bitmap(img, img.Width, img.Height);

			for (int y = 0; y < bm.Height; y++)
			{
				for (int x = 0; x < bm.Width; x++)
				{
					Color c = bm.GetPixel(x, y);
					int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
					bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
				}
			}
			bm.MakeTransparent();

			return bm;
		}

		/// <summary>
		/// Change the cursor to a wait cursor
		/// </summary>
		public static void WaitCursor()
		{
			Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
		}

		/// <summary>
		/// Change the cursor to the default cursor
		/// </summary>
		public static void DefaultCursor()
		{
			Cursor.Current = System.Windows.Forms.Cursors.Default;
		}

		/// <summary>
		/// Size the image and keep its aspect ratio in tact
		/// Either a height or width is passed in to use as a max level
		/// </summary>
		public static Image SizeImageKeepAspectRatio(Image img,
			int? width, int? height)
		{
			int newWidth = 0, newHeight = 0;
			if (img == null) { return img; }

			// Check to make sure they didn't pass in both
			if (width != null &&
				height != null)
			{
				throw new Exception("You can't pass in both Height and Width to this method...");
				return null;
			}

			Bitmap bmp = new Bitmap(img);

			// Check the width
			if (width != null)
			{
				newWidth = width.Value;
				newHeight = (int)(((decimal)newWidth * (decimal)bmp.Height) / (decimal)bmp.Width);
			}

			// Check the height
			if (height != null)
			{
				newHeight = height.Value;
				newWidth = (int)(((decimal)newHeight * (decimal)bmp.Width) / (decimal)bmp.Height);
			}

			Image returnImage = new Bitmap(img, newWidth, newHeight);
			return returnImage;     // Return the image
		}

		/// <summary>
		/// Get the company image 
		/// </summary>
		/// <returns>The company image</returns>
		public static System.Drawing.Image GetCompanyImage()
		{
			//return ResLibrary.resFull.RippleLogo_25;
			return null;
		}

		/// <summary>
		/// Get the company name
		/// </summary>
		/// <returns>The company's name</returns>
		public static string GetCompanyName()
		{
			return "AA Jewel Box";
		}
	}
}
