using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace HalloDoc_BAL.Repository
{
    public class AspnetuserRepository : IAspnetuserRepository
    {
        private readonly ApplicationDbContext _context;

        public AspnetuserRepository(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public void Add(HalloDoc_DAL.Models.Aspnetuser aspnetuser)
        {
            _context.Add(aspnetuser);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(string id)
        {
            var aspnetuser = await _context.Aspnetusers.FindAsync(id);
            if (aspnetuser != null)
            {
                _context.Aspnetusers.Remove(aspnetuser);
            }
            await _context.SaveChangesAsync();
        }

        public bool Exists(string email)
        {
            return _context.Aspnetusers.Any(e => e.Username == email);
        }

        public Aspnetuser GetByEmail (string email)
        {
            return  _context.Aspnetusers.FirstOrDefault(m => m.Email == email);
        }

        public void Update(Aspnetuser aspnetuser)
        {
            _context.Update(aspnetuser);
            _context.SaveChanges();
        }

        public string GetUserPassword(string email)
        {
            var user = _context.Aspnetusers.FirstOrDefault(u => u.Email == email);
            if(user == null)
            {
                return null;
            }
            else
            {
            return user.Passwordhash;
            }
        }
    }
}
