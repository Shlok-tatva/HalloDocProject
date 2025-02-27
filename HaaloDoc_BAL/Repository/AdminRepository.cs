﻿using HalloDoc_BAL.Interface;
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
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Admin GetAdminById(int id)
        {
            return _context.Admins.Include(a => a.Aspnetuser).FirstOrDefault(a => a.Adminid == id);
        }
        public Admin GetAdmin(string id)
        {
            return _context.Admins.FirstOrDefault(a => a.Aspnetuserid == id);
        }

        public void updateAdmin(Admin admin)
        {
            _context.Admins.Update(admin);
            _context.SaveChanges();
        }

        public Admin GetByEmail(string email)
        {
            return _context.Admins.FirstOrDefault(b => b.Email == email);
        }
    }
}
