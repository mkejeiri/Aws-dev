using System;
using System.Collections.Generic;
using System.Linq;
using MovieRank.Contracts;
using MovieRank.Infrastructure.Models;

namespace MovieRank.Infrastructure.Mappers
{
    public class Mapper : IMapper
    {
        public IEnumerable<MovieResponse> ToMovieContract(IEnumerable<MovieDb> getAllItems)
        {
            return getAllItems.Select(MovieResponse);
        }


        private static MovieResponse MovieResponse(MovieDb c)
        {
            if (c == null)
            {
                return new MovieResponse();
            }
            var response = new MovieResponse
            {
                UserId = c.UserId,
                Description = c.Description,
                Actors = c.Actors,
                RankedDateTime = c.RankedDateTime,
                MovieName = c.MovieName,
                Ranking = c.Ranking
            };
            return response;
        }

        public MovieResponse ToMovieContract(MovieDb movieDb)
        {
            return MovieResponse(movieDb);
        }


        public MovieDb FromMovieResponseToDbModel(MovieResponse movieDb, int userId)
        {
            return MovieResponseToDbModel(movieDb, userId);
        }

        private MovieDb MovieResponseToDbModel(MovieResponse movieResponse, int userId)
        {
            return new MovieDb
            {
                UserId = userId,
                Description = movieResponse.Description,
                Actors = movieResponse.Actors,
                Ranking = movieResponse.Ranking,
                MovieName = movieResponse.MovieName,
                RankedDateTime = movieResponse.RankedDateTime
            };
        }

        public MovieDb ToMovieDbModel(int userId, MovieRankRequest movieRankRequest)
        {
            return new MovieDb
            {
                UserId = userId,
                Description = movieRankRequest.Description,
                Actors = movieRankRequest.Actors,
                Ranking = movieRankRequest.Ranking,
                MovieName = movieRankRequest.MovieName,
                RankedDateTime = DateTime.UtcNow.ToString()
            };
        }

        public MovieRankResponse ToMovieRankContract(string movieName, double overallRanking)
        {
            return new MovieRankResponse
            {
                MovieName = movieName,
                OverallRanking = overallRanking
            };
        }

        public MovieDb ToUpdateMovieDbModel(int userId, MovieUpdateRequest movieUpdateRequest, MovieDb movieDb)
        {
            return new MovieDb
            {
                UserId = userId,
                Description = movieDb.Description,
                Actors = movieDb.Actors,
                Ranking = movieUpdateRequest.Ranking,
                MovieName = movieDb.MovieName,
                RankedDateTime = DateTime.UtcNow.ToString()
            };
        }
    }
}
