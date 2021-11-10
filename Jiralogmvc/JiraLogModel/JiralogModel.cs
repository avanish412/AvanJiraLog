using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ComponentModel;

namespace JiralogMVC.Controller
{
    public struct JiraID
    {
        public string strProject;
        public string createTime;
        public string createdBy;
        public string strIssuetype;
        public string strPriority;
        public string strJiraID;
        public string strStatus;
        public string strAssignee;
        public string strAssigneeEmailid;
        public string strSummary;
        public string duedate;
        public string fixfor;
        public string sprint;
        public Nullable<long> aggtime;
        public Nullable<long> originalestimate;
    };
    public struct Jiraresponse
    {
        public JiraID jiraid;
        public object response;
    };
    public interface IJiralogModel
    {
        bool LoadConfigData();

        List<AvailableProject> Projects { get; set; }
        List<Member> Members { get; set; }
        List<Release> Releases { get; set; }

        string UserId { get; set; }
        string Passwd { get; set; }

        string BaseUrl { get; }
        //Common functions
        string GetTeamName(string strEmailid);
        string GetSubteamName(string strEmailid);
        string GetEmailid(string strName);
        string GetProjectName(string strProjectId);
        string GetProjectId(string strProjectName);
        AvailableProject GetAvailableProject(string strProjectName);
        Release GetRelease(string release);
        string GetTeamCategoryName(string strTeam);
        string GetJiraProfileId(string strEmailid);
    };
    
    public class JiralogModel : IJiralogModel
    {
        const string configFile = "\\jira.conf";
        string _userid;
        string _passwd;
        string _baseurl;
        List<AvailableProject> _projects;
        List<Release> _releases;
        List<Member> _members;
        List<Team> _teams;   

        //Properties
        public string UserId 
        {
            get { return _userid; }
            set { _userid = value; }
        }
        public string Passwd
        {
            get { return _passwd; }
            set { _passwd = value; }
        }
        public string BaseUrl
        {
            get { return _baseurl; }
  
        }
        public List<AvailableProject> Projects
        {
            get { return _projects; }
            set { _projects = value; }
        }
        public List<Release> Releases
        {
            get { return _releases; }
            set { _releases = value; }
        }
        public List<Member> Members
        {
            get { return _members; }
            set { _members = value; }
        }
        public List<Team> Teams
        {
            get { return _teams; }
            set { _teams = value; }
        }

        public bool LoadConfigData()
        {
            //File path 
            string configfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + configFile;

            _members = new List<Member>();
            _projects = new List<AvailableProject>();
            _teams = new List<Team>();

            FileStream fileStream = new FileStream(configfile, FileMode.Open, FileAccess.ReadWrite);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(JiraConfig));
            object objResponse = jsonSerializer.ReadObject(fileStream);

            JiraConfig jsonResponse = objResponse as JiraConfig;

            if (jsonResponse == null)
                return false;

            _userid = jsonResponse.credential.userid;
            _passwd = jsonResponse.credential.password;
            _baseurl = jsonResponse.baseurl;

            for (int i = 0; i < jsonResponse.members.Length; i++)
            {
                Member m = new Member();
                m.name = jsonResponse.members[i].name;
                m.emailid = jsonResponse.members[i].emailid;
                m.team = jsonResponse.members[i].team;
                m.subteam = jsonResponse.members[i].subteam;
                m.jiraprofileid = jsonResponse.members[i].jiraprofileid;
                _members.Add(m);
            }
            for (int i = 0; i < jsonResponse.projects.Length; i++)
            {
                _projects.Add(jsonResponse.projects[i]);
            }
            for (int i = 0; i < jsonResponse.teams.Length; i++)
            {
                _teams.Add(jsonResponse.teams[i]);
            }
            
            return true;
        }
        public string GetTeamName(string strEmailid)
        {
            foreach (Member member in _members)
            {
                if (member.emailid.CompareTo(strEmailid) == 0 || member.name.CompareTo(strEmailid) == 0)
                    return member.team;
            }
            return "";
        }
        public string GetSubteamName(string strEmailid)
        {
            foreach (Member member in _members)
            {
                if (member.emailid.CompareTo(strEmailid) == 0 || member.name.CompareTo(strEmailid) == 0)
                    return member.subteam;
            }
            return "Individual";
        }
        public string GetTeamCategoryName(string teamname)
        {
            foreach (Team team in _teams)
            {
                if (team.name.CompareTo(teamname) == 0)
                    return team.category;
            }
            return "Not Defined";
        }
        public string GetProjectName(string strProjectId)
        {
            foreach (AvailableProject project in _projects)
            {
                if (project.id.CompareTo(strProjectId) == 0)
                    return project.name;
            }
            return "Not Defined";
        }
        public string GetProjectId(string strProjectName)
        {
            foreach (AvailableProject project in _projects)
            {
                if (project.name.CompareTo(strProjectName) == 0)
                    return project.id;
            }
            return "Not Defined";
        }
        public  AvailableProject GetAvailableProject(string strProjectName)
        {
            foreach (AvailableProject project in _projects)
            {
                if (project.name.CompareTo(strProjectName) == 0)
                    return project;
            }
            return null;
        }
        public Release GetRelease(string releasename)
        {
            foreach (AvailableProject project in _projects)
            {
                foreach(Release rel in project.releases)
                {
                    if (rel.name.CompareTo(releasename) == 0)
                        return rel;
                }
            }
            return null;
        }
        public string GetEmailid(string strName)
        {
            foreach (Member member in _members)
            {
                if (member.name.CompareTo(strName) == 0)
                    return member.emailid;
            }
            return "";
        }
        public string GetJiraProfileId(string strEmailid)
        {
            foreach (Member member in _members)
            {
                if (member.emailid.CompareTo(strEmailid) == 0 || member.name.CompareTo(strEmailid) == 0)
                    return member.jiraprofileid;
            }
            return "";
        }
        public static string GetTeamNameFromType(CommonData.Team team)
        {
            switch(team)
            {
                case CommonData.Team.TEAM_DOSEIQ_COMMON:
                    return "DoseIQ Common";
                case CommonData.Team.TEAM_DOSEIQ_DEV:
                    return "DoseIQ Dev";
                case CommonData.Team.TEAM_DOSEIQ_QA_MANUAL:
                    return "DoseIQ QA Manual";
                case CommonData.Team.TEAM_DOSEIQ_QA_AUTOMATION:
                    return "DoseIQ QA Automation";
                case CommonData.Team.TEAM_ALL:
                    return "ALL"; 
                default:
                    return "";
            }
        }
    }
}
