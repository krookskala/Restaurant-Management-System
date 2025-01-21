namespace ConsoleApp1.Model
{
    public class Manager : Employee, SerializableObject<Manager>
    {
        public string Level { get; set; }
        public List<Employee> SupervisedEmployees { get; set; } = new List<Employee>();  // Reflexive Association

        // Constructor
        public Manager(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                       int idEmployee, WorkDetails workDetails, string level, DateTime? dateOfLeaving = null)   
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
        {
            if (string.IsNullOrWhiteSpace(level))
                throw new ArgumentException("Level Cannot Be Empty.");


            Level = level;
            InstanceCollection.Add(this);
        }

        // Method To Assign A Table To A Waiter
        public void AssignTableToWaiter(Waiter waiter, Table table)
        {
            if (waiter == null || table == null)
                throw new ArgumentNullException("Waiter And Table Cannot Be Null.");

            waiter.AssignedTables.Add(table);
            Console.WriteLine($"Manager {FirstName} Assigned Table {table.IdTable} To Waiter {waiter.IdWaiter}.");
        }

        // Method To Reassign A Table From One Waiter To Another
        public void ReassignTableToWaiter(Waiter fromWaiter, Waiter toWaiter, Table table)
        {
            if (fromWaiter == null || toWaiter == null || table == null)
                throw new ArgumentNullException("Waiters And Table Cannot Be Null.");

            if (fromWaiter.AssignedTables.Contains(table))
            {
                fromWaiter.AssignedTables.Remove(table);
                toWaiter.AssignedTables.Add(table);
                Console.WriteLine($"Manager {FirstName} Reassigned Table {table.IdTable} From Waiter {fromWaiter.IdWaiter} To Waiter {toWaiter.IdWaiter}.");
            }
            else
            {
                Console.WriteLine($"Table {table.IdTable} Is Not Assigned To Waiter {fromWaiter.IdWaiter}.");
            }
        }

        // Method To Add A Supervised Employee
        public void AddSupervisedEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("Employee Cannot Be Null.");

            SupervisedEmployees.Add(employee);
            Console.WriteLine($"Manager {FirstName} Is Now Supervising {employee.FirstName} {employee.LastName}.");
        }

        // Method To Remove A Supervised Employee
        public void RemoveSupervisedEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("Employee Cannot Be Null.");

            if (SupervisedEmployees.Contains(employee))
            {
                SupervisedEmployees.Remove(employee);
                Console.WriteLine($"Manager {FirstName} Is No Longer Supervising {employee.FirstName} {employee.LastName}.");
            }
            else
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName} Is Not Supervised By {FirstName}.");
            }
        }
    }
}