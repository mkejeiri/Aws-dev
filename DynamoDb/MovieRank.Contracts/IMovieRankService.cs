using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieRank.Contracts
{
    public interface IMovieRankService
    {
        Task<IEnumerable<MovieResponse>> GetAllItemsFromDatabase(CancellationToken cancellationToken);
        Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken);
        Task<IEnumerable<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken);
        Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken);
        Task UpdateMovie(int userId, string movieName, MovieUpdateRankingRequest movieRankRequest, CancellationToken cancellationToken);
        Task<MovieRankResponse> GetMovieRank(string movieName, CancellationToken cancellationToken);
    }
}
