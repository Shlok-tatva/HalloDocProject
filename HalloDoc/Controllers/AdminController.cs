
using HalloDoc.Models;

using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repository;
using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_BAL.ViewModel.Records;
using HalloDoc_BAL.ViewModel.Schedule;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Transactions;

namespace HalloDocAdmin.Controllers
{

    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IRequestNotesRepository _requestNotesRepository;
        private readonly IAdminFunctionRepository _adminFunctionRepository;
        private readonly ICommonFunctionRepository _commonFunctionrepo;
        private readonly IAdminRepository _adminrepo;
        private readonly IHealthProfessionalRepository _healthprofessionalRepository;

        public AdminController(ILogger<AdminController> logger, IRequestRepository requestRepository, IRequestClientRepository requestClientRepository, IAdminFunctionRepository adminFunctionRepository, IRequestNotesRepository requestNotesRepository, ICommonFunctionRepository commonFunctionrepo, IAdminRepository adminrepo, IHealthProfessionalRepository healthprofessionalRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _adminFunctionRepository = adminFunctionRepository;
            _requestNotesRepository = requestNotesRepository;
            _commonFunctionrepo = commonFunctionrepo;
            _adminrepo = adminrepo;
            _healthprofessionalRepository = healthprofessionalRepository;
            _userRepository = userRepository;
        }

        [CustomAuth("Admin")]
        [RouteAuthFilter]
        public IActionResult Dashboard()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            DashboardView view = _adminFunctionRepository.GetDashboardView();
            return View(view);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public IActionResult GetRequestByStatusId(int statusID, int reqtype, int regionFilter)
        {
            string providerid = HttpContext.Session.GetString("providerId");
            IEnumerable<RequestDataTableView> statusIdWiseRequest;
            statusIdWiseRequest = _adminFunctionRepository.GetRequestsByStatusID(statusID, null);
            if (providerid != null)
            {
                int providerId = Int32.Parse(providerid);
                statusIdWiseRequest = _adminFunctionRepository.GetRequestsByStatusID(statusID, providerId);
            }

            if (reqtype != 0 && regionFilter != 0)
            {
                return Ok(statusIdWiseRequest.Where(x => x.RequestTyepid == reqtype && x.regionId == regionFilter).ToList());
            }
            else if (reqtype == 0 && regionFilter != 0)
            {
                return Ok(statusIdWiseRequest.Where(x => x.regionId == regionFilter).ToList());

            }
            else if (reqtype != 0 && regionFilter == 0)
            {
                return Ok(statusIdWiseRequest.Where(x => x.RequestTyepid == reqtype).ToList());
            }

            return Ok(statusIdWiseRequest.ToList());
        }


        #region All-Options-requests

        [CustomAuth("Admin", "Provider")]
        [HttpPost]
        public IActionResult AssignCase(int requestId, int physicianId)
        {

            if (requestId != null && physicianId != null)
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.assignCase(requestId, physicianId, adminId);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public IActionResult GetRequestClient(int requestId)
        {
            var request = _requestClientRepository.Get(requestId);
            return Ok(request);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public IActionResult getRequestCountPerStatusId()
        {
            Dictionary<int, int> statusCounts = new Dictionary<int, int>();

            for (int i = 1; i <= 6; i++)
            {

                int[] statusIds = _adminFunctionRepository.GetStatus(i);
                int count = _requestRepository.GetAll().Count(row => statusIds.Contains(row.Status));
                statusCounts.Add(i, count);
            }
            Console.Write(statusCounts);
            return Ok(statusCounts);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpPost("SendLink")]
        public IActionResult SendLink(string firstNameSendLink, string lastNameSendLink, string phoneSendLink, string emailSendLink)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                var title = "Create Request Link";
                var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/PatientRequest";

                var message = "Please use this link to create Request " + accountCreationLink;
                bool isSent = _adminFunctionRepository.SendEmail(emailSendLink, title, message);
                string name = firstNameSendLink + " , " + lastNameSendLink;
                _commonFunctionrepo.EmailLog(emailSendLink, message, title, name, 1, null, adminId, null, 1, isSent, 1);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult createRequest()
        {
            return View();
        }

        [CustomAuth("Admin", "Provider")]
        public IActionResult ViewCase()
            {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            int requestId = Int32.Parse(Request.Query["request"]);
            ViewCaseView view = _adminFunctionRepository.GetViewCase(requestId);
            var castag = _adminFunctionRepository.GetAllCaseTag();
            ViewData["casetag"] = castag;
            return View(view);
        }


        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult GetPhysiciansByRegion(int regionId)
        {
            var physicians = _adminFunctionRepository.GetPhysiciansByRegion(regionId).Select(p => new { Id = p.Physicianid, Name = p.Firstname + " " + p.Lastname });
            return Ok(physicians);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpPost("UpdateEmail")]
        public IActionResult UpdateEmail(int requestId, string Email)
        {
            try
            {
                Requestclient requestclient = _requestClientRepository.Get(requestId);
                requestclient.Email = Email;
                _requestClientRepository.Update(requestclient);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [CustomAuth("Admin", "Provider")]
        public IActionResult ViewNotes()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            int requestId = Int32.Parse(Request.Query["request"]);
            ViewNotesView view = _adminFunctionRepository.GetViewNotesView(requestId);
            ViewBag.requestId = requestId;
            return View(view);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpPost("UpdateNotes")]
        public IActionResult UpdateNotes(int requestId, string adminNotes)
        {
            try
            {
                Requestnote requestnote = _requestNotesRepository.Get(requestId);
                if (requestnote == null)
                {
                    Requestnote note = new Requestnote();
                    note.Requestid = requestId;
                    note.Adminnotes = adminNotes;
                    note.Createdby = "faeb647e-a0fe-4b31-a87d-4a2c9693242b";
                    note.Createddate = DateTime.Now;
                    _requestNotesRepository.Add(note);
                    return Ok();
                }
                else
                {
                    requestnote.Adminnotes = adminNotes;
                    requestnote.Modifiedby = "faeb647e-a0fe-4b31-a87d-4a2c9693242b";
                    requestnote.Modifieddate = DateTime.Now;
                    _requestNotesRepository.Update(requestnote);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [CustomAuth("Admin")]
        [HttpPost("BlockPatient")]
        public IActionResult BlockPatient(int requestId, string reason)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));

            try
            {
                _adminFunctionRepository.blockRequst(requestId, reason, adminId);
                return Ok();
            }
            catch (Exception ex)
            {

                return NotFound();
            }
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult CancelCase(int requestId, string reason, string notes)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));

            try
            {
                _adminFunctionRepository.cancelCase(requestId, adminId, reason, notes);
                return Ok();

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        #region DocumetnView
        [CustomAuth("Admin", "Provider")]
        public IActionResult ViewUpload()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");

            int requestId = Int32.Parse(Request.Query["request"]);
            Request request = _requestRepository.Get(requestId);

            List<ViewUploadView> documetns = _adminFunctionRepository.GetuploadedDocuments(requestId);
            ViewBag.requestId = requestId;
            ViewBag.CFnumber = request.Confirmationnumber;
            ViewBag.patientName = request.Firstname + " " + request.Lastname;
            return View(documetns);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile file, int requestId)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            try
            {
                _commonFunctionrepo.HandleFileUpload(file, requestId, adminId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { success = false, message = "An error occurred while Uploading the file" });
            }
        }

        [CustomAuth("Admin", "Provider")]
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

        [CustomAuth("Admin", "Provider")]
        public IActionResult DeleteFile(string filePath, int fileId)
        {
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('~', '/'));

            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound();
            }
            try
            {
                _adminFunctionRepository.DeletefileFromDatabase(fileId);
                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred while deleting the file: {ex.Message}");
            }
        }

        [CustomAuth("Admin", "Provider")]
        public IActionResult SendfilesonMail(string receverEmail, string[] filePaths , int requestId)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                var title = "Files attachment below";
                var message = "In this mail you receive you file as a attachment";
                bool isSent = _adminFunctionRepository.SendEmail("shlok.jadeja@etatvasoft.com", title, message, filePaths);
                HalloDoc_DAL.Models.Request rq = _requestRepository.Get(requestId);
                string name = rq.Firstname + " , " + rq.Lastname;
                _commonFunctionrepo.EmailLog("shlok.jadeja@etatvasoft.com" , message, "Sent files on Mail", name, 1 , requestId, adminId, null, 2, isSent, 1);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        [HttpPost("TransferCase")]
        public IActionResult TransferCase(int requestId, int physicianId, string note)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            try
            {
                _adminFunctionRepository.transferCase(requestId, physicianId, adminId, note);
                return Ok();
            }

            catch (Exception ex)
            {

                return NotFound();
            }
        }
        #endregion

        [CustomAuth("Admin")]
        public IActionResult closeCase()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");

            int requestId = Int32.Parse(Request.Query["request"]);
            Request request = _requestRepository.Get(requestId);

            List<ViewUploadView> documetns = _adminFunctionRepository.GetuploadedDocuments(requestId);
            ViewCaseView view = _adminFunctionRepository.GetViewCase(requestId);

            var closeCaseData = new
            {
                Document = documetns,
                RequestId = requestId,
                CFnumber = request.Confirmationnumber,
                FirstName = view.firstName,
                LastName = view.lastName,
                DateOfBirth = view.dateofBirth,
                Email = view.email,
                PhoneNumber = view.phoneNumber,
            };

            ViewBag.requestId = requestId;
            ViewBag.CFnumber = request.Confirmationnumber;
            ViewBag.patientName = request.Firstname + " " + request.Lastname;
            return View(closeCaseData);
        }

        [CustomAuth("Admin")]
        [HttpPost("closeCaseUpdate")]
        public IActionResult closeCaseUpdate(int requestId, string phone, string email)
        {
            try
            {
                Requestclient requestClient = _requestClientRepository.Get(requestId);
                requestClient.Phonenumber = phone;
                requestClient.Email = email;
                _requestClientRepository.Update(requestClient);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        [HttpPost("HandleCloseCase")]
        public IActionResult HandleCloseCase(int requestId)
        {
            try
            {
                Request request = _requestRepository.Get(requestId);
                request.Status = 9;
                _requestRepository.Update(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [CustomAuth("Admin")]
        [HttpPost("ClearCase")]
        public IActionResult ClearCase(int requestId)

        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            try
            {
                _adminFunctionRepository.clearCase(requestId, adminId);
                return Ok();
            }
            catch (Exception ex)
            {

                return NotFound();
            }
        }

        [CustomAuth("Admin", "Provider")]
        [HttpPost("SendAgreement")]
        public IActionResult SendAgreement(int requestId, string email, string phoneNumber)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            string key = "770A8A65DA156D24EE2A093277530142";
            string encryptedrequestId = _commonFunctionrepo.Encrypt(requestId.ToString(), key);
            var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/reviewagreement?&requestId={encryptedrequestId}";
            _adminFunctionRepository.sendAgreement(requestId, adminId, email, accountCreationLink);

            return Ok();
        }
        #endregion

        #region Encounter-Page-and-PDFGenration

        [HttpGet]
        [CustomAuth("Admin", "Provider")]
        public IActionResult Encounter()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");

            int requestId = Int32.Parse(Request.Query["request"]);
            EncounterFormView view = _adminFunctionRepository.GetEncounterFormView(requestId);
            if (view == null)
            {
                TempData["Error"] = "Form finalized";
                return RedirectToAction("");
            }
            return View(view);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpPost("encounter")]
        public IActionResult Encounter(EncounterFormView formData)
        {
            try
            {
                _adminFunctionRepository.SubmitEncounterForm(formData);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [CustomAuth("Admin", "Provider")]
        public int EcounterFormStatus(int requestId)
        {

            return _adminFunctionRepository.getEcounterFormStatus(requestId);

        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public IActionResult GetEncounterFormDetails(int requestId)
        {
            var encounterFormView = _adminFunctionRepository.GetEncounterForm(requestId);
            return View("EncounterFormDetails", encounterFormView);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public IActionResult GeneratePDF(int requestId)
        {
            var encounterFormView = _adminFunctionRepository.GetEncounterForm(requestId);
            if (encounterFormView == null)
            {
                return NotFound();
            }
            // return View("EncounterFormDetails", encounterFormView);
            return new ViewAsPdf("EncounterFormDetails", encounterFormView)
            {
                FileName = "Encounter_Form.pdf"
            };

        }
        #endregion

        #region Physician-location-page

        [CustomAuth("Admin")]
        [RouteAuthFilter]
        public IActionResult ProviderLocation()
        {
            ViewData["ViewName"] = "ProviderLocation";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View("Provider/ProviderLocation");
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult GetPhysicianLocation()
        {
            var location = _adminFunctionRepository.GetPhysicianlocations();
            return Json(location);
        }
        #endregion

        #region Order-Page
        [CustomAuth("Admin", "Provider")]
        public IActionResult Orders()
        {
            var requestId = Request.Query["request"];
            ViewData["ViewName"] = "Dashboard";
            ViewData["requestId"] = requestId;
            ViewBag.Username = HttpContext.Session.GetString("Username");

            return View();
        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public IActionResult getProfessions()
        {
            List<Healthprofessionaltype> allProfession = _adminFunctionRepository.getAllProfessions();
            return Json(allProfession);

        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public ActionResult getBusinesses(int professionId)
        {
            List<Healthprofessional> businesses = _adminFunctionRepository.GetBusinessesByProfession(professionId);
            return Json(businesses);
        }

        [CustomAuth("Admin", "Provider")]
        [HttpGet]
        public ActionResult GetBusinessDetails(int Vendorid)
        {
            Healthprofessional business = _adminFunctionRepository.GetBusinessDetailsById(Vendorid);
            return Json(business);
        }

        [CustomAuth("Admin", "Provider")]
        public IActionResult Addorder(Orderdetail details)
        {
            try
            {
                if (details != null)
                {
                    details.Createddate = DateTime.Now;
                    _adminFunctionRepository.AddOrder(details);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region AdminProfile-page
        [RouteAuthFilter]
        [CustomAuth("Admin")]
        public IActionResult AdminProfile()
        {
            ViewData["ViewName"] = "AdminProfile";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            AdminProfileView view = _adminFunctionRepository.GetAdminProfileView(adminId);
            return View(view);
        }

        [CustomAuth("Admin")]
        [HttpPost("changeAdminPassword")]
        public IActionResult changeAdminPassword(int adminId, string password)
        {
            try
            {
                _adminFunctionRepository.ChagePassword(adminId, 0, password);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        public class AdminUpdateData
        {
            public AdminProfileView Formdata { get; set; }
            public List<ChangeRegionData> Regions { get; set; }
        }

        [HttpPost]
        [CustomAuth("Admin")]
        public IActionResult UpdateAdminInfo([FromBody] AdminUpdateData adminData)
        {
            try
            {
                _adminFunctionRepository.UpdateAdminData(adminData.Formdata);
                _commonFunctionrepo.updateServiceRegion(adminData.Regions, adminData.Formdata.adminId);


                return Redirect("AdminProfile");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Provider-Page
        [RouteAuthFilter]
        [CustomAuth("Admin")]
        public IActionResult Provider(int? regionId)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            var regions = _adminFunctionRepository.GetAllReagion();
            List<ProviderInfoAdmin> view = new List<ProviderInfoAdmin>();

            if (regionId != null)
            {
                view = _adminFunctionRepository.getProviderInfoView(regionId);
            }
            else
            {
                view = _adminFunctionRepository.getProviderInfoView(null);
            }

            ViewBag.regions = regions;
            ViewBag.adminId = adminId;

            if (regionId != null)
            {
                return PartialView("provider/_providerList", view);
            }

            return View("provider/index", view);
        }

        [RouteAuthFilter]
        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult CreateProvider()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            ViewBag.regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.allRoles = _adminFunctionRepository.GetAllRole();

            return View("provider/CreateProvider");
        }

        [CustomAuth("Admin")]
        [HttpPost("CreateProvider")]
        public IActionResult CreateProvider(CreateProviderView model, int[] selectedRegions)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                Admin admin = _adminrepo.GetAdminById(adminId);
                model.Createdby = admin.Aspnetuserid;
                model.allRoles = _adminFunctionRepository.GetAllRole();
                _adminFunctionRepository.CreateOrUpdateProvider(model, selectedRegions, false);
                TempData["Success"] = "Provider Created Successfully !";
                return Redirect("/admin/Provider");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while Creating Physician";
                return Redirect("/admin/CreateProvider");
            }

        }

        [CustomAuth("Admin")]
        [RouteAuthFilter]
        [HttpGet]
        public IActionResult EditProvider()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;
            int providerId = Int32.Parse(Request.Query["providerId"]);
            var view = _adminFunctionRepository.getProviderView(providerId);
            return View("provider/EditProvider", view);
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult EditProvider(CreateProviderView formData, int[] selectedRegions)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                Admin admin = _adminrepo.GetAdminById(adminId);
                formData.Modifiedby = admin.Aspnetuserid;
                _adminFunctionRepository.CreateOrUpdateProvider(formData, selectedRegions, true);
                TempData["Success"] = "Provider Edited Successfully !";
                return RedirectToAction("EditProvider", "Admin", new { providerID = formData.ProviderId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong Try after some times!";
                return RedirectToAction("EditProvider", "Admin", new { providerID = formData.ProviderId });
            }
        }

        [CustomAuth("Admin")]
        public IActionResult DeleteProvider(int providerId)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.DeleteProvider(adminId, providerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        [HttpPost("changeProviderPassword")]
        public IActionResult changeProviderPassword(int providerId, string password)
        {
            try
            {
                _adminFunctionRepository.ChagePassword(0, providerId, password);
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult contactProvider(int physicianId, int adminId, string selectedRadio, string email, string phone, string message)
        {
            try
            {
                var admin = _adminrepo.GetAdminById(adminId);
                var title = admin.Firstname + " " + admin.Lastname + " (Admin) Sent Message";
                bool isSent;

                if (selectedRadio == "Phone")
                {

                    return Ok(new { message = "Message sent successfully via Phone." });

                }
                else if (selectedRadio == "Email")
                {

                    isSent = _adminFunctionRepository.SendEmail(email, title, message);
                    _commonFunctionrepo.EmailLog(email , message, title, null, 1 , null, adminId, physicianId, 3, isSent, 1);
                    return Ok(new { message = "Message sent successfully via Email." });
                }
                else if (selectedRadio == "Both")
                {
                    isSent = _adminFunctionRepository.SendEmail(email, title, message);
                    _commonFunctionrepo.EmailLog(email, message, title, null , 1, null, adminId, physicianId, 3, isSent, 1);


                    return Ok(new { message = "Message sent successfully via Phone and Email." });
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult uploadProviderDocument(IFormFile file, string filename)
        {
            try
            {

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class PhysicianNotificationData
        {
            public int physicianId { get; set; }
            public bool isChecked { get; set; }
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult ChangeProviderNotificationStatus([FromBody] List<PhysicianNotificationData> PhysicianNotificationDataList)
        {
            try
            {
                foreach (var PhysicianNotificationData in PhysicianNotificationDataList)
                {
                    _adminFunctionRepository.updateNotificationStatus(PhysicianNotificationData.physicianId, PhysicianNotificationData.isChecked);
                }

                return Ok(new { message = "Notification status change successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        #endregion

        #region Partner-Page

        [CustomAuth("Admin")]
        [RouteAuthFilter]
        [HttpGet]
        public IActionResult Partners(int? professionId)
        {
            ViewData["ViewName"] = "Partners";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.professionType = _adminFunctionRepository.getAllProfessions();
            List<Healthprofessional> view = new List<Healthprofessional>();

            if (professionId != null)
            {
                view = _healthprofessionalRepository.getByProfession((int)professionId);
            }
            else
            {
                view = _adminFunctionRepository.getAllVendors();
            }

            view.Sort((a, b) => b.Vendorid - a.Vendorid);

            Dictionary<int, string> professionNames = new Dictionary<int, string>();
            foreach (var vendor in view)
            {
                if (vendor.Profession != null && !professionNames.ContainsKey((int)vendor.Profession))
                {
                    string professionName = _adminFunctionRepository.GetProfessionNameById((int)vendor.Profession);
                    professionNames.Add((int)vendor.Profession, professionName);
                }
            }
            ViewBag.ProfessionNames = professionNames;

            if (professionId != null)
            {
                return PartialView("Partners/_partnersData", view);
            }

            return View("Partners/index", view);
        }


        //public IActionResult vendorDataByProfession(int professionId)
        //{
        //    try
        //    {
        //        var data = _healthprofessionalRepository.getByProfession(professionId);
        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest();
        //    }
        //}



        [CustomAuth("Admin")]
        [RouteAuthFilter]
        [HttpGet]
        public IActionResult AddPartner()
        {
            ViewData["ViewName"] = "Partners";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.professionType = _adminFunctionRepository.getAllProfessions();

            return View("Partners/AddPartner");
        }

        [CustomAuth("Admin")]
        [HttpPost("AddPartner")]
        public IActionResult AddPartner(Healthprofessional formData)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Healthprofessional healthprofessional = new Healthprofessional
                    {
                        Vendorname = formData.Vendorname,
                        Businesscontact = formData.Businesscontact,
                        Email = formData.Email,
                        Phonenumber = formData.Phonenumber,
                        Profession = formData.Profession,
                        Faxnumber = formData.Faxnumber,
                        Address = formData.Address,
                        City = formData.City,
                        State = formData.State,
                        Zip = formData.Zip,
                        Createddate = DateTime.Now,
                        Isdeleted = false,
                    };
                    _healthprofessionalRepository.Add(healthprofessional);
                    TempData["Success"] = "Vendor Added Sucessfully";
                    return Redirect("/Admin/Partners");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        [RouteAuthFilter]
        [HttpGet]
        public IActionResult EditPartner()
        {
            ViewData["ViewName"] = "Partners";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            int id = Int32.Parse(Request.Query["vendorid"]);
            ViewBag.professionType = _adminFunctionRepository.getAllProfessions();
            Healthprofessional view = _healthprofessionalRepository.get(id);
            return View("Partners/EditPartner", view);
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult EditPartner(Healthprofessional formData)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    formData.Isdeleted = false;
                    formData.Modifieddate = DateTime.Now;
                    _healthprofessionalRepository.Update(formData);
                    return Ok(new { message = "Vendor Update Sucessfully" });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
           ;
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult DeletePartner(int vendorid)
        {
            try
            {
                Healthprofessional healthprofessional = _healthprofessionalRepository.get(vendorid);
                healthprofessional.Isdeleted = true;
                _healthprofessionalRepository.Update(healthprofessional);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region UserAccess-And-RoleAccess

        [CustomAuth("Admin")]
        public IActionResult Access()
        {
            ViewData["ViewName"] = "Access";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            List<Role> view = _adminFunctionRepository.GetAllRole();
            Dictionary<int, string> AccountTypeNames = new Dictionary<int, string>();
            foreach (var Role in view)
            {
                if (Role.Accounttype != null && !AccountTypeNames.ContainsKey((int)Role.Accounttype))
                {
                    string AccountTypeName = _adminFunctionRepository.GetAccountTypeNameById((int)Role.Accounttype);
                    AccountTypeNames.Add((int)Role.Accounttype, AccountTypeName);
                }
            }
            ViewBag.AccountTypeNames = AccountTypeNames;

            return View("Access/index", view);
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult CreateAccess()
        {
            ViewData["ViewName"] = "Access";
            var allRoles = _adminFunctionRepository.getAllRoleType();
            ViewBag.roles = allRoles;
            CreateEditAccessView view = new CreateEditAccessView();
            return View("Access/CreateAccess", view);
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult GetMenuByRole(int roleId)
        {
            if (roleId != null)
            {
                var menus = _adminFunctionRepository.GetAllMenu().Where(m => m.Accounttype == roleId).ToList();
                return Ok(menus);
            }
            else
            {
                return BadRequest();
            }
        }

        [CustomAuth("Admin")]
        public IActionResult CreateRole(string roleName, int accountType, int[] selectedMenu)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.CreateOrUpdateRole(adminId, roleName, accountType, selectedMenu);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult EditAccess()
        {
            ViewData["ViewName"] = "Access";
            int roleId = Int32.Parse(Request.Query["roleId"]);
            var allRoles = _adminFunctionRepository.getAllRoleType();
            ViewBag.roles = allRoles;
            Role role = _adminFunctionRepository.GetAllRole().Where(r => r.Roleid == roleId).First();
            CreateEditAccessView view = new CreateEditAccessView();
            view.roleId = role.Roleid;
            view.allmenus = _adminFunctionRepository.GetAllMenu().Where(r => r.Accounttype == role.Accounttype).ToList();
            view.roleName = role.Name;
            view.accoutType = (int)role.Accounttype;
            view.menusforRole = _adminFunctionRepository.GetMenuByRole(roleId).Select(rm => rm.Menuid).Where(id => id.HasValue).Select(id => id.Value).ToArray();
            return View("Access/EditAccess", view);
        }

        [CustomAuth("Admin")]
        public IActionResult EditRole(int roleId, string roleName, int accountType, int[] selectedMenu)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.CreateOrUpdateRole(adminId, roleName, accountType, selectedMenu, roleId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        public IActionResult DeleteRole(int roleId)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.DeleteRole(adminId, roleId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        public IActionResult UserAccess()
        {
            List<UserAccessView> view = new List<UserAccessView>();
            try
            {
                ViewData["ViewName"] = "Access";
                ViewBag.Username = HttpContext.Session.GetString("Username");
                ViewBag.accoutType = _adminFunctionRepository.getAllRoleType();
                view = _adminFunctionRepository.GetUserAccessView(0);
                return View("Access/UserAccess", view);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Cannot Get Data";
                return View("Access/UserAccess", view);
            }

        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult EditAdmin()
        {
            try
            {
                ViewData["ViewName"] = "Access";
                ViewBag.Username = HttpContext.Session.GetString("Username");
                var regions = _adminFunctionRepository.GetAllReagion();
                ViewBag.regions = regions;
                int adminId = Int32.Parse(Request.Query["adminId"]);
                AdminProfileView view = _adminFunctionRepository.GetAdminProfileView(adminId);
                return View("AdminProfile", view);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Try after sometime";
                return Redirect("Access/UserAccess");
            }
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult EditPhysician()
        {
            try
            {
                ViewData["ViewName"] = "Access";
                ViewBag.Username = HttpContext.Session.GetString("Username");
                var regions = _adminFunctionRepository.GetAllReagion();
                ViewBag.regions = regions;
                int physicianId = Int32.Parse(Request.Query["physicianId"]);
                var view = _adminFunctionRepository.getProviderView(physicianId);
                return View("provider/EditProvider", view);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Try after sometime";
                return Redirect("UserAccess");
            }
        }

        //[RouteAuthFilter]
        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult CreateAdmin()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Access";
            ViewBag.regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.allRoles = _adminFunctionRepository.GetAllRole();

            return View("Access/CreateAdmin");
        }

        [CustomAuth("Admin")]
        [HttpPost]
        public ActionResult CheckUsernameAvailability(string username)
        {

            bool isAvailable = _adminFunctionRepository.IsUsernameAvailable(username);
            return Json(new { available = isAvailable });

        }

        [CustomAuth("Admin")]
        [HttpPost]
        public IActionResult CreateAdmin(CreateAdminView formData, int[] selectedRegions)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.createAdmin(formData, selectedRegions, adminId);
                return Redirect("Access/UserAccess");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while Creating Admin";
                return Redirect("UserAccess");
            }
        }

        #endregion

        #region Scheduling
        [CustomAuth("Admin")]
        public IActionResult Scheduling()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;
            return View("Scheduling/index");
        }


        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult CheckExistingShifts(int physicianId, DateTime date, string startTime, string endTime)
        {
            try
            {
                TimeOnly parsedStartTime = TimeOnly.Parse(startTime);
                TimeOnly parsedEndTime = TimeOnly.Parse(endTime);
                bool hasExistingShifts = _adminFunctionRepository.HasExistingShifts(physicianId, date, parsedStartTime, parsedEndTime);
                return Json(hasExistingShifts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        public IActionResult CreateShift(ScheduleModel data)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                data.Status = 1;
                _adminFunctionRepository.CreateShift(data, adminId);
                TempData["Success"] = "Shift created successfully";
                return Redirect("Scheduling");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while shift creation";
                return Redirect("Scheduling");
            }
        }


        /*For week wise Data*/

        [CustomAuth("Admin")]
        public IActionResult GetShiftByMonth(int? month, int? year, int? regionId)
        {
            var data = _adminFunctionRepository.GetShift((int)month, (int)year, regionId);
            return Json(data);
            //return Ok();
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult GetPhyscianDataForShift(int? region)
        {
            TempData["Status"] = TempData["Status"];

            var data = _adminFunctionRepository.PhysicianAll();

            if (region != 0)
            {
                data = _adminFunctionRepository.PhysicianByRegion(region);

            }

            return Json(data);
        }

        [CustomAuth("Admin")]
        [HttpGet]
        public IActionResult getShiftData(int shiftId)
        {
            try
            {
                var data = _adminFunctionRepository.GetShiftByShiftdetailId(shiftId);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        public IActionResult EditShiftData(ScheduleModel data)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.EditShift(data, adminId);
                TempData["Success"] = "Shift Edited successfully";
                return Redirect("Scheduling");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error While Shift Edit";
                return Redirect("Scheduling");
            }
        }

        [CustomAuth("Admin")]
        public IActionResult UpdateshiftStatus(int shiftId)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.Updateshiftstatus(shiftId, adminId);
                TempData["Success"] = "Shift Status Change Successful";
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CustomAuth("Admin")]
        public IActionResult DeleteShift(int shiftId)
        {
            try
            {
                int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
                _adminFunctionRepository.DeleteShift(shiftId, adminId);
                TempData["Success"] = "Shift Deleted Successful";
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region MDs-on-Call

        [CustomAuth("Admin")]
        public IActionResult ProviderOnCall(int? regionId)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;

            List<CreateProviderView> data = _adminFunctionRepository.PhysicianOnCall(regionId);
            if (regionId != null)
            {
                var filteredData = data.Select(d => new
                {
                    ProviderId = d.ProviderId,
                    onCallStatus = d.onCallStatus,
                    firstName = d.firstName,
                    lastName = d.lastName
                }).ToList();

                return Json(filteredData);

            }
            return View("Scheduling/ProviderOnCall", data);
        }
        #endregion

        #region RequestedShift

        [CustomAuth("Admin")]
        public IActionResult ShiftReview(int? regionId)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;
            List<ScheduleModel> data = _adminFunctionRepository.GetAllNotApprovedShift(null, null);
            if (regionId != null)
            {
                data = _adminFunctionRepository.GetAllNotApprovedShift(regionId, null);
                return PartialView("Scheduling/_requestedShift", data);
            }
            return View("Scheduling/ShiftReview", data);
        }

        [CustomAuth("Admin")]
        public IActionResult CuurentMonthUnApprovedShift(int? regionId)
        {
            try
            {
                var month = DateTime.Now.Month;
                List<ScheduleModel> data = _adminFunctionRepository.GetAllNotApprovedShift(regionId, month);
                return PartialView("Scheduling/_requestedShift", data);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion

        #region Records

        //public IActionResult PatientRecords()
        //{
        //    ViewBag.Username = HttpContext.Session.GetString("Username");
        //    ViewData["ViewName"] = "Records";
        //    var regions = _adminFunctionRepository.GetAllReagion();
        //    ViewBag.regions = regions;
        //    List<User> data = _userRepository.GetAll();
        //    return View("Records/PatientRecords", data);
        //}

        [CustomAuth("Admin")]
        public IActionResult PatientRecords(string? firstName, string? lastName, string? email, string? phoneNumber)
           {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Records";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;
            List<User> data = new List<User>();
            if (firstName == null && lastName == null && email == null && phoneNumber == null)
            {
                data = _userRepository.GetAll();
                return View("Records/PatientRecords", data);
            }
            else
            {
                data = _userRepository.GetBySearch(firstName, lastName, email, phoneNumber);
                 return PartialView("Records/_patientRecordsData", data);

            }
        }


        [CustomAuth("Admin")]
        public IActionResult PatientRequestes()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            int userId = Int32.Parse(Request.Query["Id"]);
            List<Request> data = _requestRepository.GetAll().Where(r=>r.Userid == userId).ToList();
            ViewData["ViewName"] = "Records";
            return View("Records/PatientRequestes" , data);
        }

        [CustomAuth("Admin")]
        public IActionResult BlockHistory(string? name, DateTime? date, string? email, string? phoneNumber)
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Records";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;

            List<BlockHistoryView> data = new List<BlockHistoryView>();
            if(name == null && date == null && email == null && phoneNumber == null)
            {
                data = _adminFunctionRepository.GetBlockHistoryData(null, null, null, null);
                return View("Records/BlockHistory", data);
            }
            else
            {
                data = _adminFunctionRepository.GetBlockHistoryData(name, date, email, phoneNumber);
                return PartialView("Records/_blockUserData", data);
            }

        }


        [HttpPost]
        [CustomAuth("Admin")]
        public IActionResult unBlock(int Id , int requestId)
        {
            try
            {
                _adminFunctionRepository.unBlock(Id, requestId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [CustomAuth("Admin")]
        public IActionResult EmailLogs()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Records";
            var accountType = _adminFunctionRepository.getAllRoleType();
            ViewBag.accountType = accountType;
            ViewBag.LogType = 1;
            List<LogView> data = _adminFunctionRepository.GetEmailLogs(null , null , null , null , null);
            return View("Records/Logs" , data);
        }

        [CustomAuth("Admin")]
        public IActionResult EmailLogsBySearch(int? accountType, string? receiverName, string? emailId, DateTime? createdDate, DateTime? sentDate)
        {
            try
            {
            List<LogView> data = _adminFunctionRepository.GetEmailLogs(accountType, receiverName, emailId, createdDate, sentDate);
            return PartialView("Records/_LogData", data);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }

        }

        [CustomAuth("Admin")]
        public IActionResult SmsLogs()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Records";
            var accountType = _adminFunctionRepository.getAllRoleType();
            ViewBag.accountType = accountType;
            ViewBag.LogType = 2;
            List<LogView> data = _adminFunctionRepository.GetSMSLogs(null, null, null, null, null);
            return View("Records/Logs", data);
        }


        #endregion

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Login/index");
        }
    }
}