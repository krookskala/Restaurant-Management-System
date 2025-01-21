namespace ConsoleApp1.Model
{
    public class Payment : SerializableObject<Payment>
    {
        public int IdPayment { get; set; }
        public double Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }

        // Constructor
        public Payment(int idPayment, double amount, string method, string status)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount Must Be Greater Than Zero.");

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("Payment Cannot Be Empty.");

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Payment Status Cannot Be Empty.");


            IdPayment = idPayment;
            Amount = amount;
            Method = method;
            Status = status;
            InstanceCollection.Add(this);
        }

        // Method To Process A Payment
        public void ProcessPayment()
        {
            Status = "Completed";
            System.Console.WriteLine($"Payment {IdPayment} Of {Amount} Has Been Processed Via {Method}.");
        }

        // Method To Refund A Payment
        public void RefundPayment()
        {
            Status = "Refunded";
            System.Console.WriteLine($"Payment {IdPayment} Of {Amount} Has Been Refunded.");
        }
    }
}