using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Tools;
using Common.Models;

namespace DatabaseService.Controllers
{
    public class CosmosDbClient
    {
        public static readonly CosmosDbClient Singleton = new CosmosDbClient();

        private bool initDone = false;
        
        private static readonly string EndpointUri = "https://iegproductsdb.documents.azure.com:443/";
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string databaseId = "QuestionDatabase";
        private string containerId = "QuestionContainer";

        private CosmosDbClient()
        {
            
        }

        public async Task Init()
        {
            if (this.initDone)
                return;

            this.initDone = true;

            //var authKey = await KeyVault.Singleton.GetSecretByKey("dbPrimary");
            var authKey = "VVgqSae3zWC9XNXc2MfhQcw5y2LI4JQZQZOGXG8vy0zAbzayP0NgZB5HtFE8knAo2v2oRBlmfEdTiG2TC1zdgQ==";

            this.cosmosClient = new CosmosClient(EndpointUri, authKey);
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/id");
        }


        public async Task<T[]> Get<T>()
        {           
            var sqlQueryText = "SELECT * FROM c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<T> queryResultSetIterator = this.container.GetItemQueryIterator<T>(queryDefinition);

            List<T> products = new List<T>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (T product in currentResultSet)
                {
                    products.Add(product);
                }
            }
            return products.ToArray();            
        }

        public async Task Insert<T>(T element)
        {
            try
            {
                var res = await this.container.UpsertItemAsync(element);
            }
            catch (Exception e)
            {

                throw;
            }            
        }

        public async Task Delete<T>(string id, PartitionKey partitionKey)
        {
            await this.container.DeleteItemAsync<T>(id, partitionKey);
        }
       

    }

    
}
