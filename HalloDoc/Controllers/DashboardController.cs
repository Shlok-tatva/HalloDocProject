using HalloDoc.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repository;
using HalloDoc_BAL.ViewModel.Patient;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Transactions;

namespace HalloDoc.Controllers
{
    [CustomAuth("User")]
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
        private readonly IPatientFunctionRepository _patientFuncrepo;
        private readonly ICommonFunctionRepository _commonFunctionrepo;




        public DashboardController(IAspnetuserRepository aspnetuser, IRequestClientRepository requestclient, IRequestRepository requestrepo, IUserRepository user, IRConciergeRepository conciergerepo, IRequestConciergeRepository requestConciergerepo, IRBusinessRepository rbusinessrepo, IRequestBusinessRepository requestbusinessrepo, IRequestwisefileRepository requestwisefilerepo, IPatientFunctionRepository patientFuncrepo, ICommonFunctionRepository commonFunctionrepo)
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
            _patientFuncrepo = patientFuncrepo;
            _commonFunctionrepo = commonFunctionrepo;
        }

        public IActionResult Index()
        {
            ViewData["ViewName"] = "Dashboard";
            var email = HttpContext.Session.GetString("UserId");

            if (email == null)
            {
                return Redirect("/Login");
            }

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
            if (email == null)
            {
                return Redirect("/Login");
            }
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
                                       uploadDate = requestFile.Createddate,
                                       UploadImage = requestFile.Filename,
                                       fileName = Path.GetFileName(requestFile.Filename),
                                       confirmationNumber = request.Confirmationnumber,
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

        public IActionResult UploadFile(IFormFile file, int requestId)
        {
            try
            {
                _commonFunctionrepo.HandleFileUpload(file, requestId, null, null);
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
            if (email == null)
            {
                return Redirect("/Login");
            }
            var username = GetUsernameFromEmail(email);
            ViewBag.Username = username;
            User user = _userrepo.GetUser(email);
            userData = new UserDataviewModel
            {
                userid = user.Userid,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.Mobile,
                DateOfBirth = user.Intyear.Value.ToString("") + "-" + user.Strmonth + "-" + string.Format("{0:00}", user.Intdate.Value),
                Street = user.Street,
                State = user.State,
                City = user.City,
                ZipCode = user.Zipcode
            };
            return View(userData);
        }


        public IActionResult UpdateUser(UserDataviewModel userData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // Return validation errors
                }

                var dateOfBirth = DateTime.Parse(userData.DateOfBirth);

                User user = _userrepo.GetUserByID(userData.userid);
                if (user == null)
                {
                    return NotFound(); // Return 404 if user not found
                }

                // Update user properties
                user.Firstname = userData.FirstName;
                user.Lastname = userData.LastName;
                user.Email = userData.Email;
                user.Mobile = userData.PhoneNumber;
                user.Intyear = dateOfBirth.Year;
                user.Strmonth = dateOfBirth.ToString("MM");
                user.Intdate = dateOfBirth.Day;
                user.Street = userData.Street;
                user.State = userData.State;
                user.City = userData.City;
                user.Zipcode = userData.ZipCode;

                _userrepo.Update(user);

                return Ok(); // Return 200 OK if update is successful
            }
            catch (FormatException)
            {
                // Return 400 Bad Request for date parsing errors
                return BadRequest("Invalid date format for DateOfBirth.");
            }
            catch
            {
                // Return 500 Internal Server Error for other unexpected errors
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        public IActionResult RequestForme()
        {
            ViewData["ViewName"] = "RequestForme";
            var email = HttpContext.Session.GetString("UserId");
            var regions = _commonFunctionrepo.GetAllReagion();
            ViewBag.regions = regions;
            if (email == null)
            {
                return Redirect("/Login");
            }
            var username = GetUsernameFromEmail(email); // Extract username from email
            ViewBag.Username = username;

            User user = _userrepo.GetUser(email);
            var patinetData = new PatientFormData
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                regionId = (int)user.Regionid,
                Street = user.Street,
                DateOfBirth = DateTime.ParseExact(user.Intyear.Value.ToString("") + "-" + user.Strmonth + "-" + string.Format("{0:00}", user.Intdate.Value), "yyyy-MM-dd", null),
                City = user.City,
                ZipCode = user.Zipcode
            };
            return View(patinetData);
        }

        public IActionResult RequestForSomeone()
        {
            ViewData["ViewName"] = "RequestForSomeone";
            var email = HttpContext.Session.GetString("UserId");
            if (email == null)
            {
                return Redirect("/Login");
            }
            var username = GetUsernameFromEmail(email); // Extract username from email
            ViewBag.Username = username;
            var regions = _commonFunctionrepo.GetAllReagion();
            ViewBag.regions = regions;

            return View();
        }

        public IActionResult SubmitRequest(PatientFormData formData)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = new TransactionScope())
                {
                    var email = HttpContext.Session.GetString("UserId");
                    User user = _userrepo.GetUser(email);
                    var request = new Request();
                    var requestClient = new Requestclient();
                    string state = _commonFunctionrepo.GetAllReagion().Where(r => r.Regionid == formData.regionId).FirstOrDefault().Name;


                    request.Requesttypeid = 1;
                    request.Userid = user.Userid;
                    request.Firstname = formData.FirstName;
                    request.Lastname = formData.LastName;
                    request.Email = formData.Email;
                    request.Phonenumber = formData.PhoneNumber;
                    request.Status = 1;
                    request.Isdeleted = false;
                    request.Isurgentemailsent = false;
                    request.Createddate = DateTime.Now;
                    request.Confirmationnumber = _commonFunctionrepo.GetConfirmationNumber(state, formData.LastName, formData.FirstName);

                    if (formData.RelationWithPatinet != null)
                    {
                        request.Relationname = formData.RelationWithPatinet;
                    }

                    _requestrepo.Add(request);

                    if (formData.UploadFile != null)
                    {
                        _commonFunctionrepo.HandleFileUpload(formData.UploadFile, request.Requestid, null, null);
                    }

                    requestClient.Notes = formData.Symptoms;
                    requestClient.Requestid = request.Requestid;
                    requestClient.Firstname = formData.FirstName;
                    requestClient.Lastname = formData.LastName;
                    requestClient.Phonenumber = formData.PhoneNumber;
                    requestClient.Email = formData.Email;
                    requestClient.Strmonth = formData.DateOfBirth.Month.ToString("00");
                    requestClient.Intyear = formData.DateOfBirth.Year;
                    requestClient.Intdate = formData.DateOfBirth.Day;
                    requestClient.Street = formData.Street;
                    requestClient.City = formData.City;
                    requestClient.State = state;
                    requestClient.Regionid = formData.regionId;
                    requestClient.Zipcode = formData.ZipCode;
                    _requestclientrepo.Add(requestClient);

                    string key = "770A8A65DA156D24EE2A093277530142";
                    string encryptedEmail = _patientFuncrepo.Encrypt(formData.Email, key);
                    var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/createAccount?email={encryptedEmail}&requestId={request.Requestid}";

                    var title = "Account Creation Link";
                    var message = $"Please click <a href=\"{accountCreationLink}\">here</a> to create your account.";
                    bool isSent = _patientFuncrepo.SendEmail(formData.Email, title, message);
                    string name = formData.FirstName + " , " + formData.LastName;
                    _commonFunctionrepo.EmailLog(formData.Email, message, title, name, 3, request.Requestid, null, null, 4, isSent, 1);
                    _commonFunctionrepo.SMSLog(formData.PhoneNumber, message, title, name, 3, request.Requestid, null, null);


                    transaction.Complete();
                    TempData["Success"] = "Request Created Successfully";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                string errorMessage = string.Join(" and ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = errorMessage;
                return NotFound();
            }
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
