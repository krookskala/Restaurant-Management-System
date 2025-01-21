using ConsoleApp1.Services;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Models
{
    public class Waiter : SerializableObject<Waiter>
    {
        [Required(ErrorMessage = "Waiter ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Waiter ID must be a positive integer.")]
        public int IdWaiter { get; private set; }

        public IReadOnlyList<Table> AssignedTables => _assignedTables.AsReadOnly();
        private readonly List<Table> _assignedTables = new();

        // Derived Attribute
        public int AssignedTableCount => _assignedTables.Count;

        // Events for Logging
        public event Action<string>? OnTableAssigned;
        public event Action<string>? OnTableUnassigned;

        // Structured Logger Placeholder
        private readonly Action<string> _logger;

        public Waiter(int idWaiter, Action<string> logger = null!)
        {
            if (idWaiter < 1)
                throw new ArgumentException("Waiter ID must be a positive integer.", nameof(idWaiter));

            IdWaiter = idWaiter;
            _logger = logger ?? Console.WriteLine;
        }

        // Constructor with Assigned Tables
        public Waiter(int idWaiter, List<Table> assignedTables, Action<string> logger = null!) : this(idWaiter, logger)
        {
            if (assignedTables == null || assignedTables.Any(t => t == null))
                throw new ArgumentException("Assigned Tables cannot contain null values.", nameof(assignedTables));

            foreach (var table in assignedTables.Distinct())
            {
                AssignTable(table); // reverse connection
            }
        }

        // Assign a Table
        public void AssignTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Table Cannot Be Null.");

            if (_assignedTables.Contains(table))
            {
                OnTableAssigned?.Invoke($"Table {table.IdTable} is already assigned to Waiter {IdWaiter}.");
                return;
            }

            table.AssignWaiter(this); // Establish reverse connection
            _assignedTables.Add(table);
            OnTableAssigned?.Invoke($"Table {table.IdTable} assigned to Waiter {IdWaiter}.");
            LogAction($"Table {table.IdTable} assigned to Waiter {IdWaiter}.");
        }

        // Unassign a Table
        public bool UnassignTable(int tableId)
        {
            var table = _assignedTables.Find(t => t.IdTable == tableId);
            if (table != null)
            {
                table.UnassignWaiter(); // Remove reverse connection
                _assignedTables.Remove(table);
                OnTableUnassigned?.Invoke($"Table {table.IdTable} unassigned from Waiter {IdWaiter}.");
                LogAction($"Table {table.IdTable} unassigned from Waiter {IdWaiter}.");
                return true;
            }

            OnTableUnassigned?.Invoke($"Table with ID {tableId} not found in Waiter {IdWaiter}'s assigned tables.");
            LogAction($"Table with ID {tableId} not found in Waiter {IdWaiter}'s assigned tables.");
            return false;
        }

        // Clear All Assigned Tables
        public void ClearAssignedTables()
        {
            foreach (var table in _assignedTables)
                table.UnassignWaiter(); // Remove reverse connections

            _assignedTables.Clear();
            LogAction($"All tables unassigned from Waiter {IdWaiter}.");
        }

        private void LogAction(string message)
        {
            _logger.Invoke(message);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Waiter other)
            {
                return IdWaiter == other.IdWaiter && _assignedTables.SequenceEqual(other._assignedTables);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdWaiter, AssignedTableCount, _assignedTables.Aggregate(0, (acc, table) => acc ^ table.GetHashCode()));
        }

        public override string ToString()
        {
            return $"Waiter ID: {IdWaiter}, Assigned Tables: {AssignedTableCount}";
        }
    }
}