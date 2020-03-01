using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using MovieRank.Contracts;

namespace MovieRank.Infrastructure.Mappers
{
    public class LowLevelModelMapper : ILowLevelModelMapper
    {

       public IEnumerable<MovieResponse> ToMovieContract(ScanResponse response)
        {
            return response.Items.Select(ToMovieContract);
        }

        public IEnumerable<MovieResponse> ToMovieContract(QueryResponse response)
        {
            return response.Items.Select(ToMovieContract);
        }

        private MovieResponse ToMovieContract(Dictionary<string, AttributeValue> item)
        {
            return MovieResponse(item);
        }
        public MovieResponse ToMovieContract(GetItemResponse response)
        {
            return MovieResponse(response.Item);
        }

        private static MovieResponse MovieResponse(Dictionary<string, AttributeValue> item)
        {
            return new MovieResponse
            {
                UserId = item.TryGetValue("UserId", out var value) ? Convert.ToInt32(value.N) : 0,
                MovieName = item.TryGetValue("MovieName", out value) ? value.S : String.Empty,
                Description = item.TryGetValue("Description", out value) ? value.S : String.Empty,
                Actors = item.TryGetValue("Actors", out value) ? value.SS : new List<string>(),
                Ranking = item.TryGetValue("Ranking", out value) ? Convert.ToInt32(value.N) : 0,
                RankedDateTime = item.TryGetValue("RankedDateTime", out value) ? value.S : String.Empty
            };
        }
    }
}
