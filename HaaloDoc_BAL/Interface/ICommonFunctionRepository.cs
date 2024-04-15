using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_BAL.ViewModel.Patient;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_BAL.Interface
{
    public interface ICommonFunctionRepository
    {
        string Encrypt(string plainBytes, string Key);
        string Decrypt(string encryptEmail, string Key);
        void HandleFileUpload(IFormFile UploadFile, int requestId , int? adminId , int? providerId);
        string GetConfirmationNumber(string state, string lastname, string firstname);
        List<string> GetMenuItemsForRole(string roleid);
        int GetAccountTypeByroleId(int roleId);
        void updateServiceRegion(List<ChangeRegionData> regionsData, int adminId);
        void EmailLog(string email, string messaage, string subject, string? name ,int roleId, int? requestId, int? adminId, int? physicianId, int action, bool isSent, int sentTires);
        void createRequest(PatientFormData data, int? adminId, int? providerId , string requestScheme, HostString requestHost);
        void AddRequestStatusLog(int requestId, short status, string notes, int? adminId, int? providerId, bool trasnaporttoAdmin);
        List<Region> GetAllReagion();
    }
}