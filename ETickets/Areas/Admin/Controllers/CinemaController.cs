using ETickets.Models;
using ETickets.Repository.IRepository;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IMovieRepository _movieRepository;

        public CinemaController(ICinemaRepository cinemaRepository, IMovieRepository movieRepository)
        {
            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> CiIndex()
        {
            var cinemas = await _cinemaRepository.GetAsync(includes: new[] { (System.Linq.Expressions.Expression<Func<Cinema, object>>)(c => c.Movies) });
            return View(cinemas);
        }

        public async Task<IActionResult> CreateC()
        {
            var allMovies = await _movieRepository.GetAsync();
            var viewModel = new CinemaCreateViewModel
            {
                Movies = allMovies.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateC(CinemaCreateViewModel viewModel)
        {
            // Log the type of the view model being passed to the view
            Console.WriteLine(viewModel.GetType());

            if (ModelState.IsValid)
            {
                var selectedMovies = new List<Movie>();

                foreach (var movieId in viewModel.SelectedMovieIds)
                {
                    var movie = await _movieRepository.GetByIdAsync(movieId);
                    if (movie != null)
                    {
                        selectedMovies.Add(movie);
                    }
                }

                var cinema = new Cinema
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    CinemaLogo = viewModel.CinemaLogo,
                    Address = viewModel.Address,
                    Movies = selectedMovies
                };

                var success = await _cinemaRepository.CreateAsync(cinema);
                if (success)
                {
                    return RedirectToAction(nameof(CiIndex));
                }

                ModelState.AddModelError("", "Failed to create cinema.");
            }

            // If model is invalid, repopulate movie list for the dropdown
            var allMovies = await _movieRepository.GetAsync();
            viewModel.Movies = allMovies.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            return View(viewModel); // Pass the viewModel back to the view
        }

    }
}
