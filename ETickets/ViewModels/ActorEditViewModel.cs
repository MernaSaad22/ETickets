using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class ActorEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Bio { get; set; }

        public string ExistingProfilePicture { get; set; }  // to retain old file name

        public IFormFile? NewProfilePicture { get; set; }    // for uploading new one

        public string News { get; set; }

        [Display(Name = "Select Movies")]
        public List<int> SelectedMovieIds { get; set; } = new();

        public List<SelectListItem> Movies { get; set; } = new();
    }
}
