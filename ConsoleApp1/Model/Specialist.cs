namespace ConsoleApp1.Model 
{
    public class Specialist : Employee, SerializableObject<Specialist>
    {
        public string FieldOfExpertise { get; set; }

        // Constructor
        public Specialist(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber,
                          int idEmployee, WorkDetails workDetails, string fieldOfExpertise, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
        {
            if (string.IsNullOrWhiteSpace(fieldOfExpertise))
                throw new ArgumentException("Field Of Expertise Cannot Be Empty.");


            FieldOfExpertise = fieldOfExpertise;
            InstanceCollection.Add(this);
        }

        // Method To Design A New Recipes
        public void DesignNewRecipes()
        {
            Console.WriteLine($"Specialist {FirstName} {LastName} Is Designing New Recipes In The Field of {FieldOfExpertise}.");
        }
    }
}