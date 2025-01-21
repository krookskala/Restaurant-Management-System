using ConsoleApp1.Services;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Models
{
    public class Table : SerializableObject<Table>
    {
        private static readonly List<Table> _tableExtent = new();
        public static IReadOnlyCollection<Table> TableExtent => _tableExtent.AsReadOnly();

        [Required(ErrorMessage = "Table ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Table ID must be a positive integer.")]
        public int IdTable { get; private set; }

        [Required(ErrorMessage = "Number of chairs is required.")]
        [Range(1, 20, ErrorMessage = "Number of chairs must be between 1 and 20.")]
        public int NumberOfChairs { get; private set; }

        [Required(ErrorMessage = "Table type is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Table type must be between 3 and 50 characters.")]
        public string TableType { get; private set; }

        private readonly List<Reservation> _reservations = new();
        public IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();

        public Waiter? AssignedWaiter { get; private set; }

        // Events for Logging
        public event Action<string>? OnReservationAdded;
        public event Action<string>? OnReservationRemoved;

        public Table(int idTable, int numberOfChairs, string tableType)
        {
            if (_tableExtent.Any(t => t.IdTable == idTable))
                throw new InvalidOperationException($"Table with ID {idTable} already exists.");

            IdTable = idTable > 0 ? idTable : throw new ArgumentException("Table ID must be greater than zero.");
            NumberOfChairs = numberOfChairs is >= 1 and <= 20
                ? numberOfChairs
                : throw new ArgumentException("Number of chairs must be between 1 and 20.");
            TableType = !string.IsNullOrWhiteSpace(tableType) && tableType.Length is >= 3 and <= 50
                ? tableType
                : throw new ArgumentException("Table type must be between 3 and 50 characters.");

            _tableExtent.Add(this); 
        }

        // Method to Add a Reservation
        public void AddReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation), "Reservation cannot be null.");

            if (Reservation.IsTableReserved(this, reservation.DateOfReservation))
                throw new InvalidOperationException($"Table {IdTable} is already reserved for this date.");

            if (!_reservations.Contains(reservation))
            {
                _reservations.Add(reservation); 
                reservation.UpdateTable(this);  
            }

            LogAction($"Reservation {reservation.IdReservation} added to Table {IdTable}.");
        }
        
        // Method to Remove a Reservation
        public bool RemoveReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation), "Reservation cannot be null.");

            if (_reservations.Remove(reservation))
            {
                if (reservation.ReservedTable == this)
                {
                    reservation.UpdateTable(null); // Reverse connection cleanup
                }

                OnReservationRemoved?.Invoke($"Reservation removed from Table {IdTable}.");
                LogAction($"Reservation removed from Table {IdTable}.");
                return true;
            }

            return false;
        }

        // Method to Clear All Reservations
        public void ClearReservations()
        {
            foreach (var reservation in _reservations.ToList())
                reservation.UpdateTable(null); // Reverse connection cleanup

            _reservations.Clear();
            LogAction($"All reservations cleared from Table {IdTable}.");
        }

        // Method to Assign a Waiter
        public void AssignWaiter(Waiter waiter)
        {
            if (AssignedWaiter != null)
                throw new InvalidOperationException($"Table {IdTable} is already assigned to Waiter {AssignedWaiter.IdWaiter}.");

            AssignedWaiter = waiter;
            LogAction($"Table {IdTable} assigned to Waiter {waiter.IdWaiter}.");
        }

        // Method to Unassign a Waiter
        public void UnassignWaiter()
        {
            if (AssignedWaiter != null)
                LogAction($"Table {IdTable} unassigned from Waiter {AssignedWaiter.IdWaiter}.");

            AssignedWaiter = null;
        }

        // Static Method to Clear Extent
        public static void ClearExtent()
        {
            _tableExtent.Clear();
        }
        

        // Static Method to Remove a Table
        public static bool RemoveTable(Table table)
        {
            return _tableExtent.Remove(table);
        }

        private void LogAction(string message)
        {
            Console.WriteLine(message); 
        }

        public override string ToString()
        {
            string waiterInfo = AssignedWaiter != null ? AssignedWaiter.IdWaiter.ToString() : "None";
            return $"Table {IdTable} - Type: {TableType}, Chairs: {NumberOfChairs}, Reservations: {_reservations.Count}, Assigned Waiter: {waiterInfo}";
        }
    }
}