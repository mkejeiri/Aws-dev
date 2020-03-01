using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MovieRank.Contracts;
using MovieRank.Infrastructure.Mappers;

namespace MovieRank.Infrastructure.Repositories
{
    public  class DocumentModelRepository : IMovieRankRepository
    {
        private const string UserIdAttributeName = "UserId";
        private const string MovieNameAttributeName = "MovieName";
        private const string IndexName = "MovieName-index";
        private readonly IDocumentMapper _documentMapper;
        private const string TableName = "MovieRank";
        private readonly Table _table ;


        public DocumentModelRepository(IAmazonDynamoDB amazonDynamoDbClient, IDocumentMapper documentMapper)
        {
            _documentMapper = documentMapper;
            _table = Table.LoadTable(amazonDynamoDbClient, TableName);
        }

        public async Task<List<MovieResponse>> GetAllItems(CancellationToken cancellationToken)
        {
            var config = new ScanOperationConfig();
            return _documentMapper.ToMovieContract(await _table.Scan(config).GetRemainingAsync(cancellationToken)) ;
        }

        public async Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken)
        {
            return _documentMapper.ToMovieContract(await _table.GetItemAsync(userId, movieName, cancellationToken));
        }

        public async Task<List<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken)
        {
           var filter= new QueryFilter(attributeName: UserIdAttributeName, QueryOperator.Equal, userId);
           filter.AddCondition(keyAttributeName: MovieNameAttributeName, QueryOperator.BeginsWith, movieName);

           var rankedMoviesByMovieTitle = await _table.Query(filter).GetRemainingAsync(cancellationToken);
           return _documentMapper.ToMovieContract(rankedMoviesByMovieTitle);
        }

        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            await _table.PutItemAsync(doc:_documentMapper.ToMovieDbModel(userId, movieRankRequest), cancellationToken);
        }

        public async Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest, MovieResponse movieResponse,
            CancellationToken cancellationToken)
        {
            var documentFromDb = _documentMapper.FromMovieResponseToDbModel(movieResponse, userId);
            var document = _documentMapper.ToUpdateMovieDbModel(userId, movieUpdateRequest, documentFromDb);
            await _table.UpdateItemAsync(document, cancellationToken);
        }

        public async Task<List<MovieResponse>> GetMoviesRank(string movieName, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter(attributeName: MovieNameAttributeName, QueryOperator.Equal, movieName);
            var config = new QueryOperationConfig
            {
                IndexName = IndexName,
                Filter = filter
            };
            return _documentMapper.ToMovieContract(await _table.Query(config).GetRemainingAsync(cancellationToken));
        }
    }
}
