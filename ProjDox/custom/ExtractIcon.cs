using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Text;

namespace mgCustom
{
	/// <summary>
	/// Summary description for ExtractIcon.
	/// </summary>
	public class ExtractIcon
	{
		[DllImport("Shell32.dll")]
		private static extern int SHGetFileInfo
			(
			string pszPath,
			uint dwFileAttributes,
			out SHFILEINFO psfi,
			uint cbfileInfo,
			SHGFI uFlags
			);

		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			public SHFILEINFO(bool b)
			{
				hIcon = IntPtr.Zero; iIcon = 0; dwAttributes = 0; szDisplayName = ""; szTypeName = "";
			}
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
			public string szTypeName;
		};

		private ExtractIcon()
		{
		}

		private enum SHGFI
		{
			SmallIcon = 0x00000001,
			LargeIcon = 0x00000000,
			Icon = 0x00000100,
			DisplayName = 0x00000200,
			Typename = 0x00000400,
			SysIconIndex = 0x00004000,
			UseFileAttributes = 0x00000010
		}

		/// <summary>
		/// Get the associated Icon for a file or application, this method always returns
		/// an icon.  If the strPath is invalid or there is no icon the default icon is returned
		/// </summary>
		/// <param name="strPath">full path to the file</param>
		/// <param name="bSmall">if true, the 16x16 icon is returned otherwise the 32x32</param>
		/// <returns></returns>
		public static Icon GetIcon(string strPath, bool bSmall)
		{
			SHFILEINFO info = new SHFILEINFO(true);
			int cbFileInfo = Marshal.SizeOf(info);
			SHGFI flags;
			if (bSmall)
			{
				flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
			}
			else
			{
				flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;
			}

			SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
			return Icon.FromHandle(info.hIcon);
		}


		/// <summary>
		/// Get the icon from the temp file created
		/// </summary>
		/// <param name="extension">The extension to use with the file</param>
		/// <returns>The image for the file association</returns>
		public static Image GetIcon(string extension)
		{
			string tempDir = Utils.CheckWinTempDir();		// Check to make sure the temp directory is there

			// Try to delete any files in the temp directory
			Utils.DeleteWinTempFiles();

			string newFileName = tempDir + @"\" + DateTime.Now.ToString("MMddyyyy-hhmmssffff") +
				extension;
			FileStream fs = File.Create(newFileName);
			fs.WriteByte(42);
			fs.Close();

			return ((Icon)GetIcon(newFileName, true)).ToBitmap();
		}
	}
}
