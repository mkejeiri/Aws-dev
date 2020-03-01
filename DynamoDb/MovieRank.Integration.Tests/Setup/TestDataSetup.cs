using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace MovieRank.Integration.Tests.Setup
{
    public class TestDataSetup
    {
        private const string Uri = "http://localhost:8000";
        private static  readonly  IAmazonDynamoDB AmazonDynamoDbClient = 
            new AmazonDynamoDBClient(
                new AmazonDynamoDBConfig()
                {
                    ServiceURL = Uri
                });
        public async Task CreateTable()
        {
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "UserId",
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "MovieName",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "UserId",
                        KeyType = "HASH" //aka partition key!
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "MovieName",
                        KeyType = "RANGE" //aka as sort key!
                    }
                },
                //ProvisionedThroughput settings
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 1,
                    WriteCapacityUnits = 1
                },
                TableName = "MovieRank",

                //MovieName-index SecondaryIndex settings
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "MovieName-index",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = "MovieName",
                                KeyType = "HASH"
                            }
                        },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 1,
                            WriteCapacityUnits = 1
                        },
                        Projection = new Projection
                        {
                            ProjectionType = "ALL"
                        }
                    }
                }
            };

            await AmazonDynamoDbClient.CreateTableAsync(request);

            //Wait for the table to be created before going further!
            await WaitUntilTableActive(request.TableName);
        }

        private static async Task WaitUntilTableActive(string tableName)
        {
            string status = null;
            do
            {
                Thread.Sleep(1000);
                try
                {
                    //we call DescribeTable
                    status = await GetTableStatus(tableName);
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }

            } while (status != "ACTIVE");
        }

        private static async Task<string> GetTableStatus(string tableName)
        {
            var response = await AmazonDynamoDbClient.DescribeTableAsync(new DescribeTableRequest
            {
                TableName = tableName
            });

            return response.Table.TableStatus;
        }
    }
}