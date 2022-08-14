using CartoonWorld.Data;
using CartoonWorld.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartoonWorld.Controllers
{
    public class MovieController : Controller
    {
        private readonly CartoonWorldDbContext _dbcontext;
        private static List<Movie> lijstMovies = null;
        
        public MovieController(CartoonWorldDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
          
            lijstMovies = _dbcontext.Movies.ToList();
            if (lijstMovies == null) return NotFound();
            return View(lijstMovies);
        }

        //create
       [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titel,Genre,About,Image,MovieLink,ReleaseDate")] Movie mv)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    _dbcontext.Add(mv);
                    await _dbcontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError("", "unable to save changes");
            }
            return View("Create");
        }
        // edit
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
           
            var mv = _dbcontext.Movies.Find(id);
            if (mv == null) return NotFound();

            return View(mv);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movieToUpdate = await _dbcontext.Movies.FirstOrDefaultAsync(s => s.Id == id);
            if (await TryUpdateModelAsync<Movie>(
                movieToUpdate,
                "",
                s => s.Titel, s => s.Genre, s => s.About, s => s.ReleaseDate))
            {
                try
                {
                    await _dbcontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(movieToUpdate);
        }
        //detail 
        public async Task<IActionResult> Detail(int? id)
        {
            ViewData["gebruiker"] = _dbcontext.Users.FirstOrDefault(x => x.FirstName == User.Identity.Name);
            ViewData["movieuser"] = _dbcontext.MovieUsers.ToList();
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _dbcontext.Movies
                .Include(x => x.LijstUsers).ThenInclude(x => x.Users)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        // add movie to user list
        [Authorize(Roles = "Admin,User")]
        [HttpPost, ActionName("Detail")]
        public async Task<IActionResult> DetailPost(int id)
        {
            var user = _dbcontext.Users.FirstOrDefault(x => x.FirstName == User.Identity.Name);
            if (user == null) return NotFound();
            MovieUser mv = new MovieUser();
            mv.MovieId = id;
            mv.UserId = user.Id;
            _dbcontext.MovieUsers.Add(mv);
            //user.ijstMovies.Add(mv);
            await _dbcontext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        // remove movie from user list
        [HttpPost]
        public async Task<IActionResult> DetailPostRemove(int id)
        {
            var user = _dbcontext.Users.Include(x => x.ijstMovies).Single(x => x.FirstName == User.Identity.Name);
            if (user == null) return NotFound();
            var movieUserL = _dbcontext.MovieUsers.Where(x => x.MovieId == id && x.UserId == user.Id).FirstOrDefault();
            user.ijstMovies.Remove(movieUserL);
            await _dbcontext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        //delete

        public IActionResult Delete(int id)
        {
            if (id == null) return NotFound();
            var mv = _dbcontext.Movies.Find(id);
            if (mv == null) return NotFound();

            return View(mv);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _dbcontext.Movies.FindAsync(id);
            if (movie == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _dbcontext.Movies.Remove(movie);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        //movie watcher
        [HttpGet]
        public IActionResult Watch(int id)
        {
            if (id == null) return NotFound();
            var mv = _dbcontext.Movies.Find(id);
            if (mv == null) return NotFound();

            return View(mv);
        }
       
    }
}
