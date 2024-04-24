using TMC_DAL.Models;

namespace TMC_BAL.Interface
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        void Add(Category category);   
    }
}