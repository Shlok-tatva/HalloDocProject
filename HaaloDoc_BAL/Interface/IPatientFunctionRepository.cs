namespace HalloDoc_BAL.Interface
{
    public interface IPatientFunctionRepository
    {
        string Encrypt(string plainBytes, string Key);
        string Decrypt(string chiperText, string Key);
        void SendEmail(string toEmail, string accountCreationLink);
    }
}