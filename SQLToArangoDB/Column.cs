namespace SqlToArangoDB
{
    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public Column(string name, string type)
        {
            Name = name;
            DataType = type;
        }
    }
}
