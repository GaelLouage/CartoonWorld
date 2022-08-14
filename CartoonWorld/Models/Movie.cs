using System.ComponentModel.DataAnnotations;

namespace CartoonWorld.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="name required")]
        public string Titel { get; set; }
        [Required(ErrorMessage = "Genre required")]
        public string Genre { get; set; }
        [Required(ErrorMessage = "About required")]
        public string About { get; set; }
        [Required(ErrorMessage = "ReleaseDate required")]
        public DateTime ReleaseDate { get; set; }
        [Required(ErrorMessage= "Film heeft een afbeelding nodig")]
        public string Image { get; set; }
        [Required(ErrorMessage ="Link naar film is verplicht")]
        public string MovieLink { get; set; }
        public ICollection<MovieUser>? LijstUsers { get; set; }
    }
}
