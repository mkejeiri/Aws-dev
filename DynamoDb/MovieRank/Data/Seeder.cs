using System.Collections.Generic;
using System.Threading;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using MovieRank.Contracts;
using MovieRank.Services;
using Newtonsoft.Json;

namespace MovieRank.Data
{
    public class Seeder
    {
        private readonly IMovieRankService _rankService;

        public Seeder(IMovieRankService rankService)
        {
            _rankService = rankService;
        }

        public void Run(bool seed)
        {
            if (seed)
            {
                var data = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var movies = JsonConvert.DeserializeObject<List<Movie>>(data);

                //create user in the table 
                foreach (var movie in movies)
                {
                    _rankService.AddMovie(movie.UserId, new MovieRankRequest()
                    {
                        MovieName = movie.MovieName,
                        Actors = movie.Actors,
                        Description = movie.Description,
                        Ranking = movie.Ranking,
                    }, CancellationToken.None).Wait();
                }
            }
        }
    }

    internal class Movie
    {
        public int UserId { get; set; }
        public string MovieName { get; set; }
        public string Description { get; set; }
        public List<string> Actors { get; set; }
        public int Ranking { get; set; }
        public string RankedDateTime { get; set; }
    }
}
