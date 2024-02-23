using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IRequestClientRepository
    {
        void Add(Requestclient requestclient);
        public List<Requestclient> GetAll();
        public Requestclient Get(int id);
        public void Update(Requestclient request);
    }
}