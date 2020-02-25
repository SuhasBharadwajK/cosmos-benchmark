namespace CosmosDbBenchmark.Models
{
    public abstract class DataEntity
    {
        public DataEntity(string type)
        {
            this.type = type;
        }

        public string id { get; set; }

        public string type { get; set; }

        public abstract string entityName { get; }
    }
}