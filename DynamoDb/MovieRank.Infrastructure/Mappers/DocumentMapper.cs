using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using MovieRank.Contracts;

namespace MovieRank.Infrastructure.Mappers
{
    public class DocumentMapper : IDocumentMapper
    {
        public List<MovieResponse> ToMovieContract(List<Document> getAllItems) => getAllItems.Select(ToMovieContract).ToList();

        public MovieResponse ToMovieContract(Document document) =>
            new MovieResponse
            {
                UserId = document.TryGetValue("UserId", out var value) ? Convert.ToInt32(value) : 0,
                MovieName = document.TryGetValue("MovieName", out value) ? value.AsString() : String.Empty,
                Description = document.TryGetValue("Description", out value) ? value.AsString() : String.Empty,
                Actors = document.TryGetValue("Actors", out value) ? value.AsListOfString() : new List<string>(),
                Ranking = document.TryGetValue("Ranking", out value) ? Convert.ToInt32(value) : 0,
                RankedDateTime = document.TryGetValue("RankedDateTime", out value) ? value.AsString() : String.Empty,
            };

        public Document ToMovieDbModel(int userId, MovieRankRequest movieRankRequest)
        {
            return new Document
            {
                ["UserId"] = userId,
                ["MovieName"] = movieRankRequest.MovieName,
                ["Description"] = movieRankRequest.Description,
                ["Actors"] = movieRankRequest.Actors,
                ["RankedDateTime"] = DateTime.UtcNow.ToString(),
                ["Ranking"] = movieRankRequest.Ranking
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

        public Document ToUpdateMovieDbModel(int userId, MovieUpdateRequest movieUpdateRequest, Document document)
        {
            return new Document
            {
                ["UserId"] = userId,
                ["MovieName"] = document.TryGetValue("MovieName", out var value) ? value.AsString() : String.Empty,
                ["Description"] = document.TryGetValue("Description", out value) ? value.AsString() : String.Empty,
                ["Actors"] = document.TryGetValue("Actors", out value) ? value.AsListOfString() : new List<string>(),
                ["Ranking"] = movieUpdateRequest.Ranking,
                ["RankedDateTime"] = DateTime.UtcNow.ToString(),
            };
        }

        public Document FromMovieResponseToDbModel(MovieResponse movieResponse, int userId)
        {
            return new Document
            {
                ["UserId"] = userId,
                ["MovieName"] = movieResponse.MovieName,
                ["Description"] = movieResponse.Description,
                ["Actors"] = movieResponse.Actors,
                ["Ranking"] = movieResponse.Ranking,
                ["RankedDateTime"] = DateTime.UtcNow.ToString(),
            };
        }
    }
}
