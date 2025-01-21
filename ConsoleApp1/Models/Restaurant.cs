using ConsoleApp1.Services;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Models
{
    public class Restaurant : SerializableObject<Restaurant>
    {
        // Static Collection for Extent Management
        private static readonly List<Restaurant> _restaurantExtent = new List<Restaurant>();
        public static IReadOnlyCollection<Restaurant> RestaurantExtent => _restaurantExtent.AsReadOnly();

        [Required(ErrorMessage = "Restaurant name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Restaurant name must be between 2 and 100 characters.")]
        public string Name { get; private set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max capacity must be a positive integer.")]
        public int MaxCapacity { get; private set; }

        private readonly List<Table> _tables = new();
        public IReadOnlyList<Table> Tables => _tables.AsReadOnly();

        // Derived Attribute
        public int AvailableCapacity => MaxCapacity - _tables.Count;

        // Events for Logging
        public event Action<string>? OnTableAdded;
        public event Action<string>? OnTableRemoved;

        // Constructor
        public Restaurant(string name, int maxCapacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant Name Cannot Be Empty.", nameof(name));

            if (maxCapacity <= 0)
                throw new ArgumentException("Max Capacity Must Be Greater Than Zero.", nameof(maxCapacity));

            Name = name;
            MaxCapacity = maxCapacity;
            _restaurantExtent.Add(this); 
        }

        // Method to Get the Number of Tables
        public int GetNumberOfTables()
        {
            return _tables.Count;
        }

        // Method to Add a Table
        public void AddTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Table cannot be null.");

            if (_tables.Count >= MaxCapacity)
                throw new InvalidOperationException($"Cannot add more tables. Max capacity of {MaxCapacity} reached.");

            if (_tables.Any(t => t.IdTable == table.IdTable))
                throw new InvalidOperationException($"Table with ID {table.IdTable} already exists.");

            _tables.Add(table);
            OnTableAdded?.Invoke($"Table {table.IdTable} added to restaurant {Name}.");
        }

        // Method to Remove a Table
        public void RemoveTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Table cannot be null.");

            if (_tables.Remove(table))
            {
                OnTableRemoved?.Invoke($"Table {table.IdTable} removed from restaurant {Name}.");
            }
            else
            {
                throw new InvalidOperationException($"Table {table.IdTable} does not exist in restaurant {Name}.");
            }
        }

        // Method to Get the Current Capacity
        public int GetCurrentCapacity()
        {
            return _tables.Sum(t => t.NumberOfChairs);
        }

        // Static Method to Clear Extent
        public static void ClearExtent()
        {
            _restaurantExtent.Clear();
        }

        // Static Method to Remove a Restaurant
        public static bool RemoveRestaurant(Restaurant restaurant)
        {
            return _restaurantExtent.Remove(restaurant);
        }

         public IEnumerable<Reservation> GetReservations()
         {
         return Tables.SelectMany(table => table.Reservations);
         }

        public Table? GetMostReservedTable()
        {
        return Tables.OrderByDescending(table => table.Reservations.Count).FirstOrDefault();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Restaurant other)
            {
                return Name == other.Name && MaxCapacity == other.MaxCapacity;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, MaxCapacity);
        }

        public override string ToString()
        {
            return $"Restaurant [Name: {Name}, Max Capacity: {MaxCapacity}, Tables Count: {GetNumberOfTables()}, Available Capacity: {AvailableCapacity}]";
        }
    }
}
