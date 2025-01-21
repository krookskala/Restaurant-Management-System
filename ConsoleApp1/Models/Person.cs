using ConsoleApp1.Services;
using System.ComponentModel.DataAnnotations;
namespace ConsoleApp1.Models
{
    public abstract class Person : SerializableObject<Person>
    {
        [Required(ErrorMessage = "Person ID Is Required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Person ID Must Be A Positive Integer.")]
        public int IdPerson { get; private set; } 

        [Required(ErrorMessage = "First Name Is Required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name Must Be Between 2 And 50 Characters.")]
        public string FirstName { get; private set; } 

        [Required(ErrorMessage = "Last Name Is Required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name Must Be Between 2 And 50 Characters.")]
        public string LastName { get; private set; } 

        [Required(ErrorMessage = "Birth Date Is Required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Person), nameof(ValidateBirthDate))]
        public DateTime BirthOfDate { get; private set; } 

        [Required(ErrorMessage = "Phone Number Is Required.")]
        [Phone(ErrorMessage = "Invalid Phone Number Format.")]
        public string PhoneNumber { get; private set; } 

        public Person(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber)
        {
            // Validate ID
            if (idPerson <= 0)
                throw new ArgumentException("Person ID must be a positive integer.", nameof(idPerson));

            // Validate First Name
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 50)
                throw new ArgumentException("First name must be between 2 and 50 characters.", nameof(firstName));

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 50)
                throw new ArgumentException("Last name must be between 2 and 50 characters.", nameof(lastName));

            // Validate Birth Date
            if (birthOfDate > DateTime.Now)
                throw new ArgumentException("Birth date cannot be in the future.", nameof(birthOfDate));

            // Validate Phone Number
            if (!new PhoneAttribute().IsValid(phoneNumber))
                throw new ArgumentException("Invalid phone number format.", nameof(phoneNumber));

            // Assign properties
            IdPerson = idPerson;
            FirstName = firstName;
            LastName = lastName;
            BirthOfDate = birthOfDate;
            PhoneNumber = phoneNumber;
        }

        public void UpdatePhoneNumber(string newPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(newPhoneNumber) || !new PhoneAttribute().IsValid(newPhoneNumber))
                throw new ArgumentException("Invalid phone number format.");

            PhoneNumber = newPhoneNumber;
        }

        public void UpdateName(string newFirstName, string newLastName)
        {
            if (string.IsNullOrWhiteSpace(newFirstName) || newFirstName.Length < 2 || newFirstName.Length > 50)
                throw new ArgumentException("First name must be between 2 and 50 characters.");

            if (string.IsNullOrWhiteSpace(newLastName) || newLastName.Length < 2 || newLastName.Length > 50)
                throw new ArgumentException("Last name must be between 2 and 50 characters.");

            FirstName = newFirstName;
            LastName = newLastName;
        }

        public static ValidationResult? ValidateBirthDate(DateTime birthOfDate, ValidationContext context)
        {
            if (birthOfDate > DateTime.Now)
            {
                return new ValidationResult("Birth date cannot be in the future.");
            }
            return ValidationResult.Success;
        }

        // Abstract method
        public abstract string GetDetails();

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Person)obj;
            return IdPerson == other.IdPerson && FirstName == other.FirstName && LastName == other.LastName && BirthOfDate == other.BirthOfDate && PhoneNumber == other.PhoneNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdPerson, FirstName, LastName, BirthOfDate, PhoneNumber);
        }

        public override string ToString()
        {
            return $"Person [ID: {IdPerson}, Name: {FirstName} {LastName}, Birth Date: {BirthOfDate.ToShortDateString()}, Phone Number: {PhoneNumber}]";
        }
    }
}
