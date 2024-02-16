using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IRequestClientRepository
    {
        void Add(Requestclient requestclient);
        public List<Requestclient> GetAll();
        Requestclient Get(int id);
    }
}