using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
       


        private readonly IActorRepository _actorRepository;

        public ActorController(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public IActionResult ADetails([FromRoute] int id)
        {
            var actor = _actorRepository.GetActorWMovies(id);

            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
