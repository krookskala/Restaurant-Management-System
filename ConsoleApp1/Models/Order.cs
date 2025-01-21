using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Order : SerializableObject<Order>
    {
        private static readonly List<Order> OrderExtentList = new();
        public static IReadOnlyCollection<Order> OrderExtent => OrderExtentList.AsReadOnly();

        [Required(ErrorMessage = "Order ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order ID must be a positive integer.")]
        public int IdOrder { get; private set; }

        [Required(ErrorMessage = "Timestamp is required.")]
        public DateTime TimeStamp { get; private set; } = DateTime.Now;
        
        public bool IsPaid { get; private set; } = false;

        public List<OrderDish> OrderDishes { get; private set; } = new();

        [Required]
        public Customer Customer { get; private set; }

        // Constructor
        public Order(int idOrder, Customer customer)
        {
            if (idOrder <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(idOrder));

            if (OrderExtentList.Any(o => o.IdOrder == idOrder))
                throw new InvalidOperationException($"Order with ID {idOrder} already exists.");

            Customer = customer ?? throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");

            IdOrder = idOrder;
            OrderExtentList.Add(this);
            customer.AddOrder(this); // Reverse connection
        }

        // Method to Add an OrderDish to the Order
        public void AddOrderDish(OrderDish orderDish)
        {
            if (orderDish == null)
                throw new ArgumentNullException(nameof(orderDish), "OrderDish cannot be null.");

            if (orderDish.Quantity <= 0)
                throw new ArgumentException("OrderDish quantity must be greater than zero.");

            // Check if the dish already exists in the order
            var existingItem = OrderDishes.FirstOrDefault(d => d.Dish.Equals(orderDish.Dish));
            if (existingItem != null)
            {
                // Update the existing OrderDish quantity
                existingItem.UpdateQuantity(existingItem.Quantity + orderDish.Quantity);
                LogAction($"Updated quantity of {orderDish.Dish.Name} in Order {IdOrder}.");
            }
            else
            {
                // Add the new OrderDish
                OrderDishes.Add(orderDish);
                LogAction($"Added {orderDish.Quantity}x {orderDish.Dish.Name} to Order {IdOrder}.");
            }
        }


        // Method to Remove an OrderDish from the Order
        public void RemoveOrderDish(OrderDish orderDish)
        {
            if (orderDish == null)
                throw new ArgumentNullException(nameof(orderDish), "OrderDish cannot be null.");

            if (!OrderDishes.Remove(orderDish))
                throw new InvalidOperationException($"OrderDish {orderDish.Dish.Name} not found in Order {IdOrder}.");

            orderDish.RemoveFromDish(); // Cleanup reverse connection
            LogAction($"Removed {orderDish.Dish.Name} from Order {IdOrder}.");
        }

        // Method to Cancel the Order
        public void CancelOrder()
        {
            if (IsPaid)
                throw new InvalidOperationException($"Order {IdOrder} is already paid and cannot be canceled.");

            // Remove all OrderDishes
            foreach (var orderDish in OrderDishes.ToList())
            {
                try
                {
                    RemoveOrderDish(orderDish); 
                }
                catch (Exception ex)
                {
                    LogAction($"Failed to remove OrderDish {orderDish.Dish.Name} from Order {IdOrder}: {ex.Message}");
                }
            }

            // Remove order from extent and customer
            OrderExtentList.Remove(this);
            Customer?.RemoveOrder(this); 
            LogAction($"Order {IdOrder} has been canceled.");
        }


        // Method to Calculate the Total
        public decimal CalculateTotal()
        {
            return OrderDishes.Sum(item => item.TotalPrice);
        }

        // Method to Display the Order
        public string DisplayOrder()
        {
            var items = string.Join("\n", OrderDishes.Select(item =>
                $"- {item.Quantity}x {item.Dish.Name} (${item.UnitPrice} each)"));

            return $"Order ID: {IdOrder}\nCustomer: {Customer.IdCustomer}\nTimestamp: {TimeStamp}\nItems:\n{items}\nTotal: ${CalculateTotal():0.00}";
        }
        
        public void MarkAsPaid()
        {
            if (IsPaid)
                throw new InvalidOperationException($"Order {IdOrder} is already paid.");

            IsPaid = true;
            LogAction($"Order {IdOrder} marked as paid.");
        }
        
        public static void CancelOrdersForCustomer(int customerId)
        {
            var ordersToCancel = OrderExtentList
                .Where(o => o.Customer?.IdCustomer == customerId)
                .ToList();

            foreach (var order in ordersToCancel)
            {
                try
                {
                    order.CancelOrder();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to cancel Order {order.IdOrder}: {ex.Message}");
                }
            }

            Console.WriteLine($"Cancellation completed for all orders of Customer {customerId}.");
        }

        
        public static void ClearExtent()
        {
            Console.WriteLine($"Clearing OrderExtentList. Current count: {OrderExtentList.Count}");
            OrderExtentList.Clear();
        }
        
        public void UnassignCustomer()
        {
            if (Customer == null)
                return; 
            Customer = null;
            LogAction($"Customer unassigned from Order {IdOrder}.");
        }


        private void LogAction(string message)
        {
            Console.WriteLine(message); 
        }

        public override string ToString()
        {
            return $"Order [ID: {IdOrder}, Customer: {Customer.IdCustomer}, Total: ${CalculateTotal():0.00}]";
        }
    }
}