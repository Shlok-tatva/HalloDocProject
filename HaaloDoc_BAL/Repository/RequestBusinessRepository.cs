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
    public class RequestBusinessRepository : IRequestBusinessRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestBusinessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Requestbusiness requestBusiness)
        {
            _context.Add(requestBusiness);
            _context.SaveChanges();


        }
    }
}
