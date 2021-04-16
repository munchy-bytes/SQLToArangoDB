using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace SqlToArangoDB
{
    public class SQLReader : IDisposable
    {
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }

        public string ConnectionString { get; set; }

        public SQLReader(string connection)
        {
            ConnectionString = connection;
        }
        public void GetNodes()
        {
            Nodes = new List<Node>();
            //initialize connection
            using (SqlConnection sqlcon = new SqlConnection(ConnectionString))
            {
                sqlcon.Open();
                //retrieving nodes tables
                using (SqlCommand sqlnodes = new SqlCommand("select name from sys.tables where is_node = 1", sqlcon))
                {
                    SqlDataReader tablesreader;
                    tablesreader = sqlnodes.ExecuteReader();

                    List<string> tables = new List<string>();

                    //get nodes tables
                    while (tablesreader.Read())
                        tables.Add(tablesreader[0].ToString());

                    tablesreader.Close();
                    foreach (string tablename in tables)
                    {
                        //Get columns with data types
                        string cmdColumns = String.Format("select column_name,data_type from information_schema.columns columns where table_name = '{0}'" +
                                                                                "and exists(select 1 from information_schema.columns temp where temp.column_name like 'graph_id_%'" +
                                                                                "and temp.TABLE_SCHEMA = columns.table_schema and temp.TABLE_NAME = columns.TABLE_NAME)", tablename);
                        List<Column> columns = new List<Column>();
                        using (SqlCommand sqlcmd = new SqlCommand(cmdColumns, sqlcon))
                        {
                            SqlDataReader columnsreader = sqlcmd.ExecuteReader();
                            while (columnsreader.Read())
                            {
                                columns.Add(new Column(columnsreader[0].ToString(), columnsreader[1].ToString()));
                            }
                            columnsreader.Close();
                        }

                        string idcolumn = columns.Where(x => x.Name.StartsWith("$node_id")).FirstOrDefault().Name;
                        string propColumns = string.Join(",", columns.Select(x => x.Name).Where(y => !y.StartsWith("$") && !y.StartsWith("graph_id_")));
                        string cmdNodes = "select JSON_VALUE([" + idcolumn + "],'$.id') as node_id "
                                        + (propColumns == "" ? "" : "," + propColumns);
                        cmdNodes = cmdNodes + string.Format(" from {0}", tablename);
                        //get nodes
                        using (SqlCommand sqlcmd = new SqlCommand(cmdNodes, sqlcon))
                        {
                            SqlDataReader nodesreader = sqlcmd.ExecuteReader();
                            //Get properties

                            while (nodesreader.Read())
                            {
                                Dictionary<string, object> properties = new Dictionary<string, object>();
                                foreach (Column col in columns.Where(x => !x.Name.StartsWith("$") && !x.Name.StartsWith("graph_id_")))
                                {
                                    properties.Add(col.Name, nodesreader[col.Name]);
                                }
                                properties.Add("node_id", nodesreader["node_id"].ToString());
                                Nodes.Add(new Node(nodesreader["node_id"].ToString(), tablename, properties));
                            }
                            nodesreader.Close();
                        }
                    }
                }
            }
        }
        public void GetEdges()
        {
            Edges = new List<Edge>();
            //initialize connection
            using (SqlConnection sqlcon = new SqlConnection(ConnectionString))
            {
                sqlcon.Open();
                //retrieving nodes tables
                using (SqlCommand sqlnodes = new SqlCommand("select name from sys.tables where is_edge = 1", sqlcon))
                {
                    SqlDataReader tablesreader;
                    tablesreader = sqlnodes.ExecuteReader();

                    List<string> tables = new List<string>();

                    //get edges tables
                    while (tablesreader.Read())
                        tables.Add(tablesreader[0].ToString());

                    tablesreader.Close();
                    foreach (string tablename in tables)
                    {
                        //Get columns with data types
                        string cmdColumns = String.Format("select column_name,data_type from information_schema.columns columns where table_name = '{0}'" +
                                                                                "and exists(select 1 from information_schema.columns temp where temp.column_name like 'graph_id_%'" +
                                                                                "and temp.TABLE_SCHEMA = columns.table_schema and temp.TABLE_NAME = columns.TABLE_NAME)", tablename);
                        List<Column> columns = new List<Column>();
                        using (SqlCommand sqlcmd = new SqlCommand(cmdColumns, sqlcon))
                        {
                            SqlDataReader columnsreader = sqlcmd.ExecuteReader();
                            while (columnsreader.Read())
                            {
                                columns.Add(new Column(columnsreader[0].ToString(), columnsreader[1].ToString()));
                            }
                            columnsreader.Close();
                        }

                        string idcolumn = columns.Where(x => x.Name.StartsWith("$edge_id")).FirstOrDefault().Name;
                        string fromid = columns.Where(x => x.Name.StartsWith("$from_id")).FirstOrDefault().Name;
                        string toid = columns.Where(x => x.Name.StartsWith("$to_id")).FirstOrDefault().Name;
                        string propColumns = string.Join(",", columns.Select(x => x.Name).Where(y => !y.StartsWith("$") && !y.StartsWith("graph_id_") && !y.StartsWith("from_") && !y.StartsWith("to_")));
                        string cmdNodes = "select JSON_VALUE([" + idcolumn + "],'$.id') as edge_id " +
                            ",JSON_VALUE([" + fromid + "],'$.id') as from_id " +
                            ",JSON_VALUE([" + fromid + "],'$.table') as from_table " +
                            ",JSON_VALUE([" + toid + "],'$.id') as to_id " +
                            ",JSON_VALUE([" + toid + "],'$.table') as to_table" +
                            (propColumns == "" ? "" : "," + propColumns);
                        cmdNodes = cmdNodes + string.Format(" from {0}", tablename);

                        using (SqlCommand sqlcmd = new SqlCommand(cmdNodes, sqlcon))
                        {
                            SqlDataReader edgesreader = sqlcmd.ExecuteReader();
                            //Get properties

                            while (edgesreader.Read())
                            {
                                Dictionary<string, object> properties = new Dictionary<string, object>();
                                foreach (Column col in columns.Where(y => !y.Name.StartsWith("$") && !y.Name.StartsWith("graph_id_") && !y.Name.StartsWith("from_") && !y.Name.StartsWith("to_")))
                                {
                                    properties.Add(col.Name, edgesreader[col.Name]);
                                }
                                Edges.Add(new Edge(edgesreader["edge_id"].ToString(), tablename,
                                    edgesreader["from_table"].ToString(), edgesreader["from_id"].ToString(),
                                    edgesreader["to_table"].ToString(), edgesreader["to_id"].ToString(), properties));
                            }
                            edgesreader.Close();
                        }

                    }
                }
            }

        }
        public void Dispose()
        {
            Nodes = null;
            Edges = null;
            ConnectionString = null;
        }
    }
}