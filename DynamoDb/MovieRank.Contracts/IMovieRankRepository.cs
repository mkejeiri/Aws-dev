using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieRank.Contracts
{
    public interface IMovieRankRepository
    {
        Task<List<MovieResponse>> GetAllItems(CancellationToken cancellationToken);
        Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken);
        Task<List<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken);
        Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken);
        Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest, MovieResponse movieResponse, CancellationToken cancellationToken);
        Task<List<MovieResponse>> GetMoviesRank(string movieName, CancellationToken cancellationToken);
    }
}
