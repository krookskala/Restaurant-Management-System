using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Reservation : SerializableObject<Reservation>, IDisposable
    {
        private static readonly object LockObject = new();
        private static readonly List<Reservation> _reservationExtent = new();
        private bool _disposed;

        public static IReadOnlyCollection<Reservation> ReservationExtent
        {
            get
            {
                lock (LockObject)
                {
                    return _reservationExtent.AsReadOnly();
                }
            }
        }

        public int IdReservation { get; }
        public DateTime DateOfReservation { get; }
        public Table ReservedTable { get; private set; } 
        public Customer Customer { get; private set; } 
        public ReservationStatus Status { get; private set; }

        public event Action<string>? OnReservationMade;
        public event Action<string>? OnReservationCanceled;
        public event Action<string>? OnReservationStatusChanged;

        public Reservation(int idReservation, DateTime dateOfReservation, Table reservedTable, Customer customer)
        {
            if (reservedTable == null) throw new ArgumentNullException(nameof(reservedTable));
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            if (dateOfReservation < DateTime.Now)
                throw new ArgumentException("Reservation date cannot be in the past.", nameof(dateOfReservation));

            lock (LockObject)
            {
                if (_reservationExtent.Any(r => r.IdReservation == idReservation))
                    throw new InvalidOperationException($"Reservation ID {idReservation} already exists.");

                if (IsTableReserved(reservedTable, dateOfReservation))
                    throw new InvalidOperationException("Table is already reserved for this time.");

                IdReservation = idReservation;
                DateOfReservation = dateOfReservation;
                ReservedTable = reservedTable;
                Customer = customer;
                Status = ReservationStatus.Pending;

                _reservationExtent.Add(this);
            }
            
            reservedTable.AddReservation(this);
            customer.AddReservation(this);
            LogAction($"Created reservation {IdReservation} for Table {ReservedTable.IdTable} by Customer {Customer.IdCustomer} on {DateOfReservation.ToShortDateString()}.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Reservation));
        }

        public void ConfirmReservation()
        {
            ThrowIfDisposed();
            if (Status != ReservationStatus.Pending)
                throw new InvalidOperationException("Only pending reservations can be confirmed.");

            Status = ReservationStatus.Confirmed;
            OnReservationMade?.Invoke($"Table {ReservedTable.IdTable} reserved by {Customer.IdCustomer} on {DateOfReservation.ToShortDateString()}.");
            OnReservationStatusChanged?.Invoke($"Reservation {IdReservation} status changed to Confirmed.");
            LogAction($"Reservation {IdReservation} confirmed for Table {ReservedTable.IdTable}.");
        }

        public void CancelReservation()
        {
            ThrowIfDisposed();
            if (Status == ReservationStatus.Canceled)
                throw new InvalidOperationException("Reservation is already canceled.");

            Status = ReservationStatus.Canceled;
            OnReservationCanceled?.Invoke($"Reservation {IdReservation} for Table {ReservedTable.IdTable} by {Customer.IdCustomer} has been canceled.");
            OnReservationStatusChanged?.Invoke($"Reservation {IdReservation} status changed to Canceled.");
            LogAction($"Reservation {IdReservation} canceled for Table {ReservedTable.IdTable}.");

            lock (LockObject)
            {
                _reservationExtent.Remove(this);
            }

            Customer.RemoveReservation(this);
        }

        public void UpdateTable(Table newTable)
        {
            if (newTable == null)
                throw new ArgumentNullException(nameof(newTable), "New table cannot be null.");

            if (ReservedTable != null)
            {
                ReservedTable.RemoveReservation(this);
            }

            if (IsTableReserved(newTable, DateOfReservation))
                throw new InvalidOperationException($"Table {newTable.IdTable} is already reserved for this date.");

            ReservedTable = newTable;

            if (!newTable.Reservations.Contains(this))
            {
                newTable.AddReservation(this);
            }

            LogAction($"Updated table for Reservation {IdReservation} to Table {newTable.IdTable}.");
        }

        
        public void UpdateCustomer(Customer newCustomer)
        {
            if (newCustomer == null)
                throw new ArgumentNullException(nameof(newCustomer), "New customer cannot be null.");

            Customer.RemoveReservation(this); // Remove reverse connection
            newCustomer.AddReservation(this); // Establish new reverse connection
            Customer = newCustomer;
            LogAction($"Updated customer for Reservation {IdReservation} to Customer {newCustomer.IdCustomer}.");
        }

        public static bool IsTableReserved(Table table, DateTime date)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Table cannot be null.");

            lock (LockObject)
            {
                return _reservationExtent.Any(reservation =>
                    reservation.ReservedTable.IdTable == table.IdTable &&
                    reservation.DateOfReservation.Date == date.Date);
            }
        }


        public static void CancelReservationsForTable(int tableId)
        {
            lock (LockObject)
            {
                foreach (var reservation in _reservationExtent.Where(r => r.ReservedTable.IdTable == tableId).ToList())
                {
                    reservation.CancelReservation();
                }
            }
        }

        public static void CancelReservationsForCustomer(int customerId)
        {
            lock (LockObject)
            {
                foreach (var reservation in _reservationExtent.Where(r => r.Customer.IdCustomer == customerId).ToList())
                {
                    reservation.CancelReservation();
                }
            }
        }

        public static void ClearExtent()
        {
            lock (LockObject)
            {
                _reservationExtent.Clear();
                LogAction("All reservations cleared from the extent.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CancelReservation();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Reservation()
        {
            LogAction($"Finalizer called for Reservation {IdReservation}. Dispose method should be used instead.");
            Dispose(false);
        }

        private static void LogAction(string message)
        {
            Console.WriteLine(message); 
        }

        public override string ToString()
        {
            return $"Reservation [ID: {IdReservation}, Table: {ReservedTable.IdTable}, Customer: {Customer.IdCustomer}, Date: {DateOfReservation.ToShortDateString()}]";
        }
    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Canceled
    }
}