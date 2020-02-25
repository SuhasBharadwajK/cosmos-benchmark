using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace CosmosDemo
{
    public class MongoOperations
    {
        private string endpointUrl = "mongodb://technovert-mongo:1Ew6PhsCb4dzruEeU8OWEEb2Or5h0p3tMH8oPH0y0fOXAWIpXZrGT4zhgrI2ydk2jwBwaRM7Rc6ftVKDLG1sPw==@technovert-mongo.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        private string databaseId = "FamilyDB";

        private string collectionId = "FamilyCollection";

        private readonly IMongoCollection<Family> collection;

        public MongoOperations()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(endpointUrl));

            settings.SslSettings =
            new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            var database = mongoClient.GetDatabase(databaseId);

            collection = database.GetCollection<Family>(collectionId);

        }

        public List<Family> Get() =>
            collection.Find(item => true).ToList();

        public Family Create()
        {
            // Create a family object for the Andersen family
            Family andersenFamily = new Family
            {
                Id = "Andersen.1",
                LastName = "Andersen",
                Parents = new Parent[]
                {
           new Parent { FirstName = "Thomas" },
           new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
           new Child
            {
                FirstName = "Henriette Thaulow",
                Gender = "female",
                Grade = 5,
                Pets = new Pet[]
                {
                    new Pet { GivenName = "Fluffy" }
                }
            }
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = false
            };
            collection.InsertOne(andersenFamily);
            return andersenFamily;
        }

        public bool Update()
        {
            // Create a family object for the Andersen family
            Family andersenFamily = new Family
            {
                Id = "Andersen.1",
                LastName = "Andersen",
                Parents = new Parent[]
                {
           new Parent { FirstName = "Thomas" },
           new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
           new Child
            {
                FirstName = "Naveen Thaulow",
                Gender = "male",
                Grade = 10,
                Pets = new Pet[]
                {
                    new Pet { GivenName = "Fluffy" }
                }
            }
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = false
            };

            var t = collection.ReplaceOne(item => item.Id == andersenFamily.Id, andersenFamily);
            return t.ModifiedCount > 0 ? true : false;
        }
    }
}
