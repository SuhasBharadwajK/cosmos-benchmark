namespace CosmosDbBenchmark.Models
{
    public class CosmosResponse<T> where T : DataEntity
    {
        public CosmosResponse()
        {
        }

        public CosmosResponse(T item, double requestCharge, ComsosDbOperation comsosDbOperation, bool isSuccess = true)
        {
            this.Item = item;
            this.RequestCharge = requestCharge;
            this.IsSuccess = isSuccess;
            this.ComsosDbOperation = comsosDbOperation;
        }

        public double RequestCharge { get; set; }

        public T Item { get; set; }

        public bool IsSuccess { get; set; }

        public ComsosDbOperation ComsosDbOperation { get; set; }
    }
}