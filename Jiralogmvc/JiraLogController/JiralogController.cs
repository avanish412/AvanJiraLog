using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiralogMVC.Controller;
using static JiralogMVC.Controller.CommonData;

namespace JiralogMVC.Controller
{
    public interface IJiralogView
    {
        //Properties
        string UserId { get; set; }
        string Passwd { get; set; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        CommonData.Team Team { get; }
        string SelectedProject { get; }
        string SelectedRelease { get; }
        string SelectedTeammember { get; }
        CommonData.ReportType ReportType { get; }
        bool AssignedOnly { get; }
        int IssuesResolved { set; }
        int IssuesReopened { set; }
        int FirstTimeRight { set; }
        string Message { set; }
        string ErrorMessage { set; }
        int Progress { set; }
        bool ProgressVisibility { set; }
        int ProcessMaximum { set; }
        //Functions
        string TeamName { get; }
        void SetController(JiralogController controller);
        void SetInitialControlStates();
        void ClearGrid();
        void EnableControls(bool bEnable);
        void ShowWaiting(bool bShow);
        void AddProjects(List<AvailableProject> projects);
        void AddMembers(List<string> members);
        void AddReleases(List<string> releases);

        //Populate grid one by one 
        void Add_To_Grid_Daily_Worklog(IssueResponse response, JiraID jiraid, int index);
        void Add_To_Grid_Jira_Comments(CommentResponse response, JiraID jiraid, int index,DateTime dt);
        void Add_To_Grid_Daily_Worklog_Defaulters(Member def);
        void Add_To_Grid_Worklog_Range_Defaulters(MembersWorkLog def);
        void Add_To_Grid_No_Work_Assigned(string miss);
        void Add_To_Grid_Date_Range(IssueResponse response, JiraID jiraid, int index, DateTime dt);
        void Add_To_Grid_Team_Performance(List<PerfMemberData> perfMDList, int max_jiras);
        void Add_To_Grid_AssignedWork(ExpandedIssueResponse response, JiraID jiraid, BugTrackData btd);
        void Add_To_Grid_TimeSpent(IssueResponse response, JiraID jiraid);
        void Add_To_Grid_Defect_Leakage(ExpandedIssueResponse response, JiraID jiraid,DefectLeakageData dld);
        void Add_To_Grid_Planned_Completed(ExpandedIssueResponse response, JiraID jiraid, PlannedCompletedData pcd);
    }

    
    public interface IController
    {
        bool LoadView();
        void RequestData();
        void SetViewValuetoController();
        string GetTeamName(string strEmailid);
        string GetSubteamName(string strEmailid);
        string GetProjectCategoryName(string strProjectId);
        string GetTeamCategoryName(string strTeam);
        string GetJiraProfileId(string strEmailid);
    }

    
    public partial class JiralogController : IController
    {
        private const int TOTAL_JIRAIDS_TO_REQUEST = 5000;
        private const int NUM_JIRAIDS_PER_REQUEST = 200;
        IJiralogView _view;
        IJiralogModel _model;
        private string _baseUrl;
        private string _selectedProject;
        private string _selectedProjectId;
        private string _selectedMember;
        private List<string> _membersLoggedWork;
        private List<string> _membersAssignedWork;
        private int _numJirarequest = 0;

        JQLQuery _querylimitforPagination = new JQLQuery("500","0","");

        public JiralogController( IJiralogView jiraLogView, IJiralogModel jiraLogModel)
        {
            _responseList = new List<Jiraresponse>();
            _jiraList = new List<JiraID>();
            _membersLoggedWork = new List<string>();
            _membersAssignedWork = new List<string>();
            _view = jiraLogView;
            _model = jiraLogModel;
            jiraLogView.SetController(this);
        }
        public void SetViewValuetoController()
        {
            _selectedProject = _view.SelectedProject;
            _selectedProjectId = _model.GetProjectId(_selectedProject);
            _selectedMember = _view.SelectedTeammember;
        }
        public bool LoadView()
        {
            if (!_model.LoadConfigData())
            {
                _view.ErrorMessage = "ERROR in loading config file !!!";
                return false;
            }
            _view.SetInitialControlStates();
            //Loaded ? now update the View
            _view.UserId = _model.UserId;
            _view.Passwd = _model.Passwd;
            _view.AddProjects(_model.Projects);
            _baseUrl = _model.BaseUrl;
            return true;
        }
        public void LoadTeammembers()
        {
            List<string> members = new List<string>();
            foreach (Member mem in _model.Members)
            {
                if (String.Compare(mem.team, _view.TeamName, true) == 0 || String.Compare("All Teams", _view.TeamName, true) == 0)
                    members.Add(mem.name);
            }
            _view.AddMembers(members);
        }

        public void LoadReleases()
        {
            List<string> releases = new List<string>();
            AvailableProject proj = new AvailableProject();
            //Selected Project
            foreach (AvailableProject prj in _model.Projects)
            {
                if (String.Compare(prj.name, _view.SelectedProject, true) == 0)
                {
                    proj = prj;
                    break;
                }
            }
            if (proj.releases != null)
            {
                foreach (Release rel in proj.releases)
                {
                    releases.Add(rel.name);
                }
                _view.AddReleases(releases);
            }
        }

        public void RequestData()
        {
            _membersLoggedWork.Clear();
            _membersAssignedWork.Clear();
            _workloglist.Clear();
            _numJirarequest = 0;
            //Get all JIRS IDs to look into 
            _view.Message = "Getting number of Jira Ids to look into";
            _view.ShowWaiting(true);
            GetListofJiraids();
        }
        public void GetListofJiraids()
        {
            _jiraList.Clear();
            
           // for (int i = 0; i <= TOTAL_JIRAIDS_TO_REQUEST; i += NUM_JIRAIDS_PER_REQUEST)
                ThreadedJiralistRequest(_querylimitforPagination);
           
        }

        public void GetDataforAllJiraIds(List<JiraID> jiralist)
        {
            _view.ShowWaiting(false);
            _responseList.Clear();
            _numJiraIds = jiralist.Count;

            _view.ProgressVisibility = true;
            _view.ProcessMaximum = jiralist.Count;
            _jiraCounter = 0;

            foreach (JiraID jiraid in jiralist)
            {
                ThreadedDataRequest(jiraid);
            }
        }
        public string CreateRequestUrl(string queryStringJIRAID)
        {
            string UrlRequest = "";

            if (_view.ReportType == CommonData.ReportType.REPORT_DEFECT_LEAKAGE || _view.ReportType == CommonData.ReportType.REPORT_TEAM_ASSIGNED)
                UrlRequest = _baseUrl + "issue/" + queryStringJIRAID + "?expand=changelog";
            else if(_view.ReportType == CommonData.ReportType.REPORT_JIRA_COMMENTS)
                UrlRequest = _baseUrl + "issue/" + queryStringJIRAID + "/comment";
            else
                UrlRequest = _baseUrl + "issue/" + queryStringJIRAID + "/worklog";
           
            return (UrlRequest);
        }
        private string CreateJQLQueryForJiraIdsToRequest()
        {
            string jqlquery = "";
            string UrlPrefix = "project in ('"+_view.SelectedProject+"') AND issueType in (bug,story,task)";
            DateTime dt = _view.StartDate;
            DateTime dtNow = DateTime.Now;
            string strAssigned = "";

            AvailableProject AvailProj = _model.GetAvailableProject(_view.SelectedProject);
            Release release = _model.GetRelease(_view.SelectedRelease);
            //Epic links
            UrlPrefix += " AND ('Epic Link' in (";
            foreach (string epic in release.epiclinks)
            {
                UrlPrefix += epic +",";
            }
            //Remove "," from last 
            UrlPrefix = UrlPrefix.Remove(UrlPrefix.Length - 1, 1);
            UrlPrefix += ")";
            //Labels
            UrlPrefix += " OR labels in (";
            foreach (string label in release.labels)
            {
                UrlPrefix += label + ",";
            }
            UrlPrefix = UrlPrefix.Remove(UrlPrefix.Length - 1, 1);
            UrlPrefix += ")";
            //Fix versions
            UrlPrefix += " AND fixVersion in (";
            foreach (string fv in release.fixversions)
            {
                UrlPrefix += fv + ",";
            }
            UrlPrefix = UrlPrefix.Remove(UrlPrefix.Length - 1, 1);
            UrlPrefix += "))";
            //***************  Calculation of Assignee  : START *************************************           

            string strAssignedId = "";
            if (string.Compare(_view.SelectedTeammember,"All Team Members") == 0 )
            {
                if (_view.Team == CommonData.Team.TEAM_ALL)
                {
                    foreach (Member member in _model.Members)
                    {
                        strAssignedId += _model.GetJiraProfileId(member.emailid) + ",";
                    }
                    strAssignedId += "EMPTY,";
                }
                else
                    foreach (Member member in _model.Members)
                    {
                        if ((_view.Team == CommonData.Team.TEAM_DOSEIQ_COMMON && string.Compare(member.team, "DoseIQ Common") == 0)
                            || (_view.Team == CommonData.Team.TEAM_DOSEIQ_DEV && string.Compare(member.team, "DoseIQ Dev") == 0)
                            || (_view.Team == CommonData.Team.TEAM_DOSEIQ_QA_MANUAL && string.Compare(member.team, "DoseIQ QA Manual") == 0)
                            || (_view.Team == CommonData.Team.TEAM_DOSEIQ_QA_AUTOMATION && string.Compare(member.team, "DoseIQ QA Automation") == 0))
                            strAssignedId += member.jiraprofileid + ",";
                    }
                //Remove "," from last 
                if(strAssignedId.Length > 0)
                    strAssignedId = strAssignedId.Remove(strAssignedId.Length - 1, 1);
            }
            else
            {
                strAssignedId = _model.GetJiraProfileId(_view.SelectedTeammember);
            }
            if (_view.AssignedOnly)
                strAssigned = " AND assignee in (" + strAssignedId + ")";
            else
                strAssigned = " AND assignee was in (" + strAssignedId + ")";

            //***************  Calculation of Assignee : END  *************************************            

            string strDate = String.Format("{0}-{1}-{2}", dt.Year, dt.Month, dt.Day);
            //string strLastOneMonth = String.Format("{0}-{1}-{2}", dtNow.Month > 2 ? dtNow.Year : dtNow.Year - 1, dtNow.Month > 2 ? dtNow.Month - 2 :(dtNow.Month==2? 12:11), dtNow.Day > 28 ? 28 : dtNow.Day);
            string strLastOneMonth = String.Format("{0}-{1}-{2}", dtNow.Month > 1 ? dtNow.Year : dtNow.Year - 1, dtNow.Month > 1 ? dtNow.Month - 1 : 12, dtNow.Day > 28 ? 28 : dtNow.Day);
            
            switch(_view.ReportType)
            {
                case CommonData.ReportType.REPORT_TEAM_ASSIGNED:
                    if(_view.Team == CommonData.Team.TEAM_DOSEIQ_DEV)
                        jqlquery = UrlPrefix + " AND status in (Review, 'In Progress', Open, New, Reopened,Testing,Blocked) " + strAssigned;
                    else
                        jqlquery = UrlPrefix + " AND status in (Review, 'In Progress', Open, New, Reopened,Testing,Blocked,Done) " + strAssigned;
                    break;
                case CommonData.ReportType.REPORT_JIRA_COMMENTS:
                    /* UrlRequest = _baseUrl + "search?jql=updated >=" + strDate + " AND (status=1 OR status=3 OR status=4 OR status=5 OR status=6 OR status=10017 OR status=10044 OR status=10045 OR status=10047 OR status=10054 OR status=10272)" + strAssigned + "&&startAt=" +
                         startAt + "&&maxResults="+Convert.ToString(NUM_JIRAIDS_PER_REQUEST);*/
                    jqlquery = UrlPrefix+" AND updated >= " + strDate + strAssigned;
                    break;
                case CommonData.ReportType.REPORT_DEFECT_LEAKAGE:
                    jqlquery = UrlPrefix+" AND updated >= " + strDate + strAssigned;
                    break;
                case CommonData.ReportType.REPORT_INDIVIDUAL_PERFORMANCE:
                case CommonData.ReportType.REPORT_TEAM_PERFORMANCE:
                    jqlquery = UrlPrefix + " AND updated >= " + strDate + strAssigned;
                    break;
            }
            return (jqlquery);
        }
        private List<string> GetMissingMembers(List<string> includedmembers)
        {
            List<string> missingmembers = new List<string>();
            List<string> distinctMembers = includedmembers.Distinct().ToList();
            bool bIncluded = false;
            foreach (Member member in _model.Members)
            {
                if (_view.Team == CommonData.Team.TEAM_ALL || String.Compare(member.team, JiralogModel.GetTeamNameFromType(_view.Team), true) == 0)
                {
                    foreach (string strMem in distinctMembers)
                    {
                        bIncluded = false;
                        if (String.Compare(member.name, strMem, true) == 0)
                        {
                            bIncluded = true;
                            break;
                        }
                    }
                    if (!bIncluded)
                        missingmembers.Add(member.name);
                }
            }
            return missingmembers;
        }
        public string GetTeamName(string strEmailid)
        {
            return _model.GetTeamName(strEmailid);
        }
        public string GetSubteamName(string strEmailid)
        {
            return _model.GetSubteamName(strEmailid);
        }
        public string GetProjectCategoryName(string strProjectId)
        {
            return _model.GetProjectName(strProjectId);
        }
        public string GetTeamCategoryName(string strTeam)
        {
            return _model.GetTeamCategoryName(strTeam);
        }
        public string GetJiraProfileId(string strEmailid)
        {
            return _model.GetJiraProfileId(strEmailid);
        }
        private List<MembersWorkLog> GetWorklogRangeDefaulterslist()
        {
            List<MembersWorkLog> memberworklogs = new List<MembersWorkLog>();
            List<DateTime> workingdates = GetWorkingDates();

            foreach (Member member in _model.Members)
            {
                if (_view.Team == CommonData.Team.TEAM_ALL || String.Compare(member.team, JiralogModel.GetTeamNameFromType(_view.Team), true) == 0)
                {
                    //Working date loop
                    foreach (DateTime dt in workingdates)
                    {
                        bool found = false;
                        int hrsmissed = 8;
                        foreach (MembersWorkLog memwklg in _membersworklogs)
                        {
                            if (memwklg.name == member.name && memwklg.date.DayOfYear == dt.DayOfYear)
                            {
                                hrsmissed = hrsmissed - memwklg.hrslogged;
                                found = true;
                                //break;
                            }
                        }
                        if (!found || hrsmissed > 0)
                        {
                            MembersWorkLog mwl = new MembersWorkLog();
                            mwl.date = dt; mwl.name = member.name;
                            mwl.hrslogged = 8-hrsmissed;
                            memberworklogs.Add(mwl);
                        }
                    }
                }
            }
            return memberworklogs;
        }
        private List<DateTime> GetWorkingDates()
        {
            List<DateTime> dates = new List<DateTime>();

            for(DateTime dt=_view.StartDate;dt<=_view.EndDate;dt = dt.AddDays(1))
            {
                if(dt.DayOfWeek != DayOfWeek.Sunday && dt.DayOfWeek != DayOfWeek.Saturday)
                {
                    dates.Add(dt);
                }
            }

            return dates;
        }
        private List<Member> GetWorklogDefaulterslist()
        {
            List<Member> defaulters = new List<Member>();
            List<string> distinctMembers = _membersLoggedWork.Distinct().ToList();

            //Looping through items and subitems
            bool bWorklog = false;
            foreach (Member member in _model.Members)
            {
                if (_view.Team == CommonData.Team.TEAM_ALL || String.Compare(member.team, JiralogModel.GetTeamNameFromType(_view.Team), true) == 0)
                {
                    foreach (string strMem in distinctMembers)
                    {
                        bWorklog = false;
                        if (String.Compare(member.name, strMem, true) == 0)
                        {
                            bWorklog = true;
                            break;
                        }
                    }
                    if (!bWorklog)
                        defaulters.Add(member);
                }
            }
            return defaulters;
        }
    }
}
