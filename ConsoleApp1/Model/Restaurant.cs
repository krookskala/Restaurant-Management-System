namespace ConsoleApp1.Model
{
    public class Restaurant : SerializableObject<Restaurant>
    {
        public string Name { get; set; }    
        public int MaxCapacity { get; set; }    
        public List<Table> Tables { get; set; } = new List<Table>();  // Aggregation with Table

        // Constructor
        public Restaurant(string name, int maxCapacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant Name Cannot Be Empty.");
            if (maxCapacity <= 0)
                throw new ArgumentException("Max Capacity Must Be Greater Than Zero.");

            Name = name;
            MaxCapacity = maxCapacity;
            InstanceCollection.Add(this);
        }

        // Method To Get The Number Of Tables
        public int GetNumberOfTables()
        {
            return Tables.Count;
        }

        // Method To Add A Table
        public void AddTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException("Table Cannot Be Null.");

            int currentCapacity = GetCurrentCapacity();
            if (currentCapacity + table.Capacity > MaxCapacity)
                throw new InvalidOperationException("Adding This Table Exceeds The Restaurant's Maximum Capactiy.");
            
            Tables.Add(table);
            Console.WriteLine($"Table {table.IdTable} Added To {Name}.");
        }

        // Method To Remove A Table
        public void RemoveTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException("Table Cannot Be Null.");

            if (Tables.Contains(table))
            {
                Tables.Remove(table);
                Console.WriteLine($"Table {table.IdTable} Removed From {Name}.");
            }
            else
            {
                Console.WriteLine($"Table {table.IdTable} Does Not Exist In {Name}.");
            }
        }       

        // Method To Get The Current Capacity
        public int GetCurrentCapacity()
        {
            int totalChairs = 0;
            foreach (var table in Tables)
            {
                totalChairs += table.NumberOfChairs;
            }
            return totalChairs;
        } 
    }
}