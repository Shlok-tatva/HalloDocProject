using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using System.Transactions;
using HalloDoc_BAL.ViewModel.Admin;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Mail;
using System.Net;

namespace HalloDoc_BAL.Repository
{
    public class AdminFunctionRepository : IAdminFunctionRepository
    {
        private readonly ApplicationDbContext _context;
        public IRequestRepository _requestRepository;
        public IRequestClientRepository _requestClientRepository;
        public IRequestNotesRepository _requestNotesRepository;

        public AdminFunctionRepository(ApplicationDbContext context, IRequestRepository requestRepository, IRequestClientRepository requestClientRepository, IRequestNotesRepository requestNotesRepository)
        {
            _context = context;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _requestNotesRepository = requestNotesRepository;
        }

        public AdminDashboardView GetAdminDashboardView()
        {
            AdminDashboardView view = new AdminDashboardView();
            view.regions = GetAllReagion();
            view.casetags = GetAllCaseTag();
            return view;
        }

        public IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId)
        {
            var statusIds = GetStatus(statusId);
            var statusIdWiseRequest = from r in _context.Requests.ToList()
                                      join rc in _context.Requestclients.ToList()
                                      on r.Requestid equals rc.Requestid into rrc
                                      from rc in rrc.DefaultIfEmpty()
                                      join p in _context.Physicians.ToList()
                                      on r.Physicianid equals p.Physicianid into rp
                                      from p in rp.DefaultIfEmpty()

                                      where statusIds.Contains(r.Status)
                                      select new RequestDataTableView
                                      {
                                          requestId = r.Requestid,
                                          PatientName = rc.Firstname + " " + rc.Lastname,
                                          PatientEmail = rc.Email,
                                          RequesterEmail = r.Email,
                                          DateOfBirth = rc.Intyear.Value.ToString("") + "-" + rc.Strmonth + "-" + string.Format("{0:00}", rc.Intdate.Value),
                                          PhysicianName = p != null ? p.Firstname + " " + p.Lastname : "",
                                          RequesterName = r.Firstname + " " + r.Lastname,
                                          RequestedDate = r.Createddate.ToString(),
                                          PatientPhoneNumber = rc.Phonenumber,
                                          Address = rc.Street + " " + rc.City + " " + rc.State + ",(" + rc.Zipcode + ")",
                                          RequesterPhoneNumber = r.Phonenumber,
                                          status = r.Status,
                                          MenuOptions = GetMenuOptionsForStatus(statusId),
                                          RequestTyepid = r.Requesttypeid
                                      };

            return statusIdWiseRequest;
        }

        private List<MenuOptionEnum> GetMenuOptionsForStatus(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return new List<MenuOptionEnum> { MenuOptionEnum.assignCase, MenuOptionEnum.cancelCase, MenuOptionEnum.viewCase, MenuOptionEnum.viewNotes, MenuOptionEnum.BlockPatient }; // Map to 'New' state
                case 2:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.Transfer, MenuOptionEnum.clearCase, MenuOptionEnum.sendAgreement }; // Map to 'Panding' state
                case 3:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.doctorsNote, MenuOptionEnum.Encounter }; // Map to 'Active' state
                case 4:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.doctorsNote, MenuOptionEnum.Encounter }; // Map to 'Conclude' state
                case 5:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.doctorsNote, MenuOptionEnum.clearCase, MenuOptionEnum.Encounter };
                case 6:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes };
                default:
                    return new List<MenuOptionEnum>(); // Default case
            }
        }


        public int[] GetStatus(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return new int[] { 1 };
                case 2:
                    return new int[] { 2 };
                case 3:
                    return new int[] { 4, 5 };
                case 4:
                    return new int[] { 6 };
                case 5:
                    return new int[] { 3, 7, 8 };
                case 6:
                    return new int[] { 9 };
                default:
                    return new int[] { };
            }
        }


        public ViewCaseView GetViewCase(int requestId)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requestId);
            var requestClient = _context.Requestclients.FirstOrDefault(r => r.Requestid == requestId);
            List<Region> listofRegion = _context.Regions.ToList();

            var view = new ViewCaseView
            {
                requestId = request.Requestid,
                firstName = requestClient.Firstname,
                symptom = requestClient.Notes,
                statusId = request.Status,
                lastName = requestClient.Lastname,
                dateofBirth = requestClient.Intyear.Value.ToString("") + "-" + requestClient.Strmonth + "-" + string.Format("{0:00}", requestClient.Intdate.Value),
                phoneNumber = requestClient.Phonenumber,
                Address = requestClient.Street + " " + requestClient.City + " " + requestClient.State + " " + requestClient.Zipcode,
                email = requestClient.Email,
                Region = requestClient.Regionid,
                requesterfirstName = request.Firstname,
                requesterlastName = request.Lastname,
                requesterEmail = request.Email,
                requesterPhoneNumber = request.Phonenumber,
                requesttypeId = request.Requesttypeid,
                ListofRegion = listofRegion,
            };
            return view;
        }

        public ViewNotesView GetViewNotesView(int requestId)
        {
            ViewNotesView view = new ViewNotesView();
            Requestnote note = _requestNotesRepository.Get(requestId);

            if (note == null)
            {
                note = new Requestnote();
                note.Requestid = requestId;
                note.Adminnotes = "-";
                note.Physiciannotes = "-";
            }
            view.requestId = requestId;
            view.adminNote = note.Adminnotes;
            view.physicianNote = note.Physiciannotes;


            List<Requeststatuslog> requeststatuslogs = _context.Requeststatuslogs.Where(r => r.Requestid == requestId).ToList();

            if (requeststatuslogs.Count > 0)
            {
                List<string> transferNotes = new List<string>();
                requeststatuslogs.Sort((a, b) => b.Requeststatuslogid - a.Requeststatuslogid);

                foreach (var item in requeststatuslogs)
                {
                    if (item.Status == 3 && item.Adminid != null)
                    {
                        view.adminCancelationNote = item.Notes;
                    }
                    else if (item.Status == 3 && item.Physicianid != null)
                    {
                        view.physicianCancelationNote = item.Notes;
                    }
                    else
                    {
                        if (item.Adminid != null && item.Physicianid == null)
                        {
                            transferNotes.Add("Admin Transfer to Patient on " + item.Createddate.ToLocalTime() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes);
                        }
                        else if (item.Adminid == null && item.Physicianid != null)
                        {
                            transferNotes.Add("Physician Transfer to Patient on " + item.Createddate.ToLocalTime() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes);
                        }

                    }
                }
                view.transferNotes = transferNotes;
            }
            else
            {
                view.transferNotes = null;
            }

            return view;

        }


        public List<Region> GetAllReagion()
        {
            return _context.Regions.ToList();
        }
        public List<Casetag> GetAllCaseTag()
        {
            return _context.Casetags.ToList();
        }

        public void blockRequst(int requestId, string reason, int adminId)
        {
            using (var transaction = new TransactionScope())
            {

                try
                {
                    Request request = _requestRepository.Get(requestId);
                    Requestclient rc = _requestClientRepository.Get(requestId);

                    request.Status = 10; // it is for Block the request
                    _requestRepository.Update(request);
                    Blockrequest blockrequest = new Blockrequest();
                    blockrequest.Requestid = requestId.ToString();
                    blockrequest.Reason = reason;
                    blockrequest.Phonenumber = rc.Phonenumber;
                    blockrequest.Email = rc.Email;
                    blockrequest.Createddate = DateTime.Now;
                    _context.Blockrequests.Add(blockrequest);
                    _context.SaveChanges();

                    Requeststatuslog requestlog = new Requeststatuslog();
                    requestlog.Requestid = requestId;
                    requestlog.Status = 10;
                    requestlog.Adminid = adminId;
                    requestlog.Notes = reason;
                    requestlog.Createddate = DateTime.Now;
                    requestlog.Transtoadmin = false;
                    _context.Requeststatuslogs.Add(requestlog);
                    _context.SaveChanges();

                    transaction.Complete();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void cancelCase(int requestId, int adminId, string reason, string note)
        {
            using (var transaction = new TransactionScope())
            {

                Request request = _requestRepository.Get(requestId);
                request.Status = 3;
                request.Casetag = reason;
                _requestRepository.Update(request);

                Requeststatuslog requestlog = new Requeststatuslog();
                requestlog.Requestid = requestId;
                requestlog.Status = 3;
                requestlog.Adminid = adminId;
                requestlog.Notes = reason + "(" + note + ")";
                requestlog.Createddate = DateTime.Now;
                requestlog.Transtoadmin = false;
                _context.Requeststatuslogs.Add(requestlog);
                _context.SaveChanges();

                transaction.Complete();
            }

        }

        public List<ViewUploadView> GetuploadedDocuments(int requestId)
        {
            var requestwisefiles = from requestFile in _context.Requestwisefiles
                                   where requestFile.Requestid == requestId
                                   select new ViewUploadView
                                   {
                                       Requestid = requestId,
                                       fileId = requestFile.Requestwisefileid,
                                       uploadDate = requestFile.Createddate,
                                       UploadImage = requestFile.Filename,
                                       fileName = Path.GetFileName(requestFile.Filename),
                                   };

            return requestwisefiles.ToList();
        }

        public void DeletefileFromDatabase(int fileId)
        {
            try
            {
                Requestwisefile file = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileId);

                if (file != null)
                {
                    _context.Requestwisefiles.Remove(file);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }
        }


        public void SendEmail(string toEmail, string Title, string Message, string[] attachmentFilePaths = null)
        {
            try
            {
                // Configure SMTP client
                using (SmtpClient smtpClient = new SmtpClient("mail.etatvasoft.com"))
                {
                    var subject = Title;
                    var emailBody = Message;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("shlok.jadeja@etatvasoft.com", "Shlok@#177");
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;

                    // Construct the email message
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("shlok.jadeja@etatvasoft.com");
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = subject;
                        mailMessage.Body = emailBody;
                        mailMessage.IsBodyHtml = true;

                        // Attach files
                        if (attachmentFilePaths != null && attachmentFilePaths.Length > 0)
                        {
                            foreach (string filePath in attachmentFilePaths)
                            {
                                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                                {
                                    Attachment attachment = new Attachment(filePath);
                                    mailMessage.Attachments.Add(attachment);
                                }
                            }
                        }

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

        public void CreateProvider(CreateProviderView model, int[] selectedRegions)
        {
            model.regionOfservice = selectedRegions;

            using (var transaction = new TransactionScope())
            {
                Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(u => u.Email == model.email);
                Physician physician = new Physician();
                List<Physicianregion> physicianRegions = new List<Physicianregion>();

                if (aspnetuser == null)
                {
                    aspnetuser = new Aspnetuser();
                    Guid guid = Guid.NewGuid();
                    string str = guid.ToString();
                    string username = "MD." + model.firstName.Substring(0, 3) + "." + model.lastName.Substring(0, 2);

                    aspnetuser.Id = str;
                    aspnetuser.Username = username;
                    aspnetuser.Email = model.email;
                    aspnetuser.Phonenumber = model.phoneNumber;
                    aspnetuser.Passwordhash = model.password;
                    aspnetuser.Createddate = DateTime.Now;
                    _context.Aspnetusers.Add(aspnetuser);
                    _context.SaveChanges();

                    physician.Email = model.email;
                    physician.Aspnetuserid = aspnetuser.Id;
                    physician.Firstname = model.firstName;
                    physician.Lastname = model.lastName;
                    physician.Mobile = model.phoneNumber;
                    physician.Regionid = model.regionId;
                    physician.Createdby = "fe51db26-1ba6-4880-a20a-36d10cece24e";
                    physician.Createddate = DateTime.Now;
                    physician.Status = 1;
                    physician.Businessname = model.businessName;
                    physician.Businesswebsite = model.businessWebsite;
                    _context.Physicians.Add(physician);
                    _context.SaveChanges();

                    foreach (int regionId in selectedRegions)
                    {
                        physicianRegions.Add(new Physicianregion
                        {
                            Regionid = regionId,
                            Physicianid = physician.Physicianid
                        });
                    }
                    _context.Physicianregions.AddRange(physicianRegions);
                    _context.SaveChanges();

                    transaction.Complete();

                }

            }
        }

        public List<Physician> GetPhysiciansByRegion(int regionId)
        {
            var physicians = (from pr in _context.Physicianregions
                              join p in _context.Physicians on pr.Physicianid equals p.Physicianid
                              where pr.Regionid == regionId
                              select p).ToList();
            return physicians;
        }
    }
}
