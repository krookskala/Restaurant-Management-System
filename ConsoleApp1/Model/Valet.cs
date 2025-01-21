namespace ConsoleApp1.Model
{
    public class Valet : Employee, SerializableObject<Valet>
    {
        public int IdValet { get; set; }    
        public string AssignedLocation {get; set;}

        public Valet(int idPerson, string firstName, string lastName, DateTime birthOfDate, string phoneNumber, 
                     int idEmployee, WorkDetails workDetails, int idValet, string assignedLocation, DateTime? dateOfLeaving = null)
            : base(idPerson, firstName, lastName, birthOfDate, phoneNumber, idEmployee, workDetails, dateOfLeaving)
        {
            IdValet = idValet;
            AssignedLocation = assignedLocation;
            InstanceCollection.Add(this);
        }

        public void ParkCar()
        {
            Console.WriteLine($"Valet {FirstName} Is Parking A Car At {AssignedLocation}.");
        }
    }
}