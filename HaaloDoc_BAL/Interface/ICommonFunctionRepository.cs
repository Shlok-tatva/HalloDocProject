using HalloDoc_BAL.ViewModel.Admin;
using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.Interface
{
    public interface ICommonFunctionRepository
    {
        string Encrypt(string plainBytes, string Key);
        string Decrypt(string encryptEmail, string Key);
        void HandleFileUpload(IFormFile UploadFile, int requestId , int? adminId);
        string GetConfirmationNumber(string state, string lastname, string firstname);
        public void updateServiceRegion(List<ChangeRegionData> regionsData, int adminId);
    }
}