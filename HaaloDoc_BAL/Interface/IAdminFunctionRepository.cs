using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_BAL.ViewModel.Records;
using HalloDoc_BAL.ViewModel.Schedule;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repository
{
    public interface IAdminFunctionRepository
    {
        void assignCase(int requestId, int physicianId , int adminId);
        void blockRequst(int requestId, string reason , int adminId);
        DashboardView GetDashboardView();
        List<Casetag> GetAllCaseTag();
        List<Region> GetAllReagion();
        IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId , int? physicianId);
        int[] GetStatus(int statusId);
        ViewCaseView GetViewCase(int requestId);
        ViewNotesView GetViewNotesView(int requestId);
        void UpdateNotes(int requestId, int? adminId, string? adminNotes, int? providerId, string? providerNotes);
        void cancelCase(int requestId, int adminId, string reason, string note);
        List<ViewUploadView> GetuploadedDocuments(int requestId);
        void DeletefileFromDatabase(int fileId);
        bool SendEmail(string toEmail, string Title, string Message, string[] attachmentFilePaths = null);
        void CreateOrUpdateProvider(CreateProviderView model, int[] selectedRegions, bool isEditing);
        void DeleteProvider(int adminId, int providerId);
        List<Physician> GetPhysiciansByRegion(int regionId);
        void transferCase(int requestId, int physicianId, int adminId ,string note);
        void clearCase(int requestId, int adminId);
        void sendAgreement(int requestId, int? adminId, int? providerId , string email , string link);
        void sendRequestSupport(int adminId, string Message);
        EncounterFormView GetEncounterFormView(int requestId);
        void SubmitEncounterForm(EncounterFormView formView);
        EncounterFormView GetEncounterForm(int requestId);
        int getEcounterFormStatus(int requestId);
        int? getrequestCallType(int requestId);

        List<Healthprofessionaltype> getAllProfessions();
        List<Healthprofessional> GetBusinessesByProfession(int professionId);
        Healthprofessional GetBusinessDetailsById(int Vendorid);
        void AddOrder(Orderdetail orderdetail);
        List<Physicianlocation> GetPhysicianlocations();
        AdminProfileView GetAdminProfileView(int adminId);
        void UpdateAdminData(AdminProfileView data);
        void ChagePassword(int adminId, int providerId , string password);
        List<ProviderInfoAdmin> getProviderInfoView(int? regionId);
        CreateProviderView getProviderView(int providerId);
         void updateNotificationStatus(int providerId, bool status);
        List<Healthprofessional> getAllVendors();
        string GetProfessionNameById(int id);
        string GetAccountTypeNameById(int id);
        bool IsUsernameAvailable(string username);
        void createAdmin(CreateAdminView data, int[] regions , int adminId);

        bool HasExistingShifts(int physicianId, DateTime date, TimeOnly startTime, TimeOnly endTime);
        void CreateShift(ScheduleModel data, int? adminId , int? providerId);
        List<ScheduleModel> PhysicianAll();
        List<ScheduleModel> PhysicianByRegion(int? region);
        List<ScheduleModel> GetShift(int month, int year , int? regionId , int? providerId);
        ScheduleModel GetShiftByShiftdetailId(int Shiftdetailid);
        void EditShift(ScheduleModel shift, int? adminId , int? providerId);
        void Updateshiftstatus(int shiftId, int adminId);
        void DeleteShift(int shiftId, int? adminId , int? providerId);


        List<CreateProviderView> PhysicianOnCall(int? region);
        List<ScheduleModel> GetAllNotApprovedShift(int? regionId, int? month);


        List<SearchRecordView> GetSearchRecords(string? Email, DateTime? FromDoS, string? Phone, string? Patient, string? Provider, int RequestStatus, int RequestType, DateTime? ToDoS);
        List<BlockHistoryView> GetBlockHistoryData(string? name, DateTime? date, string? email, string? phoneNumber);
        void unBlock(int blockrequestId, int requestId);
        List<LogView> GetEmailLogs(int? accountType, string? receiverName, string? emailId, DateTime? createdDate, DateTime? sentDate);
        List<LogView> GetSMSLogs(int? accountType, string? receiverName, string? phoneNumber, DateTime? createdDate, DateTime? sentDate);


        List<Region> getProvidersRegion(int providerId);
        List<Role> GetAllRole();
        List<Menu> GetAllMenu();
        List<Aspnetrole> getAllRoleType();
        void CreateOrUpdateRole(int adminId, string roleName, int accountType, int[] selectedMenu, int? roleId = null);
        void DeleteRole(int adminId, int roleId);
        List<Rolemenu> GetMenuByRole(int roleID);
        string GetMenuNameById(int menuid);
        List<UserAccessView> GetUserAccessView(int roleId);

        /*Provider request*/
        void TransferRequestRequest(int requestId, string reason, int providerId);
        void updateOrCreateProviderLocation(int providerId, float latitude, float longitude, string address);


        }
}