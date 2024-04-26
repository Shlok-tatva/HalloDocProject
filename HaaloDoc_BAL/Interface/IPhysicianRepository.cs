using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IPhysicianRepository
    {
        Physician GetPhysician(string id);
        Physician GetPhysicianById(int id);
        Physician GetPhysicianByEmail(string email);

    }
}