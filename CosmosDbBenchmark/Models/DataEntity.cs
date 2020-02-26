using Newtonsoft.Json;

namespace CosmosDbBenchmark.Models
{
    public abstract class DataEntity
    {
        public DataEntity(string type)
        {
            this.Type = type;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public abstract string EntityName { get; }
    }
}