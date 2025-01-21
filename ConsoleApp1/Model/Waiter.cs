namespace ConsoleApp1.Model
{
    public class Waiter : Employee, SerializableObject<Waiter>
    {
        public int IdWaiter { get; set; }
        public List<Table> AssignedTables { get; set; } = new List<Table>();

        // Constructor
        public Waiter(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                      int idEmployee, WorkDetails workDetails, int idWaiter, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
        {
            IdWaiter = idWaiter;
            InstanceCollection.Add(this);
        }

        // Method
        public void ServeTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException("Table Cannot Be Null.");


            Console.WriteLine($"Waiter {FirstName} Is Serving Table {table.IdTable}.");
        }
    }
}
