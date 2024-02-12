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
    public class RConciergeRepository : IRConciergeRepository
    {
        private readonly ApplicationDbContext _context;

        public RConciergeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(RConcierge rconcierge)

        {
            _context.Add(rconcierge);
            _context.SaveChanges();

        }
    }
}
