using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace ETickets.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Movie? GetMovieWithActorsById(int id)
        {
            return _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Actor)
                .FirstOrDefault(m => m.Id == id);
        }
        public async Task<Movie?> GetByIdAsync(int id)
        {
            return await _context.Movies
                                 .Include(m => m.Cinema)
                                 .Include(m => m.Category)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CreateAsync(Movie movie)
        {
            try
            {
                await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAsync(Movie movie)
        {
            try
            {
                _context.Movies.Update(movie);  // Update the movie in the DbContext
                await _context.SaveChangesAsync();  // Save changes to the database
                return true;  // Return true if update is successful
            }
            catch
            {
                return false;  // Return false if there is an exception
            }
        }
        public async Task<IEnumerable<Movie>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Movies
                                 .Where(m => ids.Contains(m.Id))
                                 .ToListAsync();
        }
        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies.ToListAsync();
        }

    }
}
