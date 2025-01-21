using System.ComponentModel.DataAnnotations;
using ConsoleApp1.Services;

namespace ConsoleApp1.Models
{
    public class Dish : SerializableObject<Dish>
    {
        private readonly List<OrderDish> _orderDishes = new();
        private readonly object _lock = new();
        private static readonly List<Dish> DishList = new();
        public static IReadOnlyCollection<Dish> Dishes => DishList.AsReadOnly();

        [Required(ErrorMessage = "Dish name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Dish name must be between 2 and 100 characters.")]
        public string Name { get; private set; }

        [Required(ErrorMessage = "Cuisine type is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Cuisine type must be between 3 and 50 characters.")]
        public string Cuisine { get; private set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; private set; }
        public bool IsVegetarian { get; private set; }

        public List<string> Ingredients { get; private set; }

        // Events for Logging
        public event Action<string>? OnIngredientAdded;
        public event Action<string>? OnIngredientRemoved;
        public event Action<string>? OnOrderDishAdded;
        public event Action<string>? OnOrderDishRemoved;
        private Menu? _assignedMenu;

        private readonly Dictionary<string, OrderDish> _qualifiedOrderDishes = new(); // Qualified Association

        // Constructor
        public Dish(string name, string cuisine, decimal price, bool isVegetarian, List<string> ingredients)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Dish name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(cuisine))
                throw new ArgumentException("Cuisine type cannot be empty.", nameof(cuisine));
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            if (ingredients == null || ingredients.Count == 0)
                throw new ArgumentException("Ingredients cannot be null or empty.", nameof(ingredients));

            Name = name;
            Cuisine = cuisine;
            Price = price;
            IsVegetarian = isVegetarian;
            Ingredients = new List<string>(ingredients.Distinct());

            if (IsDishDuplicate(name, cuisine))
                throw new InvalidOperationException($"Dish '{name}' with cuisine '{cuisine}' already exists.");

            DishList.Add(this);
        }

        // Method AddOrderDish
        public void AddOrderDish(OrderDish orderDish)
        {
            if (orderDish == null)
                throw new ArgumentNullException(nameof(orderDish));

            lock (_lock)
            {
                if (_orderDishes.Any(od => od == orderDish))
                    throw new InvalidOperationException($"OrderDish for Dish '{Name}' already exists.");

                _orderDishes.Add(orderDish);
                LogAction($"OrderDish added for Dish '{Name}' with quantity {orderDish.Quantity}.");
                OnOrderDishAdded?.Invoke($"OrderDish added for Dish '{Name}' with quantity {orderDish.Quantity}.");
            }
        }

        
        // Method RemoveOrderDish
        public void RemoveOrderDish(OrderDish orderDish)
        {
            if (orderDish == null)
                throw new ArgumentNullException(nameof(orderDish));

            lock (_lock)
            {
                if (!_orderDishes.Remove(orderDish))
                    throw new InvalidOperationException($"OrderDish not found for Dish '{Name}'.");

                LogAction($"Removed OrderDish for Dish '{Name}'.");
                OnOrderDishRemoved?.Invoke($"OrderDish removed for Dish '{Name}'.");
            }
        }

        // Edit Method for OrderDish
        public void EditOrderDish(OrderDish oldOrderDish, OrderDish newOrderDish)
        {
            if (oldOrderDish == null || newOrderDish == null)
                throw new ArgumentNullException("OrderDish cannot be null.");

            RemoveOrderDish(oldOrderDish);
            AddOrderDish(newOrderDish);
            LogAction($"OrderDish for Dish '{Name}' updated from '{oldOrderDish.Dish.Name}' to '{newOrderDish.Dish.Name}'.");
        }

        // Method to Add a Qualified OrderDish
        public void AddQualifiedOrderDish(string qualifier, OrderDish orderDish)
        {
            if (string.IsNullOrWhiteSpace(qualifier))
                throw new ArgumentException("Qualifier cannot be null or empty.");

            lock (_lock)
            {
                if (_qualifiedOrderDishes.ContainsKey(qualifier))
                    throw new InvalidOperationException($"An OrderDish with qualifier '{qualifier}' already exists for Dish '{Name}'.");

                _qualifiedOrderDishes[qualifier] = orderDish;
                LogAction($"Qualified OrderDish added with qualifier '{qualifier}' for Dish '{Name}'.");
            }
        }

        // Method to Remove a Qualified OrderDish
        public void RemoveQualifiedOrderDish(string qualifier)
        {
            lock (_lock)
            {
                if (!_qualifiedOrderDishes.Remove(qualifier))
                    throw new InvalidOperationException($"No OrderDish found with qualifier '{qualifier}' for Dish '{Name}'.");

                LogAction($"Qualified OrderDish with qualifier '{qualifier}' removed for Dish '{Name}'.");
            }
        }

        public void AssignMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            if (_assignedMenu != null)
                throw new InvalidOperationException($"Dish '{Name}' is already assigned to menu '{_assignedMenu.Name}'.");

            _assignedMenu = menu;
            menu.AddDish(this); // Establish reverse connection
            LogAction($"Dish '{Name}' assigned to Menu '{menu.Name}'.");
        }

        // Method to Unassign Menu
        public void UnassignMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            if (_assignedMenu != menu)
                throw new InvalidOperationException($"Dish '{Name}' is not assigned to menu '{menu.Name}'.");

            menu.RemoveDish(this); // Reverse connection cleanup
            _assignedMenu = null;
            LogAction($"Dish '{Name}' unassigned from Menu '{menu.Name}'.");
        }

        // Static Validation for Duplicates
        public static bool IsDishDuplicate(string name, string cuisine)
        {
            return DishList.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                     d.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase));
        }

        // Method to Add an Ingredient
        public void AddIngredient(string ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
                throw new ArgumentException("Ingredient cannot be empty.", nameof(ingredient));

            var normalizedIngredient = ingredient.ToLowerInvariant();

            lock (_lock)
            {
                if (Ingredients.Any(i => i.Equals(normalizedIngredient, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"Ingredient '{ingredient}' already exists in Dish '{Name}'");
                Ingredients.Add(ingredient);
                OnIngredientAdded?.Invoke($"Ingredient '{ingredient}' added to Dish '{Name}'.");
                LogAction($"Ingredient '{ingredient}' added to Dish '{Name}'.");
            }
        }

        // Destructor for Composition Cleanup
        ~Dish()
        {
            lock (_lock)
            {
                foreach (var orderDish in _orderDishes.ToList())
                {
                    orderDish.RemoveFromDish();
                }
                _orderDishes.Clear();
                LogAction($"Dish '{Name}' deleted along with all associated OrderDishes.");
            }
        }

        // Remaining Methods: RemoveIngredient, AddIngredients, RemoveIngredients, ListIngredients
        public void RemoveIngredient(string ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
                throw new ArgumentException("Ingredient cannot be empty.", nameof(ingredient));

            var normalizedIngredient = ingredient.ToLowerInvariant();

            lock (_lock)
            {
                var toRemove = Ingredients.FirstOrDefault(i => i.Equals(normalizedIngredient, StringComparison.OrdinalIgnoreCase));
                if (toRemove == null)
                    throw new InvalidOperationException($"Ingredient '{ingredient}' not found in Dish '{Name}'");

                Ingredients.Remove(toRemove); 
                OnIngredientRemoved?.Invoke($"Ingredient '{toRemove}' removed from Dish '{Name}'.");
                LogAction($"Ingredient '{toRemove}' removed from Dish '{Name}'.");
            }
        }
        
        public string ListIngredients()
        {
            lock (_lock)
            {
                return string.Join(", ", Ingredients.Select(i => i.ToLowerInvariant()));
            }
        }

        private void LogAction(string message)
        {
            Console.WriteLine(message); 
        }

        public override bool Equals(object? obj)
        {
            return obj is Dish other && Name == other.Name && Cuisine == other.Cuisine;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Cuisine);
        }

        public override string ToString()
        {
            return $"Dish [Name: {Name}, Cuisine: {Cuisine}, Vegetarian: {IsVegetarian}, Ingredients: {Ingredients.Count}]";
        }
    }
}