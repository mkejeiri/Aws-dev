using System.Threading.Tasks;

namespace MovieRank.Contracts
{
    //This uses only in low level model scenarios with the 2 extra unsupported operation by
    //persistence object model and document model
    public interface IDDLMovieRankRepository
    {
        //Those are implemented  only in low level
        Task CreateDynamoTable(string tableName);
        Task DeleteDynamoDbTable(string tableName);
    }
}