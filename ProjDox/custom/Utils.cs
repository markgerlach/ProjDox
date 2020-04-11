using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

using mgModel;

namespace mgCustom
{
	public class Utils
	{
		public static string LoadedDatabaseConnectionString = string.Empty;		// The connection string loaded from the config file

		public static readonly int SecurityLoginAccountLockoutLimit = 4;
		//public static bool UseSystemAsAdminSystem = false;		// Pay attention to the AdminSystems code in each form - set at login

		public static string CurrentTimeFormat = string.Empty;
		public static string CurrentDateFormat = string.Empty;

		public static string TempDirectoryName = "temp";

		public static int ClientAreaWidth = 0;
		public static int ClientAreaHeight = 0;

		public static string AttentuationPhrase = "_BaseWinProject Rocks!";

		public static string GemBoxLicenseString = "EIKT-XYMG-Y70R-IBYV";		// This is the 3.1 Key
		//public static string GemBoxLicenseString = "ESWL-XQA7-268A-ZBGB";

		public static string NavBarImagesLibraryFile = string.Empty;		//Application.StartupPath + @"\mwsImgNav.dll"

		// Create some variables to hold onto the app icons so we're only loading them once
		public static Dictionary<string, Icon> LargeAppIcons = new Dictionary<string, Icon>();
		public static Dictionary<string, Icon> SmallAppIcons = new Dictionary<string, Icon>();

		public static string ApplicationStartupDirectory = string.Empty;

		/// <summary>
		/// Caps all chars after special chars, otherwise, makes them lower
		/// </summary>
		/// <param name="text">The text to be converted</param>
		/// <returns>A string that has been formatted</returns>
		public static string InitCaps(string text)
		{
			if (String.IsNullOrEmpty(text)) { return string.Empty; }
			StringBuilder sb = new StringBuilder();

			bool nextCharShouldBeCapped = true;
			//string specialChars = @" -/'.\r\n";
			for (int i = 0; i < text.Length; i++)
			{
				if (nextCharShouldBeCapped)
				{
					sb.Append(text.Substring(i, 1).ToUpper());
				}
				else
				{
					sb.Append(text.Substring(i, 1).ToLower());
				}

				// Figure out if the next character should be capped
				nextCharShouldBeCapped = false;
				if (text.Substring(i, 1) == " " ||
					text.Substring(i, 1) == "-" ||
					text.Substring(i, 1) == "/" ||
					text.Substring(i, 1) == "'" ||
					text.Substring(i, 1) == "\r" ||
					text.Substring(i, 1) == "\n" ||
					(i > 0 && text.Substring(i - 1, 2).ToUpper() == "MC") ||
					text.Substring(i, 1) == ".")
				{
					nextCharShouldBeCapped = true;
				}
				//nextCharShouldBeCapped = (specialChars.IndexOf(text.Substring(i, 1)) > -1);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Tells if the generic list contains a string (case insensitive search)
		/// </summary>
		/// <param name="list">The list to hunt through</param>
		/// <param name="val">The value to find</param>
		/// <returns>True if exists, otherwise, false</returns>
		public static bool ListContainsString(List<string> list, string val)
		{
			bool rtv = false;

			if (String.IsNullOrEmpty(val))
			{
				// Check for null and return it
				rtv = list.Contains(val);
			}
			else
			{
				rtv = (null != list.Find(delegate(string str)
				{ return str.ToLower().Equals(val.ToLower()); }));
			}

			return rtv;
		}

		/// <summary>
		/// Alter the field caption from the field name
		/// </summary>
		/// <param name="fieldName">The field name to alter</param>
		/// <returns>A string with the field name</returns>
		public static string AlterCaptionFromFieldName(string fieldName)
		{
			string tmpOut = string.Empty;
			string workingVar = fieldName.Trim();

			// Shear off all the lower case from the front
			for (int i = 0; i < workingVar.Length; i++)
			{
				if ("abcdefghijklmnopqrstuvwxyz".ToUpper().IndexOf(workingVar.Substring(i, 1)) > -1)
				{
					workingVar = workingVar.Substring(i);
					break;
				}
			}

			// Get the actual value, putting space in
			bool prevLetterCapped = false, nextLetterCapped = false;
			for (int i = 0; i < workingVar.Length; i++)
			{
				if ("abcdefghijklmnopqrstuvwxyz".IndexOf(workingVar.Substring(i, 1)) > -1)
				{
					tmpOut += workingVar.Substring(i, 1);
				}
				else
				{
					// Check for a cap or not
					if (i > 0 &&
						"abcdefghijklmnopqrstuvwxyz_".IndexOf(workingVar.Substring(i - 1, 1)) > -1)
					{
						// This is going to be a cap
						if (workingVar.Substring(i, 1) != "_")
						{
							//tmpOut += " " + workingVar.Substring(i, 1);
							tmpOut = tmpOut.Trim() + " " + workingVar.Substring(i, 1);
						}
						else
						{
							tmpOut += " - ";
						}
					}
					else
					{
						tmpOut += workingVar.Substring(i, 1);
					}
				}
			}

			// Go through the string and figure out if you have any 
			// mashed caps (caps that are right next to one another
			// Will take "ContactSSNNumber" and turn it into "Contact SSN Number"
			for (int i = tmpOut.Length - 1; i >= 0; i--)
			{
				if (i > 1 &&
					//i < tmpOut.Length - 1 &&
					"ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(tmpOut.Substring(i - 1, 1)) > -1 &&
					"ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(tmpOut.Substring(i - 2, 1)) > -1 &&
					"abcdefghijklmnopqrstuvwxyz_".IndexOf(tmpOut.Substring(i, 1)) > -1)
				{
					// Dump a space in there
					tmpOut = tmpOut.Substring(0, i - 1) + "-" + tmpOut.Substring(i - 1);
				}
			}

			//if (tmpOut.ToLower().Contains(" - ")) { int test = 1; }
			return tmpOut;
		}

		/// <summary>
		/// Sorts a datatable and returns the sorted datatable
		/// </summary>
		/// <param name="dt">The datatable to sort</param>
		/// <param name="sortString">The string to use as a sort - use SQL syntax</param>
		/// <returns>The sorted datatable</returns>
		public static DataTable SortDataTable(DataTable dt, string sortString)
		{
			dt.TableName = dt.TableName.Replace("_Copy", "");
			DataTable dtOutput = new DataTable(dt.TableName + "_Copy");
			foreach (DataColumn col in dt.Columns)
			{
				dtOutput.Columns.Add(col.ColumnName, col.DataType, col.Expression);
			}

			// Now that you have the structure
			DataRow[] rows = dt.Select(string.Empty, sortString);
			for (int i = 0; i < rows.Length; i++)
			{
				dtOutput.ImportRow(rows[i]);
			}

			// Return the datatable
			return dtOutput;
		}

		/// <summary>
		/// Copy the passed data table to a new datatable
		/// </summary>
		public static DataTable CopyDataTable(DataTable dt)
		{
			return SortDataTable(dt, string.Empty);		// Return from the sort method
		}

		/// <summary>
		/// Combine the lists removing duplicate values
		/// </summary>
		/// <typeparam name="T">The type of the list to combine</typeparam>
		/// <param name="lists">The lists to combine</param>
		/// <param name="addBlank">Add a blank element to the collection</param>
		/// <returns>The combined lists</returns>
		public static List<T> CombineLists<T>(List<T>[] lists, bool addBlank)
		{
			List<T> rtv = new List<T>();

			// Add the elements in
			// Add the first list to the result set
			rtv.AddRange(lists[0]);

			// Go through each of the other lists
			for (int i = 1; i < lists.Length; i++)
			{
				foreach (T item in lists[i])
				{
					if (!rtv.Contains(item))
					{
						rtv.Add(item);
					}
				}
			}

			// Check for the blank
			if (addBlank)
			{
				bool blankExists = false;
				foreach (T item in rtv)
				{
					if (item is string && String.IsNullOrEmpty(item as string)) { blankExists = true; break; }
				}
				if (!blankExists)
				{
					if (typeof(string).IsAssignableFrom(typeof(T))) { (rtv as List<string>).Add(string.Empty); }
				}
			}

			// Sort the list
			rtv.Sort();

			return rtv;			// Return the list
		}

		/// <summary>
		/// Combine the dictionaries removing duplicate values
		/// </summary>
		/// <typeparam name="TKey">The key type to combine</typeparam>
		/// <typeparam name="TValue">The value type to combine</typeparam>
		/// <param name="dicts">The dictionaries to combine</param>
		/// <param name="addBlank">Add a blank element to the collection</param>
		/// <returns>The combined dictionaries</returns>
		public static Dictionary<TKey, TValue> CombineDictionaries<TKey, TValue>(Dictionary<TKey, TValue>[] dicts, bool addBlank)
		{
			Dictionary<TKey, TValue> rtv = new Dictionary<TKey, TValue>();

			// Add the elements in
			// Add the first list to the result set
			foreach (KeyValuePair<TKey, TValue> kvp in dicts[0])
			{
				rtv.Add(kvp.Key, kvp.Value);
			}

			// Go through each of the other dictionaries
			for (int i = 1; i < dicts.Length; i++)
			{
				foreach (KeyValuePair<TKey, TValue> kvp in dicts[i])
				{
					if (!rtv.ContainsKey(kvp.Key))
					{
						rtv.Add(kvp.Key, kvp.Value);
					}
				}
			}

			// Check for the blank
			if (addBlank)
			{
				bool blankExists = false;
				foreach (KeyValuePair<TKey, TValue> kvp in rtv)
				{
					if (kvp.Key is string && String.IsNullOrEmpty(kvp.Key as string)) { blankExists = true; break; }
				}
				if (!blankExists)
				{
					if (typeof(string).IsAssignableFrom(typeof(TKey))) { (rtv as List<string>).Add(string.Empty); }
				}
			}

			return rtv;			// Return the dictionary
		}

		/// <summary>
		/// Get the current time format from a system preference
		/// </summary>
		/// <returns>The time format as a string</returns>
		public static string GetTimeFormatAsString()
		{
			if (String.IsNullOrEmpty(Utils.CurrentTimeFormat))
			{
				//string currentTimeFormat =
				//    (GlobalCollections.AgencySettings != null &&
				//    GlobalCollections.AgencySettings.GetByCategoryKey("AgencyDisplay", "TimeFormat") != null ?
				//    GlobalCollections.AgencySettings.GetByCategoryKey("AgencyDisplay", "TimeFormat").DefaultStringValue : string.Empty);
				////string currentTimeFormat = Utils.Prefs.GetStringByDistinctName("TimeFormat");
				//string outFormat = string.Empty;

				//switch (currentTimeFormat.ToLower())
				//{
				//    case "military": outFormat = "HH:mm"; break;
				//    default: outFormat = "hh:mm tt"; break;
				//}

				//Utils.CurrentTimeFormat = outFormat;

				Utils.CurrentTimeFormat = "hh:mm tt";
			}

			return Utils.CurrentTimeFormat;
		}

		/// <summary>
		/// Get the current date format from a system preference
		/// </summary>
		/// <returns>The date format as a string</returns>
		public static string GetDateFormatAsString()
		{
			if (String.IsNullOrEmpty(Utils.CurrentDateFormat))
			{
				//string currentDateFormat =
				//    (GlobalCollections.AgencySettings != null &&
				//    GlobalCollections.AgencySettings.GetByCategoryKey("AgencyDisplay", "DateFormat") != null ?
				//    GlobalCollections.AgencySettings.GetByCategoryKey("AgencyDisplay", "DateFormat").DefaultStringValue : string.Empty);
				////string currentDateFormat = Utils.Prefs.GetStringByDistinctName("DateFormat");
				//if (String.IsNullOrEmpty(currentDateFormat)) { currentDateFormat = "MM/dd/yyyy"; }

				//Utils.CurrentDateFormat = currentDateFormat;

				Utils.CurrentDateFormat = "MM/dd/yyyy";
			}

			return Utils.CurrentDateFormat;
		}

		/// <summary>
		/// Get the current date and time format from a system preference
		/// </summary>
		/// <returns>The date and time format as a string</returns>
		public static string GetDateAndTimeFormatAsString()
		{
			return GetDateFormatAsString() + " " + GetTimeFormatAsString();
		}

		/// <summary>
		/// Get what the text color should be based on the background color
		/// </summary>
		/// <param name="background">The color of the background</param>
		/// <returns>The color of the text</returns>
		public static Color GetTextColorBasedOnBackgroundColor(Color background,
			Color darkBackgroundTextColor,
			Color lightBackgroundTextColor)
		{
			decimal avg = (decimal)background.R + (decimal)background.G + (decimal)background.B;
			if (avg <= (decimal)(420))
			{
				return darkBackgroundTextColor;			// This is the darker color
			}
			else
			{
				return lightBackgroundTextColor;		// This is the lighter color
			}
		}

		/// <summary>
		/// Return the text color based on the background color sent
		/// </summary>
		/// <param name="background">The background Color</param>
		/// <returns>The finished color</returns>
		public static Color GetTextColorBasedOnBackgroundColor(Color background)
		{
			return GetTextColorBasedOnBackgroundColor(background, Color.White, Color.Black);
		}

		/// <summary>
		/// Get the IP address(es) of the machine
		/// </summary>
		/// <returns>The list of addresses for the machine</returns>
		public static List<string> GetIPAddresses()
		{
			List<string> list = new List<string>();

			try
			{
				string localHostName = System.Net.Dns.GetHostName();
				System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(localHostName);
				foreach (System.Net.IPAddress addr in hostEntry.AddressList)
				{
					if (!addr.ToString().Contains(":"))
					{
						list.Add(addr.ToString());
					}
				}
			}
			catch (Exception ex)
			{

			}

			return list;		// Return the list out
		}

		/// <summary>
		/// Splits a string by using the split string sent in
		/// </summary>
		/// <param name="stringValue">The value to split</param>
		/// <param name="splitChars">The character sequence to use as a split sequence</param>
		/// <returns>The string array that has been split</returns>
		public static string[] SplitString(string stringValue, string splitChars)
		{
			string[] rtv = new string[]{};
			try
			{
				StringBuilder phrase = new StringBuilder();
				for (int i = 0; i < splitChars.Length; i++)
				{
					if (splitChars.Substring(i, 1) != "\r" &&
						splitChars.Substring(i, 1) != "\n" &&
						splitChars.Substring(i, 1) != "\t" &&
						splitChars.Substring(i, 1) != "\\" &&
						splitChars.Substring(i, 1) != "\'" &&
						splitChars.Substring(i, 1) != "\"" &&
						splitChars.Substring(i, 1) != "\b" &&
						splitChars.Substring(i, 1) != "\f" &&
						splitChars.Substring(i, 1) != "\v" &&
						splitChars.Substring(i, 1) != "x" &&
						splitChars.Substring(i, 1) != "_")
					{
						phrase.Append(@"\");
					}
					phrase.Append(splitChars.Substring(i, 1));
				}
				rtv = Regex.Split(stringValue, phrase.ToString(), RegexOptions.IgnoreCase);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return rtv;
		}

		/// <summary>
		/// Split the string and return a list
		/// </summary>
		/// <param name="stringValue"></param>
		/// <param name="splitChars"></param>
		/// <returns></returns>
		public static List<string> SplitStringToGenericList(string stringValue, string splitChars)
		{
			return new List<string>(Utils.SplitString(stringValue, splitChars));
		}

		/// <summary>
		/// Joins a string array by using the join character string sent in
		/// </summary>
		/// <param name="stringValues">The values to join</param>
		/// <param name="joinChars">The character sequence to use as a join sequence</param>
		/// <returns>The string that has been joined</returns>
		public static string JoinString(string[] stringValues, string joinChars)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in stringValues)
			{
				if (!String.IsNullOrEmpty(sb.ToString())) { sb.Append(joinChars); }
				sb.Append(s);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Joins a string array by using the join character string sent in
		/// </summary>
		/// <param name="stringValues">The values to join</param>
		/// <param name="joinChars">The character sequence to use as a join sequence</param>
		/// <returns>The string that has been joined</returns>
		public static string JoinString(List<string> stringValues, string joinChars)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in stringValues)
			{
				if (!String.IsNullOrEmpty(sb.ToString())) { sb.Append(joinChars); }
				sb.Append(s);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Get the default browser
		/// </summary>
		/// <returns>The default browser from the registry</returns>
		public static string GetDefaultBrowser()
		{
			string browser = string.Empty;
			Microsoft.Win32.RegistryKey key = null;
			try
			{

				key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				//trim off quotes
				browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
				if (!browser.EndsWith("exe"))
				{
					//get rid of everything after the ".exe"
					browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
				}
			}
			finally
			{
				if (key != null) { key.Close(); }
			}
			return browser;
		}

		/// <summary>
		/// Get the SQL Time
		/// </summary>
		/// <returns>Gets the SQL Time from the SQL Server based on the active connection</returns>
		public static void GetSQLTime(ref DateTime zuluTime, ref DateTime localTime)
		{
			string sql = "SELECT GETUTCDATE(), GETDATE()";
			SqlCommand cmd = null;
			DataTable dt = null;

			try
			{
				cmd = new SqlCommand(sql);
				dt = DAL.SQLExecDataTable(cmd);
				if (dt.Rows.Count > 0)
				{
					zuluTime = (DateTime)dt.Rows[0][0];
					localTime = (DateTime)dt.Rows[0][1];
				}
				
			}
			catch (Exception ex)
			{
				// Reset the times
				zuluTime =
					localTime =
					DateTime.Parse("1/1/1900");
			}
		}

		/// <summary>
		/// Take a standard string and use the split characters to make a name value collection
		/// </summary>
		public static NameValueCollection GetNameValueCollectionFromString(string target,
			string firstSplitChar,
			string secondSplitChar)
		{
			NameValueCollection nvc = new NameValueCollection();

			string[] itemsToParse = Utils.SplitString(target, firstSplitChar);
			foreach (string s in itemsToParse)
			{
				if (!String.IsNullOrEmpty(s))
				{
					// Take the value and split it again
					string[] nameValue = Utils.SplitString(s, secondSplitChar);
					if (nameValue.Length == 2 &&
						!String.IsNullOrEmpty(nameValue[0]) &&
						!String.IsNullOrEmpty(nameValue[1]))
					{
						nvc.Add(nameValue[0], nameValue[1]);
					}
				}
			}

			return nvc;
		}

		/// <summary>
		/// Find out if a value exists in the enum type passed
		/// This method is case-insensitive
		/// </summary>
		public static bool EnumValueExists(Type type, string value)
		{
			bool exists = false;

			Array ary = Enum.GetValues(type);
			for (int i = 0; i < ary.Length; i++)
			{
				if (ary.GetValue(i).ToString().ToLower() == value.ToLower())
				{
					exists = true;
					break;
				}
			}

			return exists;
		}

		/// <summary>
		/// Get an image for the nav button
		/// </summary>
		/// <param name="imgWrite">The image to put onto the background</param>
		/// <param name="backColor">The back color of the button</param>
		/// <param name="foreColor">The fore color of the button</param>
		/// <param name="borderColor">The border color of the button</param>
		/// <param name="text">The text to place on the button</param>
		/// <returns>The completed image</returns>
		public static Image GetNavButtonImage(Image imgWrite, Color backColor, Color foreColor, Color borderColor, string text)
		{
			Rectangle imageSize = new Rectangle(0, 0, 64, 64);

			Image img = new Bitmap(64, 64);
			Graphics g = Graphics.FromImage(img);

			Font font = new Font("Copperplate Gothic Light", 9, FontStyle.Bold);
			SizeF size = g.MeasureString(text, font, 56);

			float newX = (64 - size.Width) / 2;
			float newY = (64 - size.Height) / 2;

			g.FillRectangle(new SolidBrush(backColor), imageSize);
			if (borderColor != Color.Empty)
			{ g.DrawRectangle(new Pen(new SolidBrush(borderColor), 2), 0, 0, 62, 62); }
			if (imgWrite != null)
			{ g.DrawImage(imgWrite, 8, 8, 48, 48); }

			// Draw the string
			if (!String.IsNullOrEmpty(text))
			{
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				RectangleF drawRect = new RectangleF(2, newY, 60, size.Height);
				g.DrawString(text, font, new SolidBrush(foreColor), drawRect, format);
			}

			g.Dispose();		// Dispose of the item

			return img;			// Return the icon		
		}

		/// <summary>
		/// Get an image for the nav button
		/// </summary>
		/// <param name="text">The text to place on the button</param>
		/// <returns>The completed image</returns>
		public static Image GetNavButtonImage(string text)
		{
			return GetNavButtonImage(null, Color.White, Color.Navy, Color.Empty, text);		// Return the icon		
		}

		/// <summary>
		/// Sort the dictionary by type
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Dictionary<TKey, TValue> SortDictionary<TKey, TValue>(Dictionary<TKey, TValue> data)
			where TValue : IComparable<TValue>
		{
			List<KeyValuePair<TKey, TValue>> kvpList =
				  new List<KeyValuePair<TKey, TValue>>(data);
			kvpList.Sort(delegate(KeyValuePair<TKey, TValue> first, KeyValuePair<TKey, TValue> second)
			{ return first.Value.CompareTo(second.Value); });
			// Copy the elements back into the return set
			Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> kvp in kvpList) { result.Add(kvp.Key, kvp.Value); }
			return result;		// Return the collection
		}

		/// <summary>
		/// Check to make sure the db is around
		/// </summary>
		/// <returns>True if it is, otherwise, false</returns>
		public static bool CheckDB(bool resetConnStr)
		{
			return DAL.IsOnline(resetConnStr);
		}

		/// <summary>
		/// Remove the provider string from the string that was passed in
		/// </summary>
		/// <param name="connString">The connection string</param>
		/// <returns>The string with the new connection string</returns>
		public static string ParseDBStringForProvider(string connString)
		{
			string rtv = connString;

			List<string> lst = Utils.SplitStringToGenericList(rtv, ";");
			for (int i = lst.Count - 1; i >= 0; i--)
			{
				if (lst[i].ToLower().Replace(" ", "").Contains("provider="))
				{
					lst.RemoveAt(i);		// Remove it
				}
			}
			rtv = Utils.JoinString(lst, ";") + ";";

			return rtv;		// Return the string
		}

		#region Temp Dir Handling
		/// <summary>
		/// Checks to make sure there is a temp dir located under the 
		/// main directory and creates one if there's not
		/// </summary>
		/// <returns>The full path to the temp directory</returns>
		public static string CheckWinTempDir()
		{
			if (String.IsNullOrEmpty(Utils.ApplicationStartupDirectory)) 
			{
				return string.Empty;
			}
			string tempDir = Utils.ApplicationStartupDirectory + @"\" + TempDirectoryName;
			if (!Directory.Exists(tempDir)) { Directory.CreateDirectory(tempDir); }
			return tempDir;
		}

		/// <summary>
		/// Checks to make sure there is a temp dir located under the 
		/// main directory and creates one if there's not
		/// </summary>
		/// <returns>The full path to the temp directory</returns>
		public static string GetWinTempDir()
		{
			// Call the CheckWinTempDir function
			return CheckWinTempDir();
		}

		/// <summary>
		/// Returns a new temp file name without the extension but with the path
		/// </summary>
		/// <returns>The temp file name</returns>
		public static string GetWinTempFile()
		{
			string rtv = string.Empty;
			string tempDir = CheckWinTempDir();
			if (!String.IsNullOrEmpty(tempDir))
			{
				rtv = tempDir + @"\" + DateTime.Now.ToString("yyyyMMdd-hhmmss-ffff");
			}

			return rtv;
		}

		/// <summary>
		/// Returns a new temp file name with the extension appended
		/// </summary>
		/// <param name="extension">The extension to append to the file</param>
		/// <returns>The temp file complete with path</returns>
		public static string GetWinTempFile(string extension)
		{
			return GetWinTempFile() + "." +
				(extension.StartsWith(".") ? extension.Substring(1) : extension);
		}

		/// <summary>
		/// Remove any files from the temp directory
		/// </summary>
		public static void DeleteWinTempFiles()
		{
			string tempDir = GetWinTempDir();
			foreach (string file in Directory.GetFiles(tempDir))
			{
				try
				{
					File.Delete(file);
				}
				catch { }
			}
		}
		#endregion Temp Dir Handling

		/// <summary>
		/// Get the application icons from the system
		/// </summary>
		public static void GetAppIcons()
		{
			GetAppIcons(string.Empty);		// Call the empty method
		}

		/// <summary>
		/// Get the application icons from the system
		/// </summary>
		public static void GetAppIcons(string extension)
		{
			string tempDir = Utils.CheckWinTempDir();		// Check to make sure the temp directory is there
			string newFileName = string.Empty;

			// Try to delete any files in the temp directory
			Utils.DeleteWinTempFiles();

			// Clear out the collections
			Utils.SmallAppIcons = new Dictionary<string, Icon>();
			Utils.LargeAppIcons = new Dictionary<string, Icon>();

			//List<string> extensions = _BaseWinProject.Attachment.GetDistinctFromDB(AttachmentField.OriginalExtension);

			//// Check to see if the extension already exists in the collection
			//if (!String.IsNullOrEmpty(extension.ToLower()) &&
			//	!Utils.ListContainsString(extensions, extension.ToLower()))
			//{
			//	extensions.Add(extension.ToLower());		// Add the extension
			//}

			//foreach (string s in extensions)
			//{
			//	string ext = s.ToLower();
			//	bool valid = true;
			//	if (!ext.StartsWith("."))
			//	{
			//		// Modify it
			//		if (ext.LastIndexOf(".") > -1)
			//		{
			//			ext = ext.Substring(ext.LastIndexOf("."));
			//		}
			//		else
			//		{
			//			valid = false;
			//		}
			//	}
			//	if (valid &&
			//		!String.IsNullOrEmpty(ext) &&
			//		!String.IsNullOrEmpty(ext.Replace(".", "").Trim()))
			//	{
			//		newFileName = tempDir + @"\" + DateTime.Now.ToString("MMddyyyy-hhmmssffff") + ext;
			//		FileStream fs = File.Create(newFileName);
			//		fs.WriteByte(42);
			//		fs.Close();

			//		Icon ico = _BaseWinProject.Model.ExtractIcon.GetIcon(newFileName, true);		// Small
			//		if (ico != null &&
			//			!Utils.SmallAppIcons.ContainsKey(ext))
			//		{
			//			Utils.SmallAppIcons.Add(ext, ico);
			//		}

			//		ico = _BaseWinProject.Model.ExtractIcon.GetIcon(newFileName, false);		// Large
			//		if (ico != null &&
			//			!Utils.LargeAppIcons.ContainsKey(ext))
			//		{
			//			Utils.LargeAppIcons.Add(ext, ico);
			//		}
			//	}
			//}
		}

		/// <summary>
		/// Extract the digits from a string and pass the string back with only numbers
		/// </summary>
		/// <param name="strIn">The string to pass in and process</param>
		/// <returns>The completed string</returns>
		public static string ExtractDigits(string strIn)
		{
			// Replace invalid characters with empty strings.
			return System.Text.RegularExpressions.Regex.Replace(strIn, @"[^0-9]+", "");
		}

        public static string ExtractDecimalString(string strIn)
        {
            // Replace invalid characters with empty strings.
            return System.Text.RegularExpressions.Regex.Replace(strIn, @"[^0-9\.]+", "");
        }

		/// <summary>
		/// Convert an IP address to it's number equivalent for ordering purposes
		/// </summary>
		/// <param name="IP">The IP address to convert</param>
		/// <returns>The numeric version of that IP</returns>
		public static long ConvertIPToNum(string IP)
		{
			long ipValue = 0;
			try
			{
				string[] ipOctets = Regex.Split(IP, @"\.", RegexOptions.IgnoreCase);
				ipValue = (long)((double.Parse(ipOctets[0]) * Math.Pow(256, 3)) +
					(double.Parse(ipOctets[1]) * Math.Pow(256, 2)) +
					(double.Parse(ipOctets[2]) * 256) +
					double.Parse(ipOctets[3]));
			}
			catch (Exception ex)
			{
			}
			return ipValue;
		}

		/// <summary>
		/// Tells if the IP address passed in is a valid one
		/// </summary>
		/// <param name="IP">The IP Address</param>
		/// <returns>True if valid, otherwise false</returns>
		public static bool IPAddressValid(string IP)
		{
			bool rtv = true;

			string[] ipOctets = Regex.Split(IP, @"\.", RegexOptions.IgnoreCase);
			if (ipOctets.Length != 4)
			{
				rtv = false;
			}
			else if (String.IsNullOrEmpty(ipOctets[0]) ||
				String.IsNullOrEmpty(ipOctets[1]) ||
				String.IsNullOrEmpty(ipOctets[2]) ||
				String.IsNullOrEmpty(ipOctets[3]))
			{
				rtv = false;
			}
			return rtv;
		}

		/// <summary>
		/// Get all addresses between two IP addresses (not including the .0 or .255)
		/// </summary>
		public static List<string> GetAddressRange(string IP1, string IP2)
		{
			List<string> list = new List<string>();

			if (!Utils.IPAddressValid(IP1) ||
				!Utils.IPAddressValid(IP2))
			{
				return list;
			}
			long ip1Value = Utils.ConvertIPToNum(IP1);
			long ip2Value = Utils.ConvertIPToNum(IP2);

			string[] ip1Octets = Regex.Split(IP1, @"\.", RegexOptions.IgnoreCase);
			string[] ip2Octets = Regex.Split(IP2, @"\.", RegexOptions.IgnoreCase);
			int[] ip1IntOctet = new int[] {
				int.Parse(ip1Octets[0]),
				int.Parse(ip1Octets[1]),
				int.Parse(ip1Octets[2]),
				int.Parse(ip1Octets[3]),
			};
			int[] ip2IntOctet = new int[] {
				int.Parse(ip2Octets[0]),
				int.Parse(ip2Octets[1]),
				int.Parse(ip2Octets[2]),
				int.Parse(ip2Octets[3]),
			};

			#region Old Code
			//int[] startingOctet = (ip1Value < ip2Value ? ip1IntOctet : ip2IntOctet);
			//int[] endingOctet = (ip1Value < ip2Value ? ip2IntOctet : ip1IntOctet);
			//for (int i1 = startingOctet[0]; i1 <= endingOctet[0]; i1++)
			//{
			//    for (int i2 = startingOctet[1]; i2 <= endingOctet[1]; i1++)
			//    {
			//        for (int i3 = startingOctet[2]; i3 <= endingOctet[2]; i1++)
			//        {
			//            for (int i4 = startingOctet[3]; i4 <= endingOctet[3]; i1++)
			//            {
			//                if (i4 + 
			//            }
			//        }
			//    }
			//}
			#endregion Old Code

			List<long> octets = new List<long>();
			for (long i = Math.Min(ip1Value, ip2Value); i <= Math.Max(ip1Value, ip2Value); i++)
			{
				// Break it back down
				long val = i;
				octets = new List<long>();
				for (int j = 3; j > 0; j--)
				{
					long res = (long)Math.Floor(val / Math.Pow(256, j));
					octets.Add(res);
					val -= (long)(res * Math.Pow(256, j));
				}
				octets.Add(val);

				// Throw it into the list
				if (octets[3] != 0 &&
					octets[3] != 255)
				{
					list.Add(octets[0] + "." + octets[1] + "." + octets[2] + "." + octets[3]);
				}
			}

			return list;
		}

		///// <summary>
		///// Send an email to multiple users
		///// </summary>
		///// <param name="to">The addresses to send the email to</param>
		///// <param name="cc">The addresses to send the email to as a CC</param>
		///// <param name="subject">The subject of the email</param>
		///// <param name="body">The body of the email</param>
		///// <param name="appVersion">The application version</param>
		///// <returns>An Error collection with the elements needed</returns>
		//public static ClassGenExceptionCollection SendEMail(List<string> to,
		//    List<string> cc,
		//    string subject,
		//    string body,
		//    string appVersion,
		//    List<System.Net.Mail.Attachment> attachments)
		//{
		//    return SendEMail("_BaseWinProject@test.com",
		//        to, 
		//        cc, 
		//        subject, 
		//        body, 
		//        appVersion,
		//        attachments);
		//}

		///// <summary>
		///// Send an email to multiple users
		///// </summary>
		///// <param name="to">The addresses to send the email to</param>
		///// <param name="cc">The addresses to send the email to as a CC</param>
		///// <param name="subject">The subject of the email</param>
		///// <param name="body">The body of the email</param>
		///// <param name="appVersion">The application version</param>
		///// <returns>An Error collection with the elements needed</returns>
		//public static ClassGenExceptionCollection SendEMail(List<string> to,
		//    List<string> cc,
		//    string subject,
		//    string body,
		//    string appVersion)
		//{
		//    return SendEMail("_BaseWinProject@test.com",
		//        to,
		//        cc,
		//        subject,
		//        body,
		//        appVersion,
		//        new List<System.Net.Mail.Attachment>());
		//}

		///// <summary>
		///// Send an email to the respective client
		///// </summary>
		///// <param name="to">The address to send the email to</param>
		///// <param name="subject">The subject of the email</param>
		///// <param name="body">The body of the email</param>
		///// <param name="appVersion">The application version</param>
		///// <returns>An Error collection with the elements needed</returns>
		//public static ClassGenExceptionCollection SendEMail(string to,
		//    string subject,
		//    string body,
		//    string appVersion)
		//{
		//    return SendEMail("_BaseWinProject@test.com",
		//        new List<string>(new string[] { to }), 
		//        new List<string>(), 
		//        subject, 
		//        body, 
		//        appVersion,
		//        new List<System.Net.Mail.Attachment>());
		//}

		///// <summary>
		///// Send an email to the respective client
		///// </summary>
		///// <param name="to">The addresses to send the email to</param>
		///// <param name="cc">The addresses to send the email to as a CC</param>
		///// <param name="subject">The subject of the email</param>
		///// <param name="body">The body of the email</param>
		///// <param name="from">The address send the email from</param>
		///// <param name="appVersion">The application version</param>
		///// <param name="attachments">A collection of attachments to send as part of the email</param>
		///// <returns>An Error collection with the elements needed</returns>
		//public static ClassGenExceptionCollection SendEMail(string from,
		//    List<string> to,
		//    List<string> cc,
		//    string subject,
		//    string body,
		//    string appVersion,
		//    List<System.Net.Mail.Attachment> attachments)
		//{
		//    ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
		//    string server = "10.1.1.15";

		//    MailMessage message = new MailMessage();
		//    message.From = new MailAddress(from);
		//    message.To.Add(Utils.JoinString(to, ","));
		//    if (cc.Count > 0) { message.CC.Add(Utils.JoinString(cc, ",")); }
		//    message.Subject = subject;
		//    message.Body = body;

		//    if (attachments.Count > 0)
		//    {
		//        foreach (System.Net.Mail.Attachment att in attachments)
		//        {
		//            message.Attachments.Add(att);
		//        }
		//    }
			
		//    SmtpClient client = new SmtpClient(server);

		//    // Credentials are necessary if the server requires the client 
		//    // to authenticate before it will send e-mail on the client's behalf.
		//    client.UseDefaultCredentials = true;

		//    try
		//    {
		//        client.Send(message);

		//        // Write a log trans messsage about who received the email
		//        LogTrans.WriteRecordSendEmail(message.Body, appVersion, "System Import");
		//    }
		//    catch (Exception ex)
		//    {
		//        errors.Add(new ClassGenException(ex));
		//    }

		//    return errors;      // Return the collection
		//}

		/// <summary>
		/// Calculate the standard deviation for a set of numbers
		/// </summary>
		/// <returns>The decimal standard deviation</returns>
		public static decimal CalcMean(List<decimal> list)
		{
			// Get the mean first
			decimal mean = 0, working = 0;
			foreach (decimal d in list)
			{
				working += d;
			}
			mean = working / list.Count;		// This is the mean
			return mean;						// Return the number
		}

		/// <summary>
		/// Calculate the standard deviation for a set of numbers
		/// </summary>
		/// <returns>The decimal standard deviation</returns>
		public static decimal CalcStandardDeviation(List<decimal> list)
		{
			// Get the mean first
			decimal mean = 0, working = 0;
			foreach (decimal d in list)
			{
				working += d;
			}
			mean = working / list.Count;		// This is the mean

			// Next, populate the standard deviation
			decimal workingTotal = 0;
			foreach (decimal d in list)
			{
				working = (d - mean) * (d - mean);
				workingTotal += working;
			}
			decimal popStdDev = (decimal)Math.Sqrt((double)workingTotal / list.Count);
			return popStdDev;			// Return the number
		}

		/// <summary>
		/// Returns the mean for a set of numbers in a table
		/// </summary>
		/// <param name="dt">The data table to search</param>
		/// <param name="columnName">The column name to use for the calc</param>
		/// <returns>The std. dev.</returns>
		public static decimal CalcMean(DataTable dt, string columnName)
		{
			List<decimal> list = new List<decimal>();
			foreach (DataRow row in dt.Rows)
			{
				if (row[columnName] != DBNull.Value)
				{
					list.Add((decimal)row[columnName]);
				}
			}
			decimal rtv = 0;
			if (list.Count > 0) { rtv = CalcMean(list); }
			return rtv;
		}

		/// <summary>
		/// Returns the std. dev for a set of numbers in a table
		/// </summary>
		/// <param name="dt">The data table to search</param>
		/// <param name="columnName">The column name to use for the calc</param>
		/// <returns>The std. dev.</returns>
		public static decimal CalcStandardDeviation(DataTable dt, string columnName)
		{
			List<decimal> list = new List<decimal>();
			foreach (DataRow row in dt.Rows)
			{
				if (row[columnName] != DBNull.Value)
				{
					list.Add((decimal)row[columnName]);
				}
			}
			decimal rtv = 0;
			if (list.Count > 0) { rtv = CalcStandardDeviation(list); }
			return rtv;
		}

		/// <summary>
		/// Returns all the numbers from a string, like for an improperly formatted phone number
		/// </summary>
		/// <param name="tmp">The value to parse</param>
		/// <returns>The numbers from the string</returns>
		public static string GetNumbersFromString(string tmp)
		{
			string val = string.Empty;
			for (int i = 0; i < tmp.Length; i++)
			{
				if (tmp.Substring(i, 1).IndexOfAny("0123456789".ToCharArray()) > -1)
				{
					val += tmp.Substring(i, 1);
				}
			}
			return val;
		}
	}
}
