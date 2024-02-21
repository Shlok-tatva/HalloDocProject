using HalloDoc_Admin.Models;
using HalloDoc_BAL.Interface;
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
        private readonly IAdminFunctionRepository _adminFunctionRepository;

        public AdminController(ILogger<AdminController> logger , IRequestRepository requestRepository, IRequestClientRepository requestClientRepository , IAdminFunctionRepository adminFunctionRepository)
        {
            _logger = logger;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _adminFunctionRepository = adminFunctionRepository;
        }

        public IActionResult Index()
        {
            ViewData["ViewName"] = "Dashboard";
            return View();
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
            // Group by the concatenated string of status categories returned by GetStatus
            var allStatusCounts = _requestRepository.GetAll()
                .GroupBy(x => string.Join(",", _adminFunctionRepository.GetStatus(x.Status)))
                .Select(g => new
                {
                    StatusCategory = g.Key, // Status category string
                    Count = g.Count() // Count the number of items in the group
                })
                .ToDictionary(g => g.StatusCategory, g => g.Count); // Convert to dictionary

            // Output the dictionary to check the counts
            Console.WriteLine(allStatusCounts);

            return Ok(allStatusCounts);
        }





    }
}