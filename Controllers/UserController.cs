using System;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SNSCakeBakery.Web.Models; // your EF models namespace
using SNSCakeBakery.Web.ViewModels; // if you use view models
using System.Security.Cryptography;
using System.Text;

namespace SNSCakeBakery.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: /user/me
        [HttpGet]
        public ActionResult Me()
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { authenticated = false }, JsonRequestBehavior.AllowGet);

            var email = User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            return Json(new 
            {
                authenticated = true,
                email = user.Email,
                id = user.Id
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: /user/register
        [HttpPost]
        public ActionResult Register(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new { success = false, message = "Email and password required." });
            }

            var exists = _db.Users.Any(u => u.Email == email);
            if (exists)
            {
                return Json(new { success = false, message = "Email already registered." });
            }

            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        // POST: /user/login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Json(new { success = false, message = "Invalid credentials." });
            }

            if (!VerifyPassword(user.PasswordHash, password))
            {
                return Json(new { success = false, message = "Invalid credentials." });
            }

            FormsAuthentication.SetAuthCookie(user.Email, true);

            return Json(new { success = true });
        }

        // POST: /user/logout
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Json(new { success = true });
        }

        // --------------------------
        // Password Functions
        // --------------------------

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string storedHash, string password)
        {
            return storedHash == HashPassword(password);
        }
    }
}
