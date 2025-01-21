using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Models{
    public abstract class Employee : Person, SerializableObject<Employee>
    {
        [Required(ErrorMessage = "Employee ID Is Required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID Must Be A Positive Integer.")]
        public int IdEmployee { get; set; }
        public WorkDetails WorkDetails { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfLeaving { get; set; }

        // Constructor
        public Employee(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                        int idEmployee, WorkDetails workDetails, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber)
        {
            if (workDetails == null)
                throw new ArgumentNullException("Work Details Cannot Be Null.");


            IdEmployee = idEmployee;
            WorkDetails = workDetails;
            DateOfLeaving = dateOfLeaving;
            InstanceCollection.Add(this);
        }

        // Method To Set Date Of Leaving
        public void setDateOfLeaving(DateTime dateOfLeaving)
        {
            if (dateOfLeaving < WorkDetails.DateOfHiring)
                throw new ArgumentException("Date Of Leaving Cannot Be Before Date Of Hiring.");

            DateOfLeaving = dateOfLeaving;
            Console.WriteLine($"Date Of Leaving Updated To: {DateOfLeaving}.");
        }

        // Method To Get Total Employment Duration
        public int GetEmploymentDuration()
        {   
            DateTime endDate = DateOfLeaving ?? DateTime.Now;
            return (endDate - WorkDetails.DateOfHiring).Days;
        }

        public override string GetDetails()
        {
            return $"Employee ID: {IdEmployee}\nName: {FirstName} {LastName}\nBirth Date: {BirthOfDate}\nDepartment: {WorkDetails.Department}";
        }
    }

    // Complex Attribute: WorkDetails
    public class WorkDetails
    {
        public DateTime DateOfHiring { get; private set; }
        public string Department { get; private set; }
        public string ShiftSchedule { get; private set; }

        // Constructor
        public WorkDetails(DateTime dateOfHiring, string department, string shiftSchedule)
        {
            if (dateOfHiring > DateTime.Now)
                throw new ArgumentException("Date Of Hiring Cannot Be In The Future.");
            
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department Cannot Be Empty.");

            if (string.IsNullOrWhiteSpace(shiftSchedule))
                throw new ArgumentException("Shift Schedule Cannot Be Empty.");


            DateOfHiring = dateOfHiring;
            Department = department;
            ShiftSchedule = shiftSchedule;
        }

        // Method To Update Department
        public void UpdateDepartment(string newDepartment)
        {
            if (string.IsNullOrWhiteSpace(newDepartment))
                throw new ArgumentException("Department Cannot Be Empty.");

            Department = newDepartment;
            Console.WriteLine($"Department Updated To: {Department}.");
        }

        // Method To Update Shift Schedule
        public void UpdateShiftSchedule(string newShiftSchedule)
        {
            if (string.IsNullOrWhiteSpace(newShiftSchedule))
                throw new ArgumentException("Shift Schedule Cannot Be Empty.");

            ShiftSchedule = newShiftSchedule;
            Console.WriteLine($"Shift Schedule Updated To: {ShiftSchedule}.");
        }

        // Method To Get Work Duration
        public int GetWorkDuration()
        {
            return (DateTime.Now - DateOfHiring).Days;
        }
    }
}