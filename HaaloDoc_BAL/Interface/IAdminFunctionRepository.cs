using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repository
{
    public interface IAdminFunctionRepository
    {
        void assignCase(int requestId, int physicianId);
        void blockRequst(int requestId, string reason , int adminId);
        AdminDashboardView GetAdminDashboardView();
        List<Casetag> GetAllCaseTag();
        List<Region> GetAllReagion();
        IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId);
        int[] GetStatus(int statusId);
        ViewCaseView GetViewCase(int requestId);
        ViewNotesView GetViewNotesView(int requestId);
        void cancelCase(int requestId, int adminId, string reason, string note);
        List<ViewUploadView> GetuploadedDocuments(int requestId);
        void DeletefileFromDatabase(int fileId);
        void SendEmail(string toEmail, string Title, string Message, string[] attachmentFilePaths = null);
        void CreateProvider(CreateProviderView model, int[] selectedRegions);
        List<Physician> GetPhysiciansByRegion(int regionId);
        void transferCase(int requestId, int physicianId, int adminId ,string note);
        void clearCase(int requestId, int adminId);
        void sendAgreement(int requestId, int adminId , string email , string link);
        public EncounterFormView GetEncounterFormView(int requestId);
        public void SubmitEncounterForm(EncounterFormView formView);
        public EncounterFormView GetEncounterForm(int requestId);
        public int getEcounterFormStatus(int requestId);
        public List<Healthprofessionaltype> getAllProfessions();
        public List<Healthprofessional> GetBusinessesByProfession(int professionId);
        public Healthprofessional GetBusinessDetailsById(int Vendorid);
        public void AddOrder(Orderdetail orderdetail);
    }
}