using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDemo
{
    public class CosmosDbRepository<T> where T : BaseEntity
    {
        private CosmosClient _client;

        /// The Azure Cosmos DB endpoint.
        private readonly string EndpointUrl = "https://technovert-test.documents.azure.com:443/";

        /// The primary key for the Azure DocumentDB account.
        private string PrimaryKey = "HtyzhloYXM0JJVODDbO6K6F8udM7c13Z0JbFGWUpixdk0lnrMQBmevdiG5sqIpNsiT4F8VjGZVcl7ay4fYH41Q==";

        // The database we will create
        private Database _database;

        // The container we will create.
        private Container _container;

        private readonly string _databaseId = "FamilyDatabase";

        private readonly string _containerId = "FamilyContainer";

        public CosmosDbRepository(string database,string container)
        {
            _client = new CosmosClient(EndpointUrl, PrimaryKey);
            _containerId = container;
            _databaseId = database;
            _database = _client.GetDatabase(_databaseId);
            _container = _client.GetContainer(_databaseId,_containerId);
        }

        public async Task CreateDatabaseAsync()
        {
            // Create a new database
            this._database = await this._client.CreateDatabaseIfNotExistsAsync(_databaseId);
            Console.WriteLine("Created Database: {0}\n", this._database.Id);
        }

        /// Create the container if it does not exist. 
        public async Task CreateContainerAsync()
        {
            // Create a new container
            this._container = await this._database.CreateContainerIfNotExistsAsync(_containerId, "/LastName");
            Console.WriteLine("Created Container: {0}\n", this._container.Id);
        }

        public async Task<T> AddAsync(T item)
        {
            T createdEntity;
            ItemResponse<T> itemResponse = await this._container.CreateItemAsync(item);
            Console.WriteLine("Add Operation consumed following Request units:"+ itemResponse.RequestCharge);
            createdEntity = JsonConvert.DeserializeObject<T>(itemResponse.Resource.ToString());
            //createdEntity = itemResponse.Resource;
            return createdEntity;
        }

        public async Task<T> GetDocumentByIdAsync(string id,string key)
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(key));
            Console.WriteLine("Get Operation consumed following Request units:" + response.RequestCharge);
            return (T)response;
        }

        public async Task<T> AddOrUpdateAsync(T item, string key)
        {
            ItemResponse<T> itemResponse = await this._container.UpsertItemAsync<T>(item, new PartitionKey(key));
            Console.WriteLine("Update Operation consumed following Request units:" + itemResponse.RequestCharge);
            T updatedEntity = JsonConvert.DeserializeObject<T>(itemResponse.Resource.ToString());
            //T updatedEntity = itemResponse.Resource;
            return updatedEntity;
        }

        public async Task<bool> RemoveAsync(string id,string key)
        {
            ItemResponse<T> result = await this._container.DeleteItemAsync<T>(id, new PartitionKey(key));
            Console.WriteLine("Remove Operation consumed following Request units:" + result.RequestCharge);
            bool isSuccess = result.StatusCode == System.Net.HttpStatusCode.NoContent;
            return isSuccess;
        }

        //public async Task<List<T>> QueryItemsAsync(Expression<Func<T,bool>> expression, IEnumerable<T> query)
        //{
        //    var queryable = _container.GetItemLinqQueryable<T>();
        //    var iterator = queryable.ToFeedIterator();
        //    List<T> items = new List<T>();
        //    while (iterator.HasMoreResults)
        //    {
        //        foreach(var document in await iterator.ReadNextAsync())
        //        {
        //            items.Add(document);
        //        }
        //    }
        //    return items;
        //}

        public async Task QueryItemsAsync(string query)
        {
            var sqlQueryText = query;

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<T> queryResultSetIterator = _container.GetItemQueryIterator<T>(queryDefinition);

            List<T> items = new List<T>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                Console.WriteLine("Query Operation consumed following Request units: " + currentResultSet.RequestCharge);
                foreach (var item in currentResultSet)
                {
                    items.Add(item);
                    //Console.WriteLine("\tRead {0}\n", item);
                    
                }
            }
        }



    }
}
