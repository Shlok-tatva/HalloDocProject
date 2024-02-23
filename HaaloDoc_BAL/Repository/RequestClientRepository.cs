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
    public class RequestClientRepository : IRequestClientRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Requestclient requestclient)
        {
            _context.Add(requestclient);
            _context.SaveChanges();

        }

        public List<Requestclient> GetAll()
        {
            return _context.Requestclients.ToList();
        }

        public Requestclient Get(int id)
        {
            return _context.Requestclients.FirstOrDefault(r => r.Requestid == id);

        }

        public void Update(Requestclient request){
            _context.Requestclients.Update(request);
            _context.SaveChanges();
        }

    }
}
