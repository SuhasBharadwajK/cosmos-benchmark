using System;
using Microsoft.Azure.Cosmos;

namespace CosmosDbBenchmark
{
    public class CosmosConnector
    {
        /// The Azure Cosmos DB endpoint for running this GetStarted sample.
        private string EndpointUrl = Environment.GetEnvironmentVariable("EndpointUrl");

        /// The primary key for the Azure DocumentDB account.
        private string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "FamilyDatabase";
        
        private string containerId = "BenchmarksCollection";

        private static Container _container;

        private static Database _database;

        public static Database Database
        {
            get
            {
                if (_database == null)
                {
                    var dbConnector = new CosmosConnector();       
                    _database = dbConnector.cosmosClient.GetDatabase(dbConnector.databaseId);
                }

                return _database;
            }
        }
        
        public static Container Container
        {
            get
            {
                if(_container == null)
                {      
                    var dbConnector = new CosmosConnector();
                    _container = dbConnector.cosmosClient.GetContainer(dbConnector.databaseId, dbConnector.containerId);
                }

                return _container;
            }
        }

        public CosmosConnector()
        {
            this.cosmosClient = new CosmosClient(this.EndpointUrl, this.PrimaryKey);
        }
    }
}
