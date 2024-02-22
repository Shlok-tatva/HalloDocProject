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

    }
}