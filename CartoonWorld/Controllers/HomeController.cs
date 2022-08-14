using CartoonWorld.Data;
using CartoonWorld.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CartoonWorld.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static User user = new User();
        private readonly CartoonWorldDbContext _dbContext;
        public static List<User> lijstGebruikers = new List<User>();
        private static List<Movie> lijstMovies = new List<Movie>();
        public HomeController(CartoonWorldDbContext dbContext,ILogger<HomeController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            lijstMovies = _dbContext.Movies.ToList();
            lijstGebruikers = _dbContext.Users.ToList();
        }
        public IActionResult Index()
        {
            return View(lijstMovies);
        }
        [HttpPost]
        public IActionResult IndexSearch()
        {
            string sort = Request.Form["sortOrder"].ToString();
            switch (sort)
            {
                case "Titel A-Z":
                    lijstMovies = lijstMovies.OrderBy(x => x.Titel).ToList();
                    break;
                case "Titel Z-A":
                    lijstMovies = lijstMovies.OrderByDescending(x => x.Titel).ToList();
                    break;
                case "ReleaseDate":
                    lijstMovies = lijstMovies.OrderBy(x => x.ReleaseDate).ToList();
                    break;
                case "Genre":
                    lijstMovies = lijstMovies.OrderBy(x => x.Genre).ToList();
                    break;
            }

            return View("Index",lijstMovies);
        }

        [HttpPost,ActionName("Index")]
        public IActionResult IndexPost(string sortOrder)
        {
            string myvalue = Request.Form["LijstFilms"].ToString();
            if (string.IsNullOrEmpty(myvalue))
            {
                return View(lijstMovies);
            }
            return View(lijstMovies.Where(x => x.Titel.Contains(myvalue)).ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = "Admin, User")]
        public IActionResult Secured()
        {
            return View();
        }
        //login
        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Validate(string gebruikersnaam, string paswoord, string returnUrl)
        { 
            var u = lijstGebruikers.FirstOrDefault(x => x.FirstName == gebruikersnaam && x.Password == paswoord);
            if (u == null) return View("login");
            user.FirstName = u.FirstName;
            user.Password = u.Password;
            user.Id = u.Id;
             if(user != null)
            {
                if (user.FirstName == gebruikersnaam && user.Password == paswoord)
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim("gebruikersnaam", gebruikersnaam));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, gebruikersnaam));
                    claims.Add(new Claim(ClaimTypes.Name, gebruikersnaam));
                    switch (u.Role)
                    {
                        case "Admin":
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                            break;
                        case "User":
                            claims.Add(new Claim(ClaimTypes.Role, "User"));
                            break;
                        default:
                            claims.Add(new Claim(ClaimTypes.Role, "Anonymous"));
                            break;
                    } 
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Index","Home");
                }
            }
            TempData["Error"] = "Gebruikersnaam en/of paswoord is niet geldig.";
            ViewData["ReturnUrl"] = returnUrl;
            return View("login");
        }
        // logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        //denied
        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}