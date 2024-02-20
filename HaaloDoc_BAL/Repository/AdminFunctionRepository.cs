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
            var statusIdWiseRequest = from r in _context.Requests.ToList()
                                      join rc in _context.Requestclients.ToList()
                                      on r.Requestid equals rc.Requestid into rrc
                                      from rc in rrc.DefaultIfEmpty()
                                      where r.Status == statusId
                                      select new RequestDataTableView
                                      {
                                          requestId = r.Requestid,
                                          PatientName = rc.Firstname + " " + rc.Lastname,
                                          PatientPhoneNumber = rc.Phonenumber,
                                          DateOfBirth = rc.Intyear.Value.ToString("") + "-" + rc.Strmonth + "-" + string.Format("{0:00}", rc.Intdate.Value),
                                          RequesterName = r.Firstname + " " + r.Lastname,
                                          RequestedDate = r.Createddate.ToString(),
                                          Address = rc.Street + " " + rc.City + " " + rc.State + ",(" + rc.Zipcode + ")",
                                          status = r.Status,
                                          MenuOptions = r.Status == 1 ? new List<MenuOptionEnum> { MenuOptionEnum.blockRequest, MenuOptionEnum.viewRequest } : new List<MenuOptionEnum> { MenuOptionEnum.blockRequest }
                                      };

            return statusIdWiseRequest;
        }

        public IEnumerable<RequestDataTableView> GetRequestByType(int requestTypeid , int statusId)
        {
            var statusIdWiseRequest = from r in _context.Requests.ToList()
                                      join rc in _context.Requestclients.ToList()
                                      on r.Requestid equals rc.Requestid into rrc
                                      from rc in rrc.DefaultIfEmpty()
                                      where r.Requesttypeid == requestTypeid && r.Status == statusId
                                      select new RequestDataTableView
                                      {
                                          requestId = r.Requestid,
                                          PatientName = rc.Firstname + " " + rc.Lastname,
                                          PatientPhoneNumber = rc.Phonenumber,
                                          DateOfBirth = rc.Intyear.Value.ToString("") + "-" + rc.Strmonth + "-" + string.Format("{0:00}", rc.Intdate.Value),
                                          RequesterName = r.Firstname + " " + r.Lastname,
                                          RequestedDate = r.Createddate.ToString(),
                                          Address = rc.Street + " " + rc.City + " " + rc.State + ",(" + rc.Zipcode + ")",
                                          status = r.Status,
                                          MenuOptions = r.Status == 1 ? new List<MenuOptionEnum> { MenuOptionEnum.blockRequest, MenuOptionEnum.viewRequest } : new List<MenuOptionEnum> { MenuOptionEnum.blockRequest }
                                      };

            return statusIdWiseRequest;
        }
    }
}
