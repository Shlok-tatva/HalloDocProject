using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.Interface
{
    public interface ICommonFunctionRepository
    {
        void HandleFileUpload(IFormFile UploadFile, int requestId , int? adminId);

    }
}