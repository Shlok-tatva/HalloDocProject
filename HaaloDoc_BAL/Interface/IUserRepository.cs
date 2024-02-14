using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IUserRepository
    {
        void Add(User user);
        public void Update(User user);
        User GetUser(string email);
        User GetUserByID(int  id);
    }
}
