using ETickets.Models;
using ETickets.Repository.IRepository;
using ETickets.Utility;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin},{SD.Company}")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IActorRepository _actorRepository;

        public MovieController(IMovieRepository movieRepository,
                                ICategoryRepository categoryRepository,
                                ICinemaRepository cinemaRepository,
                                IActorRepository actorRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
        }
        public async Task<IActionResult> MOIndex()
        {
            var movies = await _movieRepository.GetAsync(includes: new Expression<Func<Movie, object>>[] { m => m.Category });
            return View(movies);
        }

        //public IActionResult MOCreate()
        //{
        //    // Get lists of categories, cinemas, and actors
        //    var categories = _categoryRepository.GetAsync().Result;
        //    var cinemas = _cinemaRepository.GetAsync().Result;
        //    var actors = _actorRepository.GetAsync().Result;

        //    // Pass these lists to the View
        //    ViewBag.Categories = categories;
        //    ViewBag.Cinemas = cinemas;
        //    ViewBag.Actors = actors;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Movie movie, List<int> selectedActors)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Handle file upload for movie image
        //        if (Request.Form.Files.Count > 0)
        //        {
        //            var file = Request.Form.Files[0];
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Movies", file.FileName);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }

        //            movie.ImgUrl = "/images/MovieImg/" + file.FileName; // Store the file path in the movie's ImgUrl
        //        }

        //        // Assign selected actors to the movie
        //        if (selectedActors != null)
        //        {
        //            movie.ActorMovies = selectedActors.Select(actorId => new ActorMovie
        //            {
        //                ActorId = actorId,
        //                MovieId = movie.Id
        //            }).ToList();
        //        }

        //        // Create the movie
        //        var result = _movieRepository.CreateAsync(movie).Result;

        //        if (result)
        //        {
        //            TempData["SuccessMessage"] = "Movie created successfully!";
        //            return RedirectToAction("MOIndex");
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Error occurred while creating the movie.";
        //        }
        //    }

        //    // If the model is not valid, show the form again with error messages
        //    var categories = _categoryRepository.GetAsync().Result;
        //    var cinemas = _cinemaRepository.GetAsync().Result;
        //    var actors = _actorRepository.GetAsync().Result;
        //    ViewBag.Categories = categories;
        //    ViewBag.Cinemas = cinemas;
        //    ViewBag.Actors = actors;

        //    return View(movie);
        //}

        public async Task<IActionResult> Create()
        {
            // Get a list of actors for the dropdown
            var actors = await _actorRepository.GetAsync();
            ViewBag.Actors = new SelectList(actors, "Id", "FirstName");

            // Get cinemas and categories for dropdowns
            var cinemas = await _cinemaRepository.GetAsync();
            ViewBag.Cinemas = new SelectList(cinemas, "Id", "Name");

            var categories = await _categoryRepository.GetAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imgFilePath = null;
                string trailerFilePath = null;

                // Handle movie image file upload
                if (model.ImgUrl != null && model.ImgUrl.Length > 0)
                {
                    // Specify the folder for storing images (e.g., wwwroot/images/movies)
                    string imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieImg");

                    // Ensure the directory exists
                    if (!Directory.Exists(imgFolder))
                    {
                        Directory.CreateDirectory(imgFolder);
                    }

                    // Generate a unique file name
                    string imgUniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImgUrl.FileName);
                    imgFilePath = Path.Combine(imgFolder, imgUniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(imgFilePath, FileMode.Create))
                    {
                        await model.ImgUrl.CopyToAsync(fileStream);
                    }

                    // Set the relative file path for database
                    imgFilePath = imgUniqueFileName;
                }

                // Handle movie trailer file upload
                if (model.TrailerUrl != null && model.TrailerUrl.Length > 0)
                {
                    // Specify the folder for storing trailer videos (e.g., wwwroot/videos/trailers)
                    string trailerFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");

                    // Ensure the directory exists
                    if (!Directory.Exists(trailerFolder))
                    {
                        Directory.CreateDirectory(trailerFolder);
                    }

                    // Generate a unique file name
                    string trailerUniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.TrailerUrl.FileName);
                    trailerFilePath = Path.Combine(trailerFolder, trailerUniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(trailerFilePath, FileMode.Create))
                    {
                        await model.TrailerUrl.CopyToAsync(fileStream);
                    }

                    // Set the relative file path for database
                    trailerFilePath = trailerUniqueFileName;
                }

                // Create the Movie object and populate its properties
                var movie = new Movie
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ImgUrl = imgFilePath,   // Store the relative file path for the image
                    TrailerUrl = trailerFilePath,   // Store the relative file path for the trailer
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = model.Status,
                    CinemaId = model.CinemaId,
                    CategoryId = model.CategoryId,
                    ActorMovies = new List<ActorMovie>()
                };

                // Add actors if any
                foreach (var actorId in model.ActorIds)
                {
                    movie.ActorMovies.Add(new ActorMovie
                    {
                        ActorId = actorId,
                        Movie = movie
                    });
                }

                // Save the movie
                var movieCreated = await _movieRepository.CreateAsync(movie);
                if (movieCreated)
                {
                    return RedirectToAction("MOIndex"); // Or wherever you want to redirect
                }
            }

            // If model state is invalid, reload the dropdowns
            ViewBag.Cinemas = new SelectList(await _cinemaRepository.GetAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAsync(), "Id", "Name");
            ViewBag.Actors = new SelectList(await _actorRepository.GetAsync(), "Id", "FirstName");

            return View(model);
        }

        //public async Task<IActionResult> Edit(int id)
        //{
        //    var movie = await _movieRepository.GetByIdAsync(id);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    var viewModel = new MovieEditViewModel
        //    {
        //        Id = movie.Id,
        //        Name = movie.Name,
        //        Description = movie.Description,
        //        Price = movie.Price,
        //        ExistingImgUrl = movie.ImgUrl,  // Show the current image URL
        //        ExistingTrailerUrl = movie.TrailerUrl,  // Show the current trailer URL
        //        StartDate = movie.StartDate,
        //        EndDate = movie.EndDate,
        //        Status = movie.Status,
        //        CinemaId = movie.CinemaId,
        //        CategoryId = movie.CategoryId,
        //        SelectedActorIds = movie.ActorMovies?.Select(am => am.ActorId).ToList() ?? new List<int>()  // Get the selected actor IDs
        //    };

        //    // Populate dropdowns
        //    ViewBag.Cinemas = new SelectList(await _cinemaRepository.GetAsync(), "Id", "Name", movie.CinemaId);
        //    ViewBag.Categories = new SelectList(await _categoryRepository.GetAsync(), "Id", "Name", movie.CategoryId);
        //    ViewBag.Actors = new SelectList(await _actorRepository.GetAsync(), "Id", "FirstName", viewModel.SelectedActorIds);

        //    return View(viewModel);
        //}

        //// Edit movie form submission (POST request)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, MovieEditViewModel viewModel)
        //{
        //    if (id != viewModel.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var movie = await _movieRepository.GetByIdAsync(id);
        //        if (movie == null)
        //        {
        //            return NotFound();
        //        }

        //        // Process new image if provided
        //        if (viewModel.NewImgUrl != null && viewModel.NewImgUrl.Length > 0)
        //        {
        //            // Define the folder to save the image
        //            string imgFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieImg");

        //            // Create the folder if it doesn't exist
        //            if (!Directory.Exists(imgFolderPath))
        //            {
        //                Directory.CreateDirectory(imgFolderPath);
        //            }

        //            // Generate a unique filename for the image
        //            string imgUniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewImgUrl.FileName);
        //            string imgFilePath = Path.Combine(imgFolderPath, imgUniqueFileName);

        //            // Save the image to the server
        //            using (var fileStream = new FileStream(imgFilePath, FileMode.Create))
        //            {
        //                await viewModel.NewImgUrl.CopyToAsync(fileStream);
        //            }

        //            // Update the movie's ImgUrl property with the new file path
        //            movie.ImgUrl =  imgUniqueFileName;
        //        }

        //        // Process new trailer if provided
        //        if (viewModel.NewTrailerUrl != null && viewModel.NewTrailerUrl.Length > 0)
        //        {
        //            // Define the folder to save the trailer
        //            string trailerFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");

        //            // Create the folder if it doesn't exist
        //            if (!Directory.Exists(trailerFolderPath))
        //            {
        //                Directory.CreateDirectory(trailerFolderPath);
        //            }

        //            // Generate a unique filename for the trailer
        //            string trailerUniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewTrailerUrl.FileName);
        //            string trailerFilePath = Path.Combine(trailerFolderPath, trailerUniqueFileName);

        //            // Save the trailer to the server
        //            using (var fileStream = new FileStream(trailerFilePath, FileMode.Create))
        //            {
        //                await viewModel.NewTrailerUrl.CopyToAsync(fileStream);
        //            }

        //            // Update the movie's TrailerUrl property with the new file path
        //            movie.TrailerUrl =  trailerUniqueFileName;
        //        }

        //        // Update the movie properties
        //        movie.Name = viewModel.Name;
        //        movie.Description = viewModel.Description;
        //        movie.Price = viewModel.Price;
        //        movie.StartDate = viewModel.StartDate;
        //        movie.EndDate = viewModel.EndDate;
        //        movie.Status = viewModel.Status;
        //        movie.CinemaId = viewModel.CinemaId;
        //        movie.CategoryId = viewModel.CategoryId;

        //        // Remove old actor associations and add new ones
        //        movie.ActorMovies.Clear();
        //        foreach (var actorId in viewModel.SelectedActorIds)
        //        {
        //            movie.ActorMovies.Add(new ActorMovie
        //            {
        //                ActorId = actorId,
        //                MovieId = movie.Id
        //            });
        //        }

        //        // Save the updated movie
        //        var result = await _movieRepository.UpdateAsync(movie);
        //        if (result)
        //        {
        //            TempData["SuccessMessage"] = "Movie updated successfully!";
        //            return RedirectToAction("MOIndex"); // Redirect to movie index or list
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "An error occurred while updating the movie.";
        //        }
        //    }

        //    // If the model state is invalid, reload dropdowns and return to the view with validation errors
        //    ViewBag.Cinemas = new SelectList(await _cinemaRepository.GetAsync(), "Id", "Name", viewModel.CinemaId);
        //    ViewBag.Categories = new SelectList(await _categoryRepository.GetAsync(), "Id", "Name", viewModel.CategoryId);
        //    ViewBag.Actors = new SelectList(await _actorRepository.GetAsync(), "Id", "FirstName", viewModel.SelectedActorIds);

        //    return View(viewModel);
        //}


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = _movieRepository.GetOne(
                m => m.Id == id,
                includes: new Expression<Func<Movie, object>>[] { m => m.ActorMovies }
            );

            if (movie == null)
            {
                return NotFound();
            }

            // Get all actors, cinemas, and categories
            var actors = await _actorRepository.GetAsync();
            var cinemas = await _cinemaRepository.GetAsync();
            var categories = await _categoryRepository.GetAsync();

            // Map to ViewModel
            var model = new MovieEditViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                Status = movie.Status,
                CinemaId = movie.CinemaId,
                CategoryId = movie.CategoryId,
                ExistingImageUrl = movie.ImgUrl,
                ExistingTrailerUrl = movie.TrailerUrl,
                ActorIds = movie.ActorMovies?.Select(am => am.ActorId).ToList() ?? new List<int>() // Ensure non-null
            };

            // Create SelectList for actors (combine FirstName + LastName)
            ViewBag.Actors = actors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.FirstName} {a.LastName}",
                Selected = model.ActorIds.Contains(a.Id)
            }).ToList();

            ViewBag.Cinemas = new SelectList(cinemas, "Id", "Name", model.CinemaId);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns if validation fails
                var actors = await _actorRepository.GetAsync();
                var cinemas = await _cinemaRepository.GetAsync();
                var categories = await _categoryRepository.GetAsync();

                ViewBag.Actors = new MultiSelectList(actors, "Id", "FullName", model.ActorIds);
                ViewBag.Cinemas = new SelectList(cinemas, "Id", "Name", model.CinemaId);
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);

                return View(model);
            }

            var movie = _movieRepository.GetOne(m => m.Id == model.Id, includes: new Expression<Func<Movie, object>>[] { m => m.ActorMovies });

            if (movie == null)
            {
                return NotFound();
            }

            // Update basic properties
            movie.Name = model.Name;
            movie.Description = model.Description;
            movie.Price = model.Price;
            movie.StartDate = model.StartDate;
            movie.EndDate = model.EndDate;
            movie.Status = model.Status;
            movie.CinemaId = model.CinemaId;
            movie.CategoryId = model.CategoryId;

            // Handle image update (if new image is uploaded)
            if (model.ImgUrl != null && model.ImgUrl.Length > 0)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(movie.ImgUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieImg", movie.ImgUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Upload new image
                string imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieImg");
                string imgUniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImgUrl.FileName);
                string imgFilePath = Path.Combine(imgFolder, imgUniqueFileName);

                using (var fileStream = new FileStream(imgFilePath, FileMode.Create))
                {
                    await model.ImgUrl.CopyToAsync(fileStream);
                }

                movie.ImgUrl = imgUniqueFileName;
            }

            // Handle trailer update (if new trailer is uploaded)
            if (model.TrailerUrl != null && model.TrailerUrl.Length > 0)
            {
                // Delete old trailer if it exists
                if (!string.IsNullOrEmpty(movie.TrailerUrl))
                {
                    var oldTrailerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", movie.TrailerUrl);
                    if (System.IO.File.Exists(oldTrailerPath))
                    {
                        System.IO.File.Delete(oldTrailerPath);
                    }
                }

                // Upload new trailer
                string trailerFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
                string trailerUniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.TrailerUrl.FileName);
                string trailerFilePath = Path.Combine(trailerFolder, trailerUniqueFileName);

                using (var fileStream = new FileStream(trailerFilePath, FileMode.Create))
                {
                    await model.TrailerUrl.CopyToAsync(fileStream);
                }

                movie.TrailerUrl = trailerUniqueFileName;
            }

            // Update actors
            if (model.ActorIds != null)
            {
                // Remove existing actor-movie relationships
                if (movie.ActorMovies != null)
                {
                    foreach (var existingActorMovie in movie.ActorMovies.ToList())
                    {
                        movie.ActorMovies.Remove(existingActorMovie);
                    }
                }

                // Add new actor-movie relationships
                foreach (var actorId in model.ActorIds)
                {
                    movie.ActorMovies.Add(new ActorMovie { ActorId = actorId });
                }
            }

            // Save changes
            var result = await _movieRepository.EditAsync(movie);
            if (result)
            {
                return RedirectToAction("MOIndex");
            }

            ModelState.AddModelError("", "Failed to update movie.");
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Remove image file if exists
            if (!string.IsNullOrEmpty(movie.ImgUrl))
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieImg", movie.ImgUrl);
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }

            // Remove trailer file if exists
            if (!string.IsNullOrEmpty(movie.TrailerUrl))
            {
                var trailerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", movie.TrailerUrl);
                if (System.IO.File.Exists(trailerPath))
                {
                    System.IO.File.Delete(trailerPath);
                }
            }

            // Delete movie from database
            var result = await _movieRepository.DeleteAsync(movie);
            if (result)
            {
                TempData["SuccessMessage"] = "Movie and associated files deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting movie.";
            }

            return RedirectToAction("MOIndex");
        }

    }
}
