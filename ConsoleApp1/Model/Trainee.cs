namespace ConsoleApp1.Model
{
    public class Trainee : Employee, SerializableObject<Trainee>
    {
        public int TrainingDuration { get; set; }

        // Constructor
        public Trainee(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber,
                       int idEmployee, WorkDetails workDetails, int trainingDuration, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
        {
            if (trainingDuration <= 0)
                throw new ArgumentException("Training Duration Must Be Greater Than Zero.");

                
            TrainingDuration = trainingDuration;
            InstanceCollection.Add(this);
        }

        // Method To Complete Training
        public void CompleteTraining()
        {
            Console.WriteLine($"Trainee {FirstName} {LastName} Has Completed {TrainingDuration} weeks of training.");
        }
    }
}