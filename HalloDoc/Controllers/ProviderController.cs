

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

        [RouteAuthFilter]
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
            catch (Exception ex)
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

        [HttpPost]
        public IActionResult HandleCallType(int callType, int requestId)
        {
            try
            {

                Request rq = _requestRepository.Get(requestId);
                if (callType == 1)
                {
                    rq.Calltype = 1;
                    rq.Modifieddate = DateTime.Now;
                    _requestRepository.Update(rq);
                    return Ok("Request Call Type set to HOUSECALL successfully!");
                }
                else if (callType == 2)
                {
                    // Handle consult
                    rq.Calltype = 2;
                    rq.Status = 6; // set the Request into conclude state
                    rq.Modifieddate = DateTime.Now;
                    _requestRepository.Update(rq);
                    return Ok("Request Call Type set to Consult successfully !");
                }
                else
                {
                    return BadRequest("Invalid call type");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult houseCallevenetHandler(int requestId)
        {
            try
            {
                int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
                Request rq = _requestRepository.Get(requestId);
                rq.Status = 6; // send request to conclude state
                rq.Modifieddate = DateTime.Now;
                _requestRepository.Update(rq);
                _commonFunctionrepo.AddRequestStatusLog(requestId, 6, "Request Completed By Physician still not Concluded", null, providerId, false);
                return Ok();
            }
            catch(Exception ex)
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

        [HttpGet]
        public IActionResult concludeCare()
        {
            ViewData["ViewName"] = "Dashboard";
            ViewBag.Username = HttpContext.Session.GetString("Username");

            int requestId = Int32.Parse(Request.Query["request"]);
            Request request = _requestRepository.Get(requestId);

            List<ViewUploadView> documetns = _adminFunctionRepository.GetuploadedDocuments(requestId);
            ViewCaseView view = _adminFunctionRepository.GetViewCase(requestId);

            var concludeCasedata = new
            {
                Document = documetns,
                RequestId = requestId,
                CFnumber = request.Confirmationnumber,
                FirstName = view.firstName,
                LastName = view.lastName,
            };

            return View(concludeCasedata);
        }

        [HttpPost]
        public IActionResult concludeCare(int requestId , string providerNote)
        {
            try
            {
                int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
                Request rq = _requestRepository.Get(requestId);
                int formStatus = _adminFunctionRepository.getEcounterFormStatus(requestId);

                if(formStatus == null || formStatus == 0)
                {
                    TempData["Error"] = "First finalize the encounter form";
                    return RedirectToAction("Dashboard");
                }

                rq.Status = 8; // Request Concluded By Provider so it is in 
                rq.Completedbyphysician = true;
                rq.Modifieddate = DateTime.Now;
                _requestRepository.Update(rq);
                _commonFunctionrepo.AddRequestStatusLog(requestId, 8, providerNote , null, providerId, false);

                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard");
            }
        }

        [RouteAuthFilter]
        public IActionResult PhyscianProfile()
        {
            ViewData["ViewName"] = "Providers";
            int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
            return RedirectToAction("EditProvider", "Admin", new { providerId = providerId });
        }

        [RouteAuthFilter]
        public IActionResult ProviderSchedule()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["ViewName"] = "ProviderSchedule";
            int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
            var regions = _adminFunctionRepository.getProvidersRegion(providerId);
            ViewBag.regions = regions;
            ViewBag.isprovider = true;
            return View("ProviderSchedule");
        }
        public IActionResult GetShiftByMonth(int? month, int? year, int? regionId)
        {
            int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
            var data = _adminFunctionRepository.GetShift((int)month, (int)year, regionId , providerId);
            return Json(data);
        }
        public IActionResult EditShiftData(ScheduleModel data)
        {
            try
            {
                int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
                _adminFunctionRepository.EditShift(data, null ,providerId);
                TempData["Success"] = "Shift Edited successfully";
                return Redirect("ProviderSchedule");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error While Shift Edit";
                return Redirect("ProviderSchedule");
            }
        }

        public IActionResult CreateShift(ScheduleModel data)
        {
            try
            {
                int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
                data.Status = 0;
                _adminFunctionRepository.CreateShift(data, null, providerId);
                TempData["Success"] = "Shift created successfully";
                return Redirect("ProviderSchedule");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while shift creation";
                return Redirect("ProviderSchedule");
            }
        }

        public IActionResult UpdateproviderLocation(float latitude, float longitude , string address)
        {
            try
            {
                int providerId = Int32.Parse(HttpContext.Session.GetString("providerId"));
                _adminFunctionRepository.updateOrCreateProviderLocation(providerId, latitude, longitude, address);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult ProviderInvoice()
        {
            ViewData["ViewName"] = "ProviderInvoice";
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.TimesheetPeriods = _commonFunctionrepo.GetTimeSheetPeriod();
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logout Successfully";
            return Ok();
        }

    }
}