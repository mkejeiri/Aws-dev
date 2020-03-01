using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MovieRank.Contracts;
using MovieRank.Infrastructure.Mappers;

/*
  The low-level model offered by the DynamoDB SDK for .NET to interact with DynamoDB from within .NET application,
 - This model is what the Object Persistence and document model wrap. 
 - The Object Persistence and document model wraps the low-level model's operations
 -  This model gives all features offered by DynamoDB.
 - The low-level model is the only model that we can use to create, update, 
	and delete DynamoDB tables to interact with DynamoDB from within .NET application
	this model requires a significant amount of extra code to interact with DynamoDB.
 
 */
namespace MovieRank.Infrastructure.Repositories
{
    public class LowLevelModelRepository : IMovieRankRepository
    {
        private const string TableName = "MovieRank";
        private readonly IAmazonDynamoDB _amazonDynamoDbClient;
        private readonly ILowLevelModelMapper _mapper;

        public LowLevelModelRepository(IAmazonDynamoDB amazonDynamoDbClient, ILowLevelModelMapper lowLevelModelMapper)
        {
            _amazonDynamoDbClient = amazonDynamoDbClient;
            _mapper = lowLevelModelMapper;
        }

        public async Task<List<MovieResponse>> GetAllItems(CancellationToken cancellationToken)
        {
            var scanRequest = new ScanRequest(TableName);
            var scanResponse = await _amazonDynamoDbClient.ScanAsync(scanRequest, cancellationToken);
            return _mapper.ToMovieContract(scanResponse).ToList();
        }

        public async Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"UserId", new AttributeValue {N = userId.ToString()}}, //add the partition key, N =  i.e stored as a number
                    {"MovieName", new AttributeValue {S = movieName}} //add also the sort key,  S =  i.e stored as a string
                }
            };
            return _mapper.ToMovieContract(await _amazonDynamoDbClient.GetItemAsync(request, cancellationToken));
        }

        public async Task<List<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken)
        {
            var request = new QueryRequest
            {
                TableName = TableName,
                KeyConditionExpression = "UserId = :userId and begins_with (MovieName, :movieName)", //in the DynamoDB table we want to equal UserId attribute with userId param
                                                                                                     //we use begins_with to query with the first letters (movieName param)
                                                                                                     //in MovieName attribute

                //when querying (i.e. QueryAsync) the data, describe how the value(s) is (are) stored in  DynamoDB table (the values.json file)                                                                                    
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":userId", new AttributeValue { N =  userId.ToString() }}, //add the partition key, N =  i.e stored as a number
                    {":movieName", new AttributeValue { S = movieName }} //add also the sort key,  S =  i.e stored as a string
                }
            };
            return _mapper.ToMovieContract(await _amazonDynamoDbClient.QueryAsync(request, cancellationToken)).ToList();
        }

        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            var request = new PutItemRequest //build an object PutItemRequest (from amazon sdk) to add an object to the DynamoDB table.
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"UserId", new AttributeValue {N = userId.ToString()}}, //N : for number
                    {"MovieName", new AttributeValue {S = movieRankRequest.MovieName}},
                    {"Description", new AttributeValue {S = movieRankRequest.Description}}, //S : for string
                    {"Actors", new AttributeValue{SS = movieRankRequest.Actors}}, //SS: for list of string 
                    {"Ranking", new AttributeValue {N = movieRankRequest .Ranking.ToString()}},
                    {"RankedDateTime", new AttributeValue {S = DateTime.UtcNow.ToString()}}
                }
            };
            await _amazonDynamoDbClient.PutItemAsync(request, cancellationToken);
        }

        public async Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest, MovieResponse movieResponse,
            CancellationToken cancellationToken)
        {
            var request = new UpdateItemRequest //build an object UpdateItemRequest (from amazon sdk) to update the DynamoDB table
                                                //(update a ranking for user and movie)
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                { //use the partition key (i.e. UserId) and sort key (i.e. MovieName) to locate the object in DynamoDB table.
                    {"UserId", new AttributeValue {N = userId.ToString()}},
                    {"MovieName", new AttributeValue {S = movieUpdateRequest.MovieName}}
                },
                //set the properties to update (i.e. Ranking and RankedDateTime)
                AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    { "Ranking", new AttributeValueUpdate
                        {
                            //Pass in the action to perform
                            Action = AttributeAction.PUT,
                            Value = new AttributeValue {N = movieUpdateRequest.Ranking.ToString()}
                        }
                    },
                    { "RankedDateTime", new AttributeValueUpdate
                        {
                            //Pass in the action to perform
                            Action = AttributeAction.PUT,
                            Value = new AttributeValue {S = DateTime.UtcNow.ToString()}
                        }
                    }
                }
            };

            await _amazonDynamoDbClient.UpdateItemAsync(request, cancellationToken);
        }

        public async Task<List<MovieResponse>> GetMoviesRank(string movieName, CancellationToken cancellationToken)
        {
            var request = new QueryRequest //build an object QueryRequest (from amazon sdk) to query the DynamoDB table
            {
                TableName = TableName,
                IndexName = "MovieName-index", //Set the secondary index name (i.e. MovieName-index  see Indexes in MovieRank DynamoDB table aws console)
                KeyConditionExpression = "MovieName = :movieName", //MovieName attribute equals to movieName param which passed on to the GetMoviesRank method

                //when querying (i.e. QueryAsync) the data, describe how the value(s) is (are) stored in  DynamoDB table (the values.json file)                     
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":movieName", new AttributeValue { S =  movieName }}}
            }; 
             
            return _mapper.ToMovieContract(await _amazonDynamoDbClient.QueryAsync(request, cancellationToken)).ToList();
        }
    }
}
