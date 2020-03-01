using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieRank.Contracts;

namespace MovieRank.Infrastructure.Repositories
{
    public  class DocumentModelRepository : IMovieRankRepository
    {
        public Task<List<MovieResponse>> GetAllItems(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest, MovieResponse movieResponse,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<MovieResponse>> GetMoviesRank(string movieName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
