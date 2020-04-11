using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace mgCustom
{
    public class JsonWrapper
    {
        /// <summary>
        /// Attempt to serialize the object to the server
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>A string containing the return from the method</returns>
        //public static string SerializeObject(Contact obj)
        //{
        //    string rtv = string.Empty;

        //    try
        //    {
        //        rtv = JsonConvert.SerializeObject(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        rtv = "Error: " + ex.Message;
        //    }

        //    return rtv;         // Return the result
        //}

        //public static Contact DeserializeObject(string guid)
        //{
            
        //}
    }
}
