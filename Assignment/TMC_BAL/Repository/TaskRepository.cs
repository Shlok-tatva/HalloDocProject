using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMC_BAL.Interface;
using TMC_DAL.DataContext;
using TMC_DAL.Models;

namespace TMC_BAL.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TMC_DAL.Models.Task> GetAll()
        {
            return _context.Tasks.ToList();
        }

        public void Add(TMC_DAL.Models.Task task)
        {
            _context.Add(task);
            _context.SaveChanges();
        }

        public TMC_DAL.Models.Task Get(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == id);

        }

        public void Update(TMC_DAL.Models.Task task)
        {
            _context.Update(task);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            TMC_DAL.Models.Task task = Get(id);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

    }
}
