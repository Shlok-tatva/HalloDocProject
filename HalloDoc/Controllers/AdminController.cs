using HalloDoc.Models;

using HalloDoc_BAL.Interface;
using HalloDoc_BAL.ViewModel.Models;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HalloDocAdmin.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IRequestRepository _requestRepository;
        private readonly IRequestClientRepository _requestClientRepository;
        private readonly IRequestNotesRepository _requestNotesRepository;
        private readonly IAdminFunctionRepository _adminFunctionRepository;


        public AdminController(ILogger<AdminController> logger , IRequestRepository requestRepository, IRequestClientRepository requestClientRepository , IAdminFunctionRepository adminFunctionRepository , IRequestNotesRepository requestNotesRepository)
        {
            _logger = logger;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _adminFunctionRepository = adminFunctionRepository;
            _requestNotesRepository = requestNotesRepository;
        }

        public IActionResult Index()
        {
            ViewData["ViewName"] = "Dashboard";
            var regions = _adminFunctionRepository.GetAllReagion();

            return View(regions);
        }

        [HttpGet]
        public IActionResult GetRequest(int status_id)
        {
            var statusIdWiseRequest = _adminFunctionRepository.GetRequestsByStatusID(status_id);
            return Ok(statusIdWiseRequest.ToList());
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
            ViewCase view = _adminFunctionRepository.GetViewCase(requestId);
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
            Requestnote note = _requestNotesRepository.Get(requestId);
            if(note == null)
            {
                note = new Requestnote();
            }
            ViewBag.requestId = requestId;
            return View(note);
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


    }
}