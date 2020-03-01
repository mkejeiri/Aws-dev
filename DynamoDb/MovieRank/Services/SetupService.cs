using System.Threading.Tasks;
using MovieRank.Contracts;

namespace MovieRank.Services
{
    public class SetupService : ISetupService
    {
        private readonly IDDLMovieRankRepository _movieRankRepository;

        public SetupService(IDDLMovieRankRepository movieRankRepository)
        {
            _movieRankRepository = movieRankRepository;
        }

        public async Task CreateDynamoDbTable(string dynamoDbTableName)
        {
            await _movieRankRepository.CreateDynamoTable(dynamoDbTableName);
        }

        public async Task DeleteDynamoDbTable(string dynamoDbTableName)
        {
            await _movieRankRepository.DeleteDynamoDbTable(dynamoDbTableName);
        }
    }
}
