# SQLToArangoDB
A class library used to migrate SQL Server graph database into ArangoDB.

# Published Articles

- Migrating SQL Server graph database to ArangoDB (Under review)

# Dependencies

- [ArangoDB.CLient](https://www.nuget.org/packages/ArangoDB.Client/)

## Example
```csharp
using System;

namespace SQLToArangoDB
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (SQLReader reader = new SQLReader("Server=<machine name>\\<instance>;Database=<database name>;Trusted_Connection=yes;"))
            {
                reader.GetNodes();
                reader.GetEdges();

                using (ArrangoDbWriter writer = new ArrangoDbWriter("http://localhost:8529", "<database name>", "<user>", "<password>"))
                {
                    writer.ImportNodes(reader.Nodes);
                    writer.ImportEdges(reader.Edges);
                }

            }

        }
    }
}

```
