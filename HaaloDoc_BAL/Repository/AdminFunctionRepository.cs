
using HalloDoc_BAL.ViewModel.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repository
{
    public class AdminFunctionRepository : IAdminFunctionRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminFunctionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<RequestDataTableView> GetRequestsByStatusID(int statusId)
        {
            var statusIds = GetStatus(statusId);
            var statusIdWiseRequest = from r in _context.Requests.ToList()
                                      join rc in _context.Requestclients.ToList()
                                      on r.Requestid equals rc.Requestid into rrc
                                      from rc in rrc.DefaultIfEmpty()
                                      where statusIds.Contains(r.Status)
                                      select new RequestDataTableView
                                      {
                                          requestId = r.Requestid,
                                          PatientName = rc.Firstname + " " + rc.Lastname,
                                          PatientEmail = rc.Email,
                                          RequesterEmail = r.Email,
                                          DateOfBirth = rc.Intyear.Value.ToString("") + "-" + rc.Strmonth + "-" + string.Format("{0:00}", rc.Intdate.Value),
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
                    return new int[] { 4, 5};
                case 4:
                    return new int[] { 6 };
                case 5:
                    return new int[] { 3, 7 , 8 };
                case 6:
                    return new int[] { 9 };
                default:
                    return new int[] { };
            }
        }


        public ViewCase GetViewCase(int requestId)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requestId);
            var requestClient = _context.Requestclients.FirstOrDefault(r => r.Requestid == requestId);

            var view = new ViewCase
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
                requesttypeId = request.Requesttypeid
            };

            return view;
        }


        public List<Region> GetAllReagion()
        {
            return _context.Regions.ToList();

        }


    }
}
