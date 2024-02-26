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
    public class RequestNotesRepository : IRequestNotesRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestNotesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Requestnote note)
        {
            _context.Add(note);
            _context.SaveChanges();
        }

        public Requestnote Get(int requestid)
        {
            return _context.Requestnotes.FirstOrDefault(r => r.Requestid == requestid);
        }

        public void Update(Requestnote note)
        {
            _context.Update(note);
            _context.SaveChanges();
        }

    }
}
