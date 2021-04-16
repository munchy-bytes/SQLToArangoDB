using System.Collections.Generic;

namespace SqlToArangoDB
{
    public class Node
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public Node(string id, string label, Dictionary<string, object> properties = null)
        {
            ID = id;
            Label = label;

            if (properties != null)
                Properties = properties;
            else
                Properties = new Dictionary<string, object>();
        }
    }
}