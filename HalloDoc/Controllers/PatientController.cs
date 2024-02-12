using HalloDoc.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Text;
using System.Transactions;

namespace HelloDoc.Controllers
{
    public class PatientController : Controller
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


        public PatientController(IAspnetuserRepository aspnetuser, IRequestClientRepository requestclient, IRequestRepository requestrepo, IUserRepository user, IRConciergeRepository conciergerepo, IRequestConciergeRepository requestConciergerepo, IRBusinessRepository rbusinessrepo, IRequestBusinessRepository requestbusinessrepo, IRequestwisefileRepository requestwisefilerepo)
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
            return View();
        }

        public IActionResult Login()
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
                return RedirectToAction("Dashboard");

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

        public IActionResult SubmitRequest()
        {
            return View();
        }

        public IActionResult PatientRequest()
        {
            ViewData["ViewName"] = "PatientRequest";
            return View();
        }

        [HttpPost]
        public IActionResult SubmitPatientForm(PatientFormData formData)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var user = new User();
                        var request = new Request();
                        var requestClient = new Requestclient();
                        var requestwisefile = new Requestwisefile();
                        var aspnetuser = _aspnetuserrepo.GetByEmail(formData.Email);

                        if (aspnetuser == null)
                        {
                            aspnetuser = new Aspnetuser();

                            Guid guid = Guid.NewGuid();
                            string str = guid.ToString();

                            aspnetuser.Id = str;
                            aspnetuser.Username = formData.Email;
                            aspnetuser.Email = formData.Email;
                            aspnetuser.Phonenumber = formData.PhoneNumber;
                            aspnetuser.Passwordhash = formData.Password;
                            aspnetuser.Createddate = DateTime.Now;
                            _aspnetuserrepo.Add(aspnetuser);

                            user.Aspnetuserid = aspnetuser.Id;
                            user.Firstname = formData.FirstName;
                            user.Lastname = formData.LastName;
                            user.Email = formData.Email;
                            user.Mobile = formData.PhoneNumber;
                            user.Street = formData.Street;
                            user.City = formData.City;
                            user.State = formData.State;
                            user.Zipcode = formData.ZipCode;
                            user.Isdeleted = false;
                            user.Createdby = aspnetuser.Id;
                            user.Createddate = DateTime.Now;
                            _userrepo.Add(user);
                        }
                        else
                        {
                            user = _userrepo.GetUser(formData.Email);
                        }

                        request.Requesttypeid = 2;
                        request.Userid = user.Userid;
                        request.Firstname = formData.FirstName;
                        request.Lastname = formData.LastName;
                        request.Email = formData.Email;
                        request.Phonenumber = formData.PhoneNumber;
                        request.Status = 1;
                        request.Isurgentemailsent = false;
                        request.Createddate = DateTime.Now;
                        _requestrepo.Add(request);

                        if (formData.UploadFile != null)
                        {
                            string FilePath = "wwwroot\\Upload\\" + request.Requestid;
                            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            string fileNameWithPath = Path.Combine(path, formData.UploadFile.FileName);
                            formData.UploadImage = "~" + FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + formData.UploadFile.FileName;

                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                formData.UploadFile.CopyTo(stream);
                            }

                            requestwisefile.Requestid = request.Requestid;
                            requestwisefile.Filename = formData.UploadImage;
                            requestwisefile.Createddate = DateTime.Now;

                            _requestwisefilerepo.Add(requestwisefile);
                        }

                        requestClient.Notes = formData.Symptoms;
                        requestClient.Requestid = request.Requestid;
                        requestClient.Firstname = formData.FirstName;
                        requestClient.Lastname = formData.LastName;
                        requestClient.Phonenumber = formData.PhoneNumber;
                        requestClient.Email = formData.Email;
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString();
                        requestClient.Intyear = formData.DateOfBirth.Year;
                        requestClient.Intdate = formData.DateOfBirth.Day;
                        requestClient.Street = formData.Street;
                        requestClient.City = formData.City;
                        requestClient.State = formData.State;
                        requestClient.Zipcode = formData.ZipCode;
                        _requestclientrepo.Add(requestClient);

                        transaction.Complete();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to submit Form", ex);
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }   
        }


        public IActionResult CheckEmailAvailbility([FromBody] string email)
        {

            bool emailExists = _aspnetuserrepo.Exists(email);
            return Ok(new { exists = emailExists });
        }

        public IActionResult FamilyFriendRequest()
        {
            ViewData["ViewName"] = "FamilyFriendRequest";
            return View();
        }

        [HttpPost]
        public IActionResult SubmitFamilyFriendData(FamilyFriendFormData formData)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var request = new Request();
                        var requestClient = new Requestclient();

                        request.Requesttypeid = 1;
                        request.Firstname = formData.f_firstName;
                        request.Lastname = formData.f_lastName;
                        request.Email = formData.f_Email;
                        request.Phonenumber = formData.f_PhoneNumber;
                        request.Status = 1;
                        request.Isurgentemailsent = false;
                        request.Isdeleted = false;
                        request.Createddate = DateTime.Now;
                        request.Relationname = formData.relationWithPatinet;
                        
                        _requestrepo.Add(request);

                        requestClient.Notes = formData.Symptoms;
                        requestClient.Requestid = request.Requestid;
                        requestClient.Firstname = formData.FirstName;
                        requestClient.Phonenumber = formData.PhoneNumber;
                        requestClient.Address = formData.Street;
                        requestClient.Lastname = formData.LastName;
                        requestClient.Email = formData.Email;
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString();
                        requestClient.Intyear = formData.DateOfBirth.Year;
                        requestClient.Intdate = formData.DateOfBirth.Day;
                        requestClient.Street = formData.Street;
                        requestClient.City = formData.City;
                        requestClient.State = formData.State;
                        requestClient.Zipcode = formData.ZipCode;

                        _requestclientrepo.Add(requestClient);

                        transaction.Complete();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to submit Form", ex);
                    }
                }
            }
            else
            {
                return RedirectToAction("FamilyFriendRequest");
            }

        }

        public IActionResult ConciergeRequest()
        {
            ViewData["ViewName"] = "ConciergeRequest";
            return View();
        }

        [HttpPost]
        public IActionResult SubmitConciergeData(ConciergeFormData formData)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var request = new Request();
                        var requestClient = new Requestclient();
                        var rconcierge = new RConcierge();
                        var requestConcierge = new Requestconcierge();

                        request.Requesttypeid = 3;
                        request.Firstname = formData.ConciergeFirstName;
                        request.Lastname = formData.ConciergeLastName;
                        request.Email = formData.ConciergeEmail;
                        request.Phonenumber = formData.ConciergePhoneNumber;
                        request.Status = 1;
                        request.Isurgentemailsent = false;
                        request.Isdeleted = false;
                        request.Createddate = DateTime.Now;    
                        _requestrepo.Add(request);

                        requestClient.Notes = formData.Symptoms;
                        requestClient.Requestid = request.Requestid;
                        requestClient.Firstname = formData.FirstName;
                        requestClient.Lastname = formData.LastName;
                        requestClient.Email = formData.Email;
                        requestClient.Phonenumber = formData.PhoneNumber;
                        requestClient.Address = formData.HotelOrPropertyName;
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString();
                        requestClient.Intyear = formData.DateOfBirth.Year;
                        requestClient.Intdate = formData.DateOfBirth.Day;
                        requestClient.Street = formData.Street;
                        requestClient.City = formData.City;
                        requestClient.State = formData.State;
                        requestClient.Zipcode = formData.ZipCode;

                        _requestclientrepo.Add(requestClient);

                        rconcierge.Conciergename = formData.FirstName + " " + formData.LastName;
                        rconcierge.Address = formData.HotelOrPropertyName;
                        rconcierge.City = formData.City;
                        rconcierge.State = formData.State;
                        rconcierge.Street = formData.Street;
                        rconcierge.Zipcode = formData.ZipCode;
                        rconcierge.Createddate = DateTime.Now;

                        _conciergerepo.Add(rconcierge);

                        requestConcierge.Requestid = request.Requestid;
                        requestConcierge.Conciergeid = rconcierge.Conciergeid;

                        _requestConciergerepo.Add(requestConcierge);

                        transaction.Complete();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to submit Form", ex);
                    }
                }
            }
            else
            {
                return RedirectToAction("FamilyFriendRequest");
            }

        }

        public IActionResult BusinessRequest()
        {
            ViewData["ViewName"] = "BusinessRequest";
            return View();
        }

        [HttpPost]
        public IActionResult SubmitBusinessData(BusinessFormData formData)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var request = new Request();
                        var requestClient = new Requestclient();
                        var rbusiness = new RBusinessdatum();
                        var requestbusiness = new Requestbusiness();

                        request.Requesttypeid = 4;
                        request.Firstname = formData.BusinessFirstName;
                        request.Lastname = formData.BusinessLastName;
                        request.Email = formData.BusinessEmail;
                        request.Phonenumber = formData.BusinessPhoneNumber;
                        request.Status = 1;
                        request.Isurgentemailsent = false;
                        request.Isdeleted = false;
                        request.Createddate = DateTime.Now;
                        _requestrepo.Add(request);

                        requestClient.Notes = formData.Symptoms;
                        requestClient.Requestid = request.Requestid;
                        requestClient.Firstname = formData.FirstName;
                        requestClient.Lastname = formData.LastName;
                        requestClient.Email = formData.Email;
                        requestClient.Phonenumber = formData.PhoneNumber;
                        requestClient.Address = formData.BusinessOrPropertyName;
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString();
                        requestClient.Intyear = formData.DateOfBirth.Year;
                        requestClient.Intdate = formData.DateOfBirth.Day;
                        requestClient.Street = formData.Street;
                        requestClient.City = formData.City;
                        requestClient.State = formData.State;
                        requestClient.Zipcode = formData.ZipCode;

                        _requestclientrepo.Add(requestClient);

                        rbusiness.Name = formData.FirstName + " " + formData.LastName;
                        rbusiness.Address1 = formData.BusinessOrPropertyName;
                        rbusiness.City = formData.City;
                        rbusiness.Zipcode = formData.ZipCode;
                        rbusiness.Createddate = DateTime.Now;
                        rbusiness.Status = 1;

                        _rbusinessrepo.Add(rbusiness);


                        requestbusiness.Requestid = request.Requestid;
                        requestbusiness.Businessid = rbusiness.Id;

                        _requestbusinessrepo.Add(requestbusiness);

                        transaction.Complete();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to submit Form", ex);
                    }
                }
            }
            else
            {
                return RedirectToAction("BusinessRequest");
            }

        }


        public IActionResult Dashboard()
        {
            ViewData["ViewName"] = "Dashboard";
            var email = HttpContext.Session.GetString("UserId");
            var username = GetUsernameFromEmail(email); // Extract username from email
            ViewBag.Username = username;
            User user = _userrepo.GetUser(email);
            List<Request> requestlist = _requestrepo.GetAll(user.Userid);
            Debug.Write(requestlist);

            var requestData = from request in _requestrepo.GetAll(user.Userid)
                              join Requestwisefile in _requestwisefilerepo.GetAll()
                              on request.Requestid equals Requestwisefile.Requestid into gj
                              from subfile in gj.DefaultIfEmpty()
                              select new DashboardViewModel
                              {
                                  requestDate = request.Createddate,
                                  requestStatus = request.Status,
                                  documentPath = subfile?.Filename ?? string.Empty,
                              };



            var dashboardRequests = requestData.ToList();

            return View(dashboardRequests);

        }

        public IActionResult DocumentView()
        {
            var email = HttpContext.Session.GetString("UserId");
            var username = GetUsernameFromEmail(email); // Extract username from email
            ViewBag.Username = username;
            ViewData["ViewName"] = "DocumentView";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        private string GetUsernameFromEmail(string email)
        {
            if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            {
                return email.Split('@')[0];
            }
            return email;
        }

    }

}
