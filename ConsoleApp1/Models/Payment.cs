using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Payment : SerializableObject<Payment>
    {
        private static readonly List<Payment> _paymentExtent = new();
        public static IReadOnlyCollection<Payment> PaymentExtent => _paymentExtent.AsReadOnly();

        [Required(ErrorMessage = "Payment ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Payment ID must be a positive integer.")]
        public int IdPayment { get; private set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; private set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public PaymentMethod Method { get; private set; }

        [Required(ErrorMessage = "Payment status is required.")]
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

        public string? CardNumber { get; private set; }
        public string? CardHolderName { get; private set; }
        public string? CashierName { get; private set; }

        // Events for Payment Lifecycle
        public event Action<string>? OnPaymentProcessed;
        public event Action<string>? OnPaymentRefunded;

        // Constructor for Card Payment
        public Payment(int idPayment, double amount, string cardNumber, string cardHolderName)
        {
            InitializePayment(idPayment, amount, PaymentMethod.Card);

            if (string.IsNullOrWhiteSpace(cardNumber) || !cardNumber.All(char.IsDigit) || (cardNumber.Length != 16 && cardNumber.Length != 15))
                throw new ArgumentException("Invalid card number. Must be 15 or 16 digits.", nameof(cardNumber));

            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new ArgumentException("Card holder name cannot be empty.", nameof(cardHolderName));

            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
        }

        // Constructor for Cash Payment
        public Payment(int idPayment, double amount, string cashierName)
        {
            InitializePayment(idPayment, amount, PaymentMethod.Cash);

            if (string.IsNullOrWhiteSpace(cashierName))
                throw new ArgumentException("Cashier name cannot be empty.", nameof(cashierName));

            CashierName = cashierName;
        }

        private void InitializePayment(int idPayment, double amount, PaymentMethod method)
        {
            if (idPayment <= 0)
                throw new ArgumentException("Payment ID must be greater than zero.", nameof(idPayment));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            if (_paymentExtent.Any(p => p.IdPayment == idPayment))
                throw new InvalidOperationException($"Payment with ID {idPayment} already exists.");

            IdPayment = idPayment;
            Amount = amount;
            Method = method;

            _paymentExtent.Add(this);
        }

        // Method to Process Payment
        public bool ProcessPayment()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Payment {IdPayment} is already {Status}.");

            Status = PaymentStatus.Completed;
            LogAction($"Payment {IdPayment} using {Method} completed.");
            OnPaymentProcessed?.Invoke($"Payment {IdPayment} completed.");
            return true;
        }

        // Method to Refund Payment
        public bool RefundPayment(double refundAmount)
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException($"Payment {IdPayment} is not completed and cannot be refunded.");

            if (refundAmount <= 0 || refundAmount > Amount)
                throw new ArgumentException("Refund amount must be greater than zero and less than or equal to the payment amount.");

            Status = PaymentStatus.Refunded;
            LogAction($"Payment {IdPayment} has been refunded with amount {refundAmount:C}.");
            OnPaymentRefunded?.Invoke($"Payment {IdPayment} refunded.");
            return true;
        }

        public static Payment? FindPaymentById(int idPayment)
        {
            return _paymentExtent.FirstOrDefault(p => p.IdPayment == idPayment);
        }

        public static IEnumerable<Payment> GetPaymentsByStatus(PaymentStatus status)
        {
            return _paymentExtent.Where(p => p.Status == status);
        }

        public static bool RemovePayment(int idPayment)
        {
            var payment = FindPaymentById(idPayment);
            if (payment == null)
                return false;

            _paymentExtent.Remove(payment);
            return true;
        }

        public static void ClearExtent()
        {
            _paymentExtent.Clear();
        }

        private void LogAction(string message)
        {
            Console.WriteLine(message); 
        }

        public override string ToString()
        {
            string paymentDetails = $"Payment [ID: {IdPayment}, Amount: {Amount:C}, Method: {Method}, Status: {Status}]";

            if (Method == PaymentMethod.Card)
                paymentDetails += $", Card Holder: {CardHolderName}";

            if (Method == PaymentMethod.Cash)
                paymentDetails += $", Cashier: {CashierName}";

            return paymentDetails;
        }
    }

    public enum PaymentMethod
    {
        Card,
        Cash
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Refunded
    }
}
