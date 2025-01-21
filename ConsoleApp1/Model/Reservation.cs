namespace ConsoleApp1.Model
{
    public class Reservation : SerializableObject<Reservation>
    {
        public int IdReservation { get; set; }
        public DateTime DateOfReservation { get; set; }
        public Table ReservedTable { get; set; }
        public Customer Customer { get; set; }

        // Constructor
        public Reservation(int idReservation, DateTime dateOfReservation, Table reservedTable, Customer customer)
        {
            if (reservedTable == null)
                throw new ArgumentNullException("Reserved Table Cannot Be Null.");
            if (customer == null)
                throw new ArgumentNullException("Customer Cannot Be Null.");


            IdReservation = idReservation;
            DateOfReservation = dateOfReservation;
            ReservedTable = reservedTable;
            Customer = customer;

            customer.AddReservation(this);
            InstanceCollection.Add(this);
        }

        // Method To Reserve A Table
        public void ReserveTable()
        {
            Console.WriteLine($"Table {ReservedTable.IdTable} Reserved By {Customer.FirstName} On {DateOfReservation.ToShortDateString()}.");
        }

        // Method To Cancel A Reservation
        public void CancelReservation()
        {
            Console.WriteLine($"Reservation {IdReservation} For Table {ReservedTable.IdTable} By {Customer.FirstName} Has Been Canceled.");
            Customer.Reservations.Remove(this);
            InstanceCollection.Remove(this);
        }

        // Method To Check If The Table Is Already Reserved For The Requested Date  
        public static bool IsTableReserved(Table table, DateTime date, List<Reservation> reservations)
        {
            foreach (var reservation in reservations)
            {
                if (reservation.ReservedTable.IdTable == table.IdTable && reservation.DateOfReservation == date.Date)
                   {
                        return true;
                   } 
            }
            return false;
        }
    }
}