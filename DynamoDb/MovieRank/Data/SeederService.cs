using System.Threading;
using System.Threading.Tasks;
using MovieRank.Contracts;

namespace MovieRank.Data
{
    public class SeederService : ISeederService
    {
        private readonly IMovieRankRepository _movieRankRepository;

        public SeederService(IMovieRankRepository movieRankRepository)
        {
            _movieRankRepository = movieRankRepository;
        }
        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            await _movieRankRepository.AddMovie(userId, movieRankRequest, cancellationToken);
        }
    }
}
