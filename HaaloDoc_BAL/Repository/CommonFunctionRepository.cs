using HalloDoc_BAL.Interface;
using HalloDoc_BAL.ViewModel.Admin;
using HalloDoc_BAL.ViewModel.Patient;
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
using System.Transactions;

namespace HalloDoc_BAL.Repository
{
    public class CommonFunctionRepository : ICommonFunctionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequestwisefileRepository _requestwisefilerepo;
        private readonly IRequestClientRepository _requestclientrepo;
        private readonly IPatientFunctionRepository _patientFunctionRepository;
        public CommonFunctionRepository(ApplicationDbContext context, IRequestwisefileRepository requestwisefilerepo, IRequestClientRepository requestclientrepo, IPatientFunctionRepository patientFunctionRepository)
        {
            _context = context;
            _requestwisefilerepo = requestwisefilerepo;
            _requestclientrepo = requestclientrepo;
            _patientFunctionRepository = patientFunctionRepository;


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


        public void HandleFileUpload(IFormFile UploadFile, int requestId, int? adminId, int? providerId)
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

        public void EmailLog(string email, string messaage, string subject, string? name, int roleId, int? requestId, int? adminId, int? physicianId, int action, bool isSent, int sentTires)
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
                if (requestId != null)
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
            catch (Exception ex)
            {

            }
        }

        public void SMSLog(string phoneNumber, string messaage, string subject, string? name, int roleId, int? requestId, int? adminId, int? physicianId)
        {
            try
            {
                Smslog log = new Smslog();
                log.Smstemplate = messaage;
                log.Mobilenumber = phoneNumber;
                log.Roleid = roleId;
                log.Createdate = DateTime.Now;
                log.Sentdate = DateTime.Now;
                log.Adminid = adminId;
                log.Requestid = requestId;
                log.Physicianid = physicianId;
                log.Action = messaage;
                log.Receivername = name;
                log.Issmssent = true;
                if (requestId != null)
                {
                    log.Confirmationnumber = _context.Requests.FirstOrDefault(r => r.Requestid == requestId).Confirmationnumber;
                }
                log.Senttries = 1;
                _context.Smslogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
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

        public void createRequest(PatientFormData formData, int? adminId, int? providerId, string requestScheme, HostString requestHost)
        {
            using (var transaction = new TransactionScope())
            {
                User user = _context.Users.FirstOrDefault(u => u.Email == formData.Email);
                var request = new Request();
                var requestClient = new Requestclient();
                string state = _context.Regions.Where(r => r.Regionid == formData.regionId).FirstOrDefault().Name;


                request.Requesttypeid = 1;
                if (user != null)
                {
                    request.Userid = user.Userid;
                }
                else
                {
                    request.Userid = null;
                }
                request.Firstname = formData.FirstName;
                request.Lastname = formData.LastName;
                request.Email = formData.Email;
                request.Phonenumber = formData.PhoneNumber;
                request.Status = 1;
                request.Isurgentemailsent = false;
                request.Isdeleted = false;
                request.Createddate = DateTime.Now;
                request.Confirmationnumber = GetConfirmationNumber(state, formData.LastName, formData.FirstName);

                if (formData.RelationWithPatinet != null)
                {
                    request.Relationname = formData.RelationWithPatinet;
                }

                if(providerId != null)
                {
                    request.Status = 2;
                    request.Physicianid = providerId;
                }

                _context.Requests.Add(request);
                _context.SaveChanges();

                if (formData.UploadFile != null)
                {
                    HandleFileUpload(formData.UploadFile, request.Requestid, null, null);
                }

                requestClient.Notes = formData.Symptoms;
                requestClient.Requestid = request.Requestid;
                requestClient.Firstname = formData.FirstName;
                requestClient.Lastname = formData.LastName;
                requestClient.Phonenumber = formData.PhoneNumber;
                requestClient.Email = formData.Email;
                requestClient.Strmonth = formData.DateOfBirth.Month.ToString("00");
                requestClient.Intyear = formData.DateOfBirth.Year;
                requestClient.Intdate = formData.DateOfBirth.Day;
                requestClient.Street = formData.Street;
                requestClient.City = formData.City;
                requestClient.State = state;
                requestClient.Regionid = formData.regionId;
                requestClient.Zipcode = formData.ZipCode;

                _requestclientrepo.Add(requestClient);

                Requestnote note = new Requestnote();
                if(adminId != null)
                {
                    Admin admin = _context.Admins.FirstOrDefault(a => a.Adminid == adminId);
                    note.Requestid = request.Requestid;
                    note.Createddate = DateTime.Now;
                    note.Createdby = admin.Aspnetuserid;
                    note.Adminnotes = formData.adminNotes;
                    _context.Requestnotes.Add(note);

                }
                if(providerId != null)
                {
                    Physician pro = _context.Physicians.FirstOrDefault(a => a.Physicianid == providerId);
                    note.Requestid = request.Requestid;
                    note.Createddate = DateTime.Now;
                    note.Createdby = pro.Aspnetuserid;
                    note.Physiciannotes = formData.providerNotes;
                    _context.Requestnotes.Add(note);
                }
                _context.SaveChanges();

                string key = "770A8A65DA156D24EE2A093277530142";
                string encryptedEmail = Encrypt(formData.Email, key);
                var accountCreationLink = $"{requestScheme}://{requestHost}/patient/createAccount?email={encryptedEmail}&requestId={request.Requestid}";
                var title = "Account Creation Link";
                var message = $"Please click <a href=\"{accountCreationLink}\">here</a> to create your account.";
                bool isSent = _patientFunctionRepository.SendEmail(formData.Email, title, message);
                string name = formData.FirstName + " , " + formData.LastName;
                EmailLog(formData.Email, message, title, name, 3, request.Requestid, null, null, 4, isSent, 1);
                transaction.Complete();

            }
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
