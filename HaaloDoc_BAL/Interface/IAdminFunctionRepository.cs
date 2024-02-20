using HalloDoc_Admin.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IAdminFunctionRepository
    {
        IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId);
        IEnumerable<RequestDataTableView> GetRequestByType(int requestTypeid, int statusId);
    }
}