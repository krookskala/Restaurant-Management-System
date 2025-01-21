using System.ComponentModel.DataAnnotations;
namespace ConsoleApp1.Model
{
    public abstract class Person
    {
        [Required(ErrorMessage = "Person ID Is Required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Person ID Must Be A Positive Integer.")]
        public int IdPerson { get; set; }
        [Required(ErrorMessage = "First Name Is Required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name Must Be Between 2 And 50 Characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name Is Required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name Must Be Between 2 And 50 Characters.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Birth Date Is Required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Person), nameof(ValidateBirthDate))]
        public DateTime BirthOfDate { get; set; }
        [Required(ErrorMessage = "Phone Number Is Required.")]
        [Phone(ErrorMessage = "Invalid Phone Number Format.")]
        public string PhoneNumber { get; set; }

        // Constructor
        public Person(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber)
        {
            IdPerson = idPerson;
            FirstName = firstName;
            LastName = lastName;
            BirthOfDate = birthOfDate;
            PhoneNumber = phoneNumber;
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
    }
}