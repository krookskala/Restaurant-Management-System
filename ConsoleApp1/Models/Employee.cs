using ConsoleApp1.Services;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Models
{
    public abstract class Employee : SerializableObject<Employee>
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID must be a positive integer.")]
        public int IdEmployee { get; private set; }

        public WorkDetails WorkDetails { get; private set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfLeaving { get; private set; }

        public Manager? Supervisor { get; private set; } // Reverse connection to Manager

        // Constructor
        public Employee(int idEmployee, WorkDetails workDetails, DateTime? dateOfLeaving = null)
        {
            ValidateNotNull(workDetails, nameof(workDetails));

            IdEmployee = idEmployee;
            WorkDetails = workDetails;
            DateOfLeaving = dateOfLeaving;

            // Establish reverse connection
            WorkDetails.LinkToEmployee(this);
        }

        // Method to set Date of Leaving
        public void SetDateOfLeaving(DateTime dateOfLeaving)
        {
            if (dateOfLeaving < WorkDetails.DateOfHiring)
                throw new ArgumentException("Date of Leaving cannot be before Date of Hiring.");

            DateOfLeaving = dateOfLeaving;
            Console.WriteLine($"Date of Leaving updated to: {DateOfLeaving}.");
        }

        // Method to update Work Details
        public void UpdateWorkDetails(WorkDetails newWorkDetails)
        {
            ValidateNotNull(newWorkDetails, nameof(newWorkDetails));

            // Unlink existing WorkDetails
            WorkDetails.UnlinkFromEmployee();

            // Link new WorkDetails
            WorkDetails = newWorkDetails;
            WorkDetails.LinkToEmployee(this);
        }

        // Method to set Supervisor
        public void SetSupervisor(Manager manager)
        {
            if (Supervisor != null)
                throw new InvalidOperationException("Employee already has a supervisor.");

            Supervisor = manager;
        }

        // Method to remove Supervisor
        public void RemoveSupervisor()
        {
            Supervisor = null;
        }

        // Method to get Total Employment Duration
        public int GetEmploymentDuration()
        {
            DateTime endDate = DateOfLeaving ?? DateTime.Now;
            return (endDate - WorkDetails.DateOfHiring).Days;
        }

        public override string ToString()
        {
            return $"Employee [ID: {IdEmployee}, Work Details: {WorkDetails}, Date Of Leaving: {DateOfLeaving?.ToShortDateString() ?? "N/A"}, Supervisor: {Supervisor?.IdManager.ToString() ?? "None"}]";
        }

        private void ValidateNotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }
    }



    // Complex Attribute: WorkDetails
    public class WorkDetails : SerializableObject<WorkDetails>
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfHiring { get; private set; }

        [Required]
        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
        public string Department { get; private set; }

        [Required]
        [StringLength(50, ErrorMessage = "Shift schedule cannot exceed 50 characters.")]
        public string ShiftSchedule { get; private set; }

        // Reverse connection to Trainees
        private readonly List<Trainee> _trainees = new List<Trainee>();
        public IReadOnlyCollection<Trainee> Trainees => _trainees.AsReadOnly();

        // Reverse connection to Specialists
        private readonly List<Specialist> _specialists = new List<Specialist>();
        public IReadOnlyCollection<Specialist> Specialists => _specialists.AsReadOnly();

        // Reverse connection to Employee
        public Employee? AssociatedEmployee { get; private set; }

        public WorkDetails(DateTime dateOfHiring, string department, string shiftSchedule)
        {
            if (dateOfHiring > DateTime.Now)
                throw new ArgumentException("Date of Hiring cannot be in the future.");

            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department cannot be empty.");

            if (string.IsNullOrWhiteSpace(shiftSchedule))
                throw new ArgumentException("Shift Schedule cannot be empty.");

            DateOfHiring = dateOfHiring;
            Department = department;
            ShiftSchedule = shiftSchedule;
        }

        public void LinkToEmployee(Employee employee)
        {
            if (AssociatedEmployee != null)
                throw new InvalidOperationException("WorkDetails is already linked to an Employee.");

            AssociatedEmployee = employee;
        }


        public void UnlinkFromEmployee()
        {
            if (AssociatedEmployee == null)
                throw new InvalidOperationException("WorkDetails is not linked to any Employee.");

            AssociatedEmployee = null; 
        }


        public void AddTrainee(Trainee trainee)
        {
            if (!_trainees.Contains(trainee))
            {
                _trainees.Add(trainee);
            }
        }

        public bool RemoveTrainee(Trainee trainee)
        {
            return _trainees.Remove(trainee);
        }

        public void AddSpecialist(Specialist specialist)
        {
            if (!_specialists.Contains(specialist))
            {
                _specialists.Add(specialist);
            }
        }

        public bool RemoveSpecialist(Specialist specialist)
        {
            return _specialists.Remove(specialist);
        }

        public void UpdateDepartment(string newDepartment)
        {
            if (string.IsNullOrWhiteSpace(newDepartment))
                throw new ArgumentException("Department cannot be empty.");

            Department = newDepartment;
            Console.WriteLine($"Department updated to: {Department}.");
        }

        public void UpdateShiftSchedule(string newShiftSchedule)
        {
            if (string.IsNullOrWhiteSpace(newShiftSchedule))
                throw new ArgumentException("Shift Schedule cannot be empty.");

            ShiftSchedule = newShiftSchedule;
            Console.WriteLine($"Shift Schedule updated to: {ShiftSchedule}.");
        }

    
        public int GetWorkDuration()
        {
            return (DateTime.Now - DateOfHiring).Days;
        }

        public override string ToString()
        {
            return $"Date Of Hiring: {DateOfHiring}, Department: {Department}, Shift Schedule: {ShiftSchedule}, " +
                   $"Employee: {(AssociatedEmployee != null ? AssociatedEmployee.IdEmployee.ToString() : "None")}, " +
                   $"Trainees: {Trainees.Count}, Specialists: {Specialists.Count}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DateOfHiring, Department, ShiftSchedule);
        }
    }
}