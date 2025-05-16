using ETickets.Models;
using ETickets.Repository.IRepository;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CinemaController> _logger;

        public CinemaController(ICinemaRepository cinemaRepository, IMovieRepository movieRepository, IWebHostEnvironment webHostEnvironment, ILogger<CinemaController> logger)
        {
            _cinemaRepository = cinemaRepository;
            _movieRepository = movieRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<IActionResult> CiIndex()
        {
            var cinemas = await _cinemaRepository.GetAsync(includes: new[] { (System.Linq.Expressions.Expression<Func<Cinema, object>>)(c => c.Movies) });
            return View(cinemas);
        }

        //public async Task<IActionResult> CreateC()
        //{
        //    var allMovies = await _movieRepository.GetAsync();
        //    var viewModel = new CinemaCreateViewModel
        //    {
        //        Movies = allMovies.Select(m => new SelectListItem
        //        {
        //            Text = m.Name,
        //            Value = m.Id.ToString()
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateC(CinemaCreateViewModel viewModel)
        //{
        //    // Log the type of the view model being passed to the view
        //    Console.WriteLine(viewModel.GetType());

        //    if (ModelState.IsValid)
        //    {
        //        var selectedMovies = new List<Movie>();

        //        foreach (var movieId in viewModel.SelectedMovieIds)
        //        {
        //            var movie = await _movieRepository.GetByIdAsync(movieId);
        //            if (movie != null)
        //            {
        //                selectedMovies.Add(movie);
        //            }
        //        }

        //        var cinema = new Cinema
        //        {
        //            Name = viewModel.Name,
        //            Description = viewModel.Description,
        //            CinemaLogo = viewModel.CinemaLogo,
        //            Address = viewModel.Address,
        //            Movies = selectedMovies
        //        };

        //        var success = await _cinemaRepository.CreateAsync(cinema);
        //        if (success)
        //        {
        //            return RedirectToAction(nameof(CiIndex));
        //        }

        //        ModelState.AddModelError("", "Failed to create cinema.");
        //    }

        //    // If model is invalid, repopulate movie list for the dropdown
        //    var allMovies = await _movieRepository.GetAsync();
        //    viewModel.Movies = allMovies.Select(m => new SelectListItem
        //    {
        //        Text = m.Name,
        //        Value = m.Id.ToString()
        //    }).ToList();

        //    return View(viewModel); // Pass the viewModel back to the view
        //}
        // my creation form

        //public IActionResult Create()
        //{
        //    return View(); // must match Views/Admin/Cinema/Create.cshtml
        //}


        //[HttpPost]
        //public async Task<IActionResult> Create(Cinema cinema, IFormFile CinemaLogo)
        //{
        //    if (CinemaLogo != null && CinemaLogo.Length > 0)
        //    {
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoImg", fileName);

        //        var directory = Path.GetDirectoryName(path);
        //        if (!Directory.Exists(directory))
        //        {
        //            Directory.CreateDirectory(directory);
        //        }

        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            await CinemaLogo.CopyToAsync(stream);
        //        }

        //        cinema.CinemaLogo = fileName;
        //        await _cinemaRepository.CreateAsync(cinema);

        //        return RedirectToAction(nameof(CiIndex));
        //    }

        //    return NotFound();
        //}
        //problem with photo
        //public async Task<IActionResult> Create()
        //{
        //    var movies = await _movieRepository.GetAllAsync();

        //    var viewModel = new CinemaFormViewModel
        //    {
        //        AvailableMovies = movies.Select(m => new SelectListItem
        //        {
        //            Value = m.Id.ToString(),
        //            Text = m.Name
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create(CinemaFormViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var allMovies = await _movieRepository.GetAsync();
        //        viewModel.AvailableMovies = allMovies.Select(m => new SelectListItem
        //        {
        //            Text = m.Name,
        //            Value = m.Id.ToString()
        //        }).ToList();
        //        return View(viewModel);
        //    }

        //    string fileName = null;

        //    if (viewModel.CinemaLogo != null && viewModel.CinemaLogo.Length > 0)
        //    {
        //        fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.CinemaLogo.FileName);
        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoImg", fileName);

        //        Directory.CreateDirectory(Path.GetDirectoryName(path));

        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            await viewModel.CinemaLogo.CopyToAsync(stream);
        //        }
        //    }

        //    var cinema = new Cinema
        //    {
        //        Name = viewModel.Name,
        //        Description = viewModel.Description,
        //        Address = viewModel.Address,
        //        CinemaLogo = fileName,
        //        Movies = new List<Movie>()
        //    };

        //    var success = await _cinemaRepository.CreateAsync(cinema);
        //    if (!success)
        //    {
        //        ModelState.AddModelError("", "Could not create category.");
        //        return View(viewModel);
        //    }

        //    // Assign selected movies
        //    foreach (var movieId in viewModel.SelectedMovieIds)
        //    {
        //        var movie = _movieRepository.GetOne(m => m.Id == movieId);
        //        if (movie != null)
        //        {
        //            movie.CinemaId = cinema.Id;
        //            await _movieRepository.EditAsync(movie);
        //        }
        //    }

        //    return RedirectToAction(nameof(CiIndex));
        //}






        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var movies = await _movieRepository.GetAsync();

            var cinemaVM = new CinemaFormViewModel
            {
                AvailableMovies = movies.ToList()
            };

            return View(cinemaVM);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CinemaFormViewModel cinemaVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        cinemaVM.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
        //        return View(cinemaVM);
        //    }

        //    try
        //    {
        //        // Handle file upload - FIXED THE ISSUE HERE
        //        if (cinemaVM.LogoFile != null && cinemaVM.LogoFile.Length > 0)  // Changed from CinemaLogo.Length to LogoFile.Length
        //        {
        //            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(cinemaVM.LogoFile.FileName);
        //            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoImg", fileName);
        //            var directory = Path.GetDirectoryName(path);

        //            if (!Directory.Exists(directory))
        //            {
        //                Directory.CreateDirectory(directory);
        //            }

        //            using (var stream = new FileStream(path, FileMode.Create))
        //            {
        //                await cinemaVM.LogoFile.CopyToAsync(stream);
        //            }

        //            // FIXED: Store the full relative path, not just filename
        //            cinemaVM.CinemaLogo = "/images/LogoImg/" + fileName;
        //        }

        //        var cinema = new Cinema
        //        {
        //            Name = cinemaVM.Name,
        //            Description = cinemaVM.Description,
        //            CinemaLogo = cinemaVM.CinemaLogo,
        //            Address = cinemaVM.Address,
        //            Movies = new List<Movie>()
        //        };

        //        if (cinemaVM.MovieIds != null && cinemaVM.MovieIds.Any())
        //        {
        //            foreach (var movieId in cinemaVM.MovieIds)
        //            {
        //                var movie = _movieRepository.GetOne(m => m.Id == movieId);  // Added await
        //                if (movie != null)
        //                {
        //                    cinema.Movies.Add(movie);
        //                }
        //            }
        //        }

        //        var result = await _cinemaRepository.CreateAsync(cinema);
        //        if (!result)  // Check if creation was successful
        //        {
        //            ModelState.AddModelError("", "Failed to save cinema to database");
        //            cinemaVM.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
        //            return View(cinemaVM);
        //        }

        //        return RedirectToAction(nameof(CiIndex));
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the actual error
        //        Console.WriteLine($"Error creating cinema: {ex.Message}");
        //        ModelState.AddModelError("", "An error occurred while creating the cinema.");
        //        cinemaVM.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
        //        return View(cinemaVM);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaFormViewModel cinemaVM)
        {
            if (!ModelState.IsValid)
            {
                cinemaVM.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
                return View(cinemaVM);
            }

            try
            {
                // Handle file upload (only if logo file was provided)
                if (cinemaVM.LogoFile != null && cinemaVM.LogoFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(cinemaVM.LogoFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoImg", fileName);
                    var directory = Path.GetDirectoryName(path);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await cinemaVM.LogoFile.CopyToAsync(stream);
                    }

                    cinemaVM.CinemaLogo =  fileName;
                }
                // If no logo was uploaded, CinemaLogo remains null

                var cinema = new Cinema
                {
                    Name = cinemaVM.Name,
                    Description = cinemaVM.Description,
                    CinemaLogo = cinemaVM.CinemaLogo,  // This can be null
                    Address = cinemaVM.Address,
                    Movies = new List<Movie>()
                };

                if (cinemaVM.MovieIds != null && cinemaVM.MovieIds.Any())
                {
                    foreach (var movieId in cinemaVM.MovieIds)
                    {
                        var movie = _movieRepository.GetOne(m => m.Id == movieId);
                        if (movie != null)
                        {
                            cinema.Movies.Add(movie);
                        }
                    }
                }

                await _cinemaRepository.CreateAsync(cinema);
                return RedirectToAction(nameof(CiIndex));
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error creating cinema: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the cinema.");
                cinemaVM.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
                return View(cinemaVM);
            }
        }
        // i have a roblem in Edit Cinema

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var cinema =  _cinemaRepository.GetOne(c => c.Id == id);
                if (cinema == null) return NotFound();

                var movies = await _movieRepository.GetAsync();

                var viewModel = new CinemaFormViewModel
                {
                    Id = cinema.Id,
                    Name = cinema.Name,
                    Description = cinema.Description,
                    CinemaLogo = cinema.CinemaLogo,
                    Address = cinema.Address,
                    MovieIds = cinema.Movies?.Select(m => m.Id).ToList(),
                    AvailableMovies = movies.ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cinema edit form");
                TempData["ErrorMessage"] = "Failed to load cinema for editing";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CinemaFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
                return View(model);
            }

            try
            {
                var cinema =  _cinemaRepository.GetOne(c => c.Id == id);
                if (cinema == null) return NotFound();

                // Update basic properties
                UpdateCinemaProperties(cinema, model);

                // Handle logo upload if provided
                if (model.LogoFile != null)
                {
                    cinema.CinemaLogo = await HandleLogoUpload(model.LogoFile, cinema.CinemaLogo);
                }

                // Update movie associations
                await UpdateCinemaMovies(cinema, model.MovieIds);

                await _cinemaRepository.EditAsync(cinema);

                TempData["SuccessMessage"] = "Cinema updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cinema");
                ModelState.AddModelError("", "An error occurred while updating the cinema.");
                model.AvailableMovies = (await _movieRepository.GetAsync()).ToList();
                return View(model);
            }
        }

        private void UpdateCinemaProperties(Cinema cinema, CinemaFormViewModel model)
        {
            cinema.Name = model.Name;
            cinema.Description = model.Description;
            cinema.Address = model.Address;
        }

        private async Task<string> HandleLogoUpload(IFormFile logoFile, string existingLogoPath)
        {
            // Delete old logo if exists
            if (!string.IsNullOrEmpty(existingLogoPath))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingLogoPath.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            // Save new logo
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(logoFile.FileName)}";
            var relativePath = $"/images/LogoImg/{fileName}";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoImg", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }

            return relativePath;
        }

        private async Task UpdateCinemaMovies(Cinema cinema, List<int> selectedMovieIds)
        {
            cinema.Movies.Clear();

            if (selectedMovieIds != null && selectedMovieIds.Any())
            {
                var movies = await _movieRepository.GetAsync(m => selectedMovieIds.Contains(m.Id));
                cinema.Movies = movies.ToList();
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var cinema = _cinemaRepository.GetOne(c => c.Id == id);
        //    if (cinema == null)
        //    {
        //        return NotFound();
        //    }

        //    var result = await _cinemaRepository.DeleteAsync(cinema);
        //    if (!result)
        //    {
        //        ModelState.AddModelError("", "Failed to delete cinema.");
        //    }

        //    return RedirectToAction(nameof(CiIndex));
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema =  _cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            // Delete the logo file from wwwroot/images/LogoImg if it exists
            if (!string.IsNullOrEmpty(cinema.CinemaLogo))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoImg", cinema.CinemaLogo);

                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                        _logger.LogInformation($"Deleted logo file: {imagePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error deleting logo file: {imagePath}");
                        // Continue with deletion even if file deletion fails
                    }
                }
            }

            var result = await _cinemaRepository.DeleteAsync(cinema);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to delete cinema.";
                return RedirectToAction(nameof(CiIndex));
            }

            TempData["SuccessMessage"] = "Cinema deleted successfully.";
            return RedirectToAction(nameof(CiIndex));
        }






    }
}
