using System.Xml.Serialization;

namespace ConsoleApp1.Services
{
    public static class SerializationManager
    {
        public static void SerializeToXml<T>(List<T> objects, string filePath = "")
        {
            filePath ??= $"{typeof(T).Name}_Extent.xml";

            try
            {
                using var writer = new StreamWriter(filePath);
                var serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(writer, objects);
                Console.WriteLine($"Serialized {objects.Count} {typeof(T).Name} objects to {filePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during XML serialization of {typeof(T).Name}: {ex.Message}");
            }
        }
       
        public static List<T> DeserializeFromXml<T>(string filePath = "")
        {
            filePath ??= $"{typeof(T).Name}_Extent.xml";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found. Returning an empty list for {typeof(T).Name}.");
                return new List<T>();
            }

            try
            {
                using var reader = new StreamReader(filePath);
                var serializer = new XmlSerializer(typeof(List<T>));
                var deserialized = serializer.Deserialize(reader);

                if (deserialized is List<T> list)
                {
                    Console.WriteLine($"Deserialized {list.Count} {typeof(T).Name} objects from {filePath}.");
                    return list;
                }
                else
                {
                    Console.WriteLine($"Deserialization returned null or unexpected type for {typeof(T).Name}. Initializing empty list.");
                    return new List<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during XML deserialization of {typeof(T).Name}: {ex.Message}");
                return new List<T>();
            }
        }
    }
}