namespace ConsoleApp1.Models
{
    public class OrderDish : SerializableObject<OrderDish>
    {
        public Dish Dish { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        // Constructor
        public OrderDish(Dish dish, int quantity, double unitPrice)
        {
            if (dish == null)
                throw new ArgumentNullException("Dish Cannot Be Null.");
            if (quantity <= 0)
                throw new ArgumentException("Quantity Must Be Greater Than Zero.");
            if (unitPrice <= 0)
                throw new ArgumentException("Unit Price Must Be Greater Than Zero.");


            Dish = dish;
            Quantity = quantity;
            UnitPrice = unitPrice;
            InstanceCollection.Add(this);
        }

        // Method To Update The Quantity
        public void UpdateQuantity(int quantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity Must Be Greater Than Zero.");

            Quantity = newQuantity;
            Console.WriteLine($"Quantity For {Dish.Name} Updated To {Quantity}.");
        }
    }
}