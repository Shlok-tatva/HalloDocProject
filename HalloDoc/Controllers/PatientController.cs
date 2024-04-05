using HalloDoc.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Core.Types;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Text;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IPatientFunctionRepository _patientFuncrepo;
        private readonly ICommonFunctionRepository _commonFunctionrepo;




        public PatientController(IAspnetuserRepository aspnetuser, IRequestClientRepository requestclient, IRequestRepository requestrepo, IUserRepository user, IRConciergeRepository conciergerepo, IRequestConciergeRepository requestConciergerepo, IRBusinessRepository rbusinessrepo, IRequestBusinessRepository requestbusinessrepo, IRequestwisefileRepository requestwisefilerepo, IPatientFunctionRepository patientFuncrepo, ICommonFunctionRepository commonFunctionrepo)
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
                        var aspnetuser = _aspnetuserrepo.GetByEmail(formData.Email);
                        var hasher = new PasswordHasher<string>();

                        if (aspnetuser == null)
                        {
                            aspnetuser = new Aspnetuser();

                            Guid guid = Guid.NewGuid();
                            string str = guid.ToString();

                            aspnetuser.Id = str;
                            aspnetuser.Username = formData.Email;
                            aspnetuser.Email = formData.Email;
                            aspnetuser.Phonenumber = formData.PhoneNumber;
                            string hashedPassword = hasher.HashPassword(null, formData.Password);
                            aspnetuser.Passwordhash = hashedPassword;
                            aspnetuser.Createddate = DateTime.Now;
                            aspnetuser.Roleid = 3; // for user it is 3
                            _aspnetuserrepo.Add(aspnetuser);

                            user.Aspnetuserid = aspnetuser.Id;
                            user.Firstname = formData.FirstName;
                            user.Lastname = formData.LastName;
                            user.Email = formData.Email;
                            user.Mobile = formData.PhoneNumber;
                            user.Intyear = formData.DateOfBirth.Year;
                            user.Intdate = formData.DateOfBirth.Day;
                            user.Strmonth = formData.DateOfBirth.Month.ToString("00");
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

                        request.Requesttypeid = 1;
                        request.Userid = user.Userid;
                        request.Firstname = formData.FirstName;
                        request.Lastname = formData.LastName;
                        request.Email = formData.Email;
                        request.Phonenumber = formData.PhoneNumber;
                        request.Status = 1;
                        request.Confirmationnumber = _commonFunctionrepo.GetConfirmationNumber(formData.State , formData.LastName , formData.FirstName);
                        request.Isurgentemailsent = false;
                        request.Createddate = DateTime.Now;
                        _requestrepo.Add(request);

                        if (formData.UploadFile != null)
                        {
                            _commonFunctionrepo.HandleFileUpload(formData.UploadFile , request.Requestid, null);
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
                        var user = _userrepo.GetUser(formData.Email);

                        request.Requesttypeid = 2;
                        request.Firstname = formData.f_firstName;
                        request.Lastname = formData.f_lastName;
                        request.Email = formData.f_Email;

                        if(user != null)
                        {
                            request.Userid = user.Userid;
                        }

                        request.Phonenumber = formData.f_PhoneNumber;
                        request.Status = 1;
                        request.Isurgentemailsent = false;
                        request.Isdeleted = false;
                        request.Createddate = DateTime.Now;
                        request.Relationname = formData.relationWithPatinet;

                        _requestrepo.Add(request);

                        if (formData.UploadFile != null)
                        {
                            _commonFunctionrepo.HandleFileUpload(formData.UploadFile, request.Requestid, null);
                        }

                        requestClient.Notes = formData.Symptoms;
                        requestClient.Requestid = request.Requestid;
                        requestClient.Firstname = formData.FirstName;
                        requestClient.Phonenumber = formData.PhoneNumber;
                        requestClient.Address = formData.Street;
                        requestClient.Lastname = formData.LastName;
                        requestClient.Email = formData.Email;
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString("00");
                        requestClient.Intyear = formData.DateOfBirth.Year;
                        requestClient.Intdate = formData.DateOfBirth.Day;
                        requestClient.Street = formData.Street;
                        requestClient.City = formData.City;
                        requestClient.State = formData.State;
                        requestClient.Zipcode = formData.ZipCode;
                        _requestclientrepo.Add(requestClient);

                        if (user == null)
                        {
                            string key = "770A8A65DA156D24EE2A093277530142";
                            string encryptedEmail = _patientFuncrepo.Encrypt(formData.Email, key);
                            var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/createAccount?email={encryptedEmail}&requestId={request.Requestid}";
                            var title = "Account Creation LInk";
                            var message = $"Please click <a href=\"{accountCreationLink}\">here</a> to create your account.";
                            bool isSent = _patientFuncrepo.SendEmail(formData.Email, title, message);
                            string name = formData.FirstName + " , " + formData.LastName;
                            _commonFunctionrepo.EmailLog(formData.Email, message, title, name , 3, request.Requestid, null, null, 4, isSent, 1);
                        }

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
                        var user = _userrepo.GetUser(formData.Email);


                        request.Requesttypeid = 3;
                        request.Firstname = formData.ConciergeFirstName;
                        request.Lastname = formData.ConciergeLastName;
                        request.Email = formData.ConciergeEmail;
                        if (user != null)
                        {
                            request.Userid = user.Userid;
                        }
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
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString("00");
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

                        if(user == null)
                        {
                            string key = "770A8A65DA156D24EE2A093277530142";
                            string encryptedEmail = _patientFuncrepo.Encrypt(formData.Email, key);
                            var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/createAccount?email={encryptedEmail}&requestId={request.Requestid}";

                            var title = "Account Creation LInk";
                            var message = $"Please click <a href=\"{accountCreationLink}\">here</a> to create your account.";
                            bool isSent = _patientFuncrepo.SendEmail(formData.Email, title, message);
                            string name = formData.FirstName + " , " + formData.LastName;
                            _commonFunctionrepo.EmailLog(formData.Email, message, title, name , 3, request.Requestid, null, null, 4, isSent, 1);

                        }
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
                        var user = _userrepo.GetUser(formData.Email);

                        request.Requesttypeid = 4;
                        request.Firstname = formData.BusinessFirstName;
                        request.Lastname = formData.BusinessLastName;
                        request.Email = formData.BusinessEmail;
                        if (user != null)
                        {
                            request.Userid = user.Userid;
                        }
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
                        requestClient.Strmonth = formData.DateOfBirth.Month.ToString("00");
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

                        if(user == null)
                        {
                            string key = "770A8A65DA156D24EE2A093277530142";
                            string encryptedEmail = _patientFuncrepo.Encrypt(formData.Email, key);
                            var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/createAccount?email={encryptedEmail}&requestId={request.Requestid}";
                            var title = "Account Creation LInk";
                            var message = $"Please click <a href=\"{accountCreationLink}\">here</a> to create your account.";
                            bool isSent = _patientFuncrepo.SendEmail(formData.Email, title, message);
                            string name = formData.FirstName + " , " + formData.LastName;
                            _commonFunctionrepo.EmailLog(formData.Email, message, title, name , 3, request.Requestid, null, null, 4, isSent, 1);

                        }

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

        public IActionResult createAccount()
        {
            string encryptEmail = Request.Query["email"];
            string key = "770A8A65DA156D24EE2A093277530142";
            string email = _patientFuncrepo.Decrypt(encryptEmail, key);

            int requestId = Convert.ToInt32(Request.Query["requestId"]);
            Console.Write(requestId);

            CreateAccountViewModel data = new CreateAccountViewModel { Email = email, requestId = requestId, };

            return View(data);
        }

        public IActionResult submitAccount(CreateAccountViewModel formData)
        {
            Requestclient requestClient = _requestclientrepo.Get(formData.requestId);
            Request request = _requestrepo.Get(formData.requestId);
            User CheckUser = _userrepo.GetUser(formData.Email);

            if(CheckUser == null)
            {
            using(var trancation = new TransactionScope())
            {
                try
                {
                    var aspnetuser = new Aspnetuser();
                    var user = new User();
                    var hasher = new PasswordHasher<string>();

                    Guid guid = Guid.NewGuid();
                    string str = guid.ToString();

                    aspnetuser.Id = str;
                    aspnetuser.Username = formData.Email;
                    aspnetuser.Email = formData.Email;
                    aspnetuser.Phonenumber = requestClient.Phonenumber;
                    string hashedPassword = hasher.HashPassword(null, formData.Password);
                    aspnetuser.Passwordhash = hashedPassword;
                    aspnetuser.Createddate = DateTime.Now;
                    aspnetuser.Roleid = 3;
                    _aspnetuserrepo.Add(aspnetuser);

                    user.Aspnetuserid = aspnetuser.Id;
                    user.Firstname = requestClient.Firstname;
                    user.Lastname = requestClient.Lastname;
                    user.Email = formData.Email;
                    user.Intyear = requestClient.Intyear;
                    user.Intdate = requestClient.Intdate;
                    user.Strmonth = requestClient.Strmonth;
                    user.Mobile = requestClient.Phonenumber;
                    user.Street = requestClient.Street;
                    user.City = requestClient.City;
                    user.State = requestClient.State;
                    user.Zipcode = requestClient.Zipcode;
                    user.Isdeleted = false;
                    user.Createdby = aspnetuser.Id;
                    user.Createddate = DateTime.Now;
                    _userrepo.Add(user);

                    request.Userid = user.Userid;
                    _requestrepo.Update(request);

                    trancation.Complete();
                        return Redirect("/login");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                        return Redirect("/Login");

                    }
                }

            }
            else
            {
                TempData["Error"] = "User Already Exists";
                return Redirect("/Login");
            }



        }

        [HttpGet]
        public IActionResult reviewAgreement()
        {
            var requestId = Request.Query["requestId"];
            string key = "770A8A65DA156D24EE2A093277530142";
            var decryptedrequestId = Int32.Parse(_commonFunctionrepo.Decrypt(requestId, key));
            Request request = _requestrepo.Get(decryptedrequestId);
            ViewBag.PatientName = request.Firstname + " " + request.Lastname;
            ViewBag.createdDate = request.Createddate.ToLongDateString();
            ViewBag.requestId = decryptedrequestId;
            return View();
        }

        [HttpPost]
        public IActionResult reviewAgreement(int requestId , int status , string? cancellationNote)
        {
            using(var transaction = new TransactionScope())
            {
                Request request = _requestrepo.Get(requestId);
                if(request.Status == 2)
                {
                Requeststatuslog log = new Requeststatuslog();
                log.Requestid = requestId;
                log.Createddate = DateTime.Now;

                if(status == 0)
                {
                    request.Status = 4;
                    log.Status = 4;
                    log.Notes = "Agreement accepted by patient";
                    _requestrepo.Update(request);
                    _patientFuncrepo.createLog(log);
                    transaction.Complete();
                    return Ok(new { message = "Agreement accepted by patient" });
                    }
                else
                {
                    request.Status = 7;
                    log.Status = 7;
                    log.Notes = "Agreement canceled by patient , reason:- " + cancellationNote;
                    _requestrepo.Update(request);
                    _patientFuncrepo.createLog(log);
                    transaction.Complete();
                    return Ok(new { message = "Agreement canceled by patient" });
                    }
                }
                else
                {
                    return BadRequest("Maybe You already accept/cancel agreement");
                }
            }
        }
    }
}




