using CartoonWorld.Data;
using CartoonWorld.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartoonWorld.Controllers
{

    public class UserController : Controller
    {
        private static CartoonWorldDbContext _dBcontext;
        private List<User> lijstGebruikers = new List<User>();
        public UserController(CartoonWorldDbContext dbContext)
        {
            _dBcontext = dbContext;
            lijstGebruikers = _dBcontext.Users.ToList();
        }

        public IActionResult Index()
        {
            return View(lijstGebruikers);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var lijstMovieUser = await _dBcontext.Users
                    .Include(x => x.ijstMovies).ThenInclude(x => x.Movie)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

            return View(lijstMovieUser);
        }
        public IActionResult Register()
        {
            return View();
        }
        // register form 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FirstName,LastName,Password,Email")] User uVm)
        {
            bool existUser = false;
            foreach(var item in _dBcontext.Users)
            {
                if(item.FirstName.Equals(uVm.FirstName))
                {
                    existUser = true;
                }
            }
            if (!existUser)
            {
                try
                {
                    uVm.Role = "User";
                    uVm.Subscription = false;

                    _dBcontext.Add(uVm);
                    await _dBcontext.SaveChangesAsync();
                    return RedirectToAction("Login", "Home");
                }
                catch
                {
                    ModelState.AddModelError("", "unable to save changes");
                }
            }
            return View("Register");
        }
        //edit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null) return NotFound();
            var user = await _dBcontext.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]

        public async Task<IActionResult> Edit(int id, [FromForm] User mv)
        {
            // if (!TryValidateModel(vm)) return View(mv);
            var user = await _dBcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            user.FirstName = mv.FirstName;
            user.Role = mv.Role;
            await _dBcontext.SaveChangesAsync();

            return RedirectToAction("Login", "Home");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _dBcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _dBcontext.Users.FindAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _dBcontext.Users.Remove(user);
                await _dBcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
        public async Task<IActionResult> DetailUser(int id)
        {
            var user = await _dBcontext.Users.FindAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }
    }
}
