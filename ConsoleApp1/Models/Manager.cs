using ConsoleApp1.Services;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Models
{
    public class Manager : SerializableObject<Manager>
    {
        [Required(ErrorMessage = "Manager ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Manager ID must be a positive integer.")]
        public int IdManager { get; private set; } 

        [Required(ErrorMessage = "Level is required.")]
        public string Level { get; private set; } 

        // Reflexive Association
        public List<Employee> SupervisedEmployees { get; private set; } = new();

        // Derived Attribute
        public int SupervisedEmployeeCount => SupervisedEmployees.Count;

        // Events for Supervised Employee Changes
        public event Action<string>? OnSupervisedEmployeeAdded;
        public event Action<string>? OnSupervisedEmployeeRemoved;

        // Structured Logger Placeholder
        private readonly Action<string> _logger;

        // Constructor
        public Manager(int idManager, string level, Action<string> logger = null!)
        {
            if (idManager <= 0)
                throw new ArgumentOutOfRangeException(nameof(idManager), "Manager ID must be a positive integer.");

            if (string.IsNullOrWhiteSpace(level))
                throw new ArgumentException("Level cannot be null or empty.", nameof(level));

            IdManager = idManager;
            Level = level;
            _logger = logger ?? Console.WriteLine;
        }

        // Assign a Table to a Waiter
        public void AssignTableToWaiter(Waiter waiter, Table table)
        {
            if (waiter == null)
                throw new ArgumentNullException(nameof(waiter), "Waiter cannot be null.");

            if (table == null)
                throw new ArgumentNullException(nameof(table), "Table cannot be null.");

            waiter.AssignTable(table);
            LogAction($"Manager {IdManager} assigned Table {table.IdTable} to Waiter {waiter.IdWaiter}.");
        }

        // Reassign a Table from one Waiter to another
        public void ReassignTableToWaiter(Waiter fromWaiter, Waiter toWaiter, Table table)
        {
            if (fromWaiter == null)
                throw new ArgumentNullException(nameof(fromWaiter), "Source Waiter cannot be null.");

            if (toWaiter == null)
                throw new ArgumentNullException(nameof(toWaiter), "Target Waiter cannot be null.");

            if (table == null)
                throw new ArgumentNullException(nameof(table), "Table cannot be null.");

            if (!fromWaiter.UnassignTable(table.IdTable))
            {
                LogAction($"Table {table.IdTable} is not assigned to Waiter {fromWaiter.IdWaiter}.");
                return;
            }

            toWaiter.AssignTable(table);
            LogAction($"Manager {IdManager} reassigned Table {table.IdTable} from Waiter {fromWaiter.IdWaiter} to Waiter {toWaiter.IdWaiter}.");
        }

        // Add a Supervised Employee
        public void AddSupervisedEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");

            if (SupervisedEmployees.Contains(employee))
            {
                LogAction($"Employee {employee.IdEmployee} is already supervised by Manager {IdManager}.");
                return;
            }

            employee.SetSupervisor(this);
            SupervisedEmployees.Add(employee);
            OnSupervisedEmployeeAdded?.Invoke($"Employee {employee.IdEmployee} added under Manager {IdManager}.");
            LogAction($"Manager {IdManager} is now supervising Employee {employee.IdEmployee}.");
        }

        // Remove a Supervised Employee
        public void RemoveSupervisedEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");

            if (!SupervisedEmployees.Remove(employee))
            {
                LogAction($"Employee {employee.IdEmployee} is not supervised by Manager {IdManager}.");
                return;
            }

            employee.RemoveSupervisor();
            OnSupervisedEmployeeRemoved?.Invoke($"Employee {employee.IdEmployee} removed from Manager {IdManager}.");
            LogAction($"Manager {IdManager} is no longer supervising Employee {employee.IdEmployee}.");
        }

        private void LogAction(string message)
        {
            _logger.Invoke(message);
        }

        public override string ToString()
        {
            return $"Manager ID: {IdManager}, Level: {Level}, Supervised Employees: {SupervisedEmployeeCount}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Manager other)
                return IdManager == other.IdManager && Level == other.Level && SupervisedEmployees.SequenceEqual(other.SupervisedEmployees);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdManager, Level, SupervisedEmployeeCount);
        }
    }
}