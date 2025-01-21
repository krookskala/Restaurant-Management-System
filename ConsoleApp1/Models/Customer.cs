using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;
using System.Linq;

namespace ConsoleApp1.Models
{
    public class Customer : SerializableObject<Customer>
    {
        private static readonly Dictionary<string, Customer> CustomersByEmail = new();
        public static IReadOnlyCollection<Customer> Customers => CustomersByEmail.Values.ToList().AsReadOnly();

        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int IdCustomer { get; private set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; private set; }

        private readonly List<Order> _orders = new();
        public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        private readonly List<Reservation> _reservations = new();
        public IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();

        private readonly List<Payment> _payments = new();
        public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

        // Events for Logging
        public event Action<string>? OnOrderPlaced;
        public event Action<string>? OnPaymentCompleted;
        public event Action<string>? OnPaymentFailed;

        // Constructor
        public Customer(int idCustomer, string email)
        {
            if (idCustomer <= 0)
                throw new ArgumentException("Customer ID must be greater than zero.", nameof(idCustomer));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (CustomersByEmail.ContainsKey(email))
                throw new InvalidOperationException($"A customer with email {email} already exists.");

            IdCustomer = idCustomer;
            Email = email;
            CustomersByEmail[email] = this;
        }

        public static Customer? GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.");

            return CustomersByEmail.TryGetValue(email, out var customer) ? customer : null;
        }

        public static void RemoveCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.");

            CustomersByEmail.Remove(email);
        }

        public void AddReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            if (_reservations.Contains(reservation))
                throw new InvalidOperationException($"Reservation {reservation.IdReservation} is already associated with Customer {IdCustomer}.");

            _reservations.Add(reservation);
        }

        public void RemoveReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            if (!_reservations.Remove(reservation))
                throw new InvalidOperationException($"Reservation {reservation.IdReservation} is not associated with Customer {IdCustomer}.");

            reservation.UpdateCustomer(null!); // Reverse connection cleanup
        }

        public void ClearReservations()
        {
            foreach (var reservation in _reservations.ToList())
            {
                reservation.UpdateCustomer(null!); // Reverse connection cleanup
            }
            _reservations.Clear();
        }

        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (_orders.Contains(order))
                throw new InvalidOperationException($"Order {order.IdOrder} is already associated with Customer {IdCustomer}.");

            _orders.Add(order);
        }

        public void RemoveOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!_orders.Remove(order))
                return; 

            order.UnassignCustomer(); // Reverse connection cleanup
        }


        public void ClearOrders()
        {
            foreach (var order in _orders.ToList())
            {
                order.UnassignCustomer(); // Reverse connection cleanup
            }
            _orders.Clear();
        }

        public void AddPayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            if (_payments.Contains(payment))
                throw new InvalidOperationException($"Payment {payment.IdPayment} is already associated with Customer {IdCustomer}.");

            _payments.Add(payment);
        }

        public void RemovePayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            if (!_payments.Remove(payment))
                throw new InvalidOperationException($"Payment {payment.IdPayment} is not associated with Customer {IdCustomer}.");
        }

        public void ClearPayments()
        {
            _payments.Clear();
        }

        public Order PlaceOrder(Dish[] dishes)
        {
            if (dishes == null || dishes.Length == 0)
                throw new ArgumentException("At least one dish must be ordered.", nameof(dishes));

            var order = new Order(Order.OrderExtent.Count + 1, this);
            foreach (var dish in dishes)
            {
                order.AddOrderDish(new OrderDish(dish, 1)); 
            }

            AddOrder(order);

            string message = $"Order {order.IdOrder} placed by Customer {IdCustomer} with {dishes.Length} dishes.";
            OnOrderPlaced?.Invoke(message);

            return order;
        }

        public bool MakePayment(Order order, Payment payment)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order cannot be null.");

            if (payment == null)
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");

            if (order.IsPaid)
                throw new InvalidOperationException($"Order {order.IdOrder} has already been paid.");

            if (!payment.ProcessPayment())
            {
                string message = $"Payment failed for Order {order.IdOrder} by Customer {IdCustomer}.";
                OnPaymentFailed?.Invoke(message);
                return false;
            }

            order.MarkAsPaid();
            AddPayment(payment);

            string successMessage = $"Payment of {payment.Amount:C} for Order {order.IdOrder} completed by Customer {IdCustomer}.";
            OnPaymentCompleted?.Invoke(successMessage);

            return true;
        }

        public string GetOrderSummary()
        {
            return string.Join("\n", Orders.Select(order => $"Order {order.IdOrder}: {order.CalculateTotal():C} ({order.TimeStamp})"));
        }

        public IEnumerable<Order> GetUnpaidOrders()
        {
            return Orders.Where(order => !order.IsPaid);
        }

        public override string ToString()
        {
            return $"Customer [ID: {IdCustomer}, Email: {Email}, Orders Count: {_orders.Count}, Payments Count: {_payments.Count}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is Customer other && IdCustomer == other.IdCustomer;
        }

        public override int GetHashCode()
        {
            return IdCustomer.GetHashCode();
        }

        public static void ClearCustomers()
        {
            CustomersByEmail.Clear();
        }
    }
}