using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Menu : SerializableObject<Menu>
    {
        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Menu name must be between 2 and 100 characters.")]
        public string Name { get; private set; }

        [Required(ErrorMessage = "Menu type is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Menu type must be between 3 and 50 characters.")]
        public string MenuType { get; private set; }

        private readonly List<string> _availableLanguages;
        private readonly List<Dish> _dishes;
        private readonly Dictionary<string, Dish> _qualifiedDishes = new();

        public string DefaultLanguage { get; private set; }

        // Events for Logging
        public event Action<string>? OnLanguageAdded;
        public event Action<string>? OnLanguageRemoved;
        public event Action<string>? OnDishAdded;
        public event Action<string>? OnDishRemoved;

        // Constructor
        public Menu(string name, string menuType, List<string> availableLanguages)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Menu name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(menuType))
                throw new ArgumentException("Menu type cannot be empty.", nameof(menuType));
            if (availableLanguages == null || availableLanguages.Count == 0)
                throw new ArgumentException("Available languages cannot be null or empty.", nameof(availableLanguages));

            Name = name;
            MenuType = menuType;
            _availableLanguages = new List<string>(availableLanguages);
            DefaultLanguage = _availableLanguages[0]; 
            _dishes = new List<Dish>();
        }

        // Read-only properties for encapsulation
        public IReadOnlyList<string> AvailableLanguages => _availableLanguages.AsReadOnly();
        public IReadOnlyList<Dish> Dishes => _dishes.AsReadOnly();

        // Derived Method for Vegetarian Dishes
        public IReadOnlyList<Dish> GetVegetarianDishes()
        {
            return _dishes.Where(dish => dish.IsVegetarian).ToList().AsReadOnly();
        }

        // Derived Method for Dishes by Cuisine
        public IReadOnlyList<Dish> GetDishesByCuisine(string cuisine)
        {
            return _dishes
                .Where(dish => dish.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .AsReadOnly();
        }

        // Method to Add a Language
        public void AddLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language cannot be empty.", nameof(language));

            if (!_availableLanguages.Any(l => l.Equals(language, StringComparison.OrdinalIgnoreCase)))
            {
                _availableLanguages.Add(language);
                OnLanguageAdded?.Invoke($"Language '{language}' added to menu '{Name}'.");
            }
            else
            {
                LogAction($"Attempt to add duplicate language '{language}' to menu '{Name}'.");
            }
        }

        // Method to Remove a Language
        public void RemoveLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language cannot be empty.", nameof(language));

            if (_availableLanguages.RemoveAll(l => l.Equals(language, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                OnLanguageRemoved?.Invoke($"Language '{language}' removed from menu '{Name}'.");
                if (DefaultLanguage.Equals(language, StringComparison.OrdinalIgnoreCase))
                {
                    DefaultLanguage = _availableLanguages.FirstOrDefault() ?? string.Empty;
                }
            }
        }

        // Method to Set Default Language
        public void SetDefaultLanguage(string language)
        {
            if (!_availableLanguages.Contains(language, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Language '{language}' is not available in the menu.");

            DefaultLanguage = language;
        }

        // Method to Add a Dish
        public void AddDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentNullException(nameof(dish), "Dish cannot be null.");

            if (_dishes.Any(d => d.Name.Equals(dish.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Dish '{dish.Name}' already exists in menu '{Name}'.");

            dish.AssignMenu(this); // Reverse connection
            _dishes.Add(dish);
            OnDishAdded?.Invoke($"Dish '{dish.Name}' added to menu '{Name}'.");
        }
        public void RemoveDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentNullException(nameof(dish), "Dish cannot be null.");

            if (!_dishes.Remove(dish))
                throw new InvalidOperationException($"Dish '{dish.Name}' not found in menu '{Name}'.");

            dish.UnassignMenu(this); 
            OnDishRemoved?.Invoke($"Dish '{dish.Name}' removed from menu '{Name}'.");
            LogAction($"Dish '{dish.Name}' removed from menu '{Name}'.");
        }

        // Method to Add a Qualified Dish
        public void AddQualifiedDish(string qualifier, Dish dish)
        {
            if (string.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Qualifier cannot be null or empty.");

            if (_qualifiedDishes.ContainsKey(qualifier))
                throw new InvalidOperationException($"A dish with qualifier '{qualifier}' already exists in menu '{Name}'.");

            if (_qualifiedDishes.Values.Contains(dish))
                throw new InvalidOperationException($"Dish '{dish.Name}' is already added as a qualified dish in menu '{Name}'.");

            _qualifiedDishes[qualifier] = dish;
            LogAction($"Qualified dish added with qualifier '{qualifier}' for menu '{Name}'.");
        }

        // Method to Remove a Qualified Dish
        public void RemoveQualifiedDish(string qualifier)
        {
            if (!_qualifiedDishes.Remove(qualifier))
            {
                LogAction($"Failed attempt to remove non-existent qualifier '{qualifier}' for menu '{Name}'.");
                throw new InvalidOperationException($"No dish found with qualifier '{qualifier}' for menu '{Name}'.");
            }

            LogAction($"Qualified dish with qualifier '{qualifier}' removed for menu '{Name}'.");
        }

        public Dish GetQualifiedDish(string qualifier)
        {
            if (string.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Qualifier cannot be null or empty.", nameof(qualifier));

            if (!_qualifiedDishes.TryGetValue(qualifier, out var dish))
                throw new InvalidOperationException($"No dish found with qualifier '{qualifier}' in menu '{Name}'.");

            return dish;
        }

        public void HandleEmptyMenu()
        {
            if (_dishes.Count == 0)
                LogAction($"Menu '{Name}' is now empty. Consider adding dishes.");
        }

        // Method to List Dishes
        public string ListDishes()
        {
            return string.Join(", ", _dishes.Select(dish => dish.Name));
        }

        private void LogAction(string message)
        {
            Console.WriteLine(message); 
        }

        public override bool Equals(object? obj)
        {
            return obj is Menu other && Name == other.Name && MenuType == other.MenuType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, MenuType);
        }

        public override string ToString()
        {
            return $"Menu [Name: {Name}, Type: {MenuType}, Dishes Count: {_dishes.Count}]";
        }
    }
}