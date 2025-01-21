namespace ConsoleApp1.Model
{
    public abstract class Chef : Employee, SerializableObject<Chef>
    {
        public string CuisineType { get; set; }

        // Constructor
        public Chef(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                    int idEmployee, WorkDetails workDetails, string cuisineType, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
        {
            CuisineType = cuisineType;
            InstanceCollection.Add(this);
        }

        // Abstract Method To Be Implemented By Subclasses
        public abstract void PrepareSpecialDish();
    }

    // Subclass: ExecutiveChef
    public class ExecutiveChef : Chef
    {
        public int KitchenExperience { get; set; }

        public ExecutiveChef(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                             int idEmployee, WorkDetails workDetails, string cuisineType, int kitchenExperience, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, cuisineType, dateOfLeaving)
        {
            KitchenExperience = kitchenExperience;
        }

        // Implement Abstract Method
        public override void PrepareSpecialDish()
        {
            Console.WriteLine($"Executive Chef {FirstName} {LastName} Is Preparing Signature Dish.");
        }

        public void TrainSousChef()
        {
            Console.WriteLine($"Executive Chef {FirstName} {LastName} Is Training Sous Chef.");
        }
    }

    // Subclass: SousChef
    public class SousChef : Chef
    {
        public string SupervisedSections { get; set; }

        public SousChef(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                        int idEmployee, WorkDetails workDetails, string cuisineType, string supervisedSections, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, cuisineType, dateOfLeaving)
        {
            SupervisedSections = supervisedSections;
        }

        public override void PrepareSpecialDish()
        {
            Console.WriteLine($"Sous Chef {FirstName} {LastName} Is Preparing A Dish Under The Guidance Of The ExecutiveChef.");
        }
    }

    // Subclass: LineChef
    public class LineChef : Chef
    {
        public string Specialization { get; set; }

        public LineChef(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                        int idEmployee, WorkDetails workDetails, string cuisineType, string specialization, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, cuisineType, dateOfLeaving)
        {
            Specialization = specialization;
        }

        public override void PrepareSpecialDish()
        {
            Console.WriteLine($"Line Chef {FirstName} {LastName} Is Preparing A Speciality Dish In {Specialization}.");
        }
    }
}