using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IPatientFunctionRepository
    {
        string Encrypt(string plainBytes, string Key);
        string Decrypt(string chiperText, string Key);
        bool SendEmail(string toEmail, string Title , string Message );
        void createLog(Requeststatuslog log);
    }
}