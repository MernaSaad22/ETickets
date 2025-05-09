using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;


namespace ETickets.Repository
{
    public class ActorRepository :Repository<Actor>, IActorRepository
    {
        private readonly ApplicationDbContext _context;

    public ActorRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

        public Actor? GetActorWMovies(int id)
        {
            return _context.Actors
                .Include(a => a.ActorMovies)
                    .ThenInclude(am => am.Movie)
                .FirstOrDefault(a => a.Id == id);
        }
        public async Task<Actor> GetByIdAsync(int id)
        {
            return await _context.Actors.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> UpdateAsync(Actor actor)
        {
            _context.Actors.Update(actor);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Actor>> GetAsync()
        {
            return await _context.Actors.ToListAsync();  // Fetch all actors
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return false;

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
