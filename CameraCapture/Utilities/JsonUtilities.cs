using Newtonsoft.Json;

namespace CameraCapture.Utilities
{
    public static class JsonUtilities
    {
        public static string Serialize(object value, bool indented = false, bool onlyWritable = true)
        {
            return JsonConvert.SerializeObject(value,
                indented ? Formatting.Indented : Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = onlyWritable ? new WritablePropertiesOnlyResolver() : null
                });
        }

        public static T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}