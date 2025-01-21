using System;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1.Services
{
    public static class ExtentManager
    {
        public static void LoadAllExtents()
        {
            try
            {
                var serializableTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.BaseType != null &&
                                t.BaseType.IsGenericType &&
                                t.BaseType.GetGenericTypeDefinition() == typeof(SerializableObject<>));

                foreach (var type in serializableTypes)
                {
                    Console.WriteLine($"Found serializable type: {type.Name}");
                    var loadMethod = type.GetMethod("LoadExtent", BindingFlags.Public | BindingFlags.Static);
                    if (loadMethod != null)
                    {
                        Console.WriteLine($"Invoking LoadExtent for {type.Name}");
                        loadMethod.Invoke(null, null);
                    }
                    else
                    {
                        Console.WriteLine($"No LoadExtent method found for {type.Name}");
                    }
                }

                Console.WriteLine("All Extents Loaded Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Loading Extents: {ex.Message}");
            }
        }

        public static void SaveAllExtents()
        {
            try
            {
                var serializableTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.BaseType != null &&
                                t.BaseType.IsGenericType &&
                                t.BaseType.GetGenericTypeDefinition() == typeof(SerializableObject<>));

                foreach (var type in serializableTypes)
                {
                    var saveMethod = type.GetMethod("SaveExtent", BindingFlags.Public | BindingFlags.Static);
                    if (saveMethod != null)
                    {
                        Console.WriteLine($"Invoking SaveExtent for {type.Name}");
                        saveMethod.Invoke(null, null);
                    }
                }

                Console.WriteLine("All Extents Saved Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Saving Extents: {ex.Message}");
            }
        }
    }
}
