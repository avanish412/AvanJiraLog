using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static JiralogMVC.Controller.CommonData;

namespace JiralogMVC.Controller
{
        
    public partial class JiralogController
    {
        private List<Jiraresponse> _responseList = null;
        private List<JiraID> _jiraList = null;
        private static int _numJiraIds = 0;
        private static int _jiraCounter = 0;

        /*----------------------------------------------------------------------------------------*/
        public void ThreadedJiralistRequest(JQLQuery ql)
        {
            BackgroundWorker _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _bw.DoWork += bw_jlDoWork;
            _bw.ProgressChanged += bw_jlProgressChanged;
            _bw.RunWorkerCompleted += bw_jlRunWorkerCompleted;
            _bw.RunWorkerAsync(ql);
        }

        private void bw_jlDoWork(object sender, DoWorkEventArgs e)
        {
            List<JiraID> jiraList = new List<JiraID>();
            JQLQuery QL = (JQLQuery)e.Argument;
            QL.jqlQuery = CreateJQLQueryForJiraIdsToRequest();
            CommonData.Credentials crd;
            crd.userid = _view.UserId;
            crd.passwd = _view.Passwd;

            IssueData issuedataresponse = CommonFunc.MakeRequestJiraID(crd, _baseUrl+"search", QL);

            int issuesNum = 0;

            if (issuedataresponse != null)
                issuesNum = issuedataresponse.issues.Length;

            for (int i = 0; i < issuesNum; i++)
            {
                try
                {
                    if (issuedataresponse.issues[i].fields == null)
                        continue;
                    DateTime dt = Convert.ToDateTime(issuedataresponse.issues[i].fields.updated);

                    if (!_view.AssignedOnly && (_view.StartDate - dt).TotalDays > 1.0)
                        continue;

                    JiraID jiraid = new JiraID();

                    if (issuedataresponse.issues[i].fields.project != null)
                        jiraid.strProject = issuedataresponse.issues[i].fields.project.key;

                    if (String.Compare(_selectedProjectId, jiraid.strProject, true) != 0)
                        continue;

                    jiraid.createTime = jiraid.strProject = issuedataresponse.issues[i].fields.created;
                    jiraid.createdBy = issuedataresponse.issues[i].fields.reporter.displayName;

                    if (issuedataresponse.issues[i].fields.issuetype != null)
                        jiraid.strIssuetype = issuedataresponse.issues[i].fields.issuetype.name;
                    if (issuedataresponse.issues[i].fields.priority != null)
                        jiraid.strPriority = issuedataresponse.issues[i].fields.priority.name;

                    if (issuedataresponse.issues[i].fields.fixfor != null)
                        jiraid.fixfor = issuedataresponse.issues[i].fields.fixfor.name;

                    if (issuedataresponse.issues[i].fields.sprints != null)
                    {
                        string strSprints = "";
                        int ncount = 0;
                        foreach (string strDesc in issuedataresponse.issues[i].fields.sprints)
                        {
                            ncount++;
                            string[] strarray = strDesc.Split(',');

                            if (strarray != null && strarray.Length >= 5)
                            {
                                string sprintname = strarray[2].Substring(5);
                                DateTime startdate = Convert.ToDateTime(strarray[3].Substring(10));
                                DateTime enddate = Convert.ToDateTime(strarray[4].Substring(8));
                                string strStartDate = startdate.ToString("MM-dd-yyyy");
                                string strEndDate = enddate.ToString("MM-dd-yyyy");
                                strSprints += sprintname + " [" + strStartDate + " to " + strEndDate + "]";
                                strSprints += ((ncount == issuedataresponse.issues[i].fields.sprints.Length)?"":";");//remove name=
                            }
                        }
                        jiraid.sprint = strSprints;
                    }

                    jiraid.strJiraID = issuedataresponse.issues[i].key;

                    if (issuedataresponse.issues[i].fields.status != null)
                        jiraid.strStatus = issuedataresponse.issues[i].fields.status.name;

                    if (issuedataresponse.issues[i].fields.assignee != null)
                    {
                        jiraid.strAssignee = issuedataresponse.issues[i].fields.assignee.displayName;
                        jiraid.strAssigneeEmailid = issuedataresponse.issues[i].fields.assignee.emailAddress;
                    }

                    jiraid.strSummary = issuedataresponse.issues[i].fields.summary;
                    jiraid.aggtime = issuedataresponse.issues[i].fields.timespent != null ? issuedataresponse.issues[i].fields.timespent : 0; //in secs
                    jiraid.duedate = issuedataresponse.issues[i].fields.duedate;
                    jiraid.originalestimate = issuedataresponse.issues[i].fields.aggregatetimeestimate != null ? issuedataresponse.issues[i].fields.aggregatetimeestimate : 0;
                    jiraList.Add(jiraid);
                }
                catch (Exception ex)
                {
                    _view.ErrorMessage = "ERROR occurred while gettind Jira Ids to look into!!!";
                }
            }
            e.Result = jiraList as object;
        }

        private void bw_jlProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void bw_jlRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_jiraList)
            {
                _numJirarequest++;
                if (e.Result != null)
                {
                    _jiraList.AddRange((List<JiraID>)e.Result);
                }
                //if (_numJirarequest == TOTAL_JIRAIDS_TO_REQUEST / NUM_JIRAIDS_PER_REQUEST)
                //if(_jiraList.Count > 0)
                    GetDataforAllJiraIds(_jiraList);
            }
        }
        /*----------------------------------------------------------------------------------------*/
        void ThreadedDataRequest(JiraID jiraid)
        {
            BackgroundWorker _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw.RunWorkerAsync(jiraid);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            JiraID jiraid = (JiraID)e.Argument;
            try
            {
                Jiraresponse jiraresponse = new Jiraresponse();
                //if (!rdoAssignedSTC.Checked)
                {
                    CommonData.ResponseType restype = CommonData.ResponseType.RESPONSE_INVALID_TYPE;

                    switch(_view.ReportType)
                    {                        
                        case CommonData.ReportType.REPORT_TEAM_PERFORMANCE:
                            restype = CommonData.ResponseType.RESPONSE_ISSUE;
                            break;
                        case CommonData.ReportType.REPORT_JIRA_COMMENTS:
                            restype = CommonData.ResponseType.REPONSE_COMMENTS;
                            break;
                        case CommonData.ReportType.REPORT_TEAM_ASSIGNED:
                        case CommonData.ReportType.REPORT_DEFECT_LEAKAGE:
                            restype = CommonData.ResponseType.RESPONSE_EXPANDED_ISSUE;
                            break;
                    }
                    string request = this.CreateRequestUrl(jiraid.strJiraID);
                    bool bExpanded = !(_view.ReportType == CommonData.ReportType.REPORT_TEAM_PERFORMANCE);
                    CommonData.Credentials crd;
                    crd.userid = _model.UserId;
                    crd.passwd = _model.Passwd;
                    jiraresponse.response = CommonFunc.MakeRequestWithUrl(request, crd, restype);
                    jiraresponse.jiraid = jiraid;
                    e.Result = jiraresponse as object;
                }
            }
            catch (Exception ex)
            {
                _view.Message = String.Format("ERROR occurred while getting data for JIRA ID : {0}", jiraid.strJiraID);
            }
            lock ("counter")
            {
                _jiraCounter++;
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_responseList)
            {
                if (e.Result != null)
                {
                    _responseList.Add((Jiraresponse)e.Result);
                }
                _view.Progress = _responseList.Count;
                _view.Message = String.Format("{0} / {1}", _responseList.Count, _numJiraIds);

                if (_responseList.Count == _numJiraIds)
                {
                    ProcessResponseList(_responseList);
                }
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Console.WriteLine("Reached " + e.ProgressPercentage + "%");
        }
        //MULTIHREADING :: End
    }
}
