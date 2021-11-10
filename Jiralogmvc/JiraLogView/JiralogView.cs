using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JiralogMVC.Controller;
using System.Net;
using System.IO;

namespace JiralogMVC.View
{    
    public partial class JiralogView : Form, IJiralogView
    {
        struct Column
        {
            public string title;
            public int width;
            public HorizontalAlignment horAlign;
            public Column(string t,int w,HorizontalAlignment ha)
            {
                title = t;
                width = w;
                horAlign = ha;
            }
        }
        //This can be also moved to Config File
        static Column[] _columnsDaily = {
                                       new Column("Name",180,HorizontalAlignment.Left),
                                       new Column("Sprint",80,HorizontalAlignment.Left),
                                       new Column("JIRA ID",60,HorizontalAlignment.Left),
                                       new Column("Status",60,HorizontalAlignment.Left),
                                       new Column("Due Date",60,HorizontalAlignment.Left),
                                       new Column("Orig. Estim.",60,HorizontalAlignment.Left),
                                       new Column("Agg Time",60,HorizontalAlignment.Left),
                                       new Column("Time Logged",60,HorizontalAlignment.Left),
                                       new Column("Work Log",-2,HorizontalAlignment.Left)
                                   };
        static Column[] _columnsRange = {
                                       new Column("Category",120,HorizontalAlignment.Left),
                                       new Column("Resource",70,HorizontalAlignment.Center),
                                       new Column("Date",70,HorizontalAlignment.Center),
                                       new Column("Hours Spent",70,HorizontalAlignment.Center),
                                       new Column("JIRA ID",70,HorizontalAlignment.Left),
                                       new Column("Comments",-2,HorizontalAlignment.Left),
                                   };
        static Column[] _columnsPerformance = {
                                                new Column("Team Member",180, HorizontalAlignment.Left)
                                                //Rest will be added dynamically 
                                              };
        static Column[] _columnsAssignTimeSpent = {
                                                new Column("Project",60, HorizontalAlignment.Center),
                                                new Column("Jira ID",100, HorizontalAlignment.Center),
                                                new Column("Priority",60, HorizontalAlignment.Center),
                                                new Column("Status",100, HorizontalAlignment.Center),
                                                new Column("Assignee",80, HorizontalAlignment.Center),
                                                new Column("Time Spent",60, HorizontalAlignment.Center),
                                                //Rest will be added dynamically 
                                              };
        static Column[] _columnsAssign = {
                                        new Column("CreatedBy",120,HorizontalAlignment.Left),
                                       new Column("JIRA ID",80,HorizontalAlignment.Left),
                                       new Column("Summary",40,HorizontalAlignment.Left),
                                       new Column("Priority",80,HorizontalAlignment.Left),
                                       new Column("Status",80,HorizontalAlignment.Left),
                                       new Column("Current Assignee",120,HorizontalAlignment.Left),
                                       new Column("Orig. Estim.",60,HorizontalAlignment.Left),
                                       new Column("Days in Backlog",80,HorizontalAlignment.Center),
                                       new Column("Daysin Assigned",80,HorizontalAlignment.Center),
                                       new Column("Days In Progress",80,HorizontalAlignment.Center),                                       
                                       new Column("Days in Review",80,HorizontalAlignment.Left),
                                       new Column("Days in Testing",80,HorizontalAlignment.Left),
                                       new Column("Implemented By",120,HorizontalAlignment.Left),
                                       new Column("Tested By",120,HorizontalAlignment.Left),

                                   };
        static Column[] _columnsDefectLeakage = {
                                       new Column("Issue Type",80,HorizontalAlignment.Left),
                                       new Column("Issues Resolved",100,HorizontalAlignment.Left),
                                       new Column("Summary",30,HorizontalAlignment.Left),
                                       new Column("Jira Status",80,HorizontalAlignment.Left),
                                       new Column("Assignee",200,HorizontalAlignment.Left),
                                       new Column("Resolved By",200,HorizontalAlignment.Left),
                                       new Column("Resolved Date",150,HorizontalAlignment.Left),
                                       new Column("Reopened",150,HorizontalAlignment.Left),
                                       new Column("Reopened By",150,HorizontalAlignment.Left),
                                       new Column("Reopened Date",150,HorizontalAlignment.Left),
                                       new Column("FTR ?",80,HorizontalAlignment.Left),
                                       new Column("Code Rejected By",-2,HorizontalAlignment.Left),
                                       new Column("Code Rejection Date",-2,HorizontalAlignment.Left)
                                   };
        static Column[] _columnsPlannedCompleted = {
                                       new Column("Assigned Issue",100,HorizontalAlignment.Left),
                                       new Column("Summary",300,HorizontalAlignment.Left),
                                       new Column("Assign. Date",100,HorizontalAlignment.Left),
                                       new Column("Time Spent",50,HorizontalAlignment.Left),
                                       new Column("Jira Status",100,HorizontalAlignment.Left),
                                       new Column("Resolved Date",100,HorizontalAlignment.Left),
                                       new Column("Assignee",120,HorizontalAlignment.Left),
                                       new Column("Remark if Any",-2,HorizontalAlignment.Left)
                                   };

        static Column[] _columnsComments = {
                                       new Column("Issue",100,HorizontalAlignment.Left),
                                       new Column("Summary",300,HorizontalAlignment.Left),
                                       new Column("Jira Status",100,HorizontalAlignment.Left),
                                       new Column("Assignee",100,HorizontalAlignment.Left),
                                       new Column("Commented Date",100,HorizontalAlignment.Left),
                                       new Column("Commented By",120,HorizontalAlignment.Left),
                                       new Column("Comment",-2,HorizontalAlignment.Left)
                                   };

        JiralogController _controller = null;
        List<Member> _listDefaulters;

        int _issuesResolved;
        int _issuesReopened;
        int _firstTimeRight;
        int _nPlanned;
        int _nCompleted;
        string _selectedTeamMember = "";
        string _teamName = "";
        string _project = "";
        string _release = "";
        int _nAssignedCount = 0;

        ListViewColumnSorter lvwColumnSorter;

        public JiralogView()
        {
            InitializeComponent();
            _listDefaulters = new List<Member>();

            lvwColumnSorter = new ListViewColumnSorter();
            this.lvReport.ListViewItemSorter = lvwColumnSorter;

            this.Text = "JIRA Reports v17.0.0";
        }

#region View Properties
        public string UserId
        {
            get { return txtUserid.Text; }
            set { txtUserid.Text = value; }
        }
        public string Passwd
        {
            get { return txtPasswd.Text;  }
            set { txtPasswd.Text = value; }
        }
        public string TeamName
        {
            get { return _teamName; }
        }

        public string ReleaseName
        {
            get { return _release; }
        }
        public CommonData.Team Team
        {
            get
            {
                if (rdoAllTeams.Checked)
                    return CommonData.Team.TEAM_ALL;
                else if (rdoDoseIQCommon.Checked)
                    return CommonData.Team.TEAM_DOSEIQ_COMMON;
                else if (rdoDoseIQDev.Checked)
                    return CommonData.Team.TEAM_DOSEIQ_DEV;
                else if (rdoDoseIQQAManual.Checked)
                    return CommonData.Team.TEAM_DOSEIQ_QA_MANUAL;
                else if (rdoDoseIQQAAutomation.Checked)
                    return CommonData.Team.TEAM_DOSEIQ_QA_AUTOMATION;
                else
                    return CommonData.Team.TEAM_INVALID;
            }
        }
        public string SelectedProject
        {
            get { return _project; }
        }
        public string SelectedRelease
        {
            get { return _release; }
        }
        public string SelectedTeammember
        {
            get { return _selectedTeamMember; }
        }
        public CommonData.ReportType ReportType
        {
            get
            {
                if (rdoPerformance.Checked)
                    return CommonData.ReportType.REPORT_TEAM_PERFORMANCE;
                else if (rdoDefectleakage.Checked)
                    return CommonData.ReportType.REPORT_DEFECT_LEAKAGE;
                else if (rdoAssignedSTC.Checked)
                    return CommonData.ReportType.REPORT_TEAM_ASSIGNED;
                else if (rdoComments.Checked)
                    return CommonData.ReportType.REPORT_JIRA_COMMENTS;
                else
                    return CommonData.ReportType.REPORT_INVALID_TYPE;
            }
        }
        public bool AssignedOnly
        {
            get { return chkAssignedOnly.Checked; }
        }
        public DateTime StartDate
        {
            get { return dtPicker.Value; }
        }
        public DateTime EndDate
        {
            get { return dtPickerEnd.Value; }
        }
        public int IssuesResolved
        {
            set { _issuesResolved = value; }
        }
        public int IssuesReopened
        {
            set { _issuesReopened = value; }
        }
        public int FirstTimeRight
        {
            set { _firstTimeRight = value; }
        }
        public string Message
        {
            set 
            {
                lblMessage.Visible = true;
                lblMessage.Text = value; 
            }
        }
        public string ErrorMessage
        {
            set
            {
                //lblMessage.Visible = true;
                //lblMessage.Text = value;
                //EnableControls(true);
            }
        }
        public int Progress
        {
            set {
                if (value <= pbLogprogress.Maximum)
                pbLogprogress.Value = value; 
            }
        }
        public bool ProgressVisibility
        {
            set { pbLogprogress.Visible = value; }
        }
        public int ProcessMaximum
        {
            set { pbLogprogress.Maximum = value; }
        }
#endregion

#region Public functions
        public void SetController(JiralogController controller)
        {
            _controller = controller;
        }
        public void SetInitialControlStates()
        {
            rdoAllTeams.Checked = true;
            rdoAssignedSTC.Checked = true;
            cmbTeamMember.SelectedIndex = 0;
        }
        public void AddProjects(List<AvailableProject> projects)
        {
            foreach(AvailableProject proj in projects)
                cmbProjects.Items.Add(proj.name);

            if(cmbProjects.Items.Count > 0)
                cmbProjects.SelectedIndex = 0;
        }
        public void AddMembers(List<string> members)
        {
            cmbTeamMember.Items.Clear();
            cmbTeamMember.Items.Add("All Team Members");

            foreach (string name in members)
                cmbTeamMember.Items.Add(name);

            if (cmbTeamMember.Items.Count > 0)
                cmbTeamMember.SelectedIndex = 0;
        }
        public void AddReleases(List<string> releases)
        {
            cmbRelease.Items.Clear();

            foreach (string rel in releases)
                cmbRelease.Items.Add(rel);

            if (cmbRelease.Items.Count > 0)
                cmbRelease.SelectedIndex = 0;
        }
        public void EnableControls(bool bEnable)
        {
            txtUserid.Enabled = txtPasswd.Enabled = rdoDoseIQDev.Enabled = rdoDoseIQDev.Enabled = rdoDoseIQQAManual.Enabled = rdoDoseIQQAAutomation.Enabled = 
            cmbProjects.Enabled = cmbTeamMember.Enabled =
            rdoAssignedSTC.Enabled = rdoDefectleakage.Enabled = rdoPerformance.Enabled = rdoComments.Enabled  =
            dtPicker.Enabled = dtPickerEnd.Enabled = btnGenerateReport.Enabled = btnSendMailReport.Enabled = btnExport.Enabled =
            btnDeleteRow.Enabled = btnMailtoDefaulters.Enabled = chkAssignedOnly.Enabled = rdoAllTeams.Enabled
            = bEnable; 
        }  
        public void ShowWaiting(bool bShow)
        {
            pbWaiting.Visible = bShow;
        }
        public void ClearGrid()
        {
            for (int i = 0; i < lvReport.Items.Count; i++)
            {
                lvReport.Items[i].Remove();
                i--;
            }
        }
        //Populate result
        public void Add_To_Grid_Daily_Worklog(IssueResponse response, JiraID jiraid, int index)
        {
            ListViewItem lvi = new ListViewItem(CommonFunc.GetFirstName(response.workLogs[index].author.displayName));
            lvi.SubItems.Add(jiraid.sprint);
            lvi.SubItems.Add(jiraid.strJiraID);
            lvi.SubItems.Add(jiraid.strStatus);
            lvi.SubItems.Add(jiraid.duedate);

            if (jiraid.duedate != null)
            {
                DateTime dt = Convert.ToDateTime(jiraid.duedate);
                if (dt.Date.CompareTo(dtPicker.Value.Date) < 0)
                    lvi.SubItems[3].BackColor = Color.Orange;
            }

            string origEstimate = CommonFunc.Converttojiratimeformat((long)jiraid.originalestimate);

            lvi.SubItems.Add(origEstimate.CompareTo("0h") != 0 ? origEstimate : ".-.");
            lvi.SubItems.Add(CommonFunc.Converttojiratimeformat((long)jiraid.aggtime));
            if (jiraid.originalestimate != 0 && jiraid.originalestimate < jiraid.aggtime)
                lvi.SubItems[5].BackColor = Color.OrangeRed;

            lvi.UseItemStyleForSubItems = false;
            lvi.SubItems.Add(response.workLogs[index].timeSpent);

            string s = response.workLogs[index].comment;
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\t", " ");
            s = s.Replace(",", " ");

            lvi.SubItems.Add(s);
            lvReport.Items.Add(lvi);
        }
        public void Add_To_Grid_Jira_Comments(CommentResponse response, JiraID jiraid, int index, DateTime dt)
        {
            string strDateTime = CommonFunc.GetFormattedDateTime(dt);

            ListViewItem lvi = new ListViewItem(jiraid.strJiraID);
            lvi.SubItems.Add(jiraid.strSummary);
            lvi.SubItems.Add(jiraid.strStatus);
            lvi.SubItems.Add(CommonFunc.GetFirstName(jiraid.strAssignee));
            lvi.SubItems.Add(strDateTime);
            lvi.SubItems.Add(CommonFunc.GetFirstName(response.comments[index].author.displayName));

            string s = response.comments[index].body;
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\t", " ");
            s = s.Replace(",", " ");

            lvi.SubItems.Add(s);
            lvReport.Items.Add(lvi);
        }
        public void Add_To_Grid_Daily_Worklog_Defaulters(Member def)
        {
            _listDefaulters.Add(def);

            ListViewItem lvi = new ListViewItem(CommonFunc.GetFirstName(def.name));
            lvi.SubItems.Add("WORK NOT LOGGED");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvReport.Items.Add(lvi);
        }

        public void Add_To_Grid_Worklog_Range_Defaulters(MembersWorkLog def)
        {
            //_listDefaulters.Add(def);

            ListViewItem lvi = new ListViewItem(_controller.GetTeamCategoryName(_controller.GetTeamName(def.name)));
            lvi.SubItems.Add(def.name);
            lvi.SubItems.Add(CommonFunc.GetFormattedDate(def.date));
            //lvi.SubItems.Add("");
            lvi.SubItems.Add(Convert.ToString(8-def.hrslogged));
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvReport.Items.Add(lvi);
        }

        public void Add_To_Grid_No_Work_Assigned(string miss)
        {
            ListViewItem lvi = new ListViewItem("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add(CommonFunc.GetFirstName(miss));
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("WORK NOT ASSIGNED");
            lvReport.Items.Add(lvi);
        }
        public void Add_To_Grid_Date_Range(IssueResponse response, JiraID jiraid, int index, DateTime dt)
        {
            string strDate = CommonFunc.GetFormattedDate(dt);

            ListViewItem lvi = new ListViewItem(_controller.GetTeamCategoryName(_controller.GetTeamName(response.workLogs[index].author.displayName)));
            lvi.SubItems.Add(response.workLogs[index].author.displayName);
            lvi.SubItems.Add(strDate);
            lvi.SubItems.Add(Convert.ToString(CommonFunc.ConvertIntoHrs(response.workLogs[index].timeSpent)));
            //lvi.SubItems.Add("");
            lvi.SubItems.Add(jiraid.strJiraID);
            string s = response.workLogs[index].comment;
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\t", " ");
            s = s.Replace(",", " ");

            lvi.SubItems.Add(s);
            lvReport.Items.Add(lvi);
        }

        public void Add_To_Grid_Team_Performance(List<PerfMemberData> perfMDList, int max_jiras)
        {
            //First create grid columns
            for (int i=0; i < max_jiras;i++)
            {
                ColumnHeader column = new ColumnHeader();
                column.Text = "";
                column.Width = 80;
                column.TextAlign = HorizontalAlignment.Center;
                lvReport.Columns.Add(column);
            }
            //Last column 
            {
                ColumnHeader column = new ColumnHeader();
                column.Text = "Total Hrs";
                column.Width = 80;
                column.TextAlign = HorizontalAlignment.Center;
                lvReport.Columns.Add(column);
            }
            //Now add data 
            foreach(PerfMemberData pmd in perfMDList)
            {
                ListViewItem lvi = new ListViewItem(CommonFunc.GetFirstName(pmd.strName));
                int i = 0;
                int timespent = 0;
                foreach (PerfNodeData pnd in pmd.nodeDataList)
                {
                    lvi.SubItems.Add(String.Format("{0}:[{1}h]",pnd.strJiraid,pnd.spentHrs));
                    timespent += pnd.spentHrs;
                    i++;
                }
                for (int j = 0; j < max_jiras - i; j++)
                    lvi.SubItems.Add("-");

                lvi.SubItems.Add(String.Format("{0} h", timespent));
                lvReport.Items.Add(lvi);
            }
        }
        public void Add_To_Grid_AssignedWork(ExpandedIssueResponse response, JiraID jiraid, BugTrackData Btd)
        {
            ListViewItem lvi = new ListViewItem(jiraid.createdBy);//Assignee
            lvi.SubItems.Add(jiraid.strJiraID);

            string summary = jiraid.strSummary.Length > 75 ? jiraid.strSummary.Substring(0, 75) : jiraid.strSummary;
            lvi.SubItems.Add(summary + " ...");
            lvi.SubItems.Add(jiraid.strPriority);
            
            //Color CHanges
            if (jiraid.strPriority == "Blocker" || jiraid.strPriority == "Critical")
            {
                lvi.SubItems[3].BackColor = Color.Red;
                lvi.SubItems[3].ForeColor = Color.White;
            }
            else if (jiraid.strPriority == "Major")
            {
                lvi.SubItems[3].BackColor = Color.Blue;
                lvi.SubItems[3].ForeColor = Color.White;
            }
            
            lvi.UseItemStyleForSubItems = false;

            lvi.SubItems.Add(jiraid.strStatus);//Status

            if (jiraid.strStatus == "Blocked" || jiraid.strStatus == "Review" )
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
            lvi.SubItems.Add(jiraid.strAssignee);
            lvi.SubItems.Add(CommonFunc.Converttojiratimeformat((long)(jiraid.originalestimate != null ? jiraid.originalestimate : 0)));//Original Estimate
            lvi.SubItems.Add(Btd.nDaysinBacklog);
            lvi.SubItems.Add(Btd.nDaysinAssignment);
            lvi.SubItems.Add(Btd.nDaysinProgress);
            lvi.SubItems.Add(Btd.nDaysinReview);
            lvi.SubItems.Add(Btd.nDaysinTesting);

            lvi.SubItems.Add(Btd.ImplementedBy);
            lvi.SubItems.Add(Btd.TestedBy);

            lvReport.Items.Add(lvi);
        }
        public void Add_To_Grid_TimeSpent(IssueResponse response, JiraID jiraid)
        {
            ListViewItem lvi = new ListViewItem(_controller.GetProjectCategoryName(jiraid.strProject));
            lvi.SubItems.Add(jiraid.strJiraID);
            lvi.SubItems.Add(jiraid.strPriority);
            lvi.SubItems.Add(jiraid.strStatus);
            lvi.SubItems.Add(CommonFunc.GetFirstName(jiraid.strAssignee));
            lvi.SubItems.Add(CommonFunc.ConverttoDays((long)(jiraid.aggtime != null ? jiraid.aggtime : 0)));

            lvReport.Items.Add(lvi);
        }
        public void Add_To_Grid_Defect_Leakage(ExpandedIssueResponse response, JiraID jiraid, DefectLeakageData dld)
        {
            _issuesResolved++;

            ListViewItem lvi = new ListViewItem(jiraid.strIssuetype);//Issue Type
            lvi.SubItems.Add(String.Format("{0}({1})", response.key, dld.nResolvedCnt));//Jira ID(No of times resolved )
            lvi.SubItems.Add(jiraid.strSummary);//Status of issue
            lvi.SubItems.Add(jiraid.strStatus);//Status of issue
            lvi.SubItems.Add(CommonFunc.GetFirstName(jiraid.strAssignee));//Assignee
            lvi.SubItems.Add(dld.strResolvedby);//Resolved by
            lvi.SubItems.Add(dld.strResolvedDate);//Resolved Date

            if (dld.nReopenedCnt > 0)
            {
                lvi.SubItems.Add(String.Format("Yes({0})", dld.nReopenedCnt));//Reopened
                lvi.SubItems.Add(dld.strReopenedBy);//Reopened By
                lvi.SubItems.Add(dld.strReopenedDate);//Reopened Date
                _issuesReopened++;
            }
            else
            {
                lvi.SubItems.Add("");//Reopened
                lvi.SubItems.Add("");//Reopened By
                lvi.SubItems.Add("");//Reopened Date
            }
            if (dld.nNoFTRCnt > 0)
            {
                lvi.SubItems.Add(String.Format("No({0})", dld.nNoFTRCnt));//Closed
                lvi.SubItems.Add(dld.strReviewedBy);//Review By   
                lvi.SubItems.Add(dld.strReviewDate);//Review Date                                   *
            }
            else
            {
                if (dld.bReviewtoResolved || dld.bFTR)
                {
                    _firstTimeRight++;
                    lvi.SubItems.Add("Yes");
                    lvi.SubItems.Add("");//Review By
                    lvi.SubItems.Add("");
                }
                else
                {
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");//Review By
                    lvi.SubItems.Add("");
                }
            }
            lvReport.Items.Add(lvi);

            lblResult1.Text = String.Format("Resolved = {0}",_issuesResolved);
            lblResult2.Text = String.Format("Reopened = {0}", _issuesReopened);
            lblResult3.Text = String.Format("FTR = {0}", _firstTimeRight);
            lblResult1.Visible = lblResult2.Visible = lblResult3.Visible = true;
        }
        public void Add_To_Grid_Planned_Completed(ExpandedIssueResponse response, JiraID jiraid, PlannedCompletedData pcd)
        {
            if (pcd.bPlanned)
                _nPlanned++;
            else
                return;

            if (pcd.bResolved || pcd.binReview)
                _nCompleted++;


            ListViewItem lvi = new ListViewItem(jiraid.strJiraID);//Issue Type
            lvi.SubItems.Add(jiraid.strSummary);//Summary of issue
            lvi.SubItems.Add(pcd.strAssignedDate);//Assigned Date
            lvi.SubItems.Add(CommonFunc.Converttojiratimeformat((long)jiraid.aggtime));//Time Spent
            lvi.SubItems.Add(jiraid.strStatus);//Jira Status
            lvi.SubItems.Add(pcd.bResolved ? pcd.strResolvedDate : "");//Resolved Date
            lvi.SubItems.Add(CommonFunc.GetFirstName(jiraid.strAssignee));//Assignee

            {
                if (pcd.bResolved)
                    lvi.SubItems.Add("Actually RESOLVED");//Remark
                else if (pcd.binReview)
                    lvi.SubItems.Add("Remained IN REVIEW, considered RESOLVED");//Remark
                else if (pcd.bBlocked)
                    lvi.SubItems.Add("Remained BLOCKED");//Remark
            }
            lvReport.Items.Add(lvi);
            lblResult1.Text = String.Format("Planned = {0}", _nPlanned);
            lblResult2.Text = String.Format("Completed = {0}", _nCompleted);
            lblResult1.Visible = lblResult2.Visible = true;
        }
#endregion

#region UI Event Handlers
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
            this.lvReport.Sort();
        }
        private void rdoReportType_CheckedChanged(object sender, EventArgs e)
        {
            ResetView();
            HideUnhideControls(ReportType);
            CreateGridColumns(ReportType);
        }
        private void TeamSelectionChanged(object sender, EventArgs e)
        {
            RadioButton rdoBtn = (RadioButton)sender;

            string teamname = "";

            if (rdoAllTeams.Checked)
                teamname = "All Teams";
            else if (rdoDoseIQCommon.Checked)
                teamname = "DoseIQ Common";
            else if (rdoDoseIQDev.Checked)
                teamname = "DoseIQ Dev";
            else if (rdoDoseIQQAManual.Checked)
                teamname = "DoseIQ QA Manual";
            else if (rdoDoseIQQAAutomation.Checked)
                teamname = "DoseIQ QA Automation";
            else
                teamname = "INVALID TEAM";

            _teamName = teamname;

            _controller.LoadTeammembers();
        }
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            ResetView();
            _listDefaulters.Clear();
            EnableControls(false);
            _controller.SetViewValuetoController();
            _controller.RequestData();
        }
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in lvReport.SelectedItems)
            {
                lvReport.Items.Remove(eachItem);
            }
        }
        public static string GetEncodedCredentials(CommonData.Credentials crd)
        {
            string mergedCredentials = string.Format("{0}:{1}", crd.userid, crd.passwd);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
        private void btnWatchIssue_Click(object sender, EventArgs e)
        {
            try
            {
                CommonData.Credentials crd;
                crd.userid = "";
                crd.passwd = "";
                string requestUrl = "http://192.168.16.35/rest/api/2/issue/OSII-2910/watchers";

                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";
                string base64Credentials = GetEncodedCredentials(crd);
                request.Headers.Add("Authorization", "Basic " + base64Credentials);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "\"\"";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }


                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnPendingResult_Click(object sender, EventArgs e)
        {
        }
#endregion

#region Private functions
        private void ResetView()
        {
            _issuesResolved = _issuesReopened = _firstTimeRight = 0;
            _nPlanned = _nCompleted = 0;

            ClearGrid();
            lblResult1.Visible = lblResult2.Visible = lblResult3.Visible = false;
            lblMessage.Visible = false;
        }
        private void HideUnhideControls(CommonData.ReportType rt)
        {
            lblSelectdate.Visible = true;

            if (rt == CommonData.ReportType.REPORT_JIRA_COMMENTS)
            {
                /*SHOW*/ 
                dtPicker.Visible = btnMailtoDefaulters.Visible = true;
                /*HIDE*/
                dtPickerEnd.Visible = lblResult1.Visible = lblResult2.Visible = lblResult3.Visible = false;
                    //btnPendingResult.Visible = false;
            }
            else if (rt == CommonData.ReportType.REPORT_TEAM_PERFORMANCE)
            {
                /*SHOW*/
                dtPicker.Visible = dtPickerEnd.Visible = /*btnPendingResult.Visible =*/ true;
                /*HIDE*/
                lblResult1.Visible = lblResult2.Visible = lblResult3.Visible =
                    btnMailtoDefaulters.Visible = false;
            }
            else if (rt == CommonData.ReportType.REPORT_TEAM_ASSIGNED)
            {
                /*SHOW*/
                //btnPendingResult.Visible = true;
                /*HIDE*/
                lblSelectdate.Visible = dtPicker.Visible = dtPickerEnd.Visible = lblResult1.Visible = lblResult2.Visible = lblResult3.Visible =
                    btnMailtoDefaulters.Visible = false;
            }
            else if (rt == CommonData.ReportType.REPORT_DEFECT_LEAKAGE)
            {
                /*SHOW*/
                dtPicker.Visible = dtPickerEnd.Visible = /*btnPendingResult.Visible =*/ true;
                //lblResult1.Visible = lblResult2.Visible = lblResult3.Visible = true;
                /*HIDE*/
                    btnMailtoDefaulters.Visible = false;
            }
            else
            {
                /*SHOW*/
                dtPicker.Visible = dtPickerEnd.Visible = /*btnPendingResult.Visible =*/ true;
                //lblResult1.Visible = lblResult2.Visible = true;
                /*HIDE*/
                btnMailtoDefaulters.Visible = lblResult3.Visible = false;
            }
        }
        private void CreateGridColumns(CommonData.ReportType rt)
        {
            Column[] columns;
            lvReport.Clear();

            switch(rt)
            {
                case CommonData.ReportType.REPORT_TEAM_PERFORMANCE:
                    columns = _columnsPerformance;
                    break;
                case CommonData.ReportType.REPORT_TEAM_ASSIGNED:
                    columns = _columnsAssign;
                    break;
                case CommonData.ReportType.REPORT_DEFECT_LEAKAGE:
                    columns = _columnsDefectLeakage;
                    break;
                case CommonData.ReportType.REPORT_JIRA_COMMENTS:
                    columns = _columnsComments;
                    break;
                default:
                    columns = _columnsPlannedCompleted;
                    break;
            }
             
             foreach (Column col in columns)
             {
                 ColumnHeader column = new ColumnHeader();
                 column.Text = col.title;
                 column.Width = col.width;
                 column.TextAlign = col.horAlign;
                 lvReport.Columns.Add(column);
             }
        }

        #endregion

        private void cmbTeamMember_SelectedValueChanged(object sender, EventArgs e)
        {
            _selectedTeamMember = cmbTeamMember.Text;
        }

        private void cmbProjects_SelectedValueChanged(object sender, EventArgs e)
        {
            _project = cmbProjects.Text;
            _controller.LoadReleases();
        }

        private void cmbRelease_SelectedValueChanged(object sender, EventArgs e)
        {
            _release = cmbRelease.Text;
        }
    }
}
