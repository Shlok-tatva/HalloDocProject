
using HalloDoc_BAL.ViewModel.Models;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IAdminFunctionRepository
    {
        IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId);
        public int[] GetStatus(int statusId);

        public ViewCase GetViewCase(int requestId);

        public List<Region> GetAllReagion();
    }
}