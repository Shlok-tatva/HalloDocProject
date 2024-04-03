using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IHealthProfessionalRepository
    {
        void Add(Healthprofessional healthprofessional);
        void Delete(int id);
        Healthprofessional get(int id);
        List<Healthprofessional> getByProfession(int professionId);
        void Update(Healthprofessional healthprofessional); 
    }
}