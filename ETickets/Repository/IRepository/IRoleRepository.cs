
using ETickets.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace ETickets.Repository.IRepository
{
    public interface IRoleRepository : IRepository<IdentityRole>
    {
        
    }
}