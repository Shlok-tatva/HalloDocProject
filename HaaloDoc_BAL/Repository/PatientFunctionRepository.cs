using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HalloDoc_BAL.Interface;

namespace HalloDoc_BAL.Repository
{
    public class PatientFunctionRepository : IPatientFunctionRepository
    {
        public string Encrypt(string plainBytes, string Key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainBytes);

            using (Aes aesAlgo = Aes.Create())
            {
                aesAlgo.Key = Encoding.UTF8.GetBytes(Key)
        ;
                aesAlgo.IV = new byte[16];

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlgo.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(bytes, 0, bytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                   return Convert.ToBase64String(msEncrypt.ToArray()).Replace("+", "-").Replace("/", "_").Replace("=", "");
                }
            }
        }

        public string Decrypt(string encryptEmail, string Key)
        {
            string paddedData = encryptEmail;

            // Check if padding is needed
            if (paddedData.Length % 4 != 0)
            {
                paddedData += new string('=', 4 - (paddedData.Length % 4));
            }

            byte[] encryptedBytes = Convert.FromBase64String(paddedData.Replace("-", "+").Replace("_", "/"));

            using (Aes aesAlgo = Aes.Create())
            {
                aesAlgo.Key = Encoding.UTF8.GetBytes(Key);
                aesAlgo.IV = new byte[16];

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlgo.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedBytes, 0, encryptedBytes.Length);
                        csDecrypt.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(msDecrypt.ToArray());
                }
            }
        }

        public void SendEmail(string toEmail, string Title, string Message)
        {
            try
            {
                // Configure SMTP client
                 using (SmtpClient smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io"))
                //using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    var subject = Title;
                    var emailBody = Message;
                    smtpClient.UseDefaultCredentials = false;
                    //smtpClient.Credentials = new NetworkCredential("shlokjadeja177@gmail.com", "pgqv mnuc aspa cglb");
                     smtpClient.Credentials = new NetworkCredential("436da98810b726", "5c043668fb3765");
                    smtpClient.Port = 587;
                    
                    smtpClient.EnableSsl = true;

                    // Construct the email message
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("shlokjadeja177@gmail.com");
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = subject;
                        mailMessage.Body = emailBody;
                        mailMessage.IsBodyHtml = true;

                        // Send the email
                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, log error, etc.
                throw new ApplicationException("Failed to send email", ex);
            }
        }
    }
}
