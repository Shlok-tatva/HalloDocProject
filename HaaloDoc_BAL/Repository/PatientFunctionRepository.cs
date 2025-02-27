﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.DataContext;

namespace HalloDoc_BAL.Repository
{
    public class PatientFunctionRepository : IPatientFunctionRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientFunctionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

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


        public bool SendEmail(string toEmail, string Title, string Message)
        {
            try
            {
                // Configure SMTP client
                //using (SmtpClient smtpClient = new SmtpClient("mail.etatvasoft.com"))
                //using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                using (SmtpClient smtpClient = new SmtpClient("smtp.office365.com"))
                {
                    var subject = Title;
                    var emailBody = Message;
                    smtpClient.UseDefaultCredentials = false;
                    //smtpClient.Credentials = new NetworkCredential("shlokjadeja177@gmail.com", "pgqv mnuc aspa cglb");
                    //smtpClient.Credentials = new NetworkCredential("shlok.jadeja@etatvasoft.com", "Shlok@#177");
                    smtpClient.Credentials = new NetworkCredential("tatva.dotnet.shlokjadeja@outlook.com", "Shlok@#177");

                    smtpClient.Port = 587;
                    
                    smtpClient.EnableSsl = true;

                    // Construct the email message
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        //mailMessage.From = new MailAddress("shlokjadeja177@gmail.com"); // add your email here Email is sent by this Email Id
                        //mailMessage.From = new MailAddress("shlok.jadeja@etatvasoft.com");
                        mailMessage.From = new MailAddress("tatva.dotnet.shlokjadeja@outlook.com");
                        //mailMessage.To.Add(toEmail);
                        mailMessage.To.Add("shlok.jadeja@etatvasoft.com"); // for just temporary check 
                        mailMessage.Subject = subject;
                        mailMessage.Body = emailBody;
                        mailMessage.IsBodyHtml = true;

                        // Send the email
                        smtpClient.Send(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void createLog(Requeststatuslog log)
        {
            if(log != null)
            {
                _context.Requeststatuslogs.Add(log);
                _context.SaveChanges();
            }
        }
       public bool isBlockEmail(string email)
        {
            Blockrequest req = _context.Blockrequests.FirstOrDefault(r => r.Email == email);
            if (req != null)
                return true;
            else return false;

        }
    }
}
