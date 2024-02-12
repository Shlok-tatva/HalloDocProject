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
    public class RequestConciergeRepository : IRequestConciergeRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestConciergeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Requestconcierge requestConcierge)
        {
            _context.Add(requestConcierge);
            _context.SaveChanges();


        }
    }
}
