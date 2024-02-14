using HalloDoc.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAspnetuserRepository _aspnetuserrepo;
        private readonly IRequestClientRepository _requestclientrepo;
        private readonly IRequestRepository _requestrepo;
        private readonly IUserRepository _userrepo;
        private readonly IRConciergeRepository _conciergerepo;
        private readonly IRequestConciergeRepository _requestConciergerepo;
        private readonly IRBusinessRepository _rbusinessrepo;
        private readonly IRequestBusinessRepository _requestbusinessrepo;
        private readonly IRequestwisefileRepository _requestwisefilerepo;


        public DashboardController(IAspnetuserRepository aspnetuser, IRequestClientRepository requestclient, IRequestRepository requestrepo, IUserRepository user, IRConciergeRepository conciergerepo, IRequestConciergeRepository requestConciergerepo, IRBusinessRepository rbusinessrepo, IRequestBusinessRepository requestbusinessrepo, IRequestwisefileRepository requestwisefilerepo)
        {
            _aspnetuserrepo = aspnetuser;
            _requestclientrepo = requestclient;
            _requestrepo = requestrepo;
            _userrepo = user;
            _conciergerepo = conciergerepo;
            _requestConciergerepo = requestConciergerepo;
            _rbusinessrepo = rbusinessrepo;
            _requestbusinessrepo = requestbusinessrepo;
            _requestwisefilerepo = requestwisefilerepo;
        }

        public IActionResult Index()
        {
            ViewData["ViewName"] = "Dashboard";
            var email = HttpContext.Session.GetString("UserId");
            var username = GetUsernameFromEmail(email); // Extract username from email
            ViewBag.Username = username;
            User user = _userrepo.GetUser(email);

            var requestData = from request in _requestrepo.GetAll(user.Userid)
                              join requestFile in _requestwisefilerepo.GetAll()
                              on request.Requestid equals requestFile.Requestid into gj
                              from subfile in gj.DefaultIfEmpty()
                              group subfile by new { request.Requestid, request.Status, request.Createddate } into g
                              select new DashboardViewModel
                              {
                                  Requestid = g.Key.Requestid,
                                  requestDate = g.Key.Createddate,
                                  requestStatus = g.Key.Status,
                                  DocumentCount = g.Count(f => f != null)
                              };

            var dashboardRequests = requestData.ToList();

            return View(dashboardRequests);
        }

        public IActionResult DocumentView(int Requestid)
        {
            ViewData["ViewName"] = "DocumentView";
            var email = HttpContext.Session.GetString("UserId");
            var username = GetUsernameFromEmail(email); // Extract username from email
            ViewBag.Username = username;
            User user = _userrepo.GetUser(email);

            // Fetch all request-wise files for the given Requestid
            var requestwiseFiles = from request in _requestrepo.GetAll(user.Userid)
                                   join requestFile in _requestwisefilerepo.GetAll()
                                   on request.Requestid equals requestFile.Requestid
                                   where request.Requestid == Requestid
                                   select new DocumentViewModel
                                   {
                                       Requestid = request.Requestid,
                                       uploadDate = request.Createddate,
                                       UploadImage = requestFile.Filename,
                                       fileName = Path.GetFileName(requestFile.Filename),
                                   };

            // Pass the list of DocumentViewModel to the view
            return View(requestwiseFiles.ToList());
        }

        public IActionResult DownloadFile(string filePath)
        {
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('~', '/'));
            // Check if the file exists
            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound(); // Return 404 Not Found if the file does not exist
            }

            string fileName = Path.GetFileName(filePath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(physicalPath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile file, int requestId)
        {
            try
            {  
                    HandleFileUpload(file, requestId);
                    return Ok(new { success = true });            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { success = false, message = "An error occurred while Uploading the file" });
            }
        }

        public IActionResult Profile(UserDataviewModel userData)
        {
            ViewData["ViewName"] = "UserProfile";
            var email = HttpContext.Session.GetString("UserId");
            var username = GetUsernameFromEmail(email);
            ViewBag.Username = username;
            User user = _userrepo.GetUser(email);
            userData =  new UserDataviewModel
            {
                userid = user.Userid,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.Mobile,
                DateOfBirth = user.Intyear.Value.ToString() + "-" + user.Strmonth + "-" + user.Intdate.Value.ToString(),
                Street = user.Street,
                State = user.State,
                City = user.City,
                ZipCode = user.Zipcode
            };
            return View(userData);
        }


        [HttpPost("UpdateUser")]

        public IActionResult UpdateUser(int userid , string FirstName , string LastName , string Email , string PhoneNumber , string DateOfBirth ,  string Street , string State , string City , string ZipCode)
        {
            try
            {
            var date = DateTime.Parse(DateOfBirth);

            User user = _userrepo.GetUserByID(userid);
            user.Firstname = FirstName;
            user.Lastname =LastName;
            user.Email = Email;
            user.Mobile = PhoneNumber;
            user.Intyear = date.Year;
            user.Strmonth = date.ToString("MM");
            user.Intdate = date.Day;
            user.Street = Street;
            user.State = State;
            user.City = City;
            user.Zipcode = ZipCode;
            _userrepo.Update(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        private void HandleFileUpload(IFormFile UploadFile, int requestId)
        {
            var requestwisefile = new Requestwisefile();
            string FilePath = "wwwroot\\Upload\\" + requestId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
            string UploadImage = "~" + FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + UploadFile.FileName;

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                UploadFile.CopyTo(stream);
            }

            requestwisefile.Requestid = requestId;
            requestwisefile.Filename = UploadImage;
            requestwisefile.Createddate = DateTime.Now;
            _requestwisefilerepo.Add(requestwisefile);
        }

        private string GetUsernameFromEmail(string email)
        {
            if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            {
                return email.Split('@')[0];
            }
            return email;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Login/index");
        }

    }
}
