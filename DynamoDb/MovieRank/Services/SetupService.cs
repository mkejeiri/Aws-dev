using System.Threading.Tasks;
using MovieRank.Contracts;

namespace MovieRank.Services
{
    public class SetupService : ISetupService
    {
        private readonly IDDLMovieRankRepository _movieRankRepositoryRespository;

        public SetupService(IDDLMovieRankRepository movieRankRepositoryRespository)
        {
            _movieRankRepositoryRespository = movieRankRepositoryRespository;
        }

        public async Task CreateDynamoDbTable(string dynamoDbTableName)
        {
            await _movieRankRepositoryRespository.CreateDynamoTable(dynamoDbTableName);
        }

        public async Task DeleteDynamoDbTable(string dynamoDbTableName)
        {
            await _movieRankRepositoryRespository.DeleteDynamoDbTable(dynamoDbTableName);
        }
    }
}
