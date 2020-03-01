using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MovieRank.Contracts;

namespace MovieRank.Infrastructure.Repositories
{
    public class DDLMovieRankRepository : IDDLMovieRankRepository
    {
        private readonly IAmazonDynamoDB _amazonDynamoDbClient;

        public DDLMovieRankRepository(IAmazonDynamoDB amazonDynamoDbClient)
        {
            _amazonDynamoDbClient = amazonDynamoDbClient;
        }
        public async Task CreateDynamoTable(string tableName)
        {
            /*
             To create a DynamoDB table, we need to build up a CreateTableRequest.
             - set the TableName 
             - add the AttributeDefinitions: we only need to set the partition referred to as the 'hash key' and sort key (no sort key in our case!), flexible storage!) 
             - flexible storage means we could add any type of data at runtime, we are required ONLY to provide the 'hash key' and sort key if any!
             */
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    //the partition referred to as the 'hash key
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    }
                },
                //ProvisionedThroughput
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 1,
                    WriteCapacityUnits = 1
                }
            };

            await _amazonDynamoDbClient.CreateTableAsync(request);
        }

        public async Task DeleteDynamoDbTable(string tableName)
        {
            var request = new DeleteTableRequest //create a DeleteTableRequest and set in the request the TableName
            {
                TableName = tableName
            };

            //We might want to do a check to wait for the table to be created before passing back a response
            await _amazonDynamoDbClient.DeleteTableAsync(request);
        }
    }
}
