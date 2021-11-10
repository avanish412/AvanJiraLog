using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Jiraworklog
{
    [DataContract]
    public class Response
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
    public class Users
    {
        [DataMember(Name = "credential")]
        public Credential credential { get; set; }
        [DataMember(Name = "symphonyusers")]
        public User[] symphonyusers { get; set; }
        [DataMember(Name = "projects")]
        public string[] projects { get; set; }
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
    public class User
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "team")]
        public string team { get; set; }

        [DataMember(Name = "emailid")]
        public string emailid { get; set; }
    }
}