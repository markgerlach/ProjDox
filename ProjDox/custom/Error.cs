using System;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.IO;
using System.Windows.Forms;

using mgModel;
using ProjDox;

namespace mgCustom
{
	/// <summary>
	/// Summary description for Error.
	/// </summary>
	public class Error
	{
		public Error()
		{
		}

		/// <summary>
		/// Get the error file name path 
		/// </summary>
		/// <returns>String containing the filename to write errors to</returns>
		private static string GetErrorFileName()
		{
			DateTime dtNow = DateTime.Now;
			string errFileName = Application.StartupPath;
			if (errFileName.Substring(errFileName.Length - 1, 1) != @"\") { errFileName += @"\"; }
			errFileName += @"Logs\";
			if (!Directory.Exists(errFileName)) { Directory.CreateDirectory(errFileName); }
			string assyName = Assembly.GetExecutingAssembly().GetName().ToString();
			errFileName += assyName.Substring(0, assyName.IndexOf(", ")) + "_" +
				dtNow.ToString("yyyyMMdd") + ".txt";
			return errFileName;		// Return the string
		}

		/// <summary>
		/// Write an exception of type Exception to an external file
		/// </summary>
		/// <param name="e">The Exception object</param>
		public static void WriteErrorLog(Exception e, bool showError)
		{
			try
			{
				// Build the Error string
				StringWriter sw = new StringWriter();
				sw.WriteLine(String.Format(DateTime.Now.ToString(), "hh:mm:ss tt"));
				sw.WriteLine("Message: " + e.Message);
				sw.WriteLine("Source: " + e.Source);
				sw.WriteLine("StackTrace: " + e.StackTrace);
				sw.WriteLine("---------------------------------");

				// Write the file
				string errFileName = GetErrorFileName();
				TextWriter wt = File.AppendText(errFileName);
				TextWriter.Synchronized(wt);
				wt.WriteLine(sw.ToString());
				wt.Close();

				Trace.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": Wrote to File: " + errFileName);
				if (showError)
				{
					DisplayError(e);
					//					Utils.PostErrorToURL(sw.ToString());
				}
			}
			catch (Exception err)
			{
				Trace.Write("Could not write to file..." + err.Message);
			}
		}

		public static void WriteErrorLog(Exception e)
		{
			WriteErrorLog(e, true);		// Call the other handler
		}

		/// <summary>
		/// Write an exception of type SqlException to an external file
		/// </summary>
		/// <param name="e">The SQLException object</param>
		public static void WriteErrorLog(SqlException e, bool showError)
		{
			try
			{
				// Build the Error string
				StringWriter sw = new StringWriter();
				sw.WriteLine(String.Format(DateTime.Now.ToString(), "hh:mm:ss tt"));
				sw.WriteLine("Message: " + e.Message);
				sw.WriteLine("Source: " + e.Source);
				sw.WriteLine("Procedure: " + e.Procedure);
				sw.WriteLine("Server: " + e.Server);
				sw.WriteLine("State: " + e.State);
				sw.WriteLine("Number: " + e.Number);
				sw.WriteLine("StackTrace: " + e.StackTrace);
				sw.WriteLine("---------------------------------");

				// Write the file
				string errFileName = GetErrorFileName();
				TextWriter wt = File.AppendText(errFileName);
				TextWriter.Synchronized(wt);
				wt.WriteLine(sw.ToString());
				wt.Close();

				Trace.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": Wrote to File: " + errFileName);
				if (showError)
				{
					DisplayError(e);
					//					Utils.PostErrorToURL(sw.ToString());
				}
			}
			catch (Exception err)
			{
				Trace.Write("Could not write to file..." + err.Message);
			}
		}

		public static void WriteErrorLog(SqlException e)
		{
			WriteErrorLog(e, true);		// Call the other handler
		}

		/// <summary>
		/// Write an exception of type SqlException to an external file
		/// </summary>
		/// <param name="e">The SQLException object</param>
		/// <param name="sql">The SQL String that's being run</param>
		public static void WriteErrorLog(SqlException e, string sql)
		{
			try
			{
				// Build the Error string
				StringWriter sw = new StringWriter();
				sw.WriteLine(String.Format(DateTime.Now.ToString(), "hh:mm:ss tt"));
				sw.WriteLine("Message: " + e.Message);
				sw.WriteLine("Source: " + e.Source);
				sw.WriteLine("Procedure: " + e.Procedure);
				sw.WriteLine("Server: " + e.Server);
				sw.WriteLine("State: " + e.State);
				sw.WriteLine("Number: " + e.Number);
				sw.WriteLine("StackTrace: " + e.StackTrace);
				sw.WriteLine("SQL: " + sql);
				sw.WriteLine("---------------------------------");

				// Write the file
				string errFileName = GetErrorFileName();
				TextWriter wt = File.AppendText(errFileName);
				TextWriter.Synchronized(wt);
				wt.WriteLine(sw.ToString());
				wt.Close();

				Trace.Write("Wrote to File: " + errFileName);
				DisplayError(e);
			}
			catch (SqlException sqle) { Trace.Write("SQL Error " + sqle.Message + "\r\n" + sqle.InnerException); }
			catch (Exception err)
			{
				Trace.Write("Could not write to file..." + err.Message);
			}
		}

		/// <summary>
		/// Get the current stack
		/// </summary>
		/// <returns>The current stack listed as a string</returns>
		public static string GetCurrentStack()
		{
			StackTrace st = new StackTrace(true);
			StringBuilder sb = new StringBuilder();
			foreach (StackFrame frame in st.GetFrames())
			{
				sb.Append(frame + Environment.NewLine);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Write an exception of type string to an external file
		/// </summary>
		/// <param name="error">The string for the error to write</param>
		public static void WriteErrorLog(string error, bool showError)
		{
			try
			{
				// Write the file
				string errFileName = GetErrorFileName();
				TextWriter wt = File.AppendText(errFileName);
				TextWriter.Synchronized(wt);

				wt.WriteLine(String.Format(DateTime.Now.ToString(), "hh:mm:ss tt") + "\r\n" + error);
				wt.WriteLine("---------------------------------");
				wt.Close();

				Trace.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + ": Wrote to File: " + errFileName);
				if (showError)
				{
					DisplayError(new Exception(error));
					//					Utils.PostErrorToURL(error);
				}
			}
			catch (Exception err)
			{
				Trace.Write("Could not write to file..." + err.Message);
			}
		}

		public static void WriteErrorLog(string error)
		{
			WriteErrorLog(error, true);		// Call the other handler
		}

		public static void DisplayError(Exception error)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.Append("There was an error in the application.  Please review the following information." + Environment.NewLine + Environment.NewLine);
			sb.Append("Error Description" + Environment.NewLine);
			sb.Append(error.Message + Environment.NewLine + Environment.NewLine);
			sb.Append("Error Source:" + Environment.NewLine);
			sb.Append(error.Source + Environment.NewLine);
#if DEBUG_NO_SPLASH || DEBUG
			sb.Append(Environment.NewLine + "Stack Trace:" + Environment.NewLine);
			sb.Append(error.StackTrace + Environment.NewLine);
#endif
			MessageBox.Show(sb.ToString(), "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static bool DisplayCustomError(string error)
		{
			return DisplayCustomError(new ClassGenException(error, ClassGenExceptionIconType.Critical));
		}

		public static bool DisplayCustomInformation(string error)
		{
			return DisplayCustomError(new ClassGenException(error, ClassGenExceptionIconType.Information));
		}

		/// <summary>
		/// Display a custom error
		/// </summary>
		/// <param name="error">The error that should be shown</param>
		public static bool DisplayCustomError(ClassGenException error)
		{
			bool continueProcessing = true;

			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();
			errors.Add(error);
			frmDialogErrorView frm = new frmDialogErrorView(errors);

			if (errors.CriticalExceptionCount == 0) { frm.ContinuableForm = true; }
			if (frm.ShowDialog() == DialogResult.Cancel)
			{
				continueProcessing = false;
			}

			return continueProcessing;
		}
	}
}
