namespace ConsoleApp1.Models
{
    public class Experienced : Employee, SerializableObject<Experienced>
    {
        public int YearsOfExperience { get; set; }

        // Constructor
        public Experienced(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber,
                           int idEmployee, WorkDetails workDetails, int yearsOfExperience, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
            {
                if (yearsOfExperience <= 0)
                    throw new ArgumentException("Years Of Experience Must Be Negative.");


                YearsOfExperience = yearsOfExperience;
                InstanceCollection.Add(this);
            }

        // Method To Mentor A Trainee
        public void MentorTrainee(Trainee trainee)
        {
            if (trainee == null)
                throw new ArgumentNullException("Trainee Cannot Be Null.");

                
            Console.WriteLine($"Experienced {FirstName} {LastName} Is Mentoring Trainee {trainee.FirstName}.");
        }
    }
}