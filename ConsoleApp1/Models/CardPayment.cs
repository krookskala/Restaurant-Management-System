namespace ConsoleApp1.Models
{
    public class CardPayment : Payment, SerializableObject<CardPayment>
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }

        // Constructor
        public CardPayment(int idPayment, double amount, string status, string cardNumber, string cardHolderName)
            : base(idPayment, amount, "Card", status)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new ArgumentException("Card Number Cannot Be Empty.");

            if (string.IsNullOrWhiteSpace(cardHolderName))
                throw new ArgumentException("Card Holder Name Cannot Be Empty.");
            

            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            InstanceCollection.Add(this);
        }
    }

    public class CashPayment : Payment, SerializableObject<CashPayment>
    {
        public string CashierName { get; set; }

        // Constructor
        public CashPayment(int idPayment, double amount, string status, string cashierName)
            : base(idPayment, amount, "Cash", status)
        {
            if (string.IsNullOrWhiteSpace(cashierName))
                throw new ArgumentException("Cashier Name Cannot Be Empty.");


            CashierName = cashierName;
            InstanceCollection.Add(this);
        }
    }
}