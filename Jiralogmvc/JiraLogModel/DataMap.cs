using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace JiralogMVC.Controller
{
    // Jira ID desc
    //http://192.168.16.35/rest/api/2/issues/OSII-2709/worklog
    // http://192.168.16.35/rest/api/2/issues/OSII-2511/comment    for comments
    /*-------------------------------JSON DATA ---------------------------------------*/


    [DataContract]
    public class IssueResponse
    {
        [DataMember(Name = "maxResults")]
        public int maxResults { get; set; }
        [DataMember(Name = "startAt")]
        public int startAt { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
        [DataMember(Name = "worklogs")]
        public WorkLog[] workLogs { get; set; }
    }

    [DataContract]
    public class WorkLog
    {
        [DataMember(Name = "author")]
        public Author author { get; set; }
        [DataMember(Name = "comment")]
        public string comment { get; set; }
        [DataMember(Name = "created")]
        public string created { get; set; }
        [DataMember(Name = "id")]
        public string id { get; set; }
        [DataMember(Name = "self")]
        public string self { get; set; }
        [DataMember(Name = "started")]
        public string started { get; set; }
        [DataMember(Name = "timeSpent")]
        public string timeSpent { get; set; }
        [DataMember(Name = "timeSpentSeconds")]
        public long timeSpentSeconds { get; set; }
        [DataMember(Name = "updateAuthor")]
        public Author updateAuthor { get; set; }
        [DataMember(Name = "updated")]
        public string updated { get; set; }
    }


    [DataContract]
    public class Author
    {
        [DataMember(Name = "active")]
        public bool active { get; set; }
        [DataMember(Name = "avatarUrls")]
        public AvatarUrl[] avatarUrls { get; set; }
        [DataMember(Name = "displayName")]
        public string displayName { get; set; }
        [DataMember(Name = "emailAddress")]
        public string emailAddress { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "self")]
        public string self { get; set; }
    }

    [DataContract]
    public class AvatarUrl
    {
        [DataMember(Name = "16x16")]
        public string str16x16 { get; set; }
        [DataMember(Name = "48x48")]
        public string str48x48 { get; set; }
    }

    [DataContract]
    public class JiraConfig
    {
        [DataMember(Name = "credential")]
        public Credential credential { get; set; }
        [DataMember(Name = "baseurl")]
        public string baseurl { get; set; }
        [DataMember(Name = "members")]
        public Member[] members { get; set; }
        [DataMember(Name = "teams")]
        public Team[] teams { get; set; } 
        [DataMember(Name = "subteams")]
        public string[] subteams { get; set; }
        [DataMember(Name = "projects")]
        public AvailableProject[] projects { get; set; }
    }
    [DataContract]
    public class Credential
    {
        [DataMember(Name = "userid")]
        public string userid { get; set; }
        [DataMember(Name = "password")]
        public string password { get; set; }
    }

    [DataContract]
    public class Team
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "category")]
        public string category { get; set; }
    }

    [DataContract]
    public class AvailableProject
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "releases")]
        public Release[] releases { get; set; }
    }
    [DataContract]
    public class Release
    {
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "labels")]
        public string[] labels { get; set; }
        [DataMember(Name = "epiclinks")]
        public string[] epiclinks { get; set; }
        [DataMember(Name = "fixversions")]
        public string[] fixversions { get; set; }
    }

    [DataContract]
    public class Member
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "team")]
        public string team { get; set; }

        [DataMember(Name = "subteam")]
        public string subteam { get; set; }

        [DataMember(Name = "emailid")]
        public string emailid { get; set; }

        [DataMember(Name = "jiraprofileid")]
        public string jiraprofileid { get; set; }
    }

    /*----------------------JIRA DATA ---------------------------------*/
    [DataContract]
    public class IssueData
    {
        [DataMember(Name = "expand")]
        public string expand { get; set; }
        [DataMember(Name = "issues")]
        public Issue[] issues { get; set; }
        [DataMember(Name = "maxResults")]
        public int maxResults { get; set; }
        [DataMember(Name = "startAt")]
        public int startAt { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }

    [DataContract]
    public class Issue
    {
        [DataMember(Name = "expand")]
        public string expand { get; set; }
        [DataMember(Name = "fields")]
        public Field fields { get; set; }
        [DataMember(Name = "id")]
        public string id { get; set; }
        [DataMember(Name = "key")]
        public string key { get; set; }
        [DataMember(Name = "self")]
        public string self { get; set; }
    }


    [DataContract]
    public class Field
    {
        /*[DataMember(Name = "aggregateprogress")]
        public Progress aggregateprogress { get; set; }*/
        [DataMember(Name = "aggregatetimeestimate")]
        public Nullable<long> aggregatetimeestimate { get; set; }
        //[DataMember(Name = "aggregatetimeoriginalestimate")]
        //public long aggregatetimeoriginalestimate { get; set; }
        //[DataMember(Name = "aggregatetimespent")]
        //public long aggregatetimespent { get; set; }
        [DataMember(Name = "assignee")]
        public Author assignee { get; set; }
        [DataMember(Name = "reporter")]
        public Author reporter { get; set; }
        /*[DataMember(Name = "components")]
        public Component[] components { get; set; }*/
        [DataMember(Name = "created")]
        public string created { get; set; }
        [DataMember(Name = "priority")]
        public Priority priority { get; set; }
        [DataMember(Name = "customfield_18385")]
        public FixFor fixfor { get; set; }
        [DataMember(Name = "customfield_18089")]
        public string[] sprints { get; set; }
        /*[DataMember(Name = "customfield_10000")]
        public string customfield_10000 { get; set; }
        [DataMember(Name = "customfield_10012")]
        public string customfield_10012 { get; set; }
        [DataMember(Name = "customfield_10013")]
        public string customfield_10013 { get; set; }
        [DataMember(Name = "customfield_10014")]
        public string customfield_10014 { get; set; }
        [DataMember(Name = "customfield_10020")]
        public Author[] customfield_10020 { get; set; }
        [DataMember(Name = "customfield_10030")]
        public string customfield_10030 { get; set; }
        [DataMember(Name = "customfield_10031")]
        public string customfield_10031 { get; set; }
        [DataMember(Name = "customfield_10032")]
        public string customfield_10032 { get; set; }
        [DataMember(Name = "customfield_17834")]
        public string customfield_17834 { get; set; }
        [DataMember(Name = "customfield_17835")]
        public string customfield_17835 { get; set; }
        [DataMember(Name = "customfield_17854")]
        public string customfield_17854 { get; set; }
        [DataMember(Name = "customfield_17894")]
        public string customfield_17894 { get; set; }
        [DataMember(Name = "customfield_17915")]
        public string customfield_17915 { get; set; }
        [DataMember(Name = "customfield_18015")]
        public string customfield_18015 { get; set; }
        [DataMember(Name = "customfield_18016")]
        public string customfield_18016 { get; set; }
        [DataMember(Name = "customfield_18018")]
        public string customfield_18018 { get; set; }
        [DataMember(Name = "customfield_18034")]
        public string customfield_18034 { get; set; }
        [DataMember(Name = "customfield_18048")]
        public string customfield_18048 { get; set; }
        [DataMember(Name = "customfield_18074")]
        public string customfield_18074 { get; set; }
        [DataMember(Name = "customfield_18084")]
        public string customfield_18084 { get; set; }
        [DataMember(Name = "customfield_18085")]
        public string customfield_18085 { get; set; }
        [DataMember(Name = "customfield_18088")]
        public string customfield_18088 { get; set; }
        [DataMember(Name = "customfield_18089")]
        public string customfield_18089 { get; set; }
        [DataMember(Name = "customfield_18090")]
        public string customfield_18090 { get; set; }
        [DataMember(Name = "customfield_18184")]
        public string customfield_18184 { get; set; }
        [DataMember(Name = "customfield_18185")]
        public string customfield_18185 { get; set; }
        [DataMember(Name = "customfield_18186")]
        public string customfield_18186 { get; set; }
        [DataMember(Name = "customfield_18187")]
        public string customfield_18187 { get; set; }
        [DataMember(Name = "description")]
        public string description { get; set; }*/
        [DataMember(Name = "duedate")]
        public string duedate { get; set; }
        /*[DataMember(Name = "fixVersions")]
        public string[] fixVersions { get; set; }
        [DataMember(Name = "issuelinks")]
        public string[] issuelinks { get; set; }*/
        [DataMember(Name = "issuetype")]
        public IssueType issuetype { get; set; }
        /*[DataMember(Name = "labels")]
        public string[] labels { get; set; }
        [DataMember(Name = "lastViewed")]
        public string lastViewed { get; set; }
        [DataMember(Name = "progress")]
        public Progress progress { get; set; }
         */
        [DataMember(Name = "project")]
        public Project project { get; set; }

        /*[DataMember(Name = "reporter")]
        public Author reporter { get; set; }
        [DataMember(Name = "resolution")]
        public Resolution resolution { get; set; }
        [DataMember(Name = "resolutiondate")]
        public string resolutiondate { get; set; }*/
        [DataMember(Name = "status")]
        public Status status { get; set; }
        /*[DataMember(Name = "subtasks")]
        public SubTask[] subtasks { get; set; }*/
        [DataMember(Name = "summary")]
        public string summary { get; set; }
        [DataMember(Name = "timeestimate")]
        public Nullable<long> timeestimate { get; set; }
        [DataMember(Name = "timeoriginalestimate")]
        public Nullable<long> timeoriginalestimate { get; set; }
        [DataMember(Name = "timespent")]
        public Nullable<long> timespent { get; set; }
        [DataMember(Name = "updated")]
        public string updated { get; set; }
        /*[DataMember(Name = "votes")]
        public Vote[] votes { get; set; }
        [DataMember(Name = "watches")]
        public Watch[] watches { get; set; }
        [DataMember(Name = "workratio")]
        public int workratio { get; set; }*/
    }

    [DataContract]
    public class Project
    {
        [DataMember(Name = "id")]
        public string id { get; set; }
        [DataMember(Name = "key")]
        public string key { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
    }

    [DataContract]
    public class Progress
    {
        [DataMember(Name = "progress")]
        public int progress { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }

    [DataContract]
    public class Component
    {
        [DataMember(Name = "progress")]
        public int progress { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }
    [DataContract]
    public class Resolution
    {
        [DataMember(Name = "progress")]
        public int progress { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }
    [DataContract]
    public class Status
    {
        [DataMember(Name = "iconUrl")]
        public string progress { get; set; }
        [DataMember(Name = "id")]
        public string total { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "self")]
        public string self { get; set; }
    }
    [DataContract]
    public class SubTask
    {
        [DataMember(Name = "progress")]
        public int progress { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }
    [DataContract]
    public class Vote
    {
        [DataMember(Name = "progress")]
        public int progress { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }
    [DataContract]
    public class Watch
    {
        [DataMember(Name = "progress")]
        public int progress { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
    }

    [DataContract]
    public class IssueType
    {
        [DataMember(Name = "self")]
        public string self { get; set; }
        [DataMember(Name = "id")]
        public string id { get; set; }
        [DataMember(Name = "description")]
        public string description { get; set; }
        [DataMember(Name = "iconUrl")]
        public string iconUrl { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "subtask")]
        public bool subtask { get; set; }
    }

    [DataContract]
    public class Priority
    {
        [DataMember(Name = "self")]
        public string self { get; set; }
        [DataMember(Name = "iconurl")]
        public string iconurl { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "id")]
        public string id { get; set; }
    }
    [DataContract]
    public class FixFor
    {
        [DataMember(Name = "self")]
        public string self { get; set; }
        [DataMember(Name = "id")]
        public string id { get; set; }
        [DataMember(Name = "name")]
        public string name { get; set; }
        [DataMember(Name = "archived")]
        public bool archived { get; set; }
        [DataMember(Name = "released")]
        public bool released { get; set; }


    }
    /*-------------------------------LOG DATA -----------------------------------------*/
    [DataContract]
    public class ExpandedIssueResponse
    {
        [DataMember(Name = "expand")]
        public string expand { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "self")]
        public string self { get; set; }

        [DataMember(Name = "key")]
        public string key { get; set; }

        [DataMember(Name = "fields")]
        public Field[] fields { get; set; }

        [DataMember(Name = "changelog")]
        public ChangeLog changelog { get; set; }
    }

    [DataContract]
    public class ChangeLog
    {
        [DataMember(Name = "startAt")]
        public int startAt { get; set; }

        [DataMember(Name = "maxResults")]
        public int maxResults { get; set; }

        [DataMember(Name = "total")]
        public int total { get; set; }

        [DataMember(Name = "histories")]
        public History[] histories { get; set; }

    }

    [DataContract]
    public class History
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "author")]
        public Author author { get; set; }

        [DataMember(Name = "created")]
        public string created { get; set; }

        [DataMember(Name = "items")]
        public Item[] items { get; set; }
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "field")]
        public string field { get; set; }

        [DataMember(Name = "fieldtype")]
        public string fieldtype { get; set; }

        [DataMember(Name = "from")]
        public string from { get; set; }

        [DataMember(Name = "fromString")]
        public string fromString { get; set; }

        [DataMember(Name = "to")]
        public string to { get; set; }

        [DataMember(Name = "toString")]
        public string toString { get; set; }
    }

    /*----------------------------COMMENT DATA --------------------------------------*/
    [DataContract]
    public class CommentResponse
    {
        [DataMember(Name = "maxResults")]
        public int maxResults { get; set; }
        [DataMember(Name = "startAt")]
        public int startAt { get; set; }
        [DataMember(Name = "total")]
        public int total { get; set; }
        [DataMember(Name = "comments")]
        public Comment[] comments { get; set; }
    }
    [DataContract]
    public class Comment
    {
        [DataMember(Name = "author")]
        public Author author { get; set; }
        [DataMember(Name = "body")]
        public string body { get; set; }
        [DataMember(Name = "created")]
        public string created { get; set; }
    }
}
