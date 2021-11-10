using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jiraworklog
{
    public struct Credentials
    {
        public string userid;
        public string passwd;
    }
    class common
    {
        public static string GetEncodedCredentials(Credentials crd)
        {
            string mergedCredentials = string.Format("{0}:{1}", crd.userid, crd.passwd);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        public static object MakeRequestWithUrl(string requestUrl, Credentials crd, bool bExpanded)
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
                    if(!bExpanded)
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());

                        return objResponse;
                    }
                    else
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ExpandedIssue));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                        /*Response jsonResponse
                        = objResponse as Response;*/
                        return objResponse;
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return null;
            }
        }

        public static IssueData MakeRequestJiraID(string requestUrl , Credentials crd)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Method = "GET";

                string base64Credentials = common.GetEncodedCredentials(crd);
                request.Headers.Add("Authorization", "Basic " + base64Credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(IssueData));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    IssueData jsonResponse
                    = objResponse as IssueData;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
