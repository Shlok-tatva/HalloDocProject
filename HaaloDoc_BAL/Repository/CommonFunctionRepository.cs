using HalloDoc_BAL.Interface;
using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{
    public class CommonFunctionRepository : ICommonFunctionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequestwisefileRepository _requestwisefilerepo;
        private readonly IRequestClientRepository _requestclientrepo;

        public CommonFunctionRepository(ApplicationDbContext context, IRequestwisefileRepository requestwisefilerepo , IRequestClientRepository requestclientrepo)
        {
            _context = context;
            _requestwisefilerepo = requestwisefilerepo;
            _requestclientrepo = requestclientrepo;


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


        public void HandleFileUpload(IFormFile UploadFile, int requestId , int? adminId , int? providerId)
        {
            var requestwisefile = new Requestwisefile();
            string FilePath = "wwwroot\\Upload\\" + requestId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
            string UploadImage = "~" + FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + UploadFile.FileName;

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                UploadFile.CopyTo(stream);
            }

            requestwisefile.Requestid = requestId;
            requestwisefile.Filename = UploadImage;
            requestwisefile.Createddate = DateTime.Now;
            requestwisefile.Adminid = adminId;
            requestwisefile.Physicianid = providerId;
            _context.Requestwisefiles.Add(requestwisefile);
            _context.SaveChanges();
        }

        public string GetConfirmationNumber(string state, string lastname, string firstname)
        {

            string Region = state.Substring(0, 2).ToUpperInvariant();

            string NameAbbr = lastname.Substring(0, 2).ToUpperInvariant() + firstname.Substring(0, 2).ToUpperInvariant();

            DateTime requestDateTime = DateTime.Now;

            string datePart = requestDateTime.ToString("ddMMyyyy");

            int requestsCount = GetCountOfTodayRequests() + 1;

            string newRequestCount = requestsCount.ToString("D4");

            string ConfirmationNumber = Region + datePart + NameAbbr + newRequestCount;

            return ConfirmationNumber;

        }

        public void updateServiceRegion(List<ChangeRegionData> regionsData, int adminId)
        {
            try
            {
                regionsData.ForEach(region =>
                {
                   int regionId = Int32.Parse(region.RegionId);
                    if (region.IsChecked == true)
                    {
                        Adminregion adminregion = new Adminregion();
                        adminregion.Adminid = adminId;
                        adminregion.Regionid = regionId;
                        _context.Adminregions.Add(adminregion);
                       

                    }
                    else if (region.IsChecked == false)
                    {
                        _context.Adminregions.Where(r => r.Adminid == adminId && r.Regionid == regionId).ExecuteDelete();
                    }
                    _context.SaveChanges();

                });

            }
            catch (Exception ex)
            {
                
            }
        }

        public List<string> GetMenuItemsForRole(string roleid)
        {
            List<string> menuNames = new List<string>();
            List<Rolemenu> menus = _context.Rolemenus.Where(rm => rm.Roleid == Int32.Parse(roleid)).ToList();
            foreach (var menu in menus)
            {
                var menuName = _context.Menus.Where(rm => rm.Menuid == menu.Menuid).FirstOrDefault().Name;
                menuNames.Add(menuName);
            }
            return menuNames;
        }

        public int GetAccountTypeByroleId(int roleId)
        {
                return (int)_context.Roles.FirstOrDefault(r => r.Roleid == roleId).Accounttype;
        }

        public void EmailLog(string email , string messaage , string subject , string? name , int roleId , int? requestId , int? adminId , int? physicianId , int action , bool isSent , int sentTires)
        {
            try
            {
                Emaillog log = new Emaillog();
                log.Emailtemplate = messaage;
                log.Subjectname = subject;
                log.Emailid = email;
                log.Roleid = roleId;
                log.Createdate = DateTime.Now;
                log.Sentdate = DateTime.Now;
                log.Adminid = adminId;
                log.Requestid = requestId;
                log.Physicianid = physicianId;
                log.Action = action;
                log.Receivername = name;
                if(requestId != null)
                {
                log.Confirmationnumber = _context.Requests.FirstOrDefault(r => r.Requestid == requestId).Confirmationnumber;
                }

                if (isSent)
                {
                    log.Isemailsent = true;
                }
                else
                {
                    log.Isemailsent = false;
                }
                log.Senttries = sentTires;
                _context.Emaillogs.Add(log);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }


        public void AddRequestStatusLog(int requestId, short status, string notes, int? adminId, int? providerId, bool trasnaporttoAdmin)
        {
            Requeststatuslog log = new Requeststatuslog();

            log.Requestid = requestId;
            log.Status = status;
            log.Notes = notes;
            log.Createddate = DateTime.Now;
            log.Physicianid = providerId;
            log.Transtoadmin = trasnaporttoAdmin;
            _context.Requeststatuslogs.Add(log);
            _context.SaveChanges();

        }

        private int GetCountOfTodayRequests()
        {
            var currentDate = DateTime.Now.Date;

            return _context.Requests.Where(u => u.Createddate.Date == currentDate).Count();
        }

        public List<Region> GetAllReagion()
        {
            return _context.Regions.ToList();
        }



    }
}
