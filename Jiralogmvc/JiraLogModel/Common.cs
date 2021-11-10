using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using static JiralogMVC.Controller.CommonData;

namespace JiralogMVC.Controller
{
    public class CommonData
    {
        public struct Credentials
        {
            public string userid;
            public string passwd;
        }

        public class JQLQuery
        {
            public string maxResults;
            public string startAt;
            public string jqlQuery;

            public JQLQuery(string maxRes, string startat, string jqlquery)
            {
                maxResults = maxRes;
                startAt = startat;
                jqlQuery = jqlquery;
            }
        }
        public enum Team
        {
            TEAM_INVALID = -1,
            TEAM_ALL = 0,
            TEAM_DOSEIQ_COMMON,
            TEAM_DOSEIQ_DEV,
            TEAM_DOSEIQ_QA_MANUAL,
            TEAM_DOSEIQ_QA_AUTOMATION,
        };

        public enum ReportType
        {
            REPORT_INVALID_TYPE = -1,
            /*REPORT_DAILY_WORKLOG = 0,
            REPORT_DATE_RANGE,
            REPORT_STC_ASSIGNED,
            REPORT_TIME_SPENT,
            REPORT_DEFECT_LEAKAGE,
            REPORT_PLANNED_COMPLETED,
            REPORT_TEAM_PERFORMANCE,
            REPORT_JIRA_COMMENTS
            REPORT_DATE_RANGE*/
            REPORT_TEAM_ASSIGNED,
            REPORT_DEFECT_LEAKAGE,
            REPORT_INDIVIDUAL_PERFORMANCE,
            REPORT_TEAM_PERFORMANCE,
            REPORT_JIRA_COMMENTS
        };
        public enum ResponseType
        {
            RESPONSE_INVALID_TYPE = -1,
            RESPONSE_ISSUE=0,
            RESPONSE_EXPANDED_ISSUE,
            REPONSE_COMMENTS
        };
    }    

    public class CommonFunc
    {
        public static string GetFormattedDate(DateTime time)
        {
            return String.Format("{0}-{1:00}-{2:00}", time.Date.Year, time.Date.Month >= 10 ? Convert.ToString(time.Date.Month) : "0" + Convert.ToString(time.Date.Month), time.Date.Day >= 10 ? Convert.ToString(time.Date.Day) : "0" + Convert.ToString(time.Date.Day));
        }
        public static string GetFormattedDateTime(DateTime time)
        {
            return String.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Date.Year, time.Date.Month >= 10 ? Convert.ToString(time.Date.Month) : "0" + Convert.ToString(time.Date.Month), time.Date.Day >= 10 ? Convert.ToString(time.Date.Day) : "0" + Convert.ToString(time.Date.Day),time.Hour,time.Minute,time.Second);
        }
        public static int ConvertIntoHrs(string timelogged)
        {
            try
            {
                //Remove all spaces first
                timelogged.Replace(" ", "");

                if(timelogged.IndexOf('h') > 0 && timelogged.IndexOf('d') > 0)
                {
                    //Complex case
                    int id = timelogged.IndexOf('d');
                    int ih = timelogged.IndexOf('h');
                    int days = Convert.ToInt32(timelogged.Substring(0, id));
                    int hours = Convert.ToInt32(timelogged.Substring(id + 1, ih - id - 1));
                    return (8 * days + hours);
                }
                else if (timelogged.IndexOf('h') > 0)
                {
                    return Convert.ToInt32(timelogged.Replace("h", String.Empty));
                }
                else if (timelogged.IndexOf('d') > 0)
                {
                    return 8 * Convert.ToInt32(timelogged.Replace("d", String.Empty));
                }
            }
            catch(Exception e)
            {
                //Do nothing 
            }
            //Todo
            return 0;
        }
        public static string Converttojiratimeformat(long nSec)
        {
            int nHours = (int)(nSec / 3600);
            if (nHours == 0)
                return "";

            if (nHours > 8)
            {
                if ((nHours % 8) != 0)
                    return String.Format("{0}d {1}h", (int)nHours / 8, (int)nHours % 8);
                else
                    return String.Format("{0}d", (int)nHours / 8);
            }
            else
            {
                return String.Format("{0}h", (int)nHours);
            }
        }
        public static string ConverttoDays(long nSec)
        {
            int nHours = (int)(nSec / 3600);
            if (nHours == 0)
                return "";

            return String.Format("{0:0.0} d", (double)nHours/8.0);

        }
        public static string ConvertToDate(string strTime)
        {
            DateTime time = Convert.ToDateTime(strTime);
            return String.Format("{0}-{1}-{2}", time.Date.Year, time.Date.Month >= 10 ? Convert.ToString(time.Date.Month) : "0" + Convert.ToString(time.Date.Month), time.Date.Day >= 10 ? Convert.ToString(time.Date.Day) : "0" + Convert.ToString(time.Date.Day));
        }
        public static string GetEncodedCredentials(CommonData.Credentials crd)
        {
            string mergedCredentials = string.Format("{0}:{1}", crd.userid, crd.passwd);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        public static object MakeRequestWithUrl(string requestUrl, CommonData.Credentials crd, CommonData.ResponseType restype)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Method = "GET";

                string base64Credentials = GetEncodedCredentials(crd);
                request.Headers.Add("Authorization", "Basic " + base64Credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));

                    //
                    if (restype == CommonData.ResponseType.RESPONSE_ISSUE)
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(IssueResponse));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());

                        return objResponse;
                    }
                    else if (restype == CommonData.ResponseType.RESPONSE_EXPANDED_ISSUE)
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ExpandedIssueResponse));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                        /*Response jsonResponse
                        = objResponse as Response;*/
                        return objResponse;
                    }
                    else if(restype == CommonData.ResponseType.REPONSE_COMMENTS)
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(CommentResponse));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());

                        return objResponse;
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return null;
            }
            return null;
        }

        public static IssueData MakeRequestJiraID(CommonData.Credentials crd, string jiraapi, JQLQuery QL)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(jiraapi) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Method = "POST";
                
                string base64Credentials = CommonFunc.GetEncodedCredentials(crd);
                request.Headers.Add("Authorization", "Basic " + base64Credentials);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"startAt\":\""+ QL.startAt + "\"," +
                                  "\"maxResults\":\""+ QL.maxResults+ "\"," +
                                "\"jql\":\"" + QL.jqlQuery + "\"}";

                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(IssueData));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    response.Close();
                    IssueData jsonResponse
                    = objResponse as IssueData;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return null;
            }
        }
        public static string GetFirstName(string strFullName)
        {
            if (strFullName != null)
            {
                string[] starray = strFullName.Split(' ');
                return starray[0];
            }
            else
                return null;
        }
    }
}
