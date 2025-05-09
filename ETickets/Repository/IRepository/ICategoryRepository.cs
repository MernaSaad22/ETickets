using ETickets.Models;
namespace ETickets.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        IQueryable<Category> Categories { get; }
    }
}
