using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Member : SerializableObject<Member>
    {
        [Required(ErrorMessage = "Member ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Member ID must be a positive integer.")]
        public int IdMember { get; private set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; private set; }

        [Range(0, int.MaxValue, ErrorMessage = "Credit points cannot be negative.")]
        public int CreditPoints { get; private set; }

        private static int _creditPointsRate = 10;

        // Static Property for Credit Points Rate
        public static int CreditPointsRate
        {
            get => _creditPointsRate;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Credit points rate must be greater than zero.");
                _creditPointsRate = value;
            }
        }

        // Events for Logging Actions
        public event Action<string>? OnCreditPointsUsed;
        public event Action<string>? OnCreditPointsEarned;

        // Constructor
        public Member(int idMember, int initialCreditPoints = 0, string? email = null)
        {
            if (idMember <= 0)
                throw new ArgumentException("Member ID must be a positive integer.", nameof(idMember));
            if (initialCreditPoints < 0)
                throw new ArgumentException("Initial credit points cannot be negative.", nameof(initialCreditPoints));

            IdMember = idMember;
            CreditPoints = initialCreditPoints;
            Email = email;
        }

        // Method to Use Credit Points
        public bool UseCreditPoints(int points)
        {
            if (points <= 0)
                throw new ArgumentException("Points must be greater than zero.", nameof(points));

            if (CreditPoints >= points)
            {
                CreditPoints -= points;
                OnCreditPointsUsed?.Invoke($"Used {points} credit points. Remaining: {CreditPoints}.");
                return true;
            }

            return false;
        }

        // Method to Earn Credit Points
        public void EarnCreditPoints(int orderValue)
        {
            if (orderValue <= 0)
                throw new ArgumentException("Order value must be greater than zero.", nameof(orderValue));

            int earnedPoints = orderValue / CreditPointsRate;
            CreditPoints += earnedPoints;
            OnCreditPointsEarned?.Invoke($"Earned {earnedPoints} credit points. Total: {CreditPoints}.");
        }

        public override string ToString()
        {
            return $"Member [ID: {IdMember}, Email: {Email}, Credit Points: {CreditPoints}]";
        }
    }

    public class NonMember : SerializableObject<NonMember>
    {
        [Required(ErrorMessage = "NonMember ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "NonMember ID must be a positive integer.")]
        public int Id { get; private set; }

        private static readonly List<NonMember> NonMembersList = new();
        public static IReadOnlyCollection<NonMember> NonMembers => NonMembersList.AsReadOnly();

        public Member? ConvertedMember { get; private set; }

        public NonMember(int id)
        {
            if (id <= 0)
                throw new ArgumentException("NonMember ID must be a positive integer.", nameof(id));

            Id = id;
            NonMembersList.Add(this);
        }

        // Method to Become a Member
        public Member BecomeMember(int idMember, string? email = null)
        {
            if (idMember <= 0)
                throw new ArgumentException("Member ID must be a positive integer.", nameof(idMember));

            var newMember = new Member(idMember, email: email);
            Member.AddInstance(newMember);
            ConvertedMember = newMember; 
            NonMembersList.Remove(this);
            return newMember;
        }

        public override string ToString()
        {
            return $"NonMember [ID: {Id}]";
        }
    }
}