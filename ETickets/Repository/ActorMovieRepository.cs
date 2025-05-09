using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Repository
{
    public class ActorMovieRepository : Repository<ActorMovie>, IActorMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public ActorMovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(ActorMovie actorMovie)
        {
            try
            {
                await _context.ActorMovies.AddAsync(actorMovie);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<IEnumerable<ActorMovie>> GetActorMoviesByActorId(int actorId)
        {
            return await _context.ActorMovies.Where(am => am.ActorId == actorId).ToListAsync();
        }

        public async Task<bool> DeleteByActorIdAsync(int actorId)
        {
            var actorMovies = await _context.ActorMovies.Where(am => am.ActorId == actorId).ToListAsync();
            _context.ActorMovies.RemoveRange(actorMovies);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
