using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Experienced : SerializableObject<Experienced>
    {
        private static readonly List<Experienced> _experiencedExtent = new List<Experienced>();
        public static IReadOnlyCollection<Experienced> ExperiencedExtent => _experiencedExtent.AsReadOnly();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Years of experience must be a positive integer.")]
        public int YearsOfExperience { get; private set; }

        public Guid AssociationId { get; private set; }

        // Event for Logging
        public event Action<string>? OnMentorshipStarted;

        // Constructor
        public Experienced(int yearsOfExperience)
        {
            if (yearsOfExperience <= 0)
                throw new ArgumentException("Years of experience must be greater than zero.", nameof(yearsOfExperience));

            YearsOfExperience = yearsOfExperience;
            AssociationId = Guid.NewGuid();

            _experiencedExtent.Add(this);
        }

        // Method to Mentor a Trainee
        public void MentorTrainee(Trainee trainee)
        {
            if (trainee == null)
                throw new ArgumentNullException(nameof(trainee), "Trainee cannot be null.");

            string message = $"Experienced mentor with {YearsOfExperience} years of experience is mentoring trainee.";
            OnMentorshipStarted?.Invoke(message);
        }

        // Static Method to Clear Extent
        public static void ClearExtent()
        {
            Console.WriteLine($"Clearing all {ExperiencedExtent.Count} experienced mentors.");
            _experiencedExtent.Clear();
        }

        // Static Method to Remove Experienced Mentor
        public static bool RemoveExperienced(Experienced experienced)
        {
            bool result = _experiencedExtent.Remove(experienced);
            if (result)
                Console.WriteLine($"Experienced mentor with {experienced.YearsOfExperience} years of experience removed.");
            return result;
        }

        public override string ToString()
        {
            return $"Experienced [Years of Experience: {YearsOfExperience}, Association ID: {AssociationId}]";
        }
    }
}