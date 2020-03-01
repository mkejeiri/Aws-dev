using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieRank.Contracts;

namespace MovieRank.Controllers
{
    [Produces("application/json")]
    [Route("movies")]
    public class MovieController : Controller
    {
        private readonly IMovieRankService _movieRankService;

        public MovieController(IMovieRankService movieRankService)
        {
            _movieRankService = movieRankService;
        }

        /// <summary>
        /// Get All Items From Database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovieResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<MovieResponse>> GetAllItemsFromDatabase()
        {
            var results = await _movieRankService.GetAllItemsFromDatabase(CancellationToken.None);
            return results;
        }

        /// <summary>
        /// Get Users Ranked Movies By Movie Title
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}/{movieName}")]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName)
        {
            var movieResponse = await _movieRankService.GetMovie(userId, movieName, CancellationToken.None);
            return movieResponse;
        }
        /// <summary>
        /// Get Movie
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}/ranked-movies/{movieName}")]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMovie(int userId, string movieName)
        {
            var results = await _movieRankService.GetMovie(userId, movieName, CancellationToken.None);

            return Ok(results);
        }
        /// <summary>
        /// Add Movie
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="movieRankRequest"></param>
        /// <returns></returns>
        [HttpPost("{userId}")]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMovie([FromRoute]int userId, [FromBody] MovieRankRequest movieRankRequest)
        {
            await _movieRankService.AddMovie(userId, movieRankRequest, CancellationToken.None);
            return Ok();
        }

        /// <summary>
        /// Update Movie
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="movieName"></param>
        /// <param name="movieUpdateRankingRequest"></param>
        /// <returns></returns>
        [HttpPatch("{userId}/{movieName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMovie([FromRoute]int userId, [FromRoute]string movieName, [FromBody] MovieUpdateRankingRequest movieUpdateRankingRequest)
        {
            await _movieRankService.UpdateMovie(userId, movieName, movieUpdateRankingRequest, CancellationToken.None);
            return Ok();
        }


        /// <summary>
        /// Get Movie Ranking
        /// </summary>
        /// <returns></returns>
        [HttpGet("{movieName}/ranking")]
        [ProducesResponseType(typeof(MovieRankResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieRankResponse>> GetMovieRanking(string movieName)
        {
            var movieRank = await _movieRankService.GetMovieRank(movieName, CancellationToken.None);
            return Ok(movieRank);
        }
    }
}