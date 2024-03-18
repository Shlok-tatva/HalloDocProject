using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using System.Transactions;
using HalloDoc_BAL.ViewModel.Admin;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Identity;

namespace HalloDoc_BAL.Repository
{
    public class AdminFunctionRepository : IAdminFunctionRepository
    {
        private readonly ApplicationDbContext _context;
        public IRequestRepository _requestRepository;
        public IRequestClientRepository _requestClientRepository;
        public IRequestNotesRepository _requestNotesRepository;
        public IAdminRepository _adminRepo;
        public ICommonFunctionRepository _commonFunctionrepo;

        public AdminFunctionRepository(ApplicationDbContext context, IRequestRepository requestRepository, IRequestClientRepository requestClientRepository, IRequestNotesRepository requestNotesRepository, ICommonFunctionRepository commonFunctionrepo, IAdminRepository adminRepo)
        {
            _context = context;
            _requestRepository = requestRepository;
            _requestClientRepository = requestClientRepository;
            _requestNotesRepository = requestNotesRepository;
            _commonFunctionrepo = commonFunctionrepo;
            _adminRepo = adminRepo;
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
                                      join rs in _context.Requeststatuslogs.ToList()
                                      on r.Requestid equals rs.Requestid into rrs
                                      from rs in rrs.OrderByDescending(x => x.Createddate).Take(1).DefaultIfEmpty()
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
                                          Notes = rs != null ? rs.Notes : "-", // Store result into notes
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
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.Encounter }; // Map to 'Active' state
                case 4:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.Encounter }; // Map to 'Conclude' state
                case 5:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.closeCase, MenuOptionEnum.clearCase, MenuOptionEnum.Encounter };
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
                    else if (item.Status == 4)
                    {
                        transferNotes.Add("On " + item.Createddate.ToLongDateString() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes);
                    }
                    else if (item.Status == 7)
                    {
                        view.patientCancelationNote = item.Notes;
                    }
                    else
                    {
                        if (item.Adminid != null && item.Physicianid == null)
                        {
                            transferNotes.Add("Admin Transfer to Patient on " + item.Createddate.ToLongDateString() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes); ;
                        }
                        else if (item.Adminid == null && item.Physicianid != null)
                        {
                            transferNotes.Add("Physician Transfer to Patient on " + item.Createddate.ToLongDateString() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes);
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
        public void assignCase(int requestId, int physicianId)
        {
            using (var transaction = new TransactionScope())
            {
                Request request = _requestRepository.Get(requestId);
                request.Physicianid = physicianId;
                Physician physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == physicianId);
                request.Status = 2;
                _requestRepository.Update(request);
                Requeststatuslog log = new Requeststatuslog();

                log.Requestid = requestId;
                log.Status = 2;
                log.Notes = "Request Assign to Physician " + physician.Firstname + " " + physician.Lastname;
                log.Createddate = DateTime.Now;
                log.Adminid = 4;
                log.Transtoadmin = false;
                _context.Requeststatuslogs.Add(log);
                _context.SaveChanges();

                transaction.Complete();

            }
        }

        public void transferCase(int requestId, int physicianId, int adminId, string note)
        {
            using (var transaction = new TransactionScope())
            {
                Request request = _requestRepository.Get(requestId);
                request.Physicianid = physicianId;
                Physician physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == physicianId);
                _requestRepository.Update(request);

                Requeststatuslog log = new Requeststatuslog();
                log.Requestid = requestId;
                log.Status = 2;
                log.Notes = "Request Re-Assign to Physician " + physician.Firstname + " " + physician.Lastname + "(" + note + ")";
                log.Createddate = DateTime.Now;
                log.Adminid = adminId;
                log.Transtoadmin = false;
                _context.Requeststatuslogs.Add(log);
                _context.SaveChanges();
                transaction.Complete();
            }
        }


        public void blockRequst(int requestId, string reason, int adminId)
        {
            using (var transaction = new TransactionScope())
            {

                try
                {
                    Request request = _requestRepository.Get(requestId);
                    Requestclient rc = _requestClientRepository.Get(requestId);

                    request.Status = 11; // it is for Block the request
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
                                   where requestFile.Requestid == requestId && requestFile.Isdeleted != true
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
                    file.Isdeleted = true;
                    _context.Requestwisefiles.Update(file);
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
                    physician.Createdby = "faeb647e-a0fe-4b31-a87d-4a2c9693242b";
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

        public void clearCase(int requestId, int adminId)
        {
            using (var transaction = new TransactionScope())
            {
                Request request = _requestRepository.Get(requestId);
                request.Status = 10;
                _requestRepository.Update(request);

                Requeststatuslog log = new Requeststatuslog();
                log.Requestid = requestId;
                log.Adminid = adminId;
                log.Status = 10; // clear the request
                log.Notes = "Reqest Cleared";
                log.Createddate = DateTime.Now;
                _context.Requeststatuslogs.Add(log);
                _context.SaveChanges();

                transaction.Complete();

            }
        }


        public void sendAgreement(int requestId, int adminId, string email, string link)
        {
            using (var transaction = new TransactionScope())
            {

                var title = "Accept the Agreement for your request";
                var message = $"Please click <a href=\"{link}\">here</a> to accept your agreement for request you created on HalloDoc.";
                SendEmail(email, title, message);
                Requeststatuslog log = new Requeststatuslog();
                log.Requestid = requestId;
                log.Createddate = DateTime.Now;
                log.Adminid = adminId;
                log.Status = 2;
                log.Notes = "Agreement sent to patient by Admin";
                _context.Requeststatuslogs.Add(log);
                _context.SaveChanges();
                transaction.Complete();

            }
        }

        public EncounterFormView GetEncounterFormView(int requestId)
        {
            EncounterFormView formView = new EncounterFormView();
            Requestclient requestclient = _requestClientRepository.Get(requestId);
            Encounterform encounterform = _context.Encounterforms.FirstOrDefault(r => r.Requestid == requestId);

            if (encounterform != null && encounterform.Isfinalize == true)
            {
                return null;
            }

            if (requestclient != null)
            {
                formView.requestId = requestId;
                formView.firstName = requestclient.Firstname;
                formView.lastName = requestclient.Lastname;
                formView.dateOfBirth = requestclient.Intyear.Value.ToString("") + "-" + requestclient.Strmonth + "-" + string.Format("{0:00}", requestclient.Intdate.Value);
                formView.dateOfRequest = String.Format("{0:yyyy-MM-dd}", _context.Requests.FirstOrDefault(r => r.Requestid == requestId).Createddate);
                formView.phone = requestclient.Phonenumber;
                formView.email = requestclient.Email;
                formView.location = requestclient.Street + " " + requestclient.City + "," + requestclient.State + ", (" + requestclient.Zipcode + ")";
                if (encounterform != null)
                {
                    formView.historyOfPresentIllnessOrInjury = encounterform.Historyofpresentillnessorinjury;
                    formView.medicalHistory = encounterform.Medicalhistory;
                    formView.medications = encounterform.Medications;
                    formView.allergies = encounterform.Allergies;
                    formView.temp = encounterform.Temp;
                    formView.hr = encounterform.Hr;
                    formView.rr = encounterform.Rr;
                    formView.bloodPressureDiastolic = encounterform.Bloodpressurediastolic;
                    formView.bloodPressureSystolic = encounterform.Bloodpressurediastolic;
                    formView.o2 = encounterform.O2;
                    formView.pain = encounterform.Pain;
                    formView.heent = encounterform.Heent;
                    formView.pain = encounterform.Pain;
                    formView.cv = encounterform.Cv;
                    formView.chest = encounterform.Chest;
                    formView.abd = encounterform.Abd;
                    formView.extremities = encounterform.Extremities;
                    formView.skin = encounterform.Skin;
                    formView.neuro = encounterform.Neuro;
                    formView.other = encounterform.Other;
                    formView.diagnosis = encounterform.Diagnosis;
                    formView.treatmentPlan = encounterform.TreatmentPlan;
                    formView.followup = encounterform.Followup;
                    formView.medicalDispensed = encounterform.Medicaldispensed;
                    formView.procedures = encounterform.Procedures;
                    formView.adminId = encounterform.Adminid;
                    formView.isFinalize = encounterform.Isfinalize == false ? 0 : 1;
                }

            }

            return formView;
        }


        public EncounterFormView GetEncounterForm(int requestId)
        {
            EncounterFormView formView = new EncounterFormView();
            Requestclient requestclient = _requestClientRepository.Get(requestId);
            Encounterform encounterform = _context.Encounterforms.FirstOrDefault(r => r.Requestid == requestId);

            if (requestclient != null)
            {
                formView.requestId = requestId;
                formView.firstName = requestclient.Firstname;
                formView.lastName = requestclient.Lastname;
                formView.dateOfBirth = requestclient.Intyear.Value.ToString("") + "-" + requestclient.Strmonth + "-" + string.Format("{0:00}", requestclient.Intdate.Value);
                formView.dateOfRequest = String.Format("{0:yyyy-MM-dd}", _context.Requests.FirstOrDefault(r => r.Requestid == requestId).Createddate);
                formView.phone = requestclient.Phonenumber;
                formView.email = requestclient.Email;
                formView.location = requestclient.Street + " " + requestclient.City + "," + requestclient.State + ", (" + requestclient.Zipcode + ")";
                if (encounterform != null)
                {
                    formView.historyOfPresentIllnessOrInjury = encounterform.Historyofpresentillnessorinjury;
                    formView.medicalHistory = encounterform.Medicalhistory;
                    formView.medications = encounterform.Medications;
                    formView.allergies = encounterform.Allergies;
                    formView.temp = encounterform.Temp;
                    formView.hr = encounterform.Hr;
                    formView.rr = encounterform.Rr;
                    formView.bloodPressureDiastolic = encounterform.Bloodpressurediastolic;
                    formView.bloodPressureSystolic = encounterform.Bloodpressurediastolic;
                    formView.o2 = encounterform.O2;
                    formView.pain = encounterform.Pain;
                    formView.heent = encounterform.Heent;
                    formView.pain = encounterform.Pain;
                    formView.cv = encounterform.Cv;
                    formView.chest = encounterform.Chest;
                    formView.abd = encounterform.Abd;
                    formView.extremities = encounterform.Extremities;
                    formView.skin = encounterform.Skin;
                    formView.neuro = encounterform.Neuro;
                    formView.other = encounterform.Other;
                    formView.diagnosis = encounterform.Diagnosis;
                    formView.treatmentPlan = encounterform.TreatmentPlan;
                    formView.followup = encounterform.Followup;
                    formView.medicalDispensed = encounterform.Medicaldispensed;
                    formView.procedures = encounterform.Procedures;
                    formView.adminId = encounterform.Adminid;
                    formView.isFinalize = encounterform.Isfinalize == false ? 0 : 1;
                }

            }

            return formView;
        }

        public void SubmitEncounterForm(EncounterFormView formView)
        {
            if (formView != null)
            {
                using (var transaction = new TransactionScope())
                {
                    Encounterform newform = _context.Encounterforms.FirstOrDefault(f => f.Requestid == formView.requestId);

                    if (newform == null)
                    {
                        newform = new Encounterform();
                    }

                    newform.Requestid = formView.requestId;
                    newform.Historyofpresentillnessorinjury = formView.historyOfPresentIllnessOrInjury;
                    newform.Medicalhistory = formView.medicalHistory;
                    newform.Medications = formView.medications;
                    newform.Allergies = formView.allergies;
                    newform.Temp = formView.temp;
                    newform.Hr = formView.hr;
                    newform.Rr = formView.rr;
                    newform.Bloodpressurediastolic = formView.bloodPressureDiastolic;
                    newform.Bloodpressuresystolic = formView.bloodPressureSystolic;
                    newform.O2 = formView.o2;
                    newform.Pain = formView.pain;
                    newform.Heent = formView.heent;
                    newform.Pain = formView.pain;
                    newform.Cv = formView.cv;
                    newform.Chest = formView.chest;
                    newform.Abd = formView.abd;
                    newform.Extremities = formView.extremities;
                    newform.Skin = formView.skin;
                    newform.Neuro = formView.neuro;
                    newform.Other = formView.other;
                    newform.Diagnosis = formView.diagnosis;
                    newform.TreatmentPlan = formView.treatmentPlan;
                    newform.Followup = formView.followup;
                    newform.Medicaldispensed = formView.medicalDispensed;
                    newform.Procedures = formView.procedures;
                    newform.Adminid = formView.adminId;
                    newform.Isfinalize = formView.isFinalize == 0 ? false : true;

                    if (newform == null)
                    {
                        _context.Encounterforms.Add(newform);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Encounterforms.Update(newform);
                        _context.SaveChanges();
                    }
                    transaction.Complete();
                }
            }
        }

        public int getEcounterFormStatus(int requestId)
        {
            var form = _context.Encounterforms.FirstOrDefault(f => f.Requestid == requestId);
            if (form == null)
            {
                return 0;
            }
            else
            {
                bool status = _context.Encounterforms.FirstOrDefault(f => f.Requestid == requestId).Isfinalize;
                return status ? 1 : 0;
            }
        }

        public List<Healthprofessionaltype> getAllProfessions()
        {
            return _context.Healthprofessionaltypes.ToList();
        }

        public List<Healthprofessional> GetBusinessesByProfession(int professionId)
        {
            return _context.Healthprofessionals.Where(h => h.Profession == professionId).ToList();
        }

        public Healthprofessional GetBusinessDetailsById(int Vendorid)
        {
            return _context.Healthprofessionals.FirstOrDefault(h => h.Vendorid == Vendorid);
        }

        public List<Physicianlocation> GetPhysicianlocations()
        {
            return _context.Physicianlocations.ToList();
        }

        public void AddOrder(Orderdetail orderdetail)
        {
            try
            {
                _context.Orderdetails.Add(orderdetail);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public AdminProfileView GetAdminProfileView(int adminId)
        {
            if (adminId != null)
            {
                try
                {
                    AdminProfileView view = new AdminProfileView();
                    Admin admin = _adminRepo.GetAdminById(adminId);
                    view.adminId = adminId;
                    view.FirstName = admin.Firstname;
                    view.LastName = admin.Lastname;
                    view.Email = admin.Email;
                    view.ConfirmEmail = admin.Email;
                    view.Phone = admin.Mobile;
                    view.Address1 = admin.Address1;
                    view.Address2 = admin.Address2;
                    view.City = admin.City;
                    view.Zip = admin.Zip;
                    view.StateId = admin.Regionid;
                    view.billingPhone = admin.Altphone;
                    view.Status = admin.Status;
                    string role = _context.Roles.FirstOrDefault(r => r.Roleid == admin.Roleid).Name;
                    view.role = role;
                    view.UserName = _context.Aspnetusers.FirstOrDefault(u => u.Id == admin.Aspnetuserid).Username;
                    view.AdminRegions = (from ar in _context.Adminregions.ToList()
                                         where ar.Adminid == adminId
                                         select (int)ar.Regionid).ToList();

                    return view;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }


        }

        public void UpdateAdminData(AdminProfileView data)
        {
            Admin admin = _adminRepo.GetAdminById(data.adminId);
            if (admin != null)
            {
                admin.Firstname = data.FirstName;
                admin.Lastname = data.LastName;
                admin.Email = data.Email;
                admin.Mobile = data.Phone;
                admin.City = data.City;
                admin.Address1 = data.Address1;
                admin.Address2 = data.Address2;
                admin.Regionid = data.StateId;
                admin.Zip = data.Zip;
                admin.Altphone = data.billingPhone;
                _adminRepo.updateAdmin(admin);
            }
        }

        public void ChagePassword(int adminId, string password)
        {
            Admin admin = _adminRepo.GetAdminById(adminId);
            Aspnetuser user = _context.Aspnetusers.FirstOrDefault(u => u.Id == admin.Aspnetuserid);
            if (user != null)
            {
                var hasher = new PasswordHasher<string>();
                string hashedPassword = hasher.HashPassword(null, password);
                user.Passwordhash = hashedPassword;
                _context.Aspnetusers.Update(user);
                _context.SaveChanges();
            }
        }

        public List<ProviderInfoAdmin> getProviderInfoView()
        {
            List<ProviderInfoAdmin> providerInfoAdmin = new List<ProviderInfoAdmin>();
            List<Physician> providers = _context.Physicians.ToList();
            providers.Sort((a, b) => b.Physicianid - a.Physicianid);
            foreach (Physician pro in providers)
            {
                ProviderInfoAdmin provider = new ProviderInfoAdmin();
                provider.providerId = pro.Physicianid;
                provider.providerName = pro.Firstname + " " + pro.Lastname;
                provider.providerEmail = pro.Email;
                provider.providerPhone = pro.Mobile;
                provider.providerStatus = pro.Status;
                provider.providerRole = pro.Roleid.ToString();
                provider.stopNotification = providerNotificationStatus(pro.Physicianid);
                providerInfoAdmin.Add(provider);
            }

            return providerInfoAdmin;
        }

        public bool providerNotificationStatus(int providerId)
        {
            return _context.Physiciannotifications.FirstOrDefault(p => p.Physicianid == providerId).Isnotificationstopped;
        }

        public void updateNotificationStatus(int providerId, bool status)
        {
            using (var transaction = new TransactionScope())
            {

                Physiciannotification physiciannotification = _context.Physiciannotifications.FirstOrDefault(p => p.Physicianid == providerId);
                if (physiciannotification != null)
                {
                    physiciannotification.Isnotificationstopped = status;
                    _context.Physiciannotifications.Update(physiciannotification);
                    _context.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public List<Healthprofessional> getAllVendors()
        {
            return _context.Healthprofessionals.Where(vendor => vendor.Isdeleted == false).ToList();
        }
        public string GetProfessionNameById(int id)
        {
            return _context.Healthprofessionaltypes.FirstOrDefault(pt => pt.Healthprofessionalid == id).Professionname;
        }


        public List<Region> GetAllReagion()
        {
            return _context.Regions.ToList();
        }
        public List<Casetag> GetAllCaseTag()
        {
            return _context.Casetags.ToList();
        }
    }
}
