using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Valet : SerializableObject<Valet>
    {
        [Required(ErrorMessage = "Valet ID Is Required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Valet ID Must Be A Positive Integer.")]
        public int IdValet { get; private set; }   

        [Required(ErrorMessage = "Assigned Location Is Required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Assigned Location Must Be Between 3 And 100 Characters.")]
        public string AssignedLocation { get; private set; } 

        // Derived Attribute
        public string LocationSummary => $"Valet {IdValet} is at {AssignedLocation}.";

        // Event for Logging
        public event Action<string>? OnCarParked;

        // Structured Logger Placeholder
        private readonly Action<string> _logger;

        public Valet() : this(1, "Default Garage", Console.WriteLine)
        {
        }

        public Valet(int idValet, string assignedLocation, Action<string> logger = null!)
        {
            if (string.IsNullOrWhiteSpace(assignedLocation) || assignedLocation.Length < 3 || assignedLocation.Length > 100)
                throw new ArgumentException("Assigned Location must be between 3 and 100 characters.");

            IdValet = idValet;
            AssignedLocation = assignedLocation;
            _logger = logger ?? Console.WriteLine;
        }

        // Method
        public void ParkCar()
        {
            var message = $"Valet {IdValet} is parking a car at {AssignedLocation}.";
            OnCarParked?.Invoke(message);
            LogAction(message);
        }

        private void LogAction(string message)
        {
            _logger.Invoke(message);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Valet other)
            {
                return IdValet == other.IdValet && AssignedLocation == other.AssignedLocation;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdValet, AssignedLocation);
        }

        public override string ToString()
        {
            return $"Valet {IdValet} Assigned To {AssignedLocation}";
        }
    }
}