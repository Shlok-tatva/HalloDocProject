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
    public class RequestwisefileRepository : IRequestwisefileRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestwisefileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Requestwisefile requestwisefile)
        {
            _context.Add(requestwisefile);
            _context.SaveChanges();
        }

        public List<Requestwisefile> GetAll()
        {
            return _context.Requestwisefiles.ToList();
        }
    }
}
