using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Request request)
        {
            _context.Add(request);
            _context.SaveChanges();
        }
        public Request Get(int id)
        {
            return _context.Requests.FirstOrDefault(r => r.Requestid == id);

        }

        public List<Request> GetAll(int userId) {
            return _context.Requests.Where(r => r.Userid == userId).ToList();
        }

        public void Update(Request request)
        {
            _context.Update(request);
            _context.SaveChanges();
        }

        public List<Request> GetRequestFromStatusId(int statusId)
        {
            return _context.Requests.Where(r => r.Status == statusId).ToList();
        }

        public List<Request> GetAll()
        {
            return _context.Requests.ToList();
        }
    }

}