using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class CategoryCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "Select Movies")]
        public List<int> SelectedMovieIds { get; set; } = new();

        public List<SelectListItem> Movies { get; set; } = new();
    }
}
