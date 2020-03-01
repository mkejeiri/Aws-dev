namespace MovieRank.Contracts
{
    //Supports all operations including create and delete table (low level model only!)
    public interface IAllOperationMovieRankRepository: IMovieRankRepository, IDDLMovieRankRepository {}
}