using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Text;

using mgModel;

namespace mgCustom
{
	public class SystemUpdates
	{
		#region CreateBDAUsers
		///// <summary>
		///// Get a listing of the BDA accounts
		///// </summary>
		///// <returns>The dictionary of users/bda accounts</returns>
		////public static Dictionary<string, User> GetBDAAccounts()
		//public static List<string> GetBDAAccounts()
		//{
		//    //Dictionary<string, User> bdadmins = new Dictionary<string, User>();
		//    List<string> bdadmins = new List<string>();

		//    bdadmins.Add("mgerlachadmin");
		//    bdadmins.Add("aturneradmin");

		//    return bdadmins;
		//}
		#endregion CreateBDAUsers

		#region Run Update(s)
		/// <summary>
		/// Run Individual Updates
		/// </summary>
		/// <param name="errors">The errors collection</param>
		/// <returns>True if it ran the update - otherwise, false</returns>
		//public bool RunIndividualUpdate(SystemUpdateType updateType, ref ClassGenExceptionCollection errors)
		//{
		//	bool updatesRun = false;
		//	LogTrans logItem = null;

		//	// Get the machine name
		//	string machName = "Machine Name: " + System.Environment.MachineName;
		//	if (machName.Trim().Length >= 50) { machName = machName.Substring(50); }

		//	try
		//	{
		//		// Log the update
		//		logItem = new LogTrans();
		//		logItem.LogGUID = System.Guid.NewGuid().ToString();
		//		logItem.Action = ActionType.RunSystemUtils.ToString();
		//		logItem.ActionType = updateType.ToString();
		//		logItem.DateLogged = DateTime.Now;
		//		logItem.Desc = "Individual Update";
		//		logItem.MachineName = machName;
		//		errors.AddRange(logItem.AddUpdate());

		//		// Run the update
		//		switch (updateType)
		//		{
		//			//case SystemUpdateType.UpdateSystemSettingsTable: errors.AddRange(UpdateSystemSettingsTable()); break;
		//			//case SystemUpdateType.CreateBDAUsers: errors.AddRange(CreateBDAUsers()); break;
		//			//case SystemUpdateType.UpdateSecObjects: errors.AddRange(UpdateSecObjects()); break;
		//			//case SystemUpdateType.RemoveDuplicateEmployeeRecords: errors.AddRange(RemoveDuplicateEmployeeRecords()); break;
		//			//case SystemUpdateType.FixUserExtendedProperty: errors.AddRange(FixUserExtendedProperty()); break;

		//			case SystemUpdateType.ResetBDAPermissions: errors.AddRange(ResetBDAPermissions()); break;
		//			//case SystemUpdateType.UpdateSystemAddressBook: errors.AddRange(UpdateSystemAddressBook()); break;
		//			//case SystemUpdateType.UpdateNULLStatus: errors.AddRange(UpdateNULLStatus()); break;
		//			//case SystemUpdateType.KillNonLinkedEmployees: errors.AddRange(KillNonLinkedEmployees()); break;
		//			//case SystemUpdateType.UpdateNonExistentBins: errors.AddRange(UpdateNonExistentBins()); break;

		//			//case SystemUpdateType.UpdateNULLColors: errors.AddRange(UpdateNULLColors()); break;
		//			//case SystemUpdateType.DeleteNULLEmployees: errors.AddRange(DeleteNULLEmployees()); break;
		//			//case SystemUpdateType.CheckMixedSSNs: errors.AddRange(CheckMixedSSNs()); break;
		//			//case SystemUpdateType.FixSearches: errors.AddRange(FixSearches()); break;
		//			//case SystemUpdateType.UpdateTrainingBlockSortOrder: errors.AddRange(UpdateTrainingBlockSortOrder()); break;

		//			//case SystemUpdateType.BuildWFScheduleForDay: errors.AddRange(BuildWFScheduleForDay()); break;
		//			//case SystemUpdateType.CleanRTFTable: errors.AddRange(CleanRTFTable()); break;

		//			case SystemUpdateType.StartUpdate:
		//			case SystemUpdateType.EndUpdate:
		//				break;
		//		}

		//		// Check to see if we have any errors
		//		if (errors.WarningExceptionCount == 0 &&
		//			errors.CriticalExceptionCount == 0)
		//		{
		//			updatesRun = true;
		//		}
		//		else
		//		{
		//			// Log the error to the database
		//			ClassGenExceptionCollection errorLogging = new ClassGenExceptionCollection();
		//			foreach (ClassGenException err in errors)
		//			{
		//				if (err.ClassGenExceptionIconType == ClassGenExceptionIconType.Critical ||
		//					err.ClassGenExceptionIconType == ClassGenExceptionIconType.Warning)
		//				{
		//					logItem = new LogTrans();

		//					logItem.LogGUID = System.Guid.NewGuid().ToString();
		//					logItem.Action = ActionType.RunSystemUtils.ToString();
		//					logItem.ActionType = updateType.ToString();
		//					logItem.DateLogged = DateTime.Now;
		//					logItem.Desc = "Error: " + err.DescriptionWithException;
		//					logItem.UserName = machName;
		//					errorLogging.AddRange(logItem.AddUpdate());
		//				}
		//			}
		//			errors.Clear();		// Clear out the errors
		//			int test = 1;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		errors.Add(new ClassGenException(ex));

		//		// Log the error to the database
		//		logItem = new LogTrans();

		//		logItem.LogGUID = System.Guid.NewGuid().ToString();
		//		logItem.Action = ActionType.RunSystemUtils.ToString();
		//		logItem.ActionType = updateType.ToString();
		//		logItem.DateLogged = DateTime.Now;
		//		logItem.Desc = "Error: " + ex.Message +
		//			(!String.IsNullOrEmpty(ex.InnerException.Message) ? "  " + ex.InnerException.Message : string.Empty);
		//		logItem.UserName = machName;
		//		errors.AddRange(logItem.AddUpdate());
		//	}

		//	return updatesRun;		// Return whether or not they were run
		//}

		/// <summary>
		/// Run the updates on the system
		/// </summary>
		/// <param name="errors">The errors collection run into</param>
		/// <returns>Returns true if updates were run, otherwise, false</returns>
		//public bool RunUpdates(ref ClassGenExceptionCollection errors)
		//{
		//	LogTrans logItem = new LogTrans();
		//	string machName = "Machine Name: " + System.Environment.MachineName;
		//	SqlCommand cmd = null;
		//	string sql = string.Empty;
		//	int recsAffected = 0;
		//	DataTable dt = null;
		//	errors = new ClassGenExceptionCollection();
		//	bool updatesRun = false;

		//	// Check to see if they've already been run today
		//	// And check to see if they've done an update since the last run
		//	DateTime? rtv = null;
		//	AppDBVersionCollection versionCollection = new AppDBVersionCollection(string.Empty);
		//	foreach (AppDBVersion item in versionCollection)
		//	{
		//		if (rtv == null ||
		//			item.DateVersionDate > rtv) { rtv = item.DateVersionDate; }
		//	}
		//	DateTime dtLastDBUpdate = (rtv.HasValue ? rtv.Value : DateTime.Parse("1/1/1900"));
		//	DateTime now = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy"));

		//	// Get the maximum date
		//	DateTime maxDate = (dtLastDBUpdate > now ? dtLastDBUpdate : now);
		//	LogTransCollection coll = new LogTransCollection("dtLogged >= '" +
		//		maxDate.ToString("MM/dd/yyyy hh:mm tt") + "' " +
		//		"AND dtLogged <= '" + maxDate.ToString("MM/dd/yyyy") + " 11:59:59 PM' " +
		//		"AND sAction = '" +
		//		ActionType.RunSystemUtils.ToString() + "'");
		//	if (coll.Count > 0)
		//	{
		//		// See if we had an error in the previous run
		//		coll.Sort(LogTrans.FN_DateLogged + " DESC");
		//		bool errorExists = false;
		//		foreach (LogTrans l in coll)
		//		{
		//			if (l.Desc.ToLower().StartsWith("error"))
		//			{
		//				errorExists = true;
		//				break;
		//			}
		//			else if (l.ActionType.ToLower() == SystemUpdateType.StartUpdate.ToString().ToLower())
		//			{
		//				break;
		//			}
		//		}
		//		if (!errorExists)
		//		{
		//			// We're good - return
		//			logItem = new LogTrans();
		//			logItem.LogGUID = System.Guid.NewGuid().ToString();
		//			logItem.Action = ActionType.RunSystemUtils.ToString();
		//			logItem.ActionType = "Information";
		//			logItem.DateLogged = DateTime.Now;
		//			logItem.Desc = "System Updates Have Already Been Run Today...";
		//			if (machName.Trim().Length >= 50) { machName = machName.Substring(50); }
		//			logItem.UserName = machName;
		//			errors.AddRange(logItem.AddUpdate());

		//			return updatesRun;
		//		}
		//	}

		//	// If they haven't, run them
		//	updatesRun = true;

		//	DataTable dtUpdates = SystemUpdates.GetUpdateSchema();
		//	//int totalCount = 15, currentIndex = 0;
		//	int totalCount = dtUpdates.Rows.Count, currentIndex = 0;

		//	// Go through each element and run the update
		//	//// Check to see if we failed on the last update (today's)
		//	//ContactCollection existingContacts = new ContactCollection("iStatusCode = 1");
		//	//UserCollection existingUsers = new UserCollection("sContactGUID IN (SELECT sContactGUID FROM tContact WHERE iStatusCode = 1)");
		//	foreach (DataRow row in dtUpdates.Rows)
		//	{
		//		this.OnSystemUpdateRun(row["Key"].ToString(), row["Description"].ToString(), currentIndex, totalCount);
		//		currentIndex++;

		//		// Get the type of update to run
		//		SystemUpdateType updateType = (SystemUpdateType)Enum.Parse(typeof(SystemUpdateType), row["Key"].ToString());
		//		RunIndividualUpdate(updateType, ref errors);
		//	}

		//	// Log the transaction
		//	logItem = new LogTrans();
		//	logItem.LogGUID = System.Guid.NewGuid().ToString();
		//	logItem.Action = ActionType.RunSystemUtils.ToString();
		//	logItem.ActionType = "Information";
		//	logItem.DateLogged = DateTime.Now;
		//	logItem.Desc = "System Updates Run - (SystemUpdates.RunUpdates)";
		//	machName = "Machine Name: " + System.Environment.MachineName;
		//	if (machName.Trim().Length >= 50) { machName = machName.Substring(50); }
		//	logItem.UserName = machName;

		//	errors.AddRange(logItem.AddUpdate());

		//	return updatesRun;		// Run the updates
		//}

		///// <summary>
		///// Get the update schema
		///// </summary>
		///// <returns>A DataTable containing the update schema</returns>
		//public static DataTable GetUpdateSchema()
		//{
		//	DataTable dt = new DataTable();

		//	dt.Columns.Add("Key", typeof(System.String));
		//	dt.Columns.Add("Description", typeof(System.String));
		//	dt.Columns.Add("Progress", typeof(System.Int32));

		//	// Add all the updates in there
		//	dt.Rows.Add(new object[] { SystemUpdateType.StartUpdate.ToString(), "Starting Updates", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateSystemSettingsTable.ToString(), "Updating System Settings Table", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.CreateBDAUsers.ToString(), "Creating Backdoor Admins", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateSecObjects.ToString(), "Updating the System Objects Table", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.RemoveDuplicateEmployeeRecords.ToString(), "Remove Duplicate Employees", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.FixUserExtendedProperty.ToString(), "Fix User Extended Properties", 0 });

		//	dt.Rows.Add(new object[] { SystemUpdateType.ResetBDAPermissions.ToString(), "Updating Backdoor Admin Permissions", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateSystemAddressBook.ToString(), "Updating System Address Book", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateNULLStatus.ToString(), "Updating NULL Status Elements", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.KillNonLinkedEmployees.ToString(), "Kill Employees that do not have a contact", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateNonExistentBins.ToString(), "Updating Non-existent Bins - Workforce", 0 });

		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateNULLColors.ToString(), "Updating NULL Colors", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.DeleteNULLEmployees.ToString(), "Deleting NULL Employees", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.CheckMixedSSNs.ToString(), "Checking Mixed Security on SSN in Contacts", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.FixSearches.ToString(), "Fixing Searches to not have erroneous info", 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.UpdateTrainingBlockSortOrder.ToString(), "Updating the Training Block Sorting", 0 });

		//	//dt.Rows.Add(new object[] { SystemUpdateType.BuildWFScheduleForDay.ToString(), "Building Workforce Schedule for Day: " + 
		//	//    GetDateForWFBuild().ToShortDateString(), 0 });
		//	//dt.Rows.Add(new object[] { SystemUpdateType.CleanRTFTable.ToString(), "Cleaning the RTF Table of Orphaned Records", 0 });

		//	dt.Rows.Add(new object[] { SystemUpdateType.EndUpdate.ToString(), "End of Updates", 0 });

		//	return dt;
		//}
		#endregion Run Update(s)

		#region ResetBDAPermissions
		/// <summary>
		/// Resets the Back door administrator permissions
		/// </summary>
		public static ClassGenExceptionCollection ResetBDAPermissions()
		{
			SqlCommand cmd = null;
			string sql = string.Empty;
			ClassGenExceptionCollection errors = new ClassGenExceptionCollection();

			try
			{
				#region SQL
				sql = "DECLARE @BDAGroupGUID uniqueidentifier " + Environment.NewLine +

					"SELECT	@BDAGroupGUID = g.sGroupGUID " + Environment.NewLine +
					"FROM	[dbo].[tSecGroup]	AS [g] " + Environment.NewLine +
					"WHERE	g.bSystemGroup <> 0 " + Environment.NewLine +

					"IF @BDAGroupGUID IS NOT NULL " + Environment.NewLine +
					"	BEGIN " + Environment.NewLine +
					"		BEGIN TRANSACTION " + Environment.NewLine + 
					//"		INSERT INTO [dbo].[tSecUserToGroup]	( " +
					//"					sGroupGUID, " +
					//"					sUserGUID, " +
					//"					sUpdatedByUser ) " +
					//"		SELECT		@BDAGroupGUID, " +
					//"					u.sUserGUID, " +
					//"					'system autoconfig' " +
					//"		FROM		[dbo].[tUser]			AS [u] " +
					//"		INNER JOIN	[dbo].[tContact]		AS [c] ON u.sContactGUID 	= c.sContactGUID " +
					//"		WHERE		c.iStatusCode = 1 " +
					//"			AND		u.sUserGUID NOT IN " +
					//"					( " +
					//"						SELECT		m.sUserGUID " +
					//"						FROM		[dbo].[tSecUserToGroup] AS [m] " +
					//"						WHERE		m.sGroupGUID = @BDAGroupGUID " +
					//"					) " +

					//"		IF @@ERROR <> 0 " +
					//"			BEGIN " +
					//"				GOTO ERROR_HANDLER " +
					//"			END " +

					"		UPDATE	[dbo].[tSecGroupToObject] " + Environment.NewLine + 
					"		SET		[iAccessLevel] = 9 " + Environment.NewLine +
					"		WHERE	[sGroupGUID] = @BDAGroupGUID " + Environment.NewLine +
					"			AND	[iAccessLevel] <> 9 " + Environment.NewLine +

					"		IF @@ERROR <> 0 " + Environment.NewLine +
					"			BEGIN " + Environment.NewLine + 
					"				GOTO ERROR_HANDLER " + Environment.NewLine +
					"			END " + Environment.NewLine +

					"		INSERT INTO	[dbo].[tSecGroupToObject] ( " + Environment.NewLine +
					"					sObjectGUID, " + Environment.NewLine +
					"					sGroupGUID, " + Environment.NewLine +
					"					iAccessLevel) " + Environment.NewLine +
					"		SELECT		o.sObjectGUID, " + Environment.NewLine +
					"					@BDAGroupGUID, " + Environment.NewLine +
					"					9 " + Environment.NewLine +
					"		FROM		[dbo].[tSecObject] AS [o] " + Environment.NewLine +
					"		WHERE		sObjectGUID NOT IN " + Environment.NewLine +
					"					( " + Environment.NewLine +
					"					SELECT 	x.sObjectGUID " + Environment.NewLine +
					"					FROM	[dbo].[tSecGroupToObject] AS [x] " + Environment.NewLine +
					"					WHERE	x.sGroupGUID = @BDAGroupGUID " + Environment.NewLine +
					"					) " + Environment.NewLine +

					"		IF @@ERROR <> 0 " + Environment.NewLine +
					"			BEGIN " + Environment.NewLine +
					"				GOTO ERROR_HANDLER " + Environment.NewLine +
					"			END " + Environment.NewLine +

					"		COMMIT TRANSACTION " + Environment.NewLine +
					"	END " + Environment.NewLine +

					"ERROR_HANDLER: " + Environment.NewLine +
					"	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION " + Environment.NewLine + 
					"";
				#endregion SQL

				cmd = new SqlCommand(sql);

				int i = DAL.SQLExecNonQuery(cmd);
				cmd.Dispose();
			}
			catch (SqlException sqle) { errors.Add(new ClassGenException(sqle)); }
			catch (Exception err) { errors.Add(new ClassGenException(err)); }

			return errors;		// Return the errors
		}
		#endregion ResetBDAPermissions

		#region Event For System Updates
		public delegate void SystemUpdateRunEventHandler(object sender, PopulateUpdateDelete_EventArgs e);
		public event SystemUpdateRunEventHandler SystemUpdateRun;
		protected void OnSystemUpdateRun(string name, string desc, int currentIndex, int totalCount)
		{
			if (SystemUpdateRun != null)
			{
				PopulateUpdateDelete_EventArgs e = new
					PopulateUpdateDelete_EventArgs(name, desc, string.Empty, totalCount, currentIndex);
				SystemUpdateRun(this, e);
			}
		}
		#endregion Event For System Updates
	}

	public enum SystemUpdateType : int
	{
		Empty = 0,

		StartUpdate,

		ResetBDAPermissions,

		EndUpdate,
	}
}
