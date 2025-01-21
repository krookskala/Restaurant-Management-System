using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Specialist : SerializableObject<Specialist>
    {
        private static readonly List<Specialist> _specialistExtent = new List<Specialist>();
        public static IReadOnlyCollection<Specialist> SpecialistExtent => _specialistExtent.AsReadOnly();

        public string FieldOfExpertise { get; private set; }
        public Guid AssociationId { get; private set; } // Unique identifier for association

        // Event for logging when a new recipe is designed
        public event Action<string>? OnRecipeDesigned;

        // Constructor
        public Specialist(string fieldOfExpertise)
        {
            if (string.IsNullOrWhiteSpace(fieldOfExpertise))
                throw new ArgumentException("Field Of Expertise cannot be empty.");

            FieldOfExpertise = fieldOfExpertise;
            AssociationId = Guid.NewGuid();
            _specialistExtent.Add(this);
        }

        // Method to design new recipes
        public void DesignNewRecipes()
        {
            string message = $"Specialist in {FieldOfExpertise} is designing new recipes.";
            OnRecipeDesigned?.Invoke(message);
        }

        // Static method to clear extent
        public static void ClearExtent()
        {
            Console.WriteLine($"Clearing all {SpecialistExtent.Count} specialists.");
            _specialistExtent.Clear();
        }

        // Static method to remove a specialist
        public static bool RemoveSpecialist(Specialist specialist)
        {
            bool result = _specialistExtent.Remove(specialist);
            if (result)
                Console.WriteLine($"Specialist with expertise in {specialist.FieldOfExpertise} removed.");
            return result;
        }

        // Override ToString 
        public override string ToString()
        {
            return $"Specialist [Field Of Expertise: {FieldOfExpertise}, Association ID: {AssociationId}]";
        }
    }
}
