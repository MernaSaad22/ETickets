using ETickets.Models;
namespace ETickets.Repository.IRepository
{
    public interface IMovieRepository : IRepository<Movie>
    {

        Movie? GetMovieWithActorsById(int id);
        Task<Movie?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Movie movie);  // Add this method signature
    }
}
