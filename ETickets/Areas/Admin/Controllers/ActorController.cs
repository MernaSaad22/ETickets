using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using ETickets.Utility;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class ActorController : Controller
    {
        private readonly IActorRepository _actorRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IActorMovieRepository _actorMovieRepository;

        public ActorController(IActorRepository actorRepository, IMovieRepository movieRepository, IActorMovieRepository actorMovieRepository)
        {
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
            _actorMovieRepository = actorMovieRepository;
        }

        public async Task<IActionResult> ACIndex()
        {
            var actors = await _actorRepository.GetAsync(); 
            return View(actors);
        }

        [Authorize(Roles = $"{SD.SuperAdmin}")]
        public async Task<IActionResult> Create()
        {
            var allMovies = await _movieRepository.GetAsync(); // Get all movies to populate the dropdown
            var viewModel = new ActorCreateViewModel
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
        public async Task<IActionResult> Create(ActorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // Check if a profile picture was uploaded
                if (viewModel.ProfilePicture != null && viewModel.ProfilePicture.Length > 0)
                {
                    // Specify custom folder path (e.g., inside wwwroot/images/CastImg)
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "CastImg");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);  // Create directory if it doesn't exist
                    }

                    // Generate a unique file name using GUID and the original file extension
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ProfilePicture.FileName);

                    // Define the full path for the file
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the image file to the specified path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ProfilePicture.CopyToAsync(fileStream);
                    }
                }

                // Create actor record and store the file name in the database (ProfilePicture is a string)
                var actor = new Actor
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Bio = viewModel.Bio,
                    ProfilePicture = uniqueFileName,  // Store only the file name in the database
                    News = viewModel.News
                };

                // Save the actor to the database
                var success = await _actorRepository.CreateAsync(actor);

                if (!success)
                {
                    ModelState.AddModelError("", "Could not create actor.");
                    return View(viewModel);
                }

                // Create ActorMovie associations if any
                foreach (var movieId in viewModel.SelectedMovieIds)
                {
                    var actorMovie = new ActorMovie
                    {
                        ActorId = actor.Id,
                        MovieId = movieId
                    };
                    await _actorMovieRepository.CreateAsync(actorMovie);
                }

                return RedirectToAction(nameof(ACIndex));  // Redirect to actor index page after successful creation
            }

            // If validation fails, repopulate the movie dropdown
            var allMovies = await _movieRepository.GetAsync();
            viewModel.Movies = allMovies.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            return View(viewModel);
        }


        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            var allMovies = await _movieRepository.GetAsync();
            var viewModel = new ActorEditViewModel
            {
                Id = actor.Id,
                FirstName = actor.FirstName,
                LastName = actor.LastName,
                Bio = actor.Bio,
                ExistingProfilePicture = actor.ProfilePicture,
                News = actor.News,
                SelectedMovieIds = (await _actorMovieRepository.GetActorMoviesByActorId(id))
                                    .Select(am => am.MovieId)
                                    .ToList(),
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
        public async Task<IActionResult> Edit(int id, ActorEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string uniqueFileName = viewModel.ExistingProfilePicture;

                if (viewModel.NewProfilePicture != null && viewModel.NewProfilePicture.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "CastImg");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewProfilePicture.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.NewProfilePicture.CopyToAsync(fileStream);
                    }
                }

                var actor = new Actor
                {
                    Id = id,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Bio = viewModel.Bio,
                    ProfilePicture = uniqueFileName,
                    News = viewModel.News
                };

                var success = await _actorRepository.UpdateAsync(actor);
                if (!success)
                {
                    ModelState.AddModelError("", "Could not update actor.");
                    return View(viewModel);
                }

                await _actorMovieRepository.DeleteByActorIdAsync(actor.Id);

                foreach (var movieId in viewModel.SelectedMovieIds)
                {
                    var actorMovie = new ActorMovie
                    {
                        ActorId = actor.Id,
                        MovieId = movieId
                    };
                    await _actorMovieRepository.CreateAsync(actorMovie);
                }

                return RedirectToAction(nameof(ACIndex));
            }

            var allMovies = await _movieRepository.GetAsync();
            viewModel.Movies = allMovies.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            return View(viewModel);
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            // First delete ActorMovie associations
            await _actorMovieRepository.DeleteByActorIdAsync(id);

            // Then delete the actor itself
            var deleted = await _actorRepository.DeleteAsync(id);

            if (!deleted)
            {
                ModelState.AddModelError("", "Could not delete actor.");
            }

            return RedirectToAction(nameof(ACIndex));
        }



    }
}
