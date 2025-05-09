using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETickets.ViewModels;
using System.Linq.Expressions;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMovieRepository _movieRepository;

        public CategoryController(ICategoryRepository categoryRepository, IMovieRepository movieRepository)
        {
            _categoryRepository = categoryRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> CIndex()
        {
            var categories = await _categoryRepository
                .Categories
                .Include(c => c.Movies)
                .ToListAsync();

            return View(categories);
        }
     

[HttpGet]
    public async Task<IActionResult> Create()
    {
        var allMovies = await _movieRepository.GetAsync(); // Assuming GetAsync gets all movies
        var viewModel = new CategoryCreateViewModel
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
        public async Task<IActionResult> Create(CategoryCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var allMovies = await _movieRepository.GetAsync();
                viewModel.Movies = allMovies.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                }).ToList();
                return View(viewModel);
            }

            // Step 1: Create the category (without assigning movies)
            var category = new Category
            {
                Name = viewModel.Name,
                Movies = new List<Movie>() // optional
            };

            var success = await _categoryRepository.CreateAsync(category);

            if (!success)
            {
                ModelState.AddModelError("", "Could not create category.");
                return View(viewModel);
            }

            // Step 2: Update selected movies to link them to this new category
            foreach (var movieId in viewModel.SelectedMovieIds)
            {
                var movie =  _movieRepository.GetOne(m => m.Id == movieId);
                if (movie != null)
                {
                    movie.CategoryId = category.Id;
                    await _movieRepository.EditAsync(movie);
                }
            }

            return RedirectToAction(nameof(CIndex));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category =  _categoryRepository.GetOne(c => c.Id == id, new Expression<Func<Category, object>>[] { c => c.Movies });

            if (category == null)
            {
                return NotFound();
            }

            var allMovies = await _movieRepository.GetAsync();

            var viewModel = new CategoryCreateViewModel
            {
                Name = category.Name,
                SelectedMovieIds = category.Movies.Select(m => m.Id).ToList(),
                Movies = allMovies.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString(),
                    Selected = category.Movies.Any(cm => cm.Id == m.Id)
                }).ToList()
            };

            ViewData["CategoryId"] = id;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var allMovies = await _movieRepository.GetAsync();
                viewModel.Movies = allMovies.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                }).ToList();
                return View(viewModel);
            }

            var category =  _categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = viewModel.Name;

            // Unlink old movies from the category
            var allCategoryMovies = await _movieRepository.GetAsync(m => m.CategoryId == category.Id);
            foreach (var movie in allCategoryMovies)
            {
                movie.CategoryId = 0; // or some default/null value
                await _movieRepository.EditAsync(movie);
            }

            // Link selected movies
            foreach (var movieId in viewModel.SelectedMovieIds)
            {
                var movie =  _movieRepository.GetOne(m => m.Id == movieId);
                if (movie != null)
                {
                    movie.CategoryId = category.Id;
                    await _movieRepository.EditAsync(movie);
                }
            }

            await _categoryRepository.EditAsync(category);

            return RedirectToAction(nameof(CIndex));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = _categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var success = await _categoryRepository.DeleteAsync(category);

            if (success)
            {
                return RedirectToAction(nameof(CIndex));
            }

            ModelState.AddModelError("", "Failed to delete category.");
            return RedirectToAction(nameof(CIndex));
        }






    }
}
