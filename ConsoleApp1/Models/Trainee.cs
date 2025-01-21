using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Trainee : SerializableObject<Trainee>
    {
        private static readonly List<Trainee> _traineeExtent = new List<Trainee>();
        public static IReadOnlyCollection<Trainee> TraineeExtent => _traineeExtent.AsReadOnly();

        public int TrainingDuration { get; private set; }
        public WorkDetails WorkDetails { get; private set; }
        public Guid AssociationId { get; private set; }
        public DateTime TrainingEndDate => WorkDetails.DateOfHiring.AddDays(TrainingDuration * 7);

        public event Action<string>? OnTrainingCompleted;

        // Constructor
        public Trainee(int trainingDuration, WorkDetails workDetails)
        {
            if (trainingDuration <= 0 || trainingDuration > 52)
                throw new ArgumentException("Training Duration must be between 1 and 52 weeks.");

            TrainingDuration = trainingDuration;
            WorkDetails = workDetails ?? throw new ArgumentNullException(nameof(workDetails));
            AssociationId = Guid.NewGuid();

            _traineeExtent.Add(this);

            // Establish reverse association
            WorkDetails.AddTrainee(this);
        }

        public void CompleteTraining()
        {
            string message = $"Trainee with Hiring Date {WorkDetails.DateOfHiring} has completed {TrainingDuration} weeks of training.";
            OnTrainingCompleted?.Invoke(message);
        }

        public static void ClearExtent()
        {
            _traineeExtent.Clear();
        }

        public static bool RemoveTrainee(Trainee trainee)
        {
            // Remove reverse association
            trainee.WorkDetails.RemoveTrainee(trainee);
            return _traineeExtent.Remove(trainee);
        }
    }
}
