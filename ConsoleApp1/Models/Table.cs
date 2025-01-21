namespace ConsoleApp1.Models
{
    public class Table : SerializableObject<Table>
    {
        public int IdTable { get; set; }
        public int NumberOfChairs { get; set; }
        public string TableType { get; set; }

        // Constructor
        public Table(int idTable, int numberOfChairs, string tableType)
        {
            if (idTable <= 0)
                throw new ArgumentException("Table ID Must Be Greater Than Zero.");
            
            if (numberOfChairs <= 0)
                throw new ArgumentException("Number Of Chairs Must Be Greater Than Zero.");

            if (string.IsNullOrWhiteSpace(tableType))
                throw new ArgumentException("Table Type Cannot Be Empty.");


            IdTable = idTable;
            NumberOfChairs = numberOfChairs;
            TableType = tableType;
            InstanceCollection.Add(this);
        }
    }
}