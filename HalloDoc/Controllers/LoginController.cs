using Azure.Core;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAspnetuserRepository _aspnetuserrepo;
        private readonly IPatientFunctionRepository _petientfunctionrepo;
        private readonly IUserRepository _userrepo;

        public LoginController(IAspnetuserRepository aspnetuser, IPatientFunctionRepository petientfunctionrepo ,IUserRepository userrepo)
        {
            _aspnetuserrepo = aspnetuser;
            _petientfunctionrepo = petientfunctionrepo;
            _userrepo = userrepo;
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

        [HttpPost]
        public IActionResult resetPassword(string email)
        {
            User user = _userrepo.GetUser(email);
            if(user == null)
            {
                TempData["Error"] = "User Not Found";
                return Redirect("/login/forgetPassword");
            }

            string key = "770A8A65DA156D24EE2A093277530142";
            String encryptedEmail = _petientfunctionrepo.Encrypt(email , key);
            DateTime date = DateTime.Now;
            string encryptedDate = _petientfunctionrepo.Encrypt(date.ToString(), key);
            var resetPassowrdLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Login/changePassword?email={encryptedEmail}&datetime={encryptedDate}";
            _petientfunctionrepo.SendEmail(email, resetPassowrdLink);
            return Ok();
        }

        public IActionResult changePassword()
        {
            string encryptedEmail = Request.Query["email"];
            string encryptedDatetime = Request.Query["datetime"];

            string key = "770A8A65DA156D24EE2A093277530142";
            string email = _petientfunctionrepo.Decrypt(encryptedEmail, key);
            string datetime = _petientfunctionrepo.Decrypt(encryptedDatetime, key);

            DateTime getDate = DateTime.Parse(datetime);
            DateTime currentDate = DateTime.Now;

            var diffOfDate = currentDate - getDate;

            if (diffOfDate.TotalSeconds < 1200) {
                return View();
            }
            else
            {
                TempData["Error"] = "Link Expired Please Create New Link";
                return Redirect("/Login/forgetPassword");
            }
        }
    }
}
