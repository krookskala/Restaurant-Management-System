namespace ConsoleApp1.Models
{
    public class Order : SerializableObject<Order>
    {
        public int IdOrder { get; set; }
        public DateTime Timestamp { get; set; }
        public List<OrderDish> OrderItems { get; set; } = new List<OrderDish>();
        public Customer Customer { get; set; }

        // Constructor
        public Order(int idOrder, Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("Customer Cannot Be Null.");


            IdOrder = idOrder;
            Timestamp = DateTime.Now;
            Customer = customer;

            customer.AddOrder(this);
            InstanceCollection.Add(this);
        }

        // Method To Add An Item To The Order
        public void AddItem(OrderDish item)
        {
            if (item == null)
                throw new ArgumentNullException("OrderDish Cannot Be Null.");


            OrderItems.Add(item);
            Console.WriteLine($"Dish {item.Dish.Name} Added To Order {IdOrder}.");
        }

        // Method To Remove An Item From The Order
        public void RemoveItem(OrderDish item)
        {
            if (item == null)
                throw new ArgumentNullException("OrderDish Cannot Be Null.");


            OrderItems.Remove(item);
            Console.WriteLine($"Dish {item.Dish.Name} Removed From Order {IdOrder}.");
        }

        // Method To Cancel The Order
        public void CancelOrder()
        {
            Console.WriteLine($"Order {IdOrder} Has Been Canceled.");
            OrderItems.Clear();
            InstanceCollection.Remove(this);
        }

        // Method To Calculate The Total
        public double CalculateTotal()
        {
            double total = 0;
            foreach (var item in OrderItems)
            {
                total += item.Quantity * item.UnitPrice;
            }
            return total;
        }

        // Method To Display The Order
        public void DisplayOrder()
        {
            Console.WriteLine($"Order ID: {IdOrder}\nCustomer: {Customer.FirstName} {Customer.LastName}\nTimestamp: {Timestamp}");
            Console.WriteLine("Items:");
            foreach (var item in OrderItems)
            {
                Console.WriteLine($"- {item.Quantity}x {item.Dish.Name} (${item.UnitPrice} each)");
            }
            Console.WriteLine($"Total: ${CalculateTotal():0.00}");
        }
    }
}