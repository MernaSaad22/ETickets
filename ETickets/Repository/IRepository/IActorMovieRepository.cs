using ETickets.Models;

namespace ETickets.Repository.IRepository
{
    public interface IActorMovieRepository : IRepository<ActorMovie>
    {
        Task<bool> CreateAsync(ActorMovie actorMovie);
        Task<IEnumerable<ActorMovie>> GetActorMoviesByActorId(int actorId);  // Add this method
       
        Task<bool> DeleteByActorIdAsync(int actorId);  // Add this method for deleting actor-movie associations
                                                       // Any other methods you might have
    }
}
