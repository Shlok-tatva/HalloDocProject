using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repository
{
    public interface IAdminFunctionRepository
    {
        void assignCase(int requestId, int physicianId , int adminId);
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
        void CreateOrUpdateProvider(CreateProviderView model, int[] selectedRegions, bool isEditing);
        List<Physician> GetPhysiciansByRegion(int regionId);
        void transferCase(int requestId, int physicianId, int adminId ,string note);
        void clearCase(int requestId, int adminId);
        void sendAgreement(int requestId, int adminId , string email , string link);
        EncounterFormView GetEncounterFormView(int requestId);
        void SubmitEncounterForm(EncounterFormView formView);
        EncounterFormView GetEncounterForm(int requestId);
        int getEcounterFormStatus(int requestId);
        List<Healthprofessionaltype> getAllProfessions();
        List<Healthprofessional> GetBusinessesByProfession(int professionId);
        Healthprofessional GetBusinessDetailsById(int Vendorid);
        void AddOrder(Orderdetail orderdetail);
        List<Physicianlocation> GetPhysicianlocations();
        AdminProfileView GetAdminProfileView(int adminId);
        void UpdateAdminData(AdminProfileView data);
        void ChagePassword(int adminId, int providerId , string password);
        List<ProviderInfoAdmin> getProviderInfoView();
        CreateProviderView getProviderView(int providerId);
        void updateNotificationStatus(int providerId, bool status);
        List<Healthprofessional> getAllVendors();
        string GetProfessionNameById(int id);
        string GetAccountTypeNameById(int id);
        List<Role> GetAllRole();
        List<Menu> GetAllMenu();
        List<Aspnetrole> getAllRoleType();
        void CreateRole(int adminId , string roleName, int accountType, int[] selectedMenu);
        List<Rolemenu> GetMenuByRole(int roleID);

        }
}