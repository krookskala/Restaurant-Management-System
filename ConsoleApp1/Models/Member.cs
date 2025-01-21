namespace ConsoleApp1.Models
{
    public class Member : Customer, SerializableObject<Member>
    {
        public int IdMember { get; set; }
        public string Email { get; set; }
        public int CreditPoints { get; set; }
        public static int CreditPointsRate { get; set; } = 10;

        // Constructor
        public Member(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                      int idCustomer, int idMember, string email, int creditPoints = 0)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idCustomer)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email Cannot Be Empty.");

            if (creditPoints < 0)
                throw new ArgumentException("Credit Points Cannot Be Negative.");


            IdMember = idMember;
            Email = email;
            CreditPoints = creditPoints;
            InstanceCollection.Add(this);
        }

        // Method To Use Credit Points
        public void UseCreditPoints(int points)
        {
            if (points <= 0)
            {
                Console.WriteLine($"Invalid Points Value: {points}. Must Be Greater Than Zero.");
                return;
            }

            if (CreditPoints >= points)
            {
                CreditPoints -= points;
                Console.WriteLine($"{FirstName} {LastName} Used {points} Credit Points. Remaining: {CreditPoints}.");
            }
            else
            {
                Console.WriteLine($"{FirstName} {LastName} Does Not Have Enough Credit Points. Current Balance: {CreditPoints}.");
            }
        }

        // Method To Earn Credit Points
        public void EarnCreditPoints(int orderValue)
        {
            if (orderValue <= 0)
            {
                Console.WriteLine($"Invalid Order Value: {orderValue}. Must Be Greater Than Zero.");
                return;
            }


            int earnedPoints = orderValue / CreditPointsRate;
            CreditPoints += earnedPoints;
            Console.WriteLine($"{FirstName} {LastName} Earned {earnedPoints} Credit Points. Total: {CreditPoints}");
        }
    }

    public class NonMember : Customer, SerializableObject<NonMember>
    {
        // Constructor
        public NonMember(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                         int idCustomer)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idCustomer)
        {
            InstanceCollection.Add(this);
        }

        // Method To Become A Member
        public Member BecomeMember(int idMember, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email Cannot Be Empty.");

            Console.WriteLine($"{FirstName} {LastName} Is Now A Member.");
            return new Member(IdPerson, FirstName, LastName, BirthOfDate, PhoneNumber, IdCustomer, idMember, email);
        }
    }
}