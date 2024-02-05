using Microsoft.AspNetCore.Mvc;

namespace HelloDoc.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        public IActionResult SubmitRequest()
        {
            return View();
        }
        public IActionResult PatientRequest()
        {
            ViewData["ViewName"] = "PatientRequest";
            return View();
        }
        public IActionResult FamilyFriendRequest()
        {
            ViewData["ViewName"] = "FamilyFriendRequest";
            return View();
        }
        public IActionResult ConciergeRequest()
        {
            ViewData["ViewName"] = "ConciergeRequest";
            return View();
        }
        public IActionResult BusinessRequest()
        {
            ViewData["ViewName"] = "BusinessRequest";
            return View();
        }

    }
}

