using HalloDoc.Models;

using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repository;
using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Transactions;

namespace HalloDocAdmin.Controllers
{
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
            AdminDashboardView view = _adminFunctionRepository.GetAdminDashboardView();

            return View(view);
        }

        [HttpGet]
        public IActionResult GetRequestByStatusId(int status_id)
        {
            var statusIdWiseRequest = _adminFunctionRepository.GetRequestsByStatusID(status_id);
            return Ok(statusIdWiseRequest.ToList());
        }

        [HttpGet]
        public IActionResult GetRequest(int requestId)
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
            int requestId = Int32.Parse(Request.Query["request"]);
            ViewCaseView view = _adminFunctionRepository.GetViewCase(requestId);
            return View(view);
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
        public IActionResult BlockPatient(int requestId , string reason , int adminId)
        {
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
        public IActionResult CancelCase(int requestId , int adminId , string reason , string notes)
        {
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


          public IActionResult ViewUpload()
          {
            int requestId = Int32.Parse(Request.Query["request"]);
            Request request = _requestRepository.Get(requestId);

            List<ViewUploadView> documetns =  _adminFunctionRepository.GetuploadedDocuments(requestId);
            ViewBag.requestId = requestId;
            ViewBag.Username = request.Firstname + " " + request.Lastname;
            return View(documetns);
          }

        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile file, int requestId)
        {
            int adminId = 4;

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
                System.IO.File.Delete(physicalPath);
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
            var title = "Files attachment below";
            var message = "In this mail you receive you file as a attachment";

            _adminFunctionRepository.SendEmail("shlok.jadeja@etatvasoft.com", title, message, filePaths);

            return Ok();
        }
    }
}