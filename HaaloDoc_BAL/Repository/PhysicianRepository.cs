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

    public class PhysicianRepository : IPhysicianRepository
    {
        private readonly ApplicationDbContext _context;
        public PhysicianRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Physician GetPhysicianById(int id)
        {
            return _context.Physicians.FirstOrDefault(a => a.Physicianid == id);
        }
        public Physician GetPhysician(string id)
        {
            return _context.Physicians.FirstOrDefault(a => a.Aspnetuserid == id);
        }

    }

}
