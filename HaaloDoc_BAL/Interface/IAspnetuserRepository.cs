using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IAspnetuserRepository
    {
        void Add(Aspnetuser aspnetuser);
        Task DeleteAsync(string id);
        bool Exists(string email);
        Task<List<Aspnetuser>> GetAllAsync();
        Task<Aspnetuser> GetByIdAsync(string id);
        void Update(Aspnetuser aspnetuser);
        string GetUserPassword(string email);
    }
}