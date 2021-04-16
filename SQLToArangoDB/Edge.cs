using System.Collections.Generic;

namespace SqlToArangoDB
{
    public class Edge
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public string FromNode { get; set; }
        public string FromNodeID { get; set; }
        public string ToNode { get; set; }
        public string ToNodeID { get; set; }
        public Edge(string id, string label, string from, string fromID, string to, string toID, Dictionary<string, object> properties = null)
        {
            ID = id;
            Label = label;
            FromNode = from;
            FromNodeID = fromID;
            ToNodeID = toID;
            ToNode = to;
            if (properties != null)
                Properties = properties;
            else
                Properties = new Dictionary<string, object>();

        }

    }
}