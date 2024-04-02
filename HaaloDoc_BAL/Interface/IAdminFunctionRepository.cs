using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_BAL.ViewModel.Schedule;
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
        void DeleteProvider(int adminId, int providerId);
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
        bool IsUsernameAvailable(string username);
        void createAdmin(CreateAdminView data, int[] regions , int adminId);

        bool HasExistingShifts(int physicianId, DateTime date, TimeOnly startTime, TimeOnly endTime);
        void CreateShift(ScheduleModel data, int adminId);
        List<ScheduleModel> PhysicianAll();
        List<ScheduleModel> PhysicianByRegion(int? region);
        List<ScheduleModel> GetShift(int month, int? regionId);
        ScheduleModel GetShiftByShiftdetailId(int Shiftdetailid);
        void EditShift(ScheduleModel shift, int adminId);
        void Updateshiftstatus(int shiftId, int adminId);
        void DeleteShift(int shiftId, int adminId);


        List<CreateProviderView> PhysicianOnCall(int? region);
        List<ScheduleModel> GetAllNotApprovedShift(int? regionId, int? month);



        List<Role> GetAllRole();
        List<Menu> GetAllMenu();
        List<Aspnetrole> getAllRoleType();
        void CreateOrUpdateRole(int adminId, string roleName, int accountType, int[] selectedMenu, int? roleId = null);
        void DeleteRole(int adminId, int roleId);
        List<Rolemenu> GetMenuByRole(int roleID);
        string GetMenuNameById(int menuid);
        List<UserAccessView> GetUserAccessView(int roleId);

        }
}