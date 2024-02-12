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
    public class RBusinessRepository : IRBusinessRepository
    {
        private readonly ApplicationDbContext _context;

        public RBusinessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(RBusinessdatum rbusinessdata)
        {
            _context.Add(rbusinessdata);
            _context.SaveChanges();
        }
    }
}
