namespace TMC_BAL.Interface
{
    public interface ITaskRepository
    {
        void Add(TMC_DAL.Models.Task task);
        TMC_DAL.Models.Task Get(int id);
        List<TMC_DAL.Models.Task> GetAll();
        void Update(TMC_DAL.Models.Task task);
        void Delete(int id);
    }
}