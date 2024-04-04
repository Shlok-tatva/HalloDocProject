using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<User> GetAll()
        {
            return _context.Users.Where(u=>u.Isdeleted == false).ToList();
        }
        public void Add(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }
        public  User GetUser(string email)
        {
            return _context.Users.FirstOrDefault(m => m.Email == email);
        }

        public User GetUserByID(int id)
        {
            return _context.Users.FirstOrDefault(m => m.Userid == id);
        }

        public List<User> GetBySearch(string? firstName, string? lastName, string? email, string? phoneNumber)
        {
            var query = _context.Users.AsQueryable();
            firstName = firstName?.ToLower();
            lastName = lastName?.ToLower();
            email = email?.ToLower();
            phoneNumber = phoneNumber?.ToLower();

            query = query.Where(u =>
                (string.IsNullOrEmpty(firstName) || EF.Functions.Like(u.Firstname.ToLower(), $"%{firstName}%")) &&
                (string.IsNullOrEmpty(lastName) || EF.Functions.Like(u.Lastname.ToLower(), $"%{lastName}%")) &&
                (string.IsNullOrEmpty(email) || EF.Functions.Like(u.Email.ToLower(), $"%{email}%")) &&
                (string.IsNullOrEmpty(phoneNumber) || u.Mobile.ToLower() == phoneNumber));
            return query.ToList();
        }

    }
}
