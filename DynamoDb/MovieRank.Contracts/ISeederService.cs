using System.Threading;
using System.Threading.Tasks;
using MovieRank.Contracts;

namespace MovieRank.Data
{
    public interface ISeederService
    {
        Task AddMovie(int userId, MovieRankRequest movieRankRequest, CancellationToken cancellationToken);
    }
}
