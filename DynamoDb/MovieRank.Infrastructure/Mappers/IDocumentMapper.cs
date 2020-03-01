using System.Collections.Generic;
using Amazon.DynamoDBv2.DocumentModel;
using MovieRank.Contracts;
using MovieRank.Infrastructure.Models;

namespace MovieRank.Infrastructure.Mappers
{
    public interface IDocumentMapper
    {
        List<MovieResponse> ToMovieContract(List<Document> getAllItems);
        MovieResponse ToMovieContract(Document document);
        Document ToMovieDbModel(int userId, MovieRankRequest movieRankRequest);
        MovieRankResponse ToMovieRankContract(string movieName, double overallRanking);
        Document ToUpdateMovieDbModel(int userId, MovieUpdateRequest movieUpdateRequest, Document document);
        Document FromMovieResponseToDbModel(MovieResponse movieResponse, int userId);
    }
}
