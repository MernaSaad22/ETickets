using ETickets.Data;
using ETickets.Models;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ETickets.Utility.DBInitilizer
{
    public class DBInitilizer : IDBInitilizer
    {

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitilizer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initilize()
        {
            try
            {

                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (_roleManager.Roles.IsNullOrEmpty())
                {
                    _roleManager.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Company)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Customer)).GetAwaiter().GetResult();
                }



                _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = "Admin@gmail.com",
                    UserName = "Admin"
                    
                }, "Admin123$").GetAwaiter().GetResult();

                var user = _userManager.FindByEmailAsync("Admin@gmail.com").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.SuperAdmin).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
