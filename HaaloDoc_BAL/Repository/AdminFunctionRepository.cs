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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections;
using HalloDoc_BAL.ViewModel.Schedule;
using HalloDoc_BAL.ViewModel.Records;
using System.Collections.Generic;
using System.Xml.Linq;

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

        public DashboardView GetDashboardView()
        {
            DashboardView view = new DashboardView();
            view.regions = GetAllReagion();
            view.casetags = GetAllCaseTag();
            return view;
        }

        public IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId, int? physicianId)
        {
            var statusIds = GetStatus(statusId);
            var statusIdWiseRequest = Enumerable.Empty<RequestDataTableView>();
            if (physicianId == null)
            {
                statusIdWiseRequest = from r in _context.Requests.ToList()
                                      join rc in _context.Requestclients.ToList()
                                      on r.Requestid equals rc.Requestid into rrc
                                      from rc in rrc.DefaultIfEmpty()
                                      join p in _context.Physicians.ToList()
                                      on r.Physicianid equals p.Physicianid into rp
                                      from p in rp.DefaultIfEmpty()
                                      join rs in _context.Requeststatuslogs.ToList()
                                      on r.Requestid equals rs.Requestid into rrs
                                      from rs in rrs.OrderByDescending(x => x.Createddate).Take(1).DefaultIfEmpty()
                                      where statusIds.Contains(r.Status) && (physicianId == null || r.Physicianid == physicianId)
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
                                          RequestTyepid = r.Requesttypeid,
                                          regionId = rc.Regionid,
                                          isPhysicianDashboard = false
                                      };
            }
            else
            {
                statusIdWiseRequest = from r in _context.Requests.ToList()
                                      join rc in _context.Requestclients.ToList()
                                      on r.Requestid equals rc.Requestid into rrc
                                      from rc in rrc.DefaultIfEmpty()
                                      join p in _context.Physicians.ToList()
                                      on r.Physicianid equals p.Physicianid into rp
                                      from p in rp.DefaultIfEmpty()
                                      join rs in _context.Requeststatuslogs.ToList()
                                      on r.Requestid equals rs.Requestid into rrs
                                      from rs in rrs.OrderByDescending(x => x.Createddate).Take(1).DefaultIfEmpty()
                                      where statusIds.Contains(r.Status) && (physicianId == null || r.Physicianid == physicianId)
                                      select new RequestDataTableView
                                      {
                                          requestId = r.Requestid,
                                          PatientName = rc.Firstname + " " + rc.Lastname,
                                          PatientEmail = rc.Email,
                                          RequesterEmail = r.Email,
                                          DateOfBirth = rc.Intyear.Value.ToString("") + "-" + rc.Strmonth + "-" + string.Format("{0:00}", rc.Intdate.Value),
                                          PatientPhoneNumber = rc.Phonenumber,
                                          Address = rc.Street + " " + rc.City + " " + rc.State + ",(" + rc.Zipcode + ")",
                                          RequesterPhoneNumber = r.Phonenumber,
                                          status = r.Status,
                                          MenuOptions = GetMenuOptionsForStatusForPhysician(statusId),
                                          RequestTyepid = r.Requesttypeid,
                                          regionId = rc.Regionid,
                                          callStatus = r.Calltype,
                                          isPhysicianDashboard = true
                                      };
            }


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

        private List<MenuOptionEnum> GetMenuOptionsForStatusForPhysician(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return new List<MenuOptionEnum> { MenuOptionEnum.Accept, MenuOptionEnum.viewCase, MenuOptionEnum.viewNotes }; // Map to 'New' state
                case 2:
                    return new List<MenuOptionEnum> { MenuOptionEnum.sendAgreement, MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.Transfer }; // Map to 'Panding' state
                case 3:
                    return new List<MenuOptionEnum> { MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.orders, MenuOptionEnum.Encounter }; // Map to 'Active' state
                case 4:
                    return new List<MenuOptionEnum> { MenuOptionEnum.ConcludeCare, MenuOptionEnum.viewCase, MenuOptionEnum.viewUpload, MenuOptionEnum.viewNotes, MenuOptionEnum.Encounter }; // Map to 'Conclude' state
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
                        else if (item.Adminid == null && item.Physicianid != null && item.Transtoadmin == false)
                        {
                            transferNotes.Add("Physician Transfer to Patient on " + item.Createddate.ToLongDateString() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes);
                        }
                        else if (item.Adminid == null && item.Physicianid != null && item.Transtoadmin == true)
                        {
                            transferNotes.Add("Physician Transfer to Admin on " + item.Createddate.ToLongDateString() + " at " + item.Createddate.ToString("h:mm:ss tt") + " :- " + item.Notes);

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

        public void UpdateNotes(int requestId, int? adminId, string? adminNotes, int? providerId, string? providerNotes)
        {
            Requestnote requestnote = _requestNotesRepository.Get(requestId);
            if (requestnote == null)
            {
                Requestnote note = new Requestnote();
                note.Requestid = requestId;
                if (adminId != null && adminNotes != null)
                {
                    note.Adminnotes = adminNotes;
                    note.Createdby = _adminRepo.GetAdminById((int)adminId).Aspnetuserid;
                }
                if (providerId != null && providerNotes != null)
                {
                    note.Physiciannotes = providerNotes;
                    note.Createdby = _context.Physicians.FirstOrDefault(ph => ph.Physicianid == (int)providerId).Aspnetuserid;
                }
                note.Createddate = DateTime.Now;
                _requestNotesRepository.Add(note);
            }
            else
            {
                if (adminId != null && adminNotes != null)
                {
                    requestnote.Adminnotes = adminNotes;
                    requestnote.Modifiedby = _adminRepo.GetAdminById((int)adminId).Aspnetuserid;
                }
                if (providerId != null && providerNotes != null)
                {
                    requestnote.Physiciannotes = providerNotes;
                }
                requestnote.Modifieddate = DateTime.Now;
                _requestNotesRepository.Update(requestnote);
            }
        }

        public void assignCase(int requestId, int physicianId, int adminId)
        {
            using (var transaction = new TransactionScope())
            {
                Request request = _requestRepository.Get(requestId);
                request.Physicianid = physicianId;
                Physician physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == physicianId);
                request.Status = 1;
                _requestRepository.Update(request);
                Requeststatuslog log = new Requeststatuslog();

                log.Requestid = requestId;
                log.Status = 1;
                log.Notes = "Request Assign to Physician " + physician.Firstname + " " + physician.Lastname;
                log.Createddate = DateTime.Now;
                log.Adminid = adminId;
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
                    blockrequest = _context.Blockrequests.Where(br => br.Email == rc.Email).FirstOrDefault();

                    if (blockrequest == null)
                    {
                        blockrequest.Requestid = requestId.ToString();
                        blockrequest.Reason = reason;
                        blockrequest.Phonenumber = rc.Phonenumber;
                        blockrequest.Email = rc.Email;
                        blockrequest.Isactive = true;
                        blockrequest.Createddate = DateTime.Now;
                        _context.Blockrequests.Add(blockrequest);
                    }
                    else
                    {
                        blockrequest.Isactive = true;
                        _context.Blockrequests.Update(blockrequest);
                    }
                    _context.SaveChanges();

                    Requeststatuslog requestlog = new Requeststatuslog();
                    requestlog.Requestid = requestId;
                    requestlog.Status = 11;
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

        public bool SendEmail(string toEmail, string Title, string Message, string[] attachmentFilePaths = null)
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
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
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

        public void sendAgreement(int requestId, int? adminId, int? providerId, string email, string link)
        {
            using (var transaction = new TransactionScope())
            {

                var title = "Accept the Agreement for your request";
                var message = $"Please click <a href=\"{link}\">here</a> to accept your agreement for request you created on HalloDoc.";
                bool isSent = SendEmail(email, title, message);
                Request rq = _requestRepository.Get(requestId);
                string name = rq.Firstname + " " + rq.Lastname;
                _commonFunctionrepo.EmailLog(email, message, "Sent Agreement to Patient", name, 3, requestId, adminId, providerId, 5, isSent, 1);

                Requeststatuslog log = new Requeststatuslog();
                log.Requestid = requestId;
                log.Createddate = DateTime.Now;
                log.Adminid = adminId;
                log.Physicianid = providerId;
                log.Status = 2;
                if (adminId != null)
                {
                    Admin ad = _adminRepo.GetAdminById((int)adminId);
                    name = ad.Firstname + " " + ad.Lastname;
                    log.Notes = "Agreement sent to patient by Admin ( " + name + " )";
                }
                if (providerId != null)
                {
                    Physician ph = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerId);
                    name = ph.Firstname + " " + ph.Lastname;
                    log.Notes = "Agreement sent to patient by Physician ( " + name + " )";
                }

                _context.Requeststatuslogs.Add(log);
                _context.SaveChanges();
                transaction.Complete();

            }
        }


        #region Encounter-Form
        public EncounterFormView GetEncounterFormView(int requestId)
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

        public int? getrequestCallType(int requestId)
        {
            Request request = _requestRepository.Get(requestId);
            return request.Calltype;
        }

        #endregion

        #region Partner-Page

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
        #endregion

        #region AdminProfile-page
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

        public void ChagePassword(int adminId, int providerId, string password)
        {
            Aspnetuser user = new Aspnetuser();

            if (adminId != 0)
            {
                Admin admin = _adminRepo.GetAdminById(adminId);
                user = _context.Aspnetusers.FirstOrDefault(u => u.Id == admin.Aspnetuserid);
            }
            if (providerId != 0)
            {
                Physician physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerId);
                user = _context.Aspnetusers.FirstOrDefault(u => u.Id == physician.Aspnetuserid);

            }
            if (user != null)
            {
                var hasher = new PasswordHasher<string>();
                string hashedPassword = hasher.HashPassword(null, password);
                user.Passwordhash = hashedPassword;
                _context.Aspnetusers.Update(user);
                _context.SaveChanges();
            }
        }
        #endregion

        #region Providers-Page
        public List<ProviderInfoAdmin> getProviderInfoView(int? regionId)
        {
            List<ProviderInfoAdmin> providerInfoAdmin = new List<ProviderInfoAdmin>();
            List<Physician> providers = _context.Physicians.Where(p => p.Isdeleted == false && (regionId == 0 || regionId == null || regionId == p.Regionid)).ToList();
            providers.Sort((a, b) => b.Physicianid - a.Physicianid);
            foreach (Physician pro in providers)
            {
                ProviderInfoAdmin provider = new ProviderInfoAdmin();
                provider.providerId = pro.Physicianid;
                provider.providerName = pro.Firstname + " " + pro.Lastname;
                provider.providerEmail = pro.Email;
                provider.providerPhone = pro.Mobile;
                provider.providerStatus = pro.Status;

                provider.providerRole = _context.Roles.FirstOrDefault(r => r.Roleid == pro.Roleid).Name;
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

        public void CreateOrUpdateProvider(CreateProviderView model, int[] selectedRegions, bool isEditing)
        {
            model.regionOfservice = selectedRegions;

            using (var transaction = new TransactionScope())
            {
                Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(u => u.Email == model.email);
                Physician physician = new Physician();
                List<Physicianregion> physicianRegions = new List<Physicianregion>();

                if (!isEditing || aspnetuser == null)
                {
                    if (aspnetuser == null)
                    {
                        aspnetuser = new Aspnetuser();
                        Guid guid = Guid.NewGuid();
                        string str = guid.ToString();
                        string username = "MD." + model.firstName.Substring(0, 3) + "." + model.lastName.Substring(0, 2);

                        aspnetuser.Id = str;
                        aspnetuser.Username = username;
                        aspnetuser.Createddate = DateTime.Now;
                        _context.Aspnetusers.Add(aspnetuser);
                    }

                    aspnetuser.Email = model.email;
                    aspnetuser.Roleid = 2;
                    aspnetuser.Phonenumber = model.phoneNumber;
                    aspnetuser.Passwordhash = model.password;

                    _context.SaveChanges();

                    physician.Email = model.email;
                    physician.Aspnetuserid = aspnetuser.Id;
                    physician.Createdby = model.Createdby;
                    physician.Createddate = DateTime.Now;
                    physician.Status = 1;
                }
                else
                {
                    physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == model.ProviderId);
                    if (physician == null)
                    {
                        // Handle the case where the physician to edit is not found
                        // You can throw an exception or handle it based on your requirements
                        return;
                    }
                    physician.Modifieddate = DateTime.Now;
                    if (model.Modifiedby != null)
                    {
                        physician.Modifiedby = model.Modifiedby;
                    }

                    if (model.Status != null)
                    {
                        physician.Status = model.Status;
                    }

                }

                physician.Firstname = model.firstName;
                physician.Lastname = model.lastName;
                physician.Mobile = model.phoneNumber;

                if (model.roleid != null)
                {
                    physician.Roleid = model.roleid;
                }

                physician.Regionid = model.regionId;
                physician.Businessname = model.businessName;
                physician.Businesswebsite = model.businessWebsite;
                physician.Address1 = model.Address1;
                physician.Address2 = model.Address2;
                physician.City = model.city;
                physician.Zip = model.Zip;
                physician.Syncemailaddress = model.Syncemailaddress;
                physician.Adminnotes = model.Adminnotes;
                physician.Isagreementdoc = model.isAggrementDoc;
                physician.Isbackgrounddoc = model.isbackgroundDoc;
                physician.Islicensedoc = model.islicensedoc;
                physician.Istrainingdoc = model.istrainginDoc;
                physician.Isnondisclosuredoc = model.isnondisclosuredoc;
                physician.Isdeleted = false;
                physician.Npinumber = model.NPInumber;
                physician.Medicallicense = model.medicalLicence;
                physician.Altphone = model.Altphone;

                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    physician.Photo = model.PhotoFile.FileName;
                }
                if (model.SignatureFile != null && model.SignatureFile.Length > 0)
                {
                    physician.Signature = model.SignatureFile.FileName;
                }

                if (!isEditing)
                {
                    _context.Physicians.Add(physician);
                }
                else
                {
                    _context.Physicians.Update(physician);
                }

                _context.SaveChanges();

                string physicianFolder = Path.Combine("wwwroot\\", "Upload", "physician", physician.Physicianid.ToString());

                // Create the physician folder if it doesn't exist
                if (!Directory.Exists(physicianFolder))
                {
                    Directory.CreateDirectory(physicianFolder);
                }

                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    UploadFile(model.PhotoFile, physicianFolder, "photo");
                }
                if (model.SignatureFile != null && model.SignatureFile.Length > 0)
                {
                    UploadFile(model.SignatureFile, physicianFolder, "Signature");
                }
                if (model.isAggrementDoc && model.Agreementdoc != null && model.Agreementdoc.Length > 0)
                {
                    UploadFile(model.Agreementdoc, physicianFolder, "Aggrementdoc");
                }
                if (model.islicensedoc && model.Licensedoc != null && model.Licensedoc.Length > 0)
                {
                    UploadFile(model.Licensedoc, physicianFolder, "licensedoc");
                }
                if (model.isbackgroundDoc && model.BackGrounddoc != null && model.BackGrounddoc.Length > 0)
                {
                    UploadFile(model.BackGrounddoc, physicianFolder, "backgrounddoc");
                }
                if (model.istrainginDoc && model.Trainingdoc != null && model.Trainingdoc.Length > 0)
                {
                    UploadFile(model.Trainingdoc, physicianFolder, "trainginDoc");
                }
                if (model.isnondisclosuredoc && model.NonDisclosuredoc != null && model.NonDisclosuredoc.Length > 0)
                {
                    UploadFile(model.NonDisclosuredoc, physicianFolder, "nondisclosuredoc");
                }


                if (!isEditing)
                {
                    // If it's a new provider, add regions directly
                    foreach (int regionId in selectedRegions)
                    {
                        physicianRegions.Add(new Physicianregion
                        {
                            Regionid = regionId,
                            Physicianid = physician.Physicianid
                        });
                    }
                    _context.Physicianregions.AddRange(physicianRegions);
                    Physiciannotification notification = new Physiciannotification();
                    notification.Physicianid = physician.Physicianid;
                    notification.Isnotificationstopped = false;
                    _context.Physiciannotifications.Add(notification);
                }
                else
                {
                    // If it's an edit operation, remove existing regions and add the new ones
                    var existingRegions = _context.Physicianregions.Where(pr => pr.Physicianid == physician.Physicianid).ToList();
                    _context.Physicianregions.RemoveRange(existingRegions);

                    // Add new regions
                    foreach (int regionId in selectedRegions)
                    {
                        physicianRegions.Add(new Physicianregion
                        {
                            Regionid = regionId,
                            Physicianid = physician.Physicianid
                        });
                    }
                    _context.Physicianregions.AddRange(physicianRegions);
                }

                _context.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteProvider(int adminId, int providerId)
        {
            Admin admin = _adminRepo.GetAdminById(adminId);
            Physician pro = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerId);
            pro.Isdeleted = true;
            pro.Modifiedby = admin.Aspnetuserid;
            pro.Modifieddate = DateTime.Now;
            _context.Physicians.Update(pro);
            _context.SaveChanges();
        }

        private void UploadFile(IFormFile file, string folderPath, string fileName)
        {
            string filePath = Path.Combine(folderPath, fileName + Path.GetExtension(file.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        public CreateProviderView getProviderView(int providerId)
        {
            CreateProviderView view = new CreateProviderView();
            Physician phy = _context.Physicians.FirstOrDefault(ph => ph.Physicianid == providerId);

            if (phy != null)
            {
                Aspnetuser user = _context.Aspnetusers.FirstOrDefault(user => user.Id == phy.Aspnetuserid);

                view.UserName = user.Username;
                view.ProviderId = phy.Physicianid;
                view.Status = phy.Status;
                view.allRoles = GetAllRole();
                view.roleid = phy.Roleid;
                view.firstName = phy.Firstname;
                view.lastName = phy.Lastname;
                view.email = phy.Email;
                view.phoneNumber = phy.Mobile;
                view.medicalLicence = phy.Medicallicense;
                view.NPInumber = phy.Npinumber;
                view.Syncemailaddress = phy.Syncemailaddress;
                view.Address1 = phy.Address1;
                view.Address2 = phy.Address2;
                view.Altphone = phy.Altphone;
                view.city = phy.City;
                view.regionId = (int)phy.Regionid;
                view.Zip = phy.Zip;
                view.businessName = phy.Businessname;
                view.businessWebsite = phy.Businesswebsite;
                view.Adminnotes = phy.Adminnotes;
                view.isAggrementDoc = (bool)phy.Isagreementdoc;
                view.isbackgroundDoc = (bool)phy.Isbackgrounddoc;
                view.islicensedoc = (bool)phy.Islicensedoc;
                view.isnondisclosuredoc = (bool)phy.Isnondisclosuredoc;
                view.istrainginDoc = (bool)phy.Istrainingdoc;
                view.photo = phy.Photo;
                view.signature = phy.Signature;
                List<Physicianregion> regions = _context.Physicianregions.Where(ph => ph.Physicianid == phy.Physicianid).ToList();
                int[] regionIds = regions.Select(pr => pr.Regionid).ToArray();
                view.regionOfservice = regionIds;
            }

            return view;
        }


        #endregion

        #region UserAccess-And-AccountAccess
        public void CreateOrUpdateRole(int adminId, string roleName, int accountType, int[] selectedMenu, int? roleId = null)
        {
            using (var transaction = new TransactionScope())
            {
                Admin admin = _adminRepo.GetAdminById(adminId);

                if (roleId.HasValue)
                {
                    // Update existing role
                    var existingRole = _context.Roles.Include(r => r.Rolemenus).FirstOrDefault(r => r.Roleid == roleId.Value);
                    if (existingRole != null)
                    {
                        existingRole.Name = roleName;
                        existingRole.Accounttype = (short?)accountType;
                        existingRole.Modifieddate = DateTime.Now;
                        existingRole.Modifiedby = admin.Aspnetuserid;

                        // Delete existing menu associations
                        _context.Rolemenus.RemoveRange(existingRole.Rolemenus);

                        // Add new menu associations
                        foreach (var menuId in selectedMenu)
                        {
                            Rolemenu roleMenu = new Rolemenu();
                            roleMenu.Roleid = roleId.Value;
                            roleMenu.Menuid = menuId;
                            _context.Rolemenus.Add(roleMenu);
                        }
                    }
                }
                else
                {
                    // Create new role
                    Role newRole = new Role();
                    newRole.Name = roleName;
                    newRole.Accounttype = (short?)accountType;
                    newRole.Isdeleted = false;
                    newRole.Createddate = DateTime.Now;
                    newRole.Createdby = admin.Aspnetuserid;
                    _context.Roles.Add(newRole);
                    _context.SaveChanges();


                    // Add the selected menus to the Rolemenus table
                    foreach (var menuId in selectedMenu)
                    {
                        Rolemenu roleMenu = new Rolemenu();
                        roleMenu.Roleid = newRole.Roleid;
                        roleMenu.Menuid = menuId;
                        _context.Rolemenus.Add(roleMenu);
                    }
                }

                _context.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteRole(int adminId, int roleId)
        {
            Admin admin = _context.Admins.FirstOrDefault(a => a.Adminid == adminId);
            Role role = _context.Roles.FirstOrDefault(r => r.Roleid == roleId);
            role.Isdeleted = true;
            role.Modifieddate = DateTime.Now;
            role.Modifiedby = admin.Aspnetuserid;
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public List<UserAccessView> GetUserAccessView(int roleId)
        {
            IQueryable<Aspnetuser> usersQuery = _context.Aspnetusers;

            if (roleId == 1 || roleId == 2)
            {
                usersQuery = usersQuery.Where(user => user.Roleid == roleId);
            }
            else if (roleId != 0)
            {
                return new List<UserAccessView>(); // If an invalid roleId is passed, return empty list
            }

            List<Aspnetuser> allUser = usersQuery.Where(u => u.Roleid == 1 || u.Roleid == 2).ToList();
            List<UserAccessView> viewlist = new List<UserAccessView>();

            foreach (var item in allUser)
            {
                UserAccessView view = new UserAccessView();
                view.aspnetUserId = item.Id;
                view.phoneNumber = item.Phonenumber;
                view.accountTypeId = (int)item.Roleid;
                view.accountType = _context.Aspnetroles.Where(type => type.Id == item.Roleid).FirstOrDefault().Name;

                if (item.Roleid == 1)
                {
                    Admin admin = _adminRepo.GetAdmin(item.Id);
                    view.accountPOC = admin.Firstname + " " + admin.Lastname;
                    view.adminId = admin.Adminid;
                    view.statusId = (int)admin.Status;
                    view.openRequest = _requestRepository.GetAll().Count();
                }
                else if (item.Roleid == 2)
                {
                    Physician phy = _context.Physicians.FirstOrDefault(p => p.Aspnetuserid == item.Id);
                    view.accountPOC = phy.Firstname + " " + phy.Lastname;
                    view.physicianId = phy.Physicianid;
                    view.statusId = (int)phy.Status;
                    view.openRequest = _requestRepository.GetAll().Where(r => r.Physicianid == phy.Physicianid).Count();
                }
                viewlist.Add(view);
            }

            return viewlist;
        }

        public bool IsUsernameAvailable(string username)
        {
            try
            {
                bool userExists = _context.Aspnetusers.Any(user => user.Username == username);
                return !userExists;
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                return false;
            }
        }

        public void createAdmin(CreateAdminView data, int[] regions, int adminId)
        {
            using (var transaction = new TransactionScope())
            {
                Aspnetuser user = new Aspnetuser();
                Guid guid = Guid.NewGuid();
                string str = guid.ToString();
                var hasher = new PasswordHasher<string>();
                string hashedPassword = hasher.HashPassword(null, data.Password);

                user.Id = str;
                user.Username = data.UserName;
                user.Createddate = DateTime.Now;
                user.Phonenumber = data.PhoneNumber;
                user.Email = data.Email;
                user.Passwordhash = hashedPassword;
                user.Roleid = 1;
                _context.Aspnetusers.Add(user);
                _context.SaveChanges();

                Admin admin = new Admin();
                Admin accountcreatedAdmin = _adminRepo.GetAdminById(adminId);

                admin.Aspnetuserid = user.Id;
                admin.Firstname = data.FirstName;
                admin.Lastname = data.LastName;
                admin.Email = data.Email;
                admin.Mobile = data.PhoneNumber;
                admin.Address1 = data.Address1;
                admin.Address2 = data.Address2;
                admin.City = data.City;
                admin.Altphone = data.Altphone;
                admin.Createddate = DateTime.Now;
                admin.Createdby = accountcreatedAdmin.Aspnetuserid;
                admin.Status = 2;
                admin.Isdeleted = false;
                admin.Roleid = data.RoleId;
                _context.Admins.Add(admin);
                _context.SaveChanges();

                List<Adminregion> adminRegions = new List<Adminregion>();

                foreach (int regionId in regions)
                {
                    adminRegions.Add(new Adminregion
                    {
                        Regionid = regionId,
                        Adminid = admin.Adminid
                    });
                }
                _context.Adminregions.AddRange(adminRegions);
                _context.SaveChanges();

                transaction.Complete();
            }
        }

        #endregion

        #region Scheduling

        public bool HasExistingShifts(int physicianId, DateTime date, TimeOnly startTime, TimeOnly endTime)
        {
            bool hasShift = _context.Shiftdetails
                .Any(sd => sd.Shift.Physicianid == physicianId &&
                           sd.Shiftdate == date &&
                           ((sd.Starttime >= startTime && sd.Starttime < endTime) ||
                           (sd.Endtime > startTime && sd.Endtime <= endTime) ||
                           (sd.Starttime <= startTime && sd.Endtime >= endTime)));

            return hasShift;
        }

        public void CreateShift(ScheduleModel data, int? adminId, int? providerId)
        {

            using (var transaction = new TransactionScope())
            {
                Shift shift = new Shift();

                if (adminId != null) shift.Physicianid = data.Physicianid;
                if (providerId != null) shift.Physicianid = (int)providerId;

                shift.Repeatupto = data.Repeatupto;
                shift.Startdate = data.Startdate;

                if (adminId != null) shift.Createdby = _adminRepo.GetAdminById((int)adminId).Aspnetuserid;
                if (providerId != null) shift.Createdby = _context.Physicians.Where(p => p.Physicianid == (int)providerId).FirstOrDefault().Aspnetuserid;

                shift.Createddate = DateTime.Now;
                shift.Isrepeat = data.Isrepeat;
                shift.Repeatupto = data.Repeatupto;
                _context.Shifts.Add(shift);
                _context.SaveChanges();

                Shiftdetail sd = new Shiftdetail();
                sd.Shiftid = shift.Shiftid;
                sd.Shiftdate = new DateTime(data.Startdate.Year, data.Startdate.Month, data.Startdate.Day);
                sd.Starttime = data.Starttime;
                sd.Endtime = data.Endtime;
                sd.Regionid = data.Regionid;
                sd.Status = data.Status;
                sd.Isdeleted = false;
                _context.Shiftdetails.Add(sd);
                _context.SaveChanges();

                Shiftdetailregion sr = new Shiftdetailregion();
                sr.Shiftdetailid = sd.Shiftdetailid;
                sr.Regionid = data.Regionid;
                sr.Isdeleted = false;
                _context.Shiftdetailregions.Add(sr);
                _context.SaveChanges();

                if (data.Isrepeat == true && data.checkWeekday.Length > 0)
                {
                    List<int> day = data.checkWeekday.Split(',').Select(int.Parse).ToList();

                    foreach (int d in day)
                    {
                        DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                        DateTime nextOccurrence = new DateTime(data.Startdate.Year, data.Startdate.Month, data.Startdate.Day + 1);
                        int occurrencesFound = 0;
                        while (occurrencesFound < data.Repeatupto)
                        {
                            if (nextOccurrence.DayOfWeek == desiredDayOfWeek)
                            {

                                Shiftdetail sdd = new Shiftdetail();
                                sdd.Shiftid = shift.Shiftid;
                                sdd.Shiftdate = nextOccurrence;
                                sdd.Starttime = data.Starttime;
                                sdd.Endtime = data.Endtime;
                                sdd.Regionid = data.Regionid;
                                sdd.Status = data.Status;
                                sdd.Isdeleted = false;
                                _context.Shiftdetails.Add(sdd);
                                _context.SaveChanges();

                                Shiftdetailregion srr = new Shiftdetailregion();
                                srr.Shiftdetailid = sdd.Shiftdetailid;
                                srr.Regionid = data.Regionid;
                                srr.Isdeleted = false;
                                _context.Shiftdetailregions.Add(srr);
                                _context.SaveChanges();
                                occurrencesFound++;
                            }
                            nextOccurrence = nextOccurrence.AddDays(1);
                        }
                    }
                }

                transaction.Complete();
            }


        }

        public List<ScheduleModel> PhysicianAll()
        {

            List<ScheduleModel> ScheduleDetails = new List<ScheduleModel>();

            List<CreateProviderView> pl = (from r in _context.Physicians
                                           join Notifications in _context.Physiciannotifications
                                           on r.Physicianid equals Notifications.Physicianid into aspGroup
                                           from nof in aspGroup.DefaultIfEmpty()
                                           join role in _context.Roles
                                           on r.Roleid equals role.Roleid into roleGroup
                                           from roles in roleGroup.DefaultIfEmpty()
                                           where r.Isdeleted == false
                                           select new CreateProviderView
                                           {
                                               Createddate = r.Createddate,
                                               ProviderId = r.Physicianid,
                                               Address1 = r.Address1,
                                               Address2 = r.Address2,
                                               Adminnotes = r.Adminnotes,
                                               Altphone = r.Altphone,
                                               businessName = r.Businessname,
                                               businessWebsite = r.Businesswebsite,
                                               city = r.City,
                                               firstName = r.Firstname,
                                               lastName = r.Lastname,
                                               roleid = r.Roleid,
                                               regionName = _context.Regions.FirstOrDefault(r => r.Regionid == (int)r.Regionid).Name,
                                               Status = r.Status,
                                               email = r.Email,
                                               photo = r.Photo
                                           }).ToList();

            foreach (CreateProviderView schedule in pl)
            {
                List<ScheduleModel> ss = (from s in _context.Shifts
                                          join pd in _context.Physicians
                                          on s.Physicianid equals pd.Physicianid
                                          join sd in _context.Shiftdetails
                                          on s.Shiftid equals sd.Shiftid into shiftGroup
                                          from sd in shiftGroup.DefaultIfEmpty()
                                          join rg in _context.Regions
                                          on sd.Regionid equals rg.Regionid
                                          where s.Physicianid == schedule.ProviderId && sd.Isdeleted == false
                                          select new ScheduleModel
                                          {
                                              RegionName = rg.Name,
                                              Shiftid = sd.Shiftdetailid,
                                              Status = sd.Status,
                                              Starttime = sd.Starttime,
                                              Shiftdate = sd.Shiftdate,

                                              Endtime = sd.Endtime,
                                              PhysicianName = pd.Firstname + ' ' + pd.Lastname,

                                          }).ToList();

                ScheduleModel temp = new ScheduleModel();
                temp.PhysicianName = schedule.firstName + ' ' + schedule.lastName;
                temp.PhysicianPhoto = schedule.photo;
                temp.RegionName = schedule.regionName;
                temp.Physicianid = (int)schedule.ProviderId;
                temp.DayList = ss;
                ScheduleDetails.Add(temp);

            }

            return ScheduleDetails;


        }

        public List<ScheduleModel> PhysicianByRegion(int? region)
        {
            List<ScheduleModel> ScheduleDetails = new List<ScheduleModel>();
            List<CreateProviderView> pl = (
                                        from pr in _context.Physicianregions

                                        join ph in _context.Physicians
                                         on pr.Physicianid equals ph.Physicianid into rGroup
                                        from r in rGroup.DefaultIfEmpty()

                                        join Notifications in _context.Physiciannotifications
                                         on r.Physicianid equals Notifications.Physicianid into aspGroup
                                        from nof in aspGroup.DefaultIfEmpty()

                                        join role in _context.Roles
                                        on r.Roleid equals role.Roleid into roleGroup
                                        from roles in roleGroup.DefaultIfEmpty()

                                        where pr.Regionid == region && r.Isdeleted == false
                                        select new CreateProviderView
                                        {
                                            Createddate = r.Createddate,
                                            ProviderId = r.Physicianid,
                                            Address1 = r.Address1,
                                            Address2 = r.Address2,
                                            Adminnotes = r.Adminnotes,
                                            Altphone = r.Altphone,
                                            businessName = r.Businessname,
                                            businessWebsite = r.Businesswebsite,
                                            city = r.City,
                                            firstName = r.Firstname,
                                            lastName = r.Lastname,
                                            roleid = r.Roleid,
                                            regionName = _context.Regions.FirstOrDefault(r => r.Regionid == (int)r.Regionid).Name,
                                            Status = r.Status,
                                            email = r.Email,
                                            photo = r.Photo

                                        }).ToList();


            foreach (CreateProviderView schedule in pl)
            {
                List<ScheduleModel> ss = (from s in _context.Shifts
                                          join pd in _context.Physicians
                                          on s.Physicianid equals pd.Physicianid
                                          join sd in _context.Shiftdetails
                                          on s.Shiftid equals sd.Shiftid into shiftGroup
                                          from sd in shiftGroup.DefaultIfEmpty()
                                          join rg in _context.Regions
                                          on sd.Regionid equals rg.Regionid
                                          where s.Physicianid == schedule.ProviderId && sd.Isdeleted == false && sd.Regionid == region
                                          select new ScheduleModel
                                          {
                                              RegionName = rg.Abbreviation,
                                              Shiftid = sd.Shiftdetailid,
                                              Status = sd.Status,
                                              Starttime = sd.Starttime,
                                              Shiftdate = sd.Shiftdate,
                                              Endtime = sd.Endtime,
                                              PhysicianName = pd.Firstname + ' ' + pd.Lastname,
                                          }).ToList();

                ScheduleModel temp = new ScheduleModel();
                temp.PhysicianName = schedule.firstName + ' ' + schedule.lastName;
                temp.PhysicianPhoto = schedule.photo;
                temp.RegionName = schedule.regionName;
                temp.Physicianid = (int)schedule.ProviderId;
                temp.DayList = ss;
                ScheduleDetails.Add(temp);
            }

            return ScheduleDetails;

        }

        public List<ScheduleModel> GetShift(int month, int year, int? regionId, int? providerId)
        {
            List<ScheduleModel> ScheduleDetails = new List<ScheduleModel>();

            var uniqueDates = _context.Shiftdetails
                            .Where(sd => sd.Shiftdate.Month == month && sd.Shiftdate.Year == year && sd.Isdeleted == false && (regionId == null || regionId == 0 || sd.Regionid == regionId))
                            .Select(sd => sd.Shiftdate.Date) // Select the date part of Shiftdate
                            .Distinct() // Get distinct dates
                            .ToList();


            foreach (DateTime schedule in uniqueDates)
            {
                List<ScheduleModel> ss = (from s in _context.Shifts
                                          join pd in _context.Physicians
                                          on s.Physicianid equals pd.Physicianid
                                          join sd in _context.Shiftdetails
                                          on s.Shiftid equals sd.Shiftid into shiftGroup
                                          from sd in shiftGroup.DefaultIfEmpty()
                                          where sd.Shiftdate == schedule && sd.Isdeleted == false && (providerId == null || s.Physicianid == providerId)
                                          select new ScheduleModel
                                          {
                                              Shiftid = sd.Shiftdetailid,
                                              Status = sd.Status,
                                              Starttime = sd.Starttime,
                                              Endtime = sd.Endtime,
                                              PhysicianName = pd.Firstname + ' ' + pd.Lastname,
                                          }).ToList();

                ScheduleModel temp = new ScheduleModel();
                temp.Shiftdate = schedule;
                temp.DayList = ss;
                ScheduleDetails.Add(temp);
            }


            return ScheduleDetails;

        }

        public ScheduleModel GetShiftByShiftdetailId(int Shiftdetailid)
        {

            ScheduleModel shiftdata = (from s in _context.Shifts
                                       join pd in _context.Physicians
                                       on s.Physicianid equals pd.Physicianid
                                       join sd in _context.Shiftdetails
                                       on s.Shiftid equals sd.Shiftid into shiftGroup
                                       from sd in shiftGroup.DefaultIfEmpty()
                                       join rg in _context.Regions
                                       on sd.Regionid equals rg.Regionid
                                       where sd.Shiftdetailid == Shiftdetailid
                                       select new ScheduleModel
                                       {
                                           Regionid = (int)sd.Regionid,
                                           Shiftid = sd.Shiftdetailid,
                                           Status = sd.Status,
                                           Starttime = sd.Starttime,
                                           Endtime = sd.Endtime,
                                           Physicianid = s.Physicianid,
                                           PhysicianName = pd.Firstname + ' ' + pd.Lastname,
                                           Shiftdate = sd.Shiftdate
                                       })
                                          .FirstOrDefault();

            return shiftdata;
        }

        public void EditShift(ScheduleModel shift, int? adminId, int? providerId)
        {
            Shiftdetail sd = _context.Shiftdetails.FirstOrDefault(sd => sd.Shiftdetailid == shift.Shiftid);
            sd.Shiftdate = (DateTime)shift.Shiftdate;
            sd.Starttime = shift.Starttime;
            sd.Endtime = shift.Endtime;
            if (adminId != null) sd.Modifiedby = _adminRepo.GetAdminById((int)adminId).Aspnetuserid;
            if (providerId != null) sd.Modifiedby = _context.Physicians.Where(p => p.Physicianid == (int)providerId).FirstOrDefault().Aspnetuserid;

            sd.Modifieddate = DateTime.Now;
            _context.Shiftdetails.Update(sd);
            _context.SaveChanges();
        }

        public void Updateshiftstatus(int shiftId, int adminId)
        {
            Shiftdetail sd = _context.Shiftdetails.FirstOrDefault(sd => sd.Shiftdetailid == shiftId);
            var temp = sd.Status;
            sd.Status = (temp == 0) ? (short)1 : (short)0;
            var admin = _adminRepo.GetAdminById(adminId);
            sd.Modifiedby = admin?.Aspnetuserid;
            sd.Modifieddate = DateTime.Now;
            _context.Shiftdetails.Update(sd);
            _context.SaveChanges();

        }

        public void DeleteShift(int shiftId, int? adminId, int? providerId)
        {
            Shiftdetail sd = _context.Shiftdetails.FirstOrDefault(sd => sd.Shiftdetailid == shiftId);
            sd.Isdeleted = true;
            if (adminId != null) sd.Modifiedby = _adminRepo.GetAdminById((int)adminId).Aspnetuserid;
            if (providerId != null) sd.Modifiedby = _context.Physicians.Where(p => p.Physicianid == (int)providerId).FirstOrDefault().Aspnetuserid;
            sd.Modifieddate = DateTime.Now;
            _context.Shiftdetails.Update(sd);
            _context.SaveChanges();
        }
        #endregion

        #region Provider-on-call
        public List<CreateProviderView> PhysicianOnCall(int? region)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeOnly currentTimeOfDay = TimeOnly.FromDateTime(DateTime.Now);
            List<CreateProviderView> providerList = (from r in _context.Physicians
                                                     where r.Isdeleted == false && (region == null || region == 0 || r.Regionid == region)
                                                     select new CreateProviderView
                                                     {
                                                         Createddate = r.Createddate,
                                                         ProviderId = r.Physicianid,
                                                         Address1 = r.Address1,
                                                         Address2 = r.Address2,
                                                         Adminnotes = r.Adminnotes,
                                                         Altphone = r.Altphone,
                                                         businessName = r.Businessname,
                                                         businessWebsite = r.Businesswebsite,
                                                         city = r.City,
                                                         firstName = r.Firstname,
                                                         lastName = r.Lastname,
                                                         Status = r.Status,
                                                         email = r.Email,
                                                         photo = r.Photo
                                                     }).ToList();

            foreach (var item in providerList)
            {
                List<int> shiftIds = (from s in _context.Shifts
                                      where s.Physicianid == item.ProviderId
                                      select s.Shiftid).ToList();

                foreach (var shift in shiftIds)
                {
                    var shiftDetail = (from sd in _context.Shiftdetails
                                       where sd.Shiftid == shift &&
                                             sd.Shiftdate.Date == currentDateTime.Date &&
                                             sd.Starttime <= currentTimeOfDay &&
                                             currentTimeOfDay <= sd.Endtime
                                       select sd).FirstOrDefault();

                    if (shiftDetail != null)
                    {
                        item.onCallStatus = 1;
                    }
                }
            }

            return providerList;


        }
        #endregion

        #region ShiftReview 
        public List<ScheduleModel> GetAllNotApprovedShift(int? regionId, int? month)
        {

            List<ScheduleModel> shiftList = (from s in _context.Shifts
                                             join pd in _context.Physicians
                                             on s.Physicianid equals pd.Physicianid
                                             join sd in _context.Shiftdetails
                                             on s.Shiftid equals sd.Shiftid into shiftGroup
                                             from sd in shiftGroup.DefaultIfEmpty()
                                             join rg in _context.Regions
                                             on sd.Regionid equals rg.Regionid
                                             where (regionId == null || regionId == 0 || sd.Regionid == regionId) && sd.Status == 0 && sd.Isdeleted == false && (month == null || month == 0 || sd.Shiftdate.Month == month)
                                             select new ScheduleModel
                                             {
                                                 Regionid = (int)sd.Regionid,
                                                 RegionName = rg.Name,
                                                 Shiftid = sd.Shiftdetailid,
                                                 Status = sd.Status,
                                                 Starttime = sd.Starttime,
                                                 Endtime = sd.Endtime,
                                                 Physicianid = s.Physicianid,
                                                 PhysicianName = pd.Firstname + ' ' + pd.Lastname,
                                                 Shiftdate = sd.Shiftdate
                                             }).ToList();

            return shiftList;

        }
        #endregion

        #region Records

        public List<SearchRecordView> GetSearchRecords(string? Email, DateTime? FromDoS, string? Phone, string? Patient, string? Provider, int RequestStatus, int RequestType, DateTime? ToDoS)
        {
            var data = (from r in _context.Requests
                        join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                        join p in _context.Physicians on r.Physicianid equals p.Physicianid into prJoin
                        from p in prJoin.DefaultIfEmpty()
                        join rn in _context.Requestnotes on r.Requestid equals rn.Requestid into rrnJoin
                        from rn in rrnJoin.DefaultIfEmpty()
                        where r.Isdeleted == false
                        select new
                        {
                            Request = r,
                            RequestClient = rc,
                            Physician = p,
                            RequestNote = rn
                        }).ToList();

            var result = data.Select(item => new SearchRecordView
            {
                RequestId = item.Request.Requestid,
                PatientName = $"{item.RequestClient.Firstname} {item.RequestClient.Lastname}",
                Requestor = $"{item.Request.Firstname} {item.Request.Lastname}",
                DateOfService = item.Request.Accepteddate,
                ServiceDate = item.Request.Accepteddate?.ToString("MMMM dd, yyyy"),
                DateofClose = GetCloseDate(item.Request.Requestid)?.ToString("MMMM dd, yyyy") ?? "",
                CloseDate = GetCloseDate(item.Request.Requestid),
                Email = item.RequestClient.Email,
                PhoneNumber = item.RequestClient.Phonenumber,
                Address = item.RequestClient.Street + " " + item.RequestClient.City + "," + item.RequestClient.State + " (" + item.RequestClient.Zipcode + " )",
                Zip = item.RequestClient.Zipcode,
                RequestStatus = item.Request.Status,
                PhysicianName = item.Physician != null ? $"{item.Physician.Firstname} {item.Physician.Lastname}" : "", // Handle null Physician
                PhysicianNote = item.RequestNote?.Physiciannotes,
                CancelledByProvidor = GetPatientCancellationNotes(item.Request.Requestid),
                PatientNote = item.RequestClient.Notes,
                RequestTypeId = item.Request.Requesttypeid,
                AdminNotes = item.RequestNote?.Adminnotes
            }).ToList();


            result = result.Where(item =>
                (string.IsNullOrEmpty(Email) || item.Email.Contains(Email)) &&
                (string.IsNullOrEmpty(Phone) || item.PhoneNumber.Contains(Phone)) &&
                (string.IsNullOrEmpty(Patient) || item.PatientName.ToLower().Contains(Patient)) &&
                (string.IsNullOrEmpty(Provider) || item.PhysicianName.ToLower().Contains(Provider)) &&
                (RequestStatus == 0 || item.RequestStatus == RequestStatus) &&
                (RequestType == 0 || item.RequestTypeId == RequestType) &&
                (FromDoS == null || item.DateOfService?.Date >= FromDoS.Value.Date) &&
                (ToDoS == null || item.DateOfService?.Date <= ToDoS.Value.Date)
            ).ToList();

            return result;
        }

        public DateTime? GetConcludeDate(int requestid)
        {
            Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 6 && x.Physicianid != null);
            return log?.Createddate;
        }

        public DateTime? GetCloseDate(int requestid)
        {
            Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 9);
            return log?.Createddate;
        }
        public string? GetPatientCancellationNotes(int requestid)
        {
            Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 3 && x.Physicianid != null);
            return log?.Notes;
        }

        public bool DeletePatientRequest(int requestid)
        {
            Request? request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            if (request != null)
            {
                try
                {
                    request.Isdeleted = true;
                    request.Modifieddate = DateTime.Now;
                    _context.Requests.Update(request);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to submit Form", ex);
                }
            }
            else
            {
                return false;
            }
        }



        public List<BlockHistoryView> GetBlockHistoryData(string? name, DateTime? date, string? email, string? phoneNumber)
        {
            List<BlockHistoryView> data = new List<BlockHistoryView>();

            IQueryable<Blockrequest> query = _context.Blockrequests.Where(br => br.Isactive == true);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(br => _context.Requests.Any(r => r.Requestid.ToString() == br.Requestid && EF.Functions.Like((r.Firstname + " " + r.Lastname).ToLower(), $"%{name.ToLower()}%")));
            }

            if (date != null)
            {
                query = query.Where(br => br.Createddate.Value.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(br => br.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                query = query.Where(br => br.Phonenumber.Contains(phoneNumber));
            }

            List<Blockrequest> blockrequests = query.ToList();

            foreach (var br in blockrequests)
            {
                BlockHistoryView singleRequest = new BlockHistoryView();
                Request rq = _context.Requests.Where(r => r.Requestid == int.Parse(br.Requestid)).FirstOrDefault();
                singleRequest.requestId = Int32.Parse(br.Requestid);
                singleRequest.blockRequestId = br.Blockrequestid;
                singleRequest.Email = br.Email;
                singleRequest.PhoneNumber = br.Phonenumber;
                singleRequest.patientName = rq.Firstname + " " + rq.Lastname;
                singleRequest.isActive = br.Isactive;
                singleRequest.Notes = br.Reason;
                singleRequest.createdDate = (DateTime)br.Createddate;
                data.Add(singleRequest);
            }

            return data;
        }

        public void unBlock(int blockrequestId, int requestId)
        {
            using (var transaction = new TransactionScope())
            {

                Blockrequest blockRequest = _context.Blockrequests.Where(br => br.Blockrequestid == blockrequestId).FirstOrDefault();
                if (blockRequest != null)
                {
                    blockRequest.Isactive = false;
                }
                _context.Blockrequests.Update(blockRequest);
                _context.SaveChanges();
                Request request = _requestRepository.Get(requestId);
                request.Status = 1;
                _requestRepository.Update(request);
                transaction.Complete();
            }

        }

        public List<LogView> GetEmailLogs(int? accountType, string? receiverName, string? emailId, DateTime? createdDate, DateTime? sentDate)
        {
            List<LogView> emaillogs = new List<LogView>();

            IQueryable<Emaillog> query = _context.Emaillogs;

            if (accountType != null && accountType != 0)
            {
                query = query.Where(el => el.Roleid == accountType);
            }

            if (!string.IsNullOrEmpty(receiverName))
            {
                query = query.Where(el => EF.Functions.Like(el.Receivername.ToLower(), $"%{receiverName.ToLower()}%"));
            }

            if (!string.IsNullOrEmpty(emailId))
            {
                query = query.Where(el => el.Emailid.Contains(emailId));
            }

            if (createdDate != null)
            {
                query = query.Where(el => el.Createdate.Date == createdDate.Value.Date);
            }

            if (sentDate != null)
            {
                query = query.Where(el => el.Sentdate.Value.Date == sentDate.Value.Date);
            }

            // Retrieve the filtered email logs
            List<Emaillog> logs = query.ToList();

            // Transform email logs into LogView objects
            foreach (var item in logs)
            {
                LogView emaillog = new LogView();
                emaillog.Action = item.Subjectname;
                emaillog.emailId = item.Emailid;
                emaillog.createdDate = item.Createdate;
                emaillog.sentDate = (DateTime)item.Sentdate;
                emaillog.sentTries = (int)item.Senttries;
                emaillog.isSent = (bool)item.Isemailsent;
                emaillog.roleName = _context.Aspnetroles.FirstOrDefault(r => r.Id == item.Roleid)?.Name;
                emaillog.confirmationNumber = item.Confirmationnumber;
                if (item.Roleid == 1 && item.Physicianid != null)
                {
                    Physician ph = _context.Physicians.FirstOrDefault(ph => ph.Physicianid == item.Physicianid);
                    item.Receivername = ph?.Firstname + " , " + ph?.Lastname;
                }
                if (!string.IsNullOrEmpty(item.Receivername))
                {
                    emaillog.recipientName = item.Receivername;
                }
                emaillogs.Add(emaillog);
            }

            return emaillogs;
        }

        public List<LogView> GetSMSLogs(int? accountType, string? receiverName, string? phoneNumber, DateTime? createdDate, DateTime? sentDate)
        {
            List<LogView> smslogs = new List<LogView>();

            IQueryable<Smslog> query = _context.Smslogs;

            if (accountType != null && accountType != 0)
            {
                query = query.Where(el => el.Roleid == accountType);
            }

            if (!string.IsNullOrEmpty(receiverName))
            {
                query = query.Where(el => EF.Functions.Like(el.Receivername.ToLower(), $"%{receiverName.ToLower()}%"));
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                query = query.Where(el => el.Mobilenumber.Contains(phoneNumber));
            }

            if (createdDate != null)
            {
                query = query.Where(el => el.Createdate.Date == createdDate.Value.Date);
            }

            if (sentDate != null)
            {
                query = query.Where(el => el.Sentdate.Value.Date == sentDate.Value.Date);
            }

            // Retrieve the filtered email logs
            List<Smslog> logs = query.ToList();

            // Transform email logs into LogView objects
            foreach (var item in logs)
            {
                LogView smslog = new LogView();
                //smslog.Action = item.Action;
                smslog.phoneNumber = item.Mobilenumber;
                smslog.createdDate = item.Createdate;
                smslog.sentDate = (DateTime)item.Sentdate;
                smslog.sentTries = (int)item.Senttries;
                smslog.isSent = (bool)item.Issmssent;
                smslog.roleName = _context.Aspnetroles.FirstOrDefault(r => r.Id == item.Roleid)?.Name;
                smslog.confirmationNumber = item.Confirmationnumber;
                if (item.Roleid == 1 && item.Physicianid != null)
                {
                    Physician ph = _context.Physicians.FirstOrDefault(ph => ph.Physicianid == item.Physicianid);
                    item.Receivername = ph?.Firstname + " , " + ph?.Lastname;
                }
                if (!string.IsNullOrEmpty(item.Receivername))
                {
                    smslog.recipientName = item.Receivername;
                }
                smslogs.Add(smslog);
            }

            return smslogs;
        }



        #endregion

        public List<Physician> GetPhysiciansByRegion(int regionId)
        {
            var physicians = (from pr in _context.Physicianregions
                              join p in _context.Physicians on pr.Physicianid equals p.Physicianid
                              where pr.Regionid == regionId
                              select p).ToList();
            return physicians;
        }

        public List<Physicianlocation> GetPhysicianlocations()
        {
            return _context.Physicianlocations.ToList();
        }

        public List<Healthprofessional> getAllVendors()
        {
            return _context.Healthprofessionals.Where(vendor => vendor.Isdeleted == false).ToList();
        }
        public string GetProfessionNameById(int id)
        {
            return _context.Healthprofessionaltypes.FirstOrDefault(pt => pt.Healthprofessionalid == id).Professionname;
        }

        public string GetAccountTypeNameById(int id)
        {
            return _context.Aspnetroles.FirstOrDefault(r => r.Id == id).Name;
        }

        public List<Region> GetAllReagion()
        {
            return _context.Regions.ToList();
        }
        public List<Region> getProvidersRegion(int providerId)
        {
            List<int> regionIds = _context.Physicianregions.Where(pr => pr.Physicianid == providerId).Select(pr => pr.Regionid).ToList();
            List<Region> regions = new List<Region>();
            foreach (var id in regionIds)
            {
                Region rj = _context.Regions.FirstOrDefault(r => r.Regionid == id);
                regions.Add(rj);
            }
            return regions;
        }
        public List<Casetag> GetAllCaseTag()
        {
            return _context.Casetags.ToList();
        }
        public List<Role> GetAllRole()
        {
            return _context.Roles.Where(r => r.Isdeleted == false).ToList();
        }

        public List<Menu> GetAllMenu()
        {
            return _context.Menus.ToList();
        }
        public List<Aspnetrole> getAllRoleType()
        {
            return _context.Aspnetroles.ToList();
        }
        public List<Rolemenu> GetMenuByRole(int roleID)
        {
            return _context.Rolemenus.Where(rm => rm.Roleid == roleID).ToList();
        }
        public string GetMenuNameById(int menuid)
        {
            return _context.Menus.Where(m => m.Menuid == menuid).First().Name;
        }


        /*Proviser Transfer Request*/
        public void TransferRequestRequest(int requestId, string reason, int providerId)
        {
            var email = "shlok.jadeja@etatvasoft.com";
            var title = "Request:- " + requestId + " Transfer request";
            var message = "I want to transfer the request the reason is :- " + reason;
            bool isSent = SendEmail(email, title, message, null);
            _commonFunctionrepo.EmailLog(email, message, "Sent Request tranfer request", "Shlok Jadeja", 1, requestId, 4, providerId, 6, isSent, 1);
            _commonFunctionrepo.AddRequestStatusLog(requestId, 2, reason, 4, providerId, true);

        }
    }
}
