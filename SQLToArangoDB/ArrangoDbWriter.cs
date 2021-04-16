using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using System.Net;
using Newtonsoft.Json;

namespace SqlToArangoDB
{
    public class ArrangoDbWriter : IDisposable
    {
        public string url { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public IArangoDatabase db { get; set; }
        public ArrangoDbWriter(string url, string Database, string UserName, string Password)
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = Database;
                s.Url = url;
                s.Credential = new NetworkCredential(UserName, Password);
                s.SystemDatabaseCredential = new NetworkCredential(UserName, Password);
            });

            db = ArangoDatabase.CreateWithSetting();
        }


        public void ImportNodes(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {

                if(db.ListCollections().Where(x => x.Name == node.Label).Count() == 0)
                    db.CreateCollection(node.Label, type: CollectionType.Document);
                

                var obj = node.Properties;
                obj.Add("_key", node.ID);
                string json = JsonConvert.SerializeObject(obj);
                db.Collection(node.Label).Insert(obj);
            }
        }

        public void ImportEdges(List<Edge> edges)
        {
            foreach (Edge edge in edges)
            {
                if (db.ListCollections().Where(x => x.Name == edge.Label).Count() == 0)
                    db.CreateCollection(edge.Label, type: CollectionType.Edge);
             
                var obj = edge.Properties;
                obj.Add("_key", edge.ID);
                obj.Add("_from",edge.FromNode + @"/" + edge.FromNodeID);
                obj.Add("_to",edge.ToNode + @"/" + edge.ToNodeID);
                string json = JsonConvert.SerializeObject(obj);
                db.Collection(edge.Label).Insert(obj);

            }
        }

        public void Dispose()
        {
            url = null;
            Database = null;
            UserName = null;
            Password = null;
        }

    }
}