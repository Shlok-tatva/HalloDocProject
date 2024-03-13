using HalloDoc.Models;

using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repository;
using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;
using System.Transactions;

namespace HalloDocAdmin.Controllers
{
    //[CustomAuth("Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IRequestRepository _requestRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IRequestNotesRepository _requestNotesRepository;
        private readonly IAdminFunctionRepository _adminFunctionRepository;
        private readonly ICommonFunctionRepository _commonFunctionrepo;


        public AdminController(ILogger<AdminController> logger, IRequestRepository requestRepository, IRequestClientRepository requestClientRepository, IAdminFunctionRepository adminFunctionRepository, IRequestNotesRepository requestNotesRepository, ICommonFunctionRepository commonFunctionrepo)
        {
            _logger = logger;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _adminFunctionRepository = adminFunctionRepository;
            _requestNotesRepository = requestNotesRepository;
            _commonFunctionrepo = commonFunctionrepo;
        }

        public IActionResult Index()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
           AdminDashboardView view = _adminFunctionRepository.GetAdminDashboardView();

            return View(view);
        }

        [HttpGet]
        public IActionResult GetRequestByStatusId(int status_id)
        {
            var statusIdWiseRequest = _adminFunctionRepository.GetRequestsByStatusID(status_id);
            return Ok(statusIdWiseRequest.ToList());
        }

        [HttpPost("AssignCase")]
        public IActionResult AssignCase(int requestId , int physicianId)
        {
            if (requestId != null && physicianId != null)
            { 
            _adminFunctionRepository.assignCase(requestId, physicianId);
            return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult GetRequestClient(int requestId)
        {
            var request = _requestClientRepository.Get(requestId);
            return Ok(request);
        }

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

        public IActionResult ViewCase()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            int requestId = Int32.Parse(Request.Query["request"]);
            ViewCaseView view = _adminFunctionRepository.GetViewCase(requestId);
            return View(view);
        }


        [HttpGet]
        public IActionResult GetPhysiciansByRegion (int regionId)
        {
            var physicians = _adminFunctionRepository.GetPhysiciansByRegion(regionId).Select(p => new { Id = p.Physicianid, Name = p.Firstname + " " + p.Lastname });
            return Ok(physicians);
        }


        [HttpPost("UpdateEmail")]
        public IActionResult UpdateEmail(int requestId , string Email)
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

        public IActionResult ViewNotes()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            int requestId = Int32.Parse(Request.Query["request"]);
            ViewNotesView view = _adminFunctionRepository.GetViewNotesView(requestId);
            ViewBag.requestId = requestId;
            return View(view);
        }

        [HttpPost("UpdateNotes")]
        public IActionResult UpdateNotes(int requestId , string adminNotes)
        {
            try
            {
                Requestnote requestnote = _requestNotesRepository.Get(requestId);
                if(requestnote == null)
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

        [HttpPost("BlockPatient")]
        public IActionResult BlockPatient(int requestId , string reason)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
  
            try
            {
                _adminFunctionRepository.blockRequst(requestId, reason , adminId);
                return Ok();
            }
            catch (Exception ex)
            {

                return NotFound();
            }
        }


        [HttpPost("CancelCase")]
        public IActionResult CancelCase(int requestId , string reason , string notes)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));

            try
            {
                _adminFunctionRepository.cancelCase(requestId, adminId, reason, notes);
                return Ok();

            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }

          #region DocumetnView
          public IActionResult ViewUpload()
          {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");

            int requestId = Int32.Parse(Request.Query["request"]);
            Request request = _requestRepository.Get(requestId);

            List<ViewUploadView> documetns =  _adminFunctionRepository.GetuploadedDocuments(requestId);
            ViewBag.requestId = requestId;
            ViewBag.CFnumber = request.Confirmationnumber;
            ViewBag.patientName = request.Firstname + " " + request.Lastname;
            return View(documetns);
          }


        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile file, int requestId)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            try
            {
                _commonFunctionrepo.HandleFileUpload(file, requestId , adminId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { success = false, message = "An error occurred while Uploading the file" });
            }
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

        public IActionResult DeleteFile(string filePath , int fileId)
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

        public IActionResult SendfilesonMail(string receverEmail , string[] filePaths)
        {
            try
            {
            var title = "Files attachment below";
            var message = "In this mail you receive you file as a attachment";
            _adminFunctionRepository.SendEmail("shlok.jadeja@etatvasoft.com", title, message, filePaths);
             return Ok();
            } catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost("TransferCase")]
        public IActionResult TransferCase(int requestId , int physicianId, string note)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            try
            {
                _adminFunctionRepository.transferCase(requestId, physicianId, adminId, note);
                return Ok();
            }

            catch(Exception ex)
            {

                return NotFound();
            }
        }
        #endregion


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

        [HttpPost("closeCaseUpdate")]
        public IActionResult closeCaseUpdate(int requestId , string phone, string email)
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
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("ClearCase")]
        public IActionResult ClearCase(int requestId)

        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            try
            {
                _adminFunctionRepository.clearCase(requestId , adminId);
                return Ok();
            }
            catch(Exception ex)
            {

                return NotFound();
            }
        }

         [HttpPost("SendAgreement")]
        public IActionResult SendAgreement(int requestId , string email , string phoneNumber)
        {
            int adminId = Int32.Parse(HttpContext.Session.GetString("AdminId"));
            string key = "770A8A65DA156D24EE2A093277530142";
            string encryptedrequestId = _commonFunctionrepo.Encrypt(requestId.ToString(), key);
            var accountCreationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/patient/reviewagreement?&requestId={encryptedrequestId}";
            _adminFunctionRepository.sendAgreement(requestId, adminId, email ,accountCreationLink);

            return Ok();
        }

        public IActionResult Provider(){
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            var regions = _adminFunctionRepository.GetAllReagion();

            List<ProviderInfoAdmin> view = _adminFunctionRepository.getProviderInfoView();
            ViewBag.regions = regions;

            return View(view);
        }

        [HttpGet]
        [Route("/admin/CreateProvider")]
        public IActionResult ProviderCreate(){
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "Providers";
            var regions = _adminFunctionRepository.GetAllReagion();
            ViewBag.regions = regions;
            return View();
        }


        [HttpPost("ProviderCreate")]
        [Route("/admin/CreateProvider")]
        public IActionResult ProviderCreate(CreateProviderView model , int[] selectedRegions)
        {
            if (ModelState.IsValid)
            {
                //remanign work of Physicina Add
             _adminFunctionRepository.CreateProvider(model, selectedRegions);

            }
            return Ok();
        }

        [HttpGet]
        public IActionResult Encounter()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");

            int requestId = Int32.Parse(Request.Query["request"]);
            EncounterFormView view = _adminFunctionRepository.GetEncounterFormView(requestId);
            if(view == null){
                TempData["Error"] = "Form finalized";
                return RedirectToAction("");
            }
            return View(view);
        }

        [HttpPost("encounter")]
        public IActionResult Encounter(EncounterFormView formData)
        {
            try
            {
            _adminFunctionRepository.SubmitEncounterForm(formData);
            return Ok();
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }

        public int EcounterFormStatus(int requestId)
        {

            return _adminFunctionRepository.getEcounterFormStatus(requestId);
            
        }

        [HttpGet]
        public IActionResult GetEncounterFormDetails(int requestId)
        {
            var encounterFormView = _adminFunctionRepository.GetEncounterForm(requestId);
            return View("EncounterFormDetails" , encounterFormView); 
        }

        [HttpGet]
        public IActionResult GeneratePDF(int requestId)
        {
            var encounterFormView = _adminFunctionRepository.GetEncounterForm(requestId);
            if(encounterFormView == null)
            {
                return NotFound();
            }
            // return View("EncounterFormDetails", encounterFormView);
            return new ViewAsPdf("EncounterFormDetails", encounterFormView)
            {
               FileName = "Encounter_Form.pdf"
            };

        }


        public IActionResult ProviderLocation()
        {
             ViewData["ViewName"] = "ProviderLocation";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }
        [HttpGet]
        public IActionResult GetPhysicianLocation()
        {
            var location = _adminFunctionRepository.GetPhysicianlocations();
            return Json(location);
        }

        public IActionResult Orders(){
            var requestId = Request.Query["request"];
            ViewData["ViewName"] = "Dashboard";
            ViewData["requestId"] = requestId;
            ViewBag.Username = HttpContext.Session.GetString("Username");

            return View();
        }

        [HttpGet]
        public IActionResult getProfessions()
        {
            List<Healthprofessionaltype> allProfession = _adminFunctionRepository.getAllProfessions();
            return Json(allProfession);

        }

        [HttpGet]
        public ActionResult getBusinesses(int professionId)
        {
            List<Healthprofessional> businesses = _adminFunctionRepository.GetBusinessesByProfession(professionId);
            return Json(businesses);
        }

        [HttpGet]
        public ActionResult GetBusinessDetails(int Vendorid)
        {
            Healthprofessional business = _adminFunctionRepository.GetBusinessDetailsById(Vendorid);
            return Json(business);
        }

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

        [HttpPost("changePassword")]
        public IActionResult changePassword(int adminId , string password)
        {
            try
            {
                _adminFunctionRepository.ChagePassword(adminId, password);
                return Ok();
            }
            catch(Exception ex) { 
            
                return BadRequest(ex.Message);
            }
        }
        public IActionResult updateAdmin(AdminProfileView formData)
        {
            try
            {
             _adminFunctionRepository.UpdateAdminData(formData);
            return Redirect("AdminProfile");
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Login/index");
        }
    }
}