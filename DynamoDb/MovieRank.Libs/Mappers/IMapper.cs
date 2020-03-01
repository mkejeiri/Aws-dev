using System.Collections.Generic;
using MovieRank.Contracts;
using MovieRank.Infrastructure.Models;

namespace MovieRank.Infrastructure.Mappers
{
    public interface IMapper
    {
        IEnumerable<MovieResponse> ToMovieContract(IEnumerable<MovieDb> getAllItems);
        MovieResponse ToMovieContract(MovieDb movieDb);
        MovieDb ToMovieDbModel(int userId, MovieRankRequest movieRankRequest);
        MovieRankResponse ToMovieRankContract(string movieName, double overallRanking);
        MovieDb ToUpdateMovieDbModel(int userId, MovieUpdateRequest movieUpdateRequest, MovieDb movieDb);
        MovieDb FromMovieResponseToDbModel(MovieResponse movieResponse, int userId);
    }
}
