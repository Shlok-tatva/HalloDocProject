using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IAdminRepository
    {
        Admin GetAdmin(string id);
    }
}