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
    [CustomAuth("Provider")]
    public class ProviderController : Controller
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

        public ProviderController(ILogger<AdminController> logger, IRequestRepository requestRepository, IRequestClientRepository requestClientRepository, IAdminFunctionRepository adminFunctionRepository, IRequestNotesRepository requestNotesRepository, ICommonFunctionRepository commonFunctionrepo, IAdminRepository adminrepo, IHealthProfessionalRepository healthprofessionalRepository, IUserRepository userRepository)
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

        //[RouteAuthFilter]
        public IActionResult Dashboard()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            DashboardView view = _adminFunctionRepository.GetDashboardView();
            return View(view);
        }

      

        public IActionResult getRequestCountPerStatusId()
        {
            int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
            Dictionary<int, int> statusCounts = new Dictionary<int, int>();

            for (int i = 1; i <= 4; i++)
            {

                int[] statusIds = _adminFunctionRepository.GetStatus(i);
                int count = _requestRepository.GetAll().Count(row => row.Physicianid == providerId && statusIds.Contains(row.Status));
               
                statusCounts.Add(i, count);
            }
            Console.Write(statusCounts);
            return Ok(statusCounts);
        }


        public IActionResult AcceptCase(int requestId)
        {
            try
            {
                Request rq = _requestRepository.Get(requestId);
                rq.Status = 2;
                rq.Accepteddate = DateTime.Now;
                _requestRepository.Update(rq);

                  int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
                _commonFunctionrepo.AddRequestStatusLog(requestId, 2, "Request Accept by Physician", null, providerId, false);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult TransferRequest(int requestId, string reason)
        {
            int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
            try
            {
                _adminFunctionRepository.TransferRequestRequest(requestId, reason, providerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult ViewCase()
        {
            int requestId = Int32.Parse(Request.Query["request"]);
            return RedirectToAction("ViewCase", "Admin", new { request = requestId });
        }

        public IActionResult ViewNotes()
        {
            int requestId = Int32.Parse(Request.Query["request"]);
            return RedirectToAction("ViewNotes", "Admin", new { request = requestId });
        }

        public IActionResult ViewUpload()
        {
            int requestId = Int32.Parse(Request.Query["request"]);
            return RedirectToAction("ViewUpload", "Admin", new { request = requestId });
        }

        public IActionResult Encounter()
        {
            int requestId = Int32.Parse(Request.Query["request"]);
            return RedirectToAction("Encounter", "Admin", new { request = requestId });
        }

        public IActionResult Orders()
        {
            var requestId = Request.Query["request"];
            return RedirectToAction("Orders", "Admin", new { request = requestId });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Login/index");
        }

    }
}