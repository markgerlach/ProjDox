using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mgCustom
{
	public class Logging
	{
		//public static ClassGenExceptionCollection WriteLog(ActionType actionType,
		//    ClassGenExceptionIconType type,
		//    string objectName,
		//    string description,
		//    string userName,
		//    //string machineName,
		//    string objectXML,
		//    string appVersion)
		//{
		//    ClassGenExceptionCollection errors = new ClassGenExceptionCollection();

		//    try
		//    {
		//        // Write the entry into the log
		//        LogTrans newItem = new LogTrans();

		//        newItem.LogGUID = System.Guid.NewGuid().ToString();
		//        newItem.Action = actionType.ToString();
		//        newItem.ActionType = type.ToString();
		//        if (!String.IsNullOrEmpty(objectName.Trim())) { newItem.ObjectName = objectName; }
		//        newItem.DateLogged = DateTime.Now;

		//        //newItem.MachineName = machineName;
		//        newItem.MachineName = System.Environment.MachineName;
		//        newItem.AppVersion = appVersion;

		//        newItem.Desc = description;
		//        newItem.UserName = userName;
		//        if (!String.IsNullOrEmpty(objectXML.Trim())) { newItem.ObjectXML = objectXML; }

		//        errors.AddRange(newItem.AddUpdate());		// Send the element to the database
		//    }
		//    catch (Exception ex) { errors.Add(new ClassGenException(ex)); }

		//    return errors;			// Return the errors collection
		//}
	}
}
