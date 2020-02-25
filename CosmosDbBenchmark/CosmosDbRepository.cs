using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbBenchmark.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CosmosDbBenchmark
{
    public class CosmosDbRepository<T> where T : DataEntity
    {
        private Container _container;

        public CosmosDbRepository()
        {
            this._container = CosmosConnector.Container;
        }

        public async Task<CosmosResponse<T>> AddAsync(T item)
        {
            ItemResponse<T> itemResponse = await this._container.CreateItemAsync(item);
            T createdEntity = JsonConvert.DeserializeObject<T>(itemResponse.Resource.ToString());
            return new CosmosResponse<T>(createdEntity, itemResponse.RequestCharge, ComsosDbOperation.Create);
        }

        public async Task<CosmosResponse<T>> GetDocumentByIdAsync(string id, string key)
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(key));
            return new CosmosResponse<T>((T) response, response.RequestCharge, ComsosDbOperation.Read);
        }

        public async Task<CosmosResponse<T>> AddOrUpdateAsync(T item, string key)
        {
            ItemResponse<T> itemResponse = await this._container.UpsertItemAsync<T>(item, new PartitionKey(key));
            T updatedEntity = JsonConvert.DeserializeObject<T>(itemResponse.Resource.ToString());
            return new CosmosResponse<T>(updatedEntity, itemResponse.RequestCharge, ComsosDbOperation.Update);
        }

        public async Task<CosmosResponse<T>> RemoveAsync(string id, string key)
        {
            ItemResponse<T> result = await this._container.DeleteItemAsync<T>(id, new PartitionKey(key));
            bool isSuccess = result.StatusCode == System.Net.HttpStatusCode.NoContent;
            return new CosmosResponse<T> { RequestCharge = result.RequestCharge, IsSuccess = isSuccess, ComsosDbOperation = ComsosDbOperation.Delete };
        }

        public async Task<List<CosmosResponse<T>>> QueryItemsAsync(string query)
        {
            var sqlQueryText = query;
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<T> queryResultSetIterator = _container.GetItemQueryIterator<T>(queryDefinition);

            List<CosmosResponse<T>> responses = new List<CosmosResponse<T>>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var item in currentResultSet)
                {
                    responses.Add(new CosmosResponse<T>(item, currentResultSet.RequestCharge, ComsosDbOperation.Query));
                }
            }

            return responses;
        }
    }
}