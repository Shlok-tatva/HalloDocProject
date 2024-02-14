using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
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
                return Json("Username and password cannot be empty.");
            }

            var storedPassword = _aspnetuserrepo.GetUserPassword(user.Email);
            if (storedPassword != null && storedPassword == user.Passwordhash)
            {
                HttpContext.Session.SetString("UserId", user.Email);
                return Redirect("/dashboard/index");

            }
            else
            {
                return Json("Wrong password.");
            }
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }
    }
}
