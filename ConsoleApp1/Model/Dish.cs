namespace ConsoleApp1.Model
{
    public class Dish : SerializableObject<Dish>
    {
        public string Name {get; set; }
        public string Cuisine {get; set; }
        public bool IsVegetarian {get; set; }
        public List<string> Ingredients { get; private set; } // Multi-Value Attribute

        // Constructor
        public Dish(string name, string cuisine, bool isVegetarian, List<string> ingredients)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Dish Name Cannot Be Empty.");

            if (string.IsNullOrWhiteSpace(cuisine))
                throw new ArgumentException("Cuisine Cannot Be Empty.");

            if (ingredients == null || ingredients.Count == 0)
                throw new ArgumentException("Ingredients Cannot Be Null Or Empty.");


            Name = name;
            Cuisine = cuisine;
            IsVegetarian = isVegetarian;
            Ingredients = new List<string>(ingredients);
            InstanceCollection.Add(this);
        }

        // Method to Add an Ingredient
        public void AddIngredient(string ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
                throw new ArgumentException("Ingredient Cannot Be Empty.");

            Ingredients.Add(ingredient);
            Console.WriteLine($"Ingredient '{ingredient}' Added To Dish '{Name}'.");
        }

        // Method to Remove an Ingredient
        public void RemoveIngredient(string ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
                throw new ArgumentException("Ingredient Cannot Be Empty.");

            if (Ingredients.Remove(ingredient))
                Console.WriteLine($"Ingredient '{ingredient}' Removed From Dish '{Name}'.");
            else
                Console.WriteLine($"Ingredient '{ingredient}' Not Found In Dish '{Name}'.");
        }

        // Method to List All Ingredients
        public void ListIngredients()
        {
            Console.WriteLine($"Ingredients For '{Name}':");
            foreach (var ingredient in Ingredients)
            {
                Console.WriteLine($"- {ingredient}");
            }
        }
    }
}