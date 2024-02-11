using HalloDoc.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
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


        public PatientController(IAspnetuserRepository aspnetuser, IRequestClientRepository requestclient, IRequestRepository requestrepo, IUserRepository user)
        {
            _aspnetuserrepo = aspnetuser;
            _requestclientrepo = requestclient;
            _requestrepo = requestrepo;
            _userrepo = user;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CheckUser(Aspnetuser user)
        {
            if (user.Email == null && user.Passwordhash == null)
            {
                return Json("Username and password are NULL");
            }
            if (user.Passwordhash != null && _aspnetuserrepo.GetUserPassword(user.Email) == user.Passwordhash)
            {
                return RedirectToAction("SubmitRequest");
            }
            else
            {
                return Json("Wrong Password");
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
                using var transaction = new TransactionScope();
                try
                {
                    var user = new User();
                    var request = new Request();
                    var requestclient = new Requestclient();
                    var aspnetuser = new Aspnetuser();

                    if (!_aspnetuserrepo.Exists(formData.Email))
                    {


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


                        requestclient.Notes = formData.Symptoms;
                        requestclient.Requestid = request.Requestid;
                        requestclient.Firstname = formData.FirstName;
                        requestclient.Address = formData.Street;
                        requestclient.Lastname = formData.LastName;
                        requestclient.Phonenumber = formData.PhoneNumber;
                        requestclient.Email = formData.Email;
                        _requestclientrepo.Add(requestclient);

                        transaction.Complete();
                    }
                    else
                    {
                        user = _userrepo.GetUser(formData.Email);

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


                        requestclient.Notes = formData.Symptoms;
                        requestclient.Requestid = request.Requestid;
                        requestclient.Firstname = formData.FirstName;
                        requestclient.Address = formData.Street;
                        requestclient.Lastname = formData.LastName;
                        requestclient.Phonenumber = formData.PhoneNumber;
                        requestclient.Email = formData.Email;
                        _requestclientrepo.Add(requestclient);

                        transaction.Complete();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to submit Form", ex);

                }
            }
            else
            {
                return RedirectToAction("Login");
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
               
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }

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


