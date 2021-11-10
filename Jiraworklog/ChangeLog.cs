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
    class ExpandedIssue
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
    class ChangeLog
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
    class History
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
    class Item
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

}
