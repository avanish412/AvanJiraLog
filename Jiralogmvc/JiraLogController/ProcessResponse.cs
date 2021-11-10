using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiralogMVC.Controller
{
    public struct BugTrackData
    {
        public string nDaysinBacklog;
        public string nDaysinAssignment;
        public string nDaysinProgress;
        public string nDaysinReview;
        public string nDaysinTesting;
        public string CreatedBy;
        public string ImplementedBy;
        public string TestedBy;
    };
    public struct DefectLeakageData
    {
        public int nResolvedCnt;
        public int nReopenedCnt;
        public int nNoFTRCnt;
        public bool bFTR;
        public bool bReviewtoResolved;
        public string strResolvedby;
        public string strResolvedDate;
        public string strReopenedBy;
        public string strReopenedDate;
        public string strReviewedBy;    
        public string strReviewDate;
    };

    public struct PlannedCompletedData
    {
        public bool bPlanned;
        public bool bResolved;
        public bool binReview;
        public bool bBlocked;
        public string strAssignedDate;
        public string strResolvedDate;
    };
    public struct WorklogData
    {
        public string strDisplayname;
        public string strJiraId;
        public string strTimespent;
    }
    public struct PerfNodeData
    {
        public string strJiraid;
        public int spentHrs;
    }
    public struct PerfMemberData
    {
        public string strName;
        public List<PerfNodeData> nodeDataList;
    }

    public struct MembersWorkLog
    {
        public string name;
        public DateTime date;
        public int hrslogged;
    }

    public partial class JiralogController
    {
        private const string cstrDelimiter = " / ";
        private List<WorklogData> _workloglist = new List<WorklogData>();
        private List<MembersWorkLog> _membersworklogs = new List<MembersWorkLog>();

        private void ProcessResponseList(List<Jiraresponse> responseList)
        {
            _view.ProgressVisibility = false;
            foreach (Jiraresponse jiraresponse in responseList)
            {
                ProcessResponse(jiraresponse.response, jiraresponse.jiraid);
            }
            if (_view.ReportType == CommonData.ReportType.REPORT_TEAM_ASSIGNED)
            {
                List<string> missings = GetMissingMembers(_membersAssignedWork);
                foreach (string miss in missings)
                {
                    _view.Add_To_Grid_No_Work_Assigned(miss);
                }
            }
            else if(_view.ReportType == CommonData.ReportType.REPORT_TEAM_PERFORMANCE)
            {
                List<Member> members = _model.Members;
                List<string> teammembers = new List<string>();
                List<PerfMemberData> perfMemDataList = new List<PerfMemberData>();
                //Get the list of team members
                foreach(Member mbr in members)
                {
                    if(String.Compare(_view.TeamName,mbr.team)==0)
                    {
                        teammembers.Add(mbr.name);
                    }
                }
                int max_jiras = 0;
                //Multiloop 
                foreach (string strMem in teammembers)
                {
                    PerfMemberData pmData;
                    pmData.nodeDataList = new List<PerfNodeData>();
                    pmData.strName = strMem;

                    foreach (WorklogData wld in _workloglist)
                    {
                        if(String.Compare(strMem,wld.strDisplayname)==0)
                        {                            
                            PerfNodeData pnData;
                            pnData.strJiraid = wld.strJiraId;
                            pnData.spentHrs = 0;
                            
                            bool bJiraIdExists = false;
                            for (int i = 0; i < pmData.nodeDataList.Count;i++ )
                            {
                                if (wld.strJiraId == pmData.nodeDataList[i].strJiraid)
                                {
                                    PerfNodeData pd;
                                    pd.strJiraid = pmData.nodeDataList[i].strJiraid;
                                    pd.spentHrs = pmData.nodeDataList[i].spentHrs;
                                    pmData.nodeDataList.RemoveAt(i);
                                    pd.spentHrs += CommonFunc.ConvertIntoHrs(wld.strTimespent);
                                    pmData.nodeDataList.Insert(i, pd);
                                    bJiraIdExists = true;
                                }
                                
                            }
                            if (!bJiraIdExists)
                            {
                                pnData.spentHrs = CommonFunc.ConvertIntoHrs(wld.strTimespent);
                                pmData.strName = strMem;
                                pmData.nodeDataList.Add(pnData);
                            }                            
                        }                        
                    }
                    max_jiras = pmData.nodeDataList.Count > max_jiras ? pmData.nodeDataList.Count : max_jiras;
                    perfMemDataList.Add(pmData);
                }
                _view.Add_To_Grid_Team_Performance(perfMemDataList, max_jiras);
            }
            _view.Message = "Report generated successfully !!!";
        }
        private bool IsFromSelectedTeam(string strName)
        {
            string strTmName = _model.GetTeamName(strName);

            if (String.Compare(_view.SelectedTeammember, "All Team Members", true) != 0 && String.Compare(_view.SelectedTeammember, strName, true) != 0)
                return false;

            if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTmName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                return false;

            return true;
        }
        private void ProcessResponse(object response, JiraID jiraid)
        {
            int logNum = 0;
            IssueResponse worklogsResponse = null;
            ExpandedIssueResponse changelogResponse = null;
            CommentResponse commentResponse = null;

            if (response != null)
            {
                switch (_view.ReportType)
                {
                    case CommonData.ReportType.REPORT_TEAM_ASSIGNED:
                        changelogResponse = response as ExpandedIssueResponse;
                        break;
                    case CommonData.ReportType.REPORT_TEAM_PERFORMANCE:
                        worklogsResponse = response as IssueResponse;
                        break;
                    case CommonData.ReportType.REPORT_JIRA_COMMENTS:
                        commentResponse = response as CommentResponse;
                        break;                    
                    case CommonData.ReportType.REPORT_DEFECT_LEAKAGE:
                        changelogResponse = response as ExpandedIssueResponse;
                        break;
                }
            }
            else
                return;

            if (worklogsResponse != null)
                logNum = worklogsResponse.workLogs.Length;
            else if (changelogResponse != null)
                logNum = changelogResponse.changelog.histories.Length;
            else if (commentResponse != null)
                logNum = commentResponse.comments.Length;
            else
                return;

            switch(_view.ReportType)
            {
                case CommonData.ReportType.REPORT_TEAM_PERFORMANCE:
                    Generate_Report_Date_Range(worklogsResponse, jiraid, logNum);
                    break;
                case CommonData.ReportType.REPORT_JIRA_COMMENTS:
                    Generate_Report_Comments(commentResponse, jiraid, logNum);
                    break;
                case CommonData.ReportType.REPORT_TEAM_ASSIGNED:
                    Generate_Report_Assigned(changelogResponse, jiraid, logNum);
                    break;
                case CommonData.ReportType.REPORT_DEFECT_LEAKAGE:
                    Generate_Report_Defect_Leakage(changelogResponse, jiraid, logNum);
                    break;
                default:
                    _view.ErrorMessage = "ERROR : In generating Report due to wrong selection of Report Type";
                    break;
            }
            _view.EnableControls(true);
        }
        void Generate_Report_Daily_Worklog(IssueResponse response, JiraID jiraid, int logNum)
        {
            for (int i = 0; i < logNum; i++)
            {
                DateTime time = Convert.ToDateTime(response.workLogs[i].started);

                if (time.Date.CompareTo(_view.StartDate.Date) != 0)
                    continue;

                if (String.Compare(_view.SelectedTeammember, "All Team Members", true) != 0 && String.Compare(_view.SelectedTeammember, response.workLogs[i].author.displayName, true) != 0)
                    continue;
                
                string strTeamName = _model.GetTeamName(response.workLogs[i].author.displayName);

                if (strTeamName == "")
                    continue;
                if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTeamName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                    continue;

                _membersLoggedWork.Add(response.workLogs[i].author.displayName);

                _view.Add_To_Grid_Daily_Worklog(response, jiraid, i);               
            }
        }
        void Generate_Report_Comments(CommentResponse response, JiraID jiraid, int logNum)
        {
            for (int i = 0; i < logNum; i++)
            {
                DateTime time = Convert.ToDateTime(response.comments[i].created);

                string strTeamName = _model.GetTeamName(jiraid.strAssignee);

                if (strTeamName == "")
                    continue;
                if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTeamName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                    continue;

                if (time.Date.CompareTo(_view.StartDate.Date) < 0)
                    continue;

                _view.Add_To_Grid_Jira_Comments(response, jiraid, i, time);
            }
        }
        void Generate_Report_Date_Range(IssueResponse response, JiraID jiraid, int logNum)
        {
            for (int i = 0; i < logNum; i++)
            {
                DateTime time = Convert.ToDateTime(response.workLogs[i].started);

                if (time.Date.CompareTo(_view.StartDate.Date) < 0 || time.Date.CompareTo(_view.EndDate.Date) > 0)
                    continue;

                if (String.Compare(_view.SelectedTeammember, "All Team Members", true) != 0 && String.Compare(_view.SelectedTeammember, response.workLogs[i].author.displayName, true) != 0)
                    continue;
                
                string strTeamName = _model.GetTeamName(response.workLogs[i].author.emailAddress);
                if (strTeamName == "")
                    continue;
                if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTeamName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                    continue;
               if (_view.ReportType == CommonData.ReportType.REPORT_TEAM_PERFORMANCE)
                {
                    WorklogData wld;

                    wld.strDisplayname = response.workLogs[i].author.displayName;
                    wld.strJiraId = jiraid.strJiraID;
                    wld.strTimespent = response.workLogs[i].timeSpent;

                    _workloglist.Add(wld);
                }
            }
            
        }
        void Generate_Report_Assigned(ExpandedIssueResponse response, JiraID jiraid, int logNum)
        {
  
            if (String.Compare(_view.SelectedTeammember, "All Team Members", true) != 0 && String.Compare(_view.SelectedTeammember, jiraid.strAssignee, true) != 0)
                return;

            string strTeamName = _model.GetTeamName(jiraid.strAssignee);

            if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTeamName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                return;

            if (jiraid.strStatus.CompareTo("Blocked")!=0 && jiraid.strStatus.CompareTo("Done")!=0)
                _membersAssignedWork.Add(jiraid.strAssignee);


            BugTrackData Btd = new BugTrackData();
            Btd.nDaysinBacklog = Btd.nDaysinAssignment = Btd.nDaysinProgress = Btd.nDaysinReview = Btd.nDaysinTesting = "";

            string field= "", toString ="",fromString="" ,creationtime="" , authorname = "";
            bool bAssignedforfirsttime = false;
            bool statuschange = false;

            TimeSpan ts = TimeSpan.FromDays(0);
            DateTime dtAssignedDate, dtInProgressDate, dtReviewDate, dtTestingDate,dtDoneDate, dtBlockedDate;
            dtAssignedDate = dtInProgressDate = dtReviewDate = dtTestingDate = dtDoneDate = dtBlockedDate = DateTime.Now;

            Btd.CreatedBy = jiraid.createdBy;

            for (int i = 0; i < logNum; i++)
            {
                try
                {
                    for (int j = 0; j < response.changelog.histories[i].items.Length; j++)
                    {
                        try
                        {
                            DateTime time = Convert.ToDateTime(response.changelog.histories[i].created);
                            field = response.changelog.histories[i].items[j].field;

                            toString = response.changelog.histories[i].items[j].toString;
                            fromString = response.changelog.histories[i].items[j].fromString;
                            creationtime = response.changelog.histories[i].created;
                            authorname = response.changelog.histories[i].author.displayName;

                            if (field == "assignee")
                            {
                                if ((fromString == null || fromString == "Avanish Kumar") && toString != "Avanish Kumar" && !bAssignedforfirsttime)
                                {
                                    dtAssignedDate = Convert.ToDateTime(creationtime);
                                    ts = Convert.ToDateTime(creationtime).Subtract(Convert.ToDateTime(jiraid.createTime));
                                    Btd.nDaysinBacklog += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days+1) +" & ";

                                    bAssignedforfirsttime = true;
                                }
                                else if (fromString != null && fromString != "Avanish Kumar" && !bAssignedforfirsttime)
                                {
                                    dtAssignedDate = Convert.ToDateTime(jiraid.createTime);
                                    Btd.nDaysinBacklog = "0 & ";
                                    bAssignedforfirsttime = true;
                                }
                            }
                            else if(field == "status")
                            {             
                               
                                DateTime dt = Convert.ToDateTime(creationtime); ;
                                /************** CALCULATIONS ************************************/
                                //Days in Assignment    :   Assigned --> In Progress
                                if (toString == "In Progress")
                                    dtInProgressDate = dt;
                                else if (toString == "Review")
                                    dtReviewDate = dt;
                                else if (toString == "Testing")
                                {
                                    dtTestingDate = dt;
                                    Btd.ImplementedBy = authorname;
                                }
                                else if (toString == "Blocked")
                                    dtBlockedDate = dt;
                                else if (toString == "Done")
                                {
                                    dtDoneDate = dt;
                                    Btd.TestedBy = authorname;
                                }


                                if (fromString == "New")
                                {
                                    if (!bAssignedforfirsttime)
                                        dtAssignedDate = Convert.ToDateTime(jiraid.createTime);

                                    ts = dt.Subtract(dtAssignedDate);
                                    Btd.nDaysinAssignment += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1) + " & ";                                    
                                }
                                //Days in Progress      :   In Progress --> Reviews
                                else if (fromString == "In Progress")
                                {
                                    ts = dt.Subtract(dtInProgressDate);
                                    Btd.nDaysinProgress += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1) + " & ";
                                }
                                //Days in Reviews       :   Reviews --> Testing
                                else if (fromString == "Review")
                                {
                                    ts = dt.Subtract(dtReviewDate);
                                    Btd.nDaysinReview += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1) + " & ";
                                }
                                //Days in Testing       :   Testing --> Done
                                else if (fromString == "Testing")
                                {
                                    ts = dt.Subtract(dtTestingDate);
                                    Btd.nDaysinTesting += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1) + " & ";
                                }
                                statuschange = true;
                            }                            
                        }
                        catch (Exception ex)
                        {
                            _view.ErrorMessage = "ERROR occurred while generating report !";
                        }
                    }

                }
                catch (Exception ex)
                {
                    _view.ErrorMessage = "ERROR occurred while generating report !";
                }
            }
            
            //Assignment duration - Dev ( New Open ) = CurrentTime - Assignmentdate
            if (jiraid.strStatus == "New" || jiraid.strStatus == "Open" && jiraid.strAssignee != "")
            {
                ts = DateTime.Now.Subtract(dtAssignedDate);
                Btd.nDaysinAssignment += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1);
            }
            //In Progress duration = CurrentTime - in progress/review
            else if (jiraid.strStatus == "In Progress")
            {
                ts = DateTime.Now.Subtract(dtInProgressDate);
                Btd.nDaysinProgress += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1);
            }
            //In Progress duration = CurrentTime - in progress/review
            else if (jiraid.strStatus == "Review")
            {
                ts = DateTime.Now.Subtract(dtReviewDate);
                Btd.nDaysinReview += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1);
            }
            //Assignment duration - Testing  =   Currenttime - Testing
            else if (jiraid.strStatus == "Testing")
            {
                ts = DateTime.Now.Subtract(dtTestingDate);
                Btd.nDaysinTesting += Convert.ToString(ts.Hours < 14 ? ts.Days : ts.Days + 1);
            }

            //Trimming " & "
            char[] charstotrim = { ' ', '&' };

            Btd.nDaysinBacklog = Btd.nDaysinBacklog.TrimEnd(charstotrim);
            Btd.nDaysinAssignment = Btd.nDaysinAssignment.TrimEnd(charstotrim);
            Btd.nDaysinProgress = Btd.nDaysinProgress.TrimEnd(charstotrim);
            Btd.nDaysinReview = Btd.nDaysinReview.TrimEnd(charstotrim);
            Btd.nDaysinTesting = Btd.nDaysinTesting.TrimEnd(charstotrim);

            _view.Add_To_Grid_AssignedWork(response, jiraid, Btd);
        }
        void Generate_Report_Time_Spent(IssueResponse response, JiraID jiraid, int logNum)
        {
            if (String.Compare(_view.SelectedTeammember, "All Team Members", true) != 0 && String.Compare(_view.SelectedTeammember, jiraid.strAssignee, true) != 0)
                return;
            string strTeamName = _model.GetTeamName(jiraid.strAssignee);

            if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTeamName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                return;

            _view.Add_To_Grid_TimeSpent(response, jiraid);
        }
        void Generate_Report_Defect_Leakage(ExpandedIssueResponse response, JiraID jiraid, int logNum)
        {
            //int nResolvedCnt, nReopenedCnt, nNoFTRCnt;
            //string strResolvedDate, strReopenedDate, strReviewDate, strResolvedby;
            DefectLeakageData dld;
            dld.nResolvedCnt = dld.nReopenedCnt = dld.nNoFTRCnt = 0;
            dld.strResolvedby = dld.strResolvedDate = dld.strReopenedDate = dld.strReopenedBy = dld.strReviewedBy = dld.strReviewDate = "";
            dld.bReviewtoResolved = false;
            dld.bFTR = true;

            bool bInReview = false;
            //bool bReviewtoResolved = false;
            bool bOutofDuration = false;
            bool bOnceRejected = false;
            //bool bFTR = true;
            bool bReopened = false;

            for (int i = 0; i < logNum; i++)
            {
                try
                {
                    for (int j = 0; j < response.changelog.histories[i].items.Length; j++)
                    {
                        //ASSIGNED ? Criteria # 1 : Current Assignee from the team

                        //RESOLVED ? Criteria # 1 : Resolved by someone in the team in that period
                        //RESOLVED ? Criteria # 2 : In Review done by Team Member & In Review for remaining period of time ( T_END + BUFFERDAYES )
                        //RESOLVED ? Criteria # 3 : In Review to Resolved and In Review done by team member 

                        //REOPENED ? Criteria # 1 : "Reopened" any time in full history

                        //CODE DEFECT ? Criteria # 1 :  to "Ready to Dev" in full history
                        try
                        {
                            DateTime time = Convert.ToDateTime(response.changelog.histories[i].created);
                            string field = response.changelog.histories[i].items[j].field;
                            string toString = response.changelog.histories[i].items[j].toString;
                            string fromString = response.changelog.histories[i].items[j].fromString;
                            string creationtime = response.changelog.histories[i].created;
                            string authorname = response.changelog.histories[i].author.displayName;

                            if (field == "status")
                            {
                                //if ( bInReview && toString !=  && (time.Date - dtPickerEnd.Value.Date).Days < 0)
                                //bInReview = false;

                                //IS THE ISSUE RESOLVED ?
                                if (toString == "Testing")
                                {
                                    dld.bReviewtoResolved = bOutofDuration = false;
                                    if (time.Date.CompareTo(_view.StartDate.Date) < 0 || time.Date.CompareTo(_view.EndDate.Date) > 0)
                                    {
                                        bOutofDuration = true;
                                    }

                                    //RESOLVED ? Criteria # 1 : Resolved by someone in the team in that period
                                    if (!bOutofDuration && IsFromSelectedTeam(authorname))
                                        dld.nResolvedCnt++;
                                    else if (bInReview && fromString == "Review")
                                    {
                                        if (!bOnceRejected)
                                            dld.nNoFTRCnt--;
                                        dld.nResolvedCnt++;
                                        dld.bReviewtoResolved = true;
                                    }
                                    else
                                        continue;

                                    if (dld.nResolvedCnt > 1)
                                    {
                                        dld.strResolvedDate += cstrDelimiter + CommonFunc.ConvertToDate(creationtime);
                                        dld.strResolvedby += cstrDelimiter + authorname;
                                    }
                                    else
                                    {
                                        dld.strResolvedDate += CommonFunc.ConvertToDate(creationtime);
                                        dld.strResolvedby += authorname;
                                    }
                                }
                                else if (toString == "Review")
                                {
                                    if (IsFromSelectedTeam(authorname) && time.Date.CompareTo(_view.StartDate.Date) >= 0 && time.Date.CompareTo(_view.EndDate.Date) <= 0)
                                        bInReview = true;
                                }
                                //REOPENED ? Criteria # 1 : "Reopened" any time in full history
                                else if (toString == "Reopened" || toString == "Open" || toString == "New" || toString == "In Progress")
                                {
                                    dld.bFTR = false;
                                    bReopened = true;
                                    dld.nReopenedCnt++;
                                    if (dld.nReopenedCnt > 1)
                                    {
                                        dld.strReopenedDate += cstrDelimiter + CommonFunc.ConvertToDate(creationtime);
                                        dld.strReopenedBy += cstrDelimiter + authorname;
                                    }
                                    else
                                    {
                                        dld.strReopenedDate += CommonFunc.ConvertToDate(creationtime);
                                        dld.strReopenedBy += authorname;
                                    }
                                }
                                //CODE DEFECT ? Criteria # 1 :  to "Ready to Dev" in full history
                                else if (fromString == "Review" && (toString == "New" || toString == "Open" || toString == "Reopened" || toString == "In Progress"))
                                {
                                    bInReview = false;
                                    bOnceRejected = true;
                                    dld.nNoFTRCnt++;
                                    if (dld.nNoFTRCnt > 1)
                                    {
                                        dld.strReviewDate += cstrDelimiter + CommonFunc.ConvertToDate(creationtime);
                                        dld.strReviewedBy += cstrDelimiter + authorname;
                                    }
                                    else
                                    {
                                        dld.strReviewDate += CommonFunc.ConvertToDate(creationtime);
                                        dld.strReviewedBy += authorname;
                                    }
                                }
                                else if (fromString == "Testing" && (toString == "Done"))
                                {
                                    if (!bReopened)
                                        dld.bFTR = true;
                                    dld.nNoFTRCnt--;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _view.ErrorMessage = "ERROR occurred while generating report !";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _view.ErrorMessage = "ERROR occurred while generating report !";
                }
            }
            if (dld.nResolvedCnt > 0 || bInReview)
            {
                _view.Add_To_Grid_Defect_Leakage(response, jiraid, dld);
            }
        }
        void Generate_Report_Planned_Completed(ExpandedIssueResponse response, JiraID jiraid, int logNum)
        {
            PlannedCompletedData pcd;
            pcd.bPlanned = false;        
            pcd.bResolved = false;            
            pcd.bBlocked = false;            
            pcd.binReview = false;
            pcd.strResolvedDate = "";    
            pcd.strAssignedDate = "";

            DateTime firstResolvedDate = _view.EndDate;
            bool bAssignedTaskSymphony = false;
            

            //TO DO: Don't depend on current assignee only
            /*if (String.Compare(cmbTeamMember.Text, "All Team Members", true) != 0 && String.Compare(cmbTeamMember.Text, jiraid.strAssignee, true) != 0)
                return;
            string strTeamName = GetTeamName(jiraid.strAssignee);
            string strTeamToCompare = rdoJukebox.Checked ? "Jukebox" : "Server";
            if (String.Compare(strTeamName, strTeamToCompare, true) != 0)
                return;*/

            //Reading the history log
            for (int i = 0; i < logNum; i++)
            {
                if (bAssignedTaskSymphony && pcd.bResolved && pcd.bBlocked)
                    break;

                for (int j = 0; j < response.changelog.histories[i].items.Length; j++)
                {
                    DateTime time = Convert.ToDateTime(response.changelog.histories[i].created);
                    string field = response.changelog.histories[i].items[j].field;
                    string toString = response.changelog.histories[i].items[j].toString;
                    string fromString = response.changelog.histories[i].items[j].fromString;
                    string creationtime = response.changelog.histories[i].created;
                    string authorname = response.changelog.histories[i].author.displayName;

                    //PLANNED ? Criteria # 1 Assigned to some one in the team in this duration
                    //PLANNED ? Criteria # 2 Already assigned to someone in team AND changed to "Ready for Dev" 
                    //PLANNED ? Criteria # 3 Already assigned to someone in team AND Not remained "Blocked" for the whole duration

                    //COMPLETED ? Criteria # 1 status changed to "Resolved || Closed" AND didn't "Reopen"
                    //COMPLETED ? Criteria # 2 status changed to  and didn't change to "Ready for Dev" for the remining duration
                    // to "Ready to Dev" is considered as INCOMPLETE

                    if (field == "assignee")
                    {
                        string strTmName = _model.GetTeamName(toString);
                        if (strTmName == "")
                            continue;
                        //Is assigned to some one in selected team
                        if (_view.Team != CommonData.Team.TEAM_ALL && String.Compare(strTmName, JiralogModel.GetTeamNameFromType(_view.Team), true) != 0)
                            continue;

                        //PLANNED ? Criteria # 1 Assigned to some one in the team in this duration
                        if (IsFromSelectedTeam(toString))
                            bAssignedTaskSymphony = true;
                        else
                            bAssignedTaskSymphony = false;

                        if (bAssignedTaskSymphony && time.Date.CompareTo(_view.StartDate.Date) >= 0 && time.Date.CompareTo(_view.EndDate.Date) <= 0)
                        {
                            pcd.strAssignedDate = CommonFunc.ConvertToDate(creationtime);
                            pcd.bPlanned = true;
                        }


                    }
                    if (field == "status")
                    {
                        if (time.Date.CompareTo(_view.StartDate.Date) < 0 || time.Date.CompareTo(_view.EndDate.Date) > 0)
                            continue;

                        //PLANNED ? Criteria # 2 Already assigned to someone in team AND changed to "Ready for Dev"  
                        if (bAssignedTaskSymphony)
                        {
                            if (toString == "New" || toString == "Open")
                            {
                                pcd.strAssignedDate = CommonFunc.ConvertToDate(creationtime);
                                pcd.bPlanned = true;
                            }
                        }
                        //COMPLETED ? Criteria # 1 status changed to "Resolved || Closed" AND didn't "Reopen"
                        if (bAssignedTaskSymphony && (toString == "Resolved" || toString == "Closed"))
                        {
                            pcd.bResolved = true;
                            pcd.strResolvedDate = CommonFunc.ConvertToDate(creationtime);
                        }
                        //COMPLETED ? Criteria # 2 status changed to  and didn't change to "Ready for Dev" for the remining duration
                        if (toString == "Review")
                            pcd.binReview = true;
                        else if (fromString == "New" || fromString=="Open")
                            pcd.binReview = false;
                        //PLANNED ? Criteria # 3 Already assigned to someone in team AND Not remained "Blocked" for the whole duration
                        if (toString == "Blocked")
                            pcd.bBlocked = true;
                        else if (fromString == "Blocked")
                            pcd.bBlocked = false;
                    }
                }
            }
            if (bAssignedTaskSymphony)
            {
                _view.Add_To_Grid_Planned_Completed(response, jiraid,pcd);
            }
        }
                   
    }
}
