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
                    {"UserId", new AttributeValue {N = userId.ToString()}},
                    {"MovieName", new AttributeValue {S = movieName}}
                }
            };
            return _mapper.ToMovieContract(await _amazonDynamoDbClient.GetItemAsync(request, cancellationToken));
        }

        public async Task<List<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken)
        {
            var request = new QueryRequest
            {
                TableName = TableName,
                KeyConditionExpression = "UserId = :userId and begins_with (MovieName, :movieName)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":userId", new AttributeValue { N =  userId.ToString() }},
                    {":movieName", new AttributeValue { S = movieName }}
                }
            };
            return _mapper.ToMovieContract(await _amazonDynamoDbClient.QueryAsync(request, cancellationToken)).ToList();
        }

        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"UserId", new AttributeValue {N = userId.ToString()}},
                    {"MovieName", new AttributeValue {S = movieRankRequest.MovieName}},
                    {"Description", new AttributeValue {S = movieRankRequest.Description}},
                    {"Actors", new AttributeValue{SS = movieRankRequest.Actors}},
                    {"Ranking", new AttributeValue {N = movieRankRequest.Ranking.ToString()}},
                    {"RankedDateTime", new AttributeValue {S = DateTime.UtcNow.ToString()}}
                }
            };
            await _amazonDynamoDbClient.PutItemAsync(request, cancellationToken);
        }

        public async Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest, MovieResponse movieResponse,
            CancellationToken cancellationToken)
        {
            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"UserId", new AttributeValue {N = userId.ToString()}},
                    {"MovieName", new AttributeValue {S = movieUpdateRequest.MovieName}}
                },
                AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    { "Ranking", new AttributeValueUpdate
                        {
                            Action = AttributeAction.PUT,
                            Value = new AttributeValue {N = movieUpdateRequest.Ranking.ToString()}
                        }
                    },
                    { "RankedDateTime", new AttributeValueUpdate
                        {
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
            var request = new QueryRequest
            {
                TableName = TableName,
                IndexName = "MovieName-index",
                KeyConditionExpression = "MovieName = :movieName",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":movieName", new AttributeValue { S =  movieName }}}
            };

            return _mapper.ToMovieContract(await _amazonDynamoDbClient.QueryAsync(request, cancellationToken)).ToList();
        }
    }
}
