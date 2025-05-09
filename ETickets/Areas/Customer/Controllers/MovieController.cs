using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ETickets.Areas.Customer.Controllers
{
     [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;

        public MovieController(IMovieRepository movieRepository) {
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> MIndex(string search,int page = 1)
        {
            const int pageSize = 6;

            var allMovies = await _movieRepository.GetAsync(
                includes: new Expression<Func<Movie, object>>[]
                {
            m => m.Cinema,
            m => m.Category
                });

            if (!string.IsNullOrEmpty(search))
            {
                allMovies = allMovies
                    .Where(m => m.Name.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            var totalMovies = allMovies.Count();
            var moviesOnPage = allMovies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
            ViewBag.Search = search;
            return View(moviesOnPage);
        }
        //public IActionResult MDetails([FromRoute]int id)
        //{
        //    var movie = _movieRepository.GetOne(
        //        expression: m => m.Id == id,
        //        includes: new Expression<Func<Movie, object>>[]
        //        {
        //     m => m.Cinema,
        //     m => m.Category

        //        });

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(movie);
        //}

        public IActionResult MDetails([FromRoute] int id)
        {
            var movie = _movieRepository.GetMovieWithActorsById(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }




    }
}
