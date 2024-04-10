using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Records
{
    public class SearchRecordView
    {
        public int RequestId { get; set; }
        public string? PatientName { get; set; }
        public string? Requestor { get; set; }
        public int RequestTypeId { get; set; }
        public string? RequestTypeName { get; set; }
        public DateTime? CloseDate { get; set; }
        public string? DateofClose { get; set; }
        public DateTime? DateOfService { get; set; }
        public string? ServiceDate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Zip { get; set; }
        public int RequestStatus { get; set; }
        public string? StatusName
        {
            get
            {
                switch (RequestStatus)
                {
                    case 1:
                        return "Unassigned";
                    case 2:
                        return "Accepted";
                    case 3:
                        return "Cancelled";
                    case 4:
                        return "MDEnRoute";
                    case 5:
                        return "MDONSite";
                    case 6:
                        return "Conclude";
                    case 7:
                        return "Cancelled";
                    case 8:
                        return "CancelledByPatient";
                    case 9:
                        return "Closed";
                    case 10:
                        return "Unpaid";
                    case 11:
                        return "Blocked";
                    default:
                        return "";
                }
            }
        }
        public string? PhysicianName { get; set; }
        public string? PhysicianNote { get; set; }
        public string? CancelledByProvidor { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNote { get; set; }
    }
}


