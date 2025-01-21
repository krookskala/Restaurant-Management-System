namespace ConsoleApp1.Models
{
    public class Menu : SerializableObject<Menu>
    {
        public string Name { get; private set; }
        public string MenuType { get; private set; }
        public List<string> AvailableLanguages { get; private set; } // Multi-Value Attribute
        public List<Dish> Dishes { get; private set; }

        // Constructor
        public Menu(string name, string menuType, List<string> availableLanguages)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Menu Name Cannot Be Empty.");

            if (string.IsNullOrWhiteSpace(menuType))
                throw new ArgumentException("Menu Type Cannot Be Empty.");

            if (availableLanguages == null || availableLanguages.Count == 0)
                throw new ArgumentException("Available Languages Cannot Be Null Or Empty.");


            Name = name;
            MenuType = menuType;
            AvailableLanguages = new List<string>(availableLanguages);
            Dishes = new List<Dish>();
            InstanceCollection.Add(this);
        }

        // Method To Add A Language
        public void AddLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language Cannot Be Empty.");

            if (!AvailableLanguages.Contains(language)) 
            {
            AvailableLanguages.Add(language);
                Console.WriteLine($"Language '{language}' Added To Menu '{Name}'.");
            }
            else
            {
                Console.WriteLine($"Language '{language}' Already Exists In Menu '{Name}'.");
            }
        }

        // Method To Remove A Language
        public void RemoveLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language Cannot Be Empty.");

            if (AvailableLanguages.Remove(language))
                Console.WriteLine($"Language '{language}' Removed From Menu '{Name}'.");
                
            else
                Console.WriteLine($"Language '{language}' Not Found In Menu '{Name}'.");
        }

        // Method To List All Available Languages
        public void ListLanguages()
        {
            Console.WriteLine($"Available Languages For Menu '{Name}':");
            foreach (var language in AvailableLanguages)
            {
                Console.WriteLine($"- {language}");
            }
        }

        // Method To Add A Dish
        public void AddDish(Dish dish)
        {
            if (dish == null)
                throw new ArgumentException("Dish Cannot Be Null.");

            Dishes.Add(dish);
            Console.WriteLine($"Dish '{dish.Name}' Added To Menu '{Name}'.");
        }

        // Method To List All Dishes
        public void ListDishes()
        {
            Console.WriteLine($"Dishes In Menu '{Name}':");
            foreach (var dish in Dishes)
            {
                Console.WriteLine($"- {dish.Name}");
            }
        }
    }
}