using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using MovieRank.Libs.Models;
using Newtonsoft.Json;

namespace MovieRank.Data
{
    public class Seeder
    {

        private readonly IDynamoDBContext _dynamoDbContext;
        public Seeder(IAmazonDynamoDB amazonDynamoDbClient)
        {
            _dynamoDbContext = new DynamoDBContext(amazonDynamoDbClient);

        }

        public void Run(bool seed)
        {
            if (seed)
            {
                var data = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var movieDbs = JsonConvert.DeserializeObject<List<MovieDb>>(data);

                //create user in the table 
                foreach (var movieDb in movieDbs)
                {
                    _dynamoDbContext.SaveAsync(movieDb).Wait();
                }
            }

        }
    }
}
