using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IAdminRepository
    {
        Admin GetAdminById(int id);
        Admin GetAdmin(string id);
        void updateAdmin(Admin admin);
    }
}