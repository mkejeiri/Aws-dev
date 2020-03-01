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
            //Might want to do a check to wait for the table to be created before passing back a response

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
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    }
                },
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
            var request = new DeleteTableRequest
            {
                TableName = tableName
            };

            await _amazonDynamoDbClient.DeleteTableAsync(request);
        }
    }
}
