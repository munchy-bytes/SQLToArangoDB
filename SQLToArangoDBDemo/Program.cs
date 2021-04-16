using System;
using SqlToArangoDB;
namespace SQLToArangoDBDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (SQLReader reader = new SQLReader("Server=DESKTOP-KCL006K\\DATASERVER;Database=GraphPL;Trusted_Connection=yes;"))
            {
                reader.GetNodes();
                reader.GetEdges();

                using (ArrangoDbWriter writer = new ArrangoDbWriter("http://localhost:8529", "TestGraph", "root", "123"))
                {
                    writer.ImportNodes(reader.Nodes);
                    writer.ImportEdges(reader.Edges);
                }

            }

        }
    }
}
