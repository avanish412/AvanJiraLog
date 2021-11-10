using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.Mail;
//using Outlook = Microsoft.Office.Interop.Outlook;
/*
 *************STATUS*****************************
http://192.168.16.35/rest/api/2/project/MPM/statuses
Open
Done
Requirements Needed //10272
In Progress
Reopened
Resolved
Closed
Integrated
Proposed Answer
Review In Progress
Reviewed
Test In Progress
Tested
Pending
Parking
Failed
Blocked
In Review
Need Specs
Ready for Dev
Need Estimate
****************************************/
namespace Jiraworklog
{
    public partial class frmJiraworklog : Form
    {
        public  struct JiraID
        {
            public string strProject;
            public string strIssuetype;
            public string strPriority;
            public string strJiraID;
            public string strStatus;
            public string strAssignee;
            public string strSummary;
            public string duedate;
            public Nullable<long> aggtime;
            public Nullable<long> originalestimate;            
        };
        public struct Jirauser
        {
            public string name;
            public string emailid;
            public string team;
            public bool bWorklog;
        };
        public struct Jiraresponse
        {
            public JiraID jiraid;
            public object response;
        };
        private const string m_BaseUrl = "http://192.168.16.35/rest/api/2/";
        private const string m_issue = "/issue/";
        private const string m_worklog = "/worklog";
        private const string m_Username = "";
        private const string m_Password = "";
        private ListViewColumnSorter lvwColumnSorter;
        private static List<Jirauser> jiraUsers;
        private static List<string> projects;
        private const string cstrDelimiter = " / ";
        private static int nResolved = 0;
        private static int nReopened = 0;
        private static int nFirstTimeRight = 0;
        private static int nPlanned = 0;
        private static int nCompleted = 0;
        private static Timer timer = new Timer();
        private const int cnDaysBuffer = 3;
        private static int numJiraIds = 0;
        List<Jiraresponse> responseList = null;
        private static int jiraCounter = 0;
        private static Credentials credent;

        public frmJiraworklog()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.lvDailyReport.ListViewItemSorter = lvwColumnSorter;
            this.lvDaterangeReport.ListViewItemSorter = lvwColumnSorter;
            this.lvDefectleakage.ListViewItemSorter = lvwColumnSorter;
            this.lvAssigned.ListViewItemSorter = lvwColumnSorter;
            this.lvPlannedCompleted.ListViewItemSorter = lvwColumnSorter;

            //pbLogprogress.Style = ProgressBarStyle.Marquee;
            pbLogprogress.Visible = false;
            lblMessage.Visible = false;
            txtUserid.Text = m_Username;
            txtPasswd.Text = m_Password;
            
            responseList = new List<Jiraresponse>();

            GetCredentialnUsers();
            InitializeUI();
        }
        private void InitializeUI()
        {
            foreach(string proj in projects)
            {
                cmbProjects.Items.Add(proj);
            }
            cmbProjects.SelectedIndex = cmbTeamMember.SelectedIndex = 0;
            rdoJukebox.Checked = true;
            rdoDailyreport.Checked = true;
        }
        private void EnableControls( bool bEnable)
        {
            txtUserid.Enabled = txtUserid.Enabled = btnGenerateReport.Enabled = btnExport.Enabled = dtPicker.Enabled = bEnable;
        }
        
        public  string CreateRequestUrl(string queryStringJIRAID )
        {
            string UrlRequest = "";
            if (!rdoDefectleakage.Checked && !rdoPlannedCompleted.Checked)
                UrlRequest = m_BaseUrl + "issue/" + queryStringJIRAID + "/worklog";
            else
                UrlRequest = m_BaseUrl + "issue/" + queryStringJIRAID + "?expand=changelog";
            return (UrlRequest);
        }
        public string CreateRequestJiraIds(string startAt)
        {
            string UrlRequest = "";
            DateTime dt = dtPicker.Value;
            DateTime dtNow = DateTime.Now;
            string strAssigned = "";
            if(rdoQC.Checked)
                strAssigned = (chkAssignedOnly.Checked) ? " AND (assignee in membersof('symphony qc'))" : "";
            else
                strAssigned = (chkAssignedOnly.Checked) ? " AND (assignee in membersof('symphony dev') OR assignee in membersof('symphony'))" : "";
            
            string strDate = String.Format("{0}-{1}-{2}", dt.Year, dt.Month, dt.Day);
            //string strLastOneMonth = String.Format("{0}-{1}-{2}", dtNow.Month > 2 ? dtNow.Year : dtNow.Year - 1, dtNow.Month > 2 ? dtNow.Month - 2 :(dtNow.Month==2? 12:11), dtNow.Day > 28 ? 28 : dtNow.Day);
            string strLastOneMonth = String.Format("{0}-{1}-{2}", dtNow.Month > 1 ? dtNow.Year : dtNow.Year - 1, dtNow.Month > 1 ? dtNow.Month - 1 : 12, dtNow.Day > 28 ? 28 : dtNow.Day);

            if (rdoDailyreport.Checked)
                UrlRequest = m_BaseUrl + "search?jql=updated >=" + strDate + " AND (status=1 OR status=3 OR status=4 OR status=5 OR status=6 OR status=10017 OR status=10045 OR status=10047 OR status=10054)" + strAssigned + "&&startAt=" +
                  startAt + "&&maxResults=1000";
            else if (rdoDaterange.Checked)
                UrlRequest = m_BaseUrl + "search?jql=updated >=" + strDate + " AND (status=1 OR status=3 OR status=4 OR status=5 OR status=6 OR status=10017 OR status=10045 OR status=10047 OR status=10054)"+ strAssigned + "&&startAt=" +
                  startAt + "&&maxResults=1000";
            else if (rdoAssignedSTC.Checked)
                UrlRequest = m_BaseUrl + "search?jql=updated >=" + strLastOneMonth + " AND (status=1 OR status=3 OR status=4 OR status=10006 OR status=10017 OR status=10045 OR status=10047 OR status=10054)" +
                 strAssigned +"&&startAt=" + startAt + "&&maxResults=1000";
            else if(rdoDefectleakage.Checked || rdoPlannedCompleted.Checked)
            {                
                //UrlRequest = m_BaseUrl + "search?jql=updated >= " + strDate + " AND (assignee in membersof('symphony dev') OR assignee in membersof('symphony')) &&maxResults=10000";
                UrlRequest = m_BaseUrl + "search?jql=updated >= " + strDate + strAssigned+"&&startAt="+startAt+"&&maxResults=1000";
            }
            
            return (UrlRequest);
        }
        
        
        public void ProcessResponse(object response, JiraID jiraid)
        {
            int logNum = 0;
            Response worklogsResponse = null;
            ExpandedIssue changelogResponse = null;

            if (response != null)
            {
                if (!rdoDefectleakage.Checked && !rdoPlannedCompleted.Checked)
                    worklogsResponse = response as Response;
                else
                    changelogResponse = response as ExpandedIssue;
            }

            if (worklogsResponse != null)
                logNum = worklogsResponse.workLogs.Length;
            else if (changelogResponse != null)
                logNum = changelogResponse.changelog.histories.Length;
           
            try
            {
                if (rdoDailyreport.Checked)
                {
                    for (int i = 0; i < logNum; i++)
                    {
                        DateTime time = Convert.ToDateTime(worklogsResponse.workLogs[i].started);
                        
                        if (time.Date.CompareTo(dtPicker.Value.Date) != 0 )
                            continue;

                        if (String.Compare(cmbTeamMember.Text, "All Team Members", true) != 0 && String.Compare(cmbTeamMember.Text, worklogsResponse.workLogs[i].author.displayName, true) != 0)
                            continue;

                        string strTeamName = GetTeamName(worklogsResponse.workLogs[i].author.displayName);
                        //Ignorance Area/////////////////////////////////////////////////////
                        if (strTeamName == "")
                            continue;
                        if (String.Compare(strTeamName, GetSelectedTeamName(), true) != 0)
                            continue;
                        
                        //////////////////////////////////////////////////////////////////////

                        ListViewItem lvi = new ListViewItem(strTeamName);
                        lvi.SubItems.Add(worklogsResponse.workLogs[i].author.displayName);
                        lvi.SubItems.Add(jiraid.strJiraID);
                        lvi.SubItems.Add(jiraid.strStatus);
                        lvi.SubItems.Add(jiraid.duedate);
                        
                        if (jiraid.duedate != null)
                        {
                            DateTime dt = Convert.ToDateTime(jiraid.duedate);
                            if (dt.Date.CompareTo(dtPicker.Value.Date) < 0)
                                lvi.SubItems[4].BackColor = Color.Orange;
                        }

                        string origEstimate = Converttojiratimeformat((long)jiraid.originalestimate);                        

                        lvi.SubItems.Add(origEstimate.CompareTo("0h") != 0 ? origEstimate : ".-.");
                        lvi.SubItems.Add(Converttojiratimeformat((long)jiraid.aggtime));
                        if (jiraid.originalestimate != 0 && jiraid.originalestimate < jiraid.aggtime)
                            lvi.SubItems[6].BackColor = Color.OrangeRed;

                        lvi.UseItemStyleForSubItems = false;
                        lvi.SubItems.Add(worklogsResponse.workLogs[i].timeSpent);

                        string s = worklogsResponse.workLogs[i].comment;
                        s = s.Replace("\n", " ");
                        s = s.Replace("\r", " ");
                        s = s.Replace("\t", " ");
                        s = s.Replace(",", " ");

                        lvi.SubItems.Add(s);
                        lvDailyReport.Items.Add(lvi);
                    }
                }
                else if (rdoDaterange.Checked)
                {
                    for (int i = 0; i < logNum; i++)
                    {
                        DateTime time = Convert.ToDateTime(worklogsResponse.workLogs[i].started);

                        if (time.Date.CompareTo(dtPicker.Value.Date) < 0 || time.Date.CompareTo(dtPickerEnd.Value.Date) > 0)
                            continue;

                        if (String.Compare(cmbTeamMember.Text, "All Team Members", true) != 0 && String.Compare(cmbTeamMember.Text, worklogsResponse.workLogs[i].author.displayName, true) != 0)
                            continue;

                        string strDate = GetFormattedDate(time);
                       string strTeamName = GetTeamName(worklogsResponse.workLogs[i].author.emailAddress);
                       if (strTeamName == "")
                           continue;
                       if (String.Compare(strTeamName, GetSelectedTeamName(), true) != 0)
                           continue;

                        ListViewItem lvi = new ListViewItem(strTeamName);
                        lvi.SubItems.Add(strDate);
                        lvi.SubItems.Add(worklogsResponse.workLogs[i].author.displayName);
                        lvi.SubItems.Add(jiraid.strJiraID);
                        lvi.SubItems.Add(worklogsResponse.workLogs[i].timeSpent);

                        string s = worklogsResponse.workLogs[i].comment;
                        s = s.Replace("\n", " ");
                        s = s.Replace("\r", " ");
                        s = s.Replace("\t", " ");
                        s = s.Replace(",", " ");

                        lvi.SubItems.Add(s);
                        lvDaterangeReport.Items.Add(lvi);
                    }
                }
                else if (rdoAssignedSTC.Checked)
                {
                    
                    ListViewItem lvi = new ListViewItem(jiraid.strJiraID);
                    lvi.SubItems.Add(jiraid.strPriority);
                    //Color CHanges
                    if (jiraid.strPriority == "Blocker" || jiraid.strPriority == "Critical")
                    {
                        lvi.SubItems[1].BackColor = Color.Red;
                        lvi.SubItems[1].ForeColor = Color.White;
                    }
                    else if (jiraid.strPriority == "Major")
                    {
                        lvi.SubItems[1].BackColor = Color.Blue;
                        lvi.SubItems[1].ForeColor = Color.White;
                    }
                    lvi.SubItems.Add(jiraid.strAssignee);

                    if (String.Compare(cmbTeamMember.Text, "All Team Members", true) != 0 && String.Compare(cmbTeamMember.Text, jiraid.strAssignee, true) != 0)
                        return;

                    string strTeamName = GetTeamName(jiraid.strAssignee);

                    if (String.Compare(strTeamName, GetSelectedTeamName(), true) != 0)
                        return;

                    lvi.UseItemStyleForSubItems = false;
                    lvi.SubItems.Add(strTeamName);
                    lvi.SubItems.Add(jiraid.strStatus);
                    //lvi.SubItems[4].Font = true;
                    //Color CHanges
                    if (jiraid.strStatus == "Blocked" || jiraid.strStatus == "Need Specs" || jiraid.strStatus == "In Review")
                    {
                        lvi.SubItems[4].BackColor = Color.Red;
                        lvi.SubItems[4].ForeColor = Color.White;
                    }
                    else if (jiraid.strStatus == "In Progress")
                    {
                        lvi.SubItems[4].BackColor = Color.Blue;
                        lvi.SubItems[4].ForeColor = Color.White;
                    }
                    else if (jiraid.strStatus == "Reopened")
                    {
                        lvi.SubItems[4].BackColor = Color.Pink;
                        lvi.SubItems[4].ForeColor = Color.Red;
                    }

                    lvi.SubItems.Add(jiraid.duedate);
                    lvi.SubItems.Add(Converttojiratimeformat((long)(jiraid.originalestimate!=null?jiraid.originalestimate:0)));
                    lvi.SubItems.Add(Converttojiratimeformat((long)(jiraid.aggtime!=null?jiraid.aggtime:0)));
                    lvi.SubItems.Add(jiraid.strSummary);                    

                    lvAssigned.Items.Add(lvi);
                }
                else if (rdoDefectleakage.Checked)
                {
                    int nResolvedCnt, nReopenedCnt, nNoFTRCnt;
                    string strResolvedDate, strReopenedDate, strReviewDate, strResolvedby;
                    nResolvedCnt = nReopenedCnt = nNoFTRCnt = 0;
                    strResolvedby = strResolvedDate = strReopenedDate = strReviewDate = "";

                    bool bInReview = false;
                    bool bReviewtoResolved = false;
                    bool bOutofDuration = false;
                    bool bOnceRejected = false; 
                    bool bFTR = true;
                    bool bReopened = false;

                    

                    for (int i = 0; i < logNum; i++)
                    {
                        try
                        {
                            for (int j = 0; j < changelogResponse.changelog.histories[i].items.Length; j++)
                            {
                                //ASSIGNED ? Criteria # 1 : Current Assignee from the team

                                //RESOLVED ? Criteria # 1 : Resolved by someone in the team in that period
                                //RESOLVED ? Criteria # 2 : In Review done by Team Member & In Review for remaining period of time ( T_END + BUFFERDAYES )
                                //RESOLVED ? Criteria # 3 : In Review to Resolved and In Review done by team member 

                                //REOPENED ? Criteria # 1 : "Reopened" any time in full history

                                //CODE DEFECT ? Criteria # 1 : "In Review" to "Ready to Dev" in full history
                                try
                                {
                                    DateTime time = Convert.ToDateTime(changelogResponse.changelog.histories[i].created);
                                    string field = changelogResponse.changelog.histories[i].items[j].field;
                                    string toString = changelogResponse.changelog.histories[i].items[j].toString;
                                    string fromString = changelogResponse.changelog.histories[i].items[j].fromString;
                                    string creationtime = changelogResponse.changelog.histories[i].created;
                                    string authorname = changelogResponse.changelog.histories[i].author.displayName;

                                    if (field == "status")
                                    {
                                        //if ( bInReview && toString != "In Review" && (time.Date - dtPickerEnd.Value.Date).Days < 0)
                                        //bInReview = false;

                                        //IS THE ISSUE RESOLVED ?
                                        if (toString == "Resolved")
                                        {
                                            bReviewtoResolved = bOutofDuration = false;
                                            if (time.Date.CompareTo(dtPicker.Value.Date) < 0 || time.Date.CompareTo(dtPickerEnd.Value.Date) > 0)
                                            {
                                                bOutofDuration = true;
                                            }

                                            //RESOLVED ? Criteria # 1 : Resolved by someone in the team in that period
                                            if (!bOutofDuration && IsFromSelectedTeam(authorname))
                                                nResolvedCnt++;
                                            else if (bInReview && fromString == "In Review")
                                            {
                                                if (!bOnceRejected)
                                                    nNoFTRCnt--;
                                                nResolvedCnt++;
                                                bReviewtoResolved = true;
                                            }
                                            else
                                                continue;

                                            if (nResolvedCnt > 1)
                                            {
                                                strResolvedDate += cstrDelimiter + ConvertToDate(creationtime);
                                                strResolvedby += cstrDelimiter + authorname;
                                            }
                                            else
                                            {
                                                strResolvedDate += ConvertToDate(creationtime);
                                                strResolvedby += authorname;
                                            }
                                        }
                                        else if (toString == "In Review")
                                        {
                                            if (IsFromSelectedTeam(authorname) && time.Date.CompareTo(dtPicker.Value.Date) >= 0 && time.Date.CompareTo(dtPickerEnd.Value.Date) <= 0)
                                                bInReview = true;
                                        }
                                        //REOPENED ? Criteria # 1 : "Reopened" any time in full history
                                        else if (toString == "Reopened")
                                        {
                                            bFTR = false;
                                            bReopened = true;
                                            nReopenedCnt++;
                                            if (nReopenedCnt > 1)
                                                strReopenedDate += cstrDelimiter + ConvertToDate(creationtime);
                                            else
                                                strReopenedDate += ConvertToDate(creationtime);
                                        }
                                        //CODE DEFECT ? Criteria # 1 : "In Review" to "Ready to Dev" in full history
                                        else if (fromString == "In Review" && (toString == "Ready for Dev" || toString == "Open"))
                                        {
                                            bInReview = false;
                                            bOnceRejected = true;
                                            nNoFTRCnt++;
                                            if (nNoFTRCnt > 1)
                                                strReviewDate += cstrDelimiter + ConvertToDate(creationtime);
                                            else
                                                strReviewDate += ConvertToDate(creationtime);
                                        }
                                        else if (fromString == "Resolved" && (toString == "Closed"))
                                        {
                                            if (!bReopened)
                                                bFTR = true;
                                            nNoFTRCnt--;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lblMessage.Text = "ERROR occurred while generating report !";
                                    MessageBox.Show("Specific Error1 ::" + ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            lblMessage.Text = "ERROR occurred while generating report !";
                            MessageBox.Show("Specific Error2 ::" + ex.Message);
                        }
                    }
                    if (nResolvedCnt > 0 || bInReview )
                    {
                        nResolved++;

                        ListViewItem lvi = new ListViewItem(jiraid.strIssuetype);//Issue Type
                        lvi.SubItems.Add(String.Format("{0}({1})", changelogResponse.key, nResolvedCnt));//Jira ID(No of times resolved )
                        lvi.SubItems.Add(jiraid.strSummary);//Status of issue
                        lvi.SubItems.Add(jiraid.strStatus);//Status of issue
                        lvi.SubItems.Add(jiraid.strAssignee);//Assignee
                        lvi.SubItems.Add(strResolvedby);//Resolved by
                        lvi.SubItems.Add(strResolvedDate);//Resolved Date

                        if (nReopenedCnt > 0)
                        {
                            lvi.SubItems.Add(String.Format("Yes({0})", nReopenedCnt));//Reopened
                            lvi.SubItems.Add(strReopenedDate);//Reopened Date
                            nReopened++;
                        }
                        else
                        {
                            lvi.SubItems.Add("");//Reopened
                            lvi.SubItems.Add("");//Reopened Date
                        }
                        if (nNoFTRCnt > 0)
                        {
                            lvi.SubItems.Add(String.Format("No({0})", nNoFTRCnt));//Closed
                            lvi.SubItems.Add(strReviewDate);//Closed Date                                   *
                        }
                        else
                        {
                            if (bReviewtoResolved || bFTR )
                            {
                                nFirstTimeRight++;
                                lvi.SubItems.Add("Yes");
                                lvi.SubItems.Add("");
                            }
                            else
                            {
                                lvi.SubItems.Add("");
                                lvi.SubItems.Add("");
                            }
                        }
                        lvDefectleakage.Items.Add(lvi);
                    }
                }
                else if (rdoPlannedCompleted.Checked)
                {
                    string strResolvedDate = "";
                    DateTime firstResolvedDate = dtPickerEnd.Value;
                    bool bResolved = false;
                    bool bAssignedTaskSymphony = false;
                    bool bBlocked = false;
                    bool bPlanned = false;
                    bool binReview = false;
                    string strAssignedDate = "";

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
                        if (bAssignedTaskSymphony && bResolved && bBlocked)
                            break;

                        for (int j = 0; j < changelogResponse.changelog.histories[i].items.Length; j++)
                        {
                            DateTime time = Convert.ToDateTime(changelogResponse.changelog.histories[i].created);
                            string field = changelogResponse.changelog.histories[i].items[j].field;
                            string toString = changelogResponse.changelog.histories[i].items[j].toString;
                            string fromString = changelogResponse.changelog.histories[i].items[j].fromString;
                            string creationtime = changelogResponse.changelog.histories[i].created;
                            string authorname = changelogResponse.changelog.histories[i].author.displayName;

                            //PLANNED ? Criteria # 1 Assigned to some one in the team in this duration
                            //PLANNED ? Criteria # 2 Already assigned to someone in team AND changed to "Ready for Dev" 
                            //PLANNED ? Criteria # 3 Already assigned to someone in team AND Not remained "Blocked" for the whole duration

                            //COMPLETED ? Criteria # 1 status changed to "Resolved || Closed" AND didn't "Reopen"
                            //COMPLETED ? Criteria # 2 status changed to "In Review" and didn't change to "Ready for Dev" for the remining duration
                            //"In Review" to "Ready to Dev" is considered as INCOMPLETE

                            if (field == "assignee")
                            {
                                string strTmName = GetTeamName(toString);
                                if (strTmName == "")
                                    continue;
                                //Is assigned to some one in selected team
                                if (String.Compare(strTmName, GetSelectedTeamName(), true) != 0)
                                    continue;                                

                                //PLANNED ? Criteria # 1 Assigned to some one in the team in this duration
                                if(IsFromSelectedTeam(toString))
                                    bAssignedTaskSymphony = true;
                                else
                                    bAssignedTaskSymphony = false;

                                if (bAssignedTaskSymphony && time.Date.CompareTo(dtPicker.Value.Date) >= 0 && time.Date.CompareTo(dtPickerEnd.Value.Date) <= 0)
                                {
                                    strAssignedDate = ConvertToDate(creationtime);
                                    bPlanned = true;
                                }
                                
                                
                            }
                            if (field == "status")
                            {
                                if (time.Date.CompareTo(dtPicker.Value.Date) < 0 || time.Date.CompareTo(dtPickerEnd.Value.Date) > 0)
                                    continue;

                                //PLANNED ? Criteria # 2 Already assigned to someone in team AND changed to "Ready for Dev"  
                                if (bAssignedTaskSymphony)
                                {
                                    if (toString == "Ready for Dev" || toString == "Open")
                                    {
                                        strAssignedDate = ConvertToDate(creationtime);
                                        bPlanned = true;
                                    }
                                }
                                //COMPLETED ? Criteria # 1 status changed to "Resolved || Closed" AND didn't "Reopen"
                                if (bAssignedTaskSymphony && (toString == "Resolved" || toString == "Closed"))
                                {           
                                    bResolved = true;
                                    strResolvedDate = ConvertToDate(creationtime);                                    
                                }
                                //COMPLETED ? Criteria # 2 status changed to "In Review" and didn't change to "Ready for Dev" for the remining duration
                                if (toString == "In Review")
                                    binReview = true;
                                else if (fromString == "In Review")
                                    binReview = false;
                                //PLANNED ? Criteria # 3 Already assigned to someone in team AND Not remained "Blocked" for the whole duration
                                if (toString == "Blocked")
                                    bBlocked = true;
                                else if (fromString == "Blocked")
                                    bBlocked = false;
                            }                           
                        }
                    }
                    if (bAssignedTaskSymphony)
                    {
                        if (bPlanned)
                            nPlanned++;
                        else
                            return;

                        if (bResolved || binReview)
                            nCompleted++;


                        ListViewItem lvi = new ListViewItem(jiraid.strJiraID);//Issue Type
                        lvi.SubItems.Add(jiraid.strSummary);//Summary of issue
                        lvi.SubItems.Add(strAssignedDate);//Assigned Date
                        lvi.SubItems.Add(Converttojiratimeformat((long)jiraid.aggtime));//Time Spent
                        lvi.SubItems.Add(jiraid.strStatus);//Jira Status
                        lvi.SubItems.Add(bResolved ? strResolvedDate : "");//Resolved Date
                        lvi.SubItems.Add(jiraid.strAssignee);//Assignee

                        {
                            if(bResolved)
                                lvi.SubItems.Add("Actually RESOLVED");//Remark
                            else if(binReview)
                                lvi.SubItems.Add("Remained IN REVIEW, considered RESOLVED");//Remark
                            else if(bBlocked)
                                lvi.SubItems.Add("Remained BLOCKED");//Remark
                        }

                        lvPlannedCompleted.Items.Add(lvi);
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "ERROR occurred while generating report !";
                MessageBox.Show("Error ::" + ex.Message);
            }
        }

        private bool IsFromSelectedTeam(string strName)
        {
            string strTmName = GetTeamName(strName);

            if (String.Compare(cmbTeamMember.Text, "All Team Members", true) != 0 && String.Compare(cmbTeamMember.Text, strName, true) != 0)
                return false;

            if (String.Compare(strTmName, GetSelectedTeamName(), true) != 0)
                return false; 

            return true;
        }
        private string Converttojiratimeformat(long nSec)
        {
            int nHours =(int) (nSec / 3600);
            if (nHours == 0)
                return "";

            if (nHours > 8 )
            {
                if ((nHours % 8) != 0)
                    return String.Format("{0}d {1}h",(int)nHours/8,(int)nHours%8);
                else
                    return String.Format("{0}d", (int)nHours / 8);
            }
            else
            {
                return String.Format("{0}h", (int)nHours);
            }
        }
        private string ConvertToDate(string strTime)
        {
            DateTime time = Convert.ToDateTime(strTime);
            return String.Format("{0}-{1}-{2}", time.Date.Year, time.Date.Month >= 10 ? Convert.ToString(time.Date.Month) : "0" + Convert.ToString(time.Date.Month), time.Date.Day >= 10 ? Convert.ToString(time.Date.Day) : "0" + Convert.ToString(time.Date.Day));
        }
        //MULTIHREADING :: Start
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            JiraID jiraid = (JiraID)e.Argument;
            try
            {
                Jiraresponse jiraresponse = new Jiraresponse();
                //if (!rdoAssignedSTC.Checked)
                {
                    string request = this.CreateRequestUrl(jiraid.strJiraID);
                    bool bExpanded = !(rdoAssignedSTC.Checked || rdoDailyreport.Checked || rdoDaterange.Checked);
                    jiraresponse.response = common.MakeRequestWithUrl(request, credent, bExpanded);
                    jiraresponse.jiraid = jiraid;
                    e.Result = jiraresponse as object;
                }               
            }
            catch (Exception ex)
            {
                lblMessage.Text = String.Format("ERROR occurred while getting data for JIRA ID : {0}", jiraid.strJiraID);
                //MessageBox.Show("Error - Internal ::" + ex.Message);
            }
            lock ("counter")
            {
                jiraCounter++;
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (responseList)
            {
                if (e.Result != null)
                {
                    responseList.Add((Jiraresponse)e.Result);
                }
                    pbLogprogress.Value = responseList.Count;
                    lblMessage.Text = String.Format("{0} / {1}", responseList.Count, numJiraIds);                

                if (responseList.Count == numJiraIds)
                {
                    pbLogprogress.Visible = false;
                    PopulateResultGrids();
                }                
            }
        }
        private void bw_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            //Console.WriteLine("Reached " + e.ProgressPercentage + "%");
        }
        //MULTIHREADING :: End

        private void GetDataforAllJiraIds(List<JiraID> jiralist)
        {
            responseList.Clear();
            numJiraIds = jiralist.Count;

            pbLogprogress.Visible = true;
            pbLogprogress.Maximum = jiralist.Count;
            jiraCounter=0;

            foreach (JiraID jiraid in jiralist)
            {
                BackgroundWorker _bw= new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bw.DoWork += bw_DoWork;
                _bw.ProgressChanged += bw_ProgressChanged;
                _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                _bw.RunWorkerAsync(jiraid);
            }
        }
        private void PopulateResultGrids()
        {
            foreach (Jiraresponse jiraresponse in responseList)
            {
                ProcessResponse(jiraresponse.response, jiraresponse.jiraid);
            }
            if (rdoDefectleakage.Checked)
            {
                lblResolved.Text = "Issues Resolved : " + Convert.ToString(nResolved);
                lblReopened.Text = "Issues Reopened : " + Convert.ToString(nReopened);
                lblFirstTimeRight.Text = "First Time Right-" + Convert.ToString(nFirstTimeRight);
            }
            else if (rdoPlannedCompleted.Checked)
            {
                lblResolved.Text = "Tasks Planned : " + Convert.ToString(nPlanned);
                lblReopened.Text = "Tasks Resolved : " + Convert.ToString(nCompleted);
            }

            lblMessage.Text = "Report Generated successfully !!!";
        }
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            try
            {
                EnableControls(false);
                // Reset UIs 
                nResolved = nReopened = nPlanned = nCompleted = nFirstTimeRight = 0;
                for (int i = 0; i < lvDailyReport.Items.Count; i++)
                {
                    lvDailyReport.Items[i].Remove();
                    i--;
                }
                for (int i = 0; i < lvDaterangeReport.Items.Count; i++)
                {
                    lvDaterangeReport.Items[i].Remove();
                    i--;
                }
                for (int i = 0; i < lvDefectleakage.Items.Count; i++)
                {
                    lvDefectleakage.Items[i].Remove();
                    i--;
                }
                for (int i = 0; i < lvAssigned.Items.Count; i++)
                {
                    lvAssigned.Items[i].Remove();
                    i--;
                }
                for (int i = 0; i < lvPlannedCompleted.Items.Count; i++)
                {
                    lvPlannedCompleted.Items[i].Remove();
                    i--;
                }
                
                List<JiraID> jiralist = GetListofJiraids();
                
                GetDataforAllJiraIds(jiralist);
                
            }
            catch (Exception ex)
            {
                lblMessage.Text = "ERROR occurred while generating report !";
                MessageBox.Show("Error ::" + ex.Message);
            }
            lblMessage.Visible = true; lblMessage.Refresh();
            EnableControls(true);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Export to CSV file name
            
            
            string fileName = "";

            if (rdoDailyreport.Checked)
            {
                DateTime dt = dtPicker.Value;
                string worklogfilename = String.Format("{0}-{1}-{2}-WorklogReport.csv", dt.Year, dt.Month, dt.Day);
                fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + worklogfilename;
                ListViewToCSV(lvDailyReport, fileName, false);
            }
            else if ( rdoDaterange.Checked )  
            {
                DateTime dt1 = dtPicker.Value; DateTime dt2 = dtPickerEnd.Value;
                string worklogfilename = String.Format("{0}-{1}-{2}-to-{3}-{4}-{5}-WorklogReport.csv", dt1.Year, dt1.Month, dt1.Day, dt2.Year, dt2.Month, dt2.Day);
                fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + worklogfilename;
                ListViewToCSV(lvDaterangeReport, fileName, false);
            }
            else if (rdoDefectleakage.Checked)
            {
                DateTime dt1 = dtPicker.Value; DateTime dt2 = dtPickerEnd.Value;
                string worklogfilename = String.Format("{0}-{1}-{2}-to-{3}-{4}-{5}-DefectLeakageReport.csv", dt1.Year, dt1.Month, dt1.Day, dt2.Year, dt2.Month, dt2.Day);
                fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + worklogfilename;
                ListViewToCSV(lvDefectleakage, fileName, false);
            }
            else if (rdoAssignedSTC.Checked)
            {
                DateTime dt = DateTime.Now;
                string worklogfilename = String.Format("{0}-{1}-{2}-WorkAssignmentonSTC.csv", dt.Year, dt.Month, dt.Day);
                fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + worklogfilename;
                ListViewToCSV(lvAssigned, fileName, false);
            }
            else if (rdoPlannedCompleted.Checked)
            {
                DateTime dt = DateTime.Now;
                string worklogfilename = String.Format("{0}-{1}-{2}-PlannedVsCompletedSTC.csv", dt.Year, dt.Month, dt.Day);
                fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + worklogfilename;
                ListViewToCSV(lvPlannedCompleted, fileName, false);
            }
            System.Diagnostics.Process.Start(fileName);
        }

        private void btnSendmail_Click(object sender, EventArgs e)
        {

        }

        private List<JiraID> GetListofJiraidsOnebyOne(List<JiraID> jiraList, string startAt)
        {
            string strUrl = CreateRequestJiraIds(startAt);
            IssueData issuedataresponse = common.MakeRequestJiraID(strUrl , credent);

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

                    if (!rdoAssignedSTC.Checked && (dtPicker.Value - dt).TotalDays > 1.0)
                        continue;

                    JiraID jiraid = new JiraID();

                    if(issuedataresponse.issues[i].fields.project != null )
                        jiraid.strProject = issuedataresponse.issues[i].fields.project.key;

                    if (String.Compare(cmbProjects.Text, "ALL PROJECTS", true) != 0 && String.Compare(cmbProjects.Text, jiraid.strProject, true) != 0)
                        continue;

                    if( issuedataresponse.issues[i].fields.issuetype != null )
                        jiraid.strIssuetype = issuedataresponse.issues[i].fields.issuetype.name;
                    if ( issuedataresponse.issues[i].fields.priority != null )
                        jiraid.strPriority = issuedataresponse.issues[i].fields.priority.name;

                    jiraid.strJiraID = issuedataresponse.issues[i].key;

                    if(issuedataresponse.issues[i].fields.status != null )
                        jiraid.strStatus = issuedataresponse.issues[i].fields.status.name;

                    if (issuedataresponse.issues[i].fields.assignee != null)
                        jiraid.strAssignee = issuedataresponse.issues[i].fields.assignee.displayName;

                    jiraid.strSummary = issuedataresponse.issues[i].fields.summary;
                    jiraid.aggtime = issuedataresponse.issues[i].fields.timespent != null ? issuedataresponse.issues[i].fields.timespent : 0; //in secs
                    jiraid.duedate = issuedataresponse.issues[i].fields.duedate;
                    jiraid.originalestimate = issuedataresponse.issues[i].fields.timeoriginalestimate != null ? issuedataresponse.issues[i].fields.timeoriginalestimate : 0;
                    jiraList.Add(jiraid);
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "ERROR occurred while generating report !";
                    //MessageBox.Show("Error - Internal ::" + ex.Message);
                }
            }
            return jiraList;
        }
        private List<JiraID> GetListofJiraids()
        {            
            lblMessage.Text = "Wait ... getting list of JIRA IDs to look into ....";
            lblMessage.Visible = true;
            lblMessage.Refresh();

            List<JiraID> strLine = new List<JiraID>();
            strLine = GetListofJiraidsOnebyOne(strLine, "0");
            strLine = GetListofJiraidsOnebyOne(strLine, "1000");
            strLine = GetListofJiraidsOnebyOne(strLine, "2000");
            strLine = GetListofJiraidsOnebyOne(strLine, "3000");
            //First time
            
            lblMessage.Visible = false;
            return strLine;
        }


        public void ListViewToCSV(ListView listView, string filePath, bool includeHidden)
        {
            StringBuilder sb = new StringBuilder();

            //Making columns!
            foreach (ColumnHeader ch in listView.Columns)
            {
                sb.Append(ch.Text + ",");
            }
            sb.AppendLine();

            //Looping through items and subitems
            foreach (ListViewItem lvi in listView.Items)
            {
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    if (lvs.Text.Trim() == string.Empty)
                        sb.Append(" ,");
                    else
                        sb.Append(lvs.Text + ",");
                }
                sb.AppendLine();
            }
            SaveToFile(filePath, sb);
        }
        public string ListViewToHTML(ListView lv)
        {
            string strHtmlbody = "<html><head><title></title></head><body>";
                  
            if(rdoDefectleakage.Checked)
                strHtmlbody +=String.Format("<p><b>Resolved ::  {0}      Reopened :: {1}</b></p>",nResolved,nReopened);
            else if(rdoPlannedCompleted.Checked)
                strHtmlbody += String.Format("<p><b>Planned ::  {0}      Completed :: {1}</b></p>", nPlanned, nCompleted);

                strHtmlbody += "</br> <table style=\"font-family:arial;\" border=\"1\" cellspacing=\"0\">";
            //Making columns!
            strHtmlbody += "<tr>";
            foreach (ColumnHeader ch in lv.Columns)
            {
                //sb.Append(ch.Text + ",");
                strHtmlbody += String.Format("<td><b>{0}</b></td>", ch.Text);
            }
            strHtmlbody += "</tr>";

            //Looping through items and subitems
            foreach (ListViewItem lvi in lv.Items)
            {
                strHtmlbody += "<tr>";
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    strHtmlbody += String.Format("<td>{0}</td>", lvs.Text);
                }
                strHtmlbody += "</tr>";
            }
            strHtmlbody += "</table><br /></body></html>";

            return strHtmlbody;
        }
        private void SaveToFile(string fileName , StringBuilder sb)
        {
            System.IO.TextWriter w = new System.IO.StreamWriter(fileName);
            w.Write(sb.ToString());
            w.Flush();
            w.Close();
        }

        private void lvReport_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            if (rdoDailyreport.Checked)
                this.lvDailyReport.Sort();
            else if (rdoDaterange.Checked)
                this.lvDaterangeReport.Sort();
            else if (rdoAssignedSTC.Checked)
                this.lvAssigned.Sort();
            else if (rdoDefectleakage.Checked)
                this.lvDefectleakage.Sort();
            else if (rdoPlannedCompleted.Checked)
                this.lvPlannedCompleted.Sort();
        }

        private void GetCredentialnUsers()
        {
            string jsonfilename = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\users.json";
            FileStream fileStream = new FileStream(jsonfilename, FileMode.Open, FileAccess.ReadWrite);
            jiraUsers = new List<Jirauser>();
            projects = new List<string>(); 

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Users));
            object objResponse = jsonSerializer.ReadObject(fileStream);
            Users jsonResponse = objResponse as Users;

            credent.userid = txtUserid.Text = jsonResponse.credential.userid;
            credent.passwd = txtPasswd.Text = jsonResponse.credential.password;
            
            for (int i = 0; i < jsonResponse.symphonyusers.Length; i++)
            {
                Jirauser ju = new Jirauser();
                ju.name = jsonResponse.symphonyusers[i].name;
                ju.emailid = jsonResponse.symphonyusers[i].emailid;
                ju.team = jsonResponse.symphonyusers[i].team;
                ju.bWorklog = false;

                jiraUsers.Add(ju);
            }
            for (int i = 0; i < jsonResponse.projects.Length; i++)
            {
                projects.Add(jsonResponse.projects[i]);
            }
        }
        private string GetFormattedDate(DateTime time)
        {
            return String.Format("{0}-{1}-{2}", time.Date.Year, time.Date.Month >= 10 ? Convert.ToString(time.Date.Month) : "0" + Convert.ToString(time.Date.Month), time.Date.Day >= 10 ? Convert.ToString(time.Date.Day) : "0" + Convert.ToString(time.Date.Day));
        }
        private string GetTeamName(string strEmailid)
        {
            foreach (Jirauser ju in jiraUsers)
            {
                if (ju.emailid.CompareTo(strEmailid) == 0)
                    return ju.team;
                else if (ju.name.CompareTo(strEmailid) == 0)
                    return ju.team;
            }   
            return "";
        }
        private string GetEmailid(string strName)
        {
            foreach (Jirauser ju in jiraUsers)
            {
                if (ju.name.CompareTo(strName) == 0)
                    return ju.emailid;
            }
            return "";
        }

        private List<Jirauser> GetWorklogDefaulterslist()
        {
            List<Jirauser> jusers = new List<Jirauser>();
            List<Jirauser> defaulters = new List<Jirauser>();
            
            //Looping through items and subitems
            foreach (Jirauser jusr in jiraUsers)
            {
                Jirauser ju = jusr;
                foreach (ListViewItem lvi in lvDailyReport.Items)
                {
                    if (String.Compare(jusr.name, lvi.SubItems[1].Text, true) == 0)
                    {
                        ju.bWorklog = true;
                        continue;
                    }
                }
                jusers.Add(ju);
            }
            foreach (Jirauser jusr in jusers)
            {
                string strTeam = GetSelectedTeamName() ;


                if(!jusr.bWorklog && String.Compare(strTeam,jusr.team,true) == 0 )
                    defaulters.Add(jusr);
            }
            return defaulters;
        }
        RadioButton GetCheckedRadio(Control container)
        {
            foreach (var control in container.Controls)
            {
                RadioButton radio = control as RadioButton;

                if (radio != null && radio.Checked)
                {
                    return radio;
                }
            }

            return null;
        }
        private void btnSendMailReport_Click(object sender, EventArgs e)
        {
            /*
            //Mail Config 
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem mailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
            
            //Finding to ---------------------------------------------------------------
            if (String.Compare(cmbTeamMember.Text, "All Team Members", true) == 0)
            {
                mailItem.To = rdoJukebox.Checked ? "TT_DEV_TEAM@symphonyteleca.com" : (rdoServer.Checked ?"TT_SERVER_TEAM@symphonyteleca.com":"TT_QC_TEAM@symphonyteleca.com");
            }
            else
            {
                mailItem.To = GetEmailid(cmbTeamMember.Text);
            }
            //--------------------------------------------------------------------------

            DateTime dt = dtPicker.Value;
            DateTime dtEnd = dtPickerEnd.Value;

            string strDate = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            string strEndDate = String.Format("{0}/{1}/{2}", dtEnd.Day, dtEnd.Month, dtEnd.Year);

            string strReportDate;

            //Finding Subject ****************************************************************
            if (rdoDailyreport.Checked)
                strReportDate = "for " + strDate;
            else if (rdoDaterange.Checked || rdoDefectleakage.Checked || rdoPlannedCompleted.Checked)
                strReportDate = "during " + strDate + " to " + strEndDate;
            else
                strReportDate = "";

            RadioButton rb = GetCheckedRadio((Control)grpboxReportType);
            mailItem.Subject = "TouchTunes GOC "+GetSelectedTeamName() +" Team -:-" + rb.Text + " Report " + strReportDate;
            //********************************************************************************
            mailItem.CC = "";// rdoJukebox.Checked ? "TT_DEV_TEAM@symphonyteleca.com" : "TT_SERVER_TEAM@symphonyteleca.com";

            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;

            /////Which ListView to send ?///////////////////////////////
            ListView lv = null;

            if (rdoDailyreport.Checked)
                lv = lvDailyReport;
            else if (rdoDaterange.Checked)
                lv = lvDaterangeReport;
            else if (rdoDefectleakage.Checked)
                lv = lvDefectleakage;
            else if (rdoAssignedSTC.Checked)
                lv= lvAssigned;
            else if (rdoPlannedCompleted.Checked)
                lv = lvPlannedCompleted;
            ///////////////////////////////////////////////////////////
            mailItem.HTMLBody = ListViewToHTML(lv); 
            mailItem.Display(true);
            */
        }

        private void btnSenddailyreport_Click(object sender, EventArgs e)
        {
            /*
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem mailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            mailItem.To = "";
            DateTime dt = dtPicker.Value;
            string strDate = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            mailItem.Subject = "WORKLOG REPORT for " + strDate;
            mailItem.CC = rdoJukebox.Checked ? "TT_DEV_TEAM@symphonyteleca.com" : (rdoServer.Checked ? "TT_SERVER_TEAM@symphonyteleca.com" : "TT_QC_TEAM@symphonyteleca.com");
            
            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;

            string strHtmlbody = "<html><head><title></title></head>"
                    + "<body><p><b>Following is the work log report for the team for " + strDate + "</b></p>"
                    + "</br> <table border=\"1\" cellspacing=\"0\">"
                    + "<tr><td><b>NAME</b></td><td><b>JIRA ID</b></td><td><b>WORK LOG</b></td></tr>";

            foreach (ListViewItem lvi in lvDailyReport.Items)
            {
                strHtmlbody += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", lvi.SubItems[1].Text, lvi.SubItems[2].Text, lvi.SubItems[8].Text);
            }              

            strHtmlbody += "</table><br /></body></html>";
            mailItem.HTMLBody = strHtmlbody;
            mailItem.Display(true);*/
        }

        private void btnMailtoDefaulter_Click(object sender, EventArgs e)
        {
            /*
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem mailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            List<Jirauser> defaulters = GetWorklogDefaulterslist();
            string strTo = "";
            foreach (Jirauser jusr in defaulters)
            {
                strTo += jusr.emailid + ";";
            }

            mailItem.To = strTo;
            DateTime dt = dtPicker.Value;
            string strDate = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            mailItem.Subject = "WORKLOG DEFAULTERS LIST for " + strDate;
            mailItem.CC = rdoJukebox.Checked ? "TT_DEV_TEAM@symphonyteleca.com" : "TT_SERVER_TEAM@symphonyteleca.com";
            //mailItem.Body = "Following members haven't logged their work for " + strDate;
            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            string strHtmlbody = "<html><head><title></title></head>"
                    + "<body><p><b>Following members haven't logged their work in JIRA for " + strDate + "</b></p>"
                    +"</br> <table border=\"1\" cellspacing=\"0\">";
                    
            foreach (Jirauser ju in defaulters)
            {
                strHtmlbody += String.Format("<tr><th><font color=\"0000FF\">{0}</font></th></tr>", ju.name);
            }

            strHtmlbody += "</table><br /></body></html>";
            mailItem.HTMLBody = strHtmlbody;
            mailItem.Display(true);*/
        }
        private void rdoReportType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = sender as RadioButton;

            if(rdo == rdoDailyreport)
            {
                btnPendingResult.Visible = false;
                lblSelectdate.Text = "Select Date :";
                lvDailyReport.Visible = true;
                lvDaterangeReport.Visible = false;
                lvDefectleakage.Visible = false;
                lvAssigned.Visible = false;
                lvPlannedCompleted.Visible = false;
                dtPicker.Visible = true;
                dtPickerEnd.Visible = false;
                lblResolved.Text = "";
                lblReopened.Text = "";
                lblFirstTimeRight.Text = "";
                btnSenddailyreport.Visible = btnMailtoDefaulter.Visible = true;
                
            }
            else if (rdo == rdoDaterange)
            {
                btnPendingResult.Visible = true;
                lblSelectdate.Text = "Select Date\nRange :";
                lvDailyReport.Visible = false;
                lvDaterangeReport.Visible = true;
                lvDefectleakage.Visible = false;
                lvAssigned.Visible = false;
                lvPlannedCompleted.Visible = false;
                dtPicker.Visible = true;
                dtPickerEnd.Visible = true;
                lblResolved.Text = "";
                lblReopened.Text = "";
                lblFirstTimeRight.Text = "";
                btnSenddailyreport.Visible = btnMailtoDefaulter.Visible = false;
            }
            else if (rdo == rdoAssignedSTC)
            {
                btnPendingResult.Visible = true;
                lblSelectdate.Text = "";
                lvDailyReport.Visible = false;
                lvDaterangeReport.Visible = false;
                lvDefectleakage.Visible = false;
                lvAssigned.Visible = true;
                lvPlannedCompleted.Visible = false;
                dtPicker.Visible = false;
                dtPickerEnd.Visible = false;
                lblResolved.Text = "";
                lblReopened.Text = "";
                lblFirstTimeRight.Text = "";
                btnSenddailyreport.Visible = btnMailtoDefaulter.Visible = false;
            }
            else if (rdo == rdoDefectleakage)
            {
                btnPendingResult.Visible = true;
                lblSelectdate.Text = "Select Date\nRange :";
                lvDailyReport.Visible = false;
                lvDaterangeReport.Visible = false;
                lvDefectleakage.Visible = true;
                lvAssigned.Visible = false;
                lvPlannedCompleted.Visible = false;
                dtPicker.Visible = true;
                dtPickerEnd.Visible = true;
                lblResolved.Text = "Issues Resolved : ";
                lblReopened.Text = "Issues Reopened : ";
                lblFirstTimeRight.Text = "First Time Right-";
                btnSenddailyreport.Visible = btnMailtoDefaulter.Visible = false;
            }
            else if (rdo == rdoPlannedCompleted)
            {
                btnPendingResult.Visible = true;
                lblSelectdate.Text = "Select Date\nRange :";
                lvDailyReport.Visible = false;
                lvDaterangeReport.Visible = false;
                lvDefectleakage.Visible = false;
                lvAssigned.Visible = false;
                lvPlannedCompleted.Visible = true;
                dtPicker.Visible = true;
                dtPickerEnd.Visible = true;
                lblResolved.Text = "Tasks Planned : ";
                lblReopened.Text = "Tasks Completed : ";
                lblFirstTimeRight.Text = "";
                btnSenddailyreport.Visible = btnMailtoDefaulter.Visible = false;
            }
        }

        private void TeamSelectionChanged(object sender, EventArgs e)
        {
            string strTeamName = GetSelectedTeamName();

            cmbTeamMember.Items.Clear();
            cmbTeamMember.Items.Add("All Team Members");

            foreach (Jirauser ju in jiraUsers)
            {
                if(String.Compare(ju.team , strTeamName , true )  == 0 )
                    cmbTeamMember.Items.Add(ju.name);
            }
            cmbTeamMember.SelectedIndex = 0;
        }

        private void btnPendingResult_Click(object sender, EventArgs e)
        {
            if (responseList.Count != numJiraIds)
            {
                pbLogprogress.Visible = false;
                PopulateResultGrids();
            }  
        }

        private string GetSelectedTeamName()
        {
            if (rdoJukebox.Checked)
                return "Jukebox";
            else if (rdoServer.Checked)
                return "Server";
            else
                return "QC";
        }
    }    
}
