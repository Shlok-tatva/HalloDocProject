using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IRequestwisefileRepository
    {
        void Add(Requestwisefile requestwisefile);

        List<Requestwisefile> GetAll();
    }
}