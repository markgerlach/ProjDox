using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using mgModel;

namespace mgCustom
{
	public class Search
	{
		/// <summary>
		/// Fix the search filter so that it can be passed back into the databsae
		/// </summary>
		/// <param name="srchFilter">The search filter string to fix</param>
		/// <param name="searchObjectType">The search object type to work against for field naming</param>
		/// <param name="errors">An errors collection with any errors encountered</param>
		/// <returns>The finished and fixed string</returns>
		public static string FixSearchFilter(string srchFilter, Type searchObjectType, ref ClassGenExceptionCollection errors)
		{
			// Hunt through the freakin' string to see if we can replace 
			// any of the values on the way back to the db
			string searchPhrase = @"\(?\[(.+?)\]\)?";
			try
			{
				string type = searchObjectType.ToString().Replace("_BaseWinProject.", "");
				srchFilter = srchFilter.Replace("= False", "= 0").Replace("= True", "<> 0");
				srchFilter = srchFilter.Replace("= 'False'", "= 0").Replace("= 'True'", "<> 0");
				MatchCollection matches = Regex.Matches(srchFilter, searchPhrase);
				foreach (Match m in matches)
				{
					try
					{
						// Replace the string that you're passing back to the db
						string val = m.Groups[1].Value;

						switch (type)
						{
							case "ForgeMetric":
								//ForgeMetricField fldForgeMetric = (ForgeMetricField)Enum.Parse(typeof(ForgeMetricField), val);
								//srchFilter = srchFilter.Replace(m.Value, "[" + ForgeMetric.GetDBFieldName(fldForgeMetric) + "]");
								break;
							case "Search_RunChartMasterDetail":
								//Search_RunChartMasterDetailField fldSearch_RunChartMasterDetail = (Search_RunChartMasterDetailField)Enum.Parse(typeof(Search_RunChartMasterDetailField), val);
								//srchFilter = srchFilter.Replace(m.Value, "[" + Search_RunChartMasterDetail.GetDBFieldName(fldSearch_RunChartMasterDetail) + "]");
								break;
							case "Search_RunChartFull":
								//Search_RunChartFullField fldSearch_RunChartFull = (Search_RunChartFullField)Enum.Parse(typeof(Search_RunChartFullField), val);
								//srchFilter = srchFilter.Replace(m.Value, "[" + Search_RunChartFull.GetDBFieldName(fldSearch_RunChartFull) + "]");
								break;
							case "Search_TrainingInstructor":
								//mwsModel.Search_TrainingInstructorField fldTrainingInstructor =
								//    (mwsModel.Search_TrainingInstructorField)Enum.Parse(typeof(mwsModel.Search_TrainingInstructorField), val);
								//srchFilter = srchFilter.Replace(m.Value, "[" +
								//    mwsModel.Search_TrainingInstructor.GetDBFieldName(fldTrainingInstructor) +
								//    "]");
								break;
						}

					}
					catch (Exception ex)
					{
						errors.Add(new ClassGenException(ex));
					}
				}

				// Next, go in and try to find the dates (surrounded by # signs - 
				// freakin' MS Access programmers)
				searchPhrase = @"\#(\d{4}-\d{2}\-\d{2})\#";
				srchFilter = Regex.Replace(srchFilter, searchPhrase, "'$1'");
			}
			catch (Exception ex)
			{
				errors.Add(new ClassGenException(ex));
			}

			return srchFilter;
		}
	}
}
