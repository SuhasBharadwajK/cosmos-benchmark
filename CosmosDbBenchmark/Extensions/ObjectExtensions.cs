using Newtonsoft.Json;

namespace CosmosDbBenchmark
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object source) where T : new()
        {
            var serializedParent = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serializedParent);
        }
    }
}
