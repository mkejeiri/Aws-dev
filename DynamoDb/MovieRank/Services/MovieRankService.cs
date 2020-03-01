using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieRank.Contracts;

namespace MovieRank.Services
{
    public class MovieRankService : IMovieRankService
    {
        private readonly IMovieRankRepository _movieRankRepository;

        public MovieRankService(IMovieRankRepository movieRankRepository)
        {
            _movieRankRepository = movieRankRepository;
        }

        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken)
        {
            await _movieRankRepository.AddMovie(userId, movieRankRequest, cancellationToken);
        }

        public async Task UpdateMovie(int userId, string movieName, MovieUpdateRankingRequest movieUpdateRankingRequest,
            CancellationToken cancellationToken)
        {
            var movieResponse = await _movieRankRepository.GetMovie(userId, movieName, cancellationToken);
            if (movieResponse == null)
            {
                throw new InvalidOperationException($"Invalid userId/MovieName : {userId}/{movieName}");
            }

            var movieUpdateRequest = new MovieUpdateRequest
            {
                Ranking = movieUpdateRankingRequest.Ranking,
                MovieName = movieName
            };
            await _movieRankRepository.UpdateMovie(userId, movieUpdateRequest, movieResponse, cancellationToken);
        }

       
        
        public async Task<IEnumerable<MovieResponse>> GetAllItemsFromDatabase(CancellationToken cancellationToken)
        {
            return await _movieRankRepository.GetAllItems(cancellationToken);
        }

        public async Task<MovieResponse> GetMovie(int userId, string movieName, CancellationToken cancellationToken)
        {
            return await _movieRankRepository.GetMovie(userId, movieName, cancellationToken);
        }

       public async Task<MovieRankResponse> GetMovieRank(string movieName, CancellationToken cancellationToken)
       {
           var movieResponses =  await _movieRankRepository.GetMoviesRank(movieName, cancellationToken);

           if (movieResponses == null  ||!movieResponses.Any())
           {
               throw  new InvalidOperationException($"movieName '{movieName}' doesn't exist!");
           }

           var overallRanking = Math.Round(movieResponses.Select(r => r.Ranking).Average());

           return new MovieRankResponse()
           {
                MovieName = movieName,
               OverallRanking = overallRanking
           };
        }

        public async Task<IEnumerable<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName, CancellationToken cancellationToken)
        {
            return await _movieRankRepository.GetUsersRankedMoviesByMovieTitle(userId, movieName, cancellationToken);
        }
    }
}
