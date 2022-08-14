using System.ComponentModel.DataAnnotations;

namespace CartoonWorld.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        [Required]
        public bool Subscription { get; set; }
        public ICollection<MovieUser> ijstMovies { get; set; }
    }
}
