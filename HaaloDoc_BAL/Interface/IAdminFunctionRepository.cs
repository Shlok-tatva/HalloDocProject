using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repository
{
    public interface IAdminFunctionRepository
    {
        void blockRequst(int requestId, string reason , int adminId);
        AdminDashboardView GetAdminDashboardView();
        List<Casetag> GetAllCaseTag();
        List<Region> GetAllReagion();
        IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId);
        int[] GetStatus(int statusId);
        ViewCaseView GetViewCase(int requestId);
        ViewNotesView GetViewNotesView(int requestId);
        public void cancelCase(int requestId, int adminId, string reason, string note);
        public List<ViewUploadView> GetuploadedDocuments(int requestId);
        public void DeletefileFromDatabase(int fileId);
        public void SendEmail(string toEmail, string Title, string Message, string[] attachmentFilePaths = null);
    }
}