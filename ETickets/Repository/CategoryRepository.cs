using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace ETickets.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable<Category> Categories => _context.Categories.AsQueryable();

        public async Task<bool> CreateAsync(Category entity)
        {
            try
            {
                // Use _context.Set<T>() to get the DbSet for Category
                await _context.Set<Category>().AddAsync(entity);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return true;  // Return true if successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;  // Return false if an error occurs
            }


        }

    }
}
