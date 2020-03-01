using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using MovieRank.Contracts;

namespace MovieRank.Infrastructure.Mappers
{
    public interface ILowLevelModelMapper
    {
        IEnumerable<MovieResponse> ToMovieContract(ScanResponse scanResponse);
        IEnumerable<MovieResponse> ToMovieContract(QueryResponse response);
        MovieResponse ToMovieContract(GetItemResponse getItemResponse);
    }
}