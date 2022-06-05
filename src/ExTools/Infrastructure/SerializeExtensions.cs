using Newtonsoft.Json;

using System.IO;

namespace ExTools.Infrastructure
{
    public static class SerializeExtensions
    {
        public static T DeserializeJson<T>(this FileInfo file) where T : class
        {
            string json = File.ReadAllText(file.FullName);
            JsonSerializerSettings settings = new()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static void SerializeJson<T>(this T obj, FileInfo file) where T : class
        {
            JsonSerializer serializer = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            using StreamWriter sw = new(file.FullName);
            using JsonTextWriter writer = new(sw);
            serializer.Serialize(writer, obj);
        }
    }
}