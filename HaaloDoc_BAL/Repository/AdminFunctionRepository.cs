using HalloDoc_Admin.Models;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return new int[] { 1, 2 };
                case 2:
                    return new int[] { 4, 14, 15};
                case 3:
                    return new int[] { 5,6,7};
                case 4:
                    return new int[] { 8, 9, 10, 11 };
                case 5:
                    return new int[] { 3, 13, 14, 15 };
                case 6:
                    return new int[] { 12 };
                default:
                    return new int[] { };
            }
        }


       

    }
}
