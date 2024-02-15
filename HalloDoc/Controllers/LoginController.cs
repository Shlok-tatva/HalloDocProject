using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAspnetuserRepository _aspnetuserrepo;

        public LoginController(IAspnetuserRepository aspnetuser)
        {
            _aspnetuserrepo = aspnetuser;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginUser(Aspnetuser user)
        {
            if (user.Email == null || user.Passwordhash == null)
            {
                TempData["Error"] = "Username and password cannot be empty.";
                return Redirect("/Login");
            }

            var storedPassword = _aspnetuserrepo.GetUserPassword(user.Email);

            var hasher = new PasswordHasher<string>();
            PasswordVerificationResult result = hasher.VerifyHashedPassword(null, storedPassword, user.Passwordhash);

            if (result == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetString("UserId", user.Email);
                return Redirect("/dashboard/index");

            }
            else if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                TempData["Error"] = "Please Change your Password";
                return Redirect("/Login");
            }
            else
            {
                TempData["Error"] = "Wrong Credentials";
                return Redirect("/Login");
            }
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }
    }
}
