using ConsoleApp1.Services;
 using System.ComponentModel.DataAnnotations;
 using ConsoleApp1.Validation;
 
 namespace ConsoleApp1.Models
 {
     public abstract class Chef : SerializableObject<Chef>
     {
         private readonly List<string> _preparedDishes = new List<string>();
 
         [Required(ErrorMessage = "Chef ID is required.")]
         [Range(1, int.MaxValue, ErrorMessage = "Chef ID must be a positive integer.")]
         public int IdChef { get; private set; } 
 
         [Required(ErrorMessage = "Cuisine type is required.")]
         [StringLength(50, MinimumLength = 3, ErrorMessage = "Cuisine type must be between 3 and 50 characters.")]
         public string? CuisineType { get; private set; } 
 
         [NonDefaultDate(ErrorMessage = "Date of Joining is required.")]
         [DataType(DataType.Date)]
         public DateTime DateOfJoining { get; private set; } 
 
         // Derived Attribute: Years Since Joining
         public int YearsSinceJoining => DateTime.Now.Year - DateOfJoining.Year - 
                                         (DateTime.Now < DateOfJoining.AddYears(DateTime.Now.Year - DateOfJoining.Year) ? 1 : 0);

         
         // Relationship with multiple Restaurants 
         private readonly List<Restaurant> _associatedRestaurants = new();
         public IReadOnlyList<Restaurant> AssociatedRestaurants => _associatedRestaurants.AsReadOnly();
 
         public Chef(int idChef, string cuisineType, DateTime dateOfJoining)
         {
             if (dateOfJoining > DateTime.Now)
                 throw new ArgumentException("Date of Joining cannot be in the future.");
 
             IdChef = idChef;
             CuisineType = cuisineType;
             DateOfJoining = dateOfJoining;
         }
 
         public void AddRestaurant(Restaurant restaurant)
         {
             if (restaurant == null)
                 throw new ArgumentNullException(nameof(restaurant));
 
             if (!_associatedRestaurants.Contains(restaurant))
                 _associatedRestaurants.Add(restaurant);
         }
 
         public void RemoveRestaurant(Restaurant restaurant)
         {
             if (restaurant == null)
                 throw new ArgumentNullException(nameof(restaurant));
 
             _associatedRestaurants.Remove(restaurant);
         }
 
         // Event for Logging Actions
         public event Action<string>? OnActionLogged;
 
         // Method to Log Actions
         protected void LogAction(string action)
         {
             OnActionLogged?.Invoke($"Chef {IdChef}: {action}"); 
         }
 
         // Method to Add a Dish to the Prepared List
         public void AddPreparedDish(string dishName)
         {
             if (string.IsNullOrWhiteSpace(dishName))
                 throw new ArgumentException("Dish name cannot be null or empty.", nameof(dishName));
 
             _preparedDishes.Add(dishName);
             LogAction($"Prepared dish '{dishName}' added to the list.");
         }
 
         // Method to List All Prepared Dishes
         public void ListPreparedDishes()
         {
             if (_preparedDishes.Count == 0)
             {
                 LogAction("No dishes prepared yet.");
             }
             else
             {
                 LogAction($"Listing all dishes prepared by Chef {IdChef}:");
                 foreach (var dish in _preparedDishes)
                 {
                     Console.WriteLine($"- {dish}");
                 }
             }
         }
 
         // Method to Clear Prepared Dishes
         public void ClearPreparedDishes()
         {
             _preparedDishes.Clear();
             LogAction("All prepared dishes cleared.");
         }
 
         // Abstract Method To Be Implemented By Subclasses
         public abstract void PrepareSpecialDish();
     }
 
     // Subclass: ExecutiveChef
     public class ExecutiveChef : Chef
     {
         [Required(ErrorMessage = "Kitchen experience is required.")]
         [Range(1, 50, ErrorMessage = "Kitchen experience must be between 1 and 50 years.")]
         public int KitchenExperience { get; private set; } 
 
         public event Action<string>? OnTrainingEvent;
 
         public ExecutiveChef(int idChef, string cuisineType, int kitchenExperience, DateTime dateOfJoining)
             : base(idChef, cuisineType, dateOfJoining)
         {
             KitchenExperience = kitchenExperience;
         }
 
         // Implement Abstract Method
         public override void PrepareSpecialDish()
         {
             LogAction($"Executive Chef {IdChef} is preparing a signature dish.");
         }
 
         // Event-based Logging for Training Sous Chef
         public void TrainSousChef()
         {
             OnTrainingEvent?.Invoke($"Executive Chef {IdChef} is training a Sous Chef on {DateTime.Now}.");
         }
     }
 
     // Subclass: SousChef
     public class SousChef : Chef
     {
         [Required(ErrorMessage = "Supervised sections are required.")]
         [StringLength(50, MinimumLength = 3, ErrorMessage = "Supervised sections must be between 3 and 50 characters.")]
         public string SupervisedSections { get; private set; } 
 
         // Event for Logging
         public event Action<string>? OnSectionManaged;
         public event Action<string>? OnDishPrepared;
 
         public SousChef(int idChef, string cuisineType, string supervisedSections, DateTime dateOfJoining)
             : base(idChef, cuisineType, dateOfJoining)
         {
             SupervisedSections = supervisedSections;
         }
 
         public void ManageSection()
         {
             OnSectionManaged?.Invoke($"Sous Chef {IdChef} is managing the section: {SupervisedSections}.");
         }
 
         public override void PrepareSpecialDish()
         {
             OnDishPrepared?.Invoke($"Sous Chef {IdChef} is preparing a dish under the guidance of the Executive Chef.");
         }
     }
 
     // Subclass: LineChef
     public class LineChef : Chef
     {
         [Required(ErrorMessage = "Specialization is required.")]
         [StringLength(50, MinimumLength = 3, ErrorMessage = "Specialization must be between 3 and 50 characters.")]
         public string Specialization { get; private set; } 
 
         // Event for Logging
         public event Action<string>? OnDishPrepared;
 
         public LineChef(int idChef, string cuisineType, string specialization, DateTime dateOfJoining)
             : base(idChef, cuisineType, dateOfJoining)
         {
             Specialization = specialization;
         }
 
         public override void PrepareSpecialDish()
         {
             OnDishPrepared?.Invoke($"Line Chef {IdChef} is preparing a specialty dish in {Specialization}.");
         }
     }
 }