namespace ConsoleApp1.Model
{
    public abstract class Customer : Person, SerializableObject<Customer>
    {
        public int IdCustomer { get; set; }
        public List<Reservation> Reservations { get; private set; } // Association with Reservation
        public List<Order> Orders { get; private set; } = new List<Order>(); // Association with Order

        // Constructor
        public Customer(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                        int idCustomer)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber)
        {
            IdCustomer = idCustomer;
            Reservations = new List<Reservation>();
            Orders = new List<Order>();
            InstanceCollection.Add(this);
        }

        // Methods
        public abstract void MakePayment()

        public void PlaceOrder(Order order)
        {
            if (order == null)
                throw new ArgumentException("Order Cannot Be Null.");

            Orders.Add(order);
            Console.WriteLine($"Order {order.IdOrder} Placed By {FirstName} {LastName}.");
        }
    
        // Method To Add A Reservation
        public void AddReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentException("Reservation Cannot Be Null.");

            Reservations.Add(reservation);
            Console.WriteLine($"Reservation {reservation.IdReservation} Added For {FirstName} {LastName}.");    
        }

        // Override GetDetails
        public override string GetDetails()
        {
            return $"Customer ID: {IdCustomer}\nName: {FirstName} {LastName}";
        }
    }
}