using System.Xml.Linq;
using System.Xml.Serialization;

namespace ConsoleApp1.Services
{
    public abstract class SerializableObject<T>
    {
        public static List<T> Instances { get; private set; } = new List<T>();

        public static void AddInstance(T instance)
        {
            if (Instances.Contains(instance))
            {
                Console.WriteLine($"Duplicate instance detected. Skipping addition: {instance}");
                return;
            }

            Instances.Add(instance);
            Console.WriteLine($"Added new instance: {instance}");
        }

        public static void SaveExtent()
        {
            var filePath = $"{typeof(T).Name}_Extent.xml";
            try
            {
                using var writer = new StreamWriter(filePath);
                var serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(writer, Instances);
                Console.WriteLine($"Extent for {typeof(T).Name} saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving extent for {typeof(T).Name}: {ex.Message}");
            }
        }

        public static void LoadExtent()
        {
            var filePath = $"{typeof(T).Name}_Extent.xml";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found. Initializing empty extent for {typeof(T).Name}");
                Instances = new List<T>();
                return;
            }

            try
            {
                using var reader = new StreamReader(filePath);
                var serializer = new XmlSerializer(typeof(List<T>));
                if (serializer.Deserialize(reader) is List<T> list)
                {
                    Instances = list;
                    Console.WriteLine(
                        $"Extent for {typeof(T).Name} loaded from {filePath}. Instance count: {Instances.Count}");
                }
                else
                {
                    Instances = new List<T>();
                    Console.WriteLine($"Extent for {typeof(T).Name} loaded but was empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading extent for {typeof(T).Name}: {ex.Message}");
            }
        }
    }
}
