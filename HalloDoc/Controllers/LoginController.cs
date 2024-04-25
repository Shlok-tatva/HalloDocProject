 using Azure.Core;
using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repository;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace HalloDoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAspnetuserRepository _aspnetuserrepo;
        private readonly IPatientFunctionRepository _petientfunctionrepo;
        private readonly ICommonFunctionRepository _commonfunctionrepo;
        private readonly IUserRepository _userrepo;
        private readonly IAdminRepository _adminrepo;
        private readonly IPhysicianRepository _physicianrepo;
        private readonly IJwtServices _jwtServices;


        public LoginController(IAspnetuserRepository aspnetuser, IPatientFunctionRepository petientfunctionrepo , ICommonFunctionRepository commonfunctionrepo , IUserRepository userrepo, IAdminRepository adminrepo , IPhysicianRepository physicianrepo , IJwtServices jwtServices)
        {
            _aspnetuserrepo = aspnetuser;
            _petientfunctionrepo = petientfunctionrepo;
            _commonfunctionrepo = commonfunctionrepo;
            _userrepo = userrepo;
            _adminrepo = adminrepo;
            _physicianrepo = physicianrepo;
            _jwtServices = jwtServices;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetInt32("roleid");
            if(role == 3)
            {
                return Redirect("/dashboard");
            }
            else
            {
             return View();
            }
        }

        public IActionResult LoginUser(Aspnetuser user)
        {
            
            if (user.Email == null || user.Passwordhash == null)
            {
                TempData["Error"] = "Username and password cannot be empty.";
                return Redirect("/Login");
            }
            if (_aspnetuserrepo.Exists(user.Email))
            {
                var storedPassword = _aspnetuserrepo.GetUserPassword(user.Email);
               
                var hasher = new PasswordHasher<string>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(null, storedPassword, user.Passwordhash);

                if (result == PasswordVerificationResult.Success)
                {
                    var logedinUser = _aspnetuserrepo.GetByEmail(user.Email);
                    var roleid = logedinUser.Roleid;

                    if(roleid == 3)
                    {
                        string token = _jwtServices.GenerateToken(user.Email, "User" , 3);
                        HttpContext.Session.SetString("jwttoken", token);
                        HttpContext.Session.SetString("UserId", user.Email);
                        HttpContext.Session.SetInt32("roleid", 3);
                        return Redirect("/dashboard/index");
                    }
                    else
                    {
                        TempData["Error"] = "Patient account does not exists";
                        return Redirect("/login");
                    }

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
            else
            {
                TempData["Error"] = "User Dose not Exists";
                return Redirect("/Login");
            }
            
        }

        [HttpGet]
        public IActionResult PhysicianLogin()
        {
            var role = HttpContext.Session.GetInt32("roleid");
            if (role == 1)
            {
                return Redirect("/Admin/Dashboard");
            }

            return View();
        }

        [HttpPost]
        public IActionResult PhysicianLogin(Aspnetuser user)
        {
            if (user.Email == null || user.Passwordhash == null)
            {
                TempData["Error"] = "Username and password cannot be empty.";
                return Redirect("/login/PhysicianLogin");
            }
            if (_aspnetuserrepo.Exists(user.Email))
            {
                var storedPassword = _aspnetuserrepo.GetUserPassword(user.Email);

                var hasher = new PasswordHasher<string>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(null, storedPassword, user.Passwordhash);

                if (result == PasswordVerificationResult.Success)
                {
                    var logedinUser = _aspnetuserrepo.GetByEmail(user.Email);
                    var aspnetroleid = logedinUser.Roleid; // for check either admin or Physician

                    if (aspnetroleid == 1)
                    {
                        Admin Admin = _adminrepo.GetAdmin(logedinUser.Id);
                        string token = _jwtServices.GenerateToken(user.Email, "Admin" , Admin.Roleid);
                        HttpContext.Session.SetString("jwttoken", token);
                        HttpContext.Session.SetString("Username", Admin.Firstname + " " + Admin.Lastname);
                        HttpContext.Session.SetString("AdminId", Admin.Adminid.ToString());
                        HttpContext.Session.SetInt32("roleid", (int)Admin.Roleid);
                        TempData["Success"] = "Login successfully";
                        return Redirect("/Admin/Dashboard");
                    }
                    else if (aspnetroleid == 2)
                    {
                        Physician physician = _physicianrepo.GetPhysician(logedinUser.Id);
                        string token = _jwtServices.GenerateToken(user.Email, "Provider" , physician.Roleid);
                        HttpContext.Session.SetString("jwttoken", token);
                        HttpContext.Session.SetString("Username", physician.Firstname + " " + physician.Lastname);
                        HttpContext.Session.SetString("providerId", physician.Physicianid.ToString());
                        HttpContext.Session.SetInt32("roleid", (int)physician.Roleid);
                        TempData["Success"] = "Login successfully";
                        return Redirect("/Provider/Dashboard");
                    }
                    else
                    {
                        TempData["Error"] = "Account does not exists";
                        return Redirect("/login/PhysicianLogin");
                    }

                }
                else if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    TempData["Error"] = "Please Change your Password";
                    return Redirect("/login/PhysicianLogin");
                }
                else
                {
                    TempData["Error"] = "Wrong Credentials";
                    return Redirect("/login/PhysicianLogin");
                }
            }
            else
            {
                TempData["Error"] = "User Dose not Exists";
                return Redirect("/login/PhysicianLogin");
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

            string name = user.Firstname + " , " + user.Lastname;

            string key = "770A8A65DA156D24EE2A093277530142";
            String encryptedEmail = _petientfunctionrepo.Encrypt(email , key);
            DateTime date = DateTime.Now;
            string encryptedDate = _petientfunctionrepo.Encrypt(date.ToString(), key);
            var resetPassowrdLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Login/changePassword?email={encryptedEmail}&datetime={encryptedDate}";
            var title = "Reset Password Link";
            var message = $"Please click <a href=\"{resetPassowrdLink}\">here</a> to Reset Your Password account.";
            bool isSent = _petientfunctionrepo.SendEmail(email , title , message);
            _commonfunctionrepo.EmailLog(email, message, title, name , 3, null, null, null, 3, isSent, 1);
            TempData["Success"] = "Reset password link sent to Email";
            return Redirect("/login/forgetPassword");
        }

        [HttpGet]
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

            if (diffOfDate.TotalSeconds < 1500) {
                ViewBag.email = email;
                return View();
            }
            else
            {
                TempData["Error"] = "Link Expired Please Create New Link";
                return Redirect("/Login/ForgetPassword");
            }
        }

        [HttpPost]
        public IActionResult changePassword(string email , string password , string confirmPassword)
        {
            if(password ==  confirmPassword)
            {
                var hasher = new PasswordHasher<string>();
                Aspnetuser aspnetuser = _aspnetuserrepo.GetByEmail(email);
                string hashedPassword = hasher.HashPassword(null, password);
                aspnetuser.Passwordhash = hashedPassword;
                _aspnetuserrepo.Update(aspnetuser);
                return Ok();
            }
            else
            {
                TempData["Error"] = "Both password Are not Same";
                return Redirect("/Login/changePassword");
            }
        }

        public IActionResult Accessdenied()
        {
            return View();
        }

        public IActionResult GetAllSessionData()
        {
            var sessionData = HttpContext.Session.Keys.ToDictionary(k => k, k => HttpContext.Session.GetString(k));
            return Ok(sessionData);
        }
    }
}
