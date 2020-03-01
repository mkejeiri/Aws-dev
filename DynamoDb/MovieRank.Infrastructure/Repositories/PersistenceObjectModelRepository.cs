using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MovieRank.Contracts;
using MovieRank.Infrastructure.Mappers;
using MovieRank.Infrastructure.Models;
/*
     Persistence object model 
    - Wrapper around the low level model
    - simple
    - map client class to DynamoDB table
    - missing feature create, delete, update dynamoDb table
 */
namespace MovieRank.Infrastructure.Repositories
{
    public class PersistenceObjectModelRepository : IMovieRankRepository
    {
        private const string PropertyName = "MovieName"; //Case insensitive!
        private const string IndexName = "MovieName-index"; 
        private readonly IPersistenceObjectModelMapper _mapper;
        private readonly IDynamoDBContext _dynamoDbContext;

        public PersistenceObjectModelRepository(IAmazonDynamoDB amazonDynamoDbClient, IPersistenceObjectModelMapper mapper)
        {
            _mapper = mapper;
            _dynamoDbContext = new DynamoDBContext(amazonDynamoDbClient);
        }

        public async Task<List<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
                     {
                         new ScanCondition(propertyName:PropertyName, op:ScanOperator.Contains, movieName),
                         new ScanCondition(PropertyName, ScanOperator.BeginsWith, movieName)
                     },
                ConditionalOperator = ConditionalOperatorValues.Or
            };
            return _mapper.ToMovieContract(await _dynamoDbContext.QueryAsync<MovieDb>(userId, config).GetRemainingAsync(cancellationToken)).ToList();
        }

        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            await _dynamoDbContext.SaveAsync(_mapper.ToMovieDbModel(userId, movieRankRequest), cancellationToken);
        }

        public async Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest, MovieResponse movieResponse, CancellationToken cancellationToken)
        {
            var movieResponseToDbModel = _mapper.FromMovieResponseToDbModel(movieResponse, userId);

            var request = _mapper.ToUpdateMovieDbModel(userId, movieUpdateRequest, movieResponseToDbModel);

            await _dynamoDbContext.SaveAsync(request, cancellationToken);
        }

        public async Task<List<MovieResponse>> GetMoviesRank(string movieName, CancellationToken cancellationToken)
        {

            var config = new DynamoDBOperationConfig
            {
                IndexName = IndexName
            };

            var movieDbs = await _dynamoDbContext.QueryAsync<MovieDb>(movieName, config).GetRemainingAsync(cancellationToken);
            return _mapper.ToMovieContract(movieDbs.AsEnumerable()).ToList();
        }

        public async Task<List<MovieResponse>> GetAllItems(CancellationToken cancellationToken)
        {
            var movieDbs = await _dynamoDbContext.ScanAsync<MovieDb>(new List<ScanCondition>())
                .GetRemainingAsync(cancellationToken);
            return _mapper.ToMovieContract(movieDbs.AsEnumerable()).ToList();
        }

        public async Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken)
        {
            var movieDb = await _dynamoDbContext.LoadAsync<MovieDb>(userId, movieName, cancellationToken);
            return _mapper.ToMovieContract(movieDb);
        }


    }
}
