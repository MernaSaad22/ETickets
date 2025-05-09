using ETickets.Models;
namespace ETickets.Repository.IRepository
{
    public interface IActorRepository :IRepository<Actor>
    {
        public Actor? GetActorWMovies(int id);
        Task<Actor> GetByIdAsync(int id); 
        Task<IEnumerable<Actor>> GetAsync();
        Task<bool> CreateAsync(Actor actor);
        Task<bool> UpdateAsync(Actor actor);
        Task<bool> DeleteAsync(int id);

    }
}
